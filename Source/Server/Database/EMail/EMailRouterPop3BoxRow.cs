
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
	public class EMailRouterPop3BoxRow
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
        
		int _EMailRouterPop3BoxId;
		string _Name;
		string _Server;
		int _Port;
		string _Login;
		string _Pass;
		bool _IsInternal;
		string _InternalEMailAddress;
		string _Settings = string.Empty;
        int _UseSecureConnection = 0;
        
        
		public EMailRouterPop3BoxRow()
		{
			_state = DataRowState.Added;
            
            
			_Port = 110;  
            
			_IsInternal = false;  
            
			_InternalEMailAddress = null;

            _UseSecureConnection = 0;
            
		}
        
		public EMailRouterPop3BoxRow(int EMailRouterPop3BoxId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailRouterPop3BoxSelect", DbHelper2.mp("@EMailRouterPop3BoxId", SqlDbType.Int, EMailRouterPop3BoxId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "EMailRouterPop3Box", "EMailRouterPop3BoxId", EMailRouterPop3BoxId));
			}
		}
		
		public EMailRouterPop3BoxRow(IDataReader reader)
		{
			Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_EMailRouterPop3BoxId = (int)DBNull2Null(reader["EMailRouterPop3BoxId"]);
			
			_Name = (string)DBNull2Null(reader["Name"]);
			_Server = (string)DBNull2Null(reader["Server"]);
			
			_Port = (int)DBNull2Null(reader["Port"]);
			
			_Login = (string)DBNull2Null(reader["Login"]);
			
			_Pass = (string)DBNull2Null(reader["Pass"]);
			
			_IsInternal = (bool)DBNull2Null(reader["IsInternal"]);
			
			_InternalEMailAddress = (string)DBNull2Null(reader["InternalEMailAddress"]);
			
			_Settings = (string)DBNull2Null(reader["Settings"]);

            _UseSecureConnection = (int)DBNull2Null(reader["UseSecureConnection"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
			get { return _EMailRouterPop3BoxId; }		     
		}
		
        
		public virtual int EMailRouterPop3BoxId
	    
		{
			get
			{
				return _EMailRouterPop3BoxId;
			}
			
		}

		public virtual string Name
	    
		{
			get
			{
				return _Name;
			}
			
			set
			{
				_Name = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}

		public virtual string Server
	    
		{
			get
			{
				return _Server;
			}
			
			set
			{
				_Server = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual int Port
	    
		{
			get
			{
				return _Port;
			}
			
			set
			{
				_Port = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string Login
	    
		{
			get
			{
				return _Login;
			}
			
			set
			{
				_Login = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string Pass
	    
		{
			get
			{
				return _Pass;
			}
			
			set
			{
				_Pass = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual bool IsInternal
	    
		{
			get
			{
				return _IsInternal;
			}
			
			set
			{
				_IsInternal = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string InternalEMailAddress
	    
		{
			get
			{
				return _InternalEMailAddress;
			}
			
			set
			{
				_InternalEMailAddress = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string Settings
	    
		{
			get
			{
				return _Settings;
			}
			
			set
			{
				_Settings = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}

        public int UseSecureConnection
        {
            get
            {
                return _UseSecureConnection;
            }
            set
            {
                _UseSecureConnection = value;
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
				DbHelper2.RunSp("mc_EMailRouterPop3BoxUpdate"
			    
					, DbHelper2.mp("@EMailRouterPop3BoxId", SqlDbType.Int, _EMailRouterPop3BoxId)
			        
					, DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, _Name)

					, DbHelper2.mp("@Server", SqlDbType.NVarChar, 255, _Server)
			        
					, DbHelper2.mp("@Port", SqlDbType.Int, _Port)
			        
					, DbHelper2.mp("@Login", SqlDbType.NVarChar, 255, _Login)
			        
					, DbHelper2.mp("@Pass", SqlDbType.NVarChar, 255, _Pass)
			        
					, DbHelper2.mp("@IsInternal", SqlDbType.Bit, _IsInternal)
			        
					, DbHelper2.mp("@InternalEMailAddress", SqlDbType.NVarChar, 255, _InternalEMailAddress)
			        
					, DbHelper2.mp("@Settings", SqlDbType.NText, _Settings)

                    , DbHelper2.mp("@UseSecureConnection", SqlDbType.Int, _UseSecureConnection)
			         
					);
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@EMailRouterPop3BoxId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_EMailRouterPop3BoxInsert", outPutKey
		            
					, DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, _Name)

					, DbHelper2.mp("@Server", SqlDbType.NVarChar, 255, _Server)
			        
					, DbHelper2.mp("@Port", SqlDbType.Int, _Port)
		            
					, DbHelper2.mp("@Login", SqlDbType.NVarChar, 255, _Login)
			        
					, DbHelper2.mp("@Pass", SqlDbType.NVarChar, 255, _Pass)
			        
					, DbHelper2.mp("@IsInternal", SqlDbType.Bit, _IsInternal)
		            
					, DbHelper2.mp("@InternalEMailAddress", SqlDbType.NVarChar, 255, _InternalEMailAddress)
			        
					, DbHelper2.mp("@Settings", SqlDbType.NText, _Settings)

                    , DbHelper2.mp("@UseSecureConnection", SqlDbType.Int, _UseSecureConnection)
		             
					);
		        
				_EMailRouterPop3BoxId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_EMailRouterPop3BoxId);
		}
		
		public static void Delete(int EMailRouterPop3BoxId)
		{
			DbHelper2.RunSp("mc_EMailRouterPop3BoxDelete", DbHelper2.mp("@EMailRouterPop3BoxId", SqlDbType.Int, EMailRouterPop3BoxId));
		}

		public static EMailRouterPop3BoxRow[] List(bool IsInternal)
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailRouterPop3BoxList", 
					   DbHelper2.mp("@IsInternal", SqlDbType.Bit, IsInternal)))
			{
				while(reader.Read())
				{
					retVal.Add(new EMailRouterPop3BoxRow(reader));
				}
			}
			return (EMailRouterPop3BoxRow[])retVal.ToArray(typeof(EMailRouterPop3BoxRow));
		}
	}
}
