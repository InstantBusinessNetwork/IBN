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
    /// RssSkipDays
    /// </summary>
    [Serializable]
    public class RssSkipDays
    {
        private List<string> _days;

        /// <summary>
        /// Gets or sets the days.
        /// </summary>
        /// <value>The days.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), XmlElement("day")]
        public List<string> Days
        {
            get 
            { 
                return _days; 
            }

            set 
            { 
                _days = value; 
            }
        }
    }
}
