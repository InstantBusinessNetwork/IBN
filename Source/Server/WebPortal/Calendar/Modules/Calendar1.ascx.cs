namespace Mediachase.UI.Web.Calendar.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for Calendar1.
	/// </summary>
	public partial class Calendar1 : System.Web.UI.UserControl
	{
		private string Tab
		{
			get
			{
				return Request["Tab"];
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindTabs();
		}

		private void BindTabs()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Calendar.Resources.strCalendar", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

			UserLightPropertyCollection pc = Security.CurrentUser.Properties;

			if (Tab != null)
			{
				if (Tab == "SharedCalendars" || Tab == "MyCalendar")
					pc["Calendar1_CurrentTab"] = Tab;
			}
			else if (pc["Calendar1_CurrentTab"] == null)
				pc["Calendar1_CurrentTab"] = "MyCalendar";

			using (IDataReader rdr = Mediachase.IBN.Business.CalendarView.GetListPeopleForCalendar())
			{
				if (!rdr.Read() && pc["Calendar1_CurrentTab"] == "SharedCalendars")
					pc["Calendar1_CurrentTab"] = "MyCalendar";
			}

			string controlName = "CalendarViewMy.ascx";

			if (pc["Calendar1_CurrentTab"] == "MyCalendar")
				((Mediachase.UI.Web.Modules.PageTemplateNew)this.Parent.Parent.Parent.Parent).Title = LocRM.GetString("tMyCalendar");
			else if (pc["Calendar1_CurrentTab"] == "SharedCalendars")
				((Mediachase.UI.Web.Modules.PageTemplateNew)this.Parent.Parent.Parent.Parent).Title = LocRM.GetString("tSharedCalendars");

			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
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
	}
}
