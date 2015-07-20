namespace Mediachase.UI.Web.Workspace.Modules
{
  using System;
  using System.Collections;
  using System.Configuration;
  using System.Data;
  using System.Drawing;
  using System.Web;
  using System.Web.UI.WebControls;
  using System.Web.UI.HtmlControls;
  using System.Resources;
  using Mediachase.IBN.Business;

  public partial class MCNews : System.Web.UI.UserControl
  {
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strWorkspace", typeof(MCNews).Assembly);
    protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strCustomizer", typeof(MCNews).Assembly);
    protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(MCNews).Assembly);

    protected void Page_Load(object sender, EventArgs e)
    {
      BindToolbar();
      BindValues();
	  ResolveClientUrl("~/Layouts/Images/loading_rss.gif");
    }

    private void BindValues()
    {
      Hashtable ht;
      try
      {
        ht = (Hashtable)System.Configuration.ConfigurationManager.GetSection("Mediachase.RssNews");
        string sUrl = "";
        if (ht.Contains(Security.CurrentUser.Culture.ToLower()))
          sUrl = ht[Security.CurrentUser.Culture.ToLower()].ToString();
        else
		{
			foreach(object obj in ht.Keys)
			{
				if(obj.ToString().ToLower().Contains("en"))
					sUrl = ht[obj.ToString()].ToString();
			}
		}
        Page.ClientScript.RegisterStartupScript(this.GetType(), "_load", "CreateRssNews('" + sUrl + "', 7, 'divMCNews');", true);
      }
      catch
      {
      }
    }
    
    #region BindToolbar
    private void BindToolbar()
    {
      secHeader.Title = LocRM2.GetString("tMCNews");
      //secHeader.AddLink("<img alt='' src='../Layouts/Images/rss_blue.gif' border='0' align='absmiddle' >", "javascript:{}");
      secHeader.AddLink("<img alt='' src='../Layouts/Images/b4.gif'/>", "javascript:" + Page.ClientScript.GetPostBackEventReference(lbHide, ""));
    }
    #endregion

    #region Web Form Designer generated code
    override protected void OnInit(EventArgs e)
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    ///		Required method for Designer support - do not modify
    ///		the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      lbHide.Click += new EventHandler(lbHide_Click);
    }
    #endregion

    private void lbHide_Click(object sender, EventArgs e)
    {
      //Util.CommonHelper.HideWorkspaceControl("12");
      Response.Redirect("../Workspace/default.aspx?BTab=Workspace");
    }

  }
}