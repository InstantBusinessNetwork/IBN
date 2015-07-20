using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Net.WebDavServer;
using System.Web;
using Mediachase.IBN.Business.WebDAV.Common;

namespace Mediachase.IBN.Business.WebDAV.Definition
{
    /// <summary>
    /// Реализациия WevDabRequest предназначенная для работы с виртуальными путями 
    /// </summary>
    internal class WebDavTicketRequest : IWebDavRequest
    {
        private ePluginToken _handlerType;
        private HttpRequest _innerRequest;

        public WebDavTicketRequest(HttpRequest request, ePluginToken handlerType)
        {
            _innerRequest = request;
            _handlerType = handlerType;
        }

        #region IWebDavRequest Members

        public string AbsolutePath
        {
            get { return GetAbsolutePath(InnerRequest.Url.AbsolutePath); }
        }

        public string ApplicationPath
        {
            get
            {
                return InnerRequest.ApplicationPath;
            }
        }

        public Encoding ContentEncoding
        {
            get { return InnerRequest.ContentEncoding; }
        }

        public int ContentLength
        {
            get { return InnerRequest.ContentLength; }
        }

        public string ContentType
        {
            get { return InnerRequest.ContentType; }
        }

        /// <summary>
        /// Gets the absolute path.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public string GetAbsolutePath(string url)
        {
            string retVal = url;
            try
            {
                WebDavTicket ticket = WebDavUrlBuilder.GetWebDavTicket(url);
                if (ticket != null)
                {
                    retVal = ticket.ToString();
                }
            }
            catch (System.Exception)
            {
                //throw new Exception("webdav ticket incorrect format");
            }

            return retVal;
        }

        public string GetUrl(string absolutePath)
        {
			string retVal = absolutePath;
			try
			{
				WebDavTicket ticket = WebDavTicket.Parse(absolutePath);
				retVal = WebDavUrlBuilder.GetWebDavUrl(ticket.AbsolutePath, true);
			}
			catch(Exception)
			{

			}

			return retVal;
            
        }

        public System.Collections.Specialized.NameValueCollection Headers
        {
            get { return InnerRequest.Headers; }
        }

        public string HttpMethod
        {
            get { return InnerRequest.HttpMethod; }
        }

        public System.IO.Stream InputStream
        {
            get { return InnerRequest.InputStream; }
        }

        public Uri Url
        {
            get { return InnerRequest.Url; }
        }

        #endregion

        protected  HttpRequest InnerRequest
        {
            get
            {
                return _innerRequest;
            }

            set
            {
                _innerRequest = value;
            }
        }

    }
}
