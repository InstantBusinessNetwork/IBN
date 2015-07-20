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

namespace Mediachase.UI.Web.External
{
  public partial class FileUpload : System.Web.UI.Page
  {
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strAddDoc", typeof(FileUpload).Assembly);

    protected void Page_Load(object sender, System.EventArgs e)
    {
      pT.Title = LocRM.GetString("tAddDoc");
    }
  }
}
