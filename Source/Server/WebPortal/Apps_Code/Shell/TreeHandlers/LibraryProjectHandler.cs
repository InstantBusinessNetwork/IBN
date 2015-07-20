using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;


namespace Mediachase.Ibn.Web.UI.Shell.TreeHandlers
{
	public class LibraryProjectHandler : IJsonHandler
	{
		#region IJsonHandler Members

		public string GetJsonDataSource(string nodeId)
		{
			List<JsonTreeNode> nodes = new List<JsonTreeNode>();

			using (IDataReader reader = Project.GetListProjects())
			{
				while (reader.Read())
				{
					JsonTreeNode rootNode = new JsonTreeNode();
					rootNode.iconCls = "iconNodeCls";
					int prjId = (int)reader["ProjectId"];
					rootNode.text = reader["Title"].ToString();
					rootNode.cls = "nodeCls";

					rootNode.href = "../../../FileStorage/default.aspx?ProjectId=" + prjId.ToString();
					rootNode.hrefTarget = "right";

					rootNode.id = "MLibrary21_" + prjId.ToString();
					rootNode.children = new List<JsonTreeNode>();

					JsonTreeNode node = new JsonTreeNode();
					node.iconCls = "iconNodeCls";
					node.text = CHelper.GetResFileString("{IbnShell.Navigation:tSimpleTasks}");
					node.cls = "nodeCls";
					node.href = "../../../FileStorage/default.aspx?ProjectId=" + prjId.ToString() + "&TypeId=5";
					node.hrefTarget = "right";
					node.leaf = true;
					rootNode.children.Add(node);

					node = new JsonTreeNode();
					node.iconCls = "iconNodeCls";
					node.text = CHelper.GetResFileString("{IbnShell.Navigation:tTabDocuments}");
					node.cls = "nodeCls";
					node.href = "../../../FileStorage/default.aspx?ProjectId=" + prjId.ToString() + "&TypeId=16";
					node.hrefTarget = "right";
					node.leaf = true;
					rootNode.children.Add(node);

					node = new JsonTreeNode();
					node.iconCls = "iconNodeCls";
					node.text = CHelper.GetResFileString("{IbnShell.Navigation:tSimpleEvents}");
					node.cls = "nodeCls";
					node.href = "../../../FileStorage/default.aspx?ProjectId=" + prjId.ToString() + "&TypeId=4";
					node.hrefTarget = "right";
					node.leaf = true;
					rootNode.children.Add(node);

					node = new JsonTreeNode();
					node.iconCls = "iconNodeCls";
					node.text = CHelper.GetResFileString("{IbnShell.Navigation:tSimpleToDos}");
					node.cls = "nodeCls";
					node.href = "../../../FileStorage/default.aspx?ProjectId=" + prjId.ToString() + "&TypeId=6";
					node.hrefTarget = "right";
					node.leaf = true;
					rootNode.children.Add(node);

					nodes.Add(rootNode);
				}
			}

			if (nodes.Count > 0)
				return UtilHelper.JsonSerialize(nodes);
			else
				return String.Empty;
		}

		#endregion
	}
}
