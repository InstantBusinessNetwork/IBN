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

using Mediachase.Ibn;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Directory
{
	/// <summary>
	/// Summary description for SharingEditor.
	/// </summary>
	public partial class SharingEditor : System.Web.UI.Page
	{
		protected ResourceManager LocRM;

		private int UserID
		{
			get
			{
				try
				{
					return int.Parse(Request["UserID"]);
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}

		private string BtnID
		{
			get
			{
				return Request["BtnID"];
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			if (UserID != Security.CurrentUser.UserID)
			{
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "winclose", "window.close()");
				Response.End();
			}



			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strSharingEditor", typeof(SharingEditor).Assembly);
			//btnSave.Attributes.Add("onclick","DisableButtons(this);");
			BindToolbar();
			lbUsers.Attributes.Add("ondblclick", Page.ClientScript.GetPostBackEventReference(btnAdd, ""));
			cbCanManage.Text = LocRM.GetString("CanManage");
			if (!IsPostBack)
			{
				ViewState["SearchMode"] = false;

				btnAdd.Text = LocRM.GetString("Add");
				//btnAddGroup.Text = LocRM.GetString("AddGroup");
				btnSearch.Text = LocRM.GetString("FindNow");
				GetEventData();
				BindGroups();
				BinddgMemebers();
			}
		}

		private void GetEventData()
		{
			DataTable dt = Mediachase.IBN.Business.User.GetListPeopleForSharingDataTable(UserID);
			ViewState["Participants"] = dt;
		}

		private void BindGroups()
		{
			ddGroups.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "FavoritesGroup").ToString(), "0"));

			using (IDataReader reader = SecureGroup.GetListGroupsAsTree())
			{
				while (reader.Read())
				{
					int groupid = (int)reader["GroupId"];
					if (groupid != (int)InternalSecureGroups.Roles &&
						groupid != (int)InternalSecureGroups.Partner &&
						!SecureGroup.IsPartner(groupid))
					{
						string GroupName = CommonHelper.GetResFileString(reader["GroupName"].ToString());
						string GroupId = groupid.ToString();
						int Level = (int)reader["Level"];
						for (int i = 1; i < Level; i++)
							GroupName = "  " + GroupName;
						ListItem item = new ListItem(GroupName, GroupId);

						ddGroups.Items.Add(item);
					}
				}
			}

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindGroupUsers(int.Parse(li.Value));
		}

		private void BindGroupUsers(int groupId)
		{
			lbUsers.Items.Clear();
			DataTable dt = (DataTable)ViewState["Participants"];

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
			else  // Favorites
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
				ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
				//ib.AlternateText=LocRM.GetString("Delete");
				ib.ToolTip = LocRM.GetString("Delete");
			}
		}

		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tTitle");

			secHeader.AddLink("<img alt='' src='../Layouts/Images/saveitem.gif'/> " + LocRM.GetString("Save"), "javascript:FuncSave();");

			secHeader.AddSeparator();
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("Cancel"), "javascript:window.close();");
		}

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

		protected void ddGroups_ChangeGroup(object sender, System.EventArgs e)
		{
			tbSearch.Text = "";
			ViewState["SearchString"] = null;

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindGroupUsers(int.Parse(li.Value));
		}

		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			DataTable dt = (DataTable)ViewState["Participants"];
			foreach (ListItem li in lbUsers.Items)
				if (li.Selected)
				{
					DataRow dr = dt.NewRow();
					dr["UserId"] = int.Parse(li.Value);
					if (cbCanManage.Checked)
						dr["Level"] = 1;
					else
						dr["Level"] = 0;
					dt.Rows.Add(dr);
				}

			ViewState["Participants"] = dt;

			BindUsers();
			BinddgMemebers();
		}

		protected string GetLevel(int level)
		{
			if (level == 1)
				return LocRM.GetString("Manage");
			else
				return LocRM.GetString("Read");
		}

		private void BindUsers()
		{
			if (ViewState["SearchString"] == null)
			{
				ListItem _li = ddGroups.SelectedItem;
				if (_li != null)
					BindGroupUsers(int.Parse(_li.Value));
			}
			else
				BindSearchedUsers((string)ViewState["SearchString"]);
		}

		private void dgMembers_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{

			int UserId = int.Parse(e.Item.Cells[0].Text);
			DataTable dt = (DataTable)ViewState["Participants"];
			DataRow[] dr = dt.Select("UserId = " + UserId);
			if (dr.Length > 0)
				dt.Rows.Remove(dr[0]);
			ViewState["Participants"] = dt;

			BindUsers();
			BinddgMemebers();
		}

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			//Page.RegisterStartupScript("ButtonsDisable","DisableButtons(this);");
			DataTable dt = (DataTable)ViewState["Participants"];
			Mediachase.IBN.Business.User.UpdateSharing(UserID, dt);
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "<script language=javascript>" +
					  "try {window.opener.document.forms[0].submit();}" +
					  "catch (e){} window.close();</script>");

		}

		protected string GetLink(int PID, bool IsGroup)
		{
			if (IsGroup)
				return CommonHelper.GetGroupLinkUL(PID);
			else
				return CommonHelper.GetUserStatusUL(PID);
		}

		private void btnAddGroup_Click(object sender, System.EventArgs e)
		{
			DataTable dt = (DataTable)ViewState["Participants"];
			if (ddGroups.SelectedItem != null && dt.Select("UserId=" + ddGroups.SelectedItem.Value).Length == 0)
			{
				DataRow dr = dt.NewRow();
				dr["UserId"] = int.Parse(ddGroups.SelectedItem.Value);
				dt.Rows.Add(dr);
			}
			ViewState["Participants"] = dt;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindGroupUsers(int.Parse(_li.Value));
			BinddgMemebers();
		}

		protected void btnSearch_Click(object sender, System.EventArgs e)
		{
			if (tbSearch.Text != "")
			{
				ViewState["SearchString"] = tbSearch.Text;
				BindSearchedUsers(tbSearch.Text);
			}
		}

		#region BindSearchedUsers
		private void BindSearchedUsers(string searchstr)
		{
			DataTable dt = (DataTable)ViewState["Participants"];
			lbUsers.Items.Clear();

			DataView dv = Mediachase.IBN.Business.User.GetListUsersBySubstringDataTable(searchstr).DefaultView;
			dv.Sort = "LastName, FirstName";

			for (int i = 0; i < dv.Count; i++)
			{
				DataRow[] dr = dt.Select("UserId = " + (int)dv[i]["UserId"]);
				if (dr.Length == 0)
				{
					if (!Mediachase.IBN.Business.User.IsExternal((int)dv[i]["UserId"]))
						lbUsers.Items.Add(new ListItem((string)dv[i]["LastName"] + " " + (string)dv[i]["FirstName"], dv[i]["UserId"].ToString()));
				}
			}
		}
		#endregion
	}
}
