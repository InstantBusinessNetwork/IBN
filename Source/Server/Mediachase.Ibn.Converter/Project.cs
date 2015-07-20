using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Mediachase.Database;


namespace Mediachase.Ibn.Converter
{
	class Project
	{
		private const int ProjectTTBlockTypeId = 2;

		private int _projectId;
		private string _title;
		private int _statusId;
		private int _managerId;
		private int? _executiveManagerId;
		private List<int> _sponsors = new List<int>();
		private List<int> _stakeholders = new List<int>();
		private List<int> _teamMembers = new List<int>();

		private int _blockTypeInstanceId;

		public int ProjectId { get { return _projectId; } }
		public string Title { get { return _title; } }
		public int StatusId { get { return _statusId; } }
		public int ManagerId { get { return _managerId; } }
		public int? ExecutiveManagerId { get { return _executiveManagerId; } }
		public IList<int> Sponsors { get { return _sponsors; } }
		public IList<int> Stakeholders { get { return _stakeholders; } }
		public IList<int> TeamMembers { get { return _teamMembers; } }
		public int BlockTypeInstanceId { get { return _blockTypeInstanceId; } }

		#region public LoadList()
		public static IDictionary<int, Project> LoadList(DBHelper source)
		{
			Dictionary<int, Project> ret = new Dictionary<int, Project>();

			using (IDataReader reader = source.RunTextDataReader("SELECT [ProjectId],[Title],[StatusId],[ManagerId],[ExecutiveManagerId] FROM [PROJECTS]"))
			{
				while (reader.Read())
				{
					Project item = Load(reader);
					if (!ret.ContainsKey(item._projectId))
						ret.Add(item._projectId, item);
				}
			}

			foreach (Project project in ret.Values)
			{
				using (IDataReader reader = source.RunTextDataReader("SELECT [PrincipalId],[IsTeamMember],[IsSponsor],[IsStakeHolder] FROM [PROJECT_MEMBERS] WHERE [ProjectId]=@p1", DBHelper.MP("@p1", SqlDbType.Int, project._projectId)))
				{
					while (reader.Read())
					{
						int principalId = (int)reader["PrincipalId"];

						if ((bool)reader["IsTeamMember"])
							project._teamMembers.Add(principalId);

						if ((bool)reader["IsSponsor"])
							project._sponsors.Add(principalId);

						if ((bool)reader["IsStakeHolder"])
							project._stakeholders.Add(principalId);
					}
				}
			}

			return ret;
		}
		#endregion

		#region public Load(IDataRecord reader)
		public static Project Load(IDataRecord reader)
		{
			Project ret = new Project();

			ret._projectId = (int)reader["ProjectId"];
			ret._title = reader["Title"].ToString();
			ret._statusId = (int)reader["StatusId"];
			ret._managerId = (int)reader["ManagerId"];
			ret._executiveManagerId = Helper.NullToNulableInt32(reader["ExecutiveManagerId"]);

			return ret;
		}
		#endregion

		#region public void Save(DBHelper target)
		public void Save(DBHelper target)
		{
			target.RunText(
				"INSERT INTO [cls_Project] ([ProjectId],[Title],[StatusId]) VALUES (@p1,@p2,@p3)"
			, DBHelper.MP("@p1", SqlDbType.Int, _projectId)
			, DBHelper.MP("@p2", SqlDbType.NVarChar, 100, _title)
			, DBHelper.MP("@p3", SqlDbType.Int, _statusId)
			);
			SaveProjectRoles(target, "cls_Project_Role_Principal", _projectId);

			_blockTypeInstanceId = target.RunTextInteger(
				"INSERT INTO [cls_TimeTrackingBlockTypeInstance] ([Title],[BlockTypeId],[ProjectId]) VALUES (@p1,@p2,@p3) SELECT @retval = SCOPE_IDENTITY()"
			, DBHelper.MP("@p1", SqlDbType.NVarChar, 100, _title)
			, DBHelper.MP("@p2", SqlDbType.Int, ProjectTTBlockTypeId)
			, DBHelper.MP("@p3", SqlDbType.Int, _projectId)
			);
			SaveProjectRoles(target, "cls_TimeTrackingBlockTypeInstance_Role_Principal", _blockTypeInstanceId);
		}
		#endregion

		#region public void SaveProjectRoles(DBHelper target, string tableName, int objectId)
		public void SaveProjectRoles(DBHelper target, string tableName, int objectId)
		{
			Helper.SaveRolePrincipals(target, tableName, objectId, 1, _managerId); // ProjectManager
			if (_executiveManagerId != null)
				Helper.SaveRolePrincipals(target, tableName, objectId, 2, _executiveManagerId.Value); // ExecutiveManager
			Helper.SaveRolePrincipals(target, tableName, objectId, 3, _sponsors); // ProjectSponsor
			Helper.SaveRolePrincipals(target, tableName, objectId, 4, _stakeholders); // ProjectStakeholder
			Helper.SaveRolePrincipals(target, tableName, objectId, 5, _teamMembers); // ProjectTeamMember
		}
		#endregion
	}
}
