namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for ProjectCalendar.
	/// </summary>
	public partial class ProjectCalendar : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Calendar.Resources.strCalendar", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		private string pcKey = "ProjectCalendar_CurrentTab";

		#region ProjectId
		private int ProjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region SubTab
		private string subTab;
		protected string SubTab
		{
			get
			{
				if (subTab == null)
				{
					subTab = "";
					if (Request["SubTab"] != null)
						subTab = Request["SubTab"];
				}
				return subTab;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Add Tabs
			string link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=Calendar&SubTab=DailyCalendar", ProjectId);
			blockControl.AddTab("DailyCalendar", LocRM.GetString("DailyCalendar"), link, "../Projects/Modules/ProjectCalendarWrapper2.ascx");

			link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=Calendar&SubTab=WeeklyCalendar", ProjectId);
			blockControl.AddTab("WeeklyCalendar", LocRM.GetString("WeeklyCalendar"), link, "../Projects/Modules/ProjectCalendarWrapper2.ascx");

			link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=Calendar&SubTab=MonthlyCalendar", ProjectId);
			blockControl.AddTab("MonthlyCalendar", LocRM.GetString("MonthlyCalendar"), link, "../Projects/Modules/ProjectCalendarWrapper2.ascx");

			link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=Calendar&SubTab=YearlyCalendar", ProjectId);
			blockControl.AddTab("YearlyCalendar", LocRM.GetString("YearlyCalendar"), link, "../Projects/Modules/ProjectCalendarWrapper2.ascx");

			// Select Tab
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;
			if ((SubTab == "DailyCalendar" || SubTab == "MonthlyCalendar" || SubTab == "WeeklyCalendar" || SubTab == "YearlyCalendar"))
				pc[pcKey] = SubTab;
			else if (pc[pcKey] == null)
				pc[pcKey] = "DailyCalendar";

			blockControl.SelectTab(pc[pcKey]);
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
	}
}
