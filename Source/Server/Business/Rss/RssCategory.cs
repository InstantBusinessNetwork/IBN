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
using System.Xml;
using System.Xml.Serialization;

namespace RssToolkit.Rss
{
    /// <summary>
    /// RssCategory
    /// </summary>
    [Serializable]
    public class RssCategory
    {
        private string _domain;
        private string _text;

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        [XmlAttribute("domain")]
        public string Domain
        {
            get 
            { 
                return _domain; 
            }

            set 
            {
                _domain = value; 
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
