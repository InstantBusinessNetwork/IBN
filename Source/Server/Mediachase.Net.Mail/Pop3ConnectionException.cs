using System;

namespace Mediachase.Net.Mail
{
	/// <summary>
	/// Summary description for Pop3Exception.
	/// </summary>
	public class Pop3Exception: Exception
	{
		public Pop3Exception()
		{
		}
		public Pop3Exception(string message):base(message)
		{
		}
		public Pop3Exception(string message, Exception innerException):base(message,innerException)
		{
		}
	}

	public class Pop3ServerReturnErrorException: Pop3Exception
	{
		public Pop3ServerReturnErrorException()
		{
		}
		public Pop3ServerReturnErrorException(string message):base(message)
		{
		}
		public Pop3ServerReturnErrorException(string message, Exception innerException):base(message,innerException)
		{
		}
	}

	public class Pop3ServerIncorectAnswerException: Pop3Exception
	{
		public Pop3ServerIncorectAnswerException()
		{
		}
		public Pop3ServerIncorectAnswerException(string message):base(message)
		{
		}
		public Pop3ServerIncorectAnswerException(string message, Exception innerException):base(message,innerException)
		{
		}
	}

	public class Pop3SendDataErrorException: Pop3Exception
	{
		public Pop3SendDataErrorException()
		{
		}
		public Pop3SendDataErrorException(string message):base(message)
		{
		}
		public Pop3SendDataErrorException(string message, Exception innerException):base(message,innerException)
		{
		}
	}

	public class Pop3ServerIncorectEMailFormatException: Pop3Exception
	{
		public Pop3ServerIncorectEMailFormatException()
		{
		}
		public Pop3ServerIncorectEMailFormatException(string message):base(message)
		{
		}
		public Pop3ServerIncorectEMailFormatException(string message, Exception innerException):base(message,innerException)
		{
		}
	}

}

