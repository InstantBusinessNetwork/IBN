namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Resources;
	using System.Globalization;

	/// <summary>
	///		Summary description for Updates.
	/// </summary>
	public partial class Updates : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strActive", typeof(Updates).Assembly);
		private UserLightPropertyCollection pc =  Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindSepAndPanels();
			if (!IsPostBack)
			{
				BindVisibility();
			}
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			
		}
		#endregion

		#region BindSepAndPanels
		private void BindSepAndPanels()
		{
			Sep1.ControlledPanel = Pan1;
			Sep2.ControlledPanel = Pan2;
			Sep3.ControlledPanel = Pan3;

			Sep1.PCValue = "Updates_sep1";
			Sep2.PCValue = "Updates_sep2";
			Sep3.PCValue = "Updates_sep3";
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			if (Sep1.Visible)
				BindDGToday();

			if (Sep2.Visible)
				BindDGYesterday();

			if (Sep3.Visible)
				BindDGSomeDaysAgo();
		}
		#endregion

		#region BindDGToday
		private void BindDGToday()
		{
			DateTime StartDate = User.GetLocalDate(DateTime.UtcNow).Date;
			DateTime EndDate = StartDate.AddDays(2);	// С запасом

			DataTable dt = SystemEvents.GetSystemEventsDT(StartDate, EndDate);
			
			int RowCount = dt.Rows.Count;
			if (RowCount == 0)
			{
				Sep1.Visible = false;
				Pan1.Visible = false;
			}
			else
			{
				Sep1.Title = String.Format("{0} ({1})", LocRM.GetString("Today"), RowCount);
				if(!Sep1.IsMinimized)
				{
					dgToday.DataSource = dt.DefaultView;
					dgToday.DataBind();
				}
			}
		}
		#endregion

		#region BindDGYesterday
		private void BindDGYesterday()
		{
			DateTime StartDate = User.GetLocalDate(DateTime.UtcNow).Date.AddDays(-1);
			DateTime EndDate = StartDate.AddDays(1);

			DataTable dt = SystemEvents.GetSystemEventsDT(StartDate, EndDate);
			
			int RowCount = dt.Rows.Count;
			if (RowCount == 0)
			{
				Sep2.Visible = false;
				Pan2.Visible = false;
			}
			else
			{
				Sep2.Title = String.Format("{0} ({1})", LocRM.GetString("Yesterday"), RowCount);
				if(!Sep2.IsMinimized)
				{
					dgYesterday.DataSource = dt.DefaultView;
					dgYesterday.DataBind();
				}
			}
		}
		#endregion

		#region BindDGSomeDaysAgo
		private void BindDGSomeDaysAgo()
		{
			DateTime StartDate = User.GetLocalDate(DateTime.UtcNow).Date.AddDays(-6);
			DateTime EndDate = StartDate.AddDays(5);

			DataTable dt = SystemEvents.GetSystemEventsDT(StartDate, EndDate);
			
			int RowCount = dt.Rows.Count;
			if (RowCount == 0)
			{
				Sep3.Visible = false;
				Pan3.Visible = false;
			}
			else
			{
				Sep3.Title = String.Format("{0} ({1})", LocRM.GetString("SomeDaysAgo"), RowCount);
				if(!Sep3.IsMinimized)
				{
					dgSomeDaysAgo.DataSource = dt.DefaultView;
					dgSomeDaysAgo.DataBind();
				}
			}
		}
		#endregion

    #region Page_PreRender
    private void Page_PreRender(object sender, EventArgs e)
		{
			BindDG();
		}
		#endregion
	}
}
