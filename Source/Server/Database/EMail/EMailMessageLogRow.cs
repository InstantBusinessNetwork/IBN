
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class EMailMessageLogRow
    {
    	private enum DataRowState
		{
			Unchanged,
			Added,
			Modified,
			Deleted
		}
		
        private DataRowState _state = DataRowState.Unchanged;
        
        #region Util
		static object DBNull2Null(object Value)
		{
			if(Value==DBNull.Value)
				return null;
			return Value;
		}

		static object DBNull2Null(object Value, object DefaultValue)
		{
			if(Value==DBNull.Value)
				return DefaultValue;

			return Value;
		}

		#endregion
        
        int _EMailMessageLogId;
        DateTime _Created;
        string _From;
        string _To;
        string _Subject;
        bool _Incoming;
        int _EMailBoxId = -1;
        int _AntiSpamResult = -1;
        
        
        public EMailMessageLogRow()
        {
            _state = DataRowState.Added;
            
            
        }
        
		public EMailMessageLogRow(int EMailMessageLogId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailMessageLogSelect", DbHelper2.mp("@EMailMessageLogId", SqlDbType.Int, EMailMessageLogId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "EMailMessageLog", "EMailMessageLogId", EMailMessageLogId));
			}
		}
		
		public EMailMessageLogRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_EMailMessageLogId = (int)DBNull2Null(reader["EMailMessageLogId"]);
			
			_Created = (DateTime)DBNull2Null(reader["Created"]);
			
			_From = (string)DBNull2Null(reader["From"]);
			
			_To = (string)DBNull2Null(reader["To"]);
			
			_Subject = (string)DBNull2Null(reader["Subject"]);
			
			_Incoming = (bool)DBNull2Null(reader["Incoming"]);
			
			_EMailBoxId = (int)DBNull2Null(reader["EMailBoxId"],(int)-1);
			
			_AntiSpamResult = (int)DBNull2Null(reader["AntiSpamResult"],(int)-1);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _EMailMessageLogId; }		     
		}
		
        
	    public virtual int EMailMessageLogId
	    
		{
			get
			{
				return _EMailMessageLogId;
			}
			
		}
		
	    public virtual DateTime Created
	    
		{
			get
			{
				return _Created;
			}
			
			set
			{
				_Created = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string From
	    
		{
			get
			{
				return _From;
			}
			
			set
			{
				_From = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string To
	    
		{
			get
			{
				return _To;
			}
			
			set
			{
				_To = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string Subject
	    
		{
			get
			{
				return _Subject;
			}
			
			set
			{
				_Subject = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual bool Incoming
	    
		{
			get
			{
				return _Incoming;
			}
			
			set
			{
				_Incoming = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int EMailBoxId
	    
		{
			get
			{
				return _EMailBoxId;
			}
			
			set
			{
				_EMailBoxId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int AntiSpamResult
	    
		{
			get
			{
				return _AntiSpamResult;
			}
			
			set
			{
				_AntiSpamResult = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		#endregion
	
		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DbHelper2.RunSp("mc_EMailMessageLogUpdate"
			    
			        , DbHelper2.mp("@EMailMessageLogId", SqlDbType.Int, _EMailMessageLogId)
			        
			        , DbHelper2.mp("@Created", SqlDbType.DateTime, _Created)
			        
			        , DbHelper2.mp("@From", SqlDbType.NVarChar, 255, _From)
			        
			        , DbHelper2.mp("@To", SqlDbType.NVarChar, 255, _To)
			        
			        , DbHelper2.mp("@Subject", SqlDbType.NText, _Subject)
			        
			        , DbHelper2.mp("@Incoming", SqlDbType.Bit, _Incoming)
			        
			        , DbHelper2.mp("@EMailBoxId", SqlDbType.Int, _EMailBoxId<0?DBNull.Value:(object)_EMailBoxId)
			        
			        , DbHelper2.mp("@AntiSpamResult", SqlDbType.Int, _AntiSpamResult)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@EMailMessageLogId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_EMailMessageLogInsert", outPutKey
		            
		            , DbHelper2.mp("@Created", SqlDbType.DateTime, _Created)
		            
			        , DbHelper2.mp("@From", SqlDbType.NVarChar, 255, _From)
			        
			        , DbHelper2.mp("@To", SqlDbType.NVarChar, 255, _To)
			        
		            , DbHelper2.mp("@Subject", SqlDbType.NText, _Subject)
		            
		            , DbHelper2.mp("@Incoming", SqlDbType.Bit, _Incoming)
		            
					, DbHelper2.mp("@EMailBoxId", SqlDbType.Int, _EMailBoxId<0?DBNull.Value:(object)_EMailBoxId)
			        
					, DbHelper2.mp("@AntiSpamResult", SqlDbType.Int, _AntiSpamResult)
		             
		        );
		        
		        _EMailMessageLogId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_EMailMessageLogId);
		}
		
		public static void Delete(int EMailMessageLogId)
		{
		    DbHelper2.RunSp("mc_EMailMessageLogDelete", DbHelper2.mp("@EMailMessageLogId", SqlDbType.Int, EMailMessageLogId));
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public static void Clear()
		{
			DbHelper2.RunSp("mc_EMailMessageLogClear");
		}

		/// <summary>
		/// Cleans the up.
		/// </summary>
		/// <param name="Period">The period.</param>
		public static void CleanUp(int Period)
		{
			DbHelper2.RunSp("mc_EMailMessageLogCleanUp", DbHelper2.mp("@Period", SqlDbType.Int, Period));
		}


		/// <summary>
		/// Lists this instance.
		/// </summary>
		/// <returns></returns>
		public static EMailMessageLogRow[] List()
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailMessageLogList"))
			{
				while(reader.Read())
				{
					retVal.Add(new EMailMessageLogRow(reader));
				}
			}
			return (EMailMessageLogRow[])retVal.ToArray(typeof(EMailMessageLogRow));
		}
    }
}
