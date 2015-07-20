using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Web.UI;
using System.Resources;
using Mediachase.Ibn.Web.UI.Apps.WidgetEngine;


namespace Mediachase.UI.Web.Apps.HelpDeskManagement.Workspace
{
	public partial class PP_ChartReport : System.Web.UI.UserControl, IPropertyPageControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strReports", typeof(ChartReport).Assembly);

		protected void Page_Load(object sender, EventArgs e)
		{
			BindChartView();

			if (ControlProperties.Provider.GetValue(this.ID, "Chart") != null)
				ddChartView.SelectedValue = ControlProperties.Provider.GetValue(this.ID, "Chart").ToString();

			if (ControlProperties.Provider.GetValue(this.ID, "ChartKeyGistType") != null)
			{
				string chartType = ControlProperties.Provider.GetValue(this.ID, "ChartKeyGistType").ToString();
				if (chartType == "Pie")
					rbChartType.SelectedValue = "0";
				else
					rbChartType.SelectedValue = "1";
				//ddChartView.SelectedValue = 
			}
			else
			{
				rbChartType.SelectedValue = "0";
			}
		}

		#region BindChartView
		/// <summary>
		/// Binds the chart view.
		/// </summary>
		private void BindChartView()
		{
			ddChartView.Items.Clear();
			ddChartView.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Incident:Status}"), "Dash_Iss_Stat"));
			ddChartView.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Incident:Priority}"), "Dash_Iss_Prior"));
			ddChartView.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Incident:Type}"), "Dash_Iss_Typ"));
			if (Mediachase.IBN.Business.Configuration.ProjectManagementEnabled)
				ddChartView.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Incident:Project}"), "Dash_Iss_Prj"));
			ddChartView.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Incident:tIncBox}"), "Dash_Iss_Man"));
			ddChartView.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Incident:Severity}"), "Dash_Iss_Sev"));
			ddChartView.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Incident:GenCat}"), "Dash_Iss_GenCat"));
			ddChartView.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Incident:IssCat}"), "Dash_Iss_IssCat"));

			rbChartType.Items.Add(new ListItem(String.Format("&nbsp;{0}&nbsp;", LocRM.GetString("tPieChart")), "0"));
			rbChartType.Items.Add(new ListItem(String.Format("&nbsp;{0}", LocRM.GetString("tBarGraph")), "1"));
		}
		#endregion


		#region IPropertyPageControl Members

		public void Save()
		{
			ControlProperties.Provider.SaveValue(this.ID, "Chart", ddChartView.SelectedValue);

			if (rbChartType.SelectedValue == "0")
				ControlProperties.Provider.SaveValue(this.ID, "ChartKeyGistType", "Pie");
			else
				ControlProperties.Provider.SaveValue(this.ID, "ChartKeyGistType", "Gist");
			
		}

		#endregion
	}
}