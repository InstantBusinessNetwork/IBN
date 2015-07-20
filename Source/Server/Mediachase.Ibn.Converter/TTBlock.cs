using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Mediachase.Database;


namespace Mediachase.Ibn.Converter
{
	internal class TTBlock
	{
		private const int TTBlockStateMachineId = 2; // ProjectStateMachine

		private const int TTBlockStateInitial = 1;
		private const int TTBlockStateFinal = 2;
		private const int TTBlockStatePMApproving = 4;

		private const int NonProjectTTBlockTypeInstanceId = 1;

		// 4.5 fields
		private int _weekTimeSheetId;
		private int _userId;
		private int _projectId;
		private DateTime _startDate;
		private int _statusId;
		private string _managerComments;
		private DateTime _lastSavedDate;
		private int _lastEditorId;

		// 4.6 fields
		private int _blockId;
		private string _title;
		private int _blockTypeInstanceId;
		private bool _areFinancesRegistered;
		private bool _isRejected;

		public int ProjectId
		{
			get { return _projectId; }
		}

		public string Title
		{
			set { _title = value; }
		}

		public int BlockId
		{
			get { return _blockId; }
		}

		public int BlockTypeInstanceId
		{
			get { return _blockTypeInstanceId; }
			set { _blockTypeInstanceId = value; }
		}

		public int OwnerId
		{
			get { return _userId; }
		}

		#region public LoadList()
		public static IDictionary<int, TTBlock> LoadList(DBHelper source)
		{
			Dictionary<int, TTBlock> ret = new Dictionary<int,TTBlock>();
			using (IDataReader reader = source.RunTextDataReader("SELECT T.*, P.Title, S.ProjectSpreadSheetId FROM [WeekTimeSheet] T LEFT JOIN [PROJECTS] P ON P.ProjectId = T.ProjectId LEFT JOIN [ProjectSpreadSheet] S ON S.ProjectId = T.ProjectId"))
			{
				while (reader.Read())
				{
					TTBlock item = Load(reader);
					if (!ret.ContainsKey(item._weekTimeSheetId))
						ret.Add(item._weekTimeSheetId, item);
				}
			}
			return ret;
		}
		#endregion

		#region public Load(IDataRecord reader)
		public static TTBlock Load(IDataRecord reader)
		{
			TTBlock ret = new TTBlock();

			ret._weekTimeSheetId = (int)reader["WeekTimeSheetId"];
			ret._userId = (int)reader["UserId"];
			ret._projectId = Helper.NullToInt32(reader["ProjectId"]);
			ret._startDate = (DateTime)reader["StartDate"];
			ret._statusId = (int)reader["StatusId"];
			ret._managerComments = reader["ManagerComments"].ToString();
			ret._lastSavedDate = (DateTime)reader["LastSavedDate"];
			ret._lastEditorId = (int)reader["LastEditorId"];

			if(ret._projectId > 0)
				ret._title = reader["Title"].ToString();

			if (Helper.NullToInt32(reader["ProjectSpreadSheetId"]) > 0)
				ret._areFinancesRegistered = true;

			return ret;
		}
		#endregion

		#region public Save(DBHelper target)
		public void Save(DBHelper target, Project project)
		{
			_blockTypeInstanceId = project != null ? project.BlockTypeInstanceId : NonProjectTTBlockTypeInstanceId;

			_blockId = target.RunTextInteger("INSERT INTO [cls_TimeTrackingBlock] ([Modified],[ModifierId],[Card],[Title],[BlockTypeInstanceId],[Day1],[Day2],[Day3],[Day4],[Day5],[Day6],[Day7],[OwnerId],[mc_StateId],[mc_StateMachineId],[StartDate],[ProjectId],[AreFinancesRegistered],[IsRejected]) VALUES (@p1,@p2,@p3,@p5,@p6,@p7,@p7,@p7,@p7,@p7,@p7,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14) SELECT @retval = SCOPE_IDENTITY()"
				, DBHelper.MP("@p1", SqlDbType.DateTime, _lastSavedDate)
				, DBHelper.MP("@p2", SqlDbType.Int, _lastEditorId)
				, DBHelper.MP("@p3", SqlDbType.NVarChar, 50, "TimeTrackingBlockDescr")
				, DBHelper.MP("@p5", SqlDbType.NVarChar, 100, _title)
				, DBHelper.MP("@p6", SqlDbType.Int, _blockTypeInstanceId)
				, DBHelper.MP("@p7", SqlDbType.Float, 0F)
				, DBHelper.MP("@p8", SqlDbType.Int, _userId)

				, DBHelper.MP("@p9", SqlDbType.Int, ConvertState(_statusId, _projectId > 0, ref _isRejected))
				, DBHelper.MP("@p10", SqlDbType.Int, TTBlockStateMachineId)

				, DBHelper.MP("@p11", SqlDbType.DateTime, _startDate)
				, DBHelper.MP("@p12", SqlDbType.Int, _projectId > 0 ? (object)_projectId : DBNull.Value)
				, DBHelper.MP("@p13", SqlDbType.Bit, _areFinancesRegistered)
				, DBHelper.MP("@p14", SqlDbType.Bit, _isRejected)
				);

			if (!string.IsNullOrEmpty(_managerComments))
			{
				target.RunText("INSERT INTO [cls_TimeTrackingBlockDescr] ([TimeTrackingBlockId],[Description]) VALUES (@p1,@p2)"
					, DBHelper.MP("@p1", SqlDbType.Int, _blockId)
					, DBHelper.MP("@p2", SqlDbType.NText, _managerComments)
					);
			}

			// Save role principals
			Helper.SaveRolePrincipals(target, "cls_TimeTrackingBlock_Role_Principal", _blockId, 6, _userId); // Owner
			if (project != null)
				project.SaveProjectRoles(target, "cls_TimeTrackingBlock_Role_Principal", _blockId);
		}
		#endregion

		#region private static int ConvertState(int state45, bool isProject)
		private static int ConvertState(int state45, bool isProject, ref bool isRejected)
		{
			int ret = TTBlockStateInitial;
			isRejected = false;

			if (isProject)
			{
				if (state45 == 2) // Pending
					ret = TTBlockStatePMApproving;
				if (state45 == 3) // Denied
					isRejected = true;
				if (state45 == 4) // Approved
					ret = TTBlockStateFinal;
			}

			return ret;
		}
		#endregion

		#region private static void SaveRolePrincipals(DBHelper target, int objectId, int roleId, int principalId)
		private static void SaveRolePrincipals(DBHelper target, int objectId, int roleId, int principalId)
		{
			SaveRolePrincipals(target, objectId, roleId, new int[] { principalId });
		}
		#endregion

		#region private static void SaveRolePrincipals(DBHelper target, int objectId, int roleId, IEnumerable<int> principals)
		private static void SaveRolePrincipals(DBHelper target, int objectId, int roleId, IEnumerable<int> principals)
		{
			foreach (int principalId in principals)
			{
				target.RunText("INSERT INTO [cls_TimeTrackingBlock_Role_Principal] ([ObjectId],[PrincipalId],[RoleId]) VALUES (@p1,@p2,@p3)"
					, DBHelper.MP("@p1", SqlDbType.Int, objectId)
					, DBHelper.MP("@p2", SqlDbType.Int, principalId)
					, DBHelper.MP("@p3", SqlDbType.Int, roleId)
					);
			}
		}
		#endregion
	}
}
