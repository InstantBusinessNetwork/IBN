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
using System.Resources;
using System.Threading;
using System.Globalization;

namespace Mediachase.Ibn.WebTrial
{
    public partial class PmBoxActivate : System.Web.UI.Page
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

            divTitle.InnerHtml = LocRM.GetString("PmBox_YouAreReady");
            divMess.InnerHtml = "<i>" + LocRM.GetString("PmBox_PressActivate") + "</i>";

            //divTitle.InnerHtml = LocRM.GetString("Congratulations");
            //string PortalUrl = "http://myportal.mediachase.net";
            //String activatedString = String.Format(LocRM.GetString("Activated"), PortalUrl);
            //divMess.InnerHtml = activatedString + LocRM.GetString("Assist");

            divFailed.InnerHtml = LocRM.GetString("PmBox_PleaseCheck") + LocRM.GetString("PmBox_Assist");
            //string asppath = Settings.AspPath;
            //if (asppath != null)
            //    imgHeader.ImageUrl = asppath + "images/SignupHeader.aspx";

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "ActivatePortal('PmBoxActivatePortal.aspx?reqId=" + RequestID + "&guid=" + _Guid + "&locale=" + locale + "', '" + divMess.ClientID + "', '" + divTitle.ClientID + "');", true);
        }
    }
}
