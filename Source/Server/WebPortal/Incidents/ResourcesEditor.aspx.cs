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

namespace Mediachase.UI.Web.Incidents
{
	/// <summary>
	/// Summary description for ResourcesEditor.
	/// </summary>
	public partial class ResourcesEditor : System.Web.UI.Page
	{
		protected ResourceManager LocRM;

		#region IncidentID
		private int IncidentID
		{
			get
			{
				try
				{
					return int.Parse(Request["IncidentID"]);
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

			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strParticipantsEditor", typeof(ResourcesEditor).Assembly);
			BindToolbar();
			lbUsers.Attributes.Add("ondblclick", "DisableButtons(this);" + Page.ClientScript.GetPostBackEventReference(btnAdd, ""));
			//btnSave.Attributes.Add("onclick","DisableButtons(this);");
			btnAdd.Attributes.Add("onclick", "DisableButtons(this);");
			lblError.Visible = false;
			cbCanManage.Visible = false;
			cbCanManage.Checked = false;
			if (!IsPostBack)
			{
				ViewState["SearchMode"] = false;
				cbConfirmed.Text = LocRM.GetString("MustBeConfirmed");
				btnAdd.InnerText = LocRM.GetString("Add");
				btnSearch.Text = LocRM.GetString("FindNow");
				cbCanManage.Text = LocRM.GetString("CanManage");
				lblError.Text = LocRM.GetString("EmrtySearch");
				GetIncidentData();
				BindGroups();
				BinddgMemebers();
			}
		}

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = "&nbsp;"; //LocRM.GetString("tIssueTitle");

			secHeader.AddLink("<img alt='' src='../Layouts/Images/saveitem.gif'/> " + LocRM.GetString("Save"), "javascript:FuncSave();");

			secHeader.AddSeparator();
			if (Request["closeFramePopup"] != null)
				secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("Cancel"), String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}", Request["closeFramePopup"]));
			else
				secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("Cancel"), "javascript:FuncCancel(" + IncidentID + ");");
		}
		#endregion

