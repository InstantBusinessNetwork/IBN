
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Globalization;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.IBN.Database.EMail
{
public partial class OutgoingEmailServiceConfigRow
	{
		private DataRowState _state = DataRowState.Unchanged;
		
		#region Column's name const
		
			public const string ColumnOutgoingEmailServiceConfigId = "OutgoingEmailServiceConfigId";

			public const string ColumnServiceType = "ServiceType";

			public const string ColumnServiceKey = "ServiceKey";

			public const string ColumnSmtpBoxId = "SmtpBoxId";

		#endregion
		

		Nullable<int> _OutgoingEmailServiceConfigId;
		int _ServiceType;

		Nullable<int> _ServiceKey;
		int _SmtpBoxId;


		public OutgoingEmailServiceConfigRow()
		{
			_state = DataRowState.Added;


			_ServiceKey = null;

		}

		public OutgoingEmailServiceConfigRow(int primaryKeyId)
		{
			using (IDataReader reader = DataHelper.Select("OutgoingEmailServiceConfig", 
							"OutgoingEmailServiceConfigId", 
							SqlDbType.Int,
							primaryKeyId))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Record with {0}.{1}={2} not found", "OutgoingEmailServiceConfig", "OutgoingEmailServiceConfigId", OutgoingEmailServiceConfigId), "primaryKeyId");
			}
		}
		
		public OutgoingEmailServiceConfigRow(IDataReader reader)
		{
			Load(reader);
		}
		
		protected void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_OutgoingEmailServiceConfigId = (int)SqlHelper.DBNull2Null(reader["OutgoingEmailServiceConfigId"]);
			
			_ServiceType = (int)SqlHelper.DBNull2Null(reader["ServiceType"]);
			
		    _ServiceKey = (Nullable< int >)SqlHelper.DBNull2Null(reader["ServiceKey"]);
		    
			_SmtpBoxId = (int)SqlHelper.DBNull2Null(reader["SmtpBoxId"]);
			
		
			OnLoad(reader);
		}
		
		protected virtual void OnLoad(IDataReader reader)
		{
		}

		#region Public Properties

		public virtual Nullable<int> PrimaryKeyId
		{
			get { return _OutgoingEmailServiceConfigId; }
			set { _OutgoingEmailServiceConfigId = value; }
		}
		
        
		public virtual int OutgoingEmailServiceConfigId
	    
		{
			get
			{
				
				return (int)_OutgoingEmailServiceConfigId;
				
			}
			
		}
		
		public virtual int ServiceType
	    
		{
			get
			{
				
				return _ServiceType;
				
			}
			
			set
			{
				_ServiceType = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual Nullable< int > ServiceKey
	    
		{
			get
			{
				
				return _ServiceKey;
				
			}
			
			set
			{
				_ServiceKey = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual int SmtpBoxId
	    
		{
			get
			{
				
				return _SmtpBoxId;
				
			}
			
			set
			{
				_SmtpBoxId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		#endregion
	
		public static OutgoingEmailServiceConfigRow[] List()
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("OutgoingEmailServiceConfig"))
			{
				while (reader.Read())
				{
					list.Add(new OutgoingEmailServiceConfigRow(reader));
				}
			}
			return (OutgoingEmailServiceConfigRow[])list.ToArray(typeof(OutgoingEmailServiceConfigRow));
		}
	
		public static OutgoingEmailServiceConfigRow[] List(params SortingElement[] sorting)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("OutgoingEmailServiceConfig", sorting))
			{
				while (reader.Read())
				{
					list.Add(new OutgoingEmailServiceConfigRow(reader));
				}
			}
			return (OutgoingEmailServiceConfigRow[])list.ToArray(typeof(OutgoingEmailServiceConfigRow));
		}

		public static OutgoingEmailServiceConfigRow[] List(params FilterElement[] filters)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("OutgoingEmailServiceConfig", filters))
			{
				while (reader.Read())
				{
					list.Add(new OutgoingEmailServiceConfigRow(reader));
				}
			}
			return (OutgoingEmailServiceConfigRow[])list.ToArray(typeof(OutgoingEmailServiceConfigRow));
		}

		public static OutgoingEmailServiceConfigRow[] List(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("OutgoingEmailServiceConfig", filters, sorting))
	       {
	          while (reader.Read()) 
	          {
			      list.Add(new OutgoingEmailServiceConfigRow(reader));
			  } 
	       }
	       return (OutgoingEmailServiceConfigRow[])list.ToArray(typeof(OutgoingEmailServiceConfigRow));
	    }

	    public static OutgoingEmailServiceConfigRow[] List(FilterElementCollection filters, SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("OutgoingEmailServiceConfig", filters, sorting))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new OutgoingEmailServiceConfigRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (OutgoingEmailServiceConfigRow[])list.ToArray(typeof(OutgoingEmailServiceConfigRow));
	    }
	
	    public static OutgoingEmailServiceConfigRow[] List(SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("OutgoingEmailServiceConfig", sorting.ToArray()))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new OutgoingEmailServiceConfigRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (OutgoingEmailServiceConfigRow[])list.ToArray(typeof(OutgoingEmailServiceConfigRow));
	    }
	
	    public static int GetTotalCount(params FilterElement[] filters)
	    {
	        return DataHelper.GetTotalCount("OutgoingEmailServiceConfig", filters);
	    }
	
	    public static IDataReader GetReader()
	    {
	       return DataHelper.List("OutgoingEmailServiceConfig");
	    }
	
	    public static IDataReader GetReader(params FilterElement[] filters)
	    {
	       return DataHelper.List("OutgoingEmailServiceConfig", filters);
	    }
	
	    public static IDataReader GetReader(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       return DataHelper.List("OutgoingEmailServiceConfig", filters, sorting);
	    }
	
	    public static DataTable GetTable()
	    {
	       return DataHelper.GetDataTable("OutgoingEmailServiceConfig");
	    }
	
	    public static DataTable GetTable(params FilterElement[] filters)
	    {
	       return DataHelper.GetDataTable("OutgoingEmailServiceConfig", filters);
	    }

		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DataHelper.Update("OutgoingEmailServiceConfig"

					, SqlHelper.SqlParameter("@OutgoingEmailServiceConfigId", SqlDbType.Int, _OutgoingEmailServiceConfigId)

					, SqlHelper.SqlParameter("@ServiceType", SqlDbType.Int, _ServiceType)

					, SqlHelper.SqlParameter("@ServiceKey", SqlDbType.Int, _ServiceKey)

					, SqlHelper.SqlParameter("@SmtpBoxId", SqlDbType.Int, _SmtpBoxId)

				);
			}
			else if (_state == DataRowState.Added) 
			{

				_OutgoingEmailServiceConfigId = DataHelper.Insert("OutgoingEmailServiceConfig", "OutgoingEmailServiceConfigId" 

					, SqlHelper.SqlParameter("@OutgoingEmailServiceConfigId", SqlDbType.Int, _OutgoingEmailServiceConfigId)

					, SqlHelper.SqlParameter("@ServiceType", SqlDbType.Int, _ServiceType)

					, SqlHelper.SqlParameter("@ServiceKey", SqlDbType.Int, _ServiceKey)

					, SqlHelper.SqlParameter("@SmtpBoxId", SqlDbType.Int, _SmtpBoxId)

				);

			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(this.OutgoingEmailServiceConfigId);
		}

		public static void Delete(int primaryKeyId)
		{
			DataHelper.Delete("OutgoingEmailServiceConfig",
				"OutgoingEmailServiceConfigId",
				SqlDbType.Int,
				primaryKeyId);
		}
	}
}
