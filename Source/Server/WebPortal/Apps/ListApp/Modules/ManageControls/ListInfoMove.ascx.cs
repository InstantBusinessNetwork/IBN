using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using ComponentArt.Web.UI;

using Mediachase.Ibn.Lists;
using Mediachase.IBN.Business;
using System.Resources;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls
{
	public partial class ListInfoMove : System.Web.UI.UserControl
	{
		#region _listId
		private int _listId
		{
			get
			{
				int retVal = -1;
				int.TryParse(Request["ListId"], out retVal);
				return retVal;
			}
		} 
		#endregion

		#region _refreshButton
		private string _refreshButton
		{
			get
			{
				string retval = String.Empty;
				if (Request["btn"] != null)
					retval = Request["btn"];
				return retval;
			}
		}
		#endregion

		#region CommandName
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		/// <value>The name of the command.</value>
		protected string CommandName
		{
			get
			{
				string retval = String.Empty;
				if (Request["CommandName"] != null)
					retval = Request["CommandName"];
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				if (!ListInfoBus.CanAdmin(_listId))
					throw new AccessDeniedException();
			}

			rbList.SelectedIndexChanged += new EventHandler(rbList_SelectedIndexChanged);
			if (!Page.IsPostBack)
				BindData();

			btnMove.CustomImage = ResolveUrl("~/layouts/images/upload.gif");
			btnMove.ServerClick += new EventHandler(btnSave_ServerClick);
			btnMove.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tMove}");
			btnMove.Attributes.Add("onclick", "DisableButtons(this);");
			
			btnMoveAssign.CustomImage = ResolveUrl("~/layouts/images/upload.gif");
			btnMoveAssign.ServerClick += new EventHandler(btnMoveAssign_ServerClick);
			btnMoveAssign.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tMoveAssign}");
			btnMoveAssign.Attributes.Add("onclick", "DisableButtons(this);");
			
			ScriptManager sm = ScriptManager.GetCurrent(this.Page);
			if (sm != null)
			{
				sm.RegisterPostBackControl(btnMove);
				sm.RegisterPostBackControl(btnMoveAssign);
			}
			
			BindToolbar();
		}

		#region BindData
		private void BindData()
		{
			rbList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.ListInfo:tPrivateLists}"), "0"));
			rbList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.ListInfo:tPublicLists}"), "1"));
			if (Configuration.ProjectManagementEnabled)
			{
				rbList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.ListInfo:tPrjLists}"), "2"));
			}
			rbList.Width = Unit.Percentage(100);
			rbList.CellPadding = 5;

			ListInfo liInfo = new ListInfo(_listId);
			ListFolder folder = new ListFolder(liInfo.FolderId.Value);
			switch (folder.FolderType)
			{
				case ListFolderType.Public:
					rbList.SelectedValue = "1";
					trProject.Visible = false;
					break;
				case ListFolderType.Project:
					rbList.SelectedValue = "2";
					trProject.Visible = true;
					ucProject.ObjectId = folder.ProjectId.Value;
					ucProject.ObjectTypeId = (int)ObjectTypes.Project;
					break;
				default:
					rbList.SelectedValue = "0";
					trProject.Visible = false;
					break;
			}
			BindTree();
		} 
		#endregion

		#region BindTree
		private void BindTree()
		{
			int iFolderId = -1;
			switch (rbList.SelectedValue)
			{
				case "1":
					iFolderId = ListManager.GetPublicRoot().PrimaryKeyId.Value;
					break;
				case "2":
					int prjId = ucProject.ObjectId;
					if (prjId > 0)
						iFolderId = ListManager.GetProjectRoot(prjId).PrimaryKeyId.Value;
					break;
				default:
					iFolderId = ListManager.GetPrivateRoot(Mediachase.IBN.Business.Security.CurrentUser.UserID).PrimaryKeyId.Value;
					break;
			}

			MoveTree.Nodes.Clear();
			MoveTree.ClientSideOnNodeSelect = "onNodeClick";
			if (iFolderId > 0)
			{
				TreeViewNode node;
				ListFolder folder = new ListFolder(iFolderId);
				node = new TreeViewNode();
				if (folder.FolderType == ListFolderType.Private)
				{
					node.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tPrivateLists}");
					node.ID = "listfolder" + iFolderId.ToString() + "private";
				}
				else if (folder.FolderType == ListFolderType.Public)
				{
					node.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tPublicLists}");
					node.ID = "listfolder" + iFolderId.ToString();
				}
				else if (folder.FolderType == ListFolderType.Project)
				{
					node.Text = Task.GetProjectTitle(folder.ProjectId.Value);
					node.ID = "listfolder" + iFolderId.ToString() + "project";
				}
				node.Value = iFolderId.ToString();
				if(folder.HasChildren)
					node.ContentCallbackUrl = ResolveClientUrl("~/Modules/XmlForTreeView.aspx") + "?MoveList=" + _listId.ToString() + "&ListFolderId1=" + iFolderId.ToString();
				MoveTree.Nodes.Add(node);
			}
			btnMove.Disabled = true;
			btnMoveAssign.Disabled = true;
			btnMove.Style.Add("color", "#aeaeae");
			btnMoveAssign.Style.Add("color", "#aeaeae");
		} 
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = CHelper.GetResFileString("{IbnFramework.ListInfo:ListInfoMove}");
			secHeader.AddLink(
				String.Format("<img src='{0}' border='0' width='16' height='16' alt='' align='absmiddle' /> {1}",
					ResolveClientUrl("~/layouts/images/cancel.gif"), CHelper.GetResFileString("{IbnFramework.ListInfo:tClose}")),
				"javascript:window.close();");
		} 
		#endregion

		protected void lbChangeProject_Click(object sender, EventArgs e)
		{
			BindTree();
		}

		protected void rbList_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (rbList.SelectedValue)
			{
				case "2":
					trProject.Visible = true;
					break;
				default:
					trProject.Visible = false;
					break;
			}
			BindTree();
		}

		protected void btnSave_ServerClick(object sender, EventArgs e)
		{
			int iNewFolderId = int.Parse(destFolderId.Value);
			ListManager.MoveList(new ListInfo(_listId), new ListFolder(iNewFolderId));

			UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
			pc["ListInfo_FolderId_EntityList_GroupItemKey"] = iNewFolderId.ToString();

			CloseAndRefresh();
		}

		void btnMoveAssign_ServerClick(object sender, EventArgs e)
		{
			int iNewFolderId = int.Parse(destFolderId.Value);
			ListManager.MoveList(new ListInfo(_listId), new ListFolder(iNewFolderId));

			UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
			pc["ListInfo_FolderId_EntityList_GroupItemKey"] = iNewFolderId.ToString();

			if (String.IsNullOrEmpty(CommandName))
			{
				string script = String.Format("try{{window.opener.location.href='{0}';}}catch(e){{;}}window.close();",
					ResolveUrl("~/Apps/ListApp/Pages/ListInfoList.aspx?Assign=1&ListId=" + _listId + "&ListFolderId=" + iNewFolderId.ToString()));

				ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"),
					script, true);
			}
			else
			{
				CommandParameters cp = new CommandParameters(CommandName);
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, cp.ToString());
			}
		}

		#region CloseAndRefresh
		private void CloseAndRefresh()
		{
			if (String.IsNullOrEmpty(CommandName))
			{
				string script = String.Empty;
				if (_refreshButton != String.Empty)
				{
					script = String.Format("try{{window.opener.__doPostBack('{0}', '')}}catch(e){{;}}window.close();", _refreshButton);
				}
				else
					script = "window.close();";

				ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"),
					script, true);
			}
			else
			{
				CommandParameters cp = new CommandParameters(CommandName);
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, cp.ToString());
			}
		}
		#endregion
	}
}