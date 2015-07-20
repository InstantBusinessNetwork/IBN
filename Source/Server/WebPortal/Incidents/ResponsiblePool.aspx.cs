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
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Incidents
{
	/// <summary>
	/// Summary description for ResponsiblePool.
	/// </summary>
	public partial class ResponsiblePool : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(ResponsiblePool).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strTeamEditor", typeof(ResponsiblePool).Assembly);
		#region sUsers
		protected string sUsers
		{
			get
			{
				if (Request["sUsers"] != null)
					return Request["sUsers"];
				else
					return "";
			}
		}
		#endregion

		#region RefreshButton
		private string _refreshButton = String.Empty;
		public string RefreshButton
		{
			get { return _refreshButton; }
			set { _refreshButton = value; }
		}
		#endregion

		#region CanChange
		private bool CanChange
		{
			get
			{
				if (Request["notchange"] != null && Request["notchange"] == "1")
					return false;
				else
					return true;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			if (Request.QueryString["btn"] != null)
			{
				RefreshButton = Request.QueryString["btn"];

			}

			lbUsers.Attributes.Add("ondblclick", "DisableButtons(this);" + Page.ClientScript.GetPostBackEventReference(btnAdd, ""));
			lblError.Visible = false;

			if (!IsPostBack)
			{
				ViewState["SearchMode"] = false;

				btnAdd.InnerText = LocRM.GetString("tAdd2");
				btnSearch.Text = LocRM.GetString("tFindNow");
				lblError.Text = LocRM.GetString("tEmptySearch");
				SaveButton.Text = LocRM2.GetString("Save");
				CancelButton.Text = LocRM2.GetString("Cancel");
				GetPoolData();
				BindGroups();
				BinddgMemebers();
				if (!CanChange)
				{
					ddGroups.Enabled = false;
					tbSearch.Enabled = false;
					btnSearch.Enabled = false;
					lbUsers.Enabled = false;
					btnAdd.Disabled = true;
				}
			}

			if (!CanChange)
				SaveButton.Visible = false;

			SaveButton.Attributes.Add("onclick", "FuncSave();");
			SaveButton.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			if (Request["closeFramePopup"] != null)
				CancelButton.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}", Request["closeFramePopup"]));
			else
				CancelButton.Attributes.Add("onclick", "window.close();");
		}

		#region GetPoolData
		private void GetPoolData()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("UserId", typeof(int)));
			dt.Columns.Add(new DataColumn("Status", typeof(bool)));
			dt.Columns.Add(new DataColumn("IsNew", typeof(bool)));
			string workSUsers = sUsers;
			if (Request["IncidentId"] != null)
			{
				workSUsers = "";
				DataTable dt1 = Incident.GetResponsibleGroupDataTable(int.Parse(Request["IncidentId"]));
				if (dt1 != null)
					foreach (DataRow dr1 in dt1.Rows)
					{
						if ((bool)dr1["ResponsePending"])
							workSUsers += dr1["PrincipalId"].ToString() + "*1_";
						else
							workSUsers += dr1["PrincipalId"].ToString() + "*0_";
					}
			}
			Hashtable userList = new Hashtable();
			string sUs = workSUsers;
			while (sUs.Length > 0)
			{
				string sid = sUs.Substring(0, sUs.IndexOf("_"));

				string sUser = sid.Substring(0, sid.IndexOf("*"));
				string sTemp = sid.Substring(sid.IndexOf("*") + 1);
				string sStatus = (sTemp.IndexOf("*") >= 0) ? sTemp.Substring(0, sTemp.IndexOf("*")) : sTemp;

				sUs = sUs.Substring(sUs.IndexOf("_") + 1);
				if (!userList.Contains(int.Parse(sUser)))
					userList.Add(int.Parse(sUser), (sStatus == "1"));
			}

			DataRow dr;
			foreach (int _id in userList.Keys)
			{
				dr = dt.NewRow();
				dr["UserId"] = _id;
				dr["Status"] = (bool)userList[_id];
				dr["IsNew"] = false;
				dt.Rows.Add(dr);
			}
			ViewState["Resources"] = dt;
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			ddGroups.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "FavoritesGroup").ToString(), "0"));

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
			dgMembers.Columns[1].HeaderText = LocRM.GetString("tName");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("tStatus");

			DataTable dt = (DataTable)ViewState["Resources"];
			dgMembers.DataSource = dt.DefaultView;
			dgMembers.DataBind();

			if (!CanChange)
				dgMembers.Columns[3].Visible = false;
			else
				foreach (DataGridItem dgi in dgMembers.Items)
				{
					ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
					ib.Attributes.Add("onclick", "if(confirm('" + LocRM.GetString("tWarningPool") + "')) {DisableButtons(this); return true;} else return false;");
					ib.ToolTip = LocRM.GetString("Delete");
				}
		}
		#endregion

		#region GetLink
		protected string GetLink(int PID)
		{
			return CommonHelper.GetUserStatusUL(PID);
		}
		#endregion

		#region GetStatus
		protected string GetStatus(object _status)
		{
			bool status = false;
			if (_status != DBNull.Value)
				status = (bool)_status;

			if (status)
				return LocRM.GetString("Waiting");
			else
				return LocRM.GetString("Declined");
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
					dr["Status"] = true;
					dr["IsNew"] = true;
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
				BindSearchedUsers((string)ViewState["SearchString"]);
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
			DataTable dt = (DataTable)ViewState["Resources"];
			string sResponsible = "";
			foreach (DataRow dr in dt.Rows)
				sResponsible += dr["UserId"].ToString() + ((bool)dr["Status"] ? "*1" : "*0") + ((bool)dr["IsNew"] ? "*1_" : "*0_");

			if (Request["Command"] != null)
			{
				CommandParameters cp = new CommandParameters(Request["Command"]);
				cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
				cp.AddCommandArgument("Key", sResponsible);

				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "<script language=javascript>" +
					  String.Format("try{{window.opener.{0};}} catch (e){{}} window.close();", RefreshButton.Replace("xxx", sResponsible)) +
					  "</script>");
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
	}
}
