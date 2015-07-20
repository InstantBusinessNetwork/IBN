using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Lists;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls
{
	public partial class ListFolderEdit : System.Web.UI.UserControl
	{
		#region ListFolderId
		private int ListFolderId
		{
			get
			{
				try
				{
					return int.Parse(Request["ListFolderId"]);
				}
				catch
				{
					return -1;
				}
			}
		} 
		#endregion

		#region ProjectId
		private int ProjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
				}
				catch
				{
					return -1;
				}
			}
		} 
		#endregion

		#region parentFolderId
		private int parentFolderId
		{
			get
			{
				try
				{
					return int.Parse(Request["parentFolderId"]);
				}
				catch
				{
					ListFolder folder = new ListFolder(ListFolderId);
					return folder.ParentId.Value;
				}
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			RequiredFieldValidator1.ErrorMessage = CHelper.GetResFileString("{IbnFramework.ListInfo:Required}");
			btnCreateSave1.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:Save}");
			btnCancel.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:Cancel}");
			btnCreateSave1.Attributes.Add("onclick", "DisableButtons(this);");
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");

			BindToolBar();
			if (!IsPostBack)
				BindData();
		}

		#region BindToolBar
		private void BindToolBar()
		{
			if (ListFolderId > 0)
			{
				secHeader.Title = CHelper.GetResFileString("{IbnFramework.ListInfo:EditFolder}");
			}
			else
			{
				string sTitle = "";
				ListFolder folder = new ListFolder(parentFolderId);
				if (folder.FolderType != ListFolderType.Project)
				{
					if (folder.FolderType == ListFolderType.Private)
						sTitle = CHelper.GetResFileString("{IbnFramework.ListInfo:tPrivateLists}");
					else
						sTitle = CHelper.GetResFileString("{IbnFramework.ListInfo:tPublicLists}");
				}
				else
					sTitle = Task.GetProjectTitle(folder.ProjectId.Value);

				secHeader.Title = CHelper.GetResFileString("{IbnFramework.ListInfo:CreateFolder}") + " '" + sTitle + "'";
			}
		} 
		#endregion

		#region BindData
		private void BindData()
		{
			lblFolderTitle.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:FolderTitle}");
			if (ListFolderId > 0)
			{
				string fName = "";
				ListFolder folder = new ListFolder(ListFolderId);
				if (folder != null)
					fName = folder.Title;
				tbFolderTitle.Text = HttpUtility.HtmlDecode(fName);
			}
		} 
		#endregion

		#region Save
		protected void Button1_Click(object sender, System.EventArgs e)
		{
			if (tbFolderTitle.Text == "")
				return;
			if (ListFolderId > 0)
			{
				ListFolder folder = new ListFolder(ListFolderId);
				folder.Title = tbFolderTitle.Text;
				folder.Save();
			}
			else
				ListManager.CreateFolder(parentFolderId, tbFolderTitle.Text);
			
			if (ProjectId < 0)
				Response.Redirect("~/Apps/ListApp/Pages/ListInfoList.aspx?Tab=0&ListFolderId=" + parentFolderId.ToString());
			else
				Response.Redirect("~/Projects/ProjectView.aspx?Tab=Lists&ProjectId=" + ProjectId.ToString() + "&ListFolderId=" + parentFolderId.ToString());
		} 
		#endregion

		#region Cancel
		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			if (ProjectId < 0)
				Response.Redirect("~/Apps/ListApp/Pages/ListInfoList.aspx?Tab=0&ListFolderId=" + parentFolderId.ToString());
			else
				Response.Redirect("~/Projects/ProjectView.aspx?Tab=Lists&ProjectId=" + ProjectId.ToString() + "&ListFolderId=" + parentFolderId.ToString());
		} 
		#endregion
	}
}