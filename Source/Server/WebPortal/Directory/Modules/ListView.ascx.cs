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
	///		Summary description for ListView.
	/// </summary>
	public partial class ListView : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strListView", typeof(ListView).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		protected string dgUsers_SortColumn = "sortLastName";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			if (!IsPostBack)
			{
				if (pc["dgUsers_SortColumn"] == null)
					pc["dgUsers_SortColumn"] = dgUsers_SortColumn;
			}
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BinddgUsers();
		}

		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tbTitle");
			if (Security.IsUserInGroup(InternalSecureGroups.Administrator))
				secHeader.AddLink("<img alt='' src='../Layouts/Images/import.gif' title='" + LocRM.GetString("tImport") + "'/>", "javascript:try{ImportUsersWizard()}catch(e){}");
		}

		#region BinddgUsers()
		private void BinddgUsers()
		{
			dgUsers.Columns[1].HeaderText = LocRM.GetString("LastName");
			dgUsers.Columns[2].HeaderText = LocRM.GetString("FirstName");
			dgUsers.Columns[3].HeaderText = LocRM.GetString("Email");
			dgUsers.Columns[4].HeaderText = LocRM.GetString("Location");
			dgUsers.Columns[5].HeaderText = LocRM.GetString("Phone");
			foreach (DataGridColumn dgc in dgUsers.Columns)
			{
				if (dgc.SortExpression == pc["dgUsers_SortColumn"].ToString())
					dgc.HeaderText += "&nbsp;<img alt='' src='../layouts/images/upbtnF.jpg'/>";
				else if (dgc.SortExpression + " DESC" == pc["dgUsers_SortColumn"].ToString())
					dgc.HeaderText += "&nbsp;<img alt=''  src='../layouts/images/downbtnF.jpg'/>";
			}

			DataTable dt = new DataTable();
			dt.Columns.Add("ObjectId", typeof(int));
			dt.Columns.Add("FirstName");
			dt.Columns.Add("LastName");
			dt.Columns.Add("Email");
			dt.Columns.Add("Location");
			dt.Columns.Add("Phone");
			dt.Columns.Add("ActionView");
			dt.Columns.Add("ActionEdit");

			dt.Columns.Add(new DataColumn("sortFirstName", typeof(string)));
			dt.Columns.Add(new DataColumn("sortLastName", typeof(string)));
			dt.Columns.Add(new DataColumn("sortEMail", typeof(string)));
			dt.Columns.Add(new DataColumn("sortLocation", typeof(string)));
			dt.Columns.Add(new DataColumn("sortPhone", typeof(string)));

			bool UserCanDelete = User.CanDelete();
			bool CanUpdate = User.CanUpdateUserInfo();


			using (IDataReader reader =
							 (Security.IsUserInGroup(InternalSecureGroups.Administrator)) ?
							 User.GetListAll() :
							 User.GetListUsersBySubstring(String.Empty))
			{
				while (reader.Read())
				{
					int iUserId = (int)reader["UserId"];
					///		UserId, Login, FirstName, LastName, Email, IsActive, IMGroupId, CreatedBy, Comments, IsExternal, IsPending, OriginalId
					using (IDataReader UserReader = User.GetUserInfo(iUserId))
					{
						if (UserReader.Read())
						{
							DataRow dr = dt.NewRow();
							dr["ObjectId"] = iUserId;
							string sUser = CommonHelper.GetUserStatus(iUserId);
							if (!(bool)UserReader["IsActive"])
							{
								dr["FirstName"] = String.Format("<span style='color:#999999'>{0}</span>", UserReader["FirstName"].ToString());
								dr["LastName"] = String.Format("<span style='color:#999999'>{0}</span>", UserReader["LastName"].ToString());
							}
							else
							{
								dr["FirstName"] = UserReader["FirstName"].ToString();
								dr["LastName"] = UserReader["LastName"].ToString();
							}
							dr["sortFirstName"] = UserReader["FirstName"].ToString();
							dr["sortLastName"] = UserReader["LastName"].ToString();
							dr["Email"] = "<a href='mailto:" + (string)UserReader["Email"] + "'>" + (string)UserReader["Email"] + "</a>";
							dr["sortEMail"] = (string)UserReader["Email"];
							using (IDataReader UserProfile = User.GetUserProfile(iUserId))
							{
								if (UserProfile.Read())
								{
									dr["Location"] = UserProfile["location"].ToString();
									dr["sortLocation"] = UserProfile["location"].ToString();
									dr["Phone"] = UserProfile["phone"].ToString();
									dr["sortPhone"] = UserProfile["phone"].ToString();
								}
							}
							if (CanUpdate)
							{
								if ((bool)UserReader["IsExternal"])
									dr["ActionEdit"] = "<a href='../Directory/ExternalEdit.aspx?UserID=" + iUserId.ToString() + "' title='" + LocRM.GetString("tEdit") + "'>"
										+ "<img alt='' src='../layouts/images/edit.GIF'/></a>";
								else if ((bool)UserReader["IsPending"])
									dr["ActionEdit"] = "<a href='../Directory/UserEdit.aspx?UserID=" + iUserId.ToString() + "' title='" + LocRM.GetString("tApprove") + "'>"
										+ "<img alt='' src='../layouts/images/icon-key.GIF'/></a>";
								else
									dr["ActionEdit"] = "<a href='../Directory/UserEdit.aspx?UserID=" + iUserId.ToString() + "' title='" + LocRM.GetString("tEdit") + "'>"
										+ "<img alt='' src='../layouts/images/edit.GIF'/></a>";
							}
							dr["ActionView"] = "<a href='../Directory/UserView.aspx?UserID=" + iUserId.ToString() + "' title='" + LocRM.GetString("tViewDetails") + "'>"
								+ "<img alt='' src='../layouts/images/Icon-Search.GIF'/></a>";

							dt.Rows.Add(dr);
						}
					}
				}
			}

			DataRow[] drassets = null;
			try
			{
				drassets = dt.Select("", pc["dgUsers_SortColumn"]);
			}
			catch
			{
				pc["dgUsers_SortColumn"] = "sortFirstName";
				drassets = dt.Select("", pc["dgUsers_SortColumn"]);
			}

			DataTable result = dt.Clone();

			foreach (DataRow dr1 in drassets)
			{
				DataRow _dr = result.NewRow();
				_dr.ItemArray = (Object[])dr1.ItemArray.Clone();
				result.Rows.Add(_dr);
			}

			if (!SecureGroup.CanDelete())
				dgUsers.Columns[8].Visible = false;

			if (pc["ds_PageSize"] != null)
				dgUsers.PageSize = int.Parse(pc["ds_PageSize"]);


			int pageindex = dgUsers.CurrentPageIndex;
			int ppi = dt.Rows.Count / dgUsers.PageSize;
			if (dt.Rows.Count % dgUsers.PageSize == 0)
				ppi = ppi - 1;

			if (pageindex > ppi)
				dgUsers.CurrentPageIndex = 0;


			dgUsers.DataSource = new DataView(result);
			dgUsers.DataBind();

			foreach (DataGridItem dgi in dgUsers.Items)
			{
				int ID = int.Parse(dgi.Cells[0].Text);
				bool CanDelete = User.CanDelete(ID);
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null && CanDelete)
				{
					ib.Attributes.Add("onclick", String.Format("return DeleteUser({0})", ID));
					ib.Visible = true;
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgUsers.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_Delete);
			this.dgUsers.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageIndexChanged);
			this.dgUsers.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChanged);
			this.dgUsers.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgUsers_Sort);
		}
		#endregion

		private void dgUsers_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if (pc["dgUsers_SortColumn"] == (string)e.SortExpression)
				dgUsers_SortColumn = (string)e.SortExpression + " DESC";
			else
				dgUsers_SortColumn = (string)e.SortExpression;

			pc["dgUsers_SortColumn"] = dgUsers_SortColumn;
		}

		private void dg_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["ds_PageSize"] = e.NewPageSize.ToString();
		}

		private void dg_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dgUsers.CurrentPageIndex = e.NewPageIndex;
		}

		protected void btnRefresh_Click(object sender, System.EventArgs e)
		{
			BinddgUsers();
		}

		private void dg_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			BinddgUsers();
		}
	}
}
