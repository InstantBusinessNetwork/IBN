/*=======================================================================
  Copyright (C) Microsoft Corporation.  All rights reserved.
 
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
=======================================================================*/

using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RssToolkit.Rss 
{
    /// <summary>
    /// RssDocument
    /// </summary>
    [Serializable()]
    [XmlRoot("rss")]
    public class RssDocument : RssDocumentBase
    {
        private string _version;
        private RssChannel _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RssDocument"/> class.
        /// </summary>
        public RssDocument()
        {
        }

        #region "Properties"
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [XmlAttribute("version")]
        public new string Version
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
        /// Gets or sets the channel.
        /// </summary>
        /// <value>The channel.</value>
        [XmlElement("channel")]
        public new RssChannel Channel
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
        #endregion

        /// <summary>
        /// Loads from XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>RssDocument</returns>
        public static RssDocument Load(string xml)
        {
            return RssDocumentBase.Load<RssDocument>(xml);
        }

        /// <summary>
        /// Loads the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>RssDocument</returns>
        public static RssDocument Load(System.Uri url)
        {
            return RssDocumentBase.Load<RssDocument>(url);
        }

        /// <summary>
        /// Loads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>RssDocument</returns>
        public static RssDocument Load(XmlReader reader)
        {
            return RssDocumentBase.Load<RssDocument>(reader);
        }
        
        /// <summary>
        /// Coverts to DataSet
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet ToDataSet() 
        {
            return RssXmlHelper.ToDataSet(ToXml(DocumentType.Rss));
        }

        /// <summary>
        /// Select method for programmatic databinding
        /// </summary>
        /// <returns>IEnumerable</returns>
        public IEnumerable SelectItems()
        {
            return SelectItems(-1);
        }

        /// <summary>
        /// Selects the items.
        /// </summary>
        /// <param name="maxItems">The max items.</param>
        /// <returns>IEnumerable</returns>
        public IEnumerable SelectItems(int maxItems)
        {
            return SelectItems(maxItems, false);
        }

        /// <summary>
        /// Selects the items.
        /// </summary>
        /// <param name="maxItems">The max items.</param>
        /// <param name="reverseOrder">if set to <c>true</c> [reverse order].</param>
        /// <returns>IEnumerable</returns>
        public IEnumerable SelectItems(int maxItems, bool reverseOrder)
        {
            DataSet dataset = this.ToDataSet();
            DataView view = dataset.Tables["item"].DefaultView;

            if (reverseOrder)
            {
                if (view.Table.Columns.Contains("pubDateParsed"))
                {
                    view.Sort = "pubDateParsed asc";
                }
            }

            if (maxItems > 0)
            {
                DataTable clonedTable = view.Table.Clone();
                for (int counter = 0; counter <= maxItems - 1; counter++)
                {
                    if (counter >= view.Count)
                    {
                        break;
                    }

                    clonedTable.ImportRow(view[counter].Row);
                }

                return new DataView(clonedTable, view.RowFilter, view.Sort, view.RowStateFilter);
            }
            else
            {
                return view;
            }
        }

        /// <summary>
        /// Returns Xml in the type specified by outputType
        /// </summary>
        /// <param name="ouputType">Type of the output.</param>
        /// <returns></returns>
        public override string ToXml(DocumentType outputType)
        {
            return RssDocumentBase.ToXml<RssDocument>(outputType, this);
        }
    }
}
