using System;
using System.Collections.Generic;
using System.Text;

using Mediachase.Database;
using Mediachase.Ibn.Data;
using System.Data.SqlClient;

namespace Mediachase.Ibn.Converter
{
	internal class MetadataContext : IDisposable
	{
		private bool _commitCalled;
		private DataContext _previousContext; // write to DataContext.Current
		private DBTransaction _transaction; // commit, dispose
		private DataContext _currentContext; // dispose

		public MetadataContext(DBHelper dbHelper)
		{
			_previousContext = DataContext.Current;

			_transaction = dbHelper.BeginTransaction();

			_currentContext = new DataContext(string.Empty);
			_currentContext.SqlContext.CommandTimeout = dbHelper.CommandTimeout;
			_currentContext.SqlContext.Transaction = _transaction.SqlTran;

			DataContext.Current = _currentContext;
		}

		public void Commit()
		{
			if (!_commitCalled)
			{
				_commitCalled = true;
				if (_transaction != null)
				{
					_transaction.Commit();
				}
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool freeManagedResources)
		{
			if (freeManagedResources)
			{
				// free managed resources

				if (_currentContext != null)
				{
					_currentContext.SqlContext.Transaction = null;
					_currentContext.Dispose();
					_currentContext = null;
				}
				DataContext.Current = _previousContext;

				if (_transaction != null)
				{
					_transaction.Dispose();
					_transaction = null;
				}
			}

			// free native resources if there are any.
		}

		#endregion
	}
}
