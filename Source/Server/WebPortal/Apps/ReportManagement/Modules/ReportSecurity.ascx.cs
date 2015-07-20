using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using System.Resources;
using System.Reflection;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business.ReportSystem;
using Mediachase.UI.Web.Util;

namespace Mediachase.Ibn.Web.UI.ReportManagement.Modules
{
	public partial class ReportSecurity : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region ReportId
		protected PrimaryKeyId ReportId
		{
			get
			{
				if (Request["ReportId"] != null)
					return PrimaryKeyId.Parse(Request["ReportId"]);
				else
					return PrimaryKeyId.Empty;
			}
		}
		#endregion

		#region CommandName
		protected string CommandName
		{
			get
			{
				if (Request["CommandName"] != null)
					return Request["CommandName"];
				return String.Empty;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			this.ddGroups.SelectedIndexChanged += new EventHandler(ddGroups_SelectedIndexChanged);
			this.dgMembers.DeleteCommand += new DataGridCommandEventHandler(dgMembers_DeleteCommand);
			
			ApplyAttributes();
			if (!Page.IsPostBack)
			{
				GetACLData();
				BindGroups();
				BindRights();
				BinddgMembers();
			}
		}

		#region ApplyAttributes
		private void ApplyAttributes()
		{
			btnAdd.InnerText = LocRM.GetString("tAdd");
			dgMembers.Columns[1].HeaderText = LocRM.GetString("FieldName");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("tAllow");
			dgMembers.Columns[3].HeaderText = LocRM.GetString("tDeny");

			SaveButton.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Save").ToString();
			CancelButton.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Cancel").ToString();
			SaveButton.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			CancelButton.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");
			SaveButton.ServerClick += new EventHandler(SaveButton_ServerClick);
			CancelButton.Attributes.Add("onclick", Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));
		}

		
		#endregion

		#region GetACLData
		private void GetACLData()
		{
			ReportAcl acl = ReportAcl.GetAcl(ReportId);
			ViewState["ACL"] = acl;
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			ddGroups.Items.Add(new ListItem(LocRM.GetString("tRoles"), "0"));
			using (IDataReader reader = SecureGroup.GetListGroupsWithParameters(true, false, false, false, false, false, true, false, false, false, false))
			{
				while (reader.Read())
				{
					ddGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
				}
			}

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindUsers(int.Parse(li.Value));
		}
		#endregion

