using System;
using System.Web.UI;

using Mediachase.IBN.Business;


namespace Mediachase.UI.Web.Projects
{
	// Page parameters:
	//
	// Type: TimeType. The type of item to render.

	public partial class ResourceChartLegendImage : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Clear();
			Response.ContentType = "image/png";
			byte[] outbyte = ResourceChart.RenderLegendItem(Server.MapPath(@"~/styles/IbnFramework/ResourceChartStyle.xml"), Request["Type"]);
			Response.OutputStream.Write(outbyte, 0, outbyte.Length);
			Response.End();
		}
	}
}
