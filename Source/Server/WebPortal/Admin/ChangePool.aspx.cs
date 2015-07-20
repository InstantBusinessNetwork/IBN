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
using Mediachase.IBN.Business.EMail;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Reflection;

namespace Mediachase.UI.Web.Admin
{
	/// <summary>
	/// Summary description for ChangePool.
	/// </summary>
	public partial class ChangePool : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

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

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			if (Request.QueryString["btn"] != null)
				RefreshButton = Request.QueryString["btn"];

			BindToolbar();
			lbUsers.Attributes.Add("ondblclick", "DisableButtons(this);" + Page.ClientScript.GetPostBackEventReference(btnAdd, ""));
			lblError.Visible = false;

			if (!IsPostBack)
			{
				ViewState["SearchMode"] = false;

				btnAdd.InnerText = LocRM.GetString("tAdd");
				btnSearch.Text = LocRM.GetString("tFindNow");
				lblError.Text = LocRM.GetString("tEmptySearch");
				GetIssBoxData();
				BindGroups();
				BinddgMemebers();
			}
		}

		#region GetIssBoxData
		private void GetIssBoxData()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("UserId", typeof(int)));
			dt.Columns.Add(new DataColumn("PureName", typeof(string)));

			ArrayList userList = new ArrayList();
			string sUs = sUsers;
			while (sUs.Length > 0)
			{
				int sid = int.Parse(sUs.Substring(0, sUs.IndexOf("_")));
				sUs = sUs.Substring(sUs.IndexOf("_") + 1);
				if (!userList.Contains(sid))
					userList.Add(sid);
			}

			DataRow dr;
			foreach (int _id in userList)
			{
				dr = dt.NewRow();
				dr["UserId"] = _id;
				dr["PureName"] = Util.CommonHelper.GetUserStatusPureName(_id);
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
							lbUsers.Items.Add(new ListItem((string)reader["LastName"] + ", " + (string)reader["FirstName"], reader["UserId"].ToString()));
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

			DataTable dt = (DataTable)ViewState["Resources"];
			DataView dv = dt.DefaultView;
			dv.Sort = "PureName";
			dgMembers.DataSource = dv;
			dgMembers.DataBind();

			foreach (DataGridItem dgi in dgMembers.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				ib.Attributes.Add("onclick", "if(confirm('" + LocRM.GetString("tWarningPool") + "')) {DisableButtons(this); return true;} else return false;");
				ib.ToolTip = LocRM.GetString("tDelete");
			}
		}
		#endregion

		#region GetLink
		protected string GetLink(int PID)
		{
			return CommonHelper.GetUserStatusUL(PID);
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (Request.QueryString["Command"] == null)
				secHeader.Title = LocRM.GetString("tPoolTitle");
			secHeader.AddLink(String.Format("<img alt='' src='{0}'/>&nbsp;{1}",
					this.Page.ResolveUrl("~/Layouts/Images/saveitem.gif"),
					LocRM.GetString("tSave")),
				"javascript:FuncSave();");

			if (Request.QueryString["Command"] == null)
				secHeader.AddLink(String.Format("<img alt='' src='{0}'/>&nbsp;{1}",
						this.Page.ResolveUrl("~/Layouts/Images/cancel.gif"),
						LocRM.GetString("tCancel")),
					"javascript:window.close();");
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
					dr["PureName"] = Util.CommonHelper.GetUserStatusPureName(int.Parse(li.Value));
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
				sResponsible += dr["UserId"].ToString() + "_";

			if (Request["Command"] != null)
			{
				CommandParameters cp = new CommandParameters(Request["Command"]);
				cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
				cp.AddCommandArgument("Key", sResponsible);

				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
						String.Format("try{{window.opener.{0};}} catch (e){{}} window.close();", RefreshButton.Replace("xxx", sResponsible)),
					true);
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
