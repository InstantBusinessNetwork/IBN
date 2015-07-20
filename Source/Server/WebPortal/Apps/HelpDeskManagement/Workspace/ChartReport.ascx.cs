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

namespace Mediachase.UI.Web.Apps.HelpDeskManagement.Workspace
{
	public partial class ChartReport : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strReports", typeof(ChartReport).Assembly);
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

				return "Dash_Iss_Stat=1";
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

		#region BindGraph
		/// <summary>
		/// Binds the graph.
		/// </summary>
		private void BindGraph()
		{
			if (this.ChartKeyValue.Contains("Dash_Iss_Stat"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Incident:Status}");
			else if (this.ChartKeyValue.Contains("Dash_Iss_Prior"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Incident:Priority}");
			if (this.ChartKeyValue.Contains("Dash_Iss_Prj"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Incident:Project}");
			else if (this.ChartKeyValue.Contains("Dash_Iss_Man"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Incident:tIncBox}");
			if (this.ChartKeyValue.Contains("Dash_Iss_Sev"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Incident:Severity}");
			else if (this.ChartKeyValue.Contains("Dash_Iss_GenCat"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Incident:GenCat}");
			else if (this.ChartKeyValue.Contains("Dash_Iss_IssCat"))
				this.groupBy = CHelper.GetResFileString("{IbnFramework.Incident:IssCat}");

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
			//ddPerspect.SelectedValue = pc["Dashboard_Perspect"].ToString();
		}
		#endregion
	}
}