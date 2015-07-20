using System;
using System.Runtime.Serialization;

namespace Mediachase.IBN.Business
{
	[Serializable]
	public class UnableMoveTask : System.Exception
	{
		public UnableMoveTask()
			: this(null)
		{
		}
		public UnableMoveTask(Exception  innerException)
			: base("Unable move task.", innerException)
		{
		}
	}

	[Serializable]
	public class FeatureNotAvailable : System.Exception
	{
		public FeatureNotAvailable()
			: this(null)
		{
		}
		public FeatureNotAvailable(Exception  innerException)
			: base("Feature not available.", innerException)
		{
		}
	}

/*	[Serializable]
	public class AccessDeniedException : System.Exception
	{
		public AccessDeniedException()
			: this(null)
		{
		}
		public AccessDeniedException(Exception  innerException)
			: base("Access denied.", innerException)
		{
		}
	}
*/
	[Serializable]
	public class InvalidFileException : System.Exception
	{
		public InvalidFileException()
			: this(null)
		{
		}
		public InvalidFileException(Exception  innerException)
			: base("Invalid file ID.", innerException)
		{
		}
	}

	[Serializable]
	public class NoDefaultContentTypeException : System.Exception
	{
		public NoDefaultContentTypeException()
			: this(null)
		{
		}
		public NoDefaultContentTypeException(Exception  innerException)
			: base("Default content type is not defined.", innerException)
		{
		}
	}

	[Serializable]
	public class LoginDuplicationException : System.Exception
	{
		public LoginDuplicationException()
			: this(null)
		{
		}
		public LoginDuplicationException(Exception  innerException)
			: base("Login duplication.", innerException)
		{
		}
	}

	[Serializable]
	public class EmailDuplicationException : System.Exception
	{
		public EmailDuplicationException()
			: this(null)
		{
		}
		public EmailDuplicationException(Exception  innerException)
			: base("Email duplication.", innerException)
		{
		}
	}

	[Serializable]
	public class PasswordRequiredException : System.Exception
	{
		public PasswordRequiredException()
			: this(null)
		{
		}
		public PasswordRequiredException(Exception  innerException)
			: base("Password is required.", innerException)
		{
		}
	}

	[Serializable]
	public class InvalidIntervalException : System.Exception
	{
		public InvalidIntervalException()
			: this(null)
		{
		}
		public InvalidIntervalException(Exception  innerException)
			: base("Invalid interval.", innerException)
		{
		}
	}

	[Serializable]
	public class WrongDataException : System.Exception
	{
		public WrongDataException()
			: this(null)
		{
		}
		public WrongDataException(Exception  innerException)
			: base("Wrong data.", innerException)
		{
		}
	}

	[Serializable]
	public class EmptyCalendarException : System.Exception
	{
		public EmptyCalendarException()
			: this(null)
		{
		}
		public EmptyCalendarException(Exception  innerException)
			: base("Calendar can't be empty.", innerException)
		{
		}
	}

	[Serializable]
	public class MaxDiskSpaceException : System.Exception
	{
		public MaxDiskSpaceException()
			: this(null)
		{
		}
		public MaxDiskSpaceException(Exception  innerException)
			: base("Max Disk Space is Reached.", innerException)
		{
		}
	}

	[Serializable]
	public class MaxUsersCountException : System.Exception
	{
		public MaxUsersCountException()
			: this(null)
		{
		}
		public MaxUsersCountException(Exception  innerException)
			: base("Max Users count is reached.", innerException)
		{
		}
	}

	[Serializable]
	public class InvalidAccountException: System.Exception
	{
		public InvalidAccountException()
			: this(null)
		{
		}
		public InvalidAccountException(Exception  innerException)
			: base("Invalid Account.", innerException)
		{
		}
	}

	[Serializable]
	public class InvalidPasswordException: System.Exception
	{
		public InvalidPasswordException()
			: this(null)
		{
		}
		public InvalidPasswordException(Exception  innerException)
			: base("Invalid Password.", innerException)
		{
		}
	}

