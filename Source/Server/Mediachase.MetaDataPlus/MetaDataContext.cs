using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.MetaDataPlus.Common;


namespace Mediachase.MetaDataPlus
{
	public enum MetaFileDataStorageType
	{
		None = 0,
		DataBase = 1,
		LocalDisk = 2
	}
	/// <summary>
	/// Summary description for MetaDataContext.
	/// </summary>
	public class MetaDataContext
	{
		//		internal class MetaDataContextData
		//		{
		//			internal string				ConnectionString	=	string.Empty;
		//			internal SqlTransaction		CurrentTransaction	=	null;
		//		}

		//		private enum SqlTransactionOwnership	
		//		{
		//			/// <summary>Connection is owned and managed by SqlHelper</summary>
		//			Internal, 
		//			/// <summary>Connection is owned and managed by the caller</summary>
		//			External
		//		}

		private static MetaDataContext _current = new MetaDataContext();
		private string _connectionString = string.Empty;
		private MetaFileDataStorageType _metaFileDataStorageType = MetaFileDataStorageType.DataBase;
		private string _localDiskFolder = string.Empty;

		// Fixed : One Thread One SqlTransaction  [2/10/2005]
		[ThreadStatic]
		private static SqlTransaction _currentTransaction;

		[ThreadStatic]
		private static string _language;

		private bool _useCurrentIUCulture = true;

		protected MetaDataContext()
		{
		}

		public static MetaDataContext Current
		{
			get
			{
				return _current;
			}
		}

		public virtual string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				_connectionString = value;
			}
		}

		public virtual SqlTransaction Transaction
		{
			get
			{
				return _currentTransaction;
			}
			set
			{
				//_OwnerShip = SqlTransactionOwnership.External;
				_currentTransaction = value;
			}
		}

		internal virtual string Language
		{
			get
			{
				if (_useCurrentIUCulture)
				{
					return System.Globalization.CultureInfo.CurrentUICulture.Name;
				}
				else return _language;
			}
			set
			{
				if (!_useCurrentIUCulture)
				{
					_language = value;
				}
				else
					throw new MetaDataPlusException("Cannot change Language when UseCurrentIUCulture is on.");
			}
		}

		public bool UseCurrentIUCulture
		{
			get
			{
				return _useCurrentIUCulture;
			}
			set
			{
				_useCurrentIUCulture = value;
			}
		}

		public void ReplaceUser(int oldUserId, int newUserId)
		{
			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure,
				AsyncResources.GetConstantValue("SP_ReplaceUser"),
				new SqlParameter("@OldUserId", oldUserId),
				new SqlParameter("@NewUserId", newUserId));
		}

		public virtual void BeginTransaction()
		{
			SqlConnection myConnection = new SqlConnection(MetaDataContext.Current.ConnectionString);
			myConnection.Open();

			this.Transaction = myConnection.BeginTransaction();
			//_OwnerShip = SqlTransactionOwnership.Internal;
		}

		public virtual void Commit()
		{
			if (this.Transaction != null)
			{
				SqlConnection connection = this.Transaction.Connection;
				try
				{
					this.Transaction.Commit();
					this.Transaction = null;
				}
				finally
				{
					connection.Close();
				}
			}
		}

		public virtual void Rollback()
		{
			if (this.Transaction != null)
			{
				SqlConnection connection = this.Transaction.Connection;
				try
				{
					this.Transaction.Rollback();
					this.Transaction = null;
				}
				finally
				{
					connection.Close();
				}
			}
		}

		public bool FullTextQueriesEnable
		{
			get
			{
				object RetVal = SqlHelper.ExecuteScalar(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_FullTextQueriesEnable"));
				return ((int)RetVal) == 1;
			}
			set
			{
				if (value)
				{
					SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_FullTextQueriesActivate"));
					// Add all fields into Catalog [2/24/2005]
					SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_FullTextQueriesAddAllFields"));
				}
				else
					SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_FullTextQueriesDeactivate"));
			}
		}

		// Added File Data Storage Type [2/10/2005]
		public MetaFileDataStorageType MetaFileDataStorageType
		{
			get
			{
				return _metaFileDataStorageType;
			}
			set
			{
				_metaFileDataStorageType = value;
			}
		}

		public string LocalDiskStorage
		{
			get
			{
				return _localDiskFolder;
			}
			set
			{
				_localDiskFolder = value;
			}
		}
	}
}
