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
    /// RssChannel
    /// </summary>
    [Serializable]
    public class RssChannel
    {
        private List<RssCategory> _categories;
        private string _copyright;
        private string _description;
        private string _docs;
        private RssCloud _cloud;
        private string _generator;
        private RssImage _image;
        private List<RssItem> _items;
        private string _lastBuildDate;
        private string _link;
        private string _managingEditor;
        private string _pubDate;
        private string _rating;
        private string _timeToLive;
        private string _title;
        private RssSkipDays _skipDays;
        private RssSkipHours _skipHours;
        private string _webMaster;
        private string _language;
        private RssTextInput _textInput;

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
        /// Gets or sets the cloud.
        /// </summary>
        /// <value>The cloud.</value>
        [XmlElement("cloud")]
        public RssCloud Cloud
        {
            get 
            { 
                return _cloud; 
            }

            set 
            { 
                _cloud = value; 
            }
        }

        /// <summary>
        /// Gets or sets the copyright.
        /// </summary>
        /// <value>The copyright.</value>
        [XmlElement("copyright")]
        public string Copyright
        {
            get 
            { 
                return _copyright; 
            }

            set 
            { 
                _copyright = value;
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
        /// Gets or sets the docs.
        /// </summary>
        /// <value>The docs.</value>
        [XmlElement("docs")]
        public string Docs
        {
            get 
            { 
                return _docs; 
            }

            set 
            { 
                _docs = value; 
            }
        }

        /// <summary>
        /// Gets or sets the generator.
        /// </summary>
        /// <value>The generator.</value>
        [XmlElement("generator")]
        public string Generator
        {
            get 
            { 
                return _generator; 
            }

            set 
            { 
                _generator = value; 
            }
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), XmlElement("item")]
        public List<RssItem> Items
        {
            get
            { 
                return _items; 
            }

            set 
            { 
                _items = value; 
            }
        }

        /// <summary>
        /// Gets or sets the last build date.
        /// </summary>
        /// <value>The last build date.</value>
        [XmlElement("lastBuildDate")]
        public string LastBuildDate
        {
            get
            { 
                return _lastBuildDate; 
            }

            set 
            { 
                _lastBuildDate = value; 
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
        /// Gets or sets the managing editor.
        /// </summary>
        /// <value>The managing editor.</value>
        [XmlElement("managingEditor")]
        public string ManagingEditor
        {
            get 
            { 
                return _managingEditor; 
            }

            set 
            { 
                _managingEditor = value; 
            }
        }

        /// <summary>
        /// Gets or sets the time to live.
        /// </summary>
        /// <value>The time to live.</value>
        [XmlElement("ttl")]
        public string TimeToLive
        {
            get
            {
                return _timeToLive; 
            }

            set 
            { 
                _timeToLive = value; 
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
        /// Gets or sets the rating.
        /// </summary>
        /// <value>The rating.</value>
        [XmlElement("rating")]
        public string Rating
        {
            get 
            { 
                return _rating; 
            }

            set 
            { 
                _rating = value; 
            }
        }

        /// <summary>
        /// Gets or sets the skip days.
        /// </summary>
        /// <value>The skip days.</value>
        [XmlElement("skipDays")]
        public RssSkipDays SkipDays
        {
            get 
            { 
                return _skipDays; 
            }

            set 
            {
                _skipDays = value; 
            }
        }

        /// <summary>
        /// Gets or sets the skip hours.
        /// </summary>
        /// <value>The skip hours.</value>
        [XmlElement("skipHours")]
        public RssSkipHours SkipHours
        {
            get 
            {
                return _skipHours; 
            }

            set 
            { 
                _skipHours = value; 
            }
        }

        /// <summary>
        /// Gets or sets the web master.
        /// </summary>
        /// <value>The web master.</value>
        [XmlElement("webMaster")]
        public string WebMaster
        {
            get 
            { 
                return _webMaster; 
            }

            set 
            {
                _webMaster = value;
            }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        [XmlElement("image")]
        public RssImage Image
        {
            get
            {
                return _image; 
            }

            set 
            {
                _image = value; 
            }
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [XmlElement("language")]
        public string Language
        {
            get
            {
                return _language;
            }

            set
            {
                _language = value;
            }
        }

        /// <summary>
        /// Gets or sets the text input.
        /// </summary>
        /// <value>The text input.</value>
        [XmlElement("textInput")]
        public RssTextInput TextInput
        {
            get
            {
                return _textInput;
            }

            set
            {
                _textInput = value;
            }
        }
    }
}
