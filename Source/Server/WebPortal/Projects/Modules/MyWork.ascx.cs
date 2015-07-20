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
	///		Summary description for MyWork.
	/// </summary>
	public partial class MyWork : System.Web.UI.UserControl
	{

		private string pcKey = "ProjectMyWork_CurrentTab";

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Calendar.Resources.strCalendar", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidents", typeof(MyWork).Assembly);

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
			string link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=My&SubTab=ListView", ProjectId);
			blockControl.AddTab("ListView", LocRM.GetString("ListView"), link, "../Projects/Modules/WorksForResource.ascx");

			if(Configuration.HelpDeskEnabled)
			{
				link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=My&SubTab=MyIncidents", ProjectId);
				blockControl.AddTab("MyIncidents", LocRM2.GetString("tMyIncidents"), link, "../Incidents/Modules/IncidentsList.ascx");
			}

			// Select Tab
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;
			if ((SubTab == "ListView" || SubTab == "MyIncidents"))
				pc[pcKey] = SubTab;
			else if (pc[pcKey] == null)
				pc[pcKey] = "ListView";
			if(!Configuration.HelpDeskEnabled && pc[pcKey] == "MyIncidents")
				pc[pcKey] = "ListView";

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
