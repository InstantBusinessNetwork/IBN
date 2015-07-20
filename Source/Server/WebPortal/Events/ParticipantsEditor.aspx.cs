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

namespace Mediachase.UI.Web.Events
{
	/// <summary>
	/// Summary description for strParticipantsEditor.
	/// </summary>
	public partial class ParticipantsEditor : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strParticipantsEditor", typeof(ParticipantsEditor).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ParticipantsEditor).Assembly);

		#region EventID
		private int EventID
		{
			get
			{
				try
				{
					return int.Parse(Request["EventID"]);
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/common.js");

			lbUsers.Attributes.Add("ondblclick", "DisableButtons(this);" + Page.ClientScript.GetPostBackEventReference(btnAdd, ""));
			//btnSave.Attributes.Add("onclick","DisableButtons(this);");
			btnAdd.Attributes.Add("onclick", "DisableButtons(this);");
			btnAddGroup.Attributes.Add("onclick", "DisableButtons(this);");
			if (cbConfirmed.Checked)
				btnAddGroup.Style.Add("display", "none");
			else
				btnAddGroup.Style.Add("display", "inline");
			lblError.Visible = false;
			if (!IsPostBack)
			{
				ViewState["SearchMode"] = false;
				cbConfirmed.Text = LocRM.GetString("MustBeConfirmed");
				btnAdd.InnerText = LocRM.GetString("Add");
				btnAddGroup.InnerText = LocRM.GetString("AddGroup");
				btnSearch.Text = LocRM.GetString("FindNow");
				lblError.Text = LocRM.GetString("EmrtySearch");
				GetEventData();
				BindGroups();
				BinddgMemebers();
			}
			cbConfirmed.Attributes.Add("onclick", "ChangeButtonState();");
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			BindToolbar();
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

		#region GetEventData
		private void GetEventData()
		{
			int UserID = 0;
			using (IDataReader rdr = CalendarEntry.GetEvent(EventID))
			{
				if (rdr.Read())
				{
					UserID = (int)rdr["ManagerId"];
				}
			}

			DataTable dt = CalendarEntry.GetListEventResourcesDataTable(EventID);
			bool isinres = false;

			foreach (DataRow dr in dt.Rows)
				if ((int)dr["PrincipalId"] == UserID) isinres = true;

			if (!isinres)
			{
				DataRow dr = dt.NewRow();

				dr["PrincipalId"] = UserID;
				dr["IsGroup"] = false;
				dr["MustBeConfirmed"] = false;
				dr["ResponsePending"] = false;

				dt.Rows.InsertAt(dr, 0);
			}

			ViewState["Participants"] = dt;
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			ddGroups.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "FavoritesGroup").ToString(), "0"));

			int ProjectId = -1;
			using (IDataReader reader = CalendarEntry.GetEvent(EventID, false))
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
				ddGroups.Items.Add(new ListItem(HttpUtility.HtmlEncode(CommonHelper.GetResFileString(row["GroupName"].ToString())), row["GroupId"].ToString()));
			}

			/*
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
			*/

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
				btnAddGroup.Visible = true;
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
			else if (groupId == 0)  // Favorites
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
				btnAddGroup.Visible = false;
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
				//ib.AlternateText=LocRM.GetString("Delete");
				ib.ToolTip = LocRM.GetString("Delete");
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = "&nbsp;";// LocRM.GetString("tTitle");

			string text, link;

			if (dgMembers.Items.Count > 0)
			{
				ArrayList userList = new ArrayList();
				DataTable dt = (DataTable)ViewState["Participants"];
				foreach (DataRow row in dt.Rows)
				{
					int principalId = (int)row["PrincipalId"];
					if (Mediachase.IBN.Business.User.IsGroup(principalId))
					{
						using (IDataReader reader = SecureGroup.GetListAllUsersInGroup(principalId, false))
						{
							while (reader.Read())
							{
								if (!userList.Contains((int)reader["UserId"]))
								{
									userList.Add((int)reader["UserId"]);
								}
							}
						}
					}
					else
					{
						if (!userList.Contains(principalId))
						{
							userList.Add(principalId);
						}
					}
				}

				string users = String.Empty;
				foreach (int userId in userList)
				{
					if (!String.IsNullOrEmpty(users))
						users += ",";

					users += userId.ToString();
				}

				text = String.Format(CultureInfo.InvariantCulture,
					"<img alt='' src='{0}'/> {1}",
					Page.ResolveUrl("~/Layouts/Images/ResUtil.png"),
					LocRM2.GetString("Utilization"));
				link = String.Format(CultureInfo.InvariantCulture,
					"javascript:OpenPopUpNoScrollWindow('{0}?users={1}&amp;ObjectId={2}&amp;ObjectTypeId={3}',750,300)",
					Page.ResolveUrl("~/Common/ResourceUtilGraphForObject.aspx"),
					users,
					EventID,
					(int)ObjectTypes.CalendarEntry);
				secHeader.AddLink(text, link);
			}

			text = String.Format(CultureInfo.InvariantCulture,
				"<img alt='' src='{0}'/> {1}",
				Page.ResolveUrl("~/Layouts/Images/saveitem.gif"),
				LocRM.GetString("Save"));
			link = "javascript:FuncSave();";
			secHeader.AddLink(text, link);

			secHeader.AddSeparator();

			text = String.Format(CultureInfo.InvariantCulture,
				"<img alt='' src='{0}'/> {1}",
				Page.ResolveUrl("~/Layouts/Images/cancel.gif"),
				LocRM.GetString("Cancel"));
			if (Request["closeFramePopup"] != null)
			{
				link = String.Format(CultureInfo.InvariantCulture,
					"javascript:try{{window.parent.{0}();}}catch(ex){{;}}",
					Request["closeFramePopup"]);
			}
			else
			{
				link = "javascript:window.close();";
			}
			secHeader.AddLink(text, link);
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
			DataTable dt = (DataTable)ViewState["Participants"];
			foreach (ListItem li in lbUsers.Items)
				if (li.Selected)
				{
					DataRow dr = dt.NewRow();
					dr["PrincipalId"] = int.Parse(li.Value);
					dr["IsGroup"] = false;
					dr["MustBeConfirmed"] = cbConfirmed.Checked;
					dr["ResponsePending"] = true;
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
			//Page.RegisterStartupScript("DisableButtons","DisableButtons(this);");
			DataTable dt = (DataTable)ViewState["Participants"];
			CalendarEntry2.UpdateResources(EventID, dt);

			if (Request["FromCreate"] != null)
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}", Request["closeFramePopup"]), true);
			else if (Request["closeFramePopup"] != null)
			{
				CommandParameters cp = new CommandParameters("MC_PM_EventParticipants");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
							"try {window.opener.top.frames['right'].location.href='../Events/EventView.aspx?EventID=" + EventID + "';}" +
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

		#region btnAddGroup_Click
		protected void btnAddGroup_Click(object sender, System.EventArgs e)
		{
			DataTable dt = (DataTable)ViewState["Participants"];
			if (ddGroups.SelectedItem != null && dt.Select("PrincipalId=" + ddGroups.SelectedItem.Value).Length == 0 && int.Parse(ddGroups.SelectedValue) > 0)
			{
				DataRow dr = dt.NewRow();
				dr["PrincipalId"] = int.Parse(ddGroups.SelectedItem.Value);
				dr["IsGroup"] = true;
				dr["MustBeConfirmed"] = false;
				dt.Rows.Add(dr);
			}
			ViewState["Participants"] = dt;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindGroupUsers(int.Parse(_li.Value));
			BinddgMemebers();
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
	}
}
