/*=======================================================================
  Copyright (C) Microsoft Corporation.  All rights reserved.
 
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
=======================================================================*/

using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace RssToolkit.Rss
{
    /// <summary>
    /// Validates the Xml Schema
    /// </summary>
    public class RssXmlSchemaValidator
    {
        private bool isValidXml = true;
        private string validationError = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RssXmlSchemaValidator"/> class.
        /// </summary>
        public RssXmlSchemaValidator()
        {   
        }

        /// <summary>
        /// Gets the validation error.
        /// </summary>
        /// <value>The validation error.</value>
        public String ValidationError
        {
            get
            {
                return this.validationError;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid XML.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is valid XML; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidXml
        {
            get
            {
                return this.isValidXml;
            }
        }

        /// <summary>
        /// Valids the Xml doc.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="xsd">The XSD.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void ValidXmlDoc(string xml, XmlReader xsd)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("xml");
            }

            if (xsd == null)
            {
                throw new ArgumentNullException("xsd");
            }
            
            try 
            {
                XmlSchemaSet sc = new XmlSchemaSet();
                sc.Add(null, xsd);
                sc.Compile();

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas = sc;
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                XmlReader reader;
                using (StringReader stringReader = new StringReader(xml))
                {
                    reader = XmlReader.Create(stringReader, settings);
                    while (reader.Read());
                }
            } 
            catch (Exception ex)
            {
                this.validationError = ex.Message;
                isValidXml = false; 
            } 
        }

        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            // The xml does not match the schema.
            isValidXml = false;
            this.validationError = args.Message;
        }
    }
}