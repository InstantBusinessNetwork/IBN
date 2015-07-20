namespace Mediachase.UI.Web.Incidents.Modules
{
  using System;
  using System.Data;
  using System.Drawing;
  using System.Web;
  using System.Web.UI.WebControls;
  using System.Web.UI.HtmlControls;
  using System.Resources;
  using CS = Mediachase.IBN.Business.ControlSystem;
  using Mediachase.IBN.Business;

  public partial class ArticleList : System.Web.UI.UserControl
  {
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(ArticleList).Assembly);
    protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(ArticleList).Assembly);
    UserLightPropertyCollection pc = Security.CurrentUser.Properties;

    const string MODE_LATEST = "latest";
    const string MODE_SEARCH = "search";
    const string MODE_TAGS = "tags";

    #region guid
    protected string guid
    {
      get
      {
        if (Request["guid"] != null)
          return Request["guid"];
        return "";
      }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
      Page.ClientScript.RegisterClientScriptInclude(this.GetType(), Guid.NewGuid().ToString(),
        ResolveUrl("~/Scripts/emailsend.js"));

      BindControls();

      if (!IsPostBack)
      {
        ViewState["mode"] = MODE_LATEST;
        BindGrid();
      }
    }

    #region BindGrid
    private void BindGrid()
    {
      DataView dv = null;

      if (ViewState["mode"].ToString() == MODE_LATEST)
      {
        dv = Common.GetListArticlesUsedByUser().DefaultView;

        if (dv.Count == 0)
          ViewState["mode"] = MODE_SEARCH;
      }

      if (ViewState["mode"].ToString() == MODE_SEARCH)
        dv = Common.GetListArticles(txtSearch.Text.Trim()).DefaultView;

      if (ViewState["mode"].ToString() == MODE_TAGS)
        dv = Common.GetListArticlesByTag(ctrlTagCloud.Tag).DefaultView;

      if (ViewState["SelectArticle_CurrentPage"] == null)
        ViewState["SelectArticle_CurrentPage"] = 0;

      grdMain.DataSource = dv;

      if (ViewState["SelectArticle_PageSize"] != null)
        grdMain.PageSize = (int)ViewState["SelectArticle_PageSize"];

      int iPageIndex = (int)ViewState["SelectArticle_CurrentPage"];
      if (iPageIndex <= dv.Count / grdMain.PageSize)
        grdMain.CurrentPageIndex = iPageIndex;
      else
        grdMain.CurrentPageIndex = 0;

      grdMain.DataBind();

      grdMain.PagerStyle.Visible = (dv.Count > 10);

      foreach (DataGridItem dgi in grdMain.Items)
      {
        ImageButton ib = (ImageButton)dgi.FindControl("ibSelect");
        if (ib != null)
        {
          ib.ToolTip = LocRM.GetString("Select");

          //string sId = dgi.Cells[0].Text;

          //string sAction = "CloseAll('" + sId + "');";
          //ib.Attributes.Add("onclick", sAction);
        }
      }
    }
    #endregion

    #region btnSearch_Click
    protected void btnSearch_Click(object sender, EventArgs e)
    {
      ViewState["mode"] = MODE_SEARCH;
      ctrlTagCloud.Tag = "";
      BindGrid();
    }
    #endregion

    #region BindControls
    private void BindControls()
    {
      btnSearch.Text = LocRM.GetString("Search");
    }
    #endregion

    #region Page_PreRender
    protected void Page_PreRender(object sender, EventArgs e)
    {
      BindToolbar();
    }
    #endregion

    #region Render
    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
      foreach (DataGridItem dgi in grdMain.Items)
      {
        ImageButton ib = (ImageButton)dgi.FindControl("ibSelect");
        if (ib != null)
        {
          Page.ClientScript.RegisterForEventValidation(ib.UniqueID);
        }
      }
      base.Render(writer);
    }
    #endregion

    #region grdMain_PageIndexChanged
    protected void grdMain_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {
      ViewState["SelectArticle_CurrentPage"] = e.NewPageIndex;
      BindGrid();
    }
    #endregion

    #region grdMain_PageSizeChanged
    protected void grdMain_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
    {
      ViewState["SelectArticle_PageSize"] = e.NewPageSize;
      ViewState["SelectArticle_CurrentPage"] = 0;
      BindGrid();
    }
    #endregion

    #region grdMain_ItemCommand
    protected void grdMain_ItemCommand(object source, DataGridCommandEventArgs e)
    {
      if (e.CommandName == "Select")
      {
        int selId = int.Parse(e.Item.Cells[0].Text);

        string sText = "";
        string sTitle = "";
        using (IDataReader reader = Mediachase.IBN.Business.Common.GetArticle(selId))
        {
          if (reader.Read())
          {
            sTitle = reader["Question"].ToString();
            sText = String.Format("<div style='border-top: solid 1px #95B7F3; padding:0px;background-color:ffffe1;'>"+
                      "<div style='padding:5px; cursor:default;background-color:#FFD275;'><b>{0}</b></div>"+
											"<div style='padding:5px;'>{1}</div>"+
										"</div>", reader["Question"].ToString(), reader["AnswerHTML"].ToString());
          }
        }

        Common.AddHistory(ObjectTypes.KnowledgeBase, selId, sTitle);
        Common.IncreaseArticleCounter(selId);
        
        string sFiles = "";
        string containerName = "FileLibrary";
        string containerKey = UserRoleHelper.CreateArticleContainerKey(selId);
        CS.BaseIbnContainer bic = CS.BaseIbnContainer.Create(containerName, containerKey);
        CS.FileStorage fs = (CS.FileStorage)bic.LoadControl("FileStorage");
        CS.FileInfo[] _fi = fs.Root.GetFiles();
        if (_fi.Length > 0)
        {
          string _containerName = "FileLibrary";
          string _containerKey = "EMailAttach";
          CS.BaseIbnContainer _bic = CS.BaseIbnContainer.Create(_containerName, _containerKey);
          CS.FileStorage _fs = (CS.FileStorage)_bic.LoadControl("FileStorage");
          CS.DirectoryInfo di = _fs.GetDirectory(_fs.Root.Id, guid, true);
          foreach (CS.FileInfo fi in _fi)
            fs.CopyFile(fi.Id, di.Id, true);

          _fi = _fs.GetFiles(di);
          foreach (CS.FileInfo fi in _fi)
          {
            sFiles += String.Format("<div style='padding-bottom:1px;'><img align='absmiddle' src='{0}' width='16' height='16'>&nbsp;{1}&nbsp;&nbsp;<img src='{2}' align='absmiddle' width='16' height='16' style='cursor:pointer;' onclick='_deleteFile({3})' title='{4}' /></div>",
              ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId),
              Util.CommonHelper.GetShortFileName(fi.Name, 40),
              ResolveUrl("~/Layouts/Images/delete.gif"),
              fi.Id,
              LocRM2.GetString("tDelete"));
          }
        }
        sText = sText.Replace("\r\n", "");
        sText = sText.Replace("\t", "");
		sText = sText.Replace("\\", "\\\\");
		sText = sText.Replace("\"", "\\\"");
        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), String.Format("CloseAll(\"{0}\", \"{1}\");", sFiles, sText), true);
      }
    }
    #endregion

    #region BindToolbar
    private void BindToolbar()
    {
      secHeader.Title = LocRM.GetString("SelectArticle");
      secHeader.AddImageLink("../Layouts/images/cancel.gif", LocRM.GetString("Close"), "javascript:window.close();");

      switch (ViewState["mode"].ToString())
      {
        case MODE_LATEST:
          hdrList.AddText(LocRM.GetString("LatestArticles"));
          hdrList.AddRightLink(LocRM.GetString("atclAllArticles"), Page.ClientScript.GetPostBackClientHyperlink(btnReset, ""));
          break;
        case MODE_SEARCH:
          if (txtSearch.Text.Trim() != "")
          {
            hdrList.AddText(LocRM.GetString("SearchResults"));
            hdrList.AddRightLink(LocRM.GetString("atclAllArticles"), Page.ClientScript.GetPostBackClientHyperlink(btnReset, ""));
          }
          else
          {
            hdrList.AddText(LocRM.GetString("atclAllArticles"));
          }
          break;
        case MODE_TAGS:
          hdrList.AddText(String.Format("{0}: {1}", LocRM.GetString("SearchByTag"), ctrlTagCloud.Tag));
          hdrList.AddRightLink(LocRM.GetString("atclAllArticles"), Page.ClientScript.GetPostBackClientHyperlink(btnReset, ""));
          break;
      }

      if (ViewState["mode"].ToString() == MODE_TAGS)
        hdrTags.AddText(LocRM.GetString("Tags") + ": " + ctrlTagCloud.Tag);
      else
        hdrTags.AddText(LocRM.GetString("Tags"));
    }
    #endregion

    #region ctrlTagCloud_TagClick
    protected void ctrlTagCloud_TagClick(object sender, EventArgs e)
    {
      ViewState["mode"] = MODE_TAGS;
      txtSearch.Text = "";
      BindGrid();
    }
    #endregion

    #region btnReset_Click
    protected void btnReset_Click(object sender, EventArgs e)
    {
      ViewState["mode"] = MODE_SEARCH;
      txtSearch.Text = "";
      ctrlTagCloud.Tag = "";
      BindGrid();
    }
    #endregion

    #region GetSubstring
    protected string GetSubstring(string s, int lenght)
    {
      if (s.Length > lenght)
        return s.Substring(0, lenght);
      else
        return s;
    }
    #endregion
  }

}