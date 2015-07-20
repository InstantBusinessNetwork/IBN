using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;


namespace Mediachase.Ibn.Lists
{
	[Serializable]
	[XmlRoot(ElementName = "lists")]
	public class ListCollection : List<MetaObject>, IXmlSerializable
	{

		#region IXmlSerializable Members

		/// <summary>
		/// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
		/// </returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
		public void ReadXml(System.Xml.XmlReader reader)
		{

			throw new NotImplementedException();
			//XmlSerializer xmlser = new XmlSerializer(typeof(MetaObject));

			//reader.Read();
			//while (reader.NodeType != XmlNodeType.EndElement)
			//{
			//    MetaObject moList = (MetaObject)xmlser.Deserialize(reader);
			//    this.Add(moList);

			//    reader.MoveToContent();
			//}

			//reader.ReadEndElement();

		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			//XmlSerializer xmlser = new XmlSerializer(typeof(MetaObject));

			foreach (MetaObject moList in this)
			{
				writer.WriteStartElement("list");
				foreach (MetaField mf in moList.GetMetaType().Fields)
				{
					if (mf.InPrimaryKey)
						continue;

					object value = moList.Properties[mf.Name].Value;

					// OZ [2008-04-09] Fix Problem with Files
					FileInfo fileInfo = value as FileInfo;
					if (mf.GetMetaType().McDataType == McDataType.File && fileInfo != null)
					{
						value = fileInfo.Name;
					}

					// OZ [2008-04-09] Fix Problem with GUid
					if (mf.GetMetaType().McDataType == McDataType.Guid && value is Guid)
					{
						value = ((Guid)value).ToString();
					}

					// OZ [2008-10-30] Fix Problem with PrimaryKeyId
					if (value is PrimaryKeyId)
					{
						value = ((PrimaryKeyId)value).ToString();
					}

					writer.WriteStartElement(mf.Name);
					if (value != null)
						writer.WriteValue(value);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
				//xmlser.Serialize(writer, moList);
			}
		}

		#endregion
	}
}
