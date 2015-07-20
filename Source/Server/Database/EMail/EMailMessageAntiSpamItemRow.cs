
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class EMailMessageAntiSpamItemRow
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
        
        int _EMailMessageAntiSpamItemId;
        string _From;
        bool _IsWhite;
        
        
		/// <summary>
		/// Initializes a new instance of the <see cref="EMailMessageAntiSpamItemRow"/> class.
		/// </summary>
        public EMailMessageAntiSpamItemRow()
        {
            _state = DataRowState.Added;
            
            
            _IsWhite = false;  
            
        }
        
		/// <summary>
		/// Initializes a new instance of the <see cref="EMailMessageAntiSpamItemRow"/> class.
		/// </summary>
		/// <param name="EMailMessageAntiSpamItemId">The E mail message anti spam item id.</param>
		public EMailMessageAntiSpamItemRow(int EMailMessageAntiSpamItemId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailMessageAntiSpamItemSelect", DbHelper2.mp("@EMailMessageAntiSpamItemId", SqlDbType.Int, EMailMessageAntiSpamItemId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "EMailMessageAntiSpamItem", "EMailMessageAntiSpamItemId", EMailMessageAntiSpamItemId));
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="EMailMessageAntiSpamItemRow"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public EMailMessageAntiSpamItemRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		/// <summary>
		/// Loads the specified reader.
		/// </summary>
		/// <param name="reader">The reader.</param>
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_EMailMessageAntiSpamItemId = (int)DBNull2Null(reader["EMailMessageAntiSpamItemId"]);
			
			_From = (string)DBNull2Null(reader["From"]);
			
			_IsWhite = (bool)DBNull2Null(reader["IsWhite"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _EMailMessageAntiSpamItemId; }		     
		}
		
        
	    public virtual int EMailMessageAntiSpamItemId
	    
		{
			get
			{
				return _EMailMessageAntiSpamItemId;
			}
			
			set
			{
				_EMailMessageAntiSpamItemId = value;
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
		
	    public virtual bool IsWhite
	    
		{
			get
			{
				return _IsWhite;
			}
			
			set
			{
				_IsWhite = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		#endregion
	
		/// <summary>
		/// Updates this instance.
		/// </summary>
		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DbHelper2.RunSp("mc_EMailMessageAntiSpamItemUpdate"
			    
			        , DbHelper2.mp("@EMailMessageAntiSpamItemId", SqlDbType.Int, _EMailMessageAntiSpamItemId)
			        
			        , DbHelper2.mp("@From", SqlDbType.NVarChar, 255, _From)
			        
			        , DbHelper2.mp("@IsWhite", SqlDbType.Bit, _IsWhite)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@EMailMessageAntiSpamItemId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_EMailMessageAntiSpamItemInsert", outPutKey
		            
			        , DbHelper2.mp("@From", SqlDbType.NVarChar, 255, _From)
			        
		            , DbHelper2.mp("@IsWhite", SqlDbType.Bit, _IsWhite)
		             
		        );
		        
		        _EMailMessageAntiSpamItemId = (int)outPutKey.Value;
			}
		}
		
		/// <summary>
		/// Deletes this instance.
		/// </summary>
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_EMailMessageAntiSpamItemId);
		}
		
		/// <summary>
		/// Deletes the specified E mail message anti spam item id.
		/// </summary>
		/// <param name="EMailMessageAntiSpamItemId">The E mail message anti spam item id.</param>
		public static void Delete(int EMailMessageAntiSpamItemId)
		{
		    DbHelper2.RunSp("mc_EMailMessageAntiSpamItemDelete", DbHelper2.mp("@EMailMessageAntiSpamItemId", SqlDbType.Int, EMailMessageAntiSpamItemId));
		}

		/// <summary>
		/// Lists the specified is white.
		/// </summary>
		/// <param name="IsWhite">if set to <c>true</c> [is white].</param>
		/// <returns></returns>
		public static EMailMessageAntiSpamItemRow[] List(bool IsWhite)
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailMessageAntiSpamItemList", 
					   DbHelper2.mp("@IsWhite", SqlDbType.Bit, IsWhite)))
			{
				while(reader.Read())
				{
					retVal.Add(new EMailMessageAntiSpamItemRow(reader));
				}
			}
			return (EMailMessageAntiSpamItemRow[])retVal.ToArray(typeof(EMailMessageAntiSpamItemRow));
		}

		/// <summary>
		/// Lists the specified is white.
		/// </summary>
		/// <param name="IsWhite">if set to <c>true</c> [is white].</param>
		/// <param name="Keyword">The keyword.</param>
		/// <returns></returns>
		public static EMailMessageAntiSpamItemRow[] List(bool IsWhite, string Keyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailMessageAntiSpamItemListByKeyword", 
					   DbHelper2.mp("@IsWhite", SqlDbType.Bit, IsWhite),
					   DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 255, string.Format("%{0}%", Keyword))))
			{
				while(reader.Read())
				{
					retVal.Add(new EMailMessageAntiSpamItemRow(reader));
				}
			}
			return (EMailMessageAntiSpamItemRow[])retVal.ToArray(typeof(EMailMessageAntiSpamItemRow));
		}

		/// <summary>
		/// Lists the specified from.
		/// </summary>
		/// <param name="From">From.</param>
		/// <returns></returns>
		public static EMailMessageAntiSpamItemRow[] List(string From)
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailMessageAntiSpamItemListByFrom", 
					   DbHelper2.mp("@From", SqlDbType.NVarChar, 255, From)))
			{
				while(reader.Read())
				{
					retVal.Add(new EMailMessageAntiSpamItemRow(reader));
				}
			}
			return (EMailMessageAntiSpamItemRow[])retVal.ToArray(typeof(EMailMessageAntiSpamItemRow));
		}
    }
}
