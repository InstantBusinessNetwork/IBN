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
	using Mediachase.IBN.Business.ControlSystem;
	using Mediachase.IBN.Business.WebDAV.Common;

	public partial class ArticleView : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(ArticleView).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentsList", typeof(ArticleView).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strFileLibrary", typeof(ArticleView).Assembly);

		private string ContainerName = "FileLibrary";
		private string ContainerKey = "";
		private BaseIbnContainer bic = null;
		private FileStorage fs = null;
		private int RootFolderId = -1;

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

		protected void Page_Load(object sender, EventArgs e)
		{
			LoadRequestVariables();

			PreparePage();

			if (!IsPostBack)
			{
				BindData();
				BindFiles();
				Common.AddHistory(ObjectTypes.KnowledgeBase, ArticleId, lblQuestion.Text);
			}
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

			secHeader.Title = LocRM.GetString("tArticleView");
			secHeader.AddImageLink("../Layouts/Images/edit.gif", LocRM2.GetString("tEdit"),
						String.Format("../Incidents/ArticleEdit.aspx?ArticleId={0}", ArticleId));

			ContainerKey = UserRoleHelper.CreateArticleContainerKey(ArticleId);
			bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
			fs = (FileStorage)bic.LoadControl("FileStorage");
			RootFolderId = fs.Root.Id;

			secHeader.AddImageLink("../Layouts/Images/icons/newfile.gif", LocRM3.GetString("tAddD"),
				String.Format("javascript:ShowWizard('{0}?ParentFolderId={1}&ContainerKey={2}&ContainerName={3}', 470, 270);",
				ResolveUrl("~/FileStorage/FileUpload.aspx"),
				RootFolderId, ContainerKey, ContainerName));
			secHeader.AddImageLink("../Layouts/Images/cancel.gif", LocRM.GetString("atclBackToList"), ResolveClientUrl("~/Apps/HelpDeskManagement/Pages/ArticleListMain.aspx"));
		}
		#endregion

		#region BindData
		private void BindData()
		{
			// Article
			using (IDataReader reader = Common.GetArticle(ArticleId))
			{
				if (reader.Read())
				{
					lblQuestion.Text = reader["Question"].ToString();
					lblAnswer.Text = reader["AnswerHTML"].ToString();
					lblTags.Text = reader["Tags"].ToString();
					lblCreated.Text = String.Format("{0} {1}",
						((DateTime)reader["Created"]).ToShortDateString(),
						((DateTime)reader["Created"]).ToShortTimeString());
				}
			}
		}
		#endregion

		#region BindFiles
		private void BindFiles()
		{
			FileInfo[] files = fs.GetFiles(RootFolderId);
			if (files.Length <= 0)
			{
				trFiles.Visible = false;
				return;
			}

			trFiles.Visible = true;
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("FileId", typeof(int)));
			dt.Columns.Add(new DataColumn("FileLink", typeof(string)));

			DataRow dr;
			foreach (FileInfo fi in files)
			{
				dr = dt.NewRow();
				dr["FileId"] = fi.Id;

				string sIconLink = ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId);
				string sIcon = String.Format("<img src='{0}' width='16' height='16' border=0 align=absmiddle>", sIconLink);

				string sLink = "";
				if (fi.FileBinaryContentType.ToLower().IndexOf("url") >= 0)
					sLink = Util.CommonHelper.GetLinkText(fs, fi);
				if (sLink == "")
					sLink = WebDavUrlBuilder.GetFileStorageWebDavUrl(fi, true);
				
				dr["FileLink"] = String.Format("<a href=\"{0}\" title='{1}'{2}>{3} {4}</a>",
					sLink,
					fi.Name,
					Common.OpenInNewWindow(fi.FileBinaryContentType) ? " target='_blank'" : "",
					sIcon,
					Util.CommonHelper.GetShortFileName(fi.Name, 40)
					);
				dt.Rows.Add(dr);
			}

			rptFiles.DataSource = dt;
			rptFiles.DataBind();


			foreach (RepeaterItem item in rptFiles.Items)
			{
				ImageButton ib = (ImageButton)item.FindControl("ibDelete");
				if (ib != null)
				{
					ib.Attributes.Add("onclick", "return confirm('" + LocRM3.GetString("MesDeleteD") + "');");
					ib.ToolTip = LocRM3.GetString("DeleteDocument");
				}
			}

		}
		#endregion

		#region rptFiles_ItemCommand
		protected void rptFiles_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
			{
				fs.DeleteFile(int.Parse(e.CommandArgument.ToString()));
			}

			Response.Redirect(String.Format("../Incidents/ArticleView.aspx?ArticleId={0}", ArticleId));
		}
		#endregion
	}
}