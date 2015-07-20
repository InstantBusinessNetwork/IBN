
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.SpreadSheet
{
	public class ActualFinancesRow
	{
		#region enum DataRowState
		private enum DataRowState
		{
			Unchanged,
			Added,
			Modified,
			Deleted
		}
		#endregion
		
		private DataRowState _state = DataRowState.Unchanged;

		#region Util
		static object DBNull2Null(object Value)
		{
			if(Value==DBNull.Value)
				return null;
			return Value;
		}
		#endregion

		int _ActualFinancesId;
		int _CreatorId;
		DateTime _Date = DateTime.UtcNow;
		int _ObjectId;
		int _ObjectTypeId;
		string _RowId;
		double _Value;
		string _Comment = string.Empty;
		int? _BlockId;	// O.R. [2008-02-09]
		double? _TotalApproved;	// O.R. [2008-07-23]
		int? _OwnerId;	// O.R. [2008-07-23]

		#region CTR
		public ActualFinancesRow()
		{
			_state = DataRowState.Added;
			_Value = 0;  
		}

		public ActualFinancesRow(int ActualFinancesId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_ActualFinancesSelect", DbHelper2.mp("@ActualFinancesId", SqlDbType.Int, ActualFinancesId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "ActualFinances", "ActualFinancesId", ActualFinancesId));
			}
		}
		
		public ActualFinancesRow(IDataReader reader)
		{
		   Load(reader);
	   }
		#endregion

		#region Load
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			_ActualFinancesId = (int)DBNull2Null(reader["ActualFinancesId"]);
			_CreatorId = (int)DBNull2Null(reader["CreatorId"]);
			_Date = (DateTime)DBNull2Null(reader["Date"]);
			_ObjectId = (int)DBNull2Null(reader["ObjectId"]);
			_ObjectTypeId = (int)DBNull2Null(reader["ObjectTypeId"]);
			_RowId = (string)DBNull2Null(reader["RowId"]);
			_Value = (double)DBNull2Null(reader["Value"]);
			_Comment = (string)DBNull2Null(reader["Comment"]);
			_BlockId = (int?)DBNull2Null(reader["BlockId"]);		// O.R. [2008-02-09]
			_TotalApproved = (double?)DBNull2Null(reader["TotalApproved"]);	// O.R. [2008-07-23]
			_OwnerId = (int?)DBNull2Null(reader["OwnerId"]);		// O.R. [2008-02-09]
		}
		#endregion

		#region Public Properties
		public virtual int PrimaryKeyId
		{
			get { return _ActualFinancesId; }
		}
		
		public virtual int ActualFinancesId
		{
			get
			{
				return _ActualFinancesId;
			}
			
		}
		
		public virtual int CreatorId
		{
			get
			{
				return _CreatorId;
			}
			
			set
			{
				_CreatorId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
		}
		
		public virtual DateTime Date
		{
			get
			{
				return _Date;
			}
			
			set
			{
				_Date = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
		}
		
		public virtual int ObjectId
		{
			get
			{
				return _ObjectId;
			}
			
			set
			{
				_ObjectId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
		}
		
		public virtual int ObjectTypeId
		{
			get
			{
				return _ObjectTypeId;
			}
			
			set
			{
				_ObjectTypeId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
		}
		
		public virtual string RowId
		{
			get
			{
				return _RowId;
			}
			
			set
			{
				_RowId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
		}
		
		public virtual double Value
		{
			get
			{
				return _Value;
			}
			
			set
			{
				_Value = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
		}
		
		public virtual string Comment
		{
			get
			{
				return _Comment;
			}
			
			set
			{
				_Comment = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
		}

		// O.R. [2008-02-09]
		public virtual int? BlockId
		{
			get
			{
				return _BlockId;
			}

			set
			{
				_BlockId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}
		}

		// O.R. [2008-07-23]
		public virtual double? TotalApproved
		{
			get
			{
				return _TotalApproved;
			}

			set
			{
				_TotalApproved = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}
		}

		// O.R. [2008-07-23]
		public virtual int? OwnerId
		{
			get
			{
				return _OwnerId;
			}

			set
			{
				_OwnerId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}
		}
		#endregion

		#region Update
		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DbHelper2.RunSp("mc_ActualFinancesUpdate"
			        , DbHelper2.mp("@ActualFinancesId", SqlDbType.Int, _ActualFinancesId)
			        , DbHelper2.mp("@CreatorId", SqlDbType.Int, _CreatorId)
			        , DbHelper2.mp("@Date", SqlDbType.DateTime, _Date)
			        , DbHelper2.mp("@ObjectId", SqlDbType.Int, _ObjectId)
			        , DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, _ObjectTypeId)
			        , DbHelper2.mp("@RowId", SqlDbType.NVarChar, 250, _RowId)
			        , DbHelper2.mp("@Value", SqlDbType.Float, _Value)
			        , DbHelper2.mp("@Comment", SqlDbType.NText, _Comment)
					, DbHelper2.mp("@BlockId", SqlDbType.Int, _BlockId)		// O.R. [2008-02-09]
					, DbHelper2.mp("@TotalApproved", SqlDbType.Float, _TotalApproved)	// O.R. [2008-07-23]
					, DbHelper2.mp("@OwnerId", SqlDbType.Int, _OwnerId)		// O.R. [2008-07-23]
				);
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@ActualFinancesId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_ActualFinancesInsert", outPutKey
					, DbHelper2.mp("@CreatorId", SqlDbType.Int, _CreatorId)
					, DbHelper2.mp("@Date", SqlDbType.DateTime, _Date)
					, DbHelper2.mp("@ObjectId", SqlDbType.Int, _ObjectId)
					, DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, _ObjectTypeId)
					, DbHelper2.mp("@RowId", SqlDbType.NVarChar, 250, _RowId)
					, DbHelper2.mp("@Value", SqlDbType.Float, _Value)
					, DbHelper2.mp("@Comment", SqlDbType.NText, _Comment)
					, DbHelper2.mp("@BlockId", SqlDbType.Int, _BlockId)		// O.R. [2008-02-09]
					, DbHelper2.mp("@TotalApproved", SqlDbType.Float, _TotalApproved)	// O.R. [2008-07-23]
					, DbHelper2.mp("@OwnerId", SqlDbType.Int, _OwnerId)		// O.R. [2008-07-23]
					);

				_ActualFinancesId = (int)outPutKey.Value;
			}
		}
		#endregion

		#region Delete
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_ActualFinancesId);
		}
		
		public static void Delete(int ActualFinancesId)
		{
			DbHelper2.RunSp("mc_ActualFinancesDelete", DbHelper2.mp("@ActualFinancesId", SqlDbType.Int, ActualFinancesId));
		}

		public static void Delete(string RowId)
		{
			DbHelper2.RunSp("mc_ActualFinancesDeleteByRowId",
				DbHelper2.mp("@RowId", SqlDbType.NVarChar, 250, RowId));
		}

		// DV
		public static void Delete(int ObjectId, int ObjectTypeId)
		{
			DbHelper2.RunSp("mc_ActualFinancesDeleteByObjectIdObjectTypeId",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId));
		}

		// OR: [2008-02-09]
		public static void DeleteByBlockId(int blockId)
		{
			DbHelper2.RunSp("mc_ActualFinancesDeleteByBlockId",
				DbHelper2.mp("@BlockId", SqlDbType.Int, blockId));
		}
		#endregion

		#region List
		/// <summary>
		/// Lists the ActualFinaces.
		/// </summary>
		/// <param name="ObjectId">The object id.</param>
		/// <param name="ObjectTypeId">The object type id.</param>
		/// <returns>ActualFinancesId, CreatorId, Date, ObjectId, ObjectTypeId, RowId, Value, Comment, BlockId, TotalApproved, OwnerId</returns>
		public static ActualFinancesRow[] List(int objectId, int objectTypeId)
		{
			return List(objectId, objectTypeId, null, null);
		}

		/// <summary>
		/// Lists the ActualFinaces.
		/// </summary>
		/// <param name="objectId">The object id.</param>
		/// <param name="objectTypeId">The object type id.</param>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns>ActualFinancesId, CreatorId, Date, ObjectId, ObjectTypeId, RowId, Value, Comment, BlockId, TotalApproved, OwnerId</returns>
		public static ActualFinancesRow[] List(int objectId, int objectTypeId, DateTime? startDate, DateTime? endDate)
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_ActualFinancesList",
					  DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
					  DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, objectTypeId),
					  DbHelper2.mp("@StartDate", SqlDbType.DateTime, startDate),
					  DbHelper2.mp("@EndDate", SqlDbType.DateTime, endDate)
					  ))
			{
				while (reader.Read())
				{
					retVal.Add(new ActualFinancesRow(reader));
				}
			}
			return (ActualFinancesRow[])retVal.ToArray(typeof(ActualFinancesRow));
		}
		#endregion

		
    }
}
