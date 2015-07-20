
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Globalization;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.IBN.Database.EMail
{
public partial class SmtpBoxRow
	{
		private DataRowState _state = DataRowState.Unchanged;
		
		#region Column's name const
		
			public const string ColumnSmtpBoxId = "SmtpBoxId";

			public const string ColumnName = "Name";

			public const string ColumnServer = "Server";

			public const string ColumnPort = "Port";

			public const string ColumnSecureConnection = "SecureConnection";

			public const string ColumnAuthenticate = "Authenticate";

			public const string ColumnUser = "User";

			public const string ColumnPassword = "Password";

			public const string ColumnIsDefault = "IsDefault";

			public const string ColumnCheckUid = "CheckUid";

			public const string ColumnChecked = "Checked";

		#endregion
		

		Nullable<int> _SmtpBoxId;
		string _Name;
		string _Server;
		int _Port;
		int _SecureConnection;
		bool _Authenticate;
		string _User;
		string _Password;
		bool _IsDefault;
		Guid _CheckUid;
		bool _Checked;


		public SmtpBoxRow()
		{
			_state = DataRowState.Added;


			_Port = 25;

			_SecureConnection = 0;

			_Authenticate = false;

			_IsDefault = false;

			_Checked = false;

		}

		public SmtpBoxRow(int primaryKeyId)
		{
			using (IDataReader reader = DataHelper.Select("SmtpBox", 
							"SmtpBoxId", 
							SqlDbType.Int,
							primaryKeyId))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Record with {0}.{1}={2} not found", "SmtpBox", "SmtpBoxId", SmtpBoxId), "primaryKeyId");
			}
		}
		
		public SmtpBoxRow(IDataReader reader)
		{
			Load(reader);
		}
		
		protected void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_SmtpBoxId = (int)SqlHelper.DBNull2Null(reader["SmtpBoxId"]);
			
			_Name = (string)SqlHelper.DBNull2Null(reader["Name"]);
			
			_Server = (string)SqlHelper.DBNull2Null(reader["Server"]);
			
			_Port = (int)SqlHelper.DBNull2Null(reader["Port"]);
			
			_SecureConnection = (int)SqlHelper.DBNull2Null(reader["SecureConnection"]);
			
			_Authenticate = (bool)SqlHelper.DBNull2Null(reader["Authenticate"]);
			
			_User = (string)SqlHelper.DBNull2Null(reader["User"]);
			
			_Password = (string)SqlHelper.DBNull2Null(reader["Password"]);
			
			_IsDefault = (bool)SqlHelper.DBNull2Null(reader["IsDefault"]);
			
			_CheckUid = (Guid)SqlHelper.DBNull2Null(reader["CheckUid"]);
			
			_Checked = (bool)SqlHelper.DBNull2Null(reader["Checked"]);
			
		
			OnLoad(reader);
		}
		
		protected virtual void OnLoad(IDataReader reader)
		{
		}

		#region Public Properties

		public virtual Nullable<int> PrimaryKeyId
		{
			get { return _SmtpBoxId; }
			set { _SmtpBoxId = value; }
		}
		
        
		public virtual int SmtpBoxId
	    
		{
			get
			{
				
				return (int)_SmtpBoxId;
				
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
		
		public virtual int SecureConnection
	    
		{
			get
			{
				
				return _SecureConnection;
				
			}
			
			set
			{
				_SecureConnection = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual bool Authenticate
	    
		{
			get
			{
				
				return _Authenticate;
				
			}
			
			set
			{
				_Authenticate = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string User
	    
		{
			get
			{
				
				return _User;
				
			}
			
			set
			{
				_User = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string Password
	    
		{
			get
			{
				
				return _Password;
				
			}
			
			set
			{
				_Password = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual bool IsDefault
	    
		{
			get
			{
				
				return _IsDefault;
				
			}
			
			set
			{
				_IsDefault = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual Guid CheckUid
	    
		{
			get
			{
				
				return _CheckUid;
				
			}
			
			set
			{
				_CheckUid = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual bool Checked
	    
		{
			get
			{
				
				return _Checked;
				
			}
			
			set
			{
				_Checked = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		#endregion
	
		public static SmtpBoxRow[] List()
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("SmtpBox"))
			{
				while (reader.Read())
				{
					list.Add(new SmtpBoxRow(reader));
				}
			}
			return (SmtpBoxRow[])list.ToArray(typeof(SmtpBoxRow));
		}
	
		public static SmtpBoxRow[] List(params SortingElement[] sorting)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("SmtpBox", sorting))
			{
				while (reader.Read())
				{
					list.Add(new SmtpBoxRow(reader));
				}
			}
			return (SmtpBoxRow[])list.ToArray(typeof(SmtpBoxRow));
		}

		public static SmtpBoxRow[] List(params FilterElement[] filters)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("SmtpBox", filters))
			{
				while (reader.Read())
				{
					list.Add(new SmtpBoxRow(reader));
				}
			}
			return (SmtpBoxRow[])list.ToArray(typeof(SmtpBoxRow));
		}

		public static SmtpBoxRow[] List(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("SmtpBox", filters, sorting))
	       {
	          while (reader.Read()) 
	          {
			      list.Add(new SmtpBoxRow(reader));
			  } 
	       }
	       return (SmtpBoxRow[])list.ToArray(typeof(SmtpBoxRow));
	    }

	    public static SmtpBoxRow[] List(FilterElementCollection filters, SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("SmtpBox", filters, sorting))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new SmtpBoxRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (SmtpBoxRow[])list.ToArray(typeof(SmtpBoxRow));
	    }
	
	    public static SmtpBoxRow[] List(SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("SmtpBox", sorting.ToArray()))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new SmtpBoxRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (SmtpBoxRow[])list.ToArray(typeof(SmtpBoxRow));
	    }
	
	    public static int GetTotalCount(params FilterElement[] filters)
	    {
	        return DataHelper.GetTotalCount("SmtpBox", filters);
	    }
	
	    public static IDataReader GetReader()
	    {
	       return DataHelper.List("SmtpBox");
	    }
	
	    public static IDataReader GetReader(params FilterElement[] filters)
	    {
	       return DataHelper.List("SmtpBox", filters);
	    }
	
	    public static IDataReader GetReader(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       return DataHelper.List("SmtpBox", filters, sorting);
	    }
	
	    public static DataTable GetTable()
	    {
	       return DataHelper.GetDataTable("SmtpBox");
	    }
	
	    public static DataTable GetTable(params FilterElement[] filters)
	    {
	       return DataHelper.GetDataTable("SmtpBox", filters);
	    }

		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DataHelper.Update("SmtpBox"

					, SqlHelper.SqlParameter("@SmtpBoxId", SqlDbType.Int, _SmtpBoxId)

					, SqlHelper.SqlParameter("@Name", SqlDbType.NVarChar, 255, _Name)

					, SqlHelper.SqlParameter("@Server", SqlDbType.NVarChar, 255, _Server)

					, SqlHelper.SqlParameter("@Port", SqlDbType.Int, _Port)

					, SqlHelper.SqlParameter("@SecureConnection", SqlDbType.Int, _SecureConnection)

					, SqlHelper.SqlParameter("@Authenticate", SqlDbType.Bit, _Authenticate)

					, SqlHelper.SqlParameter("@User", SqlDbType.NVarChar, 255, _User)

					, SqlHelper.SqlParameter("@Password", SqlDbType.NVarChar, 255, _Password)

					, SqlHelper.SqlParameter("@IsDefault", SqlDbType.Bit, _IsDefault)

					, SqlHelper.SqlParameter("@CheckUid", SqlDbType.UniqueIdentifier, _CheckUid)

					, SqlHelper.SqlParameter("@Checked", SqlDbType.Bit, _Checked)

				);
			}
			else if (_state == DataRowState.Added) 
			{

				_SmtpBoxId = DataHelper.Insert("SmtpBox", "SmtpBoxId" 

					, SqlHelper.SqlParameter("@SmtpBoxId", SqlDbType.Int, _SmtpBoxId)

					, SqlHelper.SqlParameter("@Name", SqlDbType.NVarChar, 255, _Name)

					, SqlHelper.SqlParameter("@Server", SqlDbType.NVarChar, 255, _Server)

					, SqlHelper.SqlParameter("@Port", SqlDbType.Int, _Port)

					, SqlHelper.SqlParameter("@SecureConnection", SqlDbType.Int, _SecureConnection)

					, SqlHelper.SqlParameter("@Authenticate", SqlDbType.Bit, _Authenticate)

					, SqlHelper.SqlParameter("@User", SqlDbType.NVarChar, 255, _User)

					, SqlHelper.SqlParameter("@Password", SqlDbType.NVarChar, 255, _Password)

					, SqlHelper.SqlParameter("@IsDefault", SqlDbType.Bit, _IsDefault)

					, SqlHelper.SqlParameter("@CheckUid", SqlDbType.UniqueIdentifier, _CheckUid)

					, SqlHelper.SqlParameter("@Checked", SqlDbType.Bit, _Checked)

				);

			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(this.SmtpBoxId);
		}

		public static void Delete(int primaryKeyId)
		{
			DataHelper.Delete("SmtpBox",
				"SmtpBoxId",
				SqlDbType.Int,
				primaryKeyId);
		}
	}
}
