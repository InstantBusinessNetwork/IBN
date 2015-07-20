using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;

namespace Mediachase.UI.Web.Public
{

  public partial class CheckSMTPSettings : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.strContactUs", typeof(CheckSMTPSettings).Assembly);
      pT.Title = LocRM.GetString("tCheckSMTP");
    }
  }
}