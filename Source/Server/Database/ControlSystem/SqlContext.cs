using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database.ControlSystem
{
	/// <summary>
	/// Represents the sql connection information, either valid connection string for a SqlConnection or SqlTransaction.
	/// </summary>
	public class SqlContext: IDisposable
	{
		private	string					_ConnectionString	=	string.Empty;

		// Fixed : One Thread One SqlTransaction  [2/10/2005]
		private	SqlTransaction			_CurrentTransaction	=	null;

		/// <summary>
		/// Initializes a new instance of the SqlContext.
		/// </summary>
		public SqlContext()
		{
		}

		/// <summary>
		/// Initializes a new instance of the SqlContext class with valid connection string for a SqlConnection.
		/// </summary>
		/// <param name="ConnectionString"></param>
		public SqlContext(string ConnectionString)
		{
			this.ConnectionString = ConnectionString;
		}

		/// <summary>
		/// Gets or sets the connection string for a SqlConnection.
		/// </summary>
		public virtual string ConnectionString
		{
			get
			{
				return _ConnectionString;
			}
			set
			{
				_ConnectionString = value;
			}
		}

		/// <summary>
		/// Gets or sets the SqlTransaction.
		/// </summary>
		public virtual SqlTransaction Transaction
		{
			get
			{
				return _CurrentTransaction;
			}
			set
			{
				//_OwnerShip = SqlTransactionOwnership.External;
				_CurrentTransaction = value;
			}
		}


		/// <summary>
		/// Begins a new database transaction.
		/// </summary>
		public virtual void BeginTransaction()
		{
			SqlConnection myConnection = new SqlConnection(this.ConnectionString);
			myConnection.Open();

			this.Transaction = myConnection.BeginTransaction();
		}

		/// <summary>
		/// Commits the database transaction.
		/// </summary>
		public virtual void Commit()
		{
			if(this.Transaction!=null)
			{
				SqlConnection	connection = this.Transaction.Connection;
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

		/// <summary>
		/// Roll back the database transaction from a pending state.
		/// </summary>
		public virtual void Rollback()
		{
			if(this.Transaction!=null)
			{
				SqlConnection	connection = this.Transaction.Connection;
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
		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Rollback();
		}

		#endregion
	}
}
