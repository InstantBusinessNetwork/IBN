using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.IBN.Business;


namespace Mediachase.UI.Web.Projects
{
	public partial class GanttReportImage2 : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			byte[] data = null;

			bool dataIsXml = Request["xml"] != null;

			if (Project.IsWebGanttChartEnabled())
				data = GanttManager.RenderAnalysisChart(ParseInteger("OriginalPlanSlotId"), ParseInteger("BasePlanSlotId"), dataIsXml, Server.MapPath(@"~/styles/IbnFramework/gantt.xml"), ParseInteger("x"), ParseInteger("y"), ParseInteger("PageItems"), ParseInteger("PageNumber"));

			if (data != null && data.Length > 0)
			{
				if (this.Request["lx"] != null)
				{
					UserLightPropertyCollection pc = Security.CurrentUser.Properties;
					pc["GantChartMilestones_Lx_New"] = this.Request["lx"];
				}

				Response.Clear();
				Response.Cache.SetNoStore();
				if (dataIsXml)
					Response.ContentType = "text/xml";
				else
					Response.ContentType = "image/png";
				Response.OutputStream.Write(data, 0, data.Length);
				Response.End();
			}
			else
				Response.Redirect("~/Layouts/Images/Blank.gif", true);
		}

		private int ParseInteger(string name)
		{
			int ret = 0;

			string value = Request[name];
			if (!string.IsNullOrEmpty(value))
				ret = int.Parse(value, CultureInfo.InvariantCulture);

			return ret;
		}

	}
}
