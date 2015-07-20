namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for Search.
	/// </summary>
	public partial class Search : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strSearch", typeof(Search).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();

			if (!IsPostBack)
			{
				BindValues();
				//	BinddgGroupsUsers();
			}
		}

		private void BindValues()
		{
			btnSearch.Text = LocRM.GetString("FindNow");

			ddType.Items.Clear();
			ddType.Items.Add(new ListItem(LocRM.GetString("All"), "0"));
			ddType.Items.Add(new ListItem(LocRM.GetString("Users"), "1"));
			ddType.Items.Add(new ListItem(LocRM.GetString("Groups"), "2"));
		}

		#region BinddgGroupsUsers()
		private void BinddgGroupsUsers()
		{
			dgGroupsUsers.Columns[2].HeaderText = LocRM.GetString("GroupUser");
			dgGroupsUsers.Columns[3].HeaderText = LocRM.GetString("Email");

			DataTable dt = new DataTable();
			dt.Columns.Add("ObjectId", typeof(int));
			dt.Columns.Add("Type");
			dt.Columns.Add("Title");
			dt.Columns.Add("SortTitle");
			dt.Columns.Add("Email");
			dt.Columns.Add("ActionView");
			dt.Columns.Add("ActionEdit");
			dt.Columns.Add("HasChildren", typeof(int));

			bool CanUpdate = SecureGroup.CanUpdate();
			bool UserCanDelete = User.CanDelete();

			if (ddType.SelectedItem.Value == "0" || ddType.SelectedItem.Value == "2")
			{

				using (IDataReader reader = SecureGroup.GetListGroupsBySubstring((string)ViewState["SearchStr"]))
				{
					while (reader.Read())
					{
						DataRow dr = dt.NewRow();
						dr["ObjectId"] = reader["GroupId"];
						dr["Type"] = "Group";
						dr["Title"] = CommonHelper.GetGroupLink((int)reader["GroupId"], CommonHelper.GetResFileString((string)reader["GroupName"]));
						dr["SortTitle"] = (string)reader["GroupName"];
						dr["HasChildren"] = reader["HasChildren"];

						if (CanUpdate && (int)reader["GroupId"] >= 8)
							dr["ActionEdit"] = "<a href='../Directory/AddGroup.aspx?Type=0&amp;Edit=1&amp;GroupID=" + reader["GroupId"].ToString() + "'>" + "<img alt='' src='../layouts/images/edit.GIF'/></a>";
						dt.Rows.Add(dr);
					}
				}
			}

			CanUpdate = User.CanUpdateUserInfo();

			if (ddType.SelectedItem.Value == "0" || ddType.SelectedItem.Value == "1")
			{
				using (IDataReader reader = User.GetListUsersBySubstring((string)ViewState["SearchStr"]))
				{
					while (reader.Read())
					{
						DataRow dr = dt.NewRow();
						dr["ObjectId"] = reader["UserId"];
						dr["Type"] = "User";
						dr["Title"] = CommonHelper.GetUserStatus((int)reader["UserId"]);
						dr["SortTitle"] = (string)reader["LastName"] + " " + (string)reader["FirstName"];
						dr["Email"] = "<a href='mailto:" + (string)reader["Email"] + "'>" + (string)reader["Email"] + "</a>";

						if (CanUpdate)
						{
							if ((bool)reader["IsExternal"])
								dr["ActionEdit"] = "<a href='../Directory/ExternalEdit.aspx?UserID=" + reader["UserId"].ToString() + "' title='" + LocRM.GetString("tEdit") + "'>"
									+ "<img alt='' src='../layouts/images/edit.GIF'/></a>";
							else if ((bool)reader["IsPending"])
								dr["ActionEdit"] = "<a href='../Directory/UserEdit.aspx?UserID=" + reader["UserId"].ToString() + "' title='" + LocRM.GetString("tApprove") + "'>"
									+ "<img alt='' src='../layouts/images/icon-key.GIF'/></a>";
							else
								dr["ActionEdit"] = "<a href='../Directory/UserEdit.aspx?UserID=" + reader["UserId"].ToString() + "' title='" + LocRM.GetString("tEdit") + "'>"
									+ "<img alt='' src='../layouts/images/edit.GIF'/></a>";
						}
						dr["ActionView"] = "<a href='../Directory/UserView.aspx?UserID=" + reader["UserId"].ToString() + "' title='" + LocRM.GetString("tViewDetails") + "'>"
							+ "<img alt='' src='../layouts/images/Icon-Search.GIF'/></a>";

						dr["HasChildren"] = 0;

						dt.Rows.Add(dr);
					}
				}
			}

			if (!SecureGroup.CanDelete())
				dgGroupsUsers.Columns[6].Visible = false;

			if (pc["ds_PageSize"] != null)
				dgGroupsUsers.PageSize = int.Parse(pc["ds_PageSize"]);


			int pageindex = dgGroupsUsers.CurrentPageIndex;
			int ppi = dt.Rows.Count / dgGroupsUsers.PageSize;
			if (dt.Rows.Count % dgGroupsUsers.PageSize == 0)
				ppi = ppi - 1;

			if (pageindex > ppi)
				dgGroupsUsers.CurrentPageIndex = 0;


			DataView dv = dt.DefaultView;
			dv.Sort = "SortTitle";

			dgGroupsUsers.DataSource = dv;
			dgGroupsUsers.DataBind();

			foreach (DataGridItem dgi in dgGroupsUsers.Items)
			{
				int ID = int.Parse(dgi.Cells[0].Text);
				bool CanDelete = User.CanDelete(ID);
				string type = dgi.Cells[1].Text;
				int HasChildren = int.Parse(dgi.Cells[7].Text);

				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
				{
					if (type == "Group" && HasChildren == 0)
						ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
					else
						if (type != "Group" && CanDelete)
							ib.Attributes.Add("onclick", String.Format("return DeleteUser({0})", ID));
						else
							ib.Visible = false;
				}
			}
		}
		#endregion

		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tbTitle");
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgGroupsUsers.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_Delete);
			this.dgGroupsUsers.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageIndexChanged);
			this.dgGroupsUsers.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChanged);

		}
		#endregion

		protected void btnSearch_Click(object sender, System.EventArgs e)
		{
			ViewState["SearchStr"] = tbSerchStr.Text;
			BinddgGroupsUsers();
		}

		private void dg_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["ds_PageSize"] = e.NewPageSize.ToString();
			BinddgGroupsUsers();
		}

		private void dg_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dgGroupsUsers.CurrentPageIndex = e.NewPageIndex;
			BinddgGroupsUsers();
		}

		protected void btnRefresh_Click(object sender, System.EventArgs e)
		{
			BinddgGroupsUsers();
		}

		private void dg_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int GID = int.Parse(e.Item.Cells[0].Text);
			string Type = e.Item.Cells[1].Text;
			if (Type == "Group")
				SecureGroup.Delete(GID);
			BinddgGroupsUsers();
		}
	}
}
