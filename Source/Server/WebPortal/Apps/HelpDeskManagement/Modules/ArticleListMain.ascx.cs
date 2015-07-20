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

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules
{
	public partial class ArticleListMain : System.Web.UI.UserControl
	{
		private const string _className = "Article";
		private string _viewName = "";
		private const string _placeName = "ArticleList";
		protected UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		
		public const string MODE_SEARCH = "search";
		public const string MODE_TAGS = "tags";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (pc["ArticleListMain_Search"] == null)
				pc["ArticleListMain_Search"] = "";
			if (pc["ArticleListMain_Mode"] == null)
				pc["ArticleListMain_Mode"] = MODE_SEARCH;
			if (pc["ArticleListMain_Tag"] == null)
				pc["ArticleListMain_Tag"] = "";
			txtSearch.Text = pc["ArticleListMain_Search"];
			ctrlTagCloud.Tag = pc["ArticleListMain_Tag"];

			grdMain.ClassName = _className;
			grdMain.ViewName = _viewName;
			grdMain.PlaceName = _placeName;

			MainMetaToolbar.ClassName = _className;
			MainMetaToolbar.ViewName = _viewName;
			MainMetaToolbar.PlaceName = _placeName;

			BindDataGrid(!Page.IsPostBack);

			if (!Page.IsPostBack)
				BindBlockHeader();
		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			if (CHelper.NeedToBindGrid())
			{
				BindDataGrid(true);
				if (pc["ArticleListMain_Tag"] != null && String.Compare(pc["ArticleListMain_Tag"], ctrlTagCloud.Tag) != 0)
					ctrlTagCloud.Tag = pc["ArticleListMain_Tag"];
				txtSearch.Text = (pc["ArticleListMain_Search"] != null) ? pc["ArticleListMain_Search"] : String.Empty;
				upTagCloud.Update();
			}

			base.OnPreRender(e);
		}
		#endregion

		#region BindBlockHeader
		/// <summary>
		/// Binds the tool bar.
		/// </summary>
		private void BindBlockHeader()
		{
			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.Incident", "ArticlesList").ToString();

			btnSearch.Text = GetGlobalResourceObject("IbnFramework.Incident", "Search").ToString();
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			DataView dv = null;

			if (pc["ArticleListMain_Mode"] == MODE_SEARCH)
				dv = Mediachase.IBN.Business.Common.GetListArticles(pc["ArticleListMain_Search"]).DefaultView;
			else
				dv = Mediachase.IBN.Business.Common.GetListArticlesByTag(pc["ArticleListMain_Tag"]).DefaultView;

			grdMain.DataSource = dv;

			if (dataBind)
				grdMain.DataBind();
		}
		#endregion

		#region btnSearch_Click
		protected void btnSearch_Click(object sender, EventArgs e)
		{
			pc["ArticleListMain_Mode"] = MODE_SEARCH;
			pc["ArticleListMain_Tag"] = "";
			pc["ArticleListMain_Search"] = txtSearch.Text.Trim();
			CHelper.RequireBindGrid();
			grdMainPanel.Update();
		}
		#endregion

		#region ctrlTagCloud_TagClick
		protected void ctrlTagCloud_TagClick(object sender, EventArgs e)
		{
			pc["ArticleListMain_Mode"] = MODE_TAGS;
			pc["ArticleListMain_Tag"] = ctrlTagCloud.Tag;
			pc["ArticleListMain_Search"] = "";
			txtSearch.Text = "";
			CHelper.RequireBindGrid();
			upToolbarSearch.Update();
			grdMainPanel.Update();
		}
		#endregion
	}
}