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
using System.Threading;
using System.Resources;
using System.Globalization;

namespace Mediachase.Ibn.WebTrial
{
    public partial class PmBoxActivatePortal : System.Web.UI.Page
    {
        #region reqId
        private int reqId
        {
            get
            {
                try
                {
                    return int.Parse(Request["reqId"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        #endregion

        #region guid
        private string guid
        {
            get
            {
                return Request["guid"];
            }
        }
        #endregion

        #region locale
        private string locale
        {
            get
            {
                return Request["locale"];
            }
        }
        #endregion

        protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebTrial.App_GlobalResources.Resources.Activate", typeof(ActivatePortal).Assembly);

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "text/plain";

            if (locale != null && locale != String.Empty)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(locale);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);
            }

            string retVal = string.Empty;

            if (reqId == 0 || guid == null || guid == String.Empty)
            {
                retVal = "0";
            }
            else
            {
                string portalUrl;
                string login;
                string password;

                try
                {
                    portalUrl = TrialHelper.Activate(reqId, guid, out login, out password);

                    string activatedString = String.Format(LocRM.GetString("PmBox_Activated"), portalUrl, login, password);
                    retVal = activatedString + LocRM.GetString("PmBox_Assist");
                }
                catch
                {
#if !DEBUG
                    retVal = "0";
#else
                    throw;
#endif
                }
            }
            Response.Write(retVal);
        }
    }
}
