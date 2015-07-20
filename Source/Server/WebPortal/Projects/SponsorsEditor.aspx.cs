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
using System.Globalization;
using System.Resources;

using Mediachase.Ibn;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for strSponsorsEditor.
	/// </summary>
	public partial class SponsorsEditor : System.Web.UI.Page
	{
		protected ResourceManager LocRM;

		#region ProjectID
		private int ProjectID
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectID"]);
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}
		#endregion

		#region BtnID
		protected string BtnID
		{
			get
			{
				return Request["BtnID"] != null ? Request["BtnID"] : String.Empty;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strSponsorsEditor", typeof(SponsorsEditor).Assembly);
			ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strTeamEditor", typeof(SponsorsEditor).Assembly);
			//btnSave.Attributes.Add("onclick","DisableButtons(this);");
			btnAdd.Attributes.Add("onclick", "DisableButtons(this);");
			btnAddGroup.Attributes.Add("onclick", "DisableButtons(this);");
			lbUsers.Attributes.Add("ondblclick", "DisableButtons(this);" + Page.ClientScript.GetPostBackEventReference(btnAdd, ""));
			lblError.Visible = false;

			if (!IsPostBack)
			{
				ViewState["SearchMode"] = false;
				btnAdd.InnerText = LocRM.GetString("Add");
				btnAddGroup.InnerText = LocRM.GetString("AddGroup");
				btnSearch.Text = LocRM.GetString("FindNow");
				lblError.Text = LocRM.GetString("EmrtySearch");
				SaveButton.Text = LocRM2.GetString("Save");
				CancelButton.Text = LocRM2.GetString("Cancel");
				GetProjectData();
				BindGroups();
				BinddgMemebers();
			}

			SaveButton.Attributes.Add("onclick", "FuncSave();");
			SaveButton.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			if (Request["closeFramePopup"] != null)
				CancelButton.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}", Request["closeFramePopup"]));
			else
				CancelButton.Attributes.Add("onclick", "FuncCancel(" + ProjectID + ");");
		}

		#region GetProjectData
		private void GetProjectData()
		{
			ViewState["Sponsors"] = Project.GetListSponsorsDataTable(ProjectID);
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			ddGroups.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "FavoritesGroup").ToString(), "0"));

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
		private void BindGroupUsers(int groupId)
		{
			lbUsers.Items.Clear();
			DataTable dt = (DataTable)ViewState["Sponsors"];
			if (groupId > 0)
			{
				using (IDataReader rdr = SecureGroup.GetListActiveUsersInGroup(groupId))
				{
					while (rdr.Read())
					{
						DataRow[] dr = dt.Select("PrincipalId = " + (int)rdr["UserId"]);
						if (dr.Length == 0)
							lbUsers.Items.Add(new ListItem((string)rdr["LastName"] + " " + (string)rdr["FirstName"], rdr["UserId"].ToString()));

					}
				}
			}
			else // Favorites
			{
				DataTable favorites = Mediachase.IBN.Business.Common.GetListFavoritesDT(ObjectTypes.User);
				foreach (DataRow row in favorites.Rows)
				{
					DataRow[] dr = dt.Select("PrincipalId = " + (int)row["ObjectId"]);
					if (dr.Length == 0)
						lbUsers.Items.Add(new ListItem((string)row["Title"], row["ObjectId"].ToString()));
				}
			}
		}
		#endregion

		#region BinddgMemebers
		private void BinddgMemebers()
		{
			dgMembers.Columns[1].HeaderText = LocRM.GetString("Name");

			DataTable dt = (DataTable)ViewState["Sponsors"];
			dgMembers.DataSource = dt.DefaultView;
			dgMembers.DataBind();

			foreach (DataGridItem dgi in dgMembers.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				ib.Attributes.Add("onclick", "if(confirm('" + LocRM.GetString("Warning") + "')) {DisableButtons(this); return true;} else return false;");
				ib.ToolTip = LocRM.GetString("Delete");
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
			this.dgMembers.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgMembers_Delete);
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

		#region btnAdd/Group_Click
		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			DataTable dt = (DataTable)ViewState["Sponsors"];
			foreach (ListItem li in lbUsers.Items)
				if (li.Selected)
				{
					DataRow dr = dt.NewRow();
					dr["PrincipalId"] = int.Parse(li.Value);
					dr["IsGroup"] = false;
					dt.Rows.Add(dr);
				}
			ViewState["Sponsors"] = dt;

			BindUsers();
			BinddgMemebers();
		}

		protected void btnAddGroup_Click(object sender, System.EventArgs e)
		{
			DataTable dt = (DataTable)ViewState["Sponsors"];
			if (ddGroups.SelectedItem != null && dt.Select("PrincipalId=" + ddGroups.SelectedItem.Value).Length == 0 && int.Parse(ddGroups.SelectedValue) > 0)
			{
				DataRow dr = dt.NewRow();
				dr["PrincipalId"] = int.Parse(ddGroups.SelectedItem.Value);
				dr["IsGroup"] = true;
				dt.Rows.Add(dr);
			}
			ViewState["Sponsors"] = dt;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindGroupUsers(int.Parse(_li.Value));
			BinddgMemebers();
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
				BindSearchedUsers((string)ViewState["SearchString"]);
			}
		}
		#endregion

		#region dgMembers_Delete
		private void dgMembers_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{

			int PrincipalId = int.Parse(e.Item.Cells[0].Text);
			DataTable dt = (DataTable)ViewState["Sponsors"];
			DataRow[] dr = dt.Select("PrincipalId = " + PrincipalId);
			if (dr.Length > 0)
				dt.Rows.Remove(dr[0]);
			ViewState["Sponsors"] = dt;

			BindUsers();
			BinddgMemebers();
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, System.EventArgs e)
		{

			DataTable dt = (DataTable)ViewState["Sponsors"];
			ArrayList aSponsors = new ArrayList();
			foreach (DataRow dr in dt.Rows)
				aSponsors.Add((int)dr["PrincipalId"]);

			Project2.UpdateSponsors(ProjectID, aSponsors);
			if (BtnID != String.Empty)
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
							"try {window.top.frames['right'].location.href='ProjectView.aspx?ProjectId=" + ProjectID + "';}" +
							"catch (e){}", true);
			else if (Request["closeFramePopup"] != null)
			{
				CommandParameters cp = new CommandParameters("MC_PM_SponsorsEdit");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
						"try {window.opener.top.frames['right'].location.href='../Projects/ProjectView.aspx?ProjectId=" + ProjectID + "';}" +
						"catch (e){} window.close();", true);
		}
		#endregion

		#region GetLink
		protected string GetLink(int PID, bool IsGroup)
		{
			if (IsGroup)
				return CommonHelper.GetGroupLinkUL(PID);
			else
				return CommonHelper.GetUserStatusUL(PID);
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
			else
				lblError.Visible = true;
		}
		#endregion

		#region BindSearchedUsers
		private void BindSearchedUsers(string searchstr)
		{
			DataTable dt = (DataTable)ViewState["Sponsors"];
			lbUsers.Items.Clear();

			DataView dv = Mediachase.IBN.Business.User.GetListUsersBySubstringDataTable(searchstr).DefaultView;
			dv.Sort = "LastName, FirstName";

			for (int i = 0; i < dv.Count; i++)
			{
				DataRow[] dr = dt.Select("PrincipalId = " + (int)dv[i]["UserId"]);
				if (dr.Length == 0)
					lbUsers.Items.Add(new ListItem((string)dv[i]["LastName"] + " " + (string)dv[i]["FirstName"], dv[i]["UserId"].ToString()));
			}
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			btnAddGroup.Disabled = false;
			if (int.Parse(ddGroups.SelectedValue) <= 0)
				btnAddGroup.Disabled = true;
		}
		#endregion
	}
}
