using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Clients;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.Modules
{
	public partial class Profiles : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", Assembly.GetExecutingAssembly());
		private UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		
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

			chkOnlyForUser.Text = " " + LocRM.GetString("OnlyForUser");
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
			ddFieldSets.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "StandardFieldSet").ToString(), Project.ProjectFieldSet.ProjectsDefault.ToString()));
			ddFieldSets.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "TimelineFieldSet").ToString(), Project.ProjectFieldSet.ProjectsLight.ToString()));

			//Grouping
			ddGroupField.Items.Clear();
			ddGroupField.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "NoGroupBy").ToString(), Project.AvailableGroupField.NotSet.ToString()));
			ddGroupField.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Project", "GroupByPortfolio").ToString(), Project.AvailableGroupField.Portfolio.ToString()));
			if(PortalConfig.GeneralAllowClientField)
				ddGroupField.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Project", "GroupByContacts").ToString(), Project.AvailableGroupField.Client.ToString()));

			//Filters
			
			//StartDate
			dtcStartDate.SelectedDate = DateTime.Today;
			ddSDType.Items.Clear();
			ddSDType.Items.Add(new ListItem(LocRM.GetString("Any"), "0"));
			ddSDType.Items.Add(new ListItem(LocRM.GetString("GE"), "1"));
			ddSDType.Items.Add(new ListItem(LocRM.GetString("LE"), "2"));
			ddSDType.Items.Add(new ListItem(LocRM.GetString("Equal"), "3"));

			//EndDate
			dtcEndDate.SelectedDate = DateTime.Now;
			ddFDType.Items.Clear();
			ddFDType.Items.Add(new ListItem(LocRM.GetString("Any"), "0"));
			ddFDType.Items.Add(new ListItem(LocRM.GetString("GE"), "1"));
			ddFDType.Items.Add(new ListItem(LocRM.GetString("LE"), "2"));
			ddFDType.Items.Add(new ListItem(LocRM.GetString("Equal"), "3"));

			//Status
			ddStatus.Items.Clear();
			ddStatus.DataSource = Project.GetListProjectStatus();
			ddStatus.DataTextField = "StatusName";
			ddStatus.DataValueField = "StatusId";
			ddStatus.Items.Insert(0, new ListItem(LocRM.GetString("All"), "0"));
			ddStatus.Items.Insert(1, new ListItem(LocRM.GetString("tOnlyActive"), "-1"));
			ddStatus.Items.Insert(2, new ListItem(LocRM.GetString("tOnlyInactive"), "-2"));
			
			// Phase
			ddPrjPhases.Items.Clear();
			ddPrjPhases.Items.Add(new ListItem(LocRM.GetString("All"), "0"));
			using (IDataReader rdr = Project.GetListProjectPhases())
			{
				while (rdr.Read())
				{
					ddPrjPhases.Items.Add(new ListItem(rdr["PhaseName"].ToString(), rdr["PhaseId"].ToString()));
				}
			}

			// Type
			ddType.Items.Clear();
			ddType.Items.Add(new ListItem(LocRM.GetString("All"), "0"));
			using (IDataReader rdr = Project.GetListProjectTypes())
			{
				while (rdr.Read())
				{
					ddType.Items.Add(new ListItem((string)rdr["TypeName"], rdr["TypeId"].ToString()));
				}
			}

			// Mananger
			ddManager.Items.Clear();
			ddManager.DataSource = Project.GetListProjectManagers();
			ddManager.DataTextField = "UserName";
			ddManager.DataValueField = "ManagerId";
			ddManager.DataBind();
			ddManager.Items.Insert(0, new ListItem(LocRM.GetString("All"), "0"));
			ListItem lItem = new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "CurrentUser").ToString(), "-5");
			ddManager.Items.Insert(1, lItem);

			// ExeMananger
			ddExeManager.Items.Clear();
			ddExeManager.DataSource = Project.GetListExecutiveManagers();
			ddExeManager.DataTextField = "UserName";
			ddExeManager.DataValueField = "ExecutiveManagerId";
			ddExeManager.DataBind();
			ddExeManager.Items.Insert(0, new ListItem(LocRM.GetString("All"), "0"));
			lItem = new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "CurrentUser").ToString(), "-5");
			ddExeManager.Items.Insert(1, lItem);

			// Priority
			ddPriority.Items.Clear();
			ddPriority.Items.Add(new ListItem(LocRM.GetString("All"), "-1"));
			using (IDataReader rdr = Project.GetListPriorities())
			{
				while (rdr.Read())
				{
					ddPriority.Items.Add(new ListItem((string)rdr["PriorityName"], rdr["PriorityId"].ToString()));
				}
			}

			//Client
			ClientControl.ObjectType = String.Empty;
			ClientControl.ObjectId = PrimaryKeyId.Empty;

			// Project Groups
			ddPrjGrpType.Items.Clear();
			ddPrjGrpType.Items.Add(new ListItem(LocRM.GetString("Any2"), "0"));
			ddPrjGrpType.Items.Add(new ListItem(LocRM.GetString("SelectedOnly"), "1"));
			ddPrjGrpType.Items.Add(new ListItem(LocRM.GetString("ExcludeSelected"), "2"));

			lbPrjGrps.Items.Clear();
			lbPrjGrps.DataSource = ProjectGroup.GetProjectGroups();
			lbPrjGrps.DataTextField = "Title";
			lbPrjGrps.DataValueField = "ProjectGroupId";
			lbPrjGrps.DataBind();

			// General Categories
			ddGenCatType.Items.Clear();
			ddGenCatType.Items.Add(new ListItem(LocRM.GetString("Any2"), "0"));
			ddGenCatType.Items.Add(new ListItem(LocRM.GetString("SelectedOnly"), "1"));
			ddGenCatType.Items.Add(new ListItem(LocRM.GetString("ExcludeSelected"), "2"));

			lbGenCats.Items.Clear();
			lbGenCats.DataSource = Project.GetListCategoriesAll();
			lbGenCats.DataTextField = "CategoryName";
			lbGenCats.DataValueField = "CategoryId";
			lbGenCats.DataBind();

			// Project Categories
			ddPrjCatType.Items.Clear();
			ddPrjCatType.Items.Add(new ListItem(LocRM.GetString("Any2"), "0"));
			ddPrjCatType.Items.Add(new ListItem(LocRM.GetString("SelectedOnly"), "1"));
			ddPrjCatType.Items.Add(new ListItem(LocRM.GetString("ExcludeSelected"), "2"));

			lbPrjCats.Items.Clear();
			lbPrjCats.DataSource = Project.GetListProjectCategories();
			lbPrjCats.DataTextField = "CategoryName";
			lbPrjCats.DataValueField = "CategoryId";
			lbPrjCats.DataBind();

			if (Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.Partner))
				trPrGroups.Visible = false;

			cbIsPublic.Visible = Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.Administrator);

			trGenCats.Visible = PortalConfig.GeneralAllowGeneralCategoriesField;
			trClient.Visible = PortalConfig.GeneralAllowClientField;
			trPriority.Visible = PortalConfig.GeneralAllowPriorityField;
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			ListViewProfile profile = ListViewProfile.Load(_className, _uid, String.Empty);
			CHelper.SafeSelect(ddFieldSets, profile.FieldSetName);
			CHelper.SafeSelect(ddGroupField, String.IsNullOrEmpty(profile.GroupByFieldName) ? Project.AvailableGroupField.NotSet.ToString() : profile.GroupByFieldName);
			txtTitle.Text = profile.Name;
			cbIsPublic.Checked = profile.IsPublic;

			#region Filters
			FilterElementCollection fec = profile.Filters;
			foreach (FilterElement fe in fec)
			{
				switch (fe.Source)
				{
					case Project.StatusFilterKey:
						CHelper.SafeSelect(ddStatus, fe.Value.ToString());
						break;
					case Project.PhaseFilterKey:
						CHelper.SafeSelect(ddPrjPhases, fe.Value.ToString());
						break;
					case Project.TypeFilterKey:
						CHelper.SafeSelect(ddType, fe.Value.ToString());
						break;
					case Project.PriorityFilterKey:
						CHelper.SafeSelect(ddPriority, fe.Value.ToString());
						break;
					case Project.StartDateFilterKey:
						switch (fe.Type)
						{
							case FilterElementType.GreaterOrEqual:
								CHelper.SafeSelect(ddSDType, "1");
								dtcStartDate.SelectedDate = DateTime.Parse(fe.Value.ToString(), CultureInfo.InvariantCulture);
								break;
							case FilterElementType.LessOrEqual:
								CHelper.SafeSelect(ddSDType, "2");
								dtcStartDate.SelectedDate = DateTime.Parse(fe.Value.ToString(), CultureInfo.InvariantCulture);
								break;
							case FilterElementType.Equal:
								CHelper.SafeSelect(ddSDType, "3");
								dtcStartDate.SelectedDate = DateTime.Parse(fe.Value.ToString(), CultureInfo.InvariantCulture);
								break;
							default:
								CHelper.SafeSelect(ddSDType, "0");
								break;
						}
						break;
					case Project.EndDateFilterKey:
						switch (fe.Type)
						{
							case FilterElementType.GreaterOrEqual:
								CHelper.SafeSelect(ddFDType, "1");
								dtcEndDate.SelectedDate = DateTime.Parse(fe.Value.ToString(), CultureInfo.InvariantCulture);
								break;
							case FilterElementType.LessOrEqual:
								CHelper.SafeSelect(ddFDType, "2");
								dtcEndDate.SelectedDate = DateTime.Parse(fe.Value.ToString(), CultureInfo.InvariantCulture);
								break;
							case FilterElementType.Equal:
								CHelper.SafeSelect(ddFDType, "3");
								dtcEndDate.SelectedDate = DateTime.Parse(fe.Value.ToString(), CultureInfo.InvariantCulture);
								break;
							default:
								break;
						}
						break;
					case Project.ManagerFilterKey:
						CHelper.SafeSelect(ddManager, fe.Value.ToString());
						break;
					case Project.ExecManagerFilterKey:
						CHelper.SafeSelect(ddExeManager, fe.Value.ToString());
						break;
					case Project.ClientFilterKey:
						PrimaryKeyId orgUid = PrimaryKeyId.Empty;
						PrimaryKeyId contactUid = PrimaryKeyId.Empty;
						Incident.GetContactId(fe.Value.ToString(), out orgUid, out contactUid);
						if (orgUid != PrimaryKeyId.Empty)
						{
							ClientControl.ObjectId = orgUid;
							ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
						}
						else if (contactUid != PrimaryKeyId.Empty)
						{
							ClientControl.ObjectId = contactUid;
							ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
						}
						break;
					case Project.GenCategoryTypeFilterKey:
						CHelper.SafeSelect(ddGenCatType, fe.Value.ToString());
						break;
					case Project.GenCategoriesFilterKey:
						string[] mas = fe.Value.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in mas)
							CHelper.SafeMultipleSelect(lbGenCats, s);
						break;
					case Project.ProjectCategoryTypeFilterKey:
						CHelper.SafeSelect(ddPrjCatType, fe.Value.ToString());
						break;
					case Project.ProjectCategoriesFilterKey:
						string[] mas1 = fe.Value.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in mas1)
							CHelper.SafeMultipleSelect(lbPrjCats, s);
						break;
					case Project.PortfolioTypeFilterKey:
						CHelper.SafeSelect(ddPrjGrpType, fe.Value.ToString());
						break;
					case Project.PortfoliosFilterKey:
						string[] mas2 = fe.Value.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in mas2)
							CHelper.SafeMultipleSelect(lbPrjGrps, s);
						break;
					case Project.MyParticipationOnlyFilterKey:
						chkOnlyForUser.Checked = bool.Parse(fe.Value.ToString());
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
			if (ddSDType.SelectedValue != "0")
			{
				FilterElement fe = null;
				switch (ddSDType.SelectedValue)
				{
					case "1":
						fe = FilterElement.EqualElement(Project.StartDateFilterKey, dtcStartDate.SelectedDate.ToString(CultureInfo.InvariantCulture));
						fe.Type = FilterElementType.GreaterOrEqual;
						break;
					case "2":
						fe = FilterElement.EqualElement(Project.StartDateFilterKey, dtcStartDate.SelectedDate.ToString(CultureInfo.InvariantCulture));
						fe.Type = FilterElementType.LessOrEqual;
						break;
					case "3":
						fe = FilterElement.EqualElement(Project.StartDateFilterKey, dtcStartDate.SelectedDate.ToString(CultureInfo.InvariantCulture));
						fe.Type = FilterElementType.Equal;
						break;
					default:
						break;
				}
				if (fe != null)
					fec.Add(fe);
			}

			if (ddFDType.SelectedValue != "0")
			{
				FilterElement fe = null;
				switch (ddFDType.SelectedValue)
				{
					case "1":
						fe = FilterElement.EqualElement(Project.EndDateFilterKey, dtcEndDate.SelectedDate.ToString(CultureInfo.InvariantCulture));
						fe.Type = FilterElementType.GreaterOrEqual;
						break;
					case "2":
						fe = FilterElement.EqualElement(Project.EndDateFilterKey, dtcEndDate.SelectedDate.ToString(CultureInfo.InvariantCulture));
						fe.Type = FilterElementType.LessOrEqual;
						break;
					case "3":
						fe = FilterElement.EqualElement(Project.EndDateFilterKey, dtcEndDate.SelectedDate.ToString(CultureInfo.InvariantCulture));
						fe.Type = FilterElementType.Equal;
						break;
					default:
						break;
				}
				if (fe != null)
					fec.Add(fe);
			}
			if (ddStatus.SelectedValue != "0")
			{
				FilterElement fe = FilterElement.EqualElement(Project.StatusFilterKey, ddStatus.SelectedValue);
				fec.Add(fe);
			}
			if (ddPrjPhases.SelectedValue != "0")
			{
				FilterElement fe = FilterElement.EqualElement(Project.PhaseFilterKey, ddPrjPhases.SelectedValue);
				fec.Add(fe);
			}
			if (ddType.SelectedValue != "0")
			{
				FilterElement fe = FilterElement.EqualElement(Project.TypeFilterKey, ddType.SelectedValue);
				fec.Add(fe);
			}
			if (ddManager.SelectedValue != "0")
			{
				if (ddManager.SelectedValue == "-5")	//CurrentUser
				{
					FilterElement fe = FilterElement.EqualElement(Project.ManagerFilterKey, "{Security:CurrentUserId}");
					fe.ValueIsTemplate = true;
					fec.Add(fe);
				}
				else
				{
					FilterElement fe = FilterElement.EqualElement(Project.ManagerFilterKey, ddManager.SelectedValue);
					fec.Add(fe);
				}
			}
			if (ddExeManager.SelectedValue != "0")
			{
				if (ddExeManager.SelectedValue == "-5")	//CurrentUser
				{
					FilterElement fe = FilterElement.EqualElement(Project.ExecManagerFilterKey, "{Security:CurrentUserId}");
					fe.ValueIsTemplate = true;
					fec.Add(fe);
				}
				else
				{
					FilterElement fe = FilterElement.EqualElement(Project.ExecManagerFilterKey, ddExeManager.SelectedValue);
					fec.Add(fe);
				}
			}
			if (ddPriority.SelectedValue != "-1")
			{
				FilterElement fe = FilterElement.EqualElement(Project.PriorityFilterKey, ddPriority.SelectedValue);
				fec.Add(fe);
			}
			// Portfolios
			if (ddPrjGrpType.SelectedValue != "0")
			{
				FilterElement fe = FilterElement.EqualElement(Project.PortfolioTypeFilterKey, ddPrjGrpType.SelectedValue);
				fec.Add(fe);

				ArrayList alPrjGrps = new ArrayList();
				foreach (ListItem liItem in lbPrjGrps.Items)
				{
					if (liItem.Selected && !alPrjGrps.Contains(liItem.Value))
						alPrjGrps.Add(liItem.Value);
				}
				FilterElement fe1 = FilterElement.EqualElement(Project.PortfoliosFilterKey, String.Join(";", (string[])alPrjGrps.ToArray(typeof(string))));
				fec.Add(fe1);
			}
			// General Categories
			if (ddGenCatType.SelectedValue != "0")
			{
				FilterElement fe = FilterElement.EqualElement(Project.GenCategoryTypeFilterKey, ddGenCatType.SelectedValue);
				fec.Add(fe);

				ArrayList alGenCats = new ArrayList();
				foreach (ListItem liItem in lbGenCats.Items)
				{
					if (liItem.Selected && !alGenCats.Contains(liItem.Value))
						alGenCats.Add(liItem.Value);
				}
				FilterElement fe1 = FilterElement.EqualElement(Project.GenCategoriesFilterKey, String.Join(";", (string[])alGenCats.ToArray(typeof(string))));
				fec.Add(fe1);
			}
			// Project Categories
			if (ddPrjCatType.SelectedValue != "0")
			{
				FilterElement fe = FilterElement.EqualElement(Project.ProjectCategoryTypeFilterKey, ddPrjCatType.SelectedValue);
				fec.Add(fe);

				ArrayList alPrjCats = new ArrayList();
				foreach (ListItem liItem in lbPrjCats.Items)
				{
					if (liItem.Selected && !alPrjCats.Contains(liItem.Value))
						alPrjCats.Add(liItem.Value);
				}
				FilterElement fe1 = FilterElement.EqualElement(Project.ProjectCategoriesFilterKey, String.Join(";", (string[])alPrjCats.ToArray(typeof(string))));
				fec.Add(fe1);
			}
			if (chkOnlyForUser.Checked)
			{
				FilterElement fe = FilterElement.EqualElement(Project.MyParticipationOnlyFilterKey, chkOnlyForUser.Checked.ToString());
				fec.Add(fe);
			}
			//Client
			string client = "";
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				client = "org_" + ClientControl.ObjectId;
			else if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				client = "contact_" + ClientControl.ObjectId;
			if (!String.IsNullOrEmpty(client))
			{
				FilterElement fe = FilterElement.EqualElement(Project.ClientFilterKey, client);
				fec.Add(fe);
			}

			return fec;
		}
		#endregion

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			if (ddPrjCatType.SelectedValue == "0")
				lbPrjCats.Style["Display"] = "none";
			else
				lbPrjCats.Style["Display"] = "block";

			if (ddGenCatType.SelectedValue == "0")
				lbGenCats.Style["Display"] = "none";
			else
				lbGenCats.Style["Display"] = "block";

			if (ddPrjGrpType.SelectedValue == "0")
				lbPrjGrps.Style["Display"] = "none";
			else
				lbPrjGrps.Style["Display"] = "block";

			if (ddSDType.SelectedItem.Value != "0")
			{
				tdStartDate.Style.Add("display", "block");
				dtcStartDate.DateIsRequired = true;
			}
			else
			{
				tdStartDate.Style.Add("display", "none");
				dtcStartDate.DateIsRequired = false;
			}
			if (ddFDType.SelectedItem.Value != "0")
			{
				tdFinishDate.Style.Add("display", "block");
				dtcEndDate.DateIsRequired = true;
			}
			else
			{
				tdFinishDate.Style.Add("display", "none");
				dtcEndDate.DateIsRequired = false;
			}

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
			profile.GroupByFieldName = (ddGroupField.SelectedValue == Project.AvailableGroupField.NotSet.ToString()) ? "" : ddGroupField.SelectedValue;
			profile.Filters = GetFilters();
			string uid = (!String.IsNullOrEmpty(_uid)) ? _uid : Guid.NewGuid().ToString();
			profile.Id = uid;
			profile.IsPublic = cbIsPublic.Checked;
			profile.IsSystem = false;
			profile.Name = txtTitle.Text;
			ListViewProfile.SaveCustomProfile(_className, String.Empty, Mediachase.IBN.Business.Security.CurrentUser.UserID, profile);

			CommandParameters cp = new CommandParameters("MC_PM_NewViewCreated");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("ViewUid", uid);
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion
	}
}