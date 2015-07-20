using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Mediachase.Ibn.Data;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace Mediachase.Ibn.Lists.Mapping
{
	[Serializable]
    [XmlRoot(ElementName="mappingDocument")]
    public class MappingDocument : Collection<MappingElement>, IXmlSerializable
    {
        public static string TagName = "mappingDocument";

        /// <summary>
        /// Loads from XML.
        /// </summary>
        /// <param name="srcXml">The SRC XML.</param>
        /// <returns></returns>
        public static MappingDocument LoadFromXml(string srcXml)
        {
            MappingDocument retVal = (MappingDocument)
                                       McXmlSerializer.GetObject<MappingDocument>(srcXml);

            return retVal;

        }

        /// <summary>
        /// Saves to XML.
        /// </summary>
        /// <param name="mapDoc">The map doc.</param>
        /// <returns></returns>
        public static string GetXml(MappingDocument mapDoc)
        {
            String retVal = McXmlSerializer.GetString<MappingDocument>(mapDoc);

            return retVal;
        }


        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static MappingDocument LoadFromFile(String path)
        {
            MappingDocument retVal = (MappingDocument)
                                         McXmlSerializer.GetObjectFromFile<MappingDocument>(path);

            return retVal;

        }

        /// <summary>
        /// Saves to file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="mapDoc">The map doc.</param>
        public static void SaveToFile(String path, MappingDocument mapDoc)
        {
             McXmlSerializer.SaveObjectToFile<MappingDocument>(path, mapDoc);
            
        }

        #region IXmlSerizable methods
        /// <summary>
        /// Writes the XML.
        /// </summary>
        /// <param name="w">The w.</param>
        void IXmlSerializable.WriteXml(XmlWriter w)
        {
            XmlSerializer mapElser = new XmlSerializer(typeof(MappingElement));

            //w.WriteStartElement(TagName);
            foreach (MappingElement mapElColl in this)
            {
                mapElser.Serialize(w, mapElColl);
            }
            //w.WriteEndElement();
        }

        /// <summary>
        /// Reads the XML.
        /// </summary>
        /// <param name="r">The r.</param>
        void IXmlSerializable.ReadXml(XmlReader r)
        {
            XmlSerializer mapElser = new XmlSerializer(typeof(MappingElement));

            //r.Read();
            r.ReadStartElement(TagName);

            while (r.NodeType != XmlNodeType.EndElement && 
                   r.NodeType != XmlNodeType.None)
            {

                MappingElement mapElColl = (MappingElement)mapElser.Deserialize(r);
                this.Add(mapElColl);

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
