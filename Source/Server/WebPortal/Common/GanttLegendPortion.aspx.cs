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

namespace Mediachase.UI.Web.Common
{
	public partial class GanttLegendPortion : System.Web.UI.Page
	{
		private string _ganttItem
		{
			get
			{
				return Request["ganttItem"];
			}
		}

		private bool _completed
		{
			get
			{
				return (Request["Completed"] != null && Request["Completed"] == "1");
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Clear();
			Response.ContentType = "image/png";
			byte[] outbyte = GanttManager.RenderLegendItem(Server.MapPath(@"~/styles/IbnFramework/gantt.xml"), (GanttItem)Enum.Parse(typeof(GanttItem), _ganttItem), _completed);
			Response.OutputStream.Write(outbyte, 0, outbyte.Length);
			Response.End();
		}
	}
}
