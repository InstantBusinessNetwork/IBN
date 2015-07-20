namespace Mediachase.UI.Web.Events.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Collections;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.UI.WebControls;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Clients;


	/// <summary>
	///		Summary description for EventEdit.
	/// </summary>
	public partial class EventEdit : System.Web.UI.UserControl
	{

		#region HTML Vars


		protected System.Web.UI.WebControls.TextBox txtStartDate;
		protected System.Web.UI.WebControls.RangeValidator Rangevalidator1;
		protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator1;
		protected System.Web.UI.WebControls.RangeValidator txtTargetEndDateRangeValidator;
		protected System.Web.UI.WebControls.RequiredFieldValidator txtTargetEndDateRFValidator;
		protected System.Web.UI.WebControls.TextBox txtEndDate;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lgdBasicInfo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lgdCategoryInfo;

		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strEventEdit", typeof(EventEdit).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strTaskEdit", typeof(EventEdit).Assembly);
		private int EventID = 0;
		private int ProjID = 0;
		private bool AutoInvite = false;

		#region UserID
		private int UserID
		{
			get
			{
				if (SharedID > 0)
					return SharedID;
				else return Security.CurrentUser.UserID;
			}
		}
		#endregion

		#region SharedID
		private int SharedID
		{
			get
			{
				try
				{
					if (Request["SharedID"] != null)
						return int.Parse(Request["SharedID"]);
					else return -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region Delete JS func
		//        function ChangeModify()
		//    {
		//        startHour = document.forms[0].<%= dtcStartDate.Hour.ClientID %>;
		//        startMinute = document.forms[0].<%= dtcStartDate.Minute.ClientID %>;
		//        startDate = document.forms[0].<%= dtcStartDate.Date.ClientID %>;
		//        endHour = document.forms[0].<%= dtcEndDate.Hour.ClientID %>;
		//        endMinute = document.forms[0].<%= dtcEndDate.Minute.ClientID %>;

		//        var hr1 = startHour.selectedIndex;
		//        mn1 = parseInt(startMinute.value, 10);
		//        tt=hr1*60+mn1;//start in minutes from midnight
		//        t1=tt+30;		//need in same end
		//        hr2=Math.round(t1/60); //hour of enddate
		//        mn2=t1-hr2*60;		//minutes of enddate
		//        if(mn2<0)			//round failed
		//        {
		//            hr2=hr2-1;
		//            mn2=mn2+60;
		//        }
		//        if(hr2==24) // need day adding
		//        {
		///*
		//            //endMinute.value=mn2;
		//            d=new Date();
		//            if(endDate.value.indexOf("/"))
		//            {
		//                tmp=endDate.value;
		//                sMonth=tmp.substring(0,tmp.indexOf("/"));
		//                tmp=tmp.substring(tmp.indexOf("/")+1);
		//                sDay=tmp.substring(0,tmp.indexOf("/"));
		//                sYear=tmp.substring(tmp.indexOf("/")+1);
		//                d=new Date(sYear,sMonth,sDay);
		//                d.setDate(d.getDate()+1);
		//                //endHour.value="12 AM";
		//                //endDate.value= d.getMonth() + "/"+ d.getDate() + "/"+ d.getYear();
		//            }
		//            if(endDate.value.indexOf("."))
		//            {
		//                tmp=startDate.value;
		//                sDay=tmp.substring(0,tmp.indexOf("."));
		//                tmp=tmp.substring(tmp.indexOf(".")+1);
		//                sMonth=tmp.substring(0,tmp.indexOf("."));
		//                sYear=tmp.substring(tmp.indexOf(".")+1);
		//                d=new Date(sYear,sMonth,sDay);
		//                d.setDate(d.getDate()+1);
		//                _date=d.getDate();
		//                _month=d.getMonth();
		//                if(_date<10) _date="0"+_date;
		//                if(_month<10) _month="0"+_month;
		//                //endHour.value="00";
		//                //endDate.value= _date + "."+ _month + "."+ d.getYear();
		//            }
		//*/
		//            endHour.selectedIndex=endHour.length-1;
		//            endMinute.selectedIndex=endMinute.length-1;
		//        }
		//        else //only time change
		//        {
		//            endMinute.value=mn2;
		//            endHour.selectedIndex=hr2;
		//        }
		//    }
		#endregion

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{

			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			if (SharedID > 0)
			{
				apShared.Visible = true;
				lblEntryOwner.Text = CommonHelper.GetUserStatus(SharedID);
			}
			else
				apShared.Visible = false;

			if (Request["EventID"] != null)
			{
				EventID = int.Parse(Request["EventID"]);
				btnSaveAssign.Visible = false;
			}
			if (Request["ProjectID"] != null)
			{
				ProjID = int.Parse(Request["ProjectID"]);
			}
			btnAddGeneralCategory.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.Categories + ")");
			btnAddGeneralCategory.Attributes.Add("title", LocRM.GetString("EditDictionary"));
			btnAddGeneralCategory.Visible = Security.IsManager();
			if (Request["autoinvite"] != null && Request["autoinvite"].ToString() == "1")
				AutoInvite = true;

			ApplyLocalization();

			vFile.Visible = false;

			if (!Page.IsPostBack)
			{
				FillManagers();
				BindValues();
				BindVisibility();
			}

			if (EventID > 0)
				trFileLoader.Visible = false;

			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			if (EventID == 0)
			{
				btnSave.Attributes.Add("onclick", "DisableButtons(this);ShowProgress();");
				btnSaveAssign.Attributes.Add("onclick", "DisableButtons(this);ShowProgress();");
				cbOneMore.Visible = true;
			}
			else
				cbOneMore.Visible = false;
			if (Request["OldEventID"] != null && Request["Assign"] != null)
			{
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_PM_EventOldParticipants");
				string cmd = cm.AddCommand("Event", "", "EventView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				ClientScript.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
					 "function OpenAssignWizard(){" + cmd + "} setTimeout('OpenAssignWizard()', 400);", true);
			}

			if (Request["Checked"] != null)
				cbOneMore.Checked = true;
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();
			if (Page.IsPostBack)
			{
				ListBox lbTmp = lbCategory;
				ArrayList alCategories = new ArrayList();
				for (int i = 0; i < lbTmp.Items.Count; i++)
					if (lbTmp.Items[i].Selected)
						alCategories.Add(int.Parse(lbTmp.Items[i].Value));
				BindCategories();
				for (int i = 0; i < alCategories.Count; i++)
				{
					ListItem liCategory = lbTmp.Items.FindByValue(alCategories[i].ToString());
					if (liCategory != null)
						liCategory.Selected = true;
				}
			}
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			trFileLoader.Visible = (EventID <= 0) && PortalConfig.CEntryAllowEditAttachmentField;
			trPriority.Visible = PortalConfig.CommonCEntryAllowEditPriorityField;
			trClient.Visible = PortalConfig.CommonCEntryAllowEditClientField;
			trCategories.Visible = PortalConfig.CommonCEntryAllowEditGeneralCategoriesField;
		} 
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblTitleTitle.Text = LocRM.GetString("Title");
			lblProjectTitle.Text = LocRM.GetString("Project");
			lblLocationTitle.Text = LocRM.GetString("Location");
			lblDescriptionTitle.Text = LocRM.GetString("Description");
			lblStartDateTitle.Text = LocRM.GetString("StartDate");
			lblEndDateTitle.Text = LocRM.GetString("EndDate");
			lblPriorityTitle.Text = LocRM.GetString("Priority");
			lblTypeTitle.Text = LocRM.GetString("Type");
			lblCategoriesTitle.Text = LocRM.GetString("Categories");
			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");
			lblFileLoad.Text = LocRM.GetString("FileLoad");
			btnSaveAssign.Text = LocRM.GetString("SaveAssign");
			cbOneMore.Text = LocRM.GetString("tAnotherOne");
			lblClient.Text = LocRM2.GetString("tClient");
			ManagerLabel.Text = LocRM.GetString("Manager");
			btnSaveAssign.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (EventID != 0)
				tbSave.Title = LocRM.GetString("tbsave_edit");
			else
				tbSave.Title = LocRM.GetString("tbsave_add");

			tbSave.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tbsave_gotolist"), "../Calendar/default.aspx");

			hdrBasicInfo.AddText(LocRM.GetString("tBasicInfo"));
			hdrCategoryInfo.AddText(LocRM.GetString("tCategoryInfo"));
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			DateTime dt = DateTime.Today.AddHours(DateTime.UtcNow.Hour + 1);
			dtcStartDate.SelectedDate = User.GetLocalDate(Security.CurrentUser.TimeZoneId, dt);
			if (Request["start"] != null)
			{
				try
				{
					dtcStartDate.SelectedDate = DateTime.Parse(Request["start"]);
				}
				catch { }
			}

			dtcEndDate.SelectedDate = dtcStartDate.SelectedDate.AddHours(1);
			if (Request["end"] != null)
			{
				try
				{
					dtcEndDate.SelectedDate = DateTime.Parse(Request["end"]);
				}
				catch { }
			}

			if (Request["end"] == null && Request["start"] != null && dtcStartDate.SelectedDate.Hour == 0 && dtcEndDate.SelectedDate.Minute == 0)
			{
				dtcStartDate.SelectedDate = dtcStartDate.SelectedDate.AddHours(8);
				dtcEndDate.SelectedDate = dtcStartDate.SelectedDate.AddHours(1);
			}

			if (AutoInvite)
				btnSaveAssign.Visible = false;

			BindCategories();
			ddlPriority.DataSource = CalendarEntry.GetListPriorities();
			ddlPriority.DataTextField = "PriorityName";
			ddlPriority.DataValueField = "PriorityId";
			ddlPriority.DataBind();
			
			ddlType.DataSource = CalendarEntry.GetListEventTypes();
			ddlType.DataTextField = "TypeName";
			ddlType.DataValueField = "TypeId";
			ddlType.DataBind();
			CommonHelper.SafeSelect(ddlType, ((int)CalendarEntry.EventType.Meeting).ToString());
			
			if (Configuration.ProjectManagementEnabled)
			{
				ucProject.ObjectTypeId = (int)ObjectTypes.Project;
				ucProject.ObjectId = -1;
				if (ProjID != 0)
					ucProject.ObjectId = ProjID;
			}
			else
				trProject.Visible = false;

			if (EventID != 0)
			{
				using (IDataReader reader = CalendarEntry.GetEvent(EventID))
				{
					if (reader.Read())
					{
						DateTime StartDate = (DateTime)reader["StartDate"];
						DateTime EndDate = (DateTime)reader["FinishDate"];
						dtcStartDate.SelectedDate = StartDate;
						dtcEndDate.SelectedDate = EndDate;

						txtTitle.Text = HttpUtility.HtmlDecode(reader["Title"].ToString());
						txtLocation.Text = HttpUtility.HtmlDecode(reader["Location"].ToString());
						if (reader["Description"] != DBNull.Value)
							txtDescription.Text = HttpUtility.HtmlDecode(reader["Description"].ToString());
						if (reader["ProjectId"] != DBNull.Value)
							ucProject.ObjectId = (int)reader["ProjectId"];

						if (reader["PriorityId"] != DBNull.Value)
						{
							CommonHelper.SafeSelect(ddlPriority, reader["PriorityId"].ToString());
							//ListItem lItem = ddlPriority.Items.FindByValue(reader["PriorityId"].ToString());
							//if (lItem != null)
							//{
							//    ddlPriority.ClearSelection();
							//    lItem.Selected = true;
							//}
						}
						if (reader["TypeId"] != DBNull.Value)
						{
							CommonHelper.SafeSelect(ddlType, reader["TypeId"].ToString());
							//ListItem lItem = ddlType.Items.FindByValue(reader["TypeId"].ToString());
							//if (lItem != null)
							//{
							//    ddlType.ClearSelection();
							//    lItem.Selected = true;
							//}
						}
						if ((int)reader["HasRecurrence"] == 1)
						{
							dtcStartDate.Enabled = false;
							dtcEndDate.Enabled = false;
						}

						if (reader["OrgUid"] != DBNull.Value)
						{
							ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
							ClientControl.ObjectId = PrimaryKeyId.Parse(reader["OrgUid"].ToString());
						}
						else if (reader["ContactUid"] != DBNull.Value)
						{
							ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
							ClientControl.ObjectId = PrimaryKeyId.Parse(reader["ContactUid"].ToString());
						}

						CommonHelper.SafeSelect(ManagerList, reader["ManagerId"].ToString());
					}
				}

				using (IDataReader reader = CalendarEntry.GetListCategories(EventID))
				{
					while (reader.Read())
					{
						for (int i = 0; i < lbCategory.Items.Count; i++)
						{
							CommonHelper.SafeMultipleSelect(lbCategory, reader["CategoryId"].ToString());
							//ListItem lItem = lbCategory.Items.FindByText(reader["CategoryName"].ToString());
							//if (lItem != null) lItem.Selected = true;
						}
					}
				}

				EditControl.ObjectId = EventID;
				EditControl.BindData();
			}
			else  //CREATE
			{
				CommonHelper.SafeSelect(ddlPriority, PortalConfig.CEntryDefaultValuePriorityField);
				ArrayList list = Common.StringToArrayList(PortalConfig.CEntryDefaultValueGeneralCategoriesField);
				foreach (int i in list)
					CommonHelper.SafeMultipleSelect(lbCategory, i.ToString());
				PrimaryKeyId org_id = PrimaryKeyId.Empty;
				PrimaryKeyId contact_id = PrimaryKeyId.Empty;
				Common.GetDefaultClient(PortalConfig.CEntryDefaultValueClientField, out contact_id, out org_id);
				if (contact_id != PrimaryKeyId.Empty)
				{
					ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
					ClientControl.ObjectId = contact_id;
				}
				else if (org_id != PrimaryKeyId.Empty)
				{
					ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
					ClientControl.ObjectId = org_id;
				}

				CommonHelper.SafeSelect(ManagerList, Security.CurrentUser.UserID.ToString());
			}
		}
		#endregion

		#region BindCategories
		private void BindCategories()
		{
			lbCategory.DataSource = CalendarEntry.GetListCategoriesAll();
			lbCategory.DataTextField = "CategoryName";
			lbCategory.DataValueField = "CategoryId";
			lbCategory.DataBind();
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.CustomValidator1.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CustomValidator1_ServerValidate);
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			if (ProjID <= 0)
				ProjID = ucProject.ObjectId;

			Page.Validate();
			if (!Page.IsValid)
				return;
			bool valid = true;
			bool SaveAndAssign = false;
			if (sender == (object)btnSaveAssign)
				SaveAndAssign = true;

			txtTitle.Text = HttpUtility.HtmlEncode(txtTitle.Text);
			txtDescription.Text = HttpUtility.HtmlEncode(txtDescription.Text);
			txtLocation.Text = HttpUtility.HtmlEncode(txtLocation.Text);

			ArrayList alCategories = new ArrayList();
			for (int i = 0; i < lbCategory.Items.Count; i++)
				if (lbCategory.Items[i].Selected)
					alCategories.Add(int.Parse(lbCategory.Items[i].Value));

			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName())
				orgUid = ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName())
				contactUid = ClientControl.ObjectId;

			if (EventID != 0)
			{
				CalendarEntry2.Update(EventID, txtTitle.Text, txtDescription.Text, txtLocation.Text,
					ucProject.ObjectId, int.Parse(ManagerList.SelectedValue), 
					int.Parse(ddlPriority.SelectedItem.Value),
					int.Parse(ddlType.SelectedItem.Value), dtcStartDate.SelectedDate, dtcEndDate.SelectedDate,
					alCategories, contactUid, orgUid);
			}
			else
			{
				if (fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0)
					EventID = CalendarEntry.Create(txtTitle.Text, txtDescription.Text, txtLocation.Text,
						ucProject.ObjectId, int.Parse(ManagerList.SelectedValue), 
						int.Parse(ddlPriority.SelectedItem.Value),
						int.Parse(ddlType.SelectedItem.Value), dtcStartDate.SelectedDate, dtcEndDate.SelectedDate,
						alCategories, fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream,
						AutoInvite, contactUid, orgUid);
				else if (fAssetFile.PostedFile == null)
					EventID = CalendarEntry.Create(txtTitle.Text, txtDescription.Text, txtLocation.Text,
						ucProject.ObjectId, int.Parse(ManagerList.SelectedValue), 
						int.Parse(ddlPriority.SelectedItem.Value),
						int.Parse(ddlType.SelectedItem.Value), dtcStartDate.SelectedDate, dtcEndDate.SelectedDate,
						alCategories, null, null, AutoInvite, contactUid, orgUid);
				else
				{
					vFile.Visible = true;
					valid = false;
				}
			}
			if (EventID > 0)
			{
				EditControl.Save(EventID);

				string shared = String.Empty;
				if (SharedID > 0) shared = "&SharedId=" + SharedID;
				UserLightPropertyCollection pc = Security.CurrentUser.Properties;
				if (!SaveAndAssign)
				{
					if (!cbOneMore.Checked)
					{
						//pc["EventView_CurrentTab"] = "General";
						Response.Redirect("EventView.aspx?EventID=" + EventID + shared);
					}
					else
					{
						if (ProjID > 0)
							Response.Redirect("../Events/EventEdit.aspx?Checked=1&ProjectID=" + ProjID);
						else
							Response.Redirect("../Events/EventEdit.aspx?Checked=1");
					}
				}
				else
				{
					if (!cbOneMore.Checked)
					{
						pc["EventView_CurrentTab"] = "General";
						Response.Redirect("EventView.aspx?EventID=" + EventID + "&Assign=1" + shared);
					}
					else
					{
						if (ProjID > 0)
							Response.Redirect("../Events/EventEdit.aspx?Checked=1&OldEventID=" + EventID + "&Assign=1&ProjectID=" + ProjID);
						else
							Response.Redirect("../Events/EventEdit.aspx?Checked=1&OldEventID=" + EventID + "&Assign=1");
					}
				}
			}
			if (ProjID > 0 && valid)
			{
				Response.Redirect("../Projects/ProjectView.aspx?ProjectId=" + ProjID + "&Tab=6");
			}
		}

		#endregion

		#region CustomValidator1_ServerValidate
		private void CustomValidator1_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if ((dtcEndDate.SelectedDate != DateTime.MinValue) && (dtcEndDate.SelectedDate < dtcStartDate.SelectedDate))
			{
				//CustomValidator1.ErrorMessage = LocRM.GetString("tWrongDate");
				CustomValidator1.ErrorMessage = LocRM.GetString("EndDateError") + " (" + dtcStartDate.SelectedDate.ToShortDateString() + " " + dtcStartDate.SelectedDate.ToShortTimeString() + ")";
				args.IsValid = false;
			}
			else args.IsValid = true;
		}
		#endregion

		#region btnCancel_ServerClick
		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			if (EventID != 0)
			{
				if (SharedID > 0)
					Response.Redirect("EventView.aspx?EventID=" + EventID + "&SharedId=" + SharedID);
				else
					Response.Redirect("EventView.aspx?EventID=" + EventID);
			}
			else if (ProjID != 0)
				Response.Redirect("../Projects/ProjectView.aspx?ProjectId=" + ProjID);
			else
				Response.Redirect("../Workspace/default.aspx");
		}
		#endregion

		#region FillManagers
		private void FillManagers()
		{
			bool addCurrent = true;
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				using (IDataReader reader = User.GetListActive())
				{
					while (reader.Read())
					{
						ManagerList.Items.Add(new ListItem(reader["LastName"].ToString() + " " + reader["FirstName"].ToString(), reader["PrincipalId"].ToString()));
						if ((int)reader["PrincipalId"] == Security.CurrentUser.UserID)
							addCurrent = false;
					}
				}
			}
			else
			{
				// UserId, Login, FirstName, LastName, Email, IsExternal, IsPending
				using (IDataReader reader = User.GetListActiveUsersForPartnerUser(Security.CurrentUser.UserID))
				{
					while (reader.Read())
					{
						ManagerList.Items.Add(new ListItem(reader["LastName"].ToString() + " " + reader["FirstName"].ToString(), reader["UserId"].ToString()));
						if ((int)reader["UserId"] == Security.CurrentUser.UserID)
							addCurrent = false;
					}
				}
			}

			if (addCurrent)
				ManagerList.Items.Add(new ListItem(Security.CurrentUser.LastName + " " + Security.CurrentUser.FirstName, Security.CurrentUser.UserID.ToString()));
		}
		#endregion
	}
}
