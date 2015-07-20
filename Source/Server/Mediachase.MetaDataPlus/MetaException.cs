using System;
using System.Data.SqlClient;

namespace Mediachase.MetaDataPlus
{
	/// <summary>
	/// Summary description for MetaException.
	/// </summary>
	public class MetaException : Exception
	{
		public MetaException()
		{
		}

		public MetaException(string message)
			: base(message)
		{
		}

		public MetaException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public MetaException(string format, params object[] args)
			: base(string.Format(format, args))
		{
		}

		public MetaException(string format, Exception innerException, params object[] args)
			: base(string.Format(format, args), innerException)
		{
		}

		public SqlException InnerSqlException
		{
			get
			{
				return this.InnerException as SqlException;
			}
		}

	}
}
