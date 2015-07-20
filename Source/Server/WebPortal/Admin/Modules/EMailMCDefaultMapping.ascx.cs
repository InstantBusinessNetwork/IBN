namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using System.Collections.Generic;
	using System.Globalization;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Clients;
	using System.Reflection;

	/// <summary>
	///		Summary description for EMailMCDefaultMapping.
	/// </summary>
	public partial class EMailMCDefaultMapping : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));



		#region BoxId
		public int BoxId
		{
			get
			{
				if (Request["BoxId"] != null)
					return int.Parse(Request["BoxId"]);
				else return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if (!Page.IsPostBack)
			{
				BindLists();
				BindValues();
			}
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			imbSave.Text = LocRM.GetString("tSave");
			imbSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			imbCancel.Text = LocRM.GetString("tCancel");
			imbCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");
		}
		#endregion

		#region BindLists
		private void BindLists()
		{
			ddTitle.Items.Clear();
			ddTitle.Items.Add(new ListItem(LocRM.GetString("tSubject"), "Subject"));

			ddCreator.Items.Clear();
			ddCreator.DataSource = User.GetListActive();
			ddCreator.DataValueField = "PrincipalId";
			ddCreator.DataTextField = "DisplayName";
			ddCreator.DataBind();

			ddType.DataSource = Incident.GetListIncidentTypes();
			ddType.DataTextField = "TypeName";
			ddType.DataValueField = "TypeId";
			ddType.DataBind();

			ddPriority.DataSource = Incident.GetListPriorities();
			ddPriority.DataTextField = "PriorityName";
			ddPriority.DataValueField = "PriorityId";
			ddPriority.DataBind();
			ddPriority.Items.Insert(0, new ListItem(LocRM.GetString("tFromEMail"), "-1"));

			ddSeverity.DataSource = Incident.GetListIncidentSeverity();
			ddSeverity.DataTextField = "SeverityName";
			ddSeverity.DataValueField = "SeverityId";
			ddSeverity.DataBind();

			lbGenCats.DataSource = Incident.GetListCategoriesAll();
			lbGenCats.DataTextField = "CategoryName";
			lbGenCats.DataValueField = "CategoryId";
			lbGenCats.DataBind();

			lbIssCats.DataSource = Incident.GetListIncidentCategories();
			lbIssCats.DataTextField = "CategoryName";
			lbIssCats.DataValueField = "CategoryId";
			lbIssCats.DataBind();

			ddIssBox.Items.Clear();
			ddIssBox.Items.Add(new ListItem(LocRM.GetString("tIssBoxAutoSelect"), "-1"));
			foreach (IncidentBox folder in IncidentBox.List())
			{
				ddIssBox.Items.Add(new ListItem(folder.Name, folder.IncidentBoxId.ToString()));
			}

			ddDescription.Items.Clear();
			ddDescription.Items.Add(new ListItem(LocRM.GetString("tNothing"), "-1"));
			ddDescription.Items.Add(new ListItem(LocRM.GetString("tDescriptionAsBody"), "0"));
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			EMailRouterPop3BoxSettings ib = EMailRouterPop3BoxSettings.Load(BoxId);
			DefaultEMailIncidentMappingBlock dimb = ib.DefaultEMailIncidentMappingBlock;
			Util.CommonHelper.SafeSelect(ddCreator, dimb.DefaultCreator.ToString());
			ucProject.ObjectTypeId = (int)ObjectTypes.Project;
			ucProject.ObjectId = dimb.ProjectId;
			Util.CommonHelper.SafeSelect(ddIssBox, dimb.IncidentBoxId.ToString());
			Util.CommonHelper.SafeSelect(ddPriority, dimb.PriorityId.ToString());
			Util.CommonHelper.SafeSelect(ddType, dimb.TypeId.ToString());
			Util.CommonHelper.SafeSelect(ddSeverity, dimb.SeverityId.ToString());
			foreach (int catid in dimb.GeneralCategories)
				Util.CommonHelper.SafeMultipleSelect(lbGenCats, catid.ToString());
			foreach (int catid in dimb.IncidentCategories)
				Util.CommonHelper.SafeMultipleSelect(lbIssCats, catid.ToString());
			if (dimb.OrgUid != PrimaryKeyId.Empty)
			{
				ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
				ClientControl.ObjectId = dimb.OrgUid;
			}
			else if (dimb.ContactUid != PrimaryKeyId.Empty)
			{
				ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
				ClientControl.ObjectId = dimb.ContactUid;
			}

			if (dimb.DescriptionId == 0)
				ddDescription.SelectedIndex = 1;
			else
				ddDescription.SelectedIndex = 0;

			//trClient.Visible = PortalConfig.CommonIncidentAllowEditClientField;
			//trPriority.Visible = PortalConfig.CommonIncidentAllowEditPriorityField;
			//trType.Visible = PortalConfig.IncidentAllowEditTypeField;
			//trSeverity.Visible = PortalConfig.IncidentAllowEditSeverityField;
			//trCategories.Visible = PortalConfig.CommonIncidentAllowEditGeneralCategoriesField;
			//trIssCategories.Visible = PortalConfig.IncidentAllowEditIncidentCategoriesField;
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
			this.imbSave.ServerClick += new EventHandler(imbSave_ServerClick);
		}
		#endregion

		#region Save-Cancel
		private void imbSave_ServerClick(object sender, EventArgs e)
		{
			EMailRouterPop3BoxSettings ib = EMailRouterPop3BoxSettings.Load(BoxId);
			DefaultEMailIncidentMappingBlock dimb = ib.DefaultEMailIncidentMappingBlock;

			dimb.DescriptionId = Convert.ToInt32(ddDescription.SelectedValue, CultureInfo.InvariantCulture);

			dimb.DefaultCreator = int.Parse(ddCreator.SelectedValue);
			dimb.ProjectId = ucProject.ObjectId;
			dimb.IncidentBoxId = int.Parse(ddIssBox.SelectedValue);
			dimb.PriorityId = int.Parse(ddPriority.SelectedValue);
			dimb.TypeId = int.Parse(ddType.SelectedValue);

			dimb.OrgUid = PrimaryKeyId.Empty;
			dimb.ContactUid = PrimaryKeyId.Empty;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName())
				dimb.OrgUid = ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName())
				dimb.ContactUid = ClientControl.ObjectId;

			dimb.SeverityId = int.Parse(ddSeverity.SelectedValue);
			ArrayList alGen = new ArrayList();
			foreach (ListItem liItem in lbGenCats.Items)
				if (liItem.Selected)
					alGen.Add(int.Parse(liItem.Value));
			ArrayList alIss = new ArrayList();
			foreach (ListItem liItem in lbIssCats.Items)
				if (liItem.Selected)
					alIss.Add(int.Parse(liItem.Value));
			dimb.GeneralCategories = alGen;
			dimb.IncidentCategories = alIss;
			EMailRouterPop3BoxSettings.Save(ib);
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "<script language=javascript>" +
					  "try {window.opener.location.href=window.opener.location.href;}" +
					  "catch (e){} window.close();</script>");
		}
		#endregion
	}
}
