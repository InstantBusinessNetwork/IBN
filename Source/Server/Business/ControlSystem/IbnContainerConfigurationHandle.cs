using System;
using System.Xml;
using System.Configuration;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for IbnContainerConfigurationHandle.
	/// </summary>
	public class IbnContainerConfigurationHandle: IConfigurationSectionHandler
	{
		public IbnContainerConfigurationHandle()
		{
		}
		#region IConfigurationSectionHandler Members

		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			IbnContainerConfiguration cnfg = new IbnContainerConfiguration();

			foreach(XmlNode item in section.ChildNodes)
			{
				// Step 1. Extract Container
				if(item.Name=="container")
				{
					IbnContainerInfo container = new IbnContainerInfo(item.Attributes["name"].Value);

					// Step 2. Extract Conrol Infos.
					foreach(XmlNode xmlControlNode in item.ChildNodes)
					{
						if(xmlControlNode.Name=="control")
						{
							IbnControlInfo control = new IbnControlInfo(xmlControlNode.Attributes["name"].Value,
								xmlControlNode.Attributes["type"].Value);

							// Step 3. Extract Parameters
							foreach(XmlNode xmlParamNode in xmlControlNode.SelectNodes("params/param"))
							{
								control.Parameters.Add(xmlParamNode.Attributes["name"].Value,xmlParamNode.Attributes["value"].Value);
							}

							// Step 4. Extract ACL
							foreach(XmlNode xmlACLNode in xmlControlNode.SelectNodes("acl"))
							{
								string Filter = "*";

								if(xmlACLNode.Attributes["containerKey"]!=null)
									Filter = xmlACLNode.Attributes["containerKey"].Value;

								foreach(XmlNode xmlACENode in xmlACLNode.SelectNodes("ace"))
								{
									if(xmlACENode.Attributes["role"]!=null)
										// Role Ace
										control.DefaultAccessControlList.Add(Filter, new AccessControlEntry(xmlACENode.Attributes["role"].Value,
											xmlACENode.Attributes["action"].Value,bool.Parse(xmlACENode.Attributes["allow"].Value)));
									else
										// Principal Ace
										control.DefaultAccessControlList.Add(Filter, new AccessControlEntry(int.Parse(xmlACENode.Attributes["principalid"].Value),
											xmlACENode.Attributes["action"].Value,bool.Parse(xmlACENode.Attributes["allow"].Value)));
								}
							}

							container.Controls.Add(control);
						}
					}

					cnfg.Containers.Add(container);
				}
			}

			return cnfg;
		}

		#endregion
	}
}
