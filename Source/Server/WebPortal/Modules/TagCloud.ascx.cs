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

using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Modules
{
  public partial class TagCloud : System.Web.UI.UserControl
  {
    #region Tag
    public string Tag
    {
      set
      {
        hdnTag.Value = value;
      }
      get
      {
        return hdnTag.Value;
      }
    }
    #endregion

    #region TagSizeCount
    public int TagSizeCount
    {
      get
      {
        if (ViewState["TagSizeCount"] == null)
          ViewState["TagSizeCount"] = 13; // default value
        return (int)ViewState["TagSizeCount"];
      }
      set
      {
        ViewState["TagSizeCount"] = value;
      }
    }
    #endregion

    #region TagCount
    public int TagCount
    {
      get
      {
        if (ViewState["TagCount"] == null)
          ViewState["TagCount"] = 30; // default value
        return (int)ViewState["TagCount"];
      }
      set
      {
        ViewState["TagCount"] = value;
      }
    }
    #endregion

    #region ObjectType
    public int ObjectType
    {
      get
      {
        if (ViewState["ObjectType"] == null)
          ViewState["ObjectType"] = (int)ObjectTypes.KnowledgeBase; // default value
        return (int)ViewState["ObjectType"];
      }
      set
      {
        ViewState["ObjectType"] = value;
      }
    }
    #endregion

    public delegate void TagClickEventHandler(object sender, System.EventArgs e);
    public event TagClickEventHandler TagClick;

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    #region BindTags
    private void BindTags()
    {
      if (Tag != "")
        rptTags.DataSource = Mediachase.IBN.Business.Common.GetListTagsForCloudByTag(ObjectType, TagSizeCount, TagCount, Tag);
      else
        rptTags.DataSource = Mediachase.IBN.Business.Common.GetListTagsForCloud(ObjectType, TagSizeCount, TagCount);
      rptTags.DataBind();
    }
    #endregion

    #region btnTag_Click
    protected void btnTag_Click(object sender, EventArgs e)
    {
      TagClick(sender, e);
    }
    #endregion

    #region Page_PreRender
    protected void Page_PreRender(object sender, EventArgs e)
    {
      BindTags();
    }
    #endregion
  }
}