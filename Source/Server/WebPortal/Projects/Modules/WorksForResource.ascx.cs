namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Resources;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
	using ComponentArt.Web.UI;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn.Clients;
	using Mediachase.Ibn.Core.Business;
	using Mediachase.Ibn.Data;
	using System.Web.UI;
	using Mediachase.Ibn.Web.UI;

	/// <summary>
	///		Summary description for WorksForResource.
	/// </summary>
	public partial class WorksForResource : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(WorksForResource).Assembly);
		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;
		protected enum FieldSetName
		{
			tWorkResourcesDefault = 1,
			tWorkResourcesDates = 2,
			tWorkResourcesWorkTime = 3
		}

		#region _projectId
		private int _projectId
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

		#region _shared
		private bool _shared
		{
			get
			{
				try
				{
					return (Request["Shared"] == "1") ? true : false;
				}
				catch
				{
					return false;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if (!Page.IsPostBack)
			{
				if (_pc["RV_Sort"] == null)
					_pc["RV_Sort"] = "Title";

				if (_pc["RV_ViewStyle"] == null)
					_pc["RV_ViewStyle"] = FieldSetName.tWorkResourcesDefault.ToString();

				if (_pc["RV_Grouping"] == null)
					_pc["RV_Grouping"] = "0";

				if (!Configuration.ProjectManagementEnabled)
				{
					_pc["RV_Project"] = "0";
					_pc["RV_ShowTasks"] = false.ToString();
				}
				if (!Configuration.HelpDeskEnabled)
					_pc["RV_ShowIssues"] = false.ToString();
				BindDefaultValues();
			}
			BindSavedValues();
			BindInfoTable();

			BindDataGrid();
		}

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			if (!_shared)
				trPerson.Visible = false;
			else
				trPerson.Visible = true;

			FilterTable.Visible = (_pc["RV_ShowFilter"] != null && bool.Parse(_pc["RV_ShowFilter"]));
			tblFilterInfo.Visible = !FilterTable.Visible;

			trProject.Visible = false;
			if (_projectId <= 0)
				trProject.Visible = Configuration.ProjectManagementEnabled;

			if (!Configuration.ProjectManagementEnabled)
				dgObjects.Columns[6].Visible = false;

			BindToolbar();
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (this.Parent.Parent is IPageViewMenu || this.Parent is IPageViewMenu)
			{
				PageViewMenu secHeader;
				if (this.Parent.Parent is IPageViewMenu)
					secHeader = (PageViewMenu)((IPageViewMenu)this.Parent.Parent).GetToolBar();
				else
					secHeader = (PageViewMenu)((IPageViewMenu)this.Parent).GetToolBar();

				RenderMenu(secHeader.ActionsMenu);
			}
			else if (this.Parent.Parent is IToolbarLight || this.Parent is IToolbarLight)
			{
				BlockHeaderLightWithMenu secHeader;
				if (this.Parent.Parent is IToolbarLight)
					secHeader = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent.Parent).GetToolBar();
				else
					secHeader = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent).GetToolBar();

				secHeader.ClearRightItems();

				RenderMenu(secHeader.ActionsMenu);

				secHeader.EnsureRender();
			}
		}

		private void RenderMenu(ComponentArt.Web.UI.Menu actionsMenu)
		{
			ComponentArt.Web.UI.MenuItem topMenuItem;
			actionsMenu.Items.Clear();

			#region Print
			topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = LocRM.GetString("Export");
			topMenuItem.LookId = "TopItemLook";
			topMenuItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbExport, "");
			actionsMenu.Items.Add(topMenuItem);
			#endregion

			#region View Menu Items
			topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = LocRM.GetString("tView");
			topMenuItem.Look.LeftIconUrl = Page.ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;
			string sCurrentView = _pc["RV_ViewStyle"];

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (sCurrentView == FieldSetName.tWorkResourcesDefault.ToString())
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewDef, "");
			subItem.Text = LocRM.GetString(FieldSetName.tWorkResourcesDefault.ToString());
			topMenuItem.Items.Add(subItem);

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (sCurrentView == FieldSetName.tWorkResourcesDates.ToString())
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewDates, "");
			subItem.Text = LocRM.GetString(FieldSetName.tWorkResourcesDates.ToString());
			topMenuItem.Items.Add(subItem);

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (sCurrentView == FieldSetName.tWorkResourcesWorkTime.ToString())
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewTimes, "");
			subItem.Text = LocRM.GetString(FieldSetName.tWorkResourcesWorkTime.ToString());
			topMenuItem.Items.Add(subItem);
			#endregion

			actionsMenu.Items.Add(topMenuItem);

			if (_projectId > 0)
				return;

			#region Grouping Menu Items
			topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = LocRM.GetString("tGroupBy");
			topMenuItem.Look.LeftIconUrl = Page.ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			sCurrentView = _pc["RV_Grouping"];

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (sCurrentView == "0")
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(btnApplyG, "0");
			subItem.Text = LocRM.GetString("tNoGrouping");
			topMenuItem.Items.Add(subItem);

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (sCurrentView == "2")
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(btnApplyG, "2");
			subItem.Text = LocRM.GetString("tByManager");
			topMenuItem.Items.Add(subItem);

			if (Configuration.ProjectManagementEnabled)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				if (sCurrentView == "3")
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(btnApplyG, "3");
				subItem.Text = LocRM.GetString("tByProject");
				topMenuItem.Items.Add(subItem);
			}

			// Clients
			if (PortalConfig.GeneralAllowClientField)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				if (sCurrentView == "4")
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(btnApplyG, "4");
				subItem.Text = LocRM.GetString("tByClient");
				topMenuItem.Items.Add(subItem);
			}

			// Categories
			if (PortalConfig.GeneralAllowGeneralCategoriesField)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				if (sCurrentView == "5")
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(btnApplyG, "5");
				subItem.Text = LocRM.GetString("tByCategory");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			actionsMenu.Items.Add(topMenuItem);
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnApplyF.Text = LocRM.GetString("Apply");
			btnResetF.Text = LocRM.GetString("Reset");
			btnVResetF.Value = LocRM.GetString("ResetFilter");
			
			cbCalEntries.Text = LocRM.GetString("tEvents");
			cbIssues.Text = LocRM.GetString("tIssues");
			cbDocs.Text = LocRM.GetString("tDocuments");
			cbTasks.Text = LocRM.GetString("tTasks");
			cbToDo.Text = LocRM.GetString("tToDos");
			cbChkAll.Text = "&nbsp;" + LocRM.GetString("tObjects");
			lbShowFilter.Text = "<img alt='' title='" + LocRM.GetString("tShowFilter") + "' src='" + Page.ResolveUrl("~/Layouts/Images/scrolldown_hover.gif") + "'/>";
			lbHideFilter.Text = "<img alt='' title='" + LocRM.GetString("tHideFilter") + "' src='" + Page.ResolveUrl("~/Layouts/Images/scrollup_hover.gif") + "'/>";
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			ddManager.DataSource = Mediachase.IBN.Business.ToDo.GetManagers();
			ddManager.DataTextField = "FullName";
			ddManager.DataValueField = "PrincipalId";
			ddManager.DataBind();
			ddManager.Items.Insert(0, new ListItem(LocRM.GetString("All"), "0"));

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
			ddCompleted.Items.Add(new ListItem(LocRM.GetString("tForAnyPeriod"), "-11000"));

			ddUpcoming.Items.Clear();
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tDontShow"), "0"));
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tInDay"), "1"));
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tInWeek"), "7"));
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tInAnyPeriod"), "11000"));

			ddShowActive.Items.Clear();
			ddShowActive.Items.Add(new ListItem(LocRM.GetString("tShowActive"), "True"));
			ddShowActive.Items.Add(new ListItem(LocRM.GetString("tDontShowActive"), "False"));

			if (_shared)
			{
				ddPerson.Items.Clear();
				using (IDataReader reader = Mediachase.IBN.Business.CalendarView.GetListPeopleForCalendar())
				{
					while (reader.Read())
						ddPerson.Items.Add(new ListItem((string)reader["LastName"] + " " + (string)reader["FirstName"], reader["UserId"].ToString()));
				}
				if (_pc["RV_Person"] == null)
					_pc["RV_Person"] = ddPerson.SelectedValue;
			}

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
				_pc["RV_ClientNew"] = "_";
			}

			if (!PortalConfig.GeneralAllowGeneralCategoriesField)
			{
				trCategory.Visible = false;
				_pc["RV_Category"] = null;
			}
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			if (_pc["RV_Manager"] != null)
			{
				ddManager.ClearSelection();
				Util.CommonHelper.SafeSelect(ddManager, _pc["RV_Manager"]);
			}

			if (_shared && _pc["RV_Person"] != null)
			{
				ddPerson.ClearSelection();
				Util.CommonHelper.SafeSelect(ddPerson, _pc["RV_Person"]);
			}

			if (_pc["RV_Project"] != null)
			{
				ddPrjs.ClearSelection();
				Util.CommonHelper.SafeSelect(ddPrjs, _pc["RV_Project"]);
			}

			if (_pc["RV_Category"] != null)
			{
				ddCategory.ClearSelection();
				Util.CommonHelper.SafeSelect(ddCategory, _pc["RV_Category"]);
			}

			if (_pc["RV_Completed"] != null)
			{
				ddCompleted.ClearSelection();
				Util.CommonHelper.SafeSelect(ddCompleted, _pc["RV_Completed"]);
			}
			if (_pc["RV_Upcoming"] != null)
			{
				ddUpcoming.ClearSelection();
				Util.CommonHelper.SafeSelect(ddUpcoming, _pc["RV_Upcoming"]);
			}
			if (_pc["RV_Active"] != null)
			{
				ddShowActive.ClearSelection();
				Util.CommonHelper.SafeSelect(ddShowActive, _pc["RV_Active"]);
			}

			//Client
			if (_pc["RV_ClientNew"] != null && (!Page.IsPostBack || ClientControl.ObjectId == PrimaryKeyId.Empty))
			{
				string ss = _pc["RV_ClientNew"];
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

			if (_pc["RV_ShowEvents"] != null)
				cbCalEntries.Checked = bool.Parse(_pc["RV_ShowEvents"]);
			else
				cbCalEntries.Checked = true;

			if (_pc["RV_ShowIssues"] != null)
				cbIssues.Checked = bool.Parse(_pc["RV_ShowIssues"]);
			else
				cbIssues.Checked = true;

			if (_pc["RV_ShowDocs"] != null)
				cbDocs.Checked = bool.Parse(_pc["RV_ShowDocs"]);
			else
				cbDocs.Checked = true;

			if (_pc["RV_ShowTasks"] != null)
				cbTasks.Checked = bool.Parse(_pc["RV_ShowTasks"]);
			else
				cbTasks.Checked = true;

			if (_pc["RV_ShowToDo"] != null)
				cbToDo.Checked = bool.Parse(_pc["RV_ShowToDo"]);
			else
				cbToDo.Checked = true;

			bool Allchecked = cbCalEntries.Checked && cbDocs.Checked && cbToDo.Checked;
			if (Configuration.ProjectManagementEnabled)
				Allchecked = Allchecked && cbTasks.Checked;
			if (Configuration.HelpDeskEnabled)
				Allchecked = Allchecked && cbIssues.Checked;
			if (Allchecked)
				cbChkAll.Checked = true;
			else
				cbChkAll.Checked = false;
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			_pc["RV_Manager"] = ddManager.SelectedValue;
			_pc["RV_Project"] = ddPrjs.SelectedValue;
			_pc["RV_Category"] = ddCategory.SelectedValue;
			_pc["RV_Completed"] = ddCompleted.SelectedValue;
			_pc["RV_Upcoming"] = ddUpcoming.SelectedValue;
			_pc["RV_Active"] = ddShowActive.SelectedValue;
			if (_shared)
				_pc["RV_Person"] = ddPerson.SelectedValue;

			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc["RV_ClientNew"] = "org_" + ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc["RV_ClientNew"] = "contact_" + ClientControl.ObjectId;
			else
				_pc["RV_ClientNew"] = "_";

			_pc["RV_ShowEvents"] = cbCalEntries.Checked.ToString();
			_pc["RV_ShowIssues"] = cbIssues.Checked.ToString();
			_pc["RV_ShowDocs"] = cbDocs.Checked.ToString();
			_pc["RV_ShowTasks"] = cbTasks.Checked.ToString();
			_pc["RV_ShowToDo"] = cbToDo.Checked.ToString();
		}
		#endregion

		#region BindInfoTable
		private void BindInfoTable()
		{
			tblFilterInfoSet.Rows.Clear();

			//Person
			if (_shared)
			{
				ListItem li = ddPerson.Items.FindByValue(_pc["RV_Person"]);
				if (li != null && li.Value != "0")
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tPerson")), li.Text);
			}

			// Project
			if (_projectId > 0)
			{
				AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tProject")), "<font color='red'>" + Task.GetProjectTitle(_projectId) + "</font>");
			}
			else if (_pc["RV_Project"] != null)
			{
				ListItem li = ddPrjs.Items.FindByValue(_pc["RV_Project"]);
				if (li != null && li.Value != "0")
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tProject")), li.Text);
			}

			// Manager
			if (_pc["RV_Manager"] != null)
			{
				ListItem li = ddManager.Items.FindByValue(_pc["RV_Manager"]);
				if (li != null && li.Value != "0")
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tManager")), li.Text);
			}

			// Category
			if (_pc["RV_Category"] != null)
			{
				ListItem li = ddCategory.Items.FindByValue(_pc["RV_Category"]);
				if (li != null && li.Value != "0")
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Category")), li.Text);
			}

			//Client
			if (_pc["RV_ClientNew"] != null)
			{
				string ss = _pc["RV_ClientNew"];
				if (ss.IndexOf("_") >= 0)
				{
					string stype = ss.Substring(0, ss.IndexOf("_"));

					string sName = "";
					if (stype.ToLower() == "org")
					{
						OrganizationEntity orgEntity = (OrganizationEntity)BusinessManager.Load(OrganizationEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1)));
						if (orgEntity != null)
							sName = orgEntity.Name;
						AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Client")), sName);
					}
					else if (stype.ToLower() == "contact")
					{
						ContactEntity contactEntity = (ContactEntity)BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1)));
						if (contactEntity != null)
							sName = contactEntity.FullName;
						AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Client")), sName);
					}
				}
 			}

			//State
			string state = "";
			if (_pc["RV_Completed"] != null)
			{
				ListItem li = ddCompleted.Items.FindByValue(_pc["RV_Completed"]);
				if (li != null && li.Value != "0")
				{
					state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowCompleted"), li.Text);
				}
			}
			if (_pc["RV_Active"] == null || bool.Parse(_pc["RV_Active"]))
			{
				if (state != "")
					state += ", ";
				state += LocRM.GetString("tActive");
			}
			if (_pc["RV_Upcoming"] != null)
			{
				ListItem li = ddUpcoming.Items.FindByValue(_pc["RV_Upcoming"]);
				if (li != null && li.Value != "0")
				{
					if (state != "")
						state += ", ";
					state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowUpcoming"), li.Text);
				}
			}
			if (state != "")
				AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("State")), state);

			// Objects
			string Objs = "";
			if (_pc["RV_ShowEvents"] == null || bool.Parse(_pc["RV_ShowEvents"]))
			{
				if (Objs != "")
					Objs += ", ";
				Objs += LocRM.GetString("tEvents");
			}
			if (_pc["RV_ShowIssues"] == null || bool.Parse(_pc["RV_ShowIssues"]))
			{
				if (Objs != "")
					Objs += ", ";
				Objs += LocRM.GetString("tIssues");
			}
			if (_pc["RV_ShowDocs"] == null || bool.Parse(_pc["RV_ShowDocs"]))
			{
				if (Objs != "")
					Objs += ", ";
				Objs += LocRM.GetString("tDocuments");
			}
			if (_pc["RV_ShowTasks"] == null || bool.Parse(_pc["RV_ShowTasks"]))
			{
				if (Objs != "")
					Objs += ", ";
				Objs += LocRM.GetString("tTasks");
			}
			if (_pc["RV_ShowToDo"] == null || bool.Parse(_pc["RV_ShowToDo"]))
			{
				if (Objs != "")
					Objs += ", ";
				Objs += LocRM.GetString("tToDos");
			}
			if (Objs != "")
				AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tShow")), Objs);
		}

		private void AddRow(string filterName, string filterValue)
		{
			HtmlTableRow tr = new HtmlTableRow();
			HtmlTableCell td1 = new HtmlTableCell();
			td1.Align = "right";
			td1.Width = "120px";
			td1.InnerHtml = filterName;
			HtmlTableCell td2 = new HtmlTableCell();
			td2.InnerHtml = filterValue;
			tr.Cells.Add(td1);
			tr.Cells.Add(td2);
			tblFilterInfoSet.Rows.Add(tr);
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			int i = 3;
			dgObjects.Columns[i++].HeaderText = "!!!";
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("Title");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("Type");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tProject");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("Manager");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("PercentCompleted");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("StartDate");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("FinishDate2");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tActualStartDate");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tActualFinishDate");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tTaskTime");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tTotalMinutes");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tTotalApproved");

			foreach (DataGridColumn dgc in dgObjects.Columns)
			{
				if (dgc.SortExpression == _pc["RV_Sort"].ToString())
					dgc.HeaderText += String.Format("&nbsp;<img alt='' src='{0}'/>",
						Page.ResolveUrl("~/Layouts/Images/upbtnF.jpg"));
				else if (dgc.SortExpression + " DESC" == _pc["RV_Sort"].ToString())
					dgc.HeaderText += String.Format("&nbsp;<img alt='' src='{0}'/>",
						Page.ResolveUrl("~/Layouts/Images/downbtnF.jpg"));
			}

			BindDataGrid(dgObjects);
		}

		private void BindDataGrid(DataGrid dg)
		{
			int projectId = (_projectId > 0) ? _projectId : int.Parse(ddPrjs.SelectedValue);
			int resId = (_shared) ? int.Parse(ddPerson.SelectedValue) : Security.CurrentUser.UserID;
			int managerId = int.Parse(ddManager.SelectedValue);
			int categoryId = int.Parse(ddCategory.SelectedValue);
			bool showActive = bool.Parse(ddShowActive.SelectedValue);

			// Client
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName())
				orgUid = ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName())
				contactUid = ClientControl.ObjectId;

			ArrayList alTypes = new ArrayList();
			if (cbCalEntries.Checked)
				alTypes.Add((int)ObjectTypes.CalendarEntry);
			if (cbIssues.Checked)
				alTypes.Add((int)ObjectTypes.Issue);
			if (cbDocs.Checked)
				alTypes.Add((int)ObjectTypes.Document);
			if (cbTasks.Checked)
				alTypes.Add((int)ObjectTypes.Task);
			if (cbToDo.Checked)
				alTypes.Add((int)ObjectTypes.ToDo);

			DateTime dtCompleted = UserDateTime.UserNow;
			DateTime dtUpcoming = UserDateTime.UserNow;
			int iCompleted = int.Parse(ddCompleted.SelectedValue);
			if (iCompleted < 0)
				dtCompleted = UserDateTime.UserToday.AddDays(iCompleted - 1).AddSeconds(1);
			int iUpcoming = int.Parse(ddUpcoming.SelectedValue);
			if (iUpcoming > 0)
				dtUpcoming = UserDateTime.UserToday.AddDays(iUpcoming + 1).AddSeconds(-1);

			int groupBy = int.Parse(_pc["RV_Grouping"]);
			if (dg == dgExport || _projectId > 0)
				groupBy = 0;

			DataTable dt = Mediachase.IBN.Business.ToDo.GetGroupedItemsForResourceViewDataTable(groupBy,
				resId, managerId, projectId, categoryId, showActive, alTypes, dtCompleted, dtUpcoming, orgUid, contactUid);

			DataView dv = dt.DefaultView;
			if (groupBy == 0)
			{
				if (_pc["RV_Sort"] == null)
					_pc["RV_Sort"] = "Title";
				dv.Sort = _pc["RV_Sort"];
				if (dg != dgExport)
					dg.AllowSorting = true;
			}
			else
				dg.AllowSorting = false;

			dg.DataSource = dv;

			if (dg != dgExport)
			{
				if (_pc["RV_PageSize"] != null)
					dg.PageSize = int.Parse(_pc["RV_PageSize"]);

				if (_pc["RV_Page"] != null)
				{
					int pageindex = int.Parse(_pc["RV_Page"]);
					int ppi = dv.Count / dg.PageSize;
					if (dv.Count % dg.PageSize == 0)
						ppi = ppi - 1;

					if (pageindex <= ppi)
						dg.CurrentPageIndex = pageindex;
					else
						dg.CurrentPageIndex = 0;
				}

				if (_projectId > 0)
					dg.Columns[6].Visible = false;

				// FieldSet
				string fieldSet = _pc["RV_ViewStyle"];

				int j = 6;
				switch (fieldSet)
				{
					case "tWorkResourcesDates":
						dg.Columns[j++].Visible = false;	//Project
						dg.Columns[j++].Visible = false;	//Manager
						dg.Columns[j++].Visible = false;	//PercentCompleted
						dg.Columns[j++].Visible = true;		//StartDate
						dg.Columns[j++].Visible = true;		//FinishDate
						dg.Columns[j++].Visible = true;		//ActualStartDate
						dg.Columns[j++].Visible = true;		//ActualFinishDate
						dg.Columns[j++].Visible = false;	//TaskTime
						dg.Columns[j++].Visible = false;	//TotalMinutes
						dg.Columns[j++].Visible = false;	//TotalApproved
						break;
					case "tWorkResourcesWorkTime":
						dg.Columns[j++].Visible = false;	//Project
						dg.Columns[j++].Visible = false;	//Manager
						dg.Columns[j++].Visible = false;	//PercentCompleted
						dg.Columns[j++].Visible = false;	//StartDate
						dg.Columns[j++].Visible = false;	//FinishDate
						dg.Columns[j++].Visible = false;	//ActualStartDate
						dg.Columns[j++].Visible = false;	//ActualFinishDate
						dg.Columns[j++].Visible = true;		//TaskTime
						dg.Columns[j++].Visible = true;		//TotalMinutes
						dg.Columns[j++].Visible = true;		//TotalApproved
						break;
					default:	//"tWorkResourcesDefault"
						dg.Columns[j++].Visible = true;		//Project
						dg.Columns[j++].Visible = true;		//Manager
						dg.Columns[j++].Visible = true;		//PercentCompleted
						dg.Columns[j++].Visible = true;		//StartDate
						dg.Columns[j++].Visible = true;		//FinishDate
						dg.Columns[j++].Visible = false;	//ActualStartDate
						dg.Columns[j++].Visible = false;	//ActualFinishDate
						dg.Columns[j++].Visible = false;	//TaskTime
						dg.Columns[j++].Visible = false;	//TotalMinutes
						dg.Columns[j++].Visible = false;	//TotalApproved
						break;
				}
			}

			dg.DataBind();

			if (dg != dgExport)
			{
				foreach (DataGridItem dgi in dg.Items)
				{
					if (int.Parse(dgi.Cells[0].Text) == 0)
					{
						dgi.BackColor = Color.FromArgb(0xBB, 0xBB, 0xBB);
					}
					else if (int.Parse(dgi.Cells[1].Text) == (int)ObjectStates.Upcoming)
						dgi.BackColor = Color.FromArgb(0xE9, 0xFE, 0xEC);
					else if (int.Parse(dgi.Cells[1].Text) == (int)ObjectStates.Suspended || int.Parse(dgi.Cells[1].Text) == (int)ObjectStates.Completed)
						dgi.BackColor = Color.FromArgb(0xF2, 0xF2, 0xF2);

					Control ib = dgi.FindControl("ibDelete");
					if (ib != null)
					{
						switch (int.Parse(dgi.Cells[2].Text))
						{
							case (int)ObjectTypes.Task:
								if (!Task.CanDelete(int.Parse(dgi.Cells[0].Text)))
									ib.Visible = false;
								break;
							case (int)ObjectTypes.ToDo:
								if (!Mediachase.IBN.Business.ToDo.CanDelete(int.Parse(dgi.Cells[0].Text)))
									ib.Visible = false;
								break;
							case (int)ObjectTypes.Document:
								if (!Document.CanDelete(int.Parse(dgi.Cells[0].Text)))
									ib.Visible = false;
								break;
							case (int)ObjectTypes.Issue:
								if (!Incident.CanDelete(int.Parse(dgi.Cells[0].Text)))
									ib.Visible = false;
								break;
							case (int)ObjectTypes.CalendarEntry:
								if (!CalendarEntry.CanDelete(int.Parse(dgi.Cells[0].Text)))
									ib.Visible = false;
								break;
							default:
								ib.Visible = false;
								break;
						}
					}
				}
			}
		}
		#endregion

		#region Protected DG strings
		protected string GetTitle(int itemId, string title, int itemType, object groupName, int stateId, bool isOverdue, bool isNewMessage, int projectId)
		{
			string result = string.Empty;

			if (itemId == 0)
			{
				if (projectId > 0)
					result = string.Format(CultureInfo.InvariantCulture,
						"<b><a href=\"{0}\">{1}</a></b>", 
						CHelper.GetObjectLink((int)ObjectTypes.Project, projectId),
						groupName.ToString());
				else
					result = string.Format(CultureInfo.InvariantCulture, "<b>{0}</b>", groupName.ToString());
			}
			else
			{
				string link = null;
				string completedIcon = "";
				string suspendedIcon = "";
				string activeIcon = "";
				string overdueIcon = "";

				switch (itemType)
				{
					case (int)ObjectTypes.Task:
						link = String.Format("~/Tasks/TaskView.aspx?TaskId={0}{1}", itemId, (_shared) ? ("&SharedId=" + _pc["RV_Person"]) : "");
						completedIcon = "task1_completed.gif";
						activeIcon = "task1.gif";
						suspendedIcon = "task1_suspensed.gif";
						overdueIcon = "task1_overdue.gif";
						title += " (#" + itemId + ")";
						break;
					case (int)ObjectTypes.ToDo:
						link = String.Format("~/ToDo/ToDoView.aspx?ToDoId={0}{1}", itemId, (_shared) ? ("&SharedId=" + _pc["RV_Person"]) : "");
						completedIcon = "task_completed.gif";
						activeIcon = "task.gif";
						suspendedIcon = "task_suspensed.gif";
						overdueIcon = "task_overdue.gif";
						title += " (#" + itemId + ")";
						break;
					case (int)ObjectTypes.Document:
						link = String.Format("~/Documents/DocumentView.aspx?DocumentId={0}{1}", itemId, (_shared) ? ("&SharedId=" + _pc["RV_Person"]) : "");
						completedIcon = "document_completed.gif";
						activeIcon = "document.gif";
						suspendedIcon = "document_suspensed.gif";
						overdueIcon = "document.gif";
						title += " (#" + itemId + ")";
						break;
					case (int)ObjectTypes.Issue:
						link = String.Format("~/Incidents/IncidentView.aspx?IncidentId={0}{1}", itemId, (_shared) ? ("&SharedId=" + _pc["RV_Person"]) : "");
						completedIcon = "incident_closed.gif";
						activeIcon = "incident.gif";
						suspendedIcon = "incident.gif";
						overdueIcon = "incident.gif";
						title += " (#" + itemId + ")";
						break;
					case (int)ObjectTypes.CalendarEntry:
						link = String.Format("~/Events/EventView.aspx?EventId={0}{1}", itemId, (_shared) ? ("&SharedId=" + _pc["RV_Person"]) : "");
						completedIcon = "event_completed.gif";
						activeIcon = "event.gif";
						suspendedIcon = "event.gif";
						overdueIcon = "event.gif";
						break;
				}

				if (!string.IsNullOrEmpty(link))
				{
					link = Page.ResolveUrl(link);

					if (isNewMessage)
						title = string.Format(CultureInfo.InvariantCulture, "<b>{0}</b>", title);

					if (stateId == (int)ObjectStates.Completed)
						result = string.Format("<span style='color:#999999;text-decoration:line-through;'><span><a href='{0}'><img alt='' src='{2}' /></span> <span>{1}</span></a></span>", link, title, Page.ResolveUrl("~/Layouts/Images/icons/" + completedIcon));
					else if (stateId == (int)ObjectStates.Suspended)
						result = string.Format("<a href='{0}'><span><img alt='' src='{2}' /></span> <span>{1}</span></a>", link, title, Page.ResolveUrl("~/Layouts/Images/icons/" + suspendedIcon));
					else if (stateId == (int)ObjectStates.Overdue || isOverdue)
						result = string.Format("<a href='{0}'><font color='red'><span><img alt='' src='{2}' /></span> <span>{1}</span></font></a>", link, title, Page.ResolveUrl("~/Layouts/Images/icons/" + overdueIcon));
					else
						result = string.Format("<a href='{0}'><span><img alt='' src='{2}' /></span> <span>{1}</span></a>", link, title, Page.ResolveUrl("~/Layouts/Images/icons/" + activeIcon));
				}
			}

			return result;
		}

		protected string GetMinutes(object minutes, int stateId, bool isOverdue)
		{
			if (minutes == DBNull.Value)
				return "";
			string sHours = Util.CommonHelper.GetHours((int)minutes);

			if (stateId == (int)ObjectStates.Completed)
				return String.Format("<span style='color:#999999;text-decoration:line-through'><span style='color:black'>{0}</span></span>", sHours);
			else if (stateId == (int)ObjectStates.Overdue || isOverdue)
				return String.Format("<span style='color:red'>{0}</span>", sHours);
			else
				return sHours;
		}

		protected string GetType(int itemType, int stateId, bool isOverdue)
		{
			string result = string.Empty;

			if (itemType > 0)
			{
				string type = null;

				switch (itemType)
				{
					case (int)ObjectTypes.Task:
							return LocRM.GetString("tTask");
					case (int)ObjectTypes.ToDo:
							return LocRM.GetString("tTodo");
					case (int)ObjectTypes.Document:
							return LocRM.GetString("tDoc");
					case (int)ObjectTypes.Issue:
							return LocRM.GetString("tIssue");
					case (int)ObjectTypes.CalendarEntry:
							return LocRM.GetString("tCalEntry");
				}

				if (!string.IsNullOrEmpty(type))
				{
					if (stateId == (int)ObjectStates.Completed)
						result = string.Format("<span style='color:#999999;text-decoration:line-through'><span style='color:black'>{0}</span></span>", type);
					else if (isOverdue || stateId == (int)ObjectStates.Overdue)
						result = string.Format("<span style='color:red'>{0}</span>", type);
					else
						result = type;
				}
			}

			return result;
		}

		protected string GetDate(object _date, int StateId, bool isOverdue)
		{
			if (_date != DBNull.Value)
			{
				if (StateId == (int)ObjectStates.Completed)
					return String.Format("<span style='color:#999999;text-decoration:line-through'><span style='color:black'>{0}</span></span>", ((DateTime)_date).ToShortDateString());
				else if (StateId == (int)ObjectStates.Overdue || isOverdue)
					return String.Format("<span style='color:red'>{0}</span>", ((DateTime)_date).ToShortDateString());
				else
					return ((DateTime)_date).ToShortDateString();
			}
			else
				return "";
		}

		protected string GetProjectDate(int itemId, int projectId)
		{
			string result = string.Empty;

			if (itemId == 0 && projectId > 0)
			{
				using (IDataReader reader = Project.GetProject(projectId, false))
				{
					if (reader.Read() && reader["ActualFinishDate"] != DBNull.Value)
					{
						result = String.Format(CultureInfo.InvariantCulture,
							"<b>{0}</b>", ((DateTime)reader["ActualFinishDate"]).ToShortDateString());
					}
				}
			}
			return result;
		}

		protected string GetPercentCompleted(int itemType, int percentCompleted, int stateId, bool isOverdue)
		{
			string retVal = string.Empty;

			string image = Page.ResolveUrl("~/Layouts/Images/" + ((isOverdue || stateId == (int)ObjectStates.Overdue) ? "redpoint.gif" : "point.gif"));

			if (itemType == (int)ObjectTypes.Task || itemType == (int)ObjectTypes.ToDo)
			{
				string strPercCompleted = string.Empty;
				if (stateId == (int)ObjectStates.Completed)
					strPercCompleted = string.Format("<span style='color:#999999;text-decoration:line-through'><span style='color:black'>{0}%</span></span>", percentCompleted);
				else if (stateId == (int)ObjectStates.Overdue || isOverdue)
					strPercCompleted = string.Format("<span style='color:red'>{0}%</span>", percentCompleted);
				else
					strPercCompleted = percentCompleted + "%";

				retVal = string.Format(@"<table cellspacing='0' cellpadding='0'><tr><td><div class='progress'><img alt='' src='{0}' width='{1}%' /></div></td><td>{2}</td></tr></table>",
					image, percentCompleted, strPercCompleted);
			}

			return retVal;
		}

		protected string GetPriorityIcon(int ItemType, int PriorityId, string PName)
		{
			string retVal = "";
			if (ItemType > 0)
			{
				retVal = "<img src='{0}' alt='{1}' title='{1}'/>";
				switch (PriorityId)
				{
					case (int)Priority.Low:
						return String.Format(retVal, Page.ResolveUrl("~/Layouts/Images/icons/PriorityLow.gif"), PName + " " + LocRM.GetString("tPriority"));
					case (int)Priority.High:
						return String.Format(retVal, Page.ResolveUrl("~/Layouts/Images/icons/PriorityHigh.gif"), PName + " " + LocRM.GetString("tPriority"));
					case (int)Priority.VeryHigh:
						return String.Format(retVal, Page.ResolveUrl("~/Layouts/Images/icons/PriorityVeryHigh.gif"), PName + " " + LocRM.GetString("tPriority"));
					case (int)Priority.Urgent:
						return String.Format(retVal, Page.ResolveUrl("~/Layouts/Images/icons/PriorityUrgent.gif"), PName + " " + LocRM.GetString("tPriority"));
					default:
						return "";
				}
			}
			return retVal;
		}

		protected string GetManager(int ItemType, int ManagerId, int StateId, bool isOverdue)
		{
			string retVal = "";
			if (ItemType > 0)
			{
				if (StateId == (int)ObjectStates.Completed)
					retVal += String.Format("<span style='color:#999999;text-decoration:line-through'>{0}</span>", Util.CommonHelper.GetUserStatus(ManagerId, ""));
				else if (StateId == (int)ObjectStates.Overdue || isOverdue)
					retVal += Util.CommonHelper.GetUserStatus(ManagerId, "red");
				else
					retVal += Util.CommonHelper.GetUserStatus(ManagerId, "");
				return retVal;
			}
			else
				return "";
		}

		protected string GetProject(int ItemType, int ProjectId, string Title, int StateId, bool isOverdue)
		{
			string retVal = "";
			if (ItemType > 0 && ProjectId > 0)
			{
				if (StateId == (int)ObjectStates.Completed)
					retVal += String.Format("<span style='color:#999999;text-decoration:line-through'><a href='../Projects/ProjectView.aspx?ProjectId={0}'>{1}</a></span>", ProjectId, Title);
				else if (StateId == (int)ObjectStates.Overdue || isOverdue)
					retVal += String.Format("<a href='../Projects/ProjectView.aspx?ProjectId={0}'><font color='red'>{1}</font></a>", ProjectId, Title);
				else
					retVal += String.Format("<a href='../Projects/ProjectView.aspx?ProjectId={0}'>{1}</a>", ProjectId, Title);
				return retVal;
			}
			else
				return "";
		}

		protected string GetEditString(int ItemType, int ItemId)
		{
			string retVal = "";
			switch (ItemType)
			{
				case (int)ObjectTypes.Task:
					if (Task.CanUpdate(ItemId))
						retVal = String.Format("<a href='../Tasks/TaskEdit.aspx?TaskId={0}'><img alt='' title='{2}' src='{1}' /></a>", ItemId, Page.ResolveUrl("~/Layouts/Images/edit.gif"), LocRM.GetString("Edit"));
					break;
				case (int)ObjectTypes.ToDo:
					if (Mediachase.IBN.Business.ToDo.CanUpdate(ItemId))
						retVal = String.Format("<a href='../ToDo/ToDoEdit.aspx?ToDoId={0}'><img alt='' title='{2}' src='{1}' /></a>", ItemId, Page.ResolveUrl("~/Layouts/Images/edit.gif"), LocRM.GetString("Edit"));
					break;
				case (int)ObjectTypes.Document:
					if (Document.CanUpdate(ItemId))
						retVal = String.Format("<a href='../Documents/DocumentEdit.aspx?DocumentId={0}'><img alt='' title='{2}' src='{1}' /></a>", ItemId, Page.ResolveUrl("~/Layouts/Images/edit.gif"), LocRM.GetString("Edit"));
					break;
				case (int)ObjectTypes.Issue:
					if (Incident.CanUpdate(ItemId))
						retVal = String.Format("<a href='../Incidents/IncidentEdit.aspx?IncidentId={0}'><img alt='' title='{2}' src='{1}' /></a>", ItemId, Page.ResolveUrl("~/Layouts/Images/edit.gif"), LocRM.GetString("Edit"));
					break;
				case (int)ObjectTypes.CalendarEntry:
					if (CalendarEntry.CanUpdate(ItemId))
						retVal = String.Format("<a href='../Events/EventEdit.aspx?EventId={0}'><img alt='' title='{2}' src='{1}' /></a>", ItemId, Page.ResolveUrl("~/Layouts/Images/edit.gif"), LocRM.GetString("Edit"));
					break;
				default:
					break;
			}
			return retVal;
		}

		protected bool CanComplete(int itemType, int percentCompleted, int stateId)
		{
			bool retVal = false;

			if ((itemType == (int)ObjectTypes.Task || itemType == (int)ObjectTypes.ToDo)
				&& (stateId == (int)ObjectStates.Upcoming || stateId == (int)ObjectStates.Active || stateId == (int)ObjectStates.Overdue)
				&& percentCompleted < 100)
				retVal = true;

			if (itemType == (int)ObjectTypes.Document
				&& stateId == (int)ObjectStates.Active)
				retVal = true;

			if (itemType == (int)ObjectTypes.Issue
				&& (stateId == (int)ObjectStates.Active 
					|| stateId == (int)ObjectStates.Overdue
					|| stateId == (int)ObjectStates.ReOpen))
				retVal = true;

			return retVal;
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
			this.lbShowFilter.Click += new EventHandler(lbShowFilter_Click);
			this.lbHideFilter.Click += new EventHandler(lbShowFilter_Click);
			this.btnApplyF.Click += new EventHandler(btnApplyF_Click);
			this.btnResetF.Click += new EventHandler(btnResetF_Click);
			this.btnApplyG.Click += new EventHandler(btnApplyG_Click);
			this.btnVResetF.ServerClick += new EventHandler(btnVResetF_ServerClick);
			this.dgObjects.SortCommand += new DataGridSortCommandEventHandler(dgObjects_SortCommand);
			this.dgObjects.PageIndexChanged += new DataGridPageChangedEventHandler(dgObjects_PageIndexChanged);
			this.dgObjects.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(dgObjects_PageSizeChanged);
			this.lbExport.Click += new EventHandler(lbExport_Click);
			this.lbChangeViewDef.Click += new EventHandler(lbChangeViewDef_Click);
			this.lbChangeViewDates.Click += new EventHandler(lbChangeViewDates_Click);
			this.lbChangeViewTimes.Click += new EventHandler(lbChangeViewTimes_Click);
			this.CompleteButton.Click += new EventHandler(CompleteButton_Click);
		}
		#endregion

		#region dgEvents
		private void dgObjects_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (_pc["RV_Sort"] != null && _pc["RV_Sort"].ToString() == (string)e.SortExpression)
				_pc["RV_Sort"] = _pc["RV_Sort"] + " DESC";
			else
				_pc["RV_Sort"] = (string)e.SortExpression;
			BindDataGrid();
		}

		private void dgObjects_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			_pc["RV_Page"] = e.NewPageIndex.ToString();
			BindDataGrid();
		}

		private void dgObjects_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			_pc["RV_PageSize"] = e.NewPageSize.ToString();
			BindDataGrid();
		}
		#endregion

		#region Apply-Reset Filter/Grouping
		private void btnApplyF_Click(object sender, EventArgs e)
		{
			SaveValues();
			BindInfoTable();
			BindDataGrid();
		}

		private void btnResetF_Click(object sender, EventArgs e)
		{
			_pc["RV_Group"] = ((int)InternalSecureGroups.Intranet).ToString();
			BindDefaultValues();
			SaveValues();
			BindInfoTable();
			BindDataGrid();
		}

		private void btnApplyG_Click(object sender, EventArgs e)
		{
			string s = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(s))
				_pc["RV_Grouping"] = s;
			BindDataGrid();
		}

		private void btnVResetF_ServerClick(object sender, EventArgs e)
		{
			BindDefaultValues();
			SaveValues();
			BindInfoTable();
			BindDataGrid();
		}

		private void CompleteButton_Click(object sender, EventArgs e)
		{
			int id = int.Parse(hdnItemId.Value);
			int iTypeId = int.Parse(hdnItemType.Value);
			switch (iTypeId)
			{
				case (int)ObjectTypes.Task:
					Task.Tracking taskTracking = Task.GetTrackingInfo(id);
					if (taskTracking.ShowComplete)
						Task.CompleteTask(id);
					else if (taskTracking.ShowOverallStatus || taskTracking.ShowOverallStatusOnly)
						Task.UpdatePercent(id, 100);
					else if (taskTracking.ShowPersonalStatus || taskTracking.ShowPersonalStatusOnly)
						Task.UpdateResourcePercent(id, 100);
					break;
				case (int)ObjectTypes.ToDo:
					ToDo.Tracking todoTracking = ToDo.GetTrackingInfo(id);
					if (todoTracking.ShowComplete)
						ToDo.CompleteToDo(id);
					else if (todoTracking.ShowOverallStatus || todoTracking.ShowOverallStatusOnly)
						ToDo.UpdatePercent(id, 100);
					else if (todoTracking.ShowPersonalStatus || todoTracking.ShowPersonalStatusOnly)
						ToDo.UpdateResourcePercent(id, 100);
					break;
				case (int)ObjectTypes.Document:
					Document.Tracking docTracking = Document.GetTrackingInfo(id);
					if (docTracking.ShowComplete)
						Document.CompleteDocument(id);
					break;
				case (int)ObjectTypes.Issue:
					int incState = (int)ObjectStates.Upcoming;
					using (IDataReader reader = Incident.GetIncident(id))
					{
						if (reader.Read())
							incState = (int)reader["StateId"];
					}
					if (Incident.IsTransitionAllowed(id, (ObjectStates)incState, ObjectStates.Completed))
						Issue2.UpdateQuickTracking(id, String.Empty, (int)ObjectStates.Completed);
					else if (Incident.IsTransitionAllowed(id, (ObjectStates)incState, ObjectStates.OnCheck))
						Issue2.UpdateQuickTracking(id, String.Empty, (int)ObjectStates.OnCheck);
					break;
				default:
					break;
			} 
			Response.Redirect("../Projects/WorksForResource.aspx");
		}
		#endregion

		#region Show-Hide
		private void lbShowFilter_Click(object sender, EventArgs e)
		{
			if (_pc["RV_ShowFilter"] == null || !bool.Parse(_pc["RV_ShowFilter"]))
				_pc["RV_ShowFilter"] = "True";
			else
				_pc["RV_ShowFilter"] = "False";
			BindDataGrid();
		}
		#endregion

		#region Change View
		void lbChangeViewDates_Click(object sender, EventArgs e)
		{
			_pc["RV_ViewStyle"] = FieldSetName.tWorkResourcesDates.ToString();
			BindDataGrid();
		}

		void lbChangeViewDef_Click(object sender, EventArgs e)
		{
			_pc["RV_ViewStyle"] = FieldSetName.tWorkResourcesDefault.ToString();
			BindDataGrid();
		}

		void lbChangeViewTimes_Click(object sender, EventArgs e)
		{
			_pc["RV_ViewStyle"] = FieldSetName.tWorkResourcesWorkTime.ToString();
			BindDataGrid();
		}
		#endregion

		#region Export
		private void lbExport_Click(object sender, EventArgs e)
		{
			ExportGrid();
		}

		private void ExportGrid()
		{
			dgExport.Visible = true;
			BindDataGrid(dgExport);
			Util.CommonHelper.ExportExcel(dgExport, "Works.xls", null);
		}
		#endregion
	}
}
