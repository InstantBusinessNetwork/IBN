namespace Mediachase.UI.Web.Reports.Modules
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
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for DirectoryStatistics.
	/// </summary>
	public partial class DirectoryStatistics : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strDirectoryStatistics", typeof(DirectoryStatistics).Assembly);


		protected void Page_Load(object sender, System.EventArgs e)
		{
			Printheader1.Title = LocRM.GetString("Title");

			if (!IsPostBack)
			{
				GenerateReport();
			}


			if (Request["Export"] != null && Request["Export"] == "1")
			{
				ExportStatistics();
			}
		}

		public void GenerateReport()
		{
			lblStartedBy.Text = LocRM.GetString("StartedBy") + ": " + Security.CurrentUser.DisplayName;
			lblStartedAt.Text = LocRM.GetString("StartedAt") + ": " + UserDateTime.UserNow.ToString("g");
			DataBind();
			using (IDataReader reader = User.GetUserStatistic())
			{
				/// TotalUserCount, ActiveUserCount, InactiveUserCount, ExternalCount,
				/// PendingCount, SecureGroupCount, AvgCountUserInGroup
				if (reader.Read())
				{
					lblTotalUserCount.Text = reader["TotalUserCount"].ToString();
					lblActiveUserCount.Text = reader["ActiveUserCount"].ToString();
					lblInactiveUserCount.Text = reader["InactiveUserCount"].ToString();
					lblExternalUserCount.Text = reader["ExternalCount"].ToString();
					lblPendingUserCount.Text = reader["PendingCount"].ToString();
					lblTotalSequreGroupCount.Text = reader["SecureGroupCount"].ToString();
					lblAVCountUserPerGroup.Text = reader["AvgCountUserInGroup"].ToString();
				}
			}

			if (PortalConfig.UseIM)
			{
				trAvIM.Visible = true;
				trTotalIM.Visible = true;
				using (DataTable table = User.GetIMUserStatistics())
				{
					foreach (DataRow row in table.Rows)
					{
						/// IMGroupCount, AvgCountUserInIMGroup
						lblTotalImGroupCount.Text = row["IMGroupCount"].ToString();
						lblAVCountUserPerImGroup.Text = row["AvgCountUserInIMGroup"].ToString();
					}
				}
			}
			else
			{
				trAvIM.Visible = false;
				trTotalIM.Visible = false;
			}


		}

		private void ExportStatistics()
		{
			CommonHelper.ExportExcel(exportPanel, "UsersStatistics.xls", null);
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

		}
		#endregion
		#region Implementation of IPageTemplateTitle
		public string Modify(string oldValue)
		{

			return LocRM.GetString("Title");
		}
		#endregion

		protected void btnExport_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Reports/DirectoryStatistics.aspx?Export=1");
		}



		//===========================================================================
		// This public property was added by conversion wizard to allow the access of a protected, autogenerated member variable Printheader1.
		//===========================================================================
		public Mediachase.UI.Web.Modules.ReportHeader Printheader1
		{
			get { return Migrated_Printheader1; }
			//set { Migrated_Printheader1 = value; }
		}
	}
}