	[Serializable]
	public class NotActiveAccountException: System.Exception
	{
		public NotActiveAccountException()
			: this(null)
		{
		}
		public NotActiveAccountException(Exception  innerException)
			: base("The account is not active.", innerException)
		{
		}
	}

	[Serializable]
	public class ExternalOrPendingAccountException: System.Exception
	{
		public ExternalOrPendingAccountException()
			: this(null)
		{
		}
		public ExternalOrPendingAccountException(Exception  innerException)
			: base("The account is external or pending.", innerException)
		{
		}
	}

	[Serializable]
	public class WrongLoginException: System.Exception
	{
		public WrongLoginException()
			: this(null)
		{
		}
		public WrongLoginException(Exception  innerException)
			: base("Wrong Login.", innerException)
		{
		}
	}

	[Serializable]
	public class AccessWillBeDeniedException : System.Exception
	{
		public AccessWillBeDeniedException()
			: this(null)
		{
		}
		public AccessWillBeDeniedException(Exception  innerException)
			: base("Access will be denied.", innerException)
		{
		}

		public AccessWillBeDeniedException(string Message, Exception  innerException)
			: base(Message, innerException)
		{
		}
	}

	[Serializable]
	public class AllUserAccessWillBeDeniedException : AccessWillBeDeniedException
	{
		public AllUserAccessWillBeDeniedException()
			: this(null)
		{
		}
		public AllUserAccessWillBeDeniedException(Exception  innerException)
			: base("All user access will be denied.", innerException)
		{
		}
	}

	[Serializable]
	public class AdminAccessWillBeDeniedException : AccessWillBeDeniedException
	{
		public AdminAccessWillBeDeniedException()
			: this(null)
		{
		}
		public AdminAccessWillBeDeniedException(Exception  innerException)
			: base("Current User Admin access will be denied.", innerException)
		{
		}
	}

	[Serializable]
	public class NotExistingIdException : System.Exception
	{
		public NotExistingIdException()
			: this(null)
		{
		}
		public NotExistingIdException(Exception  innerException)
			: base("The item with that ID doesn't exist.", innerException)
		{
		}
	}

	[Serializable]
	public class UnsupportedSqlServerVersionException : Exception
	{
		public UnsupportedSqlServerVersionException()
			: base("SQL Server 2000 SP3a or later is required.")
		{
		}

		public UnsupportedSqlServerVersionException(string message)
			: base(message)
		{
		}

		public UnsupportedSqlServerVersionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected UnsupportedSqlServerVersionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	[Serializable]
	public class FoundDifferenceSynchronizedProjectException : System.Exception
	{
		public FoundDifferenceSynchronizedProjectException()
			: this(null)
		{
		}
		public FoundDifferenceSynchronizedProjectException(Exception innerException)
			: base("Found difference synchronized project!", innerException)
		{
		}
	}

	[Serializable]
	public class InvalidTicketException : System.Exception
	{
		public InvalidTicketException()
			: this(null)
		{
		}
		public InvalidTicketException(Exception innerException)
			: base("Invalid ticket or ticked has expired.", innerException)
		{
		}
	}

	[Serializable]
	public class DatabaseStateException : Exception, ISerializable
	{
		private DatabaseState _state;

		public DatabaseState State
		{
			get { return _state; }
		}

		public DatabaseStateException(DatabaseState state)
			: base()
		{
			_state = state;
		}

		public DatabaseStateException()
			: base()
		{
		}

		public DatabaseStateException(string message)
			: base(message)
		{
		}

		public DatabaseStateException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DatabaseStateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_state = (DatabaseState)info.GetValue("State", typeof(DatabaseState));
		}

		#region ISerializable Members

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
			throw new ArgumentNullException("info");

		base.GetObjectData(info, context);
		info.AddValue("State", _state, typeof(DatabaseState));
	}

		#endregion
	}
}
