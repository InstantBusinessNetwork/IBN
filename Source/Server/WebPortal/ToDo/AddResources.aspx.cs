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

namespace Mediachase.UI.Web.ToDo
{
	/// <summary>
	/// Summary description for strResourcesEditor.
	/// </summary>
	public partial class ResourcesEditor : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strAddResources", typeof(ResourcesEditor).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ResourcesEditor).Assembly);

		#region ToDoID
		private int ToDoID
		{
			get
			{
				try
				{
					return int.Parse(Request["ToDoID"]);
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
				return Request["BtnID"];
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
			lblError.Visible = false;
			//btnSave.Attributes.Add("onclick","DisableButtons(this);");

			if (!IsPostBack)
			{
				ViewState["SearchMode"] = false;

				btnAdd.InnerText = LocRM.GetString("Add");
				btnAddGroup.InnerText = LocRM.GetString("AddGroup");
				btnSearch.Text = LocRM.GetString("FindNow");
				cbConfirmed.Text = LocRM.GetString("MustBeConfirmed");
				lblError.Text = LocRM.GetString("EmrtySearch");
				GetToDoData();
				BindGroups();
				BinddgMemebers();
			}
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			BindToolbar();
		}
		#endregion

		#region GetToDoData
		private void GetToDoData()
		{
			DataTable dt = Mediachase.IBN.Business.ToDo.GetListResourcesDataTable(ToDoID);
			ViewState["Resources"] = dt;

			using (IDataReader rdr = Mediachase.IBN.Business.ToDo.GetToDo(ToDoID))
			{
				if (rdr.Read())
				{
					int compltype = (int)rdr["CompletionTypeId"];
					if (compltype == (int)Mediachase.IBN.Business.CompletionType.All) 
						btnAddGroup.Visible = false;
				}
			}
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			ddGroups.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "FavoritesGroup").ToString(), "0"));

			int ProjectId = -1;
			using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetToDo(ToDoID, false))
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

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindGroupUsers(int.Parse(li.Value));
		}
		#endregion

		#region BindGroupUsers
		private void BindGroupUsers(int groupId)
		{
			lbUsers.Items.Clear();
			DataTable dt = (DataTable)ViewState["Resources"];

			if (groupId > 0)
			{
				using (IDataReader reader = SecureGroup.GetListActiveUsersInGroup(groupId))
				{
					while (reader.Read())
					{
						DataRow[] dr = dt.Select("UserId = " + (int)reader["UserId"]);
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
					DataRow[] dr = dt.Select("UserId = " + (int)row["ObjectId"]);
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
						DataRow[] dr = dt.Select("UserId = " + (int)reader["UserId"]);
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

			DataTable dt = (DataTable)ViewState["Resources"];
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

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = "&nbsp;";// LocRM.GetString("tTitle");

			string text, link;

			if (dgMembers.Items.Count > 0)
			{
				ArrayList userList = new ArrayList();
				DataTable dt = (DataTable)ViewState["Resources"];
				foreach (DataRow row in dt.Rows)
				{
					int principalId = (int)row["UserId"];
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
					ToDoID,
					(int)ObjectTypes.ToDo);
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
			DataTable dt = (DataTable)ViewState["Resources"];
			foreach (ListItem li in lbUsers.Items)
				if (li.Selected)
				{
					DataRow dr = dt.NewRow();
					dr["UserId"] = int.Parse(li.Value);
					dr["MustBeConfirmed"] = cbConfirmed.Checked;
					dr["ResponsePending"] = true;

					dt.Rows.Add(dr);
				}
			ViewState["Resources"] = dt;

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

			int UserId = int.Parse(e.Item.Cells[0].Text);
			DataTable dt = (DataTable)ViewState["Resources"];
			DataRow[] dr = dt.Select("UserId = " + UserId);
			if (dr.Length > 0)
				dt.Rows.Remove(dr[0]);
			ViewState["Resources"] = dt;

			BindUsers();
			BinddgMemebers();
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			//Page.RegisterStartupScript("onclick","DisableButtons(this);");
			DataTable dt = (DataTable)ViewState["Resources"];
			Mediachase.IBN.Business.ToDo2.UpdateResources(ToDoID, dt);
			if (Request["FromCreate"] != null)
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}", Request["closeFramePopup"]), true);
			}
			else if (Request["closeFramePopup"] != null)
			{
				CommandParameters cp = new CommandParameters("MC_PM_ToDoResEdit");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
				Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoView.aspx?ToDoId=" + ToDoID, Response);
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
			DataTable dt = (DataTable)ViewState["Resources"];
			if (ddGroups.SelectedItem != null && int.Parse(ddGroups.SelectedValue) > 0 && dt.Select("UserId=" + ddGroups.SelectedValue).Length == 0)
			{
				DataRow dr = dt.NewRow();
				dr["UserId"] = int.Parse(ddGroups.SelectedItem.Value);
				dr["MustBeConfirmed"] = false;
				dt.Rows.Add(dr);
			}
			ViewState["Resources"] = dt;

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
			DataTable dt = (DataTable)ViewState["Resources"];
			lbUsers.Items.Clear();
			using (IDataReader rdr = Mediachase.IBN.Business.User.GetListUsersBySubstring(searchstr))
			{
				while (rdr.Read())
				{
					DataRow[] dr = dt.Select("UserId = " + (int)rdr["UserId"]);
					if (dr.Length == 0)
						lbUsers.Items.Add(new ListItem((string)rdr["LastName"] + " " + (string)rdr["FirstName"], rdr["UserId"].ToString()));
				}
			}
		}
		#endregion
	}
}