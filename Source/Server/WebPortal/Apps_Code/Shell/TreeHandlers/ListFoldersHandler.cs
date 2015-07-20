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
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;


namespace Mediachase.Ibn.Web.UI.Shell.TreeHandlers
{
	public class ListFoldersHandler : IJsonHandler
	{
		private const string _rootId = "MLists1_0";
		#region IJsonHandler Members

		public string GetJsonDataSource(string nodeId)
		{
			int parentId = 0;
			int.TryParse(nodeId, out parentId);

			List<JsonTreeNode> nodes = new List<JsonTreeNode>();

			JsonTreeNode node;
			if (parentId == 0)
			{
				ListFolder privFolder = ListManager.GetPrivateRoot(Mediachase.Ibn.Data.Services.Security.CurrentUserId);
				node = new JsonTreeNode();

				node.text = CHelper.GetResFileString("{IbnShell.Navigation:tPrivLists}");
				node.cls = "nodeCls";

				node.id = Guid.NewGuid().ToString("N");
				node.itemid = privFolder.PrimaryKeyId.Value.ToString();
				node.staticParentId = _rootId;

				node.href = "../../../Apps/ListApp/Pages/ListInfoList.aspx?ListFolderId=" + privFolder.PrimaryKeyId.Value.ToString();
				node.hrefTarget = "right";

				if (!privFolder.HasChildren)
					node.leaf = true;

				nodes.Add(node);

				ListFolder pubFolder = ListManager.GetPublicRoot();
				node = new JsonTreeNode();

				node.text = CHelper.GetResFileString("{IbnShell.Navigation:tPubLists}");
				node.cls = "nodeCls";

				node.id = Guid.NewGuid().ToString("N");
				node.itemid = pubFolder.PrimaryKeyId.Value.ToString();
				node.staticParentId = _rootId;

				node.href = "../../../Apps/ListApp/Pages/ListInfoList.aspx?ListFolderId=" + pubFolder.PrimaryKeyId.Value.ToString();
				node.hrefTarget = "right";

				if (!pubFolder.HasChildren)
					node.leaf = true;

				nodes.Add(node);

				if (Mediachase.IBN.Business.Configuration.ProjectManagementEnabled)
				{
					node = new JsonTreeNode();

					node.text = CHelper.GetResFileString("{IbnShell.Navigation:tPrjLists}");
					node.cls = "nodeCls";

					node.id = Guid.NewGuid().ToString("N");
					node.itemid = "-1";
					node.staticParentId = _rootId;

					node.href = "../../../Apps/ListApp/Pages/ListInfoList.aspx?ListFolderId=-1";
					node.hrefTarget = "right";

					node.leaf = true;
					nodes.Add(node);
				}
			}
			else
			{
				ListFolder fParent = new ListFolder(parentId);
				if (fParent != null)
				{
					Mediachase.Ibn.Data.Services.TreeService ts = fParent.GetService<Mediachase.Ibn.Data.Services.TreeService>();
					foreach (Mediachase.Ibn.Data.Services.TreeNode tN in ts.GetChildNodes())
					{
						MetaObject moFolder = tN.InnerObject;
						ListFolder folder = new ListFolder(moFolder.PrimaryKeyId.Value);
						int iFolderId = folder.PrimaryKeyId.Value;

						node = new JsonTreeNode();

						node.text = folder.Title;
						node.cls = "nodeCls";

						node.id = Guid.NewGuid().ToString("N");
						node.itemid = iFolderId.ToString();
						node.staticParentId = _rootId;

						node.href = "../../../Apps/ListApp/Pages/ListInfoList.aspx?ListFolderId=" + iFolderId.ToString();
						node.hrefTarget = "right";

						if (!folder.HasChildren)
							node.leaf = true;

						nodes.Add(node);
					}
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
