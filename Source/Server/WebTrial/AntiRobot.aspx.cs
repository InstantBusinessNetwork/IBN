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

namespace Mediachase.Ibn.WebTrial
{
  public partial class AntiRobot : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      string sUid = Request["uid"];
      AntiRobotImage ari = new AntiRobotImage(sUid, 150, 50);

      Response.Clear();

      Response.ContentType = "image/jpeg";
      ari.Image.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
    }
  }
}