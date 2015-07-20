using System;
using System.Configuration;
using System.Xml;
using System.Collections;

namespace Mediachase.IBN.Business.Pop3
{
	/// <summary>
	/// Summary description for Pop3ManagerConfigHandler.
	/// </summary>
	public class Pop3ManagerConfigHandler: IConfigurationSectionHandler
	{
		public Pop3ManagerConfigHandler()
		{
		}
		#region IConfigurationSectionHandler Members

		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			Pop3ManagerConfig config = new Pop3ManagerConfig();

			foreach(XmlNode item in section.ChildNodes)
			{
				try
				{
					switch(item.Name)
					{
						case "add":
							config.Handlers.Add(new Pop3MessageHandlerInfo(item.Attributes["name"].Value,
								item.Attributes["type"].Value,
								item.Attributes["propertyControl"]!=null?item.Attributes["propertyControl"].Value:string.Empty)	);
							break;
						case "remove":
							config.Handlers.Remove(item.Attributes["name"].Value);
							break;
						case "clear":
							config.Handlers.Clear();
							break;
//						default:
//							throw new NotSupportedException(string.Format("Wrong Xml Element: {0}",item.OuterXml));
					}	
				}
				catch(Exception ex)
				{
					throw new ConfigurationErrorsException(ex.Message, ex,item);
				}
			}

			return config;
		}

		#endregion
	}
}
