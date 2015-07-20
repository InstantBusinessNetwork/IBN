using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Reflection;
using System.Resources;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for GanttChart.
	/// </summary>
	public partial class GanttChart : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strGanttChart", Assembly.GetExecutingAssembly());
		protected void Page_Load(object sender, System.EventArgs e)
		{
			pT.Title = LocRM.GetString("GanttChartTitle");
		}
	}
}
