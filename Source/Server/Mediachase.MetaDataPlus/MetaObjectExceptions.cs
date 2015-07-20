using System;
using System.Runtime.Serialization;

namespace Mediachase.MetaDataPlus
{
	/// <summary>
	/// Summary description for MetaDataPlusException.
	/// </summary>
	[Serializable]
	public class MetaDataPlusException : Exception
	{
		public MetaDataPlusException()
			: base()
		{
		}

		public MetaDataPlusException(string message)
			: base(message)
		{
		}

		public MetaDataPlusException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected MetaDataPlusException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	/// <summary>
	/// Summary description for DeletedObjectInaccessibleException.
	/// </summary>
	[Serializable]
	public class DeletedObjectInaccessibleException : MetaDataPlusException
	{
		public DeletedObjectInaccessibleException()
			: base()
		{
		}

		public DeletedObjectInaccessibleException(string message)
			: base(message)
		{
		}

		public DeletedObjectInaccessibleException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DeletedObjectInaccessibleException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	/// <summary>
	/// Summary description for IncorectValueTypeException.
	/// </summary>
	[Serializable]
	public class IncorectValueTypeException : MetaDataPlusException
	{
		public IncorectValueTypeException()
			: base()
		{
		}

		public IncorectValueTypeException(string message)
			: base(message)
		{
		}

		public IncorectValueTypeException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected IncorectValueTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	/// <summary>
	/// Summary description for IncorectDictionaryItemException.
	/// </summary>
	[Serializable]
	public class IncorectDictionaryItemException : MetaDataPlusException
	{
		public IncorectDictionaryItemException()
			: base()
		{
		}

		public IncorectDictionaryItemException(string message)
			: base(message)
		{
		}

		public IncorectDictionaryItemException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected IncorectDictionaryItemException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

}
