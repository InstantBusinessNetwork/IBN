using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Reflection;
using System.Data;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Clients;
using System.Globalization;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.Projects.Modules
{
	public partial class ResourceUtilFilter : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", Assembly.GetExecutingAssembly());
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strSearch", Assembly.GetExecutingAssembly());
		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;

		#region _projectId
		private int _projectId
		{
			get
			{
				if (Request["ProjectId"] != null)
					return int.Parse(Request["ProjectId"]);
				else
					return -1;
			}
		}
		#endregion

		#region _shared
		private bool _shared
		{
			get
			{
				if (Request["Objs"] != null && Request["Objs"] != String.Empty)
					return true;
				else
					return false;
			}
		}
		#endregion

		#region _sType
		private string _sType
		{
			get
			{
				if (Request["Objs"] != null)
					return Request["Objs"];
				else
					return String.Empty;
			}
		}
		#endregion

		private bool _allChecked = false;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.ddGroups.SelectedIndexChanged += new EventHandler(ddGroups_SelectedIndexChanged);
			this.btnApplyF.Click += new EventHandler(btnApplyF_Click);
			this.btnResetF.Click += new EventHandler(btnResetF_Click);
			this.btnClose.Attributes.Add("onclick", Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));
			this.ddCreated.Attributes.Add("onchange", "ChangeModify();");
			
			ApplyLocalization();
			if (!Page.IsPostBack)
			{
				BindGroups();
				BindDefaultValues();
			}
			BindSavedValues();
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if ((Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) || Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)) && _projectId < 0)
				trManager.Visible = true;
			else
				trManager.Visible = false;

			if (_projectId > 0)
			{
				trProject.Visible = false;
				trGroup.Visible = false;
			}

			if (_shared)
			{
				trUser.Visible = false;
				if (_sType != "All")
					tdObjs.Visible = false;
			}

			if (_projectId <= 0)
				trProject.Visible = Configuration.ProjectManagementEnabled;

			string strChecked = String.Empty;
			if (_allChecked)
				strChecked = "checked='checked'";

			secHeaderObj.Title = String.Format("<input type='checkbox' onclick=\"javascript:ChangeOT(this);\" {1} />&nbsp;{0}",
				LocRM.GetString("tObjects"), strChecked);
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnApplyF.Text = LocRM.GetString("Apply");
			btnResetF.Text = LocRM.GetString("Reset");
			btnClose.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Close").ToString();
			
			cbCalEntries.Text = LocRM.GetString("tEvents");
			cbIssues.Text = LocRM.GetString("tIssues");
			cbDocs.Text = LocRM.GetString("tDocuments");
			cbTasks.Text = LocRM.GetString("tTasks");
			cbToDo.Text = LocRM.GetString("tToDos");

			ChildTodoCheckbox.Text = LocRM.GetString("ShowChildTodo");
			secHeader.Title = LocRM.GetString("State");
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			//Groups Binding
			ddGroups.Items.Clear();

			// Any
			ddGroups.Items.Add(new ListItem(LocRM.GetString("AllFem"), "1"));

			using (IDataReader reader = SecureGroup.GetListGroupsAsTree())
			{
				while (reader.Read())
				{
					string groupName = CommonHelper.GetResFileString(reader["GroupName"].ToString());
					string GroupId = reader["GroupId"].ToString();
					int Level = (int)reader["Level"];
					for (int i = 1; i < Level; i++)
						groupName = "  " + groupName;
					ListItem item = new ListItem(groupName, GroupId);
					ddGroups.Items.Add(item);
				}
			}

			//Saved Value
			if (_pc["MV_Group"] == null)
				_pc["MV_Group"] = ((int)InternalSecureGroups.Intranet).ToString();
			ddGroups.ClearSelection();
			try
			{
				ddGroups.SelectedValue = _pc["MV_Group"];
			}
			catch
			{
				ddGroups.SelectedIndex = 0;
				_pc["MV_Group"] = ddGroups.SelectedValue;
			}

			//Users Binding
			BindUsers(int.Parse(ddGroups.SelectedValue));
		}
		#endregion

		#region BindUsers
		private void BindUsers(int GroupId)
		{
			ddUser.Items.Clear();
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("UserId", typeof(string)));
			dt.Columns.Add(new DataColumn("UserName", typeof(string)));
			DataRow dr;

			dr = dt.NewRow();
			dr[0] = "0";
			dr[1] = LocRM.GetString("All");
			dt.Rows.Add(dr);

			if (GroupId > 0 || _projectId > 0)
			{
				using (IDataReader reader = GetReader(GroupId))
				{
					while (reader.Read())
					{
						if ((byte)reader["Activity"] == (byte)Mediachase.IBN.Business.User.UserActivity.Active)
						{
							dr = dt.NewRow();
							dr[0] = reader["UserId"].ToString();
							dr[1] = reader["LastName"].ToString() + " " + reader["FirstName"].ToString();
							dt.Rows.Add(dr);
						}
					}
				}
			}

			DataView dv = dt.DefaultView;

			ddUser.DataSource = dv;
			ddUser.DataTextField = "UserName";
			ddUser.DataValueField = "UserId";
			ddUser.DataBind();

			//Saved Value
			if (_pc["MV_User"] == null)
				_pc["MV_User"] = ddUser.SelectedValue;
			else
			{
				ddUser.ClearSelection();
				try
				{
					ddUser.SelectedValue = _pc["MV_User"];
				}
				catch
				{
					ddUser.SelectedIndex = 0;
					_pc["MV_User"] = ddUser.SelectedValue;
				}
			}
		}
		#endregion

		#region GetReader
		private IDataReader GetReader(int GroupId)
		{
			if (_projectId < 0)
				return SecureGroup.GetListAllUsersInGroup(GroupId);
			else
				return Project.GetListTeamMemberNames(_projectId);
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
			{
				ddManager.DataSource = Mediachase.IBN.Business.ToDo.GetManagers();
				ddManager.DataTextField = "FullName";
				ddManager.DataValueField = "PrincipalId";
				ddManager.DataBind();
				ddManager.Items.Insert(0, new ListItem(LocRM.GetString("All"), "0"));
			}
			else
				ddManager.Items.Add(new ListItem(Security.CurrentUser.LastName + ", " + Security.CurrentUser.FirstName, Security.CurrentUser.UserID.ToString()));

			ddPrjs.DataSource = Project.GetListProjects();
			ddPrjs.DataTextField = "Title";
			ddPrjs.DataValueField = "ProjectId";
			ddPrjs.DataBind();
			ddPrjs.Items.Insert(0, new ListItem(LocRM.GetString("All"), "0"));

			ddCategory.DataSource = Mediachase.IBN.Business.Project.GetListCategoriesAll();
			ddCategory.DataTextField = "CategoryName";
			ddCategory.DataValueField = "CategoryId";
			ddCategory.DataBind();
			ddCategory.Items.Insert(0, new ListItem(LocRM.GetString("AllFem"), "0"));

			ddCompleted.Items.Clear();
			ddCompleted.Items.Add(new ListItem(LocRM.GetString("tDontShow"), "0"));
			ddCompleted.Items.Add(new ListItem(LocRM.GetString("tForDay"), "-1"));
			ddCompleted.Items.Add(new ListItem(LocRM.GetString("tForWeek"), "-7"));
			ddCompleted.Items.Add(new ListItem(LocRM.GetString("tLastWeek"), "7"));
			ddCompleted.Items.Add(new ListItem(LocRM.GetString("tForMonth"), "-30"));
			ddCompleted.Items.Add(new ListItem(LocRM.GetString("tLastMonth"), "30"));
			ddCompleted.Items.Add(new ListItem(LocRM.GetString("tForAnyPeriod"), "-11000"));

			ddUpcoming.Items.Clear();
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tDontShow"), "0"));
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tInDay"), "1"));
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tInWeek"), "7"));
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tNextWeek"), "-7"));
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tInMonth"), "30"));
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tNextMonth"), "-30"));
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tInAnyPeriod"), "11000"));

			ddShowActive.Items.Clear();
			ddShowActive.Items.Add(new ListItem(LocRM.GetString("tShowActive"), "True"));
			ddShowActive.Items.Add(new ListItem(LocRM.GetString("tDontShowActive"), "False"));

			cbCalEntries.Checked = true;
			cbIssues.Checked = true;
			cbDocs.Checked = true;
			cbTasks.Checked = true;
			cbToDo.Checked = true;
			if (!Configuration.ProjectManagementEnabled)
				cbTasks.Visible = false;
			if (!Configuration.HelpDeskEnabled)
				cbIssues.Visible = false;

			//Client
			ClientControl.ObjectType = String.Empty;
			ClientControl.ObjectId = PrimaryKeyId.Empty;

			if (!PortalConfig.GeneralAllowClientField)
			{
				trClient.Visible = false;
				_pc["MV_ClientNew"] = "_";
			}

			if (!PortalConfig.GeneralAllowGeneralCategoriesField)
			{
				trCategory.Visible = false;
				_pc["MV_Category"] = null;
			}

			ddCreated.Items.Clear();
			ddCreated.Items.Add(new ListItem(CHelper.UpperFirstChar(LocRM2.GetString("tAny")), "0"));
			ddCreated.Items.Add(new ListItem(CHelper.UpperFirstChar(LocRM2.GetString("tToday")), "1"));
			ddCreated.Items.Add(new ListItem(CHelper.UpperFirstChar(LocRM2.GetString("tYesterday")), "2"));
			ddCreated.Items.Add(new ListItem(CHelper.UpperFirstChar(LocRM2.GetString("tThisWeek")), "3"));
			ddCreated.Items.Add(new ListItem(CHelper.UpperFirstChar(LocRM2.GetString("tLastWeek")), "4"));
			ddCreated.Items.Add(new ListItem(CHelper.UpperFirstChar(LocRM2.GetString("tThisMonth")), "5"));
			ddCreated.Items.Add(new ListItem(CHelper.UpperFirstChar(LocRM2.GetString("tLastMonth")), "6"));
			ddCreated.Items.Add(new ListItem(CHelper.UpperFirstChar(LocRM2.GetString("tThisYear")), "7"));
			ddCreated.Items.Add(new ListItem(CHelper.UpperFirstChar(LocRM2.GetString("tLastYear")), "8"));
			ddCreated.Items.Add(new ListItem(CHelper.UpperFirstChar(LocRM.GetString("tPeriod")), "9"));
			dtcStartDate.SelectedDate = UserDateTime.UserNow.AddMonths(-1);
			dtcEndDate.SelectedDate = UserDateTime.UserNow;
			tableDateFrom.Style.Add("display", "none");
			tableDateTo.Style.Add("display", "none");
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			if (_pc["MV_User"] != null)
			{
				ddUser.ClearSelection();
				CommonHelper.SafeSelect(ddUser, _pc["MV_User"]);
			}

			if (!Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) &&
				!Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
			{
				ListItem liItem = ddManager.Items.FindByValue(Security.CurrentUser.UserID.ToString());
				if (liItem != null)
					_pc["MV_Manager"] = liItem.Value;
			}
			if (_pc["MV_Manager"] != null)
			{
				ddManager.ClearSelection();
				CommonHelper.SafeSelect(ddManager, _pc["MV_Manager"]);
			}
			if (_pc["MV_Project"] != null)
			{
				ddPrjs.ClearSelection();
				CommonHelper.SafeSelect(ddPrjs, _pc["MV_Project"]);
			}
			if (_pc["MV_Category"] != null)
			{
				ddCategory.ClearSelection();
				Util.CommonHelper.SafeSelect(ddCategory, _pc["MV_Category"]);
			}

			//Client
			if (_pc["MV_ClientNew"] != null && (!Page.IsPostBack || ClientControl.ObjectId == PrimaryKeyId.Empty))
			{
				string ss = _pc["MV_ClientNew"];
				if (ss.IndexOf("_") >= 0)
				{
					string stype = ss.Substring(0, ss.IndexOf("_"));
					if (stype.ToLower() == "org")
					{
						ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
						ClientControl.ObjectId = PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1));
					}
					else if (stype.ToLower() == "contact")
					{
						ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
						ClientControl.ObjectId = PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1));
					}
					else
					{
						ClientControl.ObjectType = String.Empty;
						ClientControl.ObjectId = PrimaryKeyId.Empty;
					}
				}
			}

			if (_pc["MV_Completed"] != null)
			{
				ddCompleted.ClearSelection();
				CommonHelper.SafeSelect(ddCompleted, _pc["MV_Completed"]);
			}
			if (_pc["MV_Upcoming"] != null)
			{
				ddUpcoming.ClearSelection();
				CommonHelper.SafeSelect(ddUpcoming, _pc["MV_Upcoming"]);
			}
			if (_pc["MV_Active"] != null)
			{
				ddShowActive.ClearSelection();
				CommonHelper.SafeSelect(ddShowActive, _pc["MV_Active"]);
			}
			if (_pc["MV_Created"] != null)
			{
				ddCreated.ClearSelection();
				CommonHelper.SafeSelect(ddCreated, _pc["MV_Created"]);

				if (_pc["MV_Created"] == "9")	// custom
				{
					tableDateFrom.Style.Add("display", "block");
					tableDateTo.Style.Add("display", "block");

					if (_pc["MV_CreatedFrom"] != null)
						dtcStartDate.SelectedDate = DateTime.Parse(_pc["MV_CreatedFrom"], CultureInfo.InvariantCulture);
					if (_pc["MV_CreatedTo"] != null)
						dtcEndDate.SelectedDate = DateTime.Parse(_pc["MV_CreatedTo"], CultureInfo.InvariantCulture);
				}
			}

			if (_pc["MV_ShowEvents"] != null)
				cbCalEntries.Checked = bool.Parse(_pc["MV_ShowEvents"]);
			else
				cbCalEntries.Checked = true;

			if (_pc["MV_ShowIssues"] != null)
				cbIssues.Checked = bool.Parse(_pc["MV_ShowIssues"]);
			else
				cbIssues.Checked = true;

			if (_pc["MV_ShowDocs"] != null)
				cbDocs.Checked = bool.Parse(_pc["MV_ShowDocs"]);
			else
				cbDocs.Checked = true;

			if (_pc["MV_ShowTasks"] != null)
				cbTasks.Checked = bool.Parse(_pc["MV_ShowTasks"]);
			else
				cbTasks.Checked = true;

			if (_pc["MV_ShowToDo"] != null)
				cbToDo.Checked = bool.Parse(_pc["MV_ShowToDo"]);
			else
				cbToDo.Checked = true;

			_allChecked = cbCalEntries.Checked && cbDocs.Checked && cbToDo.Checked;
			if (Configuration.ProjectManagementEnabled)
				_allChecked = _allChecked && cbTasks.Checked;
			if (Configuration.HelpDeskEnabled)
				_allChecked = _allChecked && cbIssues.Checked;
			
			if (_pc["MV_ShowChildTodo"] != null)
				ChildTodoCheckbox.Checked = bool.Parse(_pc["MV_ShowChildTodo"]);
			else
				ChildTodoCheckbox.Checked = false;
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			_pc["MV_User"] = ddUser.SelectedValue;
			_pc["MV_Manager"] = ddManager.SelectedValue;
			_pc["MV_Project"] = ddPrjs.SelectedValue;
			_pc["MV_Category"] = ddCategory.SelectedValue;
			_pc["MV_Completed"] = ddCompleted.SelectedValue;
			_pc["MV_Upcoming"] = ddUpcoming.SelectedValue;
			_pc["MV_Active"] = ddShowActive.SelectedValue;
			_pc["MV_Created"] = ddCreated.SelectedValue;

			if (ddCreated.SelectedValue == "9")	// custom
			{
				_pc["MV_CreatedFrom"] = dtcStartDate.SelectedDate.Date.ToString(CultureInfo.InvariantCulture);
				_pc["MV_CreatedTo"] = dtcEndDate.SelectedDate.Date.ToString(CultureInfo.InvariantCulture);
			}

			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc["MV_ClientNew"] = "org_" + ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc["MV_ClientNew"] = "contact_" + ClientControl.ObjectId;
			else
				_pc["MV_ClientNew"] = "_";

			_pc["MV_ShowEvents"] = cbCalEntries.Checked.ToString();
			_pc["MV_ShowIssues"] = cbIssues.Checked.ToString();
			_pc["MV_ShowDocs"] = cbDocs.Checked.ToString();
			_pc["MV_ShowTasks"] = cbTasks.Checked.ToString();
			_pc["MV_ShowToDo"] = cbToDo.Checked.ToString();
			_pc["MV_ShowChildTodo"] = ChildTodoCheckbox.Checked.ToString();
		}
		#endregion

		#region Events
		void btnResetF_Click(object sender, EventArgs e)
		{
			_pc["MV_Group"] = ((int)InternalSecureGroups.Intranet).ToString();
			BindGroups();
			BindDefaultValues();
			SaveValues();
			ddUser.SelectedIndex = 0;
			_pc["MV_User"] = ddUser.SelectedValue;
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, String.Empty);
		}

		void btnApplyF_Click(object sender, EventArgs e)
		{
			SaveValues();
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, String.Empty);
		}

		void ddGroups_SelectedIndexChanged(object sender, EventArgs e)
		{
			_pc["MV_Group"] = ddGroups.SelectedValue;
			BindUsers(int.Parse(ddGroups.SelectedValue));
		} 
		#endregion
	}
}