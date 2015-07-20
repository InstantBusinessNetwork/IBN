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
	using Mediachase.UI.Web.Util;
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.UI.WebControls;

	public partial class ArticleEdit : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(ArticleEdit).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strAddGroup", typeof(ArticleEdit).Assembly);
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region ArticleId
		private int articleId = 0;
		protected int ArticleId
		{
			get
			{
				return articleId;
			}
			set
			{
				articleId = value;
			}
		}
		#endregion

		#region Back
		protected string Back
		{
			get
			{
				if (Request.QueryString["back"] != null)
					return Request.QueryString["back"].ToLower();
				else
					return "";
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			LoadRequestVariables();

			PreparePage();

			if (!IsPostBack)
				BindData();
		}

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request["ArticleId"] != null)
				ArticleId = int.Parse(Request["ArticleId"]);
		}
		#endregion

		#region PreparePage
		private void PreparePage()
		{
			fckEditor.Language = Security.CurrentUser.Culture;
			fckEditor.EnableSsl = Request.IsSecureConnection;
			fckEditor.SslUrl = ResolveUrl("~/Common/Empty.html");
			fckEditor.DesignModeCss = UtilHelper.ResolveUrl(Page, "~/Styles/IbnFramework/windows.css");

			if (ArticleId > 0)
				secHeader.Title = LocRM.GetString("tArticleEdit");
			else
				secHeader.Title = LocRM.GetString("tArticleNew");

			btnSave.Text = LocRM2.GetString("Save");
			btnCancel.Text = LocRM2.GetString("Cancel");

			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.CustomImage = Page.ResolveUrl("~/Layouts/Images/saveitem.gif");
			secHeader.AddImageLink(Page.ResolveUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("atclBackToList"), Page.ResolveUrl("~/Apps/HelpDeskManagement/Pages/ArticleListMain.aspx"));
		}
		#endregion

		#region BindData
		private void BindData()
		{
			if (ArticleId > 0)
			{
				using (IDataReader reader = Common.GetArticle(ArticleId))
				{
					if (reader.Read())
					{
						txtQuestion.Text = reader["Question"].ToString();
						fckEditor.Text = reader["AnswerHTML"].ToString();
						txtTags.Text = reader["Tags"].ToString();
						CommonHelper.SafeSelect(ddlDelimiter, reader["Delimiter"].ToString());
					}
				}
			}
		}
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			if (ArticleId > 0)
				Common.UpdateArticle(ArticleId, txtQuestion.Text, fckEditor.Text, txtTags.Text, ddlDelimiter.SelectedValue);
			else
			{
				ArticleId = Common.AddArticle(txtQuestion.Text, fckEditor.Text, txtTags.Text, ddlDelimiter.SelectedValue);

				// Ensure article visibility in list
				pc["ArticleListMain_Mode"] = ArticleListMain.MODE_SEARCH;
				pc["ArticleListMain_Tag"] = "";
				pc["ArticleListMain_Search"] = "";
				pc["ArticleListMain_CurrentPage"] = "0";
			}

			if (Back == "list")
				Response.Redirect("~/Apps/HelpDeskManagement/Pages/ArticleListMain.aspx");
			else
				Response.Redirect(String.Format("~/Incidents/ArticleView.aspx?ArticleId={0}", ArticleId));
		}
		#endregion

		#region btnCancel_ServerClick
		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			if (Back == "list" || ArticleId <= 0)
				Response.Redirect("~/Apps/HelpDeskManagement/Pages/ArticleListMain.aspx");
			else
				Response.Redirect(String.Format("~/Incidents/ArticleView.aspx?ArticleId={0}", ArticleId));
		}
		#endregion
	}

}