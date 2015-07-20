using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.IBN.Business.Rss;
using Mediachase.IBN.Business;
using Mediachase.Ibn;

namespace Mediachase.UI.Web.Modules
{
	public partial class RssDocument : System.Web.UI.Page
	{
		/// <summary>
		/// Creates from request.
		/// </summary>
		/// <returns></returns>
		public RssGeneratorParameters CreateFromRequest()
		{
			RssGeneratorParameters retVal = new RssGeneratorParameters();

			retVal.UserId = new Guid(HttpContext.Current.Request.QueryString["u"]);
			retVal.ClassName = HttpContext.Current.Request.QueryString["cn"];

			// TODO: Check retVal.UserId and logon user by usre id

			string strObjectId = HttpContext.Current.Request.QueryString["id"];
			if (!string.IsNullOrEmpty(strObjectId))
				retVal.ObjectId = int.Parse(strObjectId);

			retVal.CurrentView = HttpContext.Current.Request.QueryString["cv"];

			return retVal;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!PortalConfig.IsListRssEnabled)
				throw new AccessDeniedException("Rss Chanel Disabled");

			RssGeneratorParameters param = CreateFromRequest();

			RssGenerator.LogonUserByRssKey(param.UserId);

			string rssXml = (new RssGenerator(this, param)).Generate();

			this.Response.ContentType = "text/xml";
			this.Response.Write(rssXml);
			this.Response.End();
		}
	}
}
