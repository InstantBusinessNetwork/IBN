using System;
using System.Web;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for DefautRedirectHandler.
	/// </summary>
	public class DefautRedirectHandler: IHttpHandler 
	{
		public DefautRedirectHandler()
		{
		}
	
		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			context.Response.Redirect("./default.aspx", true);
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		#endregion
	}
}
