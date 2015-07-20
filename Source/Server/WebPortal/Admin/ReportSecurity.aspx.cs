using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.UI.Web.Modules;
using Mediachase.UI.Web.Util;
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Admin
{
	/// <summary>
	/// Summary description for ReportSecurity.
	/// </summary>
	public partial class ReportSecurity : System.Web.UI.Page
	{
		#region HTML Vars
		#endregion

		private UserLightPropertyCollection pc=Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region ReportId
		protected int ReportId
		{
			get
			{
				if(Request["ReportId"]!=null)
					return int.Parse(Request["ReportId"]);
				else
					return -1;
			}
		}
		#endregion

		BaseIbnContainer bic;
		Mediachase.IBN.Business.ControlSystem.ReportStorage rs;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			ApplyAttributes();
			BindToolbar();
			if(!Page.IsPostBack)
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
			bic = BaseIbnContainer.Create("Reports", "GlobalReports");
			rs = (ReportStorage)bic.LoadControl("ReportStorage");

			btnSave.Attributes.Add("onclick","DisableButtons(this);");
			btnAdd.InnerText = LocRM.GetString("tAdd");
			dgMembers.Columns[1].HeaderText = LocRM.GetString("FieldName");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("tAllow");
			dgMembers.Columns[3].HeaderText = LocRM.GetString("tDeny");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			ReportInfo ri = rs.GetReport(ReportId);
			string sName = ri.ShowName;
			if(sName.Length > 53)
				sName = sName.Substring(0, 50)+ "...";
//			secHeader.Title = LocRM.GetString("tEditSec")+" \""+sName+"\"";
			secHeader.Title = sName;
			secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/saveitem.gif") + "'/> " + LocRM.GetString("Save"),"javascript:FuncSave();");
			secHeader.AddSeparator();
			secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("Cancel"),"javascript:window.close();");
		}
		#endregion

		#region GetACLData
		private void GetACLData()
		{
			ReportAccessControList ACLr = ReportAccessControList.GetACL(ReportId);
			ViewState["ACL"] = ACLr;
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
			if (li!=null)
				BindUsers(int.Parse(li.Value));
		}
		#endregion

		#region BindUsers
		private void BindUsers(int GroupID)
		{
			ddUsers.Items.Clear();
			ReportAccessControList ACLr = (ReportAccessControList)ViewState["ACL"];
			ArrayList alList = new ArrayList();
			foreach (ReportAccessControlEntry ACEr in ACLr)
			{
				alList.Add((ACEr.IsRoleAce)? ACEr.Role : ACEr.PrincipalId.ToString());
			}
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("UserId", typeof(string)));					// 0
			dt.Columns.Add(new DataColumn("UserName", typeof(string)));				// 1
			DataRow dr;
			if(GroupID==0)
			{
				if(!alList.Contains(UserRoleHelper.AdminRoleName))
				{
					dr = dt.NewRow();
					dr[0] = UserRoleHelper.AdminRoleName;
					dr[1] = LocRM.GetString("tAdmins");
					dt.Rows.Add(dr);
				}

				if(!alList.Contains(UserRoleHelper.PowerProjectManagerRoleName))
				{
					dr = dt.NewRow();
					dr[0] = UserRoleHelper.PowerProjectManagerRoleName;
					dr[1] = LocRM.GetString("tPPManager");
					dt.Rows.Add(dr);
				}

				if(!alList.Contains(UserRoleHelper.HelpDeskManagerRoleName))
				{
					dr = dt.NewRow();
					dr[0] = UserRoleHelper.HelpDeskManagerRoleName;
					dr[1] = LocRM.GetString("tHDM");
					dt.Rows.Add(dr);
				}

				if(!alList.Contains(UserRoleHelper.GlobalProjectManagerRoleName))
				{
					dr = dt.NewRow();
					dr[0] = UserRoleHelper.GlobalProjectManagerRoleName;
					dr[1] = LocRM.GetString("tGlobalPManager");
					dt.Rows.Add(dr);
				}

				if(!alList.Contains(UserRoleHelper.GlobalExecutiveManagerRoleName))
				{
					dr = dt.NewRow();
					dr[0] = UserRoleHelper.GlobalExecutiveManagerRoleName;
					dr[1] = LocRM.GetString("tGlobalEManager");
					dt.Rows.Add(dr);
				}

				if (!alList.Contains(UserRoleHelper.TimeManagerRoleName))
				{
					dr = dt.NewRow();
					dr[0] = UserRoleHelper.TimeManagerRoleName;
					dr[1] = LocRM.GetString("tTM");
					dt.Rows.Add(dr);
				}
			}
			else
				using (IDataReader reader = SecureGroup.GetListAllUsersInGroup(GroupID))
				{
					while (reader.Read())
					{
						int iUserId = (int)reader["UserId"];
						if(alList.Contains(iUserId.ToString()))
							continue;
						bool IsExternal = (bool)reader["IsExternal"];
						byte Activity = (byte)reader["Activity"];
						if (!(IsExternal || Activity == (byte)Mediachase.IBN.Business.User.UserActivity.Pending))
						{
							dr = dt.NewRow();
							dr[0] = iUserId.ToString();
							dr[1] = (string)reader["LastName"] + " " + (string)reader["FirstName"];
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
			if(GroupID>0)
				ddUsers.Items.Insert(0, new ListItem(LocRM.GetString("tAny"), "0"));
			else if(ddUsers.Items.Count==0)
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
			ReportAccessControList ACLr = (ReportAccessControList)ViewState["ACL"];
			DataTable dt = ACLToDateTable(ACLr);
			DataView dv = dt.DefaultView;
			dv.Sort = "Weight";
			dgMembers.DataSource = dv;
			dgMembers.DataBind();
			
			foreach (DataGridItem dgi in dgMembers.Items)
			{
				ImageButton ib=(ImageButton)dgi.FindControl("ibDelete");
				ib.Attributes.Add("onclick","if(confirm('" + LocRM.GetString("Warning") +"')){DisableButtons(this);return true;}else return false;");
				ib.ToolTip=LocRM.GetString("tDelete");
			} 
		}
		#endregion

		#region ACLToDateTable
		private DataTable ACLToDateTable(ReportAccessControList ACLr)
		{
			DataTable dt = new DataTable();
			DataRow dr;

			dt.Columns.Add(new DataColumn("ID",typeof(int)));
			dt.Columns.Add(new DataColumn("Weight",typeof(int)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("Allow",typeof(bool)));
			foreach (ReportAccessControlEntry ACEr in ACLr)
			{
				dr = dt.NewRow();
				dr["ID"] = ACEr.Id;
				dr["Weight"] = (ACEr.IsRoleAce)? 0 : ((Mediachase.IBN.Business.User.IsGroup(ACEr.PrincipalId))? 1 : 2);
				dr["Name"] = (ACEr.IsRoleAce)? ACEr.Role : ACEr.PrincipalId.ToString();
				dr["Allow"]=ACEr.Allow;
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		#region DataGrid Protected Strings
		protected string GetLink(int Weight, string Name)
		{
			if(Weight==0)
			{
				switch(Name)
				{
					case UserRoleHelper.AdminRoleName:
						return String.Format("<img alt='' src='{1}'/>&nbsp;{0}", 
							LocRM.GetString("tAdmins"), ResolveUrl("~/Layouts/Images/icons/admins.GIF"));
					case UserRoleHelper.PowerProjectManagerRoleName:
						return String.Format("<img alt='' src='{1}'/>&nbsp;{0}",
							LocRM.GetString("tPPManager"), ResolveUrl("~/Layouts/Images/icons/admins.GIF"));
					case UserRoleHelper.HelpDeskManagerRoleName:
						return String.Format("<img alt='' src='{1}'/>&nbsp;{0}",
							LocRM.GetString("tHDM"), ResolveUrl("~/Layouts/Images/icons/admins.GIF"));
					case UserRoleHelper.GlobalProjectManagerRoleName:
						return String.Format("<img alt='' src='{1}'/>&nbsp;{0}",
							LocRM.GetString("tGlobalPManager"), ResolveUrl("~/Layouts/Images/icons/admins.GIF"));
					case UserRoleHelper.GlobalExecutiveManagerRoleName:
						return String.Format("<img alt='' src='{1}'/>&nbsp;{0}",
							LocRM.GetString("tGlobalEManager"), ResolveUrl("~/Layouts/Images/icons/admins.GIF"));
					case UserRoleHelper.TimeManagerRoleName:
						return String.Format("<img alt='' src='{1}'/>&nbsp;{0}",
							LocRM.GetString("tTM"), ResolveUrl("~/Layouts/Images/icons/admins.GIF"));
					default:
						return Name;
				}
			}
			else if(Weight==1)
				return Util.CommonHelper.GetGroupLinkUL(int.Parse(Name));
			else 
				return Util.CommonHelper.GetUserStatusUL(int.Parse(Name));
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
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.ddGroups.SelectedIndexChanged += new EventHandler(ddGroups_SelectedIndexChanged);
			this.dgMembers.DeleteCommand += new DataGridCommandEventHandler(dgMembers_DeleteCommand);
			this.btnSave.Click += new EventHandler(btnSave_Click);
		}
		#endregion

		#region ddGroups - ItemChange
		private void ddGroups_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListItem li = ddGroups.SelectedItem;
			if (li!=null)
				BindUsers(int.Parse(li.Value));
		}
		#endregion

		#region AddElement
		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			ReportAccessControList ACLr = (ReportAccessControList)ViewState["ACL"];
			ReportAccessControlEntry ACEr;
			if(ddGroups.SelectedValue=="0")
				ACEr = new ReportAccessControlEntry(ddUsers.SelectedValue, "Read", (rbList.SelectedValue=="0"));
			else
			{
				int iPrincipalId = (ddUsers.SelectedValue=="0")? int.Parse(ddGroups.SelectedValue) : int.Parse(ddUsers.SelectedValue);
				ACEr = new ReportAccessControlEntry(iPrincipalId, "Read", (rbList.SelectedValue=="0"));
			}
			ACLr.Add(ACEr);

			ViewState["ACL"] = ACLr;

			ListItem _li = ddGroups.SelectedItem;
			if (_li!=null)
				BindUsers(int.Parse(_li.Value));
			BinddgMembers();
		}
		#endregion

		#region DeleteRight
		private void dgMembers_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			ReportAccessControList ACLr = (ReportAccessControList)ViewState["ACL"];
			int Id = int.Parse(e.Item.Cells[0].Text);

			ACLr.Remove(Id);

			ViewState["ACL"] = ACLr;

			ListItem _li = ddGroups.SelectedItem;
			if (_li!=null)
				BindUsers(int.Parse(_li.Value));
			BinddgMembers();
		}
		#endregion

		#region Save
		private void btnSave_Click(object sender, EventArgs e)
		{
			ReportAccessControList ACLr = (ReportAccessControList)ViewState["ACL"];
			ReportAccessControList.SetACL(rs, ACLr);
      Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"<script language=javascript>window.close();</script>");
		}
		#endregion
	}
}
