namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
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
	public partial class IMGroups : System.Web.UI.UserControl
	{
		#region HTML Vars
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strIMGroups", typeof(IMGroups).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(IMGroups).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region GroupID
		private int GroupID
		{
			get
			{
				try
				{
					if (Request["IMGroupID"] != null)
					{
						return int.Parse(Request["IMGroupID"]);
					}
					else
						return int.Parse(pc["IMGroup_CurrentGroup"]);
				}
				catch
				{
					int groupid = 1;
					using (IDataReader rdr = User.GetUserInfo(Security.CurrentUser.UserID))
					{
						if (rdr.Read())
						{
							groupid = (int)rdr["IMGroupId"];
						}
					}
					return groupid;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (pc["IMGroup_CurrentGroup"] == null)
				pc["IMGroup_CurrentGroup"] = GroupID.ToString();
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BinddgGroupsUsers();
			BindToolbar();
		}

		#region BindToolbar()
		private void BindToolbar()
		{
			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM2.GetString("Actions");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";
			ComponentArt.Web.UI.MenuItem subItem;

			#region Create/Clone
			if (IMGroup.CanCreate())
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				if (GroupID == 0)
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/ibngroup_create.gif";
					subItem.NavigateUrl = "~/Directory/AddIMGroup.aspx";
					subItem.Text = LocRM.GetString("AddGroup");
				}
				else
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/ibngroupclone.gif";
					subItem.NavigateUrl = "~/Directory/AddIMGroup.aspx?CloneGroupID=" + GroupID;
					subItem.Text = LocRM.GetString("Clone");
				}
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Edit
			if (GroupID > 0 && IMGroup.CanUpdate())
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/ibngroup_edit.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Directory/AddIMGroup.aspx?GroupID=" + GroupID;
				subItem.Text = LocRM.GetString("EditGroup");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Delete
			if (GroupID > 0 && IMGroup.CanDelete(GroupID))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/ibngroup_delete.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:DeleteGroup(" + GroupID.ToString() + ")";
				subItem.Text = LocRM.GetString("DeleteGroup");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region new user
			if (GroupID > 0 && IMGroup.CanUpdate())
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/newuser.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Directory/MultipleUserEdit.aspx";
				subItem.Text = LocRM.GetString("AddUser");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			if (topMenuItem.Items.Count > 0)
				secHeader.ActionsMenu.Items.Add(topMenuItem);

			if (GroupID > 0)
			{
				string imGroupName = IMGroup.GetIMGroupName(GroupID, null);
				if (imGroupName != null)
					secHeader.Title = imGroupName;
			}
			else
				secHeader.Title = LocRM.GetString("tContactGroups");
		}
		#endregion

		#region BinddgGroupsUsers
		private void BinddgGroupsUsers()
		{
			dgUsers.Columns[1].HeaderText = LocRM.GetString("GroupUser");
			dgUsers.Columns[2].HeaderText = LocRM.GetString("Email");

			if (GroupID == 0)
				dgUsers.Columns[2].Visible = false;

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("ID", typeof(int)));
			dt.Columns.Add(new DataColumn("Type", typeof(int))); //0-IMGroup, 1- User
			dt.Columns.Add(new DataColumn("GroupName", typeof(string)));
			dt.Columns.Add(new DataColumn("Email", typeof(string)));
			dt.Columns.Add(new DataColumn("ActionEdit", typeof(string)));
			dt.Columns.Add(new DataColumn("ActionDelete", typeof(string)));

			DataView dv;
			DataRow dr;
			if (GroupID > 0) // Bind Users
			{
				// [..]
				dr = dt.NewRow();
				dr["ID"] = 0;
				dr["Type"] = 0;
				dr["GroupName"] = "<span style='padding-left:30px'>&nbsp;</span><a href='../Directory/directory.aspx?Tab=1&amp;IMGroupID=0'>[..]</a>";
				dt.Rows.Add(dr);

				using (IDataReader rdr = IMGroup.GetListUsers(GroupID))
				{
					while (rdr.Read())
					{
						dr = dt.NewRow();
						int iUserId = (int)rdr["UserId"];
						dr["ID"] = iUserId;
						dr["Type"] = 1;
						dr["Email"] = rdr["Email"].ToString();
						if (User.CanUpdateUserInfo(iUserId))
							dr["ActionEdit"] = String.Format("<a href='../Directory/UserEdit.aspx?UserID={0}'><img alt='{1}' src='../Layouts/Images/edit.gif' title='{1}'/></a>", iUserId.ToString(), LocRM.GetString("Edit"));
						if (User.CanDelete(iUserId))
							dr["ActionDelete"] = String.Format("<a href='javascript:DeleteUser({0})'><img alt='{1}' src='../Layouts/Images/delete.gif' title='{1}'/></a>", iUserId.ToString(), LocRM.GetString("Delete"));
						dt.Rows.Add(dr);
					}
				}
				dv = dt.DefaultView;
			}
			else  // Bind IMGroups
			{
				int userIMGroupId = 0;

				if (!IMGroup.CanCreate())
				{
					using (IDataReader rdr = User.GetUserInfo(Security.CurrentUser.UserID))
					{
						rdr.Read();
						if (rdr["IMGroupId"] != DBNull.Value)
							userIMGroupId = (int)rdr["IMGroupId"];
					}

					if (userIMGroupId > 0)
					{
						using (DataTable table = IMGroup.GetListIMGroupsYouCanSee(userIMGroupId))
						{
							foreach (DataRow row in table.Rows)
							{
								dr = dt.NewRow();

								int groupId = (int)row["IMGroupId"];
								dr["ID"] = groupId;
								dr["Type"] = 0;
								dr["GroupName"] = row["IMGroupName"].ToString();
								if (IMGroup.CanUpdate())
									dr["ActionEdit"] = String.Format("<a href='../Directory/AddIMGroup.aspx?GroupID={0}'><img alt='{1}' src='../Layouts/Images/edit.gif' title='{1}'/></a>", groupId, LocRM.GetString("Edit"));
								if (IMGroup.CanDelete(groupId))
									dr["ActionDelete"] = String.Format("<a href='javascript:DeleteGroup({0})'><img alt='{1}' src='../Layouts/Images/delete.gif' title='{1}'/></a>", groupId, LocRM.GetString("Delete"));

								dt.Rows.Add(dr);
							}
						}

						string imGroupName = IMGroup.GetIMGroupName(userIMGroupId, null);
						if (imGroupName != null)
						{
							dr = dt.NewRow();

							dr["ID"] = userIMGroupId;
							dr["Type"] = 0;
							dr["GroupName"] = imGroupName;
							if (IMGroup.CanUpdate())
								dr["ActionEdit"] = String.Format("<a href='../Directory/AddIMGroup.aspx?GroupID={0}'><img alt='{1}' src='../Layouts/Images/edit.gif' title='{1}'/></a>", userIMGroupId, LocRM.GetString("Edit"));

							dt.Rows.Add(dr);
						}
					}
				}
				else
				{
					using (DataTable table = IMGroup.GetListIMGroup())
					{
						foreach (DataRow row in table.Rows)
						{
							dr = dt.NewRow();

							int groupId = (int)row["IMGroupId"];
							dr["ID"] = groupId;
							dr["Type"] = 0;
							dr["GroupName"] = row["IMGroupName"].ToString();
							if (IMGroup.CanUpdate())
								dr["ActionEdit"] = string.Format("<a href='../Directory/AddIMGroup.aspx?GroupID={0}'><img alt='{1}' src='../Layouts/Images/edit.gif' title='{1}'/></a>", groupId, LocRM.GetString("Edit"));
							if (IMGroup.CanDelete(groupId))
								dr["ActionDelete"] = string.Format("<a href='javascript:DeleteGroup({0})'><img alt='{1}' src='../Layouts/Images/delete.gif' title='{1}'/></a>", groupId, LocRM.GetString("Delete"));

							dt.Rows.Add(dr);
						}
					}
				}
				dv = dt.DefaultView;
				dv.Sort = "GroupName";
			}

			dgUsers.DataSource = dv;
			dgUsers.DataBind();
		}
		#endregion

		#region Grid Strings
		protected string GetName(int Type, int ID, string GroupName)
		{
			if (Type == 0 && ID > 0) // IMGroup
				return String.Format("<a href='../Directory/Directory.aspx?Tab=1&amp;IMGroupID={0}'><img alt='' src='../Layouts/Images/icons/ibngroup.GIF'> {1}</a>", ID.ToString(), GroupName);
			else if (Type == 0 && ID == 0) // [..]
				return GroupName;
			else //User
				return CommonHelper.GetUserStatus(ID);
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
		}
		#endregion

		#region Events
		protected void DeleteGroup(object sender, System.EventArgs e)
		{
			int delID = int.Parse(deleteId.Value);
			IMGroup.Delete(delID);
			pc["IMGroup_CurrentGroup"] = "0";
			Response.Redirect("../Directory/Directory.aspx?Tab=1&IMGroupId=0");
		}

		protected void btnRefresh_Click(object sender, System.EventArgs e)
		{
		}
		#endregion
	}
}
