namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using System.IO;
	using Mediachase.Ibn;
	using System.Reflection;

	/// <summary>
	///		Summary description for AdminReports.
	/// </summary>
	public partial class AdminReports : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			if (!Page.IsPostBack)
			{
				SetVisibility();
				BindSnapshotInfo();
				BindStatusLog();
			}

			lblIMCount.Text = String.Format(LocRM.GetString("IMLoginCount"), IbnConst.ProductFamilyShort);
			
			Sep1.DataBind();
			Sep2.DataBind();
			Sep3.DataBind();
			Sep4.DataBind();
		}

		#region SetVisibility
		private void SetVisibility()
		{
			int i = 0;
			if (!PortalConfig.UseIM)
			{
				while (this.FindControl("trIMBlock" + i) != null)
				{
					this.FindControl("trIMBlock" + i).Visible = false;
					i++;
				}
			}
			else
			{
				while (this.FindControl("trIMBlock" + i) != null)
				{
					this.FindControl("trIMBlock" + i).Visible = true;
					i++;
				}
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			Header3.Title = LocRM.GetString("tIBNSnapshotInfo");
			Header3.AddLink(String.Format("<img width='16' height='16' title='{0}' border='0' align='top' src='{1}'/>&nbsp;{0}",
				LocRM.GetString("tAddTools"),
				this.Page.ResolveUrl("~/Layouts/Images/cancel.gif")),
				ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin8"));
		}
		#endregion

		#region BindSnapshotInfo
		private void BindSnapshotInfo()
		{
			int internalActiveUsersCount = 0;
			int externalActiveUsersCount = 0;

			/// TotalUserCount, ActiveUserCount, InactiveUserCount, 
			/// ExternalCount, ExternalActiveCount, ExternalInactiveCount,
			/// PendingCount, SecureGroupCount, AvgCountUserInGroup, PartnerGroupCount,
			/// PartnerUserCount, RegularUserCount, PortalLoginsCount,
			/// ActiveUserTotalCount, InactiveUserTotalCount
			using (IDataReader reader = User.GetUserStatistic())
			{
				if (reader.Read())
				{
					lblTotalUsers.Text = reader["TotalUserCount"].ToString();
					lblSecureGroups.Text = reader["SecureGroupCount"].ToString();

					InternalActiveUsersLabel.Text = reader["ActiveUserCount"].ToString();
					InternalDisabledUsersLabel.Text = reader["InactiveUserCount"].ToString();

					ExternalActiveUsersLabel.Text = reader["ExternalActiveCount"].ToString();
					ExternalDisabledUsersLabel.Text = reader["ExternalInactiveCount"].ToString();

					TotalActiveUsersLabel.Text = reader["ActiveUserTotalCount"].ToString();
					TotalDisabledUsersLabel.Text = reader["InactiveUserTotalCount"].ToString();

					lblPartnerUsers.Text = reader["PartnerUserCount"].ToString();
					lblRegularUsers.Text = reader["RegularUserCount"].ToString();
					lblExternalUsers.Text = reader["ExternalCount"].ToString();

					lblPartnerGroups.Text = reader["PartnerGroupCount"].ToString();
					lblPortalCount.Text = reader["PortalLoginsCount"].ToString();

					internalActiveUsersCount = (int)reader["ActiveUserCount"];
					externalActiveUsersCount = (int)reader["ExternalActiveCount"];
				}
			}

			using (IDataReader reader = Report.GetUsageStats())
			{
				if (reader.Read())
				{
					lblMessagesSent.Text = reader["MessSent"].ToString();
					lblMessagesReceived.Text = reader["MessReceived"].ToString();
					lblIMLoginCount.Text = reader["IMLogins"].ToString();
					lblContactGroups.Text = reader["IMGroups"].ToString();
					lblTotalGroups.Text = (int.Parse(lblContactGroups.Text) + int.Parse(lblSecureGroups.Text) + int.Parse(lblPartnerGroups.Text)).ToString();
				}
			}

			// DatabaseSizeInfoLabel

			int dbSizeLicense = PortalConfig.DatabaseSize;
			decimal dbSizeCurrent = Company.GetDatabaseSize() / 1024m / 1024m;
			if (dbSizeLicense > 0)
			{
				if (dbSizeCurrent < dbSizeLicense)
				{
					DatabaseSizeInfoLabel.Text = string.Format(CultureInfo.CurrentUICulture,
						"<span class=\"ibn-label\">{0} / {1}</span> ({2}: {3})",
						dbSizeCurrent,
						dbSizeLicense,
						LocRM.GetString("Left"),
						dbSizeLicense - dbSizeCurrent);
				}
				else
				{
					DatabaseSizeInfoLabel.Text = string.Format(CultureInfo.CurrentUICulture,
						"<span class=\"ibn-error\">{0} / {1}</span>",
						dbSizeCurrent,
						dbSizeLicense);
					DatabaseSizeInfoLabel.CssClass = "ibn-label";
				}
			}
			else
			{
				DatabaseSizeInfoLabel.Text = string.Format(CultureInfo.CurrentUICulture,
						"{0} / {1}",
						dbSizeCurrent,
						LocRM.GetString("Unlimited"));
				DatabaseSizeInfoLabel.CssClass = "ibn-label";
			}

			BindUsersCount(InternalUsersInfoLabel, false, internalActiveUsersCount);
			BindUsersCount(ExternalUsersInfoLabel, true, externalActiveUsersCount);
		}
		#endregion

		#region private void BindUsersCount(Label label, bool isExternal, int activeUserCurrent)
		private void BindUsersCount(Label label, bool isExternal, int activeUserCurrent)
		{
			int allowedUsersCount = User.GetAllowedUsersCount(isExternal);

			if (allowedUsersCount < 0)
			{
				label.Text = string.Format(CultureInfo.CurrentUICulture,
						"{0} / {1}",
						activeUserCurrent,
						LocRM.GetString("Unlimited"));
				label.CssClass = "ibn-label";
			}
			else
			{
				if (activeUserCurrent < allowedUsersCount)
				{
					label.Text = string.Format(CultureInfo.CurrentUICulture,
						"<span class=\"ibn-label\">{0} / {1}</span> ({2}: {3})",
						activeUserCurrent,
						allowedUsersCount,
						LocRM.GetString("Left"),
						allowedUsersCount - activeUserCurrent);
				}
				else
				{
					label.Text = string.Format(CultureInfo.CurrentUICulture,
						"<span class=\"ibn-error\">{0} / {1}</span>",
						activeUserCurrent,
						allowedUsersCount);
				}
			}
		}
		#endregion

		#region BindStatusLog
		private void BindStatusLog()
		{
			bool logUserStatus = PortalConfig.PortalLogUserStatus;

			tdStatusLog.Visible = logUserStatus;
			if (logUserStatus)
			{
				btnGenerate.Text = LocRM.GetString("Generate");

				ddlGroup.Items.Add(new ListItem(LocRM.GetString("AnyGroup"), "0"));
				using (DataTable table = IMGroup.GetListIMGroup())
				{
					foreach (DataRow row in table.Rows)
					{
						ddlGroup.Items.Add(new ListItem(row["IMGroupName"].ToString(), ((int)row["IMGroupId"]).ToString(CultureInfo.InvariantCulture)));
					}
				}

				ddlUser.Items.Add(new ListItem(LocRM.GetString("AnyUser"), "0"));

				fromDate.SelectedDate = DateTime.Now.Date.AddDays(-DateTime.Now.Day + 1);
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
		}
		#endregion

		#region btnGenerate_Click
		protected void btnGenerate_Click(object sender, EventArgs e)
		{
			Response.Clear();
			Response.ContentType = "text/plain";
			Response.AddHeader("content-disposition", String.Format("attachment; filename=\"StatusLog{0}.csv\"", DateTime.Now.ToString("yyyy-MM-dd")));
			Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
			Response.Write("Дата,Время,\"ID пользователя\",Фамилия,Имя,\"ID состояния\",\"Состояние\"\r\n");
			// user_id, dt, status, first_name, last_name
			using (IDataReader reader = Report.GetStatusLog(int.Parse(ddlGroup.SelectedValue), int.Parse(ddlUser.SelectedValue), fromDate.SelectedDate, toDate.SelectedDate))
			{
				while (reader.Read())
				{
					string s = String.Format(
						"{0},{1},{2},\"{3}\",\"{4}\",{5},\"{6}\"\r\n",
						((DateTime)reader["dt"]).ToString("yyyy-MM-dd"),
						((DateTime)reader["dt"]).ToString("HH:mm:ss"),
						reader["user_id"].ToString(),
						reader["last_name"].ToString(),
						reader["first_name"].ToString(),
						reader["status"].ToString(),
						LocRM.GetString("status" + reader["status"].ToString())
						);
					Response.Write(s);
				}
			}
			Response.End();
		}
		#endregion

		#region ddlGroup_SelectedIndexChanged
		protected void ddlGroup_SelectedIndexChanged(object sender, EventArgs e)
		{
			int groupId = int.Parse(ddlGroup.SelectedValue);

			ddlUser.Items.Clear();
			ddlUser.Items.Add(new ListItem(LocRM.GetString("AnyUser"), "0"));
			using (IDataReader reader = IMGroup.GetListUsers(groupId))
			{
				while (reader.Read())
				{
					ddlUser.Items.Add(new ListItem(reader["LastName"].ToString() + " " + reader["FirstName"].ToString(), reader["OriginalId"].ToString()));
				}
			}
		}
		#endregion
	}
}
