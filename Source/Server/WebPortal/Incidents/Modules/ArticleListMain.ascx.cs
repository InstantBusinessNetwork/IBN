namespace Mediachase.UI.Web.Incidents.Modules
{
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

  using Mediachase.IBN.Business;

  public partial class ArticleListMain : System.Web.UI.UserControl
  {
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(ArticleListMain).Assembly);
    protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentsList", typeof(ArticleListMain).Assembly);
    UserLightPropertyCollection pc = Security.CurrentUser.Properties;

    public const string MODE_SEARCH = "search";
    public const string MODE_TAGS = "tags";

    protected void Page_Load(object sender, EventArgs e)
    {
      BindControls();

      if (!IsPostBack)
        BindGrid();
    }

    #region BindControls
    private void BindControls()
    {
      if (pc["ArticleListMain_Sort"] == null)
        pc["ArticleListMain_Sort"] = "Created DESC";
      if (pc["ArticleListMain_Search"] == null)
        pc["ArticleListMain_Search"] = "";
      if (pc["ArticleListMain_Mode"] == null)
        pc["ArticleListMain_Mode"] = MODE_SEARCH;
      if (pc["ArticleListMain_Tag"] == null)
        pc["ArticleListMain_Tag"] = "";

      txtSearch.Text = pc["ArticleListMain_Search"];
      ctrlTagCloud.Tag = pc["ArticleListMain_Tag"];

      btnSearch.Text = LocRM.GetString("Search");

      secHeader.Title = LocRM.GetString("tArticleList");
      secHeader.AddImageLink("../Layouts/images/newItem.gif", LocRM.GetString("atclNew"), "../Incidents/ArticleEdit.aspx");

      switch (pc["ArticleListMain_Mode"])
      {
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
          hdrTags.AddText(LocRM.GetString("Tags"));
          break;
        case MODE_TAGS:
          hdrList.AddText(String.Format("{0}: {1}", LocRM.GetString("SearchByTag"), pc["ArticleListMain_Tag"]));
          hdrList.AddRightLink(LocRM.GetString("atclAllArticles"), Page.ClientScript.GetPostBackClientHyperlink(btnReset, ""));
          hdrTags.AddText(LocRM.GetString("Tags") + ": " + pc["ArticleListMain_Tag"]);
          break;
      }
    }
    #endregion

    #region BindGrid
    private void BindGrid()
    {
      grdMain.Columns[1].HeaderText = LocRM.GetString("atclQuestion");
      //    grdMain.Columns[2].HeaderText = LocRM.GetString("atclAnswer");
      grdMain.Columns[2].HeaderText = LocRM.GetString("atclCreated");
      foreach (DataGridColumn dgc in grdMain.Columns)
      {
        if (dgc.SortExpression == pc["ArticleListMain_Sort"].ToString())
          dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='../layouts/images/upbtnF.jpg'/>";
        else if (dgc.SortExpression + " DESC" == pc["ArticleListMain_Sort"].ToString())
          dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='../layouts/images/downbtnF.jpg'/>";
      }

      DataView dv = null;

      if (pc["ArticleListMain_Mode"] == MODE_SEARCH)
        dv = Common.GetListArticles(pc["ArticleListMain_Search"]).DefaultView;
      else
        dv = Common.GetListArticlesByTag(pc["ArticleListMain_Tag"]).DefaultView;

      dv.Sort = pc["ArticleListMain_Sort"].ToString();
      grdMain.DataSource = dv;

      if (pc["ArticleListMain_PageSize"] != null)
        grdMain.PageSize = int.Parse(pc["ArticleListMain_PageSize"]);

      if (pc["ArticleListMain_CurrentPage"] != null)
      {
        int iPageIndex = int.Parse(pc["ArticleListMain_CurrentPage"]);
        int ppi = dv.Count / grdMain.PageSize;
        if (dv.Count % grdMain.PageSize == 0)
          ppi = ppi - 1;
        if (iPageIndex <= ppi)
          grdMain.CurrentPageIndex = iPageIndex;
        else
          grdMain.CurrentPageIndex = 0;

        pc["ArticleListMain_CurrentPage"] = grdMain.CurrentPageIndex.ToString(); ;
      }
      grdMain.DataBind();

      grdMain.PagerStyle.Visible = (dv.Count > 10);

      foreach (DataGridItem dgi in grdMain.Items)
      {
        ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
        if (ib != null)
          ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarningArticle") + "')");
      }
    }
    #endregion

    #region grdMain_PageSizeChanged
    protected void grdMain_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
    {
      pc["ArticleListMain_PageSize"] = e.NewPageSize.ToString();
      pc["ArticleListMain_CurrentPage"] = "0";
      Response.Redirect("../Incidents/ArticleListMain.aspx");
    }
    #endregion

    #region grdMain_PageIndexChanged
    protected void grdMain_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
    {
      pc["ArticleListMain_CurrentPage"] = e.NewPageIndex.ToString();
      Response.Redirect("../Incidents/ArticleListMain.aspx");
    }
    #endregion

    #region grdMain_DeleteCommand
    protected void grdMain_DeleteCommand(object source, DataGridCommandEventArgs e)
    {
      int id = int.Parse(e.Item.Cells[0].Text);
      Common.DeleteArticle(id);
      Response.Redirect("../Incidents/ArticleListMain.aspx");
    }
    #endregion

    #region grdMain_SortCommand
    protected void grdMain_SortCommand(object source, DataGridSortCommandEventArgs e)
    {
      if ((pc["ArticleListMain_Sort"] != null) && (pc["ArticleListMain_Sort"].ToString() == (string)e.SortExpression))
        pc["ArticleListMain_Sort"] = (string)e.SortExpression + " DESC";
      else
        pc["ArticleListMain_Sort"] = (string)e.SortExpression;
      pc["ArticleListMain_CurrentPage"] = "0";
      Response.Redirect("../Incidents/ArticleListMain.aspx");
    }
    #endregion

    #region btnSearch_Click
    protected void btnSearch_Click(object sender, EventArgs e)
    {
      pc["ArticleListMain_Mode"] = MODE_SEARCH;
      pc["ArticleListMain_Tag"] = "";
      pc["ArticleListMain_Search"] = txtSearch.Text.Trim();
      pc["ArticleListMain_CurrentPage"] = "0";
      Response.Redirect("../Incidents/ArticleListMain.aspx");
    }
    #endregion

    #region btnReset_Click
    protected void btnReset_Click(object sender, EventArgs e)
    {
      pc["ArticleListMain_Mode"] = MODE_SEARCH;
      pc["ArticleListMain_Tag"] = "";
      pc["ArticleListMain_Search"] = "";
      pc["ArticleListMain_CurrentPage"] = "0";
      txtSearch.Text = "";
      Response.Redirect("../Incidents/ArticleListMain.aspx");
    }
    #endregion

    #region ctrlTagCloud_TagClick
    protected void ctrlTagCloud_TagClick(object sender, EventArgs e)
    {
      pc["ArticleListMain_Mode"] = MODE_TAGS;
      pc["ArticleListMain_Tag"] = ctrlTagCloud.Tag;
      pc["ArticleListMain_Search"] = "";
      pc["ArticleListMain_CurrentPage"] = "0";
      txtSearch.Text = "";
      Response.Redirect("../Incidents/ArticleListMain.aspx");
    }
    #endregion
  }

}