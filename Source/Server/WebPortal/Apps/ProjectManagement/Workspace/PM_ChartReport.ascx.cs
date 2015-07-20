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
using Mediachase.IBN.Business;
using System.Resources;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.Apps.ProjectManagement.Workspace
{
	public partial class PM_ChartReport : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strReports", typeof(PM_ChartReport).Assembly);
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		protected string groupBy = string.Empty;

		#region prop: ChartKeyValue
		//TODO: Load from control storage
		/// <summary>
		/// Gets or sets the chart key value.
		/// </summary>
		/// <value>The chart key value.</value>
		public string ChartKeyValue
		{
			get
			{
				if (ControlProperties.Provider.GetValue(this.ID, "Chart") != null)
					return string.Format("{0}=1", ControlProperties.Provider.GetValue(this.ID, "Chart").ToString());

				return "Dash_Prj_Stat=1";
			}
		}
		#endregion

		#region prop: ChartKeyGistType
		/// <summary>
		/// Gets the type of the chart key gist.
		/// </summary>
		/// <value>The type of the chart key gist.</value>
		public string ChartKeyGistType
		{
			get
			{
				if (ControlProperties.Provider.GetValue(this.ID, "ChartKeyGistType") != null)
					return string.Format("{0}&", ControlProperties.Provider.GetValue(this.ID, "ChartKeyGistType").ToString());

				return "Pie&";
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			BindLists();
			BindGraph();
		}

		#region BindChartView
		/// <summary>
		/// Binds the chart view.
		/// </summary>
		private void BindChartView()
		{
			//ddChartView.Items.Clear();
			//switch (ddPerspect.SelectedValue)
			//{
			//    case "0":	//Project
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("Status"), "Dash_Prj_Stat"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("Priority"), "Dash_Prj_Prior"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("Type"), "Dash_Prj_Typ"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("Manager"), "Dash_Prj_Man"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("GenCat"), "Dash_Prj_GenCat"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("PrjCat"), "Dash_Prj_PrjCat"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("Portfolio"), "Dash_Prj_PrjGrp"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("Phase"), "Dash_Prj_Phas"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("Client"), "Dash_Prj_Clnt"));
			//        break;
			//    case "1":	//Issue
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("Status"), "Dash_Iss_Stat"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("Priority"), "Dash_Iss_Prior"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("Type"), "Dash_Iss_Typ"));
			//        if (Configuration.ProjectManagementEnabled)
			//            ddChartView.Items.Add(new ListItem(LocRM.GetString("Project"), "Dash_Iss_Prj"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("tIncBox"), "Dash_Iss_Man"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("Severity"), "Dash_Iss_Sev"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("GenCat"), "Dash_Iss_GenCat"));
			//        ddChartView.Items.Add(new ListItem(LocRM.GetString("IssCat"), "Dash_Iss_IssCat"));
			//        break;
			//    default:
			//        break;
			//}
			//if (pc["Dashboard_GraphView"] != null && ddChartView.Items.FindByValue(pc["Dashboard_GraphView"].ToString()) != null)
			//    ddChartView.SelectedValue = pc["Dashboard_GraphView"].ToString();
		}
		#endregion

		#region BindGraph
		/// <summary>
		/// Binds the graph.
		/// </summary>
		private void BindGraph()
		{
			if (this.ChartKeyValue.Contains("Dash_Prj_Stat"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Project:Status}");
			else if (this.ChartKeyValue.Contains("Dash_Prj_Prior"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Project:Priority}");
			if (this.ChartKeyValue.Contains("Dash_Prj_Typ"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Project:Type}");
			else if (this.ChartKeyValue.Contains("Dash_Prj_Man"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Project:Manager}");
			if (this.ChartKeyValue.Contains("Dash_Prj_GenCat"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Project:GenCat}");
			else if (this.ChartKeyValue.Contains("Dash_Prj_PrjCat"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Project:PrjCat}");
			else if (this.ChartKeyValue.Contains("Dash_Prj_PrjGrp"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Project:Portfolio}");
			else if (this.ChartKeyValue.Contains("Dash_Prj_Phas"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Project:Phase}");
			else if (this.ChartKeyValue.Contains("Dash_Prj_Clnt"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Project:Client}");

			string sQ_Str = "?Type=";
			//if (rbChartType.SelectedValue == "0")
			//    sQ_Str += "Pie&";
			//else
			//    sQ_Str += "Gist&";
			sQ_Str += this.ChartKeyGistType;
			DateTime dt = DateTime.Now;
			sQ_Str += this.ChartKeyValue + "&id=" + dt.Hour + dt.Minute + dt.Second;
			imgGraph.ImageUrl = this.ResolveUrl("~/Modules/ChartImage.aspx") + sQ_Str;
		}
		#endregion

		#region BindLists
		/// <summary>
		/// Binds the lists.
		/// </summary>
		private void BindLists()
		{
			pc["Dashboard_Perspect"] = "1";
			//ddPerspect.SelectedValue = pc["Dashboard_Perspect"].ToString()

			BindChartView();
		}
		#endregion
	}
}