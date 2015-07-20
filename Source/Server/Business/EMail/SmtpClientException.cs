using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.EMail
{
    [global::System.Serializable]
    public class SmtpClientException : Exception
    {
        public SmtpClientException() { }
        public SmtpClientException(string message) : base(message) { }
        public SmtpClientException(string message, Exception inner) : base(message, inner) { }
        protected SmtpClientException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
