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

using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for strTeamEditor.
	/// </summary>
	public partial class TeamEditor : System.Web.UI.Page
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

			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strTeamEditor", typeof(TeamEditor).Assembly);
			lbUsers.Attributes.Add("ondblclick", "DisableButtons(this);" + Page.ClientScript.GetPostBackEventReference(btnAdd, ""));
			//btnSave.Attributes.Add("onclick","DisableButtons(this);");
			btnAdd.Attributes.Add("onclick", "DisableButtons(this);");
			btnSearch.Text = LocRM.GetString("FindNow");
			lblError.Visible = false;

			if (!IsPostBack)
			{
				btnAdd.InnerText = LocRM.GetString("Add");
				lblError.Text = LocRM.GetString("EmrtySearch");
				GetProjectData();
				BindGroups();
				BinddgMemebers();

				SaveButton.Text = LocRM.GetString("Save");
				CancelButton.Text = LocRM.GetString("Cancel");
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
			DataTable dt = Project.GetListTeamMembersDataTable(ProjectID);
			ViewState["Team"] = dt;
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
			DataTable dt = (DataTable)ViewState["Team"];
			if (groupId > 0)
			{
				using (IDataReader rdr = SecureGroup.GetListActiveUsersInGroup(groupId))
				{
					while (rdr.Read())
					{
						DataRow[] dr = dt.Select("UserId = " + (int)rdr["UserId"]);
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
					DataRow[] dr = dt.Select("UserId = " + (int)row["ObjectId"]);
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
			dgMembers.Columns[2].HeaderText = LocRM.GetString("Code");
			dgMembers.Columns[3].HeaderText = LocRM.GetString("Rate");

			DataTable dt = (DataTable)ViewState["Team"];
			dgMembers.DataSource = dt.DefaultView;
			dgMembers.DataBind();

			foreach (DataGridItem dgi in dgMembers.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
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

		#region btnAdd_Click
		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			SynchronizeDT();
			DataTable dt = (DataTable)ViewState["Team"];
			foreach (ListItem li in lbUsers.Items)
				if (li.Selected)
				{
					DataRow dr = dt.NewRow();
					string username = li.Text;
					string ll = username.Substring(0, 1).ToUpper();
					string fl = "";
					try
					{
						fl = username.Substring(username.IndexOf(" ") + 1, 1).ToUpper();
					}
					catch
					{
						fl = username.Substring(1, 1).ToUpper();
					}
					dr["UserId"] = int.Parse(li.Value);
					dr["Code"] = fl + ll;
					dr["Rate"] = 0;
					dt.Rows.Add(dr);
				}
			ViewState["Team"] = dt;

			BindUsers();
			BinddgMemebers();
		}
		#endregion

		#region SynchronizeDT
		private void SynchronizeDT()
		{

			DataTable dt = (DataTable)ViewState["Team"];
			foreach (DataGridItem dgi in dgMembers.Items)
			{
				int UserID = int.Parse(dgi.Cells[0].Text);
				HtmlInputText hitc = (HtmlInputText)dgi.FindControl("tCode");
				HtmlInputText hitr = (HtmlInputText)dgi.FindControl("tRate");

				DataRow[] dr = dt.Select("UserId = " + UserID);
				if (dr.Length > 0)
				{
					try
					{
						dr[0]["Rate"] = decimal.Parse(hitr.Value);
					}
					catch
					{
					}
					dr[0]["Code"] = hitc.Value;
				}
			}
			ViewState["Team"] = dt;
		}
		#endregion

		#region dgMembers_Delete
		private void dgMembers_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			SynchronizeDT();
			int UserID = int.Parse(e.Item.Cells[0].Text);
			DataTable dt = (DataTable)ViewState["Team"];
			DataRow[] dr = dt.Select("UserId = " + UserID);
			if (dr.Length > 0)
				dt.Rows.Remove(dr[0]);
			ViewState["Team"] = dt;

			BindUsers();
			BinddgMemebers();
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			//Page.RegisterStartupScript("ButtonsDesable","DisableButtons(this);");
			SynchronizeDT();
			DataTable dt = (DataTable)ViewState["Team"];
			Project2.UpdateTeamMembers(ProjectID, dt);
			if (BtnID != String.Empty)
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
							"try {window.top.frames['right'].location.href='ProjectView.aspx?ProjectId=" + ProjectID + "';}" +
							"catch (e){}", true);
			else
			{
				if (Request["closeFramePopup"] != null)
				{
					CommandParameters cp = new CommandParameters("MC_PM_TeamEdit");
					Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
				}
				else
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
							"try {window.opener.top.frames['right'].location.href='../Projects/ProjectView.aspx?ProjectId=" + ProjectID + "';}" +
							"catch (e){} window.close();", true);
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
			else
				lblError.Visible = true;
		}
		#endregion

		#region BindSearchedUsers
		private void BindSearchedUsers(string searchstr)
		{
			DataTable dt = (DataTable)ViewState["Team"];
			lbUsers.Items.Clear();

			DataView dv = Mediachase.IBN.Business.User.GetListUsersBySubstringDataTable(searchstr).DefaultView;
			dv.Sort = "LastName, FirstName";
			
			for (int i = 0; i < dv.Count; i++)
			{
				DataRow[] dr = dt.Select("UserId = " + (int)dv[i]["UserId"]);
				if (dr.Length == 0)
					lbUsers.Items.Add(new ListItem((string)dv[i]["LastName"] + " " + (string)dv[i]["FirstName"], dv[i]["UserId"].ToString()));
			}
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



	}
}
