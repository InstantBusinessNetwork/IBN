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
    /// RssGuid
    /// </summary>
    [Serializable]
    public class RssGuid
    {
        private string _isPermaLink;
        private string _text;

        /// <summary>
        /// Gets or sets the is perma link.
        /// </summary>
        /// <value>The is perma link.</value>
        [XmlAttribute("isPermaLink")]
        public string IsPermaLink
        {
            get
            {
                return _isPermaLink; 
            }

            set
            {
                _isPermaLink = value; 
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [XmlText()]
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
            }
        }
    }
}
