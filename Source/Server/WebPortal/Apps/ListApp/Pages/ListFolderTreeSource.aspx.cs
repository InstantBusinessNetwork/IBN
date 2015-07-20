using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Data;
using System.Collections.Generic;
using Mediachase.Ibn.Data.Meta;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ListApp.Pages
{
	public partial class ListFolderTreeSource : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.Form["node"] != "FL_root_id")
			{
				BindNode(Request.Form["itemId"], new ArrayList());
			}
			else
			{
				BindJsTree();
			}
		}

		#region BindJsTree
		private void BindJsTree()
		{
			string folderId = Request["FolderId"];
			
			ArrayList parentList = new ArrayList();
			if (!String.IsNullOrEmpty(folderId))
			{
				PrimaryKeyId iFolder = PrimaryKeyId.Parse(folderId);
				while (true)
				{
					ListFolder folder = new ListFolder(iFolder);
					parentList.Add(folder.PrimaryKeyId.Value);
					if (!folder.ParentId.HasValue)
						break;
					iFolder = folder.ParentId.Value;
				}
			}

			BindNode("0", parentList);
		}

		#endregion

		#region BindNode
		private void BindNode(string nodeId, ArrayList parentList)
		{
			List<JsonTreeNode> nodes = new List<JsonTreeNode>();

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("Count", typeof(int)));
			DataRow dr;

			if (nodeId == "0")
			{
				//Private Lists
				dr = dt.NewRow();
				ListFolder priv = ListManager.GetPrivateRoot(Mediachase.IBN.Business.Security.CurrentUser.UserID);
				dr["Id"] = priv.PrimaryKeyId.Value.ToString();
				dr["Name"] = GetGlobalResourceObject("IbnShell.Navigation", "tPrivLists").ToString();
				dr["Count"] = Mediachase.Ibn.Data.Services.TreeManager.GetChildNodes(priv).Length;
				dt.Rows.Add(dr);

				//Public Lists
				dr = dt.NewRow();
				ListFolder pub = ListManager.GetPublicRoot();
				dr["Id"] = pub.PrimaryKeyId.Value.ToString();
				dr["Name"] = GetGlobalResourceObject("IbnShell.Navigation", "tPubLists").ToString();
				dr["Count"] = Mediachase.Ibn.Data.Services.TreeManager.GetChildNodes(pub).Length;
				dt.Rows.Add(dr);

				//Projects Lists
				dr = dt.NewRow();
				dr["Id"] = "-1";
				dr["Name"] = GetGlobalResourceObject("IbnShell.Navigation", "tPrjLists").ToString();
				int count = 0;
				using (IDataReader reader = Project.GetListProjects())
				{
					while (reader.Read())
						count++;
				}
				dr["Count"] = count;
				dt.Rows.Add(dr);
			}
			else if (nodeId == "-1")
			{
				using (IDataReader reader = Project.GetListProjects())
				{
					while (reader.Read())
					{
						ListFolder prj = ListManager.GetProjectRoot((int)reader["ProjectId"]);
						dr = dt.NewRow();
						dr["Id"] = prj.PrimaryKeyId.Value.ToString();
						dr["Name"] = reader["Title"].ToString();
						dr["Count"] = Mediachase.Ibn.Data.Services.TreeManager.GetChildNodes(prj).Length;
						dt.Rows.Add(dr);
					}
				}
			}
			else
			{
				ListFolder folder = new ListFolder(PrimaryKeyId.Parse(nodeId));
				foreach (Mediachase.Ibn.Data.Services.TreeNode tN in Mediachase.Ibn.Data.Services.TreeManager.GetChildNodes(folder))
				{
					ListFolder inFolder = new ListFolder(tN.InnerObject.PrimaryKeyId.Value);
					dr = dt.NewRow();
					dr["Id"] = inFolder.PrimaryKeyId.Value.ToString();
					dr["Name"] = inFolder.Title;
					dr["Count"] = Mediachase.Ibn.Data.Services.TreeManager.GetChildNodes(inFolder).Length;
					dt.Rows.Add(dr);
				}
			}

			DataView dv = dt.DefaultView;
			dv.Sort = "Name";
			foreach (DataRowView drv in dv)
			{
				JsonTreeNode node = new JsonTreeNode();
				node.leaf = ((int)drv["Count"] == 0);
				node.cls = "nodeCls";
				node.iconCls = "iconStdCls";
				node.icon = ResolveClientUrl("~/layouts/images/folder.gif");
				node.text = drv["Name"].ToString();
				node.id = Guid.NewGuid().ToString("N");
				node.itemid = drv["Id"].ToString();

				if (parentList.Contains(PrimaryKeyId.Parse(drv["Id"].ToString())))
				{
					node.expanded = true;
					BindRecursive(node, parentList);
				}
				else
					node.expanded = false;

				if (node.itemid != "-1")
				{
					Dictionary<string, string> param = new Dictionary<string, string>();
					param.Add("FolderId", drv["Id"].ToString());
					string cmd = CommandManager.GetCommandString("MC_ListApp_ChangeFolderTree", param);
					cmd = cmd.Replace("\"", "&quot;");
					node.href = String.Format("javascript:{0}", cmd);
				}
				nodes.Add(node);
			}
			WriteArray(nodes);
		}

		private void BindRecursive(JsonTreeNode node, ArrayList parentList)
		{
			node.children = new List<JsonTreeNode>();

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("Count", typeof(int)));
			DataRow dr;
			string nodeId = node.itemid;
			if(nodeId == "-1")
			{
				using (IDataReader reader = Project.GetListProjects())
				{
					while (reader.Read())
					{
						ListFolder prj = ListManager.GetProjectRoot((int)reader["ProjectId"]);
						dr = dt.NewRow();
						dr["Id"] = prj.PrimaryKeyId.Value.ToString();
						dr["Name"] = reader["Title"].ToString();
						dr["Count"] = Mediachase.Ibn.Data.Services.TreeManager.GetChildNodes(prj).Length;
						dt.Rows.Add(dr);
					}
				}
			}
			else
			{
				ListFolder folder = new ListFolder(PrimaryKeyId.Parse(nodeId));
				foreach (Mediachase.Ibn.Data.Services.TreeNode tN in Mediachase.Ibn.Data.Services.TreeManager.GetChildNodes(folder))
				{
					ListFolder inFolder = new ListFolder(tN.InnerObject.PrimaryKeyId.Value);
					dr = dt.NewRow();
					dr["Id"] = inFolder.PrimaryKeyId.Value.ToString();
					dr["Name"] = inFolder.Title;
					dr["Count"] = Mediachase.Ibn.Data.Services.TreeManager.GetChildNodes(inFolder).Length;
					dt.Rows.Add(dr);
				}
			}
			
			DataView dv = dt.DefaultView;
			dv.Sort = "Name";
			foreach (DataRowView drv in dv)
			{
				JsonTreeNode inNode = new JsonTreeNode();
				inNode.leaf = ((int)drv["Count"] == 0);
				inNode.cls = "nodeCls";
				inNode.iconCls = "iconStdCls";
				inNode.icon = ResolveClientUrl("~/layouts/images/folder.gif");
				inNode.text = drv["Name"].ToString();
				inNode.id = Guid.NewGuid().ToString("N");
				inNode.itemid = drv["Id"].ToString();
				if (parentList.Contains(PrimaryKeyId.Parse(drv["Id"].ToString())))
				{
					inNode.expanded = true;
					BindRecursive(inNode, parentList);
				}
				else
					inNode.expanded = false;

				Dictionary<string, string> param = new Dictionary<string, string>();
				param.Add("FolderId", drv["Id"].ToString());
				string cmd = CommandManager.GetCommandString("MC_ListApp_ChangeFolderTree", param);
				cmd = cmd.Replace("\"", "&quot;");
				inNode.href = String.Format("javascript:{0}", cmd);
				node.children.Add(inNode);
			}
		}
		#endregion

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
	}
}
