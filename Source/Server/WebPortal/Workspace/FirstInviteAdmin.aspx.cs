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

namespace Mediachase.UI.Web.Workspace
{
  public partial class FirstInviteAdmin : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      ApplyLocalization();
    }
    private void ApplyLocalization()
    {
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strFirstLogAdmWd", typeof(FirstInviteAdmin).Assembly);
      pT.Title = LocRM.GetString("InviteGlobalTitle");
    }

  }
}