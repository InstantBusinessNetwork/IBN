namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Collections;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.Interfaces;
	
	/// <summary>
	///		Summary description for EditCategories.
	/// </summary>
	public partial class EditCategories : System.Web.UI.UserControl, IPageTemplateTitle
	{

		protected ArrayList SelectedCategories = new ArrayList();
		protected ArrayList SelectedIncidentCategories = new ArrayList();

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentEdit", typeof(EditCategories).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(EditCategories).Assembly);

		#region IncidentId
		private int IncidentId
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "IncidentId", 0);
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
				BindValues();

			btnSave.Text = LocRM2.GetString("tbsave_save");
			btnCancel.Text = LocRM2.GetString("tbsave_cancel");

			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnCancel.Attributes.Add("onclick", "window.close()");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			btnAddGeneralCategory.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.Categories + ",'" + btnRefresh.ClientID + "')");
			btnAddIncidentCategory.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.IncidentCategories + ",'" + btnRefresh.ClientID + "')");
			btnAddGeneralCategory.Attributes.Add("title", LocRM2.GetString("AddCategory"));
			btnAddIncidentCategory.Attributes.Add("title", LocRM2.GetString("AddCategory"));
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			using (IDataReader reader = Incident.GetListCategories(IncidentId))
			{
				while (reader.Read())
					SelectedCategories.Add((int)reader["CategoryId"]);
			}

			using (IDataReader reader = Incident.GetListIncidentCategoriesByIncident(IncidentId))
			{
				while (reader.Read())
					SelectedIncidentCategories.Add((int)reader["CategoryId"]);
			}

			BindGeneralCategories();
			BindIncidentCategories();

			trCategories.Visible = PortalConfig.CommonIncidentAllowEditGeneralCategoriesField;
			trIssCategories.Visible = PortalConfig.IncidentAllowEditIncidentCategoriesField;
		}

		private void BindGeneralCategories()
		{
			grdCategories.DataSource = Incident.GetListCategoriesAll();
			grdCategories.DataBind();
		}

		private void BindIncidentCategories()
		{
			grdIncidentCategories.DataSource = Incident.GetListIncidentCategories();
			grdIncidentCategories.DataBind();
		}
		#endregion

		#region btnSave_ServerClick
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			ArrayList categories = new ArrayList();
			ArrayList incidentCategories = new ArrayList();

			CollectSelectedValues(grdCategories, categories);
			CollectSelectedValues(grdIncidentCategories, incidentCategories);

			Issue2.UpdateCategories(IncidentId, categories, incidentCategories);

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"try {window.opener.top.frames['right'].location.href='../Incidents/IncidentView.aspx?IncidentId=" + IncidentId + "';}" +
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
				CollectSelectedValues(grdIncidentCategories, SelectedIncidentCategories);

				BindGeneralCategories();
				BindIncidentCategories();
			}
		}
		#endregion

		#region IPageTemplateTitle Members

		public string Modify(string oldValue)
		{
			return LocRM2.GetString("Categorization");
		}

		#endregion
	}
}
