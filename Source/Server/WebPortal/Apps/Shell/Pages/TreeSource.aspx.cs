using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.XmlTools;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Business.Customization;

namespace Mediachase.Ibn.Web.UI.Shell.Pages
{
	public partial class TreeSource : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request["mode"] == "full")
			{
				BindFullTree();
			}
			else if (Request.Form["node"] != "leftTemplate_tree_rootId")
			{
				BindNode(Request.Form["node"]);
			}
			else
			{
				BindJsTree();
			}
		}

		#region private void BindJsTree()
		private void BindJsTree()
		{
			string tabId = String.Empty;
			if (Request["tab"] != null)
				tabId = Request["tab"];
			if (String.IsNullOrEmpty(tabId))
				return;

			IXPathNavigable navigable;
			// Selector: ClassName.ViewName.PlaceName.ProfileId.UserId
			Selector selector = new Selector(string.Empty, string.Empty, string.Empty, ProfileManager.GetProfileIdByUser().ToString(), Mediachase.IBN.Business.Security.UserID.ToString());

			// don't hide items for administrator
			if (Mediachase.IBN.Business.Security.IsUserInGroup(Mediachase.IBN.Business.InternalSecureGroups.Administrator))
				navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetCustomizationXml(null, StructureType.Navigation, selector);
			else
				navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.Navigation, selector);

			XPathNavigator links = navigable.CreateNavigator().SelectSingleNode(string.Format("Navigation/Tabs/Tab[@id='{0}']", tabId));

			List<JsonTreeNode> nodes = new List<JsonTreeNode>();
			if (links != null)
				BindRecursive(nodes, links);

			WriteArray(nodes);
		}
		#endregion

		#region BindRecursive
		private int BindRecursive(List<JsonTreeNode> nodes, XPathNavigator linkItem)
		{
			int retVal = 0;
			foreach (XPathNavigator subItem in linkItem.SelectChildren(string.Empty, string.Empty))
			{
				JsonTreeNode node = new JsonTreeNode();

				string text = UtilHelper.GetResFileString(subItem.GetAttribute("text", string.Empty));
				string id = subItem.GetAttribute("id", string.Empty);

				node.text = text;
				node.id = id;

				node.cls = "nodeCls";

				node.iconCls = "iconNodeCls";

				string iconUrl = subItem.GetAttribute("iconUrl", string.Empty);
				if (!String.IsNullOrEmpty(iconUrl))
					node.icon = ResolveClientUrl(iconUrl);

				string iconCss = subItem.GetAttribute("iconCss", string.Empty);
				if (!String.IsNullOrEmpty(iconCss))
					node.iconCls = iconCss;

				string command = subItem.GetAttribute("command", string.Empty);
				if (!String.IsNullOrEmpty(command))
				{
					if (!CommandManager.IsEnableCommand("", "", "", ProfileManager.GetProfileIdByUser().ToString(), Mediachase.IBN.Business.Security.UserID.ToString(), command))
						continue;
					string cmd = CommandManager.GetCommandString(command, null);
					cmd = cmd.Replace("\"", "&quot;");
					node.href = String.Format("javascript:{0}", cmd);
				}

				bool isAsyncLoad = false;
				string treeLoader = subItem.GetAttribute("treeLoader", string.Empty);
				if (!String.IsNullOrEmpty(treeLoader))
				{
					isAsyncLoad = true;
				}

				if (!isAsyncLoad)
				{
					node.children = new List<JsonTreeNode>();
					int count = BindRecursive(node.children, subItem);
					if (count == 0)
					{
						node.leaf = true;
						node.children = null;
					}
				}

				nodes.Add(node);
				retVal++;
			}
			return retVal;
		}
		#endregion

		#region BindNode
		private void BindNode(string returnNodeId)
		{
			string tabId = String.Empty;
			if (Request["tab"] != null)
				tabId = Request["tab"];
			if (String.IsNullOrEmpty(tabId))
				return;

			string staticNodeId = returnNodeId;
			string innerNodeId = returnNodeId;
			if (Request.Form["itemId"] != null && !String.IsNullOrEmpty(Request.Form["itemId"]) && Request.Form["itemId"] != "null")
				innerNodeId = Request.Form["itemId"].ToString();
			if (Request.Form["staticParentId"] != null && !String.IsNullOrEmpty(Request.Form["staticParentId"]) && Request.Form["staticParentId"] != "null")
				staticNodeId = Request.Form["staticParentId"].ToString();

			IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.Navigation);
			XPathNavigator link = navigable.CreateNavigator().SelectSingleNode(String.Format("//Link[@id='{0}']", staticNodeId));
			if (link != null)
			{
				string treeLoader = link.GetAttribute("treeLoader", string.Empty);
				if (!String.IsNullOrEmpty(treeLoader))
				{
					IJsonHandler jsHandler = (IJsonHandler)AssemblyUtil.LoadObject(treeLoader);
					WriteArray(jsHandler.GetJsonDataSource(innerNodeId));
				}
			}
		}
		#endregion

		#region WriteArray
		private void WriteArray(List<JsonTreeNode> nodes)
		{
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
			string json = UtilHelper.JsonSerialize(nodes);

			Response.Write(json);
		}

		private void WriteArray(string json)
		{
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;

			Response.Write(json);
		}
		#endregion

		#region BindFullTree
		private void BindFullTree()
		{
			IXPathNavigable navigable;

			if (!String.IsNullOrEmpty(Request.QueryString["UserId"]))
			{
				int userId = int.Parse(Request.QueryString["UserId"]);

				// Selector: ClassName.ViewName.PlaceName.ProfileId.UserId
				Selector selector = new Selector(string.Empty, string.Empty, string.Empty, ProfileManager.GetProfileIdByUser(userId).ToString(), userId.ToString());
				navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.Navigation, selector);
			}
			if (!String.IsNullOrEmpty(Request.QueryString["ProfileId"]))
			{
				// Selector: ClassName.ViewName.PlaceName.ProfileId.UserId
				Selector selector = new Selector(string.Empty, string.Empty, string.Empty, Request.QueryString["ProfileId"], String.Empty);
				navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.Navigation, selector);
			}
			else
			{
				// don't apply profile and user level (empty selector) and don't hide items (GetCustomizationXml instead GetXml)
				navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetCustomizationXml(null, StructureType.Navigation);
			}

			XPathNavigator tabsNode = navigable.CreateNavigator().SelectSingleNode("Navigation/Tabs");
			List<JsonTreeNode> nodes = new List<JsonTreeNode>();
			foreach (XPathNavigator subItem in tabsNode.SelectChildren("Tab", string.Empty))
			{
				JsonTreeNode node = new JsonTreeNode();

				node.id = subItem.GetAttribute("id", string.Empty);
				node.text = UtilHelper.GetResFileString(subItem.GetAttribute("text", string.Empty));

				string order = subItem.GetAttribute("order", string.Empty);
				if (!string.IsNullOrEmpty(order))
					node.text += String.Concat("<span class=\"rightColumn\">", order, "</span><span class=\"clearColumn\"></span>");

				node.cls = "nodeCls";

				string iconUrl = subItem.GetAttribute("imageUrl", string.Empty);
				if (!String.IsNullOrEmpty(iconUrl))
					node.icon = ResolveClientUrl(iconUrl);

				node.children = new List<JsonTreeNode>();
				int count = BindRecursiveNoAsync(node.children, subItem);
				if (count == 0)
				{
					node.leaf = true;
					node.children = null;
				}

				nodes.Add(node);
			}

			WriteArray(nodes);
		}
		#endregion 

		#region BindRecursiveNoAsync
		private int BindRecursiveNoAsync(List<JsonTreeNode> nodes, XPathNavigator linkItem)
		{
			int retVal = 0;
			foreach (XPathNavigator subItem in linkItem.SelectChildren(string.Empty, string.Empty))
			{
				JsonTreeNode node = new JsonTreeNode();

				node.id = subItem.GetAttribute("id", string.Empty);

				node.text = UtilHelper.GetResFileString(subItem.GetAttribute("text", string.Empty));

				string order = subItem.GetAttribute("order", string.Empty);
				if (!string.IsNullOrEmpty(order))
					node.text = String.Concat("<span class=\"rightColumn\">", order, "</span><span class=\"leftColumn\">", node.text, "</span>");

				node.cls = "nodeCls";
				node.iconCls = "iconNodeCls";

				string iconUrl = subItem.GetAttribute("iconUrl", string.Empty);
				if (!String.IsNullOrEmpty(iconUrl))
					node.icon = ResolveClientUrl(iconUrl);

				string iconCss = subItem.GetAttribute("iconCss", string.Empty);
				if (!String.IsNullOrEmpty(iconCss))
					node.iconCls = iconCss;

				node.children = new List<JsonTreeNode>();
				int count = BindRecursiveNoAsync(node.children, subItem);
				if (count == 0)
				{
					node.leaf = true;
					node.children = null;
				}

				nodes.Add(node);
				retVal++;
			}
			return retVal;
		}
		#endregion
	}
}
