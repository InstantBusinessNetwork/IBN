using System;
using System.Collections;
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
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;


namespace Mediachase.Ibn.Web.UI.FileLibrary.Pages
{
	public partial class FileLibraryTreeSource : System.Web.UI.Page
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
			string containerName = Request["ContainerName"];
			string containerKey = Request["ContainerKey"];
			string folderId = Request["FolderId"];
			Mediachase.IBN.Business.ControlSystem.FileStorage fs = Mediachase.IBN.Business.ControlSystem.FileStorage.Create(containerName, containerKey);

			ArrayList parentList = new ArrayList();
			if (!String.IsNullOrEmpty(folderId))
			{
				int iFolder = int.Parse(folderId);
				while (true)
				{
					DirectoryInfo diEx = fs.GetDirectory(iFolder);
					parentList.Add(diEx.Id);
					if (iFolder == fs.Root.Id)
						break;
					iFolder = diEx.ParentDirectoryId;
				}
			}

			BindNode(fs.Root.Id.ToString(), parentList);
		}

		#endregion

		#region BindNode
		private void BindNode(string nodeId, ArrayList parentList)
		{
			List<JsonTreeNode> nodes = new List<JsonTreeNode>();
			string containerName = Request["ContainerName"];
			string containerKey = Request["ContainerKey"];

			Mediachase.IBN.Business.ControlSystem.FileStorage fs = Mediachase.IBN.Business.ControlSystem.FileStorage.Create(containerName, containerKey);
			DirectoryInfo diCur = fs.GetDirectory(int.Parse(nodeId));
			if (!fs.CanUserRead(diCur))
			{
				WriteArray(String.Empty);
			}
			else
			{
				DataTable dt = new DataTable();
				dt.Columns.Add(new DataColumn("Id", typeof(string)));
				dt.Columns.Add(new DataColumn("Name", typeof(string)));
				dt.Columns.Add(new DataColumn("Count", typeof(int)));
				DataRow dr;
				DirectoryInfo[] dMas = fs.GetDirectories(int.Parse(nodeId));
				foreach (DirectoryInfo di in dMas)
				{
					if (!fs.CanUserRead(di))
						continue;
					dr = dt.NewRow();
					dr["Id"] = di.Id.ToString();
					dr["Name"] = di.Name;

					int count = 0;
					DirectoryInfo[] check = fs.GetDirectories(di.Id);
					foreach (DirectoryInfo dIn in check)
						if (fs.CanUserRead(dIn))
							count++;
					dr["Count"] = count;

					dt.Rows.Add(dr);
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

					if (parentList.Contains(int.Parse(drv["Id"].ToString())))
					{
						node.expanded = true;
						BindRecursive(node, parentList);
					}
					else
						node.expanded = false;

					Dictionary<string, string> param = new Dictionary<string, string>();
					param.Add("ContainerKey", containerKey);
					param.Add("FolderId", drv["Id"].ToString());
					string cmd = CommandManager.GetCommandString("FL_ChangeFolderTree", param);
					cmd = cmd.Replace("\"", "&quot;");
					node.href = String.Format("javascript:{0}", cmd);

					//node.href = ResolveClientUrl("~/Apps/FileLibrary/Pages/FileStorage.aspx?FolderId=" + di.Id);
					//node.hrefTarget = "right";
					nodes.Add(node);
				}
				WriteArray(nodes);
			}
		}

		private void BindRecursive(JsonTreeNode node, ArrayList parentList)
		{
			string containerName = Request["ContainerName"];
			string containerKey = Request["ContainerKey"];

			Mediachase.IBN.Business.ControlSystem.FileStorage fs = Mediachase.IBN.Business.ControlSystem.FileStorage.Create(containerName, containerKey);

			node.children = new List<JsonTreeNode>();

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("Count", typeof(int)));
			DataRow dr;
			DirectoryInfo[] dMas = fs.GetDirectories(int.Parse(node.itemid));
			foreach (DirectoryInfo di in dMas)
			{
				if (!fs.CanUserRead(di))
					continue;
				dr = dt.NewRow();
				dr["Id"] = di.Id.ToString();
				dr["Name"] = di.Name;

				int count = 0;
				DirectoryInfo[] check = fs.GetDirectories(di.Id);
				foreach (DirectoryInfo dIn in check)
					if (fs.CanUserRead(dIn))
						count++;
				dr["Count"] = count;

				dt.Rows.Add(dr);
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
				if (parentList.Contains(int.Parse(drv["Id"].ToString())))
				{
					inNode.expanded = true;
					BindRecursive(inNode, parentList);
				}
				else
					inNode.expanded = false;

				Dictionary<string, string> param = new Dictionary<string, string>();
				param.Add("ContainerKey", containerKey);
				param.Add("FolderId", drv["Id"].ToString());
				string cmd = CommandManager.GetCommandString("FL_ChangeFolderTree", param);
				cmd = cmd.Replace("\"", "&quot;");
				inNode.href = String.Format("javascript:{0}", cmd);
				//inNode.href = ResolveClientUrl("~/Apps/FileLibrary/Pages/FileStorage.aspx?FolderId=" + di.Id);
				//inNode.hrefTarget = "right";
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
