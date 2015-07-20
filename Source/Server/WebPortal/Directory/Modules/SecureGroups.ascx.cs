namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using ComponentArt.Web.UI;

	/// <summary>
	///		Summary description for SecureGroups.
	/// </summary>
	public partial class SecureGroups : System.Web.UI.UserControl
	{

		#region HTML Vars
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strSecureGroups", typeof(SecureGroups).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(SecureGroups).Assembly);

		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		private int GroupID = 1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			AssignGroupID();
			pc["SecureGroup_CurrentGroup"] = GroupID.ToString();
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();
			BinddgGroupsUsers();
		}
		#endregion

		#region AssignGroupID
		private void AssignGroupID()
		{
			try
			{
				if (Request["SGroupID"] != null)
					GroupID = int.Parse(Request["SGroupID"]);
				else
				{
					GroupID = int.Parse(pc["SecureGroup_CurrentGroup"]);
					using (IDataReader rdr = SecureGroup.GetGroup(GroupID))
					{
						if (!rdr.Read())
						{
							GroupID = 1;
							pc["SecureGroup_CurrentGroup"] = "1";
						}
					}
				}
			}
			catch
			{
				pc["SecureGroup_CurrentGroup"] = "1";
				GroupID = 1;
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			using (IDataReader reader = SecureGroup.GetGroup(GroupID))
			{
				if (reader.Read())
					secHeader.Title = CommonHelper.GetResFileString((string)reader["GroupName"]);
			}

			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM2.GetString("Actions");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";
			ComponentArt.Web.UI.MenuItem subItem;

			#region new group
			if (SecureGroup.CanCreate(GroupID))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/newgroup.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Directory/AddGroup.aspx?Type=0&GroupID=" + GroupID.ToString();
				subItem.Text = LocRM.GetString("AddGroup");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region edit group
			if (SecureGroup.CanUpdate() && GroupID > 9)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/editgroup.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Directory/AddGroup.aspx?Type=0&Edit=1&GroupID=" + GroupID.ToString();
				subItem.Text = LocRM.GetString("EditGroup");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region move
			if (SecureGroup.CanMove(GroupID))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/movegroup.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Directory/MoveGroup.aspx?GroupID=" + GroupID.ToString();
				subItem.Text = LocRM.GetString("MoveGroup");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region delete
			if (SecureGroup.CanDelete(GroupID))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/deletegroup.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:DeleteGroup()";
				subItem.Text = LocRM.GetString("DeleteGroup");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region new user
			if (User.CanCreateExternal() || User.CanCreatePending() || User.CanCreate())
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/newuser.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Directory/MultipleUserEdit.aspx?GroupID=" + GroupID;
				subItem.Text = LocRM.GetString("AddUser");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			if (topMenuItem.Items.Count > 0)
				secHeader.ActionsMenu.Items.Add(topMenuItem);
		}
		#endregion

		#region BinddgGroupsUsers
		private void BinddgGroupsUsers()
		{
			dgGroupsUsers.Columns[2].HeaderText = LocRM.GetString("GroupUser");
			dgGroupsUsers.Columns[3].HeaderText = LocRM.GetString("Email");

			DataTable dt = new DataTable();
			dt.Columns.Add("ObjectId", typeof(int));
			dt.Columns.Add("Type");
			dt.Columns.Add("Title");
			dt.Columns.Add("Email");
			dt.Columns.Add("sortTitle");
			dt.Columns.Add("ActionView");
			dt.Columns.Add("ActionEdit");
			dt.Columns.Add("HasChildren", typeof(bool));
			dt.Columns.Add("IsActive", typeof(bool));

			bool CanUpdate = SecureGroup.CanUpdate();
			DataRow dr;
			if (GroupID != 1) // [..]
			{
				int iParentId = SecureGroup.GetParentGroup(GroupID);
				dr = dt.NewRow();
				dr["ObjectId"] = 0;
				dr["Type"] = "Group";
				dr["Title"] = "<span style='padding-left:15px'>&nbsp;</span><a href='../Directory/directory.aspx?Tab=0&amp;SGroupID=" + iParentId.ToString() + "'>[..]</a>";
				dr["HasChildren"] = true;
				dr["IsActive"] = true;
				dt.Rows.Add(dr);
			}
			using (IDataReader reader = SecureGroup.GetListChildGroups(GroupID))
			{
				while (reader.Read())
				{
					dr = dt.NewRow();
					dr["ObjectId"] = reader["GroupId"];
					dr["Type"] = "Group";
					int iGroupId = (int)reader["GroupId"];
					dr["Title"] = CommonHelper.GetGroupLink(iGroupId, CommonHelper.GetResFileString((string)reader["GroupName"]));
					dr["sortTitle"] = CommonHelper.GetResFileString(reader["GroupName"].ToString());
					dr["HasChildren"] = reader["HasChildren"];
					dr["IsActive"] = true;

					if (CanUpdate && iGroupId > 9)
						dr["ActionEdit"] = "<a href='../Directory/AddGroup.aspx?Type=0&amp;Edit=1&amp;GroupID=" + iGroupId.ToString() + "'>" + "<img alt='' src='../layouts/images/edit.GIF'/></a>";
					dt.Rows.Add(dr);
				}
			}

			if (dt.Rows.Count == 1 && GroupID == 6)	// Partner
			{
				PartnerLabel.Text = LocRM.GetString("CreatePartnerGroup");
				PartnerLabel.Visible = true;
			}
			else
			{
				PartnerLabel.Visible = false;
			}

			//SELECT PrincipalId AS UserId, Login, FirstName, LastName, Email, IsActive, IMGroupId

			CanUpdate = User.CanUpdateUserInfo();
			bool bIsPartner = SecureGroup.IsPartner(GroupID);

			using (IDataReader reader = SecureGroup.GetListUsersInGroup(GroupID))
			{
				while (reader.Read())
				{

					byte Activity = (byte)reader["Activity"];
					bool IsExternal = (bool)reader["IsExternal"];

					dr = dt.NewRow();
					dr["ObjectId"] = reader["UserId"];

					dr["Type"] = "User";
					dr["Title"] = CommonHelper.GetUserStatus((int)reader["UserId"]);
					dr["sortTitle"] = CommonHelper.GetUserStatusPureName((int)reader["UserId"]);
					dr["Email"] = "<a href='mailto:" + (string)reader["Email"] + "'>" + (string)reader["Email"] + "</a>";
					if (CanUpdate)
					{
						if (Activity == (byte)User.UserActivity.Pending)
						{
							dr["ActionEdit"] = "<a href='../Directory/UserEdit.aspx?UserID=" + reader["UserId"].ToString() + "' title='" + LocRM.GetString("tApprove") + "'>"
								+ "<img alt='' src='../layouts/images/icon-key.GIF'/></a>";
						}
						else if (GroupID == (int)InternalSecureGroups.Partner || bIsPartner)
							dr["ActionEdit"] = "<a href='../Directory/PartnerEdit.aspx?UserID=" + reader["UserId"].ToString() + "' title='" + LocRM.GetString("tEdit") + "'>"
								+ "<img alt='' src='../layouts/images/edit.GIF'/></a>";
						else if (IsExternal)
							dr["ActionEdit"] = "<a href='../Directory/ExternalEdit.aspx?UserID=" + reader["UserId"].ToString() + "' title='" + LocRM.GetString("tEdit") + "'>"
								+ "<img alt='' src='../layouts/images/edit.GIF'/></a>";
						else
							dr["ActionEdit"] = "<a href='../Directory/UserEdit.aspx?UserID=" + reader["UserId"].ToString() + "' title='" + LocRM.GetString("tEdit") + "'>"
								+ "<img alt='' src='../layouts/images/edit.GIF'/></a>";
					}
					dr["ActionView"] = "<a href='../Directory/UserView.aspx?UserID=" + reader["UserId"].ToString() + "' title='" + LocRM.GetString("tViewDetails") + "'>"
						+ "<img alt='' src='../layouts/images/Icon-Search.GIF'/></a>";

					dr["HasChildren"] = false;
					dr["IsActive"] = (Activity > 1);

					dt.Rows.Add(dr);
				}
			}

			if (!SecureGroup.CanDelete())
				dgGroupsUsers.Columns[6].Visible = false;

			DataView dv = dt.DefaultView;
			dv.Sort = "Type, IsActive DESC, sortTitle";
			dgGroupsUsers.DataSource = dv;
			dgGroupsUsers.DataBind();

			foreach (DataGridItem dgi in dgGroupsUsers.Items)
			{
				int ID = int.Parse(dgi.Cells[0].Text);
				bool UserCanDelete = User.CanDelete(ID);
				string type = dgi.Cells[1].Text;
				bool HasChildren = bool.Parse(dgi.Cells[7].Text);

				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (type == "Group" && !HasChildren && ID >= 9)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
				else
					if (type != "Group" && UserCanDelete)
						ib.Attributes.Add("onclick", String.Format("return DeleteUser({0}, {1})", ID, GroupID));
					else
						ib.Visible = false;
			}

		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgGroupsUsers.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgGroupsUsers_Delete);
		}
		#endregion

		#region Events
		private void dgGroupsUsers_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int GID = int.Parse(e.Item.Cells[0].Text);
			string Type = e.Item.Cells[1].Text;
			if (Type == "Group")
				SecureGroup.Delete(GID);
		}

		protected void DeleteGroup(object sender, System.EventArgs e)
		{
			SecureGroup.Delete(GroupID);
			pc["SecureGroup_CurrentGroup"] = "1";
			Response.Redirect("~/Directory/Directory.aspx?Tab=0&SGroupID=1");
		}

		protected void btnRefresh_Click(object sender, System.EventArgs e)
		{
		}
		#endregion
	}
}
