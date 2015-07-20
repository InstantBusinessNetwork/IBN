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
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Incidents
{
  public partial class ArticleEdit : System.Web.UI.Page
  {
    #region ArticleId
    private int ArticleId
    {
      get
      {
        return CommonHelper.GetRequestInteger(Request, "ArticleId", 0);
      }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
      ApplyLocalization();
    }

    private void ApplyLocalization()
    {
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(ArticleEdit).Assembly);
      if (ArticleId != 0)
        pT.Title = LocRM.GetString("tArticleEdit");
      else
        pT.Title = LocRM.GetString("tArticleNew");
    }
  } 
}
