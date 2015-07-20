using System;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for ValidationHelper.
	/// </summary>
	class ValidationHelper
	{
		public static string[] EmptyArray;

		internal readonly static char[] InvalidMethodChars = new char[]{' ', '\r', '\n', '\t'};

		internal readonly static char[] InvalidParamChars = new char[]{'(', ')', '<', '>', '@', ',', ';', ':', '\\', '\"', '\'', '/', '[', ']', '?', '=', '{', '}', ' ', '\t', 
																		  '\r', '\n'};


		public static string[] MakeEmptyArrayNull(string[] stringArray)
		{
			if (stringArray == null || (int)stringArray.Length == 0)
			{
				return null;
			}
			else
			{
				return stringArray;
			}
		}

		public static string MakeStringNull(string stringValue)
		{
			if (stringValue == null || stringValue.Length == 0)
			{
				return null;
			}
			else
			{
				return stringValue;
			}
		}

		public static string MakeStringEmpty(string stringValue)
		{
			if (stringValue == null || stringValue.Length == 0)
			{
				return String.Empty;
			}
			else
			{
				return stringValue;
			}
		}

		public static int HashCode(object objectValue)
		{
			if (objectValue == null)
			{
				return -1;
			}
			else
			{
				return objectValue.GetHashCode();
			}
		}

		public static string ToString(object objectValue)
		{
			if (objectValue == null)
			{
				return "(null)";
			}
			if (objectValue is String && ((String)objectValue).Length == 0)
			{
				return "(string.empty)";
			}
			else
			{
				return objectValue.ToString();
			}
		}

		public static string HashString(object objectValue)
		{
			if (objectValue == null)
			{
				return "(null)";
			}
			if (objectValue is String && ((String)objectValue).Length == 0)
			{
				return "(string.empty)";
			}
			else
			{
				return objectValue.GetHashCode().ToString();
			}
		}

		public static bool IsInvalidHttpString(string stringValue)
		{
			if (stringValue.IndexOfAny(InvalidParamChars) != -1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool IsBlankString(string stringValue)
		{
			if (stringValue == null || stringValue.Length == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool ValidateUInt32(long address)
		{
			if (address >= (long)0)
			{
				return address > long.MaxValue == false;
			}
			else
			{
				return false;
			}
		}

		public static bool ValidateTcpPort(int port)
		{
			if (port >= 0)
			{
				return port > 65535 == false;
			}
			else
			{
				return false;
			}
		}

		public static bool ValidateRange(int actual, int fromAllowed, int toAllowed)
		{
			if (actual >= fromAllowed)
			{
				return actual > toAllowed == false;
			}
			else
			{
				return false;
			}
		}

		public static bool ValidateRange(long actual, long fromAllowed, long toAllowed)
		{
			if (actual >= fromAllowed)
			{
				return actual > toAllowed == false;
			}
			else
			{
				return false;
			}
		}

		static ValidationHelper()
		{
			EmptyArray = new string[0];
		}
	}
}