		#region BindUsers
		private void BindUsers(int GroupID)
		{
			ddUsers.Items.Clear();
			ReportAcl ACLr = (ReportAcl)ViewState["ACL"];
			ArrayList alList = new ArrayList();
			foreach (ReportAce ACEr in ACLr)
			{
				alList.Add((ACEr.IsRoleAce) ? ACEr.Role : ACEr.PrincipalId.Value.ToString());
			}
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("UserId", typeof(string)));
			dt.Columns.Add(new DataColumn("UserName", typeof(string)));
			DataRow dr;
			if (GroupID == 0)
			{
				if (!alList.Contains(UserRoleHelper.AdminRoleName))
				{
					dr = dt.NewRow();
					dr["UserId"] = UserRoleHelper.AdminRoleName;
					dr["UserName"] = LocRM.GetString("tAdmins");
					dt.Rows.Add(dr);
				}

				if (!alList.Contains(UserRoleHelper.PowerProjectManagerRoleName))
				{
					dr = dt.NewRow();
					dr["UserId"] = UserRoleHelper.PowerProjectManagerRoleName;
					dr["UserName"] = LocRM.GetString("tPPManager");
					dt.Rows.Add(dr);
				}

				if (!alList.Contains(UserRoleHelper.HelpDeskManagerRoleName))
				{
					dr = dt.NewRow();
					dr["UserId"] = UserRoleHelper.HelpDeskManagerRoleName;
					dr["UserName"] = LocRM.GetString("tHDM");
					dt.Rows.Add(dr);
				}

				if (!alList.Contains(UserRoleHelper.GlobalProjectManagerRoleName))
				{
					dr = dt.NewRow();
					dr["UserId"] = UserRoleHelper.GlobalProjectManagerRoleName;
					dr["UserName"] = LocRM.GetString("tGlobalPManager");
					dt.Rows.Add(dr);
				}

				if (!alList.Contains(UserRoleHelper.GlobalExecutiveManagerRoleName))
				{
					dr = dt.NewRow();
					dr["UserId"] = UserRoleHelper.GlobalExecutiveManagerRoleName;
					dr["UserName"] = LocRM.GetString("tGlobalEManager");
					dt.Rows.Add(dr);
				}

				if (!alList.Contains(UserRoleHelper.TimeManagerRoleName))
				{
					dr = dt.NewRow();
					dr["UserId"] = UserRoleHelper.TimeManagerRoleName;
					dr["UserName"] = LocRM.GetString("tTM");
					dt.Rows.Add(dr);
				}
			}
			else
				using (IDataReader reader = SecureGroup.GetListAllUsersInGroup(GroupID))
				{
					while (reader.Read())
					{
						int iUserId = (int)reader["UserId"];
						if (alList.Contains(iUserId.ToString()))
							continue;
						bool IsExternal = (bool)reader["IsExternal"];
						byte Activity = (byte)reader["Activity"];
						if (!(IsExternal || Activity == (byte)Mediachase.IBN.Business.User.UserActivity.Pending))
						{
							dr = dt.NewRow();
							dr["UserId"] = iUserId.ToString();
							dr["UserName"] = (string)reader["LastName"] + " " + (string)reader["FirstName"];
							dt.Rows.Add(dr);
						}
					}
				}
			DataView dv = new DataView(dt);

