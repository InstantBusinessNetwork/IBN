using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.EMail
{
    [Serializable]
    public class EMailOutgoingMessage
    {
        private string _htmlBody = string.Empty;
      private List<object> _tos = new List<object>();
      private List<int> _attachs = new List<int>();

        /// <summary>
        /// Gets or sets the HTML body.
        /// </summary>
        /// <value>The HTML body.</value>
        public string HtmlBody
        {
            get { return _htmlBody; }
            set { _htmlBody = value; }
        }

        /// <summary>
        /// Gets the recipients. String -> Email or int -> Ibn UserId
        /// </summary>
        /// <value>The recipients.</value>
        public List<object> Recipients
        {
            get { return _tos; }
        }

        /// <summary>
        /// Gets the attachments.
        /// </summary>
        /// <value>The attachments.</value>
        public List<int> Attachments
        {
            get { return _attachs; }
        }


    }
}
