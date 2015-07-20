using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Mediachase.Ibn.Data;
using System.Web;
using System.Xml;


namespace Mediachase.IBN.Business.WidgetEngine
{
	public class SqlMetaControlPropertiesProvider : ControlPropertiesBase
	{
		[Serializable]
		public class XmlSerializedItem
		{
			public XmlSerializedItem()
			{
			}

			public XmlSerializedItem(string type, string data)
			{
				this.Type = type;
				this.Data = data.Replace("xsi:", "");
			}

			public string Type;
			public string Data;
		}

		#region prop: pageUid
		/// <summary>
		/// Gets the page uid.
		/// </summary>
		/// <value>The page uid.</value>
		private Guid pageUid
		{
			get
			{
				if (HttpContext.Current == null)
					throw new ArgumentNullException("HttpContext is null @ SqlMetaControlPropertiesProvider");

				if (HttpContext.Current.Items == null)
					throw new ArgumentNullException("HttpContext.Current.Items is null @ SqlMetaControlPropertiesProvider");

				if (HttpContext.Current.Items[ControlProperties._pageUidKey] != null)
					return new Guid(HttpContext.Current.Items[ControlProperties._pageUidKey].ToString());

				return Guid.Empty;
			}
		} 
		#endregion

		#region prop: profileId
		/// <summary>
		/// Gets the profile id.
		/// </summary>
		/// <value>The profile id.</value>
		private int? profileId
		{
			get
			{
				if (HttpContext.Current == null)
					throw new ArgumentNullException("HttpContext is null @ SqlMetaControlPropertiesProvider");

				if (HttpContext.Current.Items == null)
					throw new ArgumentNullException("HttpContext.Current.Items is null @ SqlMetaControlPropertiesProvider");

				return (int?)HttpContext.Current.Items[ControlProperties._profileUidKey];
			}
		} 
		#endregion

		#region prop: userId
		/// <summary>
		/// Gets the user id.
		/// </summary>
		/// <value>The user id.</value>
		private int? userId
		{
			get
			{
				if (HttpContext.Current == null)
					throw new ArgumentNullException("HttpContext is null @ SqlMetaControlPropertiesProvider");

				if (HttpContext.Current.Items == null)
					throw new ArgumentNullException("HttpContext.Current.Items is null @ SqlMetaControlPropertiesProvider");

				return (int?)HttpContext.Current.Items[ControlProperties._userUidKey];
			}
		} 
		#endregion

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <param name="cUid">The c uid.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		protected virtual string GetKey(string cUid, string key)
		{
			string retVal = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", cUid, key);
			return retVal;
		}

		public override object GetValue(string cUid, string key)
		{
			string uid = GetKey(cUid, key);
			CustomPageEntity cpe = Mediachase.IBN.Business.WidgetEngine.CustomPageManager.GetCustomPage(pageUid, profileId, userId);

			if (cpe == null)
				throw new ArgumentException(string.Format("Page not found for pageUid: {0} profileId: {1} userId: {2}", pageUid, profileId, userId));

			XmlDocument doc = new XmlDocument();
			if (String.IsNullOrEmpty(cpe.PropertyJsonData))
			{
				doc.AppendChild(doc.CreateElement("ControlProperties"));
				cpe.PropertyJsonData = doc.OuterXml;
			}
			else
			{
				doc.LoadXml(cpe.PropertyJsonData);
			}

			string value = string.Empty;

			XmlNode valNode = doc.DocumentElement.SelectSingleNode(string.Format("{0}/{1}", cUid, key));
			if (valNode != null)
			{
				value = valNode.InnerText;
			}
			//else
			//{
			//    value = PortalConfig.GetValue(uid);
			//}

			if (String.IsNullOrEmpty(value))
				return null;

			// Step 1. String to XmlSerializedItem
			value = value.Trim();
			XmlSerializedItem item = McXmlSerializer.GetObject<XmlSerializedItem>(value);

			// Step 2. XmlSerializedItem to object
			return McXmlSerializer.GetObject(item.Type, item.Data);
		}

		public override void SaveValue(string cUid, string key, object value)
		{
			string uid = GetKey(cUid, key);
			
			CustomPageEntity cpe = Mediachase.IBN.Business.WidgetEngine.CustomPageManager.GetCustomPage(pageUid, profileId, userId);

			if (cpe == null)
				throw new ArgumentException(string.Format("Page not found for pageUid: {0} profileId: {1} userId: {2}", pageUid, profileId, userId));

			XmlDocument doc = new XmlDocument();
			if (String.IsNullOrEmpty(cpe.PropertyJsonData))
			{
				doc.AppendChild(doc.CreateElement("ControlProperties"));
				cpe.PropertyJsonData = doc.OuterXml;
			}
			else
			{
				doc.LoadXml(cpe.PropertyJsonData);
			}

			XmlNode controlNode = doc.DocumentElement.SelectSingleNode(cUid);
			if (controlNode == null)
			{
				controlNode = doc.CreateElement(cUid);
				doc.DocumentElement.AppendChild(controlNode);
			}

			XmlNode keyNode = controlNode.SelectSingleNode(key);
			if (keyNode == null)
			{
				keyNode = doc.CreateElement(key);
				controlNode.AppendChild(keyNode);
			}

			if (value == null)
			{
				keyNode.InnerText = ControlProperties._nullValueKey;
			}
			else if (value is string && string.Empty == ((string)value))
			{
				keyNode.InnerText = string.Empty;
			}
			else
			{
				// Step 1. Object to XmlSerializedItem
				string typeName = null;

				if (value.GetType().IsGenericType)
					typeName = AssemblyUtil.GetTypeString(value.GetType().FullName, value.GetType().Assembly.GetName().Name);
				else
					typeName = AssemblyUtil.GetTypeString(value.GetType().ToString(), value.GetType().Assembly.GetName().Name);

				XmlSerializedItem item = new XmlSerializedItem(typeName, McXmlSerializer.GetString(value.GetType(), value));

				// Step 2. XmlSerializedItem to string
				keyNode.InnerText = McXmlSerializer.GetString<XmlSerializedItem>(item);
			}

			//todo: to debug
			Mediachase.IBN.Business.WidgetEngine.CustomPageManager.UpdateCustomPageProperty(pageUid, doc.OuterXml, profileId, userId);
		}

		public override void DeleteValue(string cUid)
		{
			CustomPageEntity cpe = Mediachase.IBN.Business.WidgetEngine.CustomPageManager.GetCustomPage(pageUid, profileId, userId);

			if (cpe == null)
				throw new ArgumentException(string.Format("Page not found for pageUid: {0} profileId: {1} userId: {2}", pageUid, profileId, userId));

			XmlDocument doc = new XmlDocument();
			if (String.IsNullOrEmpty(cpe.PropertyJsonData))
			{
				doc.AppendChild(doc.CreateElement("ControlProperties"));
				cpe.PropertyJsonData = doc.OuterXml;
			}
			else
			{
				doc.LoadXml(cpe.PropertyJsonData);
			}

			XmlNode controlNode = doc.DocumentElement.SelectSingleNode(cUid);
			if (controlNode != null)
			{
				doc.DocumentElement.RemoveChild(controlNode);
			}

			//todo: to debug
			Mediachase.IBN.Business.WidgetEngine.CustomPageManager.UpdateCustomPageProperty(pageUid, doc.OuterXml, profileId, userId);
		}
	}
}
