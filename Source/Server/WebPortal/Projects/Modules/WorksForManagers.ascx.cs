namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Collections;
	using ComponentArt.Web.UI;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Reflection;
	using System.Resources;

	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Clients;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Core.Business;
	using Mediachase.Ibn.Web.UI.WebControls;
	using Mediachase.Ibn.Web.UI;
	using System.Text;

	/// <summary>
	///		Summary description for WorksForManagers.
	/// </summary>
	public partial class WorksForManagers : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", Assembly.GetExecutingAssembly());
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strSearch", Assembly.GetExecutingAssembly());
		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;
		protected enum FieldSetName
		{
			tWorkManagersDefault,
			tWorkManagersDates,
			tWorkManagersWorkTime
		}

		#region _shared
		private bool _shared
		{
			get
			{
				if (Request["Objs"] != null)
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
					return "";
			}
		}
		#endregion

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

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if (!Page.IsPostBack)
			{
				if (_pc["MV_Sort"] == null)
					_pc["MV_Sort"] = "Title";

				if (_pc["MV_ViewStyle"] == null)
					_pc["MV_ViewStyle"] = FieldSetName.tWorkManagersDefault.ToString();

				if (_pc["MV_Grouping"] == null)
					_pc["MV_Grouping"] = "0";

				if (!Configuration.ProjectManagementEnabled)
				{
					_pc["MV_Project"] = "0";
					_pc["MV_ShowTasks"] = false.ToString();
				}
				if (!Configuration.HelpDeskEnabled)
					_pc["MV_ShowIssues"] = false.ToString();

				if (!PortalConfig.GeneralAllowClientField)
					_pc["MV_ClientNew"] = "_";

				if (!PortalConfig.GeneralAllowGeneralCategoriesField)
					_pc["MV_Category"] = "0";
				//BindGroups();
				//BindDefaultValues();
			}
			//BindSavedValues();
			BindInfoTable();

			BindDataGrid();

			if (_projectId > 0)	// Used in Project View
			{
				trHeader.Visible = false;
				tblMain.Attributes["class"] = "text";
			}
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			//if ((Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) || Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)) && _projectId < 0)
			//    trManager.Visible = true;
			//else
			//    trManager.Visible = false;

			//if (_projectId > 0)
			//{
			//    trProject.Visible = false;
			//    trGroup.Visible = false;
			//}

			//FilterTable.Visible = false;// (_pc["MV_ShowFilter"] != null && bool.Parse(_pc["MV_ShowFilter"]));
			//tblFilterInfo.Visible = !FilterTable.Visible;

			//if (_pc["MV_ShowFilter"] == null || !bool.Parse(_pc["MV_ShowFilter"]))
			//{
			//lbShowFilter.Text = "<img align='absmiddle' border='0' title='" + LocRM.GetString("tShowFilter") + "' src='../Layouts/Images/scrolldown_hover.GIF' />";
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_ResUtil_FilterEdit");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("ProjectId", _projectId.ToString());
			cp.AddCommandArgument("Objs", Request["Objs"] != null ? Request["Objs"] : String.Empty);
			string cmd = cm.AddCommand("Project", "", "ProjectView", cp);
			cmd = cmd.Replace("\"", "&quot;");
			//lblShowFilter.Text = String.Format("<a href=\"javascript:{{{0}}}\"><img align='absmiddle' border='0' title='{1}' src='{2}' /></a>",
			//    cmd, LocRM.GetString("tShowFilter"), Page.ResolveUrl("~/Layouts/Images/scrolldown_hover.GIF"));
			lblShowFilter.Text = String.Format("<a href=\"javascript:{{{0}}}\">{1}</a>",
				cmd, LocRM.GetString("tShowFilter"));
			//}
			//else
			//    lbHideFilter.Text = "<img align='absmiddle' border='0' title='" + LocRM.GetString("tHideFilter") + "' src='../Layouts/Images/scrollup_hover.GIF' />";

			//if (_shared)
			//{
			//    trUser.Visible = false;
			//    if (_sType != "All")
			//        tdObjs.Visible = false;
			//}

			//if (_projectId <= 0)
			//    trProject.Visible = Configuration.ProjectManagementEnabled;
			if (Mediachase.Ibn.Web.UI.CHelper.NeedToBindGrid())
				BindDataGrid();
			BindToolbar();
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			//btnApplyF.Text = LocRM.GetString("Apply");
			//btnResetF.Text = LocRM.GetString("Reset");
			btnVResetF.Value = LocRM.GetString("ResetFilter");

			//cbCalEntries.Text = LocRM.GetString("tEvents");
			//cbIssues.Text = LocRM.GetString("tIssues");
			//cbDocs.Text = LocRM.GetString("tDocuments");
			//cbTasks.Text = LocRM.GetString("tTasks");
			//cbToDo.Text = LocRM.GetString("tToDos");
			//cbChkAll.Text = "&nbsp;" + LocRM.GetString("tObjects");

			//ChildTodoCheckbox.Text = LocRM.GetString("ShowChildTodo");
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("GroupId", typeof(string));
			dt.Columns.Add("GroupName", typeof(string));
			DataRow dr;
			// Any

			dr = dt.NewRow();
			dr["GroupId"] = "1";
			dr["GroupName"] = LocRM.GetString("AllFem");
			dt.Rows.Add(dr);

			using (IDataReader reader = SecureGroup.GetListGroupsAsTree())
			{
				while (reader.Read())
				{
					string groupName = CommonHelper.GetResFileString(reader["GroupName"].ToString());
					string GroupId = reader["GroupId"].ToString();
					int Level = (int)reader["Level"];
					for (int i = 1; i < Level; i++)
						groupName = "  " + groupName;
					dr = dt.NewRow();
					dr["GroupId"] = GroupId.ToString();
					dr["GroupName"] = groupName;
					dt.Rows.Add(dr);
				}
			}

			//Saved Value
			if (_pc["MV_Group"] == null)
				_pc["MV_Group"] = ((int)InternalSecureGroups.Intranet).ToString();

			DataRow[] mas = dt.Select(String.Format("GroupId = '{0}'", _pc["MV_Group"]));
			if (mas.Length == 0)
				_pc["MV_Group"] = dt.Rows[0]["GroupId"].ToString();

			//Users Binding
			BindUsers(int.Parse(_pc["MV_Group"]));
		}
		#endregion

		#region BindUsers
		private void BindUsers(int GroupId)
		{
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

			//Saved Value
			if (_pc["MV_User"] == null)
				_pc["MV_User"] = dt.Rows[0]["UserId"].ToString();

			DataRow[] mas = dt.Select(String.Format("UserId = '{0}'", _pc["MV_User"]));
			if (mas.Length == 0)
				_pc["MV_User"] = dt.Rows[0]["UserId"].ToString();
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

		#region BindInfoTable
		private void BindInfoTable()
		{
			tblFilterInfoSet.Rows.Clear();

			BindGroups();
			// Group
			if (_pc["MV_Group"] != null && _projectId < 0)
			{
				AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tGroup")), CommonHelper.GetResFileString(SecureGroup.GetGroupName(int.Parse(_pc["MV_Group"]))));
			}

			if (!_shared)
			{
				//User
				if (_pc["MV_User"] != null && _pc["MV_User"] != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tUser")), CommonHelper.GetUserStatusPureName(int.Parse(_pc["MV_User"])));
				}
			}

			// Project
			if (_projectId <= 0 && _pc["MV_Project"] != null && _pc["MV_Project"] != "0")
			{
				string Txt = Task.GetProjectTitle(int.Parse(_pc["MV_Project"]));
				if (_projectId > 0)
					Txt = String.Format("<span style='color:red'>{0}</span>", Txt);

				AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tProject")), Txt);
			}

			// Manager
			if (_pc["MV_Manager"] != null && _projectId < 0 && _pc["MV_Manager"] != "0")
			{
				AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tManager")), CommonHelper.GetUserStatusPureName(int.Parse(_pc["MV_Manager"])));
			}

			//Client
			if (_pc["MV_ClientNew"] != null)
			{
				string ss = _pc["MV_ClientNew"];
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

			// Category
			if (_pc["MV_Category"] != null && _pc["MV_Category"] != "0")
			{
				string txt = Common.GetGeneralCategory(int.Parse(_pc["MV_Category"]));
				AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Category")), txt);
			}

			//State
			string state = "";
			if (_pc["MV_Completed"] != null && _pc["MV_Completed"] != "0")
			{
				switch (_pc["MV_Completed"])
				{
					case "-1":
						state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowCompleted"), LocRM.GetString("tForDay"));
						break;
					case "-7":
						state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowCompleted"), LocRM.GetString("tForWeek"));
						break;
					case "7":
						state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowCompleted"), LocRM.GetString("tLastWeek"));
						break;
					case "-30":
						state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowCompleted"), LocRM.GetString("tForMonth"));
						break;
					case "30":
						state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowCompleted"), LocRM.GetString("tLastMonth"));
						break;
					case "-11000":
						state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowCompleted"), LocRM.GetString("tForAnyPeriod"));
						break;
					default:
						break;
				}
			}
			if (_pc["MV_Active"] == null || bool.Parse(_pc["MV_Active"]))
			{
				if (state != "")
					state += ", ";
				state += LocRM.GetString("tActive");
			}

			if (_pc["MV_Upcoming"] != null && _pc["MV_Upcoming"] != "0")
			{
				if (state != "")
					state += ", ";
				switch (_pc["MV_Upcoming"])
				{
					case "1":
						state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowUpcoming"), LocRM.GetString("tInDay"));
						break;
					case "7":
						state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowUpcoming"), LocRM.GetString("tInWeek"));
						break;
					case "-7":
						state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowUpcoming"), LocRM.GetString("tNextWeek"));
						break;
					case "30":
						state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowUpcoming"), LocRM.GetString("tInMonth"));
						break;
					case "-30":
						state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowUpcoming"), LocRM.GetString("tNextMonth"));
						break;
					case "11000":
						state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowUpcoming"), LocRM.GetString("tInAnyPeriod"));
						break;
					default:
						break;
				}
			}
			if (state != "")
				AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("State")), state);

			if (!_shared || _sType == "All")
			{
				// Objects
				string Objs = "";
				if (_pc["MV_ShowEvents"] == null || bool.Parse(_pc["MV_ShowEvents"]))
				{
					if (Objs != "")
						Objs += ", ";
					Objs += LocRM.GetString("tEvents");
				}
				if (_pc["MV_ShowIssues"] == null || bool.Parse(_pc["MV_ShowIssues"]))
				{
					if (Objs != "")
						Objs += ", ";
					Objs += LocRM.GetString("tIssues");
				}
				if (_pc["MV_ShowDocs"] == null || bool.Parse(_pc["MV_ShowDocs"]))
				{
					if (Objs != "")
						Objs += ", ";
					Objs += LocRM.GetString("tDocuments");
				}
				if (_pc["MV_ShowTasks"] == null || bool.Parse(_pc["MV_ShowTasks"]))
				{
					if (Objs != "")
						Objs += ", ";
					Objs += LocRM.GetString("tTasks");
				}
				if (_pc["MV_ShowToDo"] == null || bool.Parse(_pc["MV_ShowToDo"]))
				{
					if (Objs != "")
						Objs += ", ";
					Objs += LocRM.GetString("tToDos");
				}
				if (Objs != "")
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tShow")), Objs);
			}

			if (_pc["MV_Created"] != null && _pc["MV_Created"] != "0")
			{
				string created = string.Empty;
				switch (_pc["MV_Created"])
				{
					case "1":	// Today
						created = CHelper.UpperFirstChar(LocRM2.GetString("tToday"));
						break;
					case "2":	// Yesterday
						created = CHelper.UpperFirstChar(LocRM2.GetString("tYesterday"));
						break;
					case "3":	// ThisWeek
						created = CHelper.UpperFirstChar(LocRM2.GetString("tThisWeek"));
						break;
					case "4":	// LastWeek
						created = CHelper.UpperFirstChar(LocRM2.GetString("tLastWeek"));
						break;
					case "5":	// ThisMonth
						created = CHelper.UpperFirstChar(LocRM2.GetString("tThisMonth"));
						break;
					case "6":	// LastMonth
						created = CHelper.UpperFirstChar(LocRM2.GetString("tLastMonth"));
						break;
					case "7":	// ThisYear
						created = CHelper.UpperFirstChar(LocRM2.GetString("tThisYear"));
						break;
					case "8":	// LastYear
						created = CHelper.UpperFirstChar(LocRM2.GetString("tLastYear"));
						break;
					case "9":	// Priod
						if (!String.IsNullOrEmpty(_pc["MV_CreatedFrom"]) && !String.IsNullOrEmpty(_pc["MV_CreatedTo"]))
							created = String.Format(CultureInfo.InvariantCulture,
								"{0} - {1}",
								DateTime.Parse(_pc["MV_CreatedFrom"], CultureInfo.InvariantCulture).ToShortDateString(),
								DateTime.Parse(_pc["MV_CreatedTo"], CultureInfo.InvariantCulture).ToShortDateString());
						break;
					default:
						break;
				}
				if (!String.IsNullOrEmpty(created))
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("CreatedPeriod")), created);
			}
		}

		private void AddRow(string filterName, string filterValue)
		{
			HtmlTableRow tr = new HtmlTableRow();
			HtmlTableCell td1 = new HtmlTableCell();
			td1.Align = "Right";
			td1.Width = "120px";
			td1.InnerHtml = filterName;
			HtmlTableCell td2 = new HtmlTableCell();
			td2.InnerHtml = filterValue;
			tr.Cells.Add(td1);
			tr.Cells.Add(td2);
			tblFilterInfoSet.Rows.Add(tr);
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tResUtiliz");
			if (_shared)
				switch (_sType)
				{
					case "CalEntry":
						secHeader.Title += "&nbsp;-&nbsp;" + LocRM.GetString("tEvents");
						break;
					case "Issue":
						secHeader.Title += "&nbsp;-&nbsp;" + LocRM.GetString("tIssues");
						break;
					case "Doc":
						secHeader.Title += "&nbsp;-&nbsp;" + LocRM.GetString("tDocuments");
						break;
					case "Task":
						secHeader.Title += "&nbsp;-&nbsp;" + LocRM.GetString("tTasks");
						break;
					case "Todo":
						secHeader.Title += "&nbsp;-&nbsp;" + LocRM.GetString("tToDos");
						break;
					default:
						break;
				}
			if (_projectId > 0)
			{
				if (this.Parent.Parent is IToolbarLight || this.Parent is IToolbarLight)
				{
					BlockHeaderLightWithMenu secHeader2;
					if (this.Parent.Parent is IToolbarLight)
						secHeader2 = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent.Parent).GetToolBar();
					else
						secHeader2 = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent).GetToolBar();

					secHeader2.ClearRightItems();

					RenderMenu(secHeader2.ActionsMenu);

					secHeader2.EnsureRender();
				}
			}
			else
				RenderMenu(secHeader.ActionsMenu);
		}

		private void RenderMenu(ComponentArt.Web.UI.Menu actionsMenu)
		{
			actionsMenu.Items.Clear();

			#region View Menu Items
			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = LocRM.GetString("tView");
			topMenuItem.Look.LeftIconUrl = Page.ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;
			string sCurrentView = _pc["MV_ViewStyle"];

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (sCurrentView == FieldSetName.tWorkManagersDefault.ToString())
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewDef, "");
			subItem.Text = LocRM.GetString(FieldSetName.tWorkManagersDefault.ToString());
			topMenuItem.Items.Add(subItem);

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (sCurrentView == FieldSetName.tWorkManagersDates.ToString())
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewDates, "");
			subItem.Text = LocRM.GetString(FieldSetName.tWorkManagersDates.ToString());
			topMenuItem.Items.Add(subItem);

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (sCurrentView == FieldSetName.tWorkManagersWorkTime.ToString())
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewTimes, "");
			subItem.Text = LocRM.GetString(FieldSetName.tWorkManagersWorkTime.ToString());
			topMenuItem.Items.Add(subItem);
			#endregion

			actionsMenu.Items.Add(topMenuItem);

			if (_shared)
				return;

			#region Grouping Menu Items
			topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = LocRM.GetString("tGroupBy");
			topMenuItem.Look.LeftIconUrl = Page.ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			sCurrentView = _pc["MV_Grouping"];

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
			if (sCurrentView == "1")
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(btnApplyG, "1");
			subItem.Text = LocRM.GetString("tByUser");
			topMenuItem.Items.Add(subItem);

			if (_projectId <= 0)
			{
				if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
					Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
				{
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
				}
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
			}
			#endregion

			actionsMenu.Items.Add(topMenuItem);
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			int i = 2;
			dgObjects.Columns[i++].HeaderText = "!!!";
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("Title");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("Type");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tResources");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("PercentCompleted");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("StartDate");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("FinishDate2");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tActualStartDate");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tActualFinishDate");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("CreationDate");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tTaskTime");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tTotalMinutes");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tTotalApproved");

			if (_pc["MV_Group"] == null)
				_pc["MV_Group"] = ((int)InternalSecureGroups.Intranet).ToString();
			if (_pc["MV_User"] == null)
				_pc["MV_User"] = "0";
			int principalId = (_pc["MV_User"] == "0") ? int.Parse(_pc["MV_Group"]) : int.Parse(_pc["MV_User"]);
			int prjId = _pc["MV_Project"] == null ? 0 : int.Parse(_pc["MV_Project"]);
			int managerId = _pc["MV_Manager"] == null ? 0 : int.Parse(_pc["MV_Manager"]);

			// O.R. [2010-10-08] Security fix
			if (managerId == 0 && !Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) &&
				!Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
			{
				managerId = Security.CurrentUser.UserID;
				_pc["MV_Manager"] = managerId.ToString();
			}

			int categoryId = _pc["MV_Category"] == null ? 0 : int.Parse(_pc["MV_Category"]);
			bool showActive = _pc["MV_Active"] == null ? true : bool.Parse(_pc["MV_Active"]);

			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			//Client
			if (_pc["MV_ClientNew"] != null)
			{
				string ss = _pc["MV_ClientNew"];
				if (ss.IndexOf("_") >= 0)
				{
					string stype = ss.Substring(0, ss.IndexOf("_"));
					if (stype.ToLower() == "org")
						orgUid = PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1));
					else if (stype.ToLower() == "contact")
						contactUid = PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1));
				}
			}

			DateTime dtCompleted1 = DateTime.MinValue;
			DateTime dtCompleted2 = DateTime.MinValue;
			int iCompleted = _pc["MV_Completed"] == null ? 0 : int.Parse(_pc["MV_Completed"]);
			if (iCompleted < 0)
			{
				dtCompleted1 = UserDateTime.UserToday.AddDays(iCompleted);
				dtCompleted2 = UserDateTime.UserToday.AddDays(1);
			}
			else if (iCompleted == 7)
			{
				dtCompleted1 = UserDateTime.UserToday.AddDays(1 - (int)UserDateTime.UserNow.DayOfWeek - 7);
				dtCompleted2 = UserDateTime.UserToday.AddDays(1 - (int)UserDateTime.UserNow.DayOfWeek);
			}
			else if (iCompleted == 30)
			{
				dtCompleted1 = UserDateTime.UserToday.AddDays(1 - UserDateTime.UserNow.Day).AddMonths(-1);
				dtCompleted2 = UserDateTime.UserToday.AddDays(1 - UserDateTime.UserNow.Day);
			}

			DateTime dtUpcoming1 = DateTime.MaxValue;
			DateTime dtUpcoming2 = DateTime.MaxValue;
			int iUpcoming = _pc["MV_Upcoming"] == null ? 0 : int.Parse(_pc["MV_Upcoming"]);
			if (iUpcoming > 0)
			{
				// O.R. [2009-09-07]: Not started items fix
				dtUpcoming1 = UserDateTime.UserNow.AddYears(-50);
				dtUpcoming2 = UserDateTime.UserToday.Date.AddDays(iUpcoming + 1);
			}
			else if (iUpcoming == -7)
			{
				dtUpcoming1 = UserDateTime.UserToday.AddDays(1 - (int)UserDateTime.UserNow.DayOfWeek + 7);
				dtUpcoming2 = UserDateTime.UserToday.AddDays(1 - (int)UserDateTime.UserNow.DayOfWeek + 14);
			}
			else if (iUpcoming == -30)
			{
				dtUpcoming1 = UserDateTime.UserToday.AddDays(1 - UserDateTime.UserNow.Day).AddMonths(1);
				dtUpcoming2 = UserDateTime.UserToday.AddDays(1 - UserDateTime.UserNow.Day).AddMonths(2);
			}

			DateTime dtCreated1 = DateTime.MinValue;
			DateTime dtCreated2 = DateTime.MinValue;
			if (!String.IsNullOrEmpty(_pc["MV_Created"]) && _pc["MV_Created"] != "0")
			{
				switch (_pc["MV_Created"])
				{
					case "1":	// Today
						dtCreated1 = UserDateTime.UserToday;
						dtCreated2 = UserDateTime.UserNow;
						break;
					case "2":	// Yesterday
						dtCreated1 = UserDateTime.UserToday.AddDays(-1);
						dtCreated2 = UserDateTime.UserToday;
						break;
					case "3":	// ThisWeek
						dtCreated1 = UserDateTime.UserToday.AddDays(1 - (int)DateTime.Now.DayOfWeek);
						dtCreated2 = UserDateTime.UserNow;
						break;
					case "4":	// LastWeek
						dtCreated1 = UserDateTime.UserToday.AddDays(1 - (int)DateTime.Now.DayOfWeek - 7);
						dtCreated2 = UserDateTime.UserToday.AddDays(1 - (int)DateTime.Now.DayOfWeek);
						break;
					case "5":	// ThisMonth
						dtCreated1 = UserDateTime.UserToday.AddDays(1 - DateTime.Now.Day);
						dtCreated2 = UserDateTime.UserNow;
						break;
					case "6":	// LastMonth
						dtCreated1 = UserDateTime.UserToday.AddDays(1 - DateTime.Now.Day).AddMonths(-1);
						dtCreated2 = UserDateTime.UserToday.AddDays(1 - DateTime.Now.Day);
						break;
					case "7":	// ThisYear
						dtCreated1 = UserDateTime.UserToday.AddDays(1 - DateTime.Now.DayOfYear);
						dtCreated2 = UserDateTime.UserNow;
						break;
					case "8":	// LastYear
						dtCreated1 = UserDateTime.UserToday.AddDays(1 - DateTime.Now.DayOfYear).AddYears(-1);
						dtCreated2 = UserDateTime.UserToday.AddDays(1 - DateTime.Now.DayOfYear);
						break;
					case "9":	// Period
						if (_pc["MV_CreatedFrom"] != null)
							dtCreated1 = DateTime.Parse(_pc["MV_CreatedFrom"], CultureInfo.InvariantCulture);
						if (_pc["MV_CreatedTo"] != null)
							dtCreated2 = DateTime.Parse(_pc["MV_CreatedTo"], CultureInfo.InvariantCulture);
						break;
					default:
						break;
				}
			}

			int groupBy = int.Parse(_pc["MV_Grouping"]);
			if (_shared)
			{
				groupBy = 1;
				principalId = int.Parse(_pc["MV_Group"]);
			}

			ArrayList alTypes = new ArrayList();
			if (!_shared || _sType == "All")
			{
				if (_pc["MV_ShowEvents"] == null || bool.Parse(_pc["MV_ShowEvents"]))
					alTypes.Add((int)ObjectTypes.CalendarEntry);
				if (_pc["MV_ShowIssues"] == null || bool.Parse(_pc["MV_ShowIssues"]))
					alTypes.Add((int)ObjectTypes.Issue);
				if (_pc["MV_ShowDocs"] == null || bool.Parse(_pc["MV_ShowDocs"]))
					alTypes.Add((int)ObjectTypes.Document);
				if (_pc["MV_ShowTasks"] == null || bool.Parse(_pc["MV_ShowTasks"]))
					alTypes.Add((int)ObjectTypes.Task);
				if (_pc["MV_ShowToDo"] == null || bool.Parse(_pc["MV_ShowToDo"]))
					alTypes.Add((int)ObjectTypes.ToDo);
			}
			else
				switch (_sType)
				{
					case "CalEntry":
						alTypes.Add((int)ObjectTypes.CalendarEntry);
						break;
					case "Issue":
						alTypes.Add((int)ObjectTypes.Issue);
						break;
					case "Doc":
						alTypes.Add((int)ObjectTypes.Document);
						break;
					case "Task":
						alTypes.Add((int)ObjectTypes.Task);
						break;
					case "Todo":
						alTypes.Add((int)ObjectTypes.ToDo);
						break;
					default:
						break;
				}

			if (_projectId > 0)
			{
				// Ensure no grouping or grouping by user only (exclude by project and by manager)
				if (groupBy != 0 && groupBy != 1)
				{
					groupBy = 0;
					_pc["MV_Grouping"] = "0";
				}

				/*				if (_pc["TasksProjectActivities_CurrentTab"] == "ResourceView")
									groupBy = 1;
								else
									groupBy = 0;
				 */

				if (_pc["MV_User"] == "0")
					principalId = 1; // EveryOne
				else
					principalId = int.Parse(_pc["MV_User"]);

				managerId = 0;	// Any
				prjId = _projectId;
			}

			//Sort triangle
			if (_pc["MV_ShowChildTodo"] == null)
				_pc["MV_ShowChildTodo"] = false.ToString();
			if (groupBy == 0 && !bool.Parse(_pc["MV_ShowChildTodo"]))
				foreach (DataGridColumn dgc in dgObjects.Columns)
				{
					if (dgc.SortExpression == _pc["MV_Sort"].ToString())
						dgc.HeaderText += String.Format("&nbsp;<img alt='' src='{0}'/>",
							Page.ResolveUrl("~/Layouts/Images/upbtnF.jpg"));
					else if (dgc.SortExpression + " DESC" == _pc["MV_Sort"].ToString())
						dgc.HeaderText += String.Format("&nbsp;<img alt='' src='{0}'/>",
							Page.ResolveUrl("~/Layouts/Images/downbtnF.jpg"));
				}

			DataTable dt = Mediachase.IBN.Business.ToDo.GetGroupedItemsForManagerViewDataTable(
				groupBy, principalId, managerId, prjId, categoryId, showActive, alTypes,
				dtCompleted1, dtCompleted2,
				dtUpcoming1, dtUpcoming2,
				dtCreated1, dtCreated2,
				orgUid, contactUid, bool.Parse(_pc["MV_ShowChildTodo"]));

			DataView dv = dt.DefaultView;
			if (groupBy == 0)
			{
				if (bool.Parse(_pc["MV_ShowChildTodo"]))
				{
					dv.Sort = "ContainerName, ContainerType, IsChildToDo, Title";
					dgObjects.AllowSorting = false;
				}
				else
				{
					if (_pc["MV_Sort"] == null)
						_pc["MV_Sort"] = "Title";

					dv.Sort = _pc["MV_Sort"];
					dgObjects.AllowSorting = true;
				}
			}
			else
			{
				dgObjects.AllowSorting = false;
			}

			dgObjects.DataSource = dv;

			if (_pc["MV_PageSize"] != null)
				dgObjects.PageSize = int.Parse(_pc["MV_PageSize"]);

			if (_pc["MV_Page"] != null)
			{
				int pageindex = int.Parse(_pc["MV_Page"]);
				int ppi = dv.Count / dgObjects.PageSize;
				if (dv.Count % dgObjects.PageSize == 0)
					ppi = ppi - 1;

				if (pageindex <= ppi)
					dgObjects.CurrentPageIndex = pageindex;
				else
					dgObjects.CurrentPageIndex = 0;
			}

			// FieldSet
			string fieldSet = _pc["MV_ViewStyle"];

			int j = 5;
			switch (fieldSet)
			{
				case "tWorkManagersDates":
					dgObjects.Columns[j++].Visible = false;	//Resources
					dgObjects.Columns[j++].Visible = false;	//PercentCompleted
					dgObjects.Columns[j++].Visible = true;	//StartDate
					dgObjects.Columns[j++].Visible = true;	//FinishDate
					dgObjects.Columns[j++].Visible = true;	//ActualStartDate
					dgObjects.Columns[j++].Visible = true;	//ActualFinishDate
					dgObjects.Columns[j++].Visible = true;	//CreationDate
					dgObjects.Columns[j++].Visible = false;	//TaskTime
					dgObjects.Columns[j++].Visible = false;	//TotalMinutes
					dgObjects.Columns[j++].Visible = false;	//TotalApproved
					break;
				case "tWorkManagersWorkTime":
					dgObjects.Columns[j++].Visible = false;	//Resources
					dgObjects.Columns[j++].Visible = false;	//PercentCompleted
					dgObjects.Columns[j++].Visible = false;	//StartDate
					dgObjects.Columns[j++].Visible = false;	//FinishDate
					dgObjects.Columns[j++].Visible = false;	//ActualStartDate
					dgObjects.Columns[j++].Visible = false;	//ActualFinishDate
					dgObjects.Columns[j++].Visible = false;	//CreationDate
					dgObjects.Columns[j++].Visible = true;	//TaskTime
					dgObjects.Columns[j++].Visible = true;	//TotalMinutes
					dgObjects.Columns[j++].Visible = true;	//TotalApproved
					break;
				default:	//"tWorkManagersDefault"
					dgObjects.Columns[j++].Visible = true;	//Resources
					dgObjects.Columns[j++].Visible = true;	//PercentCompleted
					dgObjects.Columns[j++].Visible = true;	//StartDate
					dgObjects.Columns[j++].Visible = true;	//FinishDate
					dgObjects.Columns[j++].Visible = false;	//ActualStartDate
					dgObjects.Columns[j++].Visible = false;	//ActualFinishDate
					dgObjects.Columns[j++].Visible = false;	//CreationDate
					dgObjects.Columns[j++].Visible = false;	//TaskTime
					dgObjects.Columns[j++].Visible = false;	//TotalMinutes
					dgObjects.Columns[j++].Visible = false;	//TotalApproved
					break;
			}

			dgObjects.DataBind();

			foreach (DataGridItem dgi in dgObjects.Items)
			{
				if (int.Parse(dgi.Cells[0].Text) == 0)
					dgi.BackColor = Color.FromArgb(0xBB, 0xBB, 0xBB);
				else if (int.Parse(dgi.Cells[1].Text) == (int)ObjectStates.Upcoming)
					dgi.BackColor = Color.FromArgb(0xE9, 0xFE, 0xEC);
				else if (int.Parse(dgi.Cells[1].Text) == (int)ObjectStates.Suspended || int.Parse(dgi.Cells[1].Text) == (int)ObjectStates.Completed)
					dgi.BackColor = Color.FromArgb(0xF2, 0xF2, 0xF2);
			}
		}
		#endregion

		#region Protected DG strings
		protected string GetTitle(int itemId, string title, int itemType, object groupName, int stateId, bool isChildTodo, bool isOverdue, bool isNewMessage)
		{
			string retval = string.Empty;

			if (itemId == 0)
			{
				retval = string.Format(CultureInfo.InvariantCulture, "<span style='font-weight:bold'>{0}</span>", groupName.ToString());
			}
			else
			{
				bool itemIsOverdue = false;
				string page = null;
				string icon = null;
				string childIcon = null;

				switch (itemType)
				{
					#region Task
					case (int)ObjectTypes.Task:
						title += " (#" + itemId + ")";

						if (Task.CanRead(itemId))
							page = "~/Tasks/TaskView.aspx?TaskId";

						if (stateId == (int)ObjectStates.Completed)
							icon = "task1_completed.gif";
						else if (stateId == (int)ObjectStates.Overdue || isOverdue)
						{
							itemIsOverdue = true;
							icon = "task1_overdue.gif";
						}
						else if (stateId == (int)ObjectStates.Suspended)
							icon = "task1_suspensed.gif";
						else
							icon = "task1.gif";

						break;
					#endregion
					#region ToDo
					case (int)ObjectTypes.ToDo:
						title += " (#" + itemId + ")";

						if (Mediachase.IBN.Business.ToDo.CanRead(itemId))
							page = "~/ToDo/ToDoView.aspx?ToDoId";

						if (stateId == (int)ObjectStates.Completed)
							icon = "task_completed.gif";
						else if (stateId == (int)ObjectStates.Overdue || isOverdue)
						{
							itemIsOverdue = true;
							icon = "task_overdue.gif";
						}
						else if (stateId == (int)ObjectStates.Suspended)
							icon = "task_suspensed.gif";
						else
							icon = "task.gif";

						// Child todo
						if (_pc["MV_ShowChildTodo"] != null && bool.Parse(_pc["MV_ShowChildTodo"]) && isChildTodo)
							childIcon = Page.ResolveUrl("~/Layouts/Images/lines/l.gif");

						break;
					#endregion
					#region Document
					case (int)ObjectTypes.Document:
						title += " (#" + itemId + ")";

						if (Document.CanRead(itemId))
							page = "~/Documents/DocumentView.aspx?DocumentId";

						if (stateId == (int)ObjectStates.Completed)
							icon = "document_completed.gif";
						else if (stateId == (int)ObjectStates.Overdue || isOverdue)
						{
							itemIsOverdue = true;
							icon = "document.gif";
						}
						else if (stateId == (int)ObjectStates.Suspended)
							icon = "document_suspensed.gif";
						else
							icon = "document.gif";

						break;
					#endregion
					#region Issue
					case (int)ObjectTypes.Issue:
						title += " (#" + itemId + ")";

						if (Incident.CanRead(itemId))
							page = "~/Incidents/IncidentView.aspx?IncidentId";

						if (stateId == (int)ObjectStates.Completed)
							icon = "incident_closed.gif";
						else if (isOverdue)
						{
							itemIsOverdue = true;
							icon = "incident.gif";
						}
						else
							icon = "incident.gif";

						break;
					#endregion
					#region CalendarEntry
					case (int)ObjectTypes.CalendarEntry:
						if (CalendarEntry.CanRead(itemId))
							page = "~/Events/EventView.aspx?EventId";

						if (stateId == (int)ObjectStates.Completed)
							icon = "event_completed.gif";
						else if (stateId == (int)ObjectStates.Overdue || isOverdue)
						{
							itemIsOverdue = true;
							icon = "event.gif";
						}
						else
							icon = "event.gif";

						break;
					#endregion
				}

				if (!string.IsNullOrEmpty(icon))
				{
					icon = Page.ResolveUrl("~/Layouts/Images/icons/" + icon);

					if (!string.IsNullOrEmpty(page))
					{
						page = Page.ResolveUrl(page);

						if (stateId == (int)ObjectStates.Completed)
							retval = string.Format("<span style='color:#999999;text-decoration:line-through;'><a href='{0}={1}'><img alt='' src='{2}' /> {3}</a></span>",
								page, itemId, icon, title);
						else if (itemIsOverdue)
							retval = string.Format("<a href='{0}={1}'><span style='color:red'><img alt='' src='{2}' /> {3}</span></a>",
								page, itemId, icon, title);
						else
							retval = string.Format("<a href='{0}={1}'><img alt='' src='{2}' /> {3}</a>",
								page, itemId, icon, title);
					}
					else
					{
						if (stateId == (int)ObjectStates.Completed)
							retval = string.Format("<span style='color:#999999;text-decoration:line-through;'><img alt='' src='{0}' /> {1}</span>",
								icon, title);
						else if (itemIsOverdue)
							retval = string.Format("<span style='color:red'><img alt='' src='{0}' /> {1}</span>",
								icon, title);
						else
							retval = string.Format("<img alt='' src='{0}' /> {1}",
								icon, title);
					}

					if (!string.IsNullOrEmpty(childIcon))
						retval = string.Format(CultureInfo.InvariantCulture, "<img alt='' src='{0}' align='absmiddle' /> {1}",
							childIcon, retval);
				}

				if (isNewMessage)
					retval = string.Format(CultureInfo.InvariantCulture, "<span style='font-weight:bold'>{0}</span>", retval);
			}

			return retval;
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
						type = LocRM.GetString("tTask");
						break;
					case (int)ObjectTypes.ToDo:
						type = LocRM.GetString("tTodo");
						break;
					case (int)ObjectTypes.Document:
						type = LocRM.GetString("tDoc");
						break;
					case (int)ObjectTypes.Issue:
						type = LocRM.GetString("tIssue");
						break;
					case (int)ObjectTypes.CalendarEntry:
						type = LocRM.GetString("tCalEntry");
						break;
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

		protected string GetDate(object _date, int stateId, bool isOverdue)
		{
			if (_date != DBNull.Value)
			{
				if (stateId == (int)ObjectStates.Completed)
					return String.Format("<span style='color:#999999;text-decoration:line-through'><span style='color:black'>{0}</span></span>", ((DateTime)_date).ToShortDateString());
				else if (stateId == (int)ObjectStates.Overdue || isOverdue)
					return String.Format("<span style='color:red'>{0}</span>", ((DateTime)_date).ToShortDateString());
				else
					return ((DateTime)_date).ToShortDateString();
			}
			else
				return "";
		}

		protected string GetResources(int CType, int itemType, int itemId, int stateId, bool isOverdue)
		{
			StringBuilder builder = new StringBuilder();

			if (itemType > 0)
			{
				bool hasResources = false;
				switch (itemType)
				{
					case (int)ObjectTypes.Task:
						using (IDataReader reader = Task.GetListResources(itemId))
						{
							while (reader.Read())
							{
								hasResources = true;

								if (stateId == (int)ObjectStates.Completed)
									builder.AppendFormat("<tr><td><span style='color:#999999;text-decoration:line-through'>{0}</span></td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], ""), (CType == (int)CompletionType.All) ? ("<td style='padding-left:7'><span style='color:#999999;text-decoration:line-through'><span style='color:black'>" + reader["PercentCompleted"].ToString() + "%</span></span></td>") : "");
								else if (stateId == (int)ObjectStates.Overdue || isOverdue)
									builder.AppendFormat("<tr><td>{0}</td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], "red"), (CType == (int)CompletionType.All) ? "<td style='padding-left:7'><span style='color:red'>" + reader["PercentCompleted"].ToString() + "%</span></td>" : "");
								else
									builder.AppendFormat("<tr><td>{0}</td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], ""), (CType == (int)CompletionType.All) ? "<td style='padding-left:7'>" + reader["PercentCompleted"].ToString() + "%</td>" : "");
							}
						}
						break;
					case (int)ObjectTypes.ToDo:
						using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetListResources(itemId))
						{
							while (reader.Read())
							{
								hasResources = true;

								if (stateId == (int)ObjectStates.Completed)
									builder.AppendFormat("<tr><td><span style='color:#999999;text-decoration:line-through'>{0}</span></td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], ""), (CType == (int)CompletionType.All) ? ("<td style='padding-left:7'><span style='color:#999999;text-decoration:line-through'><span style='color:black'>" + reader["PercentCompleted"].ToString() + "%</span></span></td>") : "");
								else if (stateId == (int)ObjectStates.Overdue || isOverdue)
									builder.AppendFormat("<tr><td>{0}</td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], "red"), (CType == (int)CompletionType.All) ? "<td style='padding-left:7'><span style='color:red'>" + reader["PercentCompleted"].ToString() + "%</span></td>" : "");
								else
									builder.AppendFormat("<tr><td>{0}</td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], ""), (CType == (int)CompletionType.All) ? "<td style='padding-left:7'>" + reader["PercentCompleted"].ToString() + "%</td>" : "");
							}
						}
						break;
					case (int)ObjectTypes.Document:
						using (IDataReader reader = Document.GetListResources(itemId))
						{
							while (reader.Read())
							{
								hasResources = true;

								if (stateId == (int)ObjectStates.Completed)
									builder.AppendFormat("<tr><td><span style='color:#999999;text-decoration:line-through'>{0}</span></td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], ""));
								else if (stateId == (int)ObjectStates.Overdue || isOverdue)
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], "red"));
								else
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], ""));
							}
						}
						break;
					case (int)ObjectTypes.Issue:
						using (IDataReader reader = Incident.GetListResourcesWithResponsible(itemId))
						{
							while (reader.Read())
							{
								hasResources = true;

								if (stateId == (int)ObjectStates.Completed)
									builder.AppendFormat("<tr><td><span style='color:#999999;text-decoration:line-through'>{0}</span></td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], ""));
								else if (isOverdue)
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], "red"));
								else
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], ""));
							}
						}
						break;
					case (int)ObjectTypes.CalendarEntry:
						using (IDataReader reader = CalendarEntry.GetListResources(itemId))
						{
							while (reader.Read())
							{
								hasResources = true;

								if (stateId == (int)ObjectStates.Completed)
									builder.AppendFormat("<tr><td><span style='color:#999999;text-decoration:line-through'>{0}</span></td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], ""));
								else if (stateId == (int)ObjectStates.Overdue || isOverdue)
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], "red"));
								else
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], ""));
							}
						}
						break;
				}

				if (hasResources)
				{
					builder.Insert(0, "<table class='text' cellspacing='0' cellpadding='1' border='0'>");
					builder.Append("</table>");
				}
			}

			return builder.ToString();
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
			else if (itemType == (int)ObjectTypes.Issue)
			{
				if (stateId == (int)ObjectStates.Completed)
					retVal = string.Format(@"<table cellspacing='0' cellpadding='0'><tr><td><div class='progress'><img alt='' src='{0}' width='100%' /></div></td><td><span style='color:#999999;text-decoration:line-through'><span style='color:black'>100%</span></span></td></tr></table>", image);
				else if (stateId == (int)ObjectStates.OnCheck)
				{
					retVal = GetGlobalResourceObject("IbnFramework.Incident", "OnCheck").ToString();
					if (isOverdue)
						retVal = string.Format("<span style='color:red'>{0}</span>", retVal);
				}
			}

			return retVal;
		}

		protected string GetPriorityIcon(int itemType, int PriorityId, string PName)
		{
			string retVal = "";
			if (itemType > 0)
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
			//this.ddGroups.SelectedIndexChanged += new EventHandler(ddGroups_SelectedIndexChanged);
			//this.btnApplyF.Click += new EventHandler(btnApplyF_Click);
			//this.btnResetF.Click += new EventHandler(btnResetF_Click);
			this.btnApplyG.Click += new EventHandler(btnApplyG_Click);
			this.btnVResetF.ServerClick += new EventHandler(btnVResetF_ServerClick);
			this.dgObjects.SortCommand += new DataGridSortCommandEventHandler(dgObjects_SortCommand);
			this.dgObjects.PageIndexChanged += new DataGridPageChangedEventHandler(dgObjects_PageIndexChanged);
			this.dgObjects.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(dgObjects_PageSizeChanged);
			//this.lbHideFilter.Click += new EventHandler(lbShowFilter_Click);
			//this.lbShowFilter.Click += new EventHandler(lbShowFilter_Click);
			this.lbChangeViewDef.Click += new EventHandler(lbChangeViewDef_Click);
			this.lbChangeViewDates.Click += new EventHandler(lbChangeViewDates_Click);
			this.lbChangeViewTimes.Click += new EventHandler(lbChangeViewTimes_Click);
		}
		#endregion

		#region dgEvents
		private void dgObjects_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (_pc["MV_Sort"] != null && _pc["MV_Sort"].ToString() == (string)e.SortExpression)
				_pc["MV_Sort"] = _pc["MV_Sort"] + " DESC";
			else
				_pc["MV_Sort"] = (string)e.SortExpression;
			Mediachase.Ibn.Web.UI.CHelper.RequireBindGrid();
			//BindDataGrid();
		}

		private void dgObjects_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			_pc["MV_Page"] = e.NewPageIndex.ToString();
			Mediachase.Ibn.Web.UI.CHelper.RequireBindGrid();
			//BindDataGrid();
		}

		private void dgObjects_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			_pc["MV_PageSize"] = e.NewPageSize.ToString();
			Mediachase.Ibn.Web.UI.CHelper.RequireBindGrid();
			//BindDataGrid();
		}
		#endregion

		//#region ddGroups_Change
		//private void ddGroups_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//    _pc["MV_Group"] = ddGroups.SelectedValue;
		//    BindUsers(int.Parse(ddGroups.SelectedValue));

		//    BindDataGrid();
		//}
		//#endregion

		#region Apply-Reset Filter/Grouping
		//private void btnApplyF_Click(object sender, EventArgs e)
		//{
		//    SaveValues();
		//    BindSavedValues();
		//    BindInfoTable();
		//    BindDataGrid();
		//}

		//private void btnResetF_Click(object sender, EventArgs e)
		//{
		//    _pc["MV_Group"] = ((int)InternalSecureGroups.Intranet).ToString();
		//    BindGroups();
		//    BindDefaultValues();
		//    SaveValues();
		//    ddUser.SelectedIndex = 0;
		//    _pc["MV_User"] = ddUser.SelectedValue;
		//    BindInfoTable();
		//    BindDataGrid();
		//}

		private void btnApplyG_Click(object sender, EventArgs e)
		{
			string s = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(s))
				_pc["MV_Grouping"] = s;
			Mediachase.Ibn.Web.UI.CHelper.RequireBindGrid();
			//BindDataGrid();
		}

		private void btnVResetF_ServerClick(object sender, EventArgs e)
		{
			_pc["MV_Group"] = ((int)InternalSecureGroups.Intranet).ToString();

			_pc["MV_User"] = "0";
			_pc["MV_Manager"] = "0";
			_pc["MV_Project"] = "0";
			_pc["MV_Category"] = "0";
			_pc["MV_Completed"] = "0";
			_pc["MV_Upcoming"] = "0";
			_pc["MV_Active"] = "True";

			_pc["MV_ClientNew"] = "_";

			_pc["MV_ShowEvents"] = true.ToString();
			_pc["MV_ShowIssues"] = true.ToString();
			_pc["MV_ShowDocs"] = true.ToString();
			_pc["MV_ShowTasks"] = true.ToString();
			_pc["MV_ShowToDo"] = true.ToString();
			_pc["MV_ShowChildTodo"] = false.ToString();
			BindInfoTable();
			Mediachase.Ibn.Web.UI.CHelper.RequireBindGrid();
			//BindDataGrid();
		}
		#endregion

		#region Show-Hide
		private void lbShowFilter_Click(object sender, EventArgs e)
		{
			if (_pc["MV_ShowFilter"] == null || !bool.Parse(_pc["MV_ShowFilter"]))
				_pc["MV_ShowFilter"] = "True";
			else
				_pc["MV_ShowFilter"] = "False";
			Mediachase.Ibn.Web.UI.CHelper.RequireBindGrid();
			//BindDataGrid();
		}
		#endregion

		#region Change View
		void lbChangeViewDates_Click(object sender, EventArgs e)
		{
			_pc["MV_ViewStyle"] = FieldSetName.tWorkManagersDates.ToString();
			Mediachase.Ibn.Web.UI.CHelper.RequireBindGrid();
			//BindDataGrid();
		}

		void lbChangeViewDef_Click(object sender, EventArgs e)
		{
			_pc["MV_ViewStyle"] = FieldSetName.tWorkManagersDefault.ToString();
			Mediachase.Ibn.Web.UI.CHelper.RequireBindGrid();
			//BindDataGrid();
		}

		void lbChangeViewTimes_Click(object sender, EventArgs e)
		{
			_pc["MV_ViewStyle"] = FieldSetName.tWorkManagersWorkTime.ToString();
			Mediachase.Ibn.Web.UI.CHelper.RequireBindGrid();
			//BindDataGrid();
		}
		#endregion
	}
}
