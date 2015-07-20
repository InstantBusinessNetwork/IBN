using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Mediachase.Ibn.Data;
using System.Xml.Schema;

namespace Mediachase.Ibn.Lists.Mapping
{
	[Serializable]
    [XmlRoot(ElementName="mapping")]
    public class MappingElement : Collection<MappingRule>, IXmlSerializable
    {
        public static string classAttrName = "class";
        public static string tableAttrName = "table";
        public static string pkAttrName = "primaryKey";
       
        private string _mapClassName;
        private string _mapTableName;
        private string _primaryKeyName;

        public MappingElement()
        {
        }

        public MappingElement(string tableName, string className)
        {
            _mapClassName = className;
            _mapTableName = tableName;
        }
            
        public MappingElement(string tableName, string className, string primaryKeyName)
        {
            _mapClassName = className;
            _mapTableName = tableName;
            _primaryKeyName = primaryKeyName;
        }
    
        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
        public string ClassName
        {
            get { return _mapClassName; }
            set { _mapClassName = value; }
        }


        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName
        {
            get { return _mapTableName; }
            set { _mapTableName = value; }
        }


        /// <summary>
        /// Gets or sets the name of the PK col.
        /// </summary>
        /// <value>The name of the PK col.</value>
        public string PrimaryKeyName
        {
            get { return _primaryKeyName; }
            set { _primaryKeyName = value; }
        }

             
	
        #region IXmlSerizable methods
        /// <summary>
        /// Writes the XML.
        /// </summary>
        /// <param name="w">The w.</param>
        void IXmlSerializable.WriteXml(XmlWriter w)
        {
            XmlSerializer mapElser = new XmlSerializer(typeof(MappingRule));
      
            //w.WriteStartElement(TagName);
                      
            //write "class" attribute
            if (!string.IsNullOrEmpty(_mapClassName))
                w.WriteAttributeString(classAttrName, _mapClassName);
            //write "table" attribute
            if (!string.IsNullOrEmpty(_mapTableName))
                w.WriteAttributeString(tableAttrName, _mapTableName);
            //write "primaryKey" attribute
            if (!string.IsNullOrEmpty(_primaryKeyName))
                w.WriteAttributeString(pkAttrName, _primaryKeyName);
            
            foreach (MappingRule mapEl in this)
            {
               mapElser.Serialize(w, mapEl);
            }
            //w.WriteEndElement();
        }

        /// <summary>
        /// Reads the XML.
        /// </summary>
        /// <param name="r">The r.</param>
        void IXmlSerializable.ReadXml(XmlReader r)
        {
            XmlSerializer mapElser = new XmlSerializer(typeof(MappingRule));
                  

            if(r.HasAttributes)
                r.ReadAttributeValue();
                     
            _mapClassName = r.GetAttribute(classAttrName);
            _mapTableName = r.GetAttribute(tableAttrName);
            _primaryKeyName = r.GetAttribute(pkAttrName);

            r.Read();
            while (r.NodeType != XmlNodeType.EndElement)
            {

                MappingRule mapEl = (MappingRule)mapElser.Deserialize(r);
                this.Add(mapEl);

                r.MoveToContent();
            }

            r.ReadEndElement();
     
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <returns></returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }
        #endregion
   

    }
}
