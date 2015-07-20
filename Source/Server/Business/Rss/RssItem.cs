/*=======================================================================
  Copyright (C) Microsoft Corporation.  All rights reserved.

  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
=======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace RssToolkit.Rss
{
    /// <summary>
    /// RssItem
    /// </summary>
    [Serializable]
    public class RssItem
    {
        private string _author;
        private List<RssCategory> _categories;
        private string _comments;
        private string _description;
        private RssEnclosure _enclosure;
        private RssGuid _guid;
        private string _link;
        private string _pubDate;
        private string _title;
        private RssSource _source;

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        [XmlElement("author")]
        public string Author
        {
            get 
            { 
                return _author; 
            }

            set 
            { 
                _author = value; 
            }
        }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        /// <value>The categories.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), XmlElement("category")]
        public List<RssCategory> Categories
        {
            get
            {
                return _categories;
            }

            set
            {
                _categories = value;
            }
        }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>The comments.</value>
        [XmlElement("comments")]
        public string Comments
        {
            get
            {
                return _comments;
            }

            set
            {
                _comments = value;
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement("description")]
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// Gets or sets the enclosure.
        /// </summary>
        /// <value>The enclosure.</value>
        [XmlElement("enclosure")]
        public RssEnclosure Enclosure
        {
            get
            {
                return _enclosure;
            }

            set
            {
                _enclosure = value;
            }
        }

        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        [XmlElement("guid")]
        public RssGuid Guid
        {
            get
            {
                return _guid;
            }

            set
            {
                _guid = value;
            }
        }

        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        /// <value>The link.</value>
        [XmlElement("link")]
        public string Link
        {
            get
            {
                return _link;
            }

            set
            {
                _link = value;
            }
        }

        /// <summary>
        /// Gets or sets the pub date.
        /// </summary>
        /// <value>The pub date.</value>
        [XmlElement("pubDate")]
        public string PubDate
        {
            get
            {
                return _pubDate;
            }

            set
            {
                _pubDate = value;
            }   
        }

        /// <summary>
        /// Gets the parsed pub date.
        /// </summary>
        /// <value>The parsed pub date.</value>
        [XmlElement("pubDateParsed")]
        public System.DateTime PubDateParsed
        {
            get
            {
                return RssXmlHelper.Parse(_pubDate);
            }

            set
            {
                _pubDate = RssXmlHelper.ToRfc822(value);
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [XmlElement("title")]
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [XmlElement("source")]
        public RssSource Source
        {
            get
            {
                return _source;
            }

            set
            {
                _source = value;
            }
        }
    }
}
