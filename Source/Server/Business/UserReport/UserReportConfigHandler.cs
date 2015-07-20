using System;
using System.Xml;
using System.Configuration;

namespace Mediachase.IBN.Business.UserReport
{
	/// <summary>
	/// Summary description for UserReportConfigHandler.
	/// </summary>
	public class UserReportConfigHandler: IConfigurationSectionHandler
	{
		public UserReportConfigHandler()
		{
		}
		#region IConfigurationSectionHandler Members

		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			UserReportConfig config = new UserReportConfig();
 
			foreach(XmlNode xmlReportNode in section.ChildNodes)
			{
				switch(xmlReportNode.Name)
				{
					case "add":
						if(xmlReportNode.Attributes["name"]==null)
							throw new ConfigurationErrorsException("Expected 'name' attribute.",xmlReportNode);

						if(xmlReportNode.Attributes["url"]==null)
							throw new ConfigurationErrorsException("Expected 'url' attribute.",xmlReportNode);

						UserReportType type = UserReportType.Global;
						if(xmlReportNode.Attributes["type"]!=null)
						{
							type = (UserReportType)Enum.Parse(typeof(UserReportType),xmlReportNode.Attributes["type"].Value);
						}

						config.Reports.Add(xmlReportNode.Attributes["name"].Value,
							xmlReportNode.Attributes["url"].Value,
							type,
							xmlReportNode.Attributes["infoClass"]!=null?xmlReportNode.Attributes["infoClass"].Value:string.Empty);
						break;
					case "remove":
						if(xmlReportNode.Attributes["name"]==null)
							throw new ConfigurationErrorsException("Expected 'name' attribute.",xmlReportNode);
						config.Reports.Remove(xmlReportNode.Attributes["name"].Value);
						break;
					case "clear":
						config.Reports.Clear();
						break;
					default:
						break;
				}
			}

			return config;
		}

		#endregion
	}
}
