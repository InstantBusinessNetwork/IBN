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
    /// RssImage
    /// </summary>
    [Serializable]
    public class RssImage
    {
        private string _description;
        private string _height;
        private string _link;
        private string _title;
        private string _url;
        private string _width;

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
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [XmlElement("height")]
        public string Height
        {
            get 
            { 
                return _height; 
            }

            set
            { 
                _height = value; 
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
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings"), XmlElement("url")]
        public string Url
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
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [XmlElement("width")]
        public string Width
        {
            get 
            { 
                return _width; 
            }

            set 
            { 
                _width = value; 
            }
        }
    }
}
