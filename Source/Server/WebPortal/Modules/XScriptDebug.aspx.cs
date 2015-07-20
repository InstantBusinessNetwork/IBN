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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Mediachase.UI.Web.Modules
{
	public partial class XScriptDebug : System.Web.UI.Page
	{
		private string fName = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "JsDebugScript.log";
		private static object lockObject = new object();

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetNoStore();
			Response.Clear();
			Response.ContentType = "text/plain";

			if (Request["msg"] != null)
			{
				lock (lockObject)
				{
					using (StreamWriter sw = File.AppendText(fName))
					{
						sw.WriteLine(String.Format("{0} :   {1} ( threadId: {2}, appId: {3} )", DateTime.Now.ToLongTimeString(), HttpUtility.HtmlDecode(Request["msg"]), Thread.CurrentThread.ManagedThreadId, AppDomain.CurrentDomain.Id));
						sw.Flush();
						sw.Close();
					}
				}
			}

		}
	}
}
