using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

using Mediachase.Database;


namespace Mediachase.Ibn.Converter
{
	internal class TTEntry
	{
		// 4.5 fields
		private int _timeSheetId;
		private int _weekTimeSheetId;
		private int _objectTypeId;
		private int _objectId;
		private int _day1;
		private int _day2;
		private int _day3;
		private int _day4;
		private int _day5;
		private int _day6;
		private int _day7;
		private int _totalApproved;
		private decimal _rate;
		private string _userComments;
		private int _actualId;

		// 4.6 fields
		private int _entryId;
		private string _title = string.Empty;
		private int _blockTypeInstanceId;
		private int _ownerId;
		private int _parentBlockId;

		public int WeekTimeSheetId
		{
			get { return _weekTimeSheetId; }
		}

		public int BlockTypeInstanceId
		{
			set { _blockTypeInstanceId = value; }
		}

		public int OwnerId
		{
			set { _ownerId = value; }
		}

		public int ParentBlockId
		{
			set { _parentBlockId = value; }
		}

		#region public LoadList()
		public static IDictionary<int, TTEntry> LoadList(DBHelper source)
		{
			Dictionary<int, TTEntry> ret = new Dictionary<int, TTEntry>();
			using (IDataReader reader = source.RunTextDataReader("SELECT * FROM [TIMESHEETS]"))
			{
				while (reader.Read())
				{
					TTEntry item = Load(reader);
					if (!ret.ContainsKey(item._timeSheetId))
						ret.Add(item._timeSheetId, item);
				}
			}

			foreach (TTEntry entry in ret.Values)
			{
				string table = null;
				string field = null;

				switch (entry._objectTypeId)
				{
					case 3:
						table = "PROJECTS";
						field = "ProjectId";
						break;
					case 4:
						table = "EVENTS";
						field = "EventId";
						break;
					case 5:
						table = "TASKS";
						field = "TaskId";
						break;
					case 6:
						table = "TODO";
						field = "ToDoId";
						break;
					case 7:
						table = "INCIDENTS";
						field = "IncidentId";
						break;
					case 11:
						table = "TIMESHEET_TODO";
						field = "TimeSheetToDoId";
						break;
					case 16:
						table = "DOCUMENTS";
						field = "DocumentId";
						break;
				}

				if (table != null && field != null)
				{
					object title = source.RunTextScalar(string.Format(CultureInfo.InvariantCulture, "SELECT [Title] FROM [{0}] WHERE [{1}] = @p1", table, field), DBHelper.MP("@p1", SqlDbType.Int, entry._objectId));
					if (title != null && title != DBNull.Value)
					{
						entry._title = title.ToString();
					}
					else
					{
						entry._objectTypeId = -1;
						entry._objectId = -1;
						entry._title = string.Format(CultureInfo.InvariantCulture, "#{0}", entry._timeSheetId);
					}
				}
			}

			return ret;
		}
		#endregion

		#region public Load(IDataRecord reader)
		public static TTEntry Load(IDataRecord reader)
		{
			TTEntry ret = new TTEntry();

			ret._timeSheetId = (int)reader["TimeSheetId"];
			ret._weekTimeSheetId = (int)reader["WeekTimeSheetId"];
			ret._objectTypeId = (int)reader["ObjectTypeId"];
			ret._objectId = (int)reader["ObjectId"];
			ret._day1 = (int)reader["Day1"];
			ret._day2 = (int)reader["Day2"];
			ret._day3 = (int)reader["Day3"];
			ret._day4 = (int)reader["Day4"];
			ret._day5 = (int)reader["Day5"];
			ret._day6 = (int)reader["Day6"];
			ret._day7 = (int)reader["Day7"];
			ret._totalApproved = (int)reader["TotalApproved"];
			ret._rate = (decimal)reader["Rate"];
			ret._userComments = reader["UserComments"].ToString();
			ret._actualId = Helper.NullToInt32(reader["ActualId"]);

			return ret;
		}
		#endregion

		#region public Save(DBHelper target)
		public void Save(DBHelper target)
		{
			int objectTypeId = _objectTypeId;
			int objectId = _objectId;
			if (objectTypeId == 11)
			{
				objectTypeId = -1;
				objectId = -1;
			}

			_entryId = target.RunTextInteger("INSERT INTO [cls_TimeTrackingEntry] ([Card],[Title],[BlockTypeInstanceId],[Day1],[Day2],[Day3],[Day4],[Day5],[Day6],[Day7],[OwnerId],[ParentBlockId],[ObjectTypeId],[ObjectId],[TotalApproved],[Rate]) VALUES (@p1,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18,@p19) SELECT @retval = SCOPE_IDENTITY()"
				, DBHelper.MP("@p1", SqlDbType.NVarChar, 50, "TimeTrackingEntryDescr")
				, DBHelper.MP("@p5", SqlDbType.NVarChar, 100, _title)
				, DBHelper.MP("@p6", SqlDbType.Int, _blockTypeInstanceId)
				, DBHelper.MP("@p7", SqlDbType.Float, _day1)
				, DBHelper.MP("@p8", SqlDbType.Float, _day2)
				, DBHelper.MP("@p9", SqlDbType.Float, _day3)
				, DBHelper.MP("@p10", SqlDbType.Float, _day4)
				, DBHelper.MP("@p11", SqlDbType.Float, _day5)
				, DBHelper.MP("@p12", SqlDbType.Float, _day6)
				, DBHelper.MP("@p13", SqlDbType.Float, _day7)
				, DBHelper.MP("@p14", SqlDbType.Int, _ownerId)
				, DBHelper.MP("@p15", SqlDbType.Int, _parentBlockId)
				, DBHelper.MP("@p16", SqlDbType.Int, objectTypeId > 0 ? (object)objectTypeId : DBNull.Value)
				, DBHelper.MP("@p17", SqlDbType.Int, objectId > 0 ? (object)objectId : DBNull.Value)
				, DBHelper.MP("@p18", SqlDbType.Float, _totalApproved)
				, DBHelper.MP("@p19", SqlDbType.Money, _rate)
				);

			if (!string.IsNullOrEmpty(_userComments))
			{
				target.RunText("INSERT INTO [cls_TimeTrackingEntryDescr] ([TimeTrackingEntryId],[Comment]) VALUES (@TimeTrackingEntryId,@Comment)"
					, DBHelper.MP("@TimeTrackingEntryId", SqlDbType.Int, _entryId)
					, DBHelper.MP("@Comment", SqlDbType.NText, _userComments)
					);
			}

			if (_actualId > 0)
			{
				target.RunText("UPDATE [ActualFinances] SET [BlockId]=@BlockId, [OwnerId]=@OwnerId, [TotalApproved]=@TotalApproved WHERE [ActualFinancesId]=@ActualFinancesId"
					, DBHelper.MP("@BlockId", SqlDbType.Int, _parentBlockId)
					, DBHelper.MP("@ActualFinancesId", SqlDbType.Int, _actualId)
					, DBHelper.MP("@OwnerId", SqlDbType.Int, _ownerId)
					, DBHelper.MP("@TotalApproved", SqlDbType.Float, _totalApproved)
					);
			}
		}
		#endregion
	}
}
