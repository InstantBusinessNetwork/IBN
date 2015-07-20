using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Xml.XPath;

using Mediachase.Ibn.Business.Customization;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.Ibn.Web.UI.Shell.Modules
{
	public partial class leftTemplate : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			dt.Columns.Add(new DataColumn("ImageUrl", typeof(string)));
			DataRow dr;
			IXPathNavigable navigable;
			// Selector: ClassName.ViewName.PlaceName.ProfileId.UserId
			Selector selector = new Selector(string.Empty, string.Empty, string.Empty, ProfileManager.GetProfileIdByUser().ToString(), Mediachase.IBN.Business.Security.UserID.ToString());

			// don't hide items for administrator
			if (Mediachase.IBN.Business.Security.IsUserInGroup(Mediachase.IBN.Business.InternalSecureGroups.Administrator))
				navigable = XmlBuilder.GetCustomizationXml(null, StructureType.Navigation, selector);
			else
				navigable = XmlBuilder.GetXml(StructureType.Navigation, selector);
				

			XPathNavigator tabs = navigable.CreateNavigator().SelectSingleNode("Navigation/Tabs");
			foreach (XPathNavigator tabItem in tabs.SelectChildren(string.Empty, string.Empty))
			{
				dr = dt.NewRow();
				string title = UtilHelper.GetResFileString(tabItem.GetAttribute("text", string.Empty));
				string id = tabItem.GetAttribute("id", string.Empty);

				string enableHandler = tabItem.GetAttribute("enableHandler", string.Empty);
				if (!string.IsNullOrEmpty(enableHandler))
				{
					ICommandEnableHandler enHandler = (ICommandEnableHandler)AssemblyUtil.LoadObject(enableHandler);
					if (enHandler != null && !enHandler.IsEnable(sender, id))
						continue;
				}

				string imageUrl = tabItem.GetAttribute("imageUrl", string.Empty);
				if (string.IsNullOrEmpty(imageUrl))
					imageUrl = "~/Images/ext/default/s.gif";

				string type = tabItem.GetAttribute("contentType", string.Empty).ToLower();
				if (string.IsNullOrEmpty(type))
					type = "default";
				
				string configUrl = tabItem.GetAttribute("configUrl", string.Empty);
				string checkUrl = configUrl;
				if (checkUrl.IndexOf("?") >= 0)
					checkUrl = checkUrl.Substring(0, checkUrl.IndexOf("?"));
				if (type.Equals("default") && string.IsNullOrEmpty(checkUrl))
				{
					checkUrl = "~/Apps/Shell/Pages/TreeSource.aspx";
					configUrl = "~/Apps/Shell/Pages/TreeSource.aspx?tab=" + id;
				}

				if(File.Exists(Server.MapPath(checkUrl)))
				{
					switch (type)
					{
						case "default":
							ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"), string.Format("leftTemplate_AddMenuTab('{0}', '{1}', '{2}');", id, title, ResolveClientUrl(configUrl)), true);
							break;
						case "custom":
							break;
						default:
							break;
					}
				}

				dr["Title"] = title;
				dr["ImageUrl"] = imageUrl;
				dt.Rows.Add(dr);
			}
			TabItems.DataSource = dt.DefaultView;
			TabItems.DataBind();

			RegisterScripts();

			//Register navigation commands
			string profileId = ProfileManager.GetProfileIdByUser().ToString();
			string userId = Mediachase.IBN.Business.Security.UserID.ToString();
			IList<XmlCommand> list = XmlCommand.GetListNavigationCommands("", "", "", profileId, userId);
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			foreach (XmlCommand cmd in list)
			{
				cm.AddCommand("", "", "", profileId, userId, cmd.CommandName);
			}
		}

		private int _index = -1;

		protected int GetIndex()
		{
			_index++;
			return _index;
		}

		#region RegisterScripts
		private void RegisterScripts()
		{
			//scripts
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), Guid.NewGuid().ToString(), ResolveClientUrl("~/Scripts/Shell/mainLeftTemplate.js"));

			ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("~/Scripts/Shell/mainLeftTemplateResizer.js")); ;
			//Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), Guid.NewGuid().ToString(), ResolveClientUrl("~/Scripts/Shell/mainLeftTemplateResizer.js"));

			//styles
			//Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), Guid.NewGuid().ToString(),
			//    String.Format("<link type='text/css' rel='stylesheet' href='{0}' />", ResolveClientUrl("~/Styles/Shell/mainLeftTemplate.css")));
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/Shell/mainLeftTemplate.css");
		}
		#endregion
	}
}