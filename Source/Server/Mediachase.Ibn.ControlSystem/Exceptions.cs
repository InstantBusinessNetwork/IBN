using System;

namespace Mediachase.Ibn.ControlSystem
{
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
}
