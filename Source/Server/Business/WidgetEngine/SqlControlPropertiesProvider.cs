using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.Hosting;
using System.Xml.Serialization;
using System.Globalization;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business.WidgetEngine
{
	public class SqlControlPropertiesProvider : ControlPropertiesBase
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

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="cUid">The c uid.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public override object GetValue(string cUid, string key)
		{
			string uid = GetKey(cUid, key);

			UserLightPropertyCollection pc = Security.CurrentUser.Properties;

			string value = pc[uid];

			if (string.IsNullOrEmpty(value))
			{
				//dvs: load default setting for all users if available
				value = PortalConfig.GetValue(uid);

				if (string.IsNullOrEmpty(value))
					return value;
			}

			// Step 1. String to XmlSerializedItem
			XmlSerializedItem item = McXmlSerializer.GetObject<XmlSerializedItem>(value);

			// Step 2. XmlSerializedItem to object
			return McXmlSerializer.GetObject(item.Type, item.Data);
		}

		/// <summary>
		/// Saves the value.
		/// </summary>
		/// <param name="cUid">The c uid.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public override void SaveValue(string cUid, string key, object value)
		{
			string uid = GetKey(cUid, key);

			if (value == null)
			{
				Security.CurrentUser.Properties[uid] = null;
			}
			else if (value is string && string.Empty == ((string)value))
			{
				Security.CurrentUser.Properties[uid] = string.Empty;
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
				Security.CurrentUser.Properties[uid] = McXmlSerializer.GetString<XmlSerializedItem>(item);
			}
		}

		/// <summary>
		/// Deletes the value.
		/// </summary>
		/// <param name="cUid">The c uid.</param>
		public override void DeleteValue(string cUid)
		{
			string uid = GetKey(cUid, string.Empty);
			Security.CurrentUser.Properties.RemoveLike(uid);
		}
	}
}
