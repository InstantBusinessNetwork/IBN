using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Mediachase.Ibn.Configuration
{
	internal class AspInfo : IAspInfo
	{
		public string Scheme { get; internal set; }
		public string Host { get; internal set; }
		public string Port { get; internal set; }
		public string Database { get; internal set; }
		public long SiteId { get; internal set; }
		public string ApplicationPool { get; internal set; }
		public bool IsApplicationPoolCreated { get; internal set; }

		internal static AspInfo Load(XmlDocument serverConfigDocument)
		{
			AspInfo settings = null;

			XmlNode asp = serverConfigDocument.SelectSingleNode("/configuration/asp");
			if (asp != null)
			{
				settings = new AspInfo();

				settings.Scheme = XmlHelper.GetAttributeValue(asp, "scheme");
				settings.Host = XmlHelper.GetAttributeValue(asp, "host");
				settings.Port = XmlHelper.GetAttributeValue(asp, "port");
				settings.Database = XmlHelper.GetAttributeValue(asp, "database");
				settings.SiteId = long.Parse(XmlHelper.GetAttributeValue(asp, "siteId"), CultureInfo.InvariantCulture);
				settings.ApplicationPool = XmlHelper.GetAttributeValue(asp, "applicationPool");
				settings.IsApplicationPoolCreated = bool.Parse(XmlHelper.GetAttributeValue(asp, "applicationPoolCreated"));
			}

			return settings;
		}

		internal void Save(XmlDocument serverConfigDocument)
		{
			XmlNode configuration = serverConfigDocument.SelectSingleNode("/configuration");
			XmlNode asp = configuration.SelectSingleNode("asp");
			if (asp == null)
				asp = configuration.AppendChild(serverConfigDocument.CreateElement("asp"));

			XmlHelper.SetAttributeValue(asp, "scheme", Scheme);
			XmlHelper.SetAttributeValue(asp, "host", Host);
			XmlHelper.SetAttributeValue(asp, "port", Port);
			XmlHelper.SetAttributeValue(asp, "database", Database);
			XmlHelper.SetAttributeValue(asp, "siteId", SiteId.ToString(CultureInfo.InvariantCulture));
			XmlHelper.SetAttributeValue(asp, "applicationPool", ApplicationPool);
			XmlHelper.SetAttributeValue(asp, "applicationPoolCreated", IsApplicationPoolCreated.ToString());
		}

		internal static void Delete(XmlDocument serverConfigDocument)
		{
			XmlNode configuration = serverConfigDocument.SelectSingleNode("/configuration");
			XmlNode asp = configuration.SelectSingleNode("asp");
			if (asp != null)
				configuration.RemoveChild(asp);
		}

		internal void AddWebServiceUri(XmlDocument schedulerConfigDocument)
		{
			XmlNode services = schedulerConfigDocument.SelectSingleNode("/configuration/scheduleService/webServices");
			XmlNode service = services.AppendChild(schedulerConfigDocument.CreateElement("add"));
			service.Attributes.Append(schedulerConfigDocument.CreateAttribute("url")).Value = BuildWebServiceUri();
		}

		internal void DeleteWebServiceUri(System.Xml.XmlDocument schedulerConfigDocument)
		{
			string uri = BuildWebServiceUri();
			XmlNode services = schedulerConfigDocument.SelectSingleNode("/configuration/scheduleService/webServices");
			XmlNode service = services.SelectSingleNode(string.Concat("add[@url='", uri, "']"));
			if (service != null)
				services.RemoveChild(service);
		}


		private string BuildWebServiceUri()
		{
			int port = (string.IsNullOrEmpty(Port) ? -1 : int.Parse(Port, CultureInfo.InvariantCulture));
			return new UriBuilder("http", Host, port, "WebServices/SchedulerService.asmx").ToString();
		}
	}
}
