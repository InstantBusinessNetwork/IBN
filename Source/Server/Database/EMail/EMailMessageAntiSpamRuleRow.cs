
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class EMailMessageAntiSpamRuleRow
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
        
        int _AntiSpamRuleId;
        bool _Accept;
        string _Key;
        int _RuleType;
        string _Value;
        int _Weight;
        
        
        public EMailMessageAntiSpamRuleRow()
        {
            _state = DataRowState.Added;
            
            
            _Weight = 0;  
            
        }
        
		public EMailMessageAntiSpamRuleRow(int AntiSpamRuleId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailMessageAntiSpamRuleSelect", DbHelper2.mp("@AntiSpamRuleId", SqlDbType.Int, AntiSpamRuleId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "EMailMessageAntiSpamRule", "AntiSpamRuleId", AntiSpamRuleId));
			}
		}
		
		public EMailMessageAntiSpamRuleRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_AntiSpamRuleId = (int)DBNull2Null(reader["AntiSpamRuleId"]);
			
			_Accept = (bool)DBNull2Null(reader["Accept"]);
			
			_Key = (string)DBNull2Null(reader["Key"]);
			
			_RuleType = (int)DBNull2Null(reader["RuleType"]);
			
			_Value = (string)DBNull2Null(reader["Value"]);
			
			_Weight = (int)DBNull2Null(reader["Weight"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _AntiSpamRuleId; }		     
		}
		
        
	    public virtual int AntiSpamRuleId
	    
		{
			get
			{
				return _AntiSpamRuleId;
			}
			
		}
		
	    public virtual bool Accept
	    
		{
			get
			{
				return _Accept;
			}
			
			set
			{
				_Accept = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string Key
	    
		{
			get
			{
				return _Key;
			}
			
			set
			{
				_Key = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int RuleType
	    
		{
			get
			{
				return _RuleType;
			}
			
			set
			{
				_RuleType = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string Value
	    
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
		
	    public virtual int Weight
	    
		{
			get
			{
				return _Weight;
			}
			
			set
			{
				_Weight = value;
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
				DbHelper2.RunSp("mc_EMailMessageAntiSpamRuleUpdate"
			    
			        , DbHelper2.mp("@AntiSpamRuleId", SqlDbType.Int, _AntiSpamRuleId)
			        
			        , DbHelper2.mp("@Accept", SqlDbType.Bit, _Accept)
			        
			        , DbHelper2.mp("@Key", SqlDbType.NVarChar, 50, _Key)
			        
			        , DbHelper2.mp("@RuleType", SqlDbType.Int, _RuleType)
			        
			        , DbHelper2.mp("@Value", SqlDbType.NText, _Value)
			        
			        , DbHelper2.mp("@Weight", SqlDbType.Int, _Weight)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@AntiSpamRuleId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_EMailMessageAntiSpamRuleInsert", outPutKey
		            
		            , DbHelper2.mp("@Accept", SqlDbType.Bit, _Accept)
		            
			        , DbHelper2.mp("@Key", SqlDbType.NVarChar, 50, _Key)
			        
		            , DbHelper2.mp("@RuleType", SqlDbType.Int, _RuleType)
		            
		            , DbHelper2.mp("@Value", SqlDbType.NText, _Value)
		            
		            , DbHelper2.mp("@Weight", SqlDbType.Int, _Weight)
		             
		        );
		        
		        _AntiSpamRuleId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_AntiSpamRuleId);
		}
		
		public static void Delete(int AntiSpamRuleId)
		{
		    DbHelper2.RunSp("mc_EMailMessageAntiSpamRuleDelete", DbHelper2.mp("@AntiSpamRuleId", SqlDbType.Int, AntiSpamRuleId));
		}

		public static EMailMessageAntiSpamRuleRow[] List()
		{
			ArrayList retVal = new ArrayList();

			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailMessageAntiSpamRuleList"))
			{
				while(reader.Read())
				{
					retVal.Add(new EMailMessageAntiSpamRuleRow(reader));
				}
			}

			return (EMailMessageAntiSpamRuleRow[])retVal.ToArray(typeof(EMailMessageAntiSpamRuleRow));
		}
    }
}
