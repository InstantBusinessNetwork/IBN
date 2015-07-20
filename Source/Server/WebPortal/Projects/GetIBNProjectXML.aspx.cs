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
using System.Text;
using Mediachase.IBN.Business;


namespace Mediachase.UI.Web.Projects
{
	
	public partial class GetIBNProjectXML : System.Web.UI.Page
	{
		/// <summary>
		/// Gets the project id.
		/// </summary>
		/// <value>The project id.</value>
		public int ProjectId
		{
			get 
			{
				if (Request["ProjectId"] != null)
				{
					return int.Parse(Request["ProjectId"].ToString());
				}
				return -1;
			}
		}

		public bool IsSynchronized
		{
			get 
			{
				if (Request["Synchronized"] != null)
					return true;
				return false;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			string projectXml = string.Empty;
			if (IsSynchronized)
			{
				projectXml = Task.TaskExport2(ProjectId).InnerXml;
			}
			else
			{
				projectXml = IBNPrj2XML.IBN2XML(ProjectId).InnerXml;
			}
			string ProjectTitle = Project.GetProjectTitle(ProjectId);
			ProjectTitle = ProjectTitle.Replace(" ", "_").Replace("&", "_").Replace("%", "_");
			if (Request != null)
			{
				if (Request.Browser.Browser.ToUpper().IndexOf("IE") >= 0)
				{
					if (ProjectTitle.Length > 28) ProjectTitle = ProjectTitle.Substring(0, 28);
					ProjectTitle = HttpContext.Current.Server.UrlPathEncode(ProjectTitle);
				}
			}
			Response.Clear();
			byte[] arr = Encoding.UTF8.GetBytes(projectXml);
			Response.AddHeader("Content-Disposition", "attachment; filename=" + ProjectTitle + ".xml");
			Response.AddHeader("Content-Length", arr.Length.ToString());
			Response.ContentType = "text/xml";
			Response.Write(projectXml);
			Response.End();
		}
	}
}