			ddUsers.DataSource = dv;
			ddUsers.DataTextField = "UserName";
			ddUsers.DataValueField = "UserId";
			ddUsers.DataBind();
			btnAdd.Disabled = false;
			if (GroupID > 0)
				ddUsers.Items.Insert(0, new ListItem(LocRM.GetString("tAny"), "0"));
			else if (ddUsers.Items.Count == 0)
			{
				ddUsers.Items.Insert(0, new ListItem(LocRM.GetString("tEmpty"), "0"));
				btnAdd.Disabled = true;
			}
		}
		#endregion

		#region BindRights
		private void BindRights()
		{
			rbList.Items.Add(new ListItem(LocRM.GetString("tAllow"), "0"));
			rbList.Items.Add(new ListItem(LocRM.GetString("tDeny"), "1"));
			rbList.Items[0].Selected = true;
		}
		#endregion

		#region BinddgMembers
		private void BinddgMembers()
		{
			ReportAcl ACLr = (ReportAcl)ViewState["ACL"];
			DataTable dt = ACLToDateTable(ACLr);
			DataView dv = dt.DefaultView;
			dv.Sort = "Weight";
			dgMembers.DataSource = dv;
			dgMembers.DataBind();

			foreach (DataGridItem dgi in dgMembers.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				ib.Attributes.Add("onclick", "if(confirm('" + LocRM.GetString("Warning") + "'))return true;else return false;");
				ib.ToolTip = LocRM.GetString("tDelete");
			}
		}
		#endregion

		#region ACLToDateTable
		private DataTable ACLToDateTable(ReportAcl ACLr)
		{
			DataTable dt = new DataTable();
			DataRow dr;

			dt.Columns.Add(new DataColumn("ID", typeof(int)));
			dt.Columns.Add(new DataColumn("Weight", typeof(int)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("Allow", typeof(bool)));
			foreach (ReportAce ACEr in ACLr)
			{
				dr = dt.NewRow();
				dr["ID"] = ACEr.Id;
				dr["Weight"] = (ACEr.IsRoleAce) ? 0 : ((Mediachase.IBN.Business.User.IsGroup(ACEr.PrincipalId.Value)) ? 1 : 2);
				dr["Name"] = (ACEr.IsRoleAce) ? ACEr.Role : ACEr.PrincipalId.Value.ToString();
				dr["Allow"] = ACEr.Allow;
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		#region DataGrid Protected Strings
		protected string GetLink(int Weight, string Name)
		{
			if (Weight == 0)
			{
				switch (Name)
				{
					case UserRoleHelper.AdminRoleName:
						return String.Format("<img src='{1}' border=0 align='absmiddle'>&nbsp;{0}", 
												LocRM.GetString("tAdmins"),
												ResolveClientUrl("~/layouts/images/icons/admins.GIF"));
					case UserRoleHelper.PowerProjectManagerRoleName:
						return String.Format("<img src='{1}' border=0 align='absmiddle'>&nbsp;{0}", 
												LocRM.GetString("tPPManager"),
												ResolveClientUrl("~/layouts/images/icons/admins.GIF"));
					case UserRoleHelper.HelpDeskManagerRoleName:
						return String.Format("<img src='{1}' border=0 align='absmiddle'>&nbsp;{0}", 
												LocRM.GetString("tHDM"),
												ResolveClientUrl("~/layouts/images/icons/admins.GIF"));
					case UserRoleHelper.GlobalProjectManagerRoleName:
						return String.Format("<img src='{1}' border=0 align='absmiddle'>&nbsp;{0}", 
												LocRM.GetString("tGlobalPManager"),
												ResolveClientUrl("~/layouts/images/icons/admins.GIF"));
					case UserRoleHelper.GlobalExecutiveManagerRoleName:
						return String.Format("<img src='{1}' border=0 align='absmiddle'>&nbsp;{0}", 
												LocRM.GetString("tGlobalEManager"),
												ResolveClientUrl("~/layouts/images/icons/admins.GIF"));
					case UserRoleHelper.TimeManagerRoleName:
						return String.Format("<img src='{1}' border=0 align='absmiddle'>&nbsp;{0}", 
												LocRM.GetString("tTM"),
												ResolveClientUrl("~/layouts/images/icons/admins.GIF"));
					default:
						return Name;
				}
			}
			else if (Weight == 1)
				return CommonHelper.GetGroupLinkUL(int.Parse(Name));
			else
				return CommonHelper.GetUserStatusUL(int.Parse(Name));
		}
		#endregion

		#region ddGroups - ItemChange
		private void ddGroups_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindUsers(int.Parse(li.Value));
		}
		#endregion

		#region AddElement
		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			ReportAcl ACLr = (ReportAcl)ViewState["ACL"];
			ReportAce ACEr;
			if (ddGroups.SelectedValue == "0")
				ACEr = new ReportAce(ddUsers.SelectedValue, (rbList.SelectedValue == "0"));
			else
			{
				int iPrincipalId = (ddUsers.SelectedValue == "0") ? int.Parse(ddGroups.SelectedValue) : int.Parse(ddUsers.SelectedValue);
				ACEr = new ReportAce(iPrincipalId, (rbList.SelectedValue == "0"));
			}
			ACLr.Add(ACEr);

			ViewState["ACL"] = ACLr;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindUsers(int.Parse(_li.Value));
			BinddgMembers();
		}
		#endregion

		#region DeleteRight
		private void dgMembers_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			ReportAcl ACLr = (ReportAcl)ViewState["ACL"];
			int Id = int.Parse(e.Item.Cells[0].Text);

			ACLr.Remove(Id);

			ViewState["ACL"] = ACLr;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindUsers(int.Parse(_li.Value));
			BinddgMembers();
		}
		#endregion

		#region Save
		private void SaveButton_ServerClick(object sender, EventArgs e)
		{
			ReportAcl ACLr = (ReportAcl)ViewState["ACL"];
			ReportAcl.SetAcl(ACLr);

			CommandParameters cp = new CommandParameters(CommandName);
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion
	}
}