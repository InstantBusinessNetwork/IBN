using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using System.Xml;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for GenerateProjectByTemplate.
	/// </summary>
	public partial class GenerateProjectByTemplate : System.Web.UI.Page
	{
		#region Html fields
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(GenerateProjectByTemplate).Assembly);

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

		#region TemplateId
		private int TemplateId
		{
			get
			{
				try
				{
					return int.Parse(Request["TemplateId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		private DataTable GetResTable
		{
			get
			{
				if (ViewState["Prj_Ass"] != null)
					return (DataTable)ViewState["Prj_Ass"];

				return new DataTable();
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			//DV 01.10.2006 Fix bug with empty roles.
			if (Task.MakeProjectAssignments(TemplateId).Rows.Count == 0)
			{
				//Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), 
				//  "window.close();", true);
				//        return;
			}

			btnSearch.Text = LocRM.GetString("tFindNow");
			if (!IsPostBack)
			{
				btnAdd.InnerText = LocRM.GetString("tAdd");
				GetRolesData();
				GenerateExistUsers();
				BindGroups();
				BindRepeator();
			}
			BindToolbar();

			if (ddRole.Items.Count != 0)
			{
				lbUsers.Attributes.Add("ondblclick", "DisableButtons(this);" + Page.ClientScript.GetPostBackEventReference(btnAdd, ""));
				btnAdd.Attributes.Add("onclick", "DisableButtons(this);");
			}
			else
				btnAdd.Disabled = true;
		}

		#region GetRolesData
		private void GetRolesData()
		{
			DataTable dtRoles = Task.MakeProjectAssignments(TemplateId);
			DataView dv = dtRoles.DefaultView;
			dv.Sort = "RoleName";
			ddRole.DataSource = dv;
			ddRole.DataBind();
			DataTable dtAssign = dtRoles.Clone();
			ViewState["Prj_Ass"] = dtAssign;
			ViewState["Roles"] = dtRoles;
		}
		#endregion

		#region BindRepeator
		private void BindRepeator()
		{
			DataTable dtRoles = (DataTable)ViewState["Roles"];
			DataView dv = dtRoles.DefaultView;
			dv.Sort = "RoleName";
			repRoles.DataSource = dv;
			repRoles.DataBind();
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tCreateFromTemplate");
			secHeader.AddLink("<img alt='' src='../Layouts/Images/saveitem.gif'/> " + LocRM.GetString("tSave"), "javascript:FuncSave();");

			secHeader.AddSeparator();
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tCancel"), "javascript:window.close();");
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			using (IDataReader reader = SecureGroup.GetListGroupsAsTree())
			{
				while (reader.Read())
				{
					string GroupName = CommonHelper.GetResFileString(reader["GroupName"].ToString());
					string GroupId = reader["GroupId"].ToString();
					int Level = (int)reader["Level"];
					for (int i = 1; i < Level; i++)
						GroupName = "  " + GroupName;
					ListItem item = new ListItem(GroupName, GroupId);

					ddGroups.Items.Add(item);
				}
			}

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindGroupUsers(int.Parse(li.Value));
		}
		#endregion

		#region BindGroupUsers
		private void BindGroupUsers(int GroupID)
		{
			lbUsers.Items.Clear();
			DataTable dt = GetResTable;

			int RoleId = -1;

			if (ddRole.Items.Count > 0)
				RoleId = int.Parse(ddRole.SelectedValue);
			//int RoleId = int.Parse(ddRole.SelectedValue);
			using (IDataReader rdr = SecureGroup.GetListActiveUsersInGroup(GroupID))
			{
				while (rdr.Read())
				{
					DataRow[] dr = dt.Select("PrincipalId = " + (int)rdr["UserId"] + " AND RoleId = " + RoleId);
					if (dr.Length == 0)
						lbUsers.Items.Add(new ListItem(rdr["LastName"].ToString() +
							" " + rdr["FirstName"].ToString(), rdr["UserId"].ToString()));
				}
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

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.repRoles.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.rep_ItemBound);
		}
		#endregion

		#region ddGroups_ChangeGroup
		protected void ddGroups_ChangeGroup(object sender, System.EventArgs e)
		{
			tbSearch.Text = "";
			ViewState["SearchString"] = null;

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindGroupUsers(int.Parse(li.Value));
		}
		#endregion

		#region ddRole_ChangeGroup
		protected void ddRole_ChangeGroup(object sender, System.EventArgs e)
		{
			if (ViewState["SearchString"] == null)
			{
				ListItem li = ddGroups.SelectedItem;
				if (li != null)
					BindGroupUsers(int.Parse(li.Value));
			}
			else
			{
				BindSearchedUsers(ViewState["SearchString"].ToString());
			}
		}
		#endregion

		#region btnSearch_Click
		protected void btnSearch_Click(object sender, System.EventArgs e)
		{
			if (tbSearch.Text != "")
			{
				ViewState["SearchString"] = tbSearch.Text;
				BindSearchedUsers(tbSearch.Text);
			}
		}
		#endregion

		#region BindSearchedUsers
		private void BindSearchedUsers(string searchstr)
		{
			DataTable dt = GetResTable;
			int RoleId = -1;
			if (ddRole.Items.Count > 0)
				RoleId = int.Parse(ddRole.SelectedValue);
			using (IDataReader rdr = Mediachase.IBN.Business.User.GetListUsersBySubstring(searchstr))
			{
				lbUsers.Items.Clear();
				while (rdr.Read())
				{
					DataRow[] dr = dt.Select("PrincipalId = " + (int)rdr["UserId"] + " AND RoleId = " + RoleId);
					if (dr.Length == 0)
						lbUsers.Items.Add(new ListItem(rdr["LastName"].ToString() + " " + rdr["FirstName"].ToString(), rdr["UserId"].ToString()));
				}
			}
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			DataTable dt = GetResTable;
			Task.MakeProjectFromTemplate(TemplateId, ProjectId, dt);
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
						  "try {window.opener.top.frames['right'].location.href='ProjectView.aspx?ProjectId=" + ProjectId + "';}" +
						  "catch (e){} window.close();", true);
		}
		#endregion

		#region btnDelete_Click
		protected void btnDelete_Click(object sender, System.EventArgs e)
		{
			int iPrincipalId = int.Parse(hidPrId.Value);
			hidPrId.Value = "";
			int iRoleId = int.Parse(hidRoleId.Value);
			hidRoleId.Value = "";
			DataTable dt = GetResTable;
			DataRow[] dr = dt.Select("PrincipalId = " + iPrincipalId.ToString() + " AND RoleId = " + iRoleId.ToString());
			if (dr.Length > 0)
				dt.Rows.Remove(dr[0]);
			ViewState["Prj_Ass"] = dt;
			BindUsers();
			BindRepeator();
		}
		#endregion

		#region btnAdd_Click
		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			DataTable dt = GetResTable;
			if (ddRole.SelectedItem != null)
			{
				int RoleId = int.Parse(ddRole.SelectedValue);
				foreach (ListItem li in lbUsers.Items)
					if (li.Selected)
					{
						DataRow dr = dt.NewRow();
						dr["PrincipalId"] = int.Parse(li.Value);
						dr["PrincipalName"] = li.Text;
						dr["RoleId"] = RoleId;
						dr["RoleName"] = ddRole.SelectedItem.Text;
						dt.Rows.Add(dr);
					}
				ViewState["Prj_Ass"] = dt;
			}
			BindUsers();
			BindRepeator();
		}
		#endregion

		#region BindUsers
		private void BindUsers()
		{
			if (ViewState["SearchString"] == null)
			{
				ListItem _li = ddGroups.SelectedItem;
				if (_li != null)
					BindGroupUsers(int.Parse(_li.Value));
			}
			else
			{
				BindSearchedUsers(ViewState["SearchString"].ToString());
			}
		}
		#endregion

		#region GenerateExistUsers
		void GenerateExistUsers()
		{
			DataTable dt = GetResTable;
			XmlDocument TempXml = new XmlDocument();

			if (this.TemplateId != -1)
			{
				using (IDataReader reader = ProjectTemplate.GetProjectTemplate(this.TemplateId))
				{
					if (reader.Read())
					{
						string s = System.Web.HttpUtility.HtmlDecode(reader["TemplateData"].ToString());
						TempXml.LoadXml(s);
					}
				}

				foreach (XmlNode node in TempXml.DocumentElement.SelectNodes("Resources/Resource"))
				{
					string name = string.Empty;
					string firstName = string.Empty;
					string lastName = string.Empty;
					int roleId = -1;

					if (node["UID"] != null)
						roleId = int.Parse(node["UID"].InnerText);

					if (node["Name"] != null)
						name = node["Name"].InnerText;

					if (name.IndexOf(" ") == -1)
						continue;

					lastName = name.Substring(name.IndexOf(" ")).Trim();
					if (lastName == string.Empty)
						continue;

					firstName = name.Split(' ')[0];

					using (IDataReader reader2 = Mediachase.IBN.Business.User.GetListUsersBySubstring(lastName))
					{
						while (reader2.Read())
						{
							if (firstName == (string)reader2["FirstName"] && lastName == (string)reader2["LastName"])
							{
								DataRow row = dt.NewRow();
								row["PrincipalId"] = (int)reader2["UserId"];
								row["PrincipalName"] = String.Format("{0} {1}", reader2["LastName"], reader2["FirstName"]);
								row["RoleId"] = roleId;
								row["RoleName"] = name;
								dt.Rows.Add(row);
							}
						}
					}
				}
			}

			ViewState["Prj_Ass"] = dt;
		}
		#endregion

		#region rep_ItemBound
		private void rep_ItemBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			int RoleId = int.Parse(DataBinder.Eval(e.Item.DataItem, "RoleId").ToString());
			Repeater repUsers = (Repeater)e.Item.FindControl("repUsers");
			Label lblNone = (Label)e.Item.FindControl("lblNone");

			if (repUsers != null)
			{
				DataTable dt = GetResTable;
				DataRow[] drMas = dt.Select("RoleId = " + RoleId);
				DataTable dtNew = dt.Clone();
				dtNew.Columns.Add(new DataColumn("Delete", typeof(string)));
				foreach (DataRow dr in drMas)
				{
					DataRow _dr = dtNew.NewRow();
					_dr.ItemArray = dr.ItemArray;
					_dr["Delete"] = String.Format("<a href=\"javascript:DeleteItem({0},{1})\"><img alt='' src='../layouts/images/delete.gif' width='16' height='16' border=0></a>", dr["PrincipalId"].ToString(), dr["RoleId"].ToString());
					dtNew.Rows.Add(_dr);
				}
				DataView dvNew = dtNew.DefaultView;
				dvNew.Sort = "PrincipalName";
				repUsers.DataSource = dvNew;
				repUsers.DataBind();
				if (repUsers.Items.Count > 0)
					lblNone.Visible = false;
				else
					lblNone.Text = LocRM.GetString("tNone");
			}
		}
		#endregion

	}
}
