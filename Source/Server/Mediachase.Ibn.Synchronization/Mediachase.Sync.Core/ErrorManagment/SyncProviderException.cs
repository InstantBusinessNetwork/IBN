using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Synchronization;

namespace Mediachase.Sync.Core.ErrorManagment
{
	public class SyncProviderException : Exception
	{
		public SaveChangeAction SaveChangeAction {get; set;}

		public SyncProviderException()
			: base()
		{
		}
		public SyncProviderException(string message)
			:base(message)
		{
		}

		public SyncProviderException(string message, Exception innerException)
			:base(message, innerException)
		{
		}

		public SyncProviderException(Exception innerException, SaveChangeAction saveChangeAction)
			: base(saveChangeAction.ToString(), innerException)
		{
			SaveChangeAction = saveChangeAction;
		}

		public SyncProviderException(string message, Exception innerException, SaveChangeAction saveChangeAction)
			:this(message, innerException)
		{
			SaveChangeAction = saveChangeAction;
		}

		public SyncProviderException(string message, SaveChangeAction saveChangeAction)
			: base(message)
		{
			SaveChangeAction = saveChangeAction;
		}
		
	}
}
