
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class EMailIssueExternalRecipientRow
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
		#endregion
        
        int _EMailIssueExternalRecipientId;
        int _IssueId;
        string _EMail;
        
        
        public EMailIssueExternalRecipientRow()
        {
            _state = DataRowState.Added;
            
            
        }
        
		public EMailIssueExternalRecipientRow(int EMailIssueExternalRecipientId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailIssueExternalRecipientSelect", DbHelper2.mp("@EMailIssueExternalRecipientId", SqlDbType.Int, EMailIssueExternalRecipientId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "EMailIssueExternalRecipient", "EMailIssueExternalRecipientId", EMailIssueExternalRecipientId));
			}
		}
		
		public EMailIssueExternalRecipientRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_EMailIssueExternalRecipientId = (int)DBNull2Null(reader["EMailIssueExternalRecipientId"]);
			
			_IssueId = (int)DBNull2Null(reader["IssueId"]);
			
			_EMail = (string)DBNull2Null(reader["EMail"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _EMailIssueExternalRecipientId; }		     
		}
		
        
	    public virtual int EMailIssueExternalRecipientId
	    
		{
			get
			{
				return _EMailIssueExternalRecipientId;
			}
			
		}
		
	    public virtual int IssueId
	    
		{
			get
			{
				return _IssueId;
			}
			
			set
			{
				_IssueId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string EMail
	    
		{
			get
			{
				return _EMail;
			}
			
			set
			{
				_EMail = value;
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
				DbHelper2.RunSp("mc_EMailIssueExternalRecipientUpdate"
			    
			        , DbHelper2.mp("@EMailIssueExternalRecipientId", SqlDbType.Int, _EMailIssueExternalRecipientId)
			        
			        , DbHelper2.mp("@IssueId", SqlDbType.Int, _IssueId)
			        
			        , DbHelper2.mp("@EMail", SqlDbType.NVarChar, 255, _EMail)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@EMailIssueExternalRecipientId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_EMailIssueExternalRecipientInsert", outPutKey
		            
		            , DbHelper2.mp("@IssueId", SqlDbType.Int, _IssueId)
		            
			        , DbHelper2.mp("@EMail", SqlDbType.NVarChar, 255, _EMail)
			         
		        );
		        
		        _EMailIssueExternalRecipientId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_EMailIssueExternalRecipientId);
		}
		
		public static void Delete(int EMailIssueExternalRecipientId)
		{
		    DbHelper2.RunSp("mc_EMailIssueExternalRecipientDelete", DbHelper2.mp("@EMailIssueExternalRecipientId", SqlDbType.Int, EMailIssueExternalRecipientId));
		}

		public static EMailIssueExternalRecipientRow[] List(int IssueId)
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailIssueExternalRecipientList", 
					   DbHelper2.mp("@IssueId", SqlDbType.Int, IssueId)))
			{
				while(reader.Read())
				{
					retVal.Add(new EMailIssueExternalRecipientRow(reader));
				}
			}
			return (EMailIssueExternalRecipientRow[])retVal.ToArray(typeof(EMailIssueExternalRecipientRow));
		}
    }
}
