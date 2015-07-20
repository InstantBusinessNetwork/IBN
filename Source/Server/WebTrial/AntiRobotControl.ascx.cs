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
  public partial class AntiRobotControl : System.Web.UI.UserControl
  {
    private Random random = new Random();

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
		if (hidValue.Value == "")
		{
			hidValue.Value = random.Next(100000, 999999).ToString();
		}
		imgARobot.ImageUrl = "AntiRobot.aspx?uid=" + hidValue.Value;
    }

    protected void cvalKeyword_ServerValidate(object source, ServerValidateEventArgs args)
    {
      if (txtKeyword.Text == hidValue.Value)
      {
        hidValue.Value = "";
        args.IsValid = true;
      }
      else
      {
        args.IsValid = false;
        Random random = new Random();
        hidValue.Value = random.Next(100000, 999999).ToString();
      }
    }

  } 
}
