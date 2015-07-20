using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.Hosting;
using System.Xml.Serialization;

namespace Mediachase.IBN.Business.WidgetEngine
{
	public class XmlControlPropertiesProvider : ControlPropertiesBase
	{
		private const string _fileName = "ControlPropertiesData.xml";
		private const string _baseTagName = "control";
		private static XmlDocument xmlDoc;

		static XmlControlPropertiesProvider()
		{
			xmlDoc = new XmlDocument();
			xmlDoc.Load(HostingEnvironment.ApplicationPhysicalPath + _fileName);
		}

		#region GetValue
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="cUid">The c uid.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public override object GetValue(string cUid, string key)
		{
			String userKey = String.Format("{0}__{1}", key, Security.CurrentUser.UserID);

			foreach (XmlNode node in xmlDoc.DocumentElement.SelectNodes(_baseTagName))
			{
				if (node.Attributes["id"] != null && node.Attributes["id"].Value == cUid)
				{
					if (node.SelectSingleNode(userKey) != null)
					{
						//TO DO: Xml Deserialize
						return node.SelectSingleNode(userKey).InnerText;
					}

					break;
				}
			}

			return null;
		} 
		#endregion

		#region SaveValue
		/// <summary>
		/// Saves the value.
		/// </summary>
		/// <param name="cUid">The c uid.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public override void SaveValue(string cUid, string key, object value)
		{			
			String userKey = String.Format("{0}__{1}", key, Security.CurrentUser.UserID);
			XmlNode controlNode = null;
			
			foreach (XmlNode node in xmlDoc.DocumentElement.SelectNodes(_baseTagName))
			{
				if (node.Attributes["id"] != null && node.Attributes["id"].Value == cUid)
				{
					controlNode = node;
					break;
				}
			}
			
			if (controlNode == null)
			{
				XmlAttribute attr = xmlDoc.CreateAttribute("id");
				attr.Value = cUid;

				controlNode = xmlDoc.DocumentElement.AppendChild(xmlDoc.CreateElement(_baseTagName));
				controlNode.Attributes.Append(attr);
			}

			XmlNode valueNode = controlNode.SelectSingleNode(userKey);
			if (valueNode == null)
			{
				valueNode = controlNode.AppendChild(xmlDoc.CreateElement(userKey));
			}
			
			//TO DO: xml serialize
			valueNode.InnerText = value.ToString();
			xmlDoc.Save(HostingEnvironment.ApplicationPhysicalPath + _fileName);
		} 
		#endregion

		#region DeleteValue
		/// <summary>
		/// Deletes the value.
		/// </summary>
		/// <param name="cUid">The c uid.</param>
		public override void DeleteValue(string cUid)
		{
			//not implemented
		} 
		#endregion
	}
}
