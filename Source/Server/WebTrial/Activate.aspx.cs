
//===========================================================================
// This file was modified as part of an ASP.NET 2.0 Web project conversion.
// The class name was changed and the class modified to inherit from the abstract base class 
// in file 'App_Code\Migrated\Stub_Activate_aspx_cs.cs'.
// During runtime, this allows other classes in your web application to bind and access 
// the code-behind page using the abstract base class.
// The associated content page 'Activate.aspx' was also modified to refer to the new class name.
// For more information on this code pattern, please refer to http://go.microsoft.com/fwlink/?LinkId=46995 
//===========================================================================
using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Resources;
using System.Threading;
using System.Globalization;

namespace Mediachase.Ibn.WebTrial
{
	/// <summary>
	/// Summary description for Activate.
	/// </summary>
	public partial class Activate : System.Web.UI.Page
	{
		protected string sHeaderFileName = "trial_header";
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebTrial.App_GlobalResources.Resources.Activate", typeof(Activate).Assembly);

		private int RequestID
		{
			get
			{
				try
				{
					return int.Parse(Request["rid"]);
				}
				catch
				{
					return 0;
				}
			}
		}

		private string _Guid
		{
			get
			{
				return Request["Guid"];
			}
		}

		private string locale
		{
			get
			{
				return Request["locale"];
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (locale != null && locale != String.Empty)
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo(locale);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);
			}

			divTitle.InnerHtml = LocRM.GetString("YouAreReady");
			divMess.InnerHtml = "<i>" + LocRM.GetString("PressActivate") + "</i>";

			//divTitle.InnerHtml = LocRM.GetString("Congratulations");
			//string PortalUrl = "http://myportal.mediachase.net";
			//String activatedString = String.Format(LocRM.GetString("Activated"), PortalUrl);
			//divMess.InnerHtml = activatedString + LocRM.GetString("Assist");

			divFailed.InnerHtml = LocRM.GetString("PleaseCheck") + LocRM.GetString("Assist");
			string asppath = Settings.AspPath;
			if (asppath != null)
				imgHeader.ImageUrl = asppath + "images/SignupHeader.aspx";

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "ActivatePortal('ActivatePortal.aspx?reqId=" + RequestID + "&guid=" + _Guid + "&locale=" + locale + "', '" + divMess.ClientID + "', '" + divTitle.ClientID + "');", true);
		}

	}
}
