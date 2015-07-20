using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Mediachase.Sync.Core.ErrorManagement
{
    [XmlType(Namespace = "urn:DataObjects")]
    [XmlRoot(Namespace = "urn:DataObjects")]
    [Serializable]
    public class SyncronizationServiceError
    {
		public enum eServiceErrorType
		{
			Undef,
			SyncFramework,
			SyncProvider,
			ProviderNotSpecified,
			AuthFailed,
			NotAuthRequest,
			ServerError
		}

		public string name { get; set; }
		public string message { get; set; }
		public string stackTrace { get; set; }
		public eServiceErrorType errorType {get; set;} 

        public SyncronizationServiceError() 
		{
			name = string.Empty;
			message = string.Empty;
			errorType = eServiceErrorType.Undef;
			stackTrace = string.Empty;
		}
		public SyncronizationServiceError(eServiceErrorType errType, Exception ex)
			: base()
		{
			errorType = errType;
			message = ex.Message;
			stackTrace = ex.StackTrace;
			name = ex.ToString();
		}

		public SyncronizationServiceError(eServiceErrorType errType, string errorMessage)
			: base()
		{
			errorType = errType;
			message = errorMessage;
		}
        
        public SyncronizationServiceError(string methodName,string errorMessage)
			: base()
        {
           name = methodName;
           message = errorMessage;
        }
        
      
        
    }
}