		#region GetIncidentData
		private void GetIncidentData()
		{
			DataTable dt = Incident.GetListIncidentResourcesDataTable(IncidentID);

			ViewState["Participants"] = dt;
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			ddGroups.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "FavoritesGroup").ToString(), "0"));

			int ProjectId = -1;
			using (IDataReader reader = Incident.GetIncident(IncidentID))
			{
				if (reader.Read())
					if (reader["ProjectId"] != DBNull.Value)
						ProjectId = (int)reader["ProjectId"];
			}

			if (ProjectId > 0)
			{
				int pID = -ProjectId;
				ddGroups.Items.Add(new ListItem(LocRM.GetString("ProjectTeam"), pID.ToString()));
			}
			DataTable dt = SecureGroup.GetListGroupsAsTreeDataTable();
			foreach (DataRow row in dt.Rows)
			{
				ddGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(row["GroupName"].ToString()), row["GroupId"].ToString()));
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
			DataTable dt = (DataTable)ViewState["Participants"];
			if (groupId > 0)
			{
				using (IDataReader reader = SecureGroup.GetListActiveUsersInGroup(groupId))
				{
					while (reader.Read())
					{
						DataRow[] dr = dt.Select("PrincipalId = " + (int)reader["UserId"]);
						if (dr.Length == 0)
							lbUsers.Items.Add(new ListItem((string)reader["LastName"] + " " + (string)reader["FirstName"], reader["UserId"].ToString()));
					}
				}
			}
			else if (groupId == 0) // Favorites
			{
				DataTable favorites = Mediachase.IBN.Business.Common.GetListFavoritesDT(ObjectTypes.User);
				foreach (DataRow row in favorites.Rows)
				{
					DataRow[] dr = dt.Select("PrincipalId = " + (int)row["ObjectId"]);
					if (dr.Length == 0)
						lbUsers.Items.Add(new ListItem((string)row["Title"], row["ObjectId"].ToString()));
				}
			}
			else
			{
				using (IDataReader reader = Project.GetListTeamMemberNamesWithManager(-groupId))
				{
					while (reader.Read())
					{
						DataRow[] dr = dt.Select("PrincipalId = " + (int)reader["UserId"]);
						if (dr.Length == 0)
							lbUsers.Items.Add(new ListItem((string)reader["LastName"] + " " + (string)reader["FirstName"], reader["UserId"].ToString()));
					}
				}
			}
		}
		#endregion

		#region BinddgMemebers
		private void BinddgMemebers()
		{
			dgMembers.Columns[1].HeaderText = LocRM.GetString("Name");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("Status");
			DataTable dt = (DataTable)ViewState["Participants"];
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

		#region GetStatus
		protected string GetStatus(object _mbc, object _rp, object _ic)
		{
			bool mbc = false;
			if (_mbc != DBNull.Value)
				mbc = (bool)_mbc;

			bool rp = false;
			if (_rp != DBNull.Value)
				rp = (bool)_rp;

			bool ic = false;
			if (_ic != DBNull.Value)
				ic = (bool)_ic;

			if (!mbc) return "";
			else
				if (rp) return LocRM.GetString("Waiting");
				else
					if (ic) return LocRM.GetString("Accepted");
					else return LocRM.GetString("Denied");
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
			DataTable dt = (DataTable)ViewState["Participants"];
			foreach (ListItem li in lbUsers.Items)
				if (li.Selected)
				{
					DataRow dr = dt.NewRow();
					dr["PrincipalId"] = int.Parse(li.Value);
					dr["IsGroup"] = false;
					dr["MustBeConfirmed"] = cbConfirmed.Checked;
					dr["ResponsePending"] = true;
					//					dr["CanManage"] = cbCanManage.Checked;
					dr["CanManage"] = false;
					dt.Rows.Add(dr);
				}

			ViewState["Participants"] = dt;

			BindUsers();
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
			SynchronizeDT();
			int PrincipalId = int.Parse(e.Item.Cells[0].Text);
			DataTable dt = (DataTable)ViewState["Participants"];
			DataRow[] dr = dt.Select("PrincipalId = " + PrincipalId);
			if (dr.Length > 0)
				dt.Rows.Remove(dr[0]);
			ViewState["Participants"] = dt;

			BindUsers();
			BinddgMemebers();
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			SynchronizeDT();
			//Page.RegisterStartupScript("DisableButtons","DisableButtons(this);");
			DataTable dt = (DataTable)ViewState["Participants"];
			Issue2.UpdateResources(IncidentID, dt);
			

			if (BtnID != String.Empty)
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
							"<script language=javascript>" +
							"try {window.top.frames['right'].location.href='IncidentView.aspx?IncidentId=" + IncidentID + "';}" +
							"catch (e){}</script>");
			else if (Request["closeFramePopup"] != null)
			{
				CommandParameters cp = new CommandParameters("MC_HDM_ResEdit");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
							"<script language=javascript>" +
							"try {window.opener.top.frames['right'].location.href='../Incidents/IncidentView.aspx?IncidentId=" + IncidentID + "';}" +
							"catch (e){} window.close();</script>");
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
			DataTable dt = (DataTable)ViewState["Participants"];
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

		#region SynchronizeDT
		private void SynchronizeDT()
		{
			//			DataTable dt = (DataTable)ViewState["Participants"];
			//			foreach (DataGridItem dgi in dgMembers.Items)
			//			{
			//				int PrincipalId = int.Parse(dgi.Cells[0].Text);
			//				HtmlInputCheckBox hitc = (HtmlInputCheckBox)dgi.FindControl("icCanManage");
			//				
			//				DataRow[] dr = dt.Select("PrincipalId = " + PrincipalId);
			//				if (dr.Length>0)
			//				{
			//					dr[0]["CanManage"] = hitc.Checked;
			//				}
			//			}
			//			ViewState["Participants"] = dt;
		}
		#endregion
	}
}
