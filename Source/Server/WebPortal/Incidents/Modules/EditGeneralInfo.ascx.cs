namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.UI.WebControls;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for EditGeneralInfo.
	/// </summary>
	public partial class EditGeneralInfo : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentEdit", typeof(EditGeneralInfo).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(EditGeneralInfo).Assembly);

		#region IncidentId
		private int IncidentId
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "IncidentId", 0);
			}
		}
		#endregion

		#region Command
		protected string Command
		{
			get
			{
				string retval = String.Empty;
				if (Request["cmd"] != null)
					retval = Request["cmd"];

				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!Page.IsPostBack)
			{
				BindDefaultValues();
				BindSavedValues();
			}

			if (String.IsNullOrEmpty(Command))
				btnCancel.Attributes.Add("onclick","window.close();return false;");
			else
				btnCancel.Attributes.Add("onclick", Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, "", false, true));
			btnSave.Attributes.Add("onclick","DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnSave.Text = LocRM.GetString("tbSaveSave");
			btnCancel.Text = LocRM.GetString("tbSaveCancel");
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
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			ddlPriority.DataSource = Incident.GetListPriorities();
			ddlPriority.DataBind();
			ddlPriority.ClearSelection();
			ListItem liPriority = ddlPriority.Items.FindByValue(Priority.Normal.GetHashCode().ToString());
			if(liPriority != null)
				liPriority.Selected = true;

			ddlType.DataSource = Incident.GetListIncidentTypes();
			ddlType.DataBind();

			ddlSeverity.DataSource = Incident.GetListIncidentSeverity();
			ddlSeverity.DataBind();

			trPriority.Visible = PortalConfig.CommonIncidentAllowEditPriorityField;
			trType.Visible = PortalConfig.IncidentAllowEditTypeField;
			trSeverity.Visible = PortalConfig.IncidentAllowEditSeverityField;
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			using (IDataReader reader = Incident.GetIncident(IncidentId))
			{
				if(reader.Read())
				{
					txtTitle.Text = HttpUtility.HtmlDecode(reader["Title"].ToString());
					ddlPriority.ClearSelection();
					CommonHelper.SafeSelect(ddlPriority, reader["PriorityId"].ToString());
					CommonHelper.SafeSelect(ddlType, reader["TypeId"].ToString());
					CommonHelper.SafeSelect(ddlSeverity, reader["SeverityId"].ToString());
					if(reader["Description"] != DBNull.Value)
						txtDescription.Text = HttpUtility.HtmlDecode(reader["Description"].ToString());
				}
			}
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			txtTitle.Text = HttpUtility.HtmlEncode(txtTitle.Text);
			txtDescription.Text = HttpUtility.HtmlEncode(txtDescription.Text);

			Issue2.UpdateGeneralInfo(IncidentId, txtTitle.Text, txtDescription.Text, int.Parse(ddlType.SelectedValue), int.Parse(ddlSeverity.SelectedValue));
			Issue2.UpdatePriority(IncidentId, int.Parse(ddlPriority.SelectedValue));

			if (!String.IsNullOrEmpty(Command))	// popup mode
			{
				CommandParameters cp = new CommandParameters(Command);
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"try {window.opener.top.frames['right'].location.href='../Incidents/IncidentView.aspx?IncidentId=" + IncidentId + "';}" +
					"catch (e){} window.close();", true);
			}
		}
		#endregion

		#region IPageTemplateTitle Members

		public string Modify(string oldValue)
		{
			return LocRM2.GetString("general_info");
		}

		#endregion
	}
}
