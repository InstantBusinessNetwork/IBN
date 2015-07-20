using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Mediachase.Ibn.Configuration
{
	internal sealed class XmlHelper
	{
		private XmlHelper()
		{
		}

		#region internal static string GetAttributeValue(XmlNode node, string attributeName)
		internal static string GetAttributeValue(XmlNode node, string attributeName)
		{
			string result = null;

			if (node != null)
			{
				XmlAttribute attribute = node.Attributes[attributeName];
				if (attribute != null)
					result = attribute.Value;
			}

			return result;
		}
		#endregion

		#region private static void SetAttributeValue(XmlNode node, string attributeName, string attributeValue)
		internal static void SetAttributeValue(XmlNode node, string attributeName, string attributeValue)
		{
			XmlAttribute attribute = node.Attributes[attributeName];
			if (attribute == null)
				attribute = node.Attributes.Append(node.OwnerDocument.CreateAttribute(attributeName));
			attribute.Value = attributeValue;
		}
		#endregion

		#region internal static string GetChildNodeText(XmlNode node, string childName)
		internal static string GetChildNodeText(XmlNode node, string childName)
		{
			string result = null;

			if (node != null)
			{
				XmlNode child = node.SelectSingleNode(childName);
				if (child != null)
					result = child.InnerText;
			}

			return result;
		}
		#endregion
	}
}
