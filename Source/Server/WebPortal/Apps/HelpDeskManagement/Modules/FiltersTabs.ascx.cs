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
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Globalization;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core;
using Mediachase.IBN.Business.EMail;
using Mediachase.Ibn.Clients;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules
{
	public partial class FiltersTabs : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentsList", Assembly.GetExecutingAssembly());
		protected UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		#region _className
		private string _className
		{
			get
			{
				if (Request["className"] != null)
					return Request["className"];
				return String.Empty;
			}
		} 
		#endregion

		#region _uid
		private string _uid
		{
			get
			{
				if (Request["uid"] != null)
					return Request["uid"];
				return String.Empty;
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				DefaultDataBind();
				if (String.IsNullOrEmpty(_uid))
					hfValue.Value = "Panel4";
				else
					hfValue.Value = "Panel3";
				if (!String.IsNullOrEmpty(_uid))
					BindSavedValues();
			}
			ApplyLocalization();

			if (Request["closeFramePopup"] != null)
				btnClose.OnClientClick = String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{}}", Request["closeFramePopup"]);
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnSave.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tSave}");
			btnClose.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tClose}");

			lgdFieldSet.AddText(GetGlobalResourceObject("IbnFramework.Incident", "SelectFieldSet").ToString());
			lgdGroupSet.AddText(GetGlobalResourceObject("IbnFramework.Incident", "SelectGroupingField").ToString());
			lgdFinish.AddText(GetGlobalResourceObject("IbnFramework.Incident", "SelectTitle").ToString());
			cbIsPublic.Text = " " + GetGlobalResourceObject("IbnFramework.Incident", "IsPublicView").ToString();
			rfTitle.ErrorMessage = GetGlobalResourceObject("IbnFramework.Incident", "TitleIsRequired").ToString();
		} 
		#endregion

		#region DefaultDataBind
		/// <summary>
		/// Default data bind.
		/// </summary>
		private void DefaultDataBind()
		{
			//FieldSet
			ddFieldSets.Items.Clear();
			ddFieldSets.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "StandardFieldSet").ToString(), Incident.IssueFieldSet.IncidentsDefault.ToString()));
			ddFieldSets.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "TimelineFieldSet").ToString(), Incident.IssueFieldSet.IncidentsLight.ToString()));
			ddFieldSets.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "TrackingFieldSet").ToString(), Incident.IssueFieldSet.IncidentsTracking.ToString()));

			//Grouping
			ddGroupField.Items.Clear();
			ddGroupField.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "NoGroupBy").ToString(), Incident.AvailableGroupField.NotSet.ToString()));
			ddGroupField.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "GroupByPrj").ToString(), Incident.AvailableGroupField.Project.ToString()));
			if (PortalConfig.GeneralAllowClientField)
				ddGroupField.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "GroupByContacts").ToString(), Incident.AvailableGroupField.Client.ToString()));
			ddGroupField.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "GroupByIssBoxes").ToString(), Incident.AvailableGroupField.IssueBox.ToString()));
			ddGroupField.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "GroupByResp").ToString(), Incident.AvailableGroupField.Responsible.ToString()));

			//Filters
			//Managers
			ddManager.DataSource = Incident.GetListIncidentManagers();
			ddManager.DataValueField = "UserId";
			ddManager.DataTextField = "UserName";
			ddManager.DataBind();

			ListItem lItem = new ListItem(LocRM.GetString("All"), "0");
			ddManager.Items.Insert(0, lItem);

			lItem = new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "CurrentUser").ToString(), "-5");
			ddManager.Items.Insert(1, lItem);

			//Priorities
			ddPriority.DataSource = Incident.GetListPriorities();
			ddPriority.DataTextField = "PriorityName";
			ddPriority.DataValueField = "PriorityId";
			ddPriority.DataBind();
			lItem = new ListItem(LocRM.GetString("All"), "-1");
			ddPriority.Items.Insert(0, lItem);

			//IssueBox
			ddIssBox.DataSource = IncidentBox.List();
			ddIssBox.DataTextField = "Name";
			ddIssBox.DataValueField = "IncidentBoxId";
			ddIssBox.DataBind();
			lItem = new ListItem(LocRM.GetString("All"), "0");
			ddIssBox.Items.Insert(0, lItem);

			//Types
			ddType.DataSource = Incident.GetListIncidentTypes();
			ddType.DataTextField = "TypeName";
			ddType.DataValueField = "TypeId";
			ddType.DataBind();

			lItem = new ListItem(LocRM.GetString("All"), "0");
			ddType.Items.Insert(0, lItem);

			//Creators
			ddCreatedBy.DataSource = Incident.GetListIncidentCreators();
			ddCreatedBy.DataTextField = "UserName";
			ddCreatedBy.DataValueField = "UserId";
			ddCreatedBy.DataBind();

			lItem = new ListItem(LocRM.GetString("All"), "0");
			ddCreatedBy.Items.Insert(0, lItem);

			lItem = new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "CurrentUser").ToString(), "-5");
			ddCreatedBy.Items.Insert(1, lItem);

			//Responsibles
			ddResponsible.DataSource = Incident.GetListIncidentResponsibles();
			ddResponsible.DataValueField = "UserId";
			ddResponsible.DataTextField = "UserName";
			ddResponsible.DataBind();

			lItem = new ListItem(LocRM.GetString("All"), "0");
			ddResponsible.Items.Insert(0, lItem);

			lItem = new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "OutOfPersonalResponsibilityFilter").ToString(), "-1");
			ddResponsible.Items.Insert(1, lItem);

			lItem = new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "ResponsibleNotAssigned").ToString(), "-2");
			ddResponsible.Items.Insert(2, lItem);

			lItem = new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "GroupResp").ToString(), "-3");
			ddResponsible.Items.Insert(3, lItem);

			lItem = new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "AllDenied").ToString(), "-4");
			ddResponsible.Items.Insert(4, lItem);

			lItem = new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "CurrentUser").ToString(), "-5");
			ddResponsible.Items.Insert(5, lItem);

			//Client
			ClientControl.ObjectType = String.Empty;
			ClientControl.ObjectId = PrimaryKeyId.Empty;

			//States
			ddState.Items.Clear();
			using (IDataReader reader = Incident.GetListIncidentStates())
			{
				while (reader.Read())
				{
					if ((int)reader["StateId"] != (int)ObjectStates.Overdue)
						ddState.Items.Add(new ListItem(reader["StateName"].ToString(), reader["StateId"].ToString()));
				}
			}

			lItem = new ListItem(LocRM.GetString("Inactive"), "-2");
			ddState.Items.Insert(0, lItem);
			lItem = new ListItem(LocRM.GetString("Active"), "-1");
			ddState.Items.Insert(0, lItem);
			lItem = new ListItem(LocRM.GetString("All"), "0");
			ddState.Items.Insert(0, lItem);

			//Severities
			ddSeverity.DataSource = Incident.GetListIncidentSeverity();
			ddSeverity.DataTextField = "SeverityName";
			ddSeverity.DataValueField = "SeverityId";
			ddSeverity.DataBind();
			lItem = new ListItem(LocRM.GetString("All"), "0");
			ddSeverity.Items.Insert(0, lItem);

			//Projects
			if (!Mediachase.IBN.Business.Configuration.ProjectManagementEnabled)
			{
				tdProject.Visible = false;
				_pc[Incident.ProjectFilterKey] = "0";
			}
			else
			{
				ddlProject.DataSource = Project.GetListProjects();
				ddlProject.DataTextField = "Title";
				ddlProject.DataValueField = "ProjectId";
				ddlProject.DataBind();
			}
			lItem = new ListItem(LocRM.GetString("All"), "0");
			ddlProject.Items.Insert(0, lItem);
			lItem = new ListItem(LocRM.GetString("NoneProject"), "-1");
			ddlProject.Items.Insert(1, lItem);
			ddlProject.DataSource = null;
			ddlProject.DataBind();

			//General Categories
			ddGenCatType.Items.Clear();
			ddGenCatType.Items.Add(new ListItem(LocRM.GetString("Any2"), "0"));
			ddGenCatType.Items.Add(new ListItem(LocRM.GetString("SelectedOnly"), "1"));
			ddGenCatType.Items.Add(new ListItem(LocRM.GetString("ExcludeSelected"), "2"));

			lbGenCats.Items.Clear();
			lbGenCats.DataSource = Project.GetListCategoriesAll();
			lbGenCats.DataTextField = "CategoryName";
			lbGenCats.DataValueField = "CategoryId";
			lbGenCats.DataBind();

			//Issue Categories
			ddIssCatType.Items.Clear();
			ddIssCatType.Items.Add(new ListItem(LocRM.GetString("Any2"), "0"));
			ddIssCatType.Items.Add(new ListItem(LocRM.GetString("SelectedOnly"), "1"));
			ddIssCatType.Items.Add(new ListItem(LocRM.GetString("ExcludeSelected"), "2"));

			lbIssCats.Items.Clear();
			lbIssCats.DataSource = Incident.GetListIncidentCategories();
			lbIssCats.DataTextField = "CategoryName";
			lbIssCats.DataValueField = "CategoryId";
			lbIssCats.DataBind();

			cbOnlyNewMess.Checked = false;

			cbIsPublic.Visible = Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.Administrator);

			trClient.Visible = PortalConfig.GeneralAllowClientField;
			trCategories.Visible = PortalConfig.GeneralAllowGeneralCategoriesField;
			tblPriority.Visible = PortalConfig.GeneralAllowPriorityField;
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			ListViewProfile profile = ListViewProfile.Load(_className, _uid, String.Empty);
			CHelper.SafeSelect(ddFieldSets, profile.FieldSetName);
			CHelper.SafeSelect(ddGroupField, String.IsNullOrEmpty(profile.GroupByFieldName) ? Incident.AvailableGroupField.NotSet.ToString() : profile.GroupByFieldName);
			txtTitle.Text = profile.Name;
			cbIsPublic.Checked = profile.IsPublic;

			#region Filters
			FilterElementCollection fec = profile.Filters;
			foreach (FilterElement fe in fec)
			{
				switch (fe.Source)
				{
					case Incident.ProjectFilterKey:
						CHelper.SafeSelect(ddlProject, fe.Value.ToString());
						break;
					case Incident.ManagerFilterKey:
						CHelper.SafeSelect(ddManager, fe.Value.ToString());
						break;
					case Incident.ResponsibleFilterKey:
						CHelper.SafeSelect(ddResponsible, fe.Value.ToString());
						break;
					case Incident.CreatorFilterKey:
						CHelper.SafeSelect(ddCreatedBy, fe.Value.ToString());
						break;
					case Incident.PriorityFilterKey:
						CHelper.SafeSelect(ddPriority, fe.Value.ToString());
						break;
					case Incident.IssueBoxFilterKey:
						CHelper.SafeSelect(ddIssBox, fe.Value.ToString());
						break;
					case Incident.TypeFilterKey:
						CHelper.SafeSelect(ddType, fe.Value.ToString());
						break;
					case Incident.ClientFilterKey:
						PrimaryKeyId orgUid = PrimaryKeyId.Empty;
						PrimaryKeyId contactUid = PrimaryKeyId.Empty;
						Incident.GetContactId(fe.Value.ToString(), out orgUid, out contactUid);
						if (orgUid != PrimaryKeyId.Empty)
						{
							try {
								Mediachase.Ibn.Core.Business.BusinessManager.Load(OrganizationEntity.GetAssignedMetaClassName(), orgUid);
								ClientControl.ObjectId = orgUid;
								ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
							}
							catch { }
						}
						else if (contactUid != PrimaryKeyId.Empty)
						{
							try
							{
								Mediachase.Ibn.Core.Business.BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), contactUid);
								ClientControl.ObjectId = contactUid;
								ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
							}
							catch { }
						}
						break;
					case Incident.StatusFilterKey:
						CHelper.SafeSelect(ddState, fe.Value.ToString());
						break;
					case Incident.SeverityFilterKey:
						CHelper.SafeSelect(ddSeverity, fe.Value.ToString());
						break;
					case Incident.GenCategoryTypeFilterKey:
						CHelper.SafeSelect(ddGenCatType, fe.Value.ToString());
						//todo - gen category filter view
						break;
					case Incident.GenCategoriesFilterKey:
						string[] mas = fe.Value.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in mas)
							CHelper.SafeMultipleSelect(lbGenCats, s);
						//todo - gen category filter view
						break;
					case Incident.IssueCategoryTypeFilterKey:
						CHelper.SafeSelect(ddIssCatType, fe.Value.ToString());
						//todo - gen category filter view
						break;
					case Incident.IssueCategoriesFilterKey:
						string[] mas1 = fe.Value.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in mas1)
							CHelper.SafeMultipleSelect(lbIssCats, s);
						//todo - gen category filter view
						break;
					case Incident.UnansweredFilterKey:
						cbOnlyNewMess.Checked = bool.Parse(fe.Value.ToString());
						break;
					case Incident.OverdueFilterKey:
						cbOnlyOverdue.Checked = bool.Parse(fe.Value.ToString());
						break;
					default:
						break;
				}
			}
			#endregion
		} 
		#endregion

		#region GetFilters
		private FilterElementCollection GetFilters()
		{
			FilterElementCollection fec = new FilterElementCollection();
			if (cbOnlyNewMess.Checked)
			{
				FilterElement fe = FilterElement.EqualElement(Incident.UnansweredFilterKey, cbOnlyNewMess.Checked.ToString());
				fec.Add(fe);
			}
			if (cbOnlyOverdue.Checked)
			{
				FilterElement fe = FilterElement.EqualElement(Incident.OverdueFilterKey, cbOnlyOverdue.Checked.ToString());
				fec.Add(fe);
			}
			if (ddManager.SelectedValue != "0")
			{
				if (ddManager.SelectedValue == "-5")	//CurrentUser
				{
					FilterElement fe = FilterElement.EqualElement(Incident.ManagerFilterKey, "{Security:CurrentUserId}");
					fe.ValueIsTemplate = true;
					fec.Add(fe);
				}
				else
				{
					FilterElement fe = FilterElement.EqualElement(Incident.ManagerFilterKey, ddManager.SelectedValue);
					fec.Add(fe);
				}
			}
			if (ddResponsible.SelectedValue != "0")
			{
				if (ddResponsible.SelectedValue == "-5")	//CurrentUser
				{
					FilterElement fe = FilterElement.EqualElement(Incident.ResponsibleFilterKey, "{Security:CurrentUserId}");
					fe.ValueIsTemplate = true;
					fec.Add(fe);
				}
				else
				{
					FilterElement fe = FilterElement.EqualElement(Incident.ResponsibleFilterKey, ddResponsible.SelectedValue);
					fec.Add(fe);
				}
			}
			if (ddPriority.SelectedValue != "-1")
			{
				FilterElement fe = FilterElement.EqualElement(Incident.PriorityFilterKey, ddPriority.SelectedValue);
				fec.Add(fe);
			}
			if (ddIssBox.SelectedValue != "0" && _pc[IncidentListNew.IssueListViewNameKey] != Incident.FieldSetName.GroupByBox.ToString())
			{
				FilterElement fe = FilterElement.EqualElement(Incident.IssueBoxFilterKey, ddIssBox.SelectedValue);
				fec.Add(fe);
			}
			if (ddType.SelectedValue != "0")
			{
				FilterElement fe = FilterElement.EqualElement(Incident.TypeFilterKey, ddType.SelectedValue);
				fec.Add(fe);
			}
			if (ddlProject.SelectedValue != "0" && _pc[IncidentListNew.IssueListViewNameKey] != Incident.FieldSetName.GroupByProject.ToString())
			{
				FilterElement fe = FilterElement.EqualElement(Incident.ProjectFilterKey, ddlProject.SelectedValue);
				fec.Add(fe);
			}
			if (ddCreatedBy.SelectedValue != "0")
			{
				if (ddCreatedBy.SelectedValue == "-5")	//CurrentUser
				{
					FilterElement fe = FilterElement.EqualElement(Incident.CreatorFilterKey, "{Security:CurrentUserId}");
					fe.ValueIsTemplate = true;
					fec.Add(fe);
				}
				else
				{
					FilterElement fe = FilterElement.EqualElement(Incident.CreatorFilterKey, ddCreatedBy.SelectedValue);
					fec.Add(fe);
				}
			}
			if (ddState.SelectedValue != "0")
			{
				FilterElement fe = FilterElement.EqualElement(Incident.StatusFilterKey, ddState.SelectedValue);
				fec.Add(fe);
			}
			if (ddSeverity.SelectedValue != "0")
			{
				FilterElement fe = FilterElement.EqualElement(Incident.SeverityFilterKey, ddSeverity.SelectedValue);
				fec.Add(fe);
			}

			//Client
			string client = "";
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				client = "org_" + ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				client = "contact_" + ClientControl.ObjectId;
			if (!String.IsNullOrEmpty(client) && _pc[IncidentListNew.IssueListViewNameKey] != Incident.FieldSetName.GroupByClient.ToString())
			{
				FilterElement fe = FilterElement.EqualElement(Incident.ClientFilterKey, client);
				fec.Add(fe);
			}

			// General Categories
			if (ddGenCatType.SelectedValue != "0")
			{
				FilterElement fe = FilterElement.EqualElement(Incident.GenCategoryTypeFilterKey, ddGenCatType.SelectedValue);
				fec.Add(fe);

				ArrayList alGenCats = new ArrayList();
				foreach (ListItem liItem in lbGenCats.Items)
				{
					if (liItem.Selected && !alGenCats.Contains(liItem.Value))
						alGenCats.Add(liItem.Value);
				}
				FilterElement fe1 = FilterElement.EqualElement(Incident.GenCategoriesFilterKey, String.Join(";", (string[])alGenCats.ToArray(typeof(string))));
				fec.Add(fe1);
			}


			// Issue Categories
			if (ddIssCatType.SelectedValue != "0")
			{
				FilterElement fe = FilterElement.EqualElement(Incident.IssueCategoryTypeFilterKey, ddIssCatType.SelectedValue);
				fec.Add(fe);

				ArrayList alIssCats = new ArrayList();
				foreach (ListItem liItem in lbIssCats.Items)
				{
					if (liItem.Selected && !alIssCats.Contains(liItem.Value))
						alIssCats.Add(liItem.Value);
				}
				FilterElement fe1 = FilterElement.EqualElement(Incident.IssueCategoriesFilterKey, String.Join(";", (string[])alIssCats.ToArray(typeof(string))));
				fec.Add(fe1);
			}
			return fec;
		}
		#endregion

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			if (ddIssCatType.SelectedValue == "0")
				lbIssCats.Style["Display"] = "none";
			else
				lbIssCats.Style["Display"] = "block";

			if (ddGenCatType.SelectedValue == "0")
				lbGenCats.Style["Display"] = "none";
			else
				lbGenCats.Style["Display"] = "block";

			ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("ChangeTab('{0}');", hfValue.Value), true);

			base.OnPreRender(e);
		} 
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, EventArgs e)
		{
			ListViewProfile profile;
			if (!String.IsNullOrEmpty(_uid))
				profile = ListViewProfile.Load(_className, _uid, String.Empty);
			else
				profile = new ListViewProfile();
			profile.FieldSetName = ddFieldSets.SelectedValue;
			profile.GroupByFieldName = (ddGroupField.SelectedValue == Incident.AvailableGroupField.NotSet.ToString()) ? "" : ddGroupField.SelectedValue;
			profile.Filters = GetFilters();
			string uid = (!String.IsNullOrEmpty(_uid)) ? _uid : Guid.NewGuid().ToString();
			profile.Id = uid;
			profile.IsPublic = cbIsPublic.Checked;
			profile.IsSystem = false;
			profile.Name = txtTitle.Text;
			ListViewProfile.SaveCustomProfile(_className, String.Empty, Mediachase.IBN.Business.Security.CurrentUser.UserID, profile);

			CommandParameters cp = new CommandParameters("MC_HDM_NewViewCreated");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("ViewUid", uid);
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		} 
		#endregion
	}
}