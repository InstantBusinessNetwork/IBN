using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for EditCategories.
	/// </summary>
	public partial class EditCategories : System.Web.UI.Page
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(EditCategories).Assembly);
		protected ArrayList SelectedCategories = new ArrayList();
		protected ArrayList SelectedProjectCategories = new ArrayList();
		protected ArrayList SelectedPortfolios = new ArrayList();

		#region ProjectId
		private int ProjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			if (!Page.IsPostBack)
				BindValues();

			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");

			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnCancel.Attributes.Add("onclick", "window.close()");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			btnAddGeneralCategory.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.Categories + ",'" + btnRefresh.ClientID + "')");
			btnAddProjectCategory.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.ProjectCategories + ",'" + btnRefresh.ClientID + "')");
			btnAddProjectCategory.Attributes.Add("title", LocRM.GetString("AddCategory"));
			btnAddGeneralCategory.Attributes.Add("title", LocRM.GetString("AddCategory"));
		}

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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			using (IDataReader reader = Project.GetListCategories(ProjectId))
			{
				while (reader.Read())
					SelectedCategories.Add((int)reader["CategoryId"]);
			}

			using (IDataReader reader = Project.GetListProjectCategoriesByProject(ProjectId))
			{
				while (reader.Read())
					SelectedProjectCategories.Add((int)reader["CategoryId"]);
			}

			using (IDataReader reader = ProjectGroup.ProjectGroupsGetByProject(ProjectId))
			{
				while (reader.Read())
					SelectedPortfolios.Add((int)reader["ProjectGroupId"]);
			}

			BindGeneralCategories();
			BindProjectCategories();
			BindPortfolios();

			trCategories.Visible = PortalConfig.GeneralAllowGeneralCategoriesField;
		}

		private void BindGeneralCategories()
		{
			grdCategories.DataSource = Project.GetListCategoriesAll();
			grdCategories.DataBind();
		}

		private void BindProjectCategories()
		{
			grdProjectCategories.DataSource = Project.GetListProjectCategories();
			grdProjectCategories.DataBind();
		}

		private void BindPortfolios()
		{
			grdPortfolios.DataSource = ProjectGroup.GetProjectGroups();
			grdPortfolios.DataBind();
		}
		#endregion

		#region btnSave_ServerClick
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			ArrayList categories = new ArrayList();
			ArrayList projectCategories = new ArrayList();
			ArrayList portfolios = new ArrayList();

			CollectSelectedValues(grdCategories, categories);
			CollectSelectedValues(grdProjectCategories, projectCategories);
			CollectSelectedValues(grdPortfolios, portfolios);

			Project2.UpdateCategories(ProjectId, categories, projectCategories, portfolios);

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "try {window.opener.top.frames['right'].location.href='../Projects/ProjectView.aspx?ProjectId=" + ProjectId + "';}" +
					  "catch (e){} window.close();", true);
		}
		#endregion

		#region CollectSelectedValues
		private void CollectSelectedValues(DataGrid grd, ArrayList arr)
		{
			foreach (DataGridItem dgi in grd.Items)
			{
				foreach (Control control in dgi.Cells[1].Controls)
				{
					if (control is CheckBox)
					{
						CheckBox checkBox = (CheckBox)control;
						if (checkBox.Checked)
							arr.Add(int.Parse(dgi.Cells[0].Text));
					}
				}
			}
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			if (Page.IsPostBack)
			{
				CollectSelectedValues(grdCategories, SelectedCategories);
				CollectSelectedValues(grdProjectCategories, SelectedProjectCategories);

				BindGeneralCategories();
				BindProjectCategories();
			}
		}
		#endregion
	}
}
