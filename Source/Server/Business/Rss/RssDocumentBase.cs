/*=======================================================================
  Copyright (C) Microsoft Corporation.  All rights reserved.
 
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
=======================================================================*/

using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RssToolkit.Rss
{
    /// <summary>
    /// Abstract class for RSS Document
    /// </summary>
    public abstract class RssDocumentBase
    {
        private string _url;
        private string _version;
        private RssChannel _channel;

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [XmlAttribute("version")]
        public string Version
        {
            get
            {
                return _version;
            }

            set
            {
                _version = value;
            }
        }

        /// <summary>
        /// Channel
        /// </summary>
        [XmlIgnore()]
        public RssChannel Channel
        {
            get
            {
                return _channel;
            }

            set
            {
                _channel = value;
            }
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        internal string Url
        {
            get
            {
                return _url;
            }

            set
            {
                _url = value;
            }
        }

        /// <summary>
        /// Returns Xml in the type specified by outputType
        /// </summary>
        /// <param name="outputType">Type of the output.</param>
        /// <returns></returns>
        public virtual string ToXml(DocumentType outputType)
        {
            return string.Empty;
        }

        /// <summary>
        /// string XML representation.
        /// </summary>
        /// <returns>string</returns>
        protected static string ToXml<T>(DocumentType outputType, T rssDocument) where T : RssDocumentBase
        {
            string rssXml = RssXmlHelper.ToRssXml<T>(rssDocument);

            return RssXmlHelper.ConvertRssTo(outputType, rssXml);
        }

        /// <summary>
        /// Loads the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>RssDocument</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        protected static T Load<T>(System.Uri url) where T : RssDocumentBase
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            // resolve app-relative URLs
            string rssUrl = RssXmlHelper.ResolveAppRelativeLinkToUrl(url.ToString());

            // download the feed
            using (Stream cachedXml = DownloadManager.GetFeed(rssUrl))
            {
                using (XmlTextReader reader = new XmlTextReader(cachedXml))
                {
                    T rss = Load<T>(reader);

                    //// remember the url
                    rss.Url = rssUrl;

                    return rss;
                }
            }
        }

        /// <summary>
        /// Loads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Generic Type of RssDocumentBase</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        protected static T Load<T>(XmlReader reader) where T : RssDocumentBase
        {
            T rss = null;
            string rssXml = RssXmlHelper.ConvertToRssXml(reader);

            if (!string.IsNullOrEmpty(rssXml))
            {
                rss = LoadFromRssXml<T>(rssXml);
            }

            return rss;
        }

        /// <summary>
        /// Loads from XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Generic Type of RssDocumentBase</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        protected static T Load<T>(string xml) where T : RssDocumentBase
        {
            T rssDocument = null;
            using (StringReader stringReader = new StringReader(xml))
            {
                using (XmlTextReader reader = new XmlTextReader(stringReader))
                {
                    rssDocument = Load<T>(reader);
                }
            }

            return rssDocument;
        }

        private static T LoadFromRssXml<T>(string rssXml) where T : RssDocumentBase
        {
            if (string.IsNullOrEmpty(rssXml))
            {
                throw new ArgumentException("xml");
            }

            T rss = RssXmlHelper.DeserializeFromXmlUsingStringReader<T>(rssXml);

            return rss;
        }
    }
}
