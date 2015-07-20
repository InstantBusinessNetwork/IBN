namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Data.Common;
	using System.Collections;
	using System.Drawing;
	using System.Globalization;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using ComponentArt.Web.UI;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Lists;
	using Mediachase.Ibn.Clients;
	using Mediachase.Ibn.Core.Business;
	using Mediachase.Ibn.Data;

	/// <summary>
	///		Summary description for ProjectsListInner.
	/// </summary>
	public partial class ProjectsListInner : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ProjectsListInner).Assembly);
		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;
		private IFormatProvider _culture = CultureInfo.InvariantCulture;
		private bool _hasMyProj;
		private string _sortColumn = "Title";

		protected enum FieldSetName
		{
			tProjectsDefault,
			tProjectsLight
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			_hasMyProj = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ProjectManager);

			ApplyLocalization();
			if (!Page.IsPostBack)
			{
				if (_pc["pl_ViewStyle"] == null)
					_pc["pl_ViewStyle"] = FieldSetName.tProjectsDefault.ToString();

				BindDefaultValues();
			}

			BindSavedData();
			BindInfoTable();

			if (Request["Export"] == "1")
				ExportGrid();
			else
				if (Request["Export"] == "2")
					ExportXMLTable();
				else
					BindDataGrid();

		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();

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

			tblFilterEdit.Visible = (_pc["ShowProjectFilter"] != null && bool.Parse(_pc["ShowProjectFilter"]));
			tblFilterInfo.Visible = !tblFilterEdit.Visible;
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblStartDate.Text = LocRM.GetString("StartDate") + ":";
			lblEndDate.Text = LocRM.GetString("FinishDate") + ":";
			lblManager.Text = LocRM.GetString("Manager") + ":";
			lblExeManager.Text = LocRM.GetString("tExeManager") + ":";
			lblType.Text = LocRM.GetString("Type") + ":";
			lblTitle.Text = LocRM.GetString("tKeyword") + ":";
			lblStatus.Text = LocRM.GetString("Status") + ":";
			lblGenCats.Text = LocRM.GetString("GeneralCats") + ":";
			lblPrjCats.Text = LocRM.GetString("ProjectCats") + ":";
			lblPriority.Text = LocRM.GetString("Priority") + ":";
			lblPrjGrps.Text = LocRM.GetString("Portfolios") + ":";
			btnApply.Value = LocRM.GetString("Apply");
			btnReset.Value = LocRM.GetString("Reset");
			btnReset2.Value = LocRM.GetString("ResetFilter");
			lblFilterNotSet.Text = LocRM.GetString("FilterNotSet");
			lblPrjPhase.Text = LocRM.GetString("tPrjPhase");
			lbShowFilter.Text = "<img align='absmiddle' border='0' title='" + LocRM.GetString("tShowFilter") + "' src='../Layouts/Images/scrolldown_hover.GIF' />";
			lbHideFilter.Text = "<img align='absmiddle' border='0' title='" + LocRM.GetString("tHideFilter") + "' src='../Layouts/Images/scrollup_hover.GIF' />";
			chkOnlyForUser.Text = LocRM.GetString("OnlyForUser");
			lblClient.Text = LocRM.GetString("Client");
		}
		#endregion

		#region BindInfoTable
		private void BindInfoTable()
		{
			int rowCount = 0;
			tblFilterInfoSet.Rows.Clear();

			// Start Date
			if (_pc["pl_SDType"] != null && _pc["pl_StartDate"] != null && _pc["pl_StartDate"] != "")
			{
				ListItem li = ddSDType.Items.FindByValue(_pc["pl_SDType"]);
				if (li != null && li.Value != "0")
				{
					DateTime _date = DateTime.Parse(_pc["pl_StartDate"], _culture);
					AddRow(
						String.Format("{0}: ", LocRM.GetString("StartDate")),
						String.Format("{0} {1}", li.Text, _date.ToShortDateString()));
					rowCount++;
				}
			}

			// Finish Date
			if (_pc["pl_FDType"] != null && _pc["pl_EndDate"] != null && _pc["pl_EndDate"] != "")
			{
				ListItem li = ddFDType.Items.FindByValue(_pc["pl_FDType"]);
				if (li != null && li.Value != "0")
				{
					DateTime _date = DateTime.Parse(_pc["pl_EndDate"], _culture);
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("FinishDate")),
						String.Format("{0} {1}", li.Text, _date.ToShortDateString()));
					rowCount++;
				}
			}

			// Status
			if (Request.QueryString["PrjStatus"] != null)
			{
				ListItem li = ddStatus.Items.FindByValue(Request.QueryString["PrjStatus"]);
				if (li != null && li.Value != "0")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("Status")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
				trStatus.Visible = false;
			}
			else
			{
				trStatus.Visible = true;
				if (_pc["pl_Status"] != null)
				{
					ListItem li = ddStatus.Items.FindByValue(_pc["pl_Status"]);
					if (li != null && li.Value != "0")
					{
						AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Status")), li.Text);
						rowCount++;
					}
				}
			}

			// Phase
			if (Request.QueryString["PrjPhs"] != null)
			{
				ListItem li = ddPrjPhases.Items.FindByValue(Request.QueryString["PrjPhs"]);
				if (li != null && li.Value != "0")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("tPrjPhase")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
				trPrjPhase.Visible = false;
			}
			else
			{
				trPrjPhase.Visible = true;
				if (_pc["pl_Phase"] != null)
				{
					ListItem li = ddPrjPhases.Items.FindByValue(_pc["pl_Phase"]);
					if (li != null && li.Value != "0")
					{
						AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tPrjPhase")), li.Text);
						rowCount++;
					}
				}
			}

			// Type
			if (Request.QueryString["PrjType"] != null)
			{
				ListItem li = ddType.Items.FindByValue(Request.QueryString["PrjType"]);
				if (li != null && li.Value != "0")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("Type")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
				trType.Visible = false;
			}
			else
			{
				trType.Visible = true;
				if (_pc["pl_Type"] != null)
				{
					ListItem li = ddType.Items.FindByValue(_pc["pl_Type"]);
					if (li != null && li.Value != "0")
					{
						AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Type")), li.Text);
						rowCount++;
					}
				}
			}

			// Manager
			if (_pc["ProjectList_CurrentTab"] == "MyProjects")
			{
				AddRow(
					String.Format("{0}:&nbsp; ", LocRM.GetString("Manager")),
					String.Format("<span style='color:red'>{0}, {1}</span>", Security.CurrentUser.LastName, Security.CurrentUser.FirstName));
				trManager.Visible = false;
			}
			else
			{
				trManager.Visible = true;
				if (_pc["pl_Manager"] != null)
				{
					ListItem li = ddManager.Items.FindByValue(_pc["pl_Manager"]);
					if (li != null && li.Value != "0")
					{
						AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Manager")), li.Text);
						rowCount++;
					}
				}
			}

			// ExeManager
			if (_pc["pl_ExeManager"] != null)
			{
				ListItem li = ddExeManager.Items.FindByValue(_pc["pl_ExeManager"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tExeManager")), li.Text);
					rowCount++;
				}
			}

			// Priority
			if (Request.QueryString["PrjPrty"] != null)
			{
				ListItem li = ddPriority.Items.FindByValue(Request.QueryString["PrjPrty"]);
				if (li != null && li.Value != "-1")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("Priority")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
				trPriority.Visible = false;
			}
			else
			{
				trPriority.Visible = true;
				if (_pc["pl_Priority"] != null)
				{
					ListItem li = ddPriority.Items.FindByValue(_pc["pl_Priority"]);
					if (li != null && li.Value != "-1")
					{
						AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Priority")), li.Text);
						rowCount++;
					}
				}
			}

			//Client
			if (_pc["pl_ClientNew"] != null)
			{
				string ss = _pc["pl_ClientNew"];
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
						rowCount++;
					}
					else if (stype.ToLower() == "contact")
					{
						ContactEntity contactEntity = (ContactEntity)BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1)));
						if (contactEntity != null)
							sName = contactEntity.FullName;
						AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Client")), sName);
						rowCount++;
					}
 				}
			}

			// Texts
			if (_pc["pl_Title"] != null && _pc["pl_Title"] != "")
			{
				AddRow(
					String.Format("{0}:&nbsp; ", LocRM.GetString("tKeyword")),
					String.Format("'{0}'", _pc["pl_Title"]));
				rowCount++;
			}

			// Project Groups
			if (Request.QueryString["PrjGrp"] != null)
			{
				ListItem li = lbPrjGrps.Items.FindByValue(Request.QueryString["PrjGrp"]);
				if (li != null)
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("Portfolios")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
				trPrGroups.Visible = false;
			}
			else
			{
				if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
					trPrGroups.Visible = true;
				if (_pc["pl_PrjGrpType"] != null)
				{
					ListItem li = ddPrjGrpType.Items.FindByValue(_pc["pl_PrjGrpType"]);
					if (li != null && li.Value != "0")
					{
						string str = "";
						foreach (ListItem item in lbPrjGrps.Items)
						{
							if (item.Selected)
							{
								if (str != "")
									str += ", ";
								str += item.Text;
							}
						}

						if (li.Value == "2")
							AddRow(String.Format("{0} ({1}):&nbsp; ", LocRM.GetString("Portfolios"), li.Text), str);
						else
							AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Portfolios")), str);
						rowCount++;
					}
				}
			}

			// General Categories
			if (Request.QueryString["GenCat"] != null)
			{
				ListItem li = lbGenCats.Items.FindByValue(Request.QueryString["GenCat"]);
				if (li != null)
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("GeneralCats")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
				trGenCats.Visible = false;
			}
			else
			{
				trGenCats.Visible = true;
				if (_pc["pl_GenCatType"] != null)
				{
					ListItem li = ddGenCatType.Items.FindByValue(_pc["pl_GenCatType"]);
					if (li != null && li.Value != "0")
					{
						string str = "";
						foreach (ListItem item in lbGenCats.Items)
						{
							if (item.Selected)
							{
								if (str != "")
									str += ", ";
								str += item.Text;
							}
						}

						if (li.Value == "2")
							AddRow(String.Format("{0} ({1}):&nbsp; ", LocRM.GetString("GeneralCats"), li.Text), str);
						else
							AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("GeneralCats")), str);
						rowCount++;
					}
				}
			}

			// Project Categories
			if (Request.QueryString["PrjCat"] != null)
			{
				ListItem li = lbPrjCats.Items.FindByValue(Request.QueryString["PrjCat"]);
				if (li != null)
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("ProjectCats")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
				trPrCats.Visible = false;
			}
			else
			{
				trPrCats.Visible = true;
				if (_pc["pl_PrjCatType"] != null)
				{
					ListItem li = ddPrjCatType.Items.FindByValue(_pc["pl_PrjCatType"]);
					if (li != null && li.Value != "0")
					{
						string str = "";
						foreach (ListItem item in lbPrjCats.Items)
						{
							if (item.Selected)
							{
								if (str != "")
									str += ", ";
								str += item.Text;
							}
						}

						if (li.Value == "2")
							AddRow(String.Format("{0} ({1}):&nbsp; ", LocRM.GetString("ProjectCats"), li.Text), str);
						else
							AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("ProjectCats")), str);
						rowCount++;
					}
				}
			}

			// OnlyForUser
			if (_pc["pl_OnlyForUser"] == "True")
			{
				AddRow("", LocRM.GetString("OnlyForUser"));
				rowCount++;
			}

			if (rowCount > 0)
			{
				lblFilterNotSet.Visible = false;
				btnReset2.Visible = true;
			}
			else
			{
				lblFilterNotSet.Visible = true;
				btnReset2.Visible = false;
			}
		}

		private void AddRow(string filterName, string filterValue)
		{
			HtmlTableRow tr = new HtmlTableRow();
			HtmlTableCell td1 = new HtmlTableCell();
			td1.Align = "Right";
			td1.InnerHtml = filterName;
			HtmlTableCell td2 = new HtmlTableCell();
			td2.InnerHtml = filterValue;
			tr.Cells.Add(td1);
			tr.Cells.Add(td2);
			tblFilterInfoSet.Rows.Add(tr);
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			// Dates
			//txtStartDate.Text = DateTime.Today.ToShortDateString();
			dtcStartDate.SelectedDate = DateTime.Today;
			//txtEndDate.Text = DateTime.Now.ToShortDateString();
			dtcEndDate.SelectedDate = DateTime.Now;
			ddSDType.Items.Clear();
			ddSDType.Items.Add(new ListItem(LocRM.GetString("Any"), "0"));
			ddSDType.Items.Add(new ListItem(LocRM.GetString("GE"), "1"));
			ddSDType.Items.Add(new ListItem(LocRM.GetString("LE"), "2"));
			ddSDType.Items.Add(new ListItem(LocRM.GetString("Equal"), "3"));

			ddFDType.Items.Clear();
			ddFDType.Items.Add(new ListItem(LocRM.GetString("Any"), "0"));
			ddFDType.Items.Add(new ListItem(LocRM.GetString("GE"), "1"));
			ddFDType.Items.Add(new ListItem(LocRM.GetString("LE"), "2"));
			ddFDType.Items.Add(new ListItem(LocRM.GetString("Equal"), "3"));

			// Status
			ddStatus.Items.Clear();
			ddStatus.Items.Add(new ListItem(LocRM.GetString("All"), "0"));
			using (IDataReader rdr = Project.GetListProjectStatus())
			{
				while (rdr.Read())
					ddStatus.Items.Add(new ListItem((string)rdr["StatusName"], rdr["StatusId"].ToString()));
			}

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

			// ExeMananger
			ddExeManager.Items.Clear();
			ddExeManager.DataSource = Project.GetListExecutiveManagers();
			ddExeManager.DataTextField = "UserName";
			ddExeManager.DataValueField = "ExecutiveManagerId";
			ddExeManager.DataBind();
			ddExeManager.Items.Insert(0, new ListItem(LocRM.GetString("All"), "0"));


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

			// Text
			txtTitle.Value = "";

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

			if (Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				_pc["pl_PrjGrpType"] = "0";
				trPrGroups.Visible = false;
			}

			chkOnlyForUser.Checked = false;

			//Client
			ClientControl.ObjectType = String.Empty;
			ClientControl.ObjectId = PrimaryKeyId.Empty;
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			Projects prj = (Projects)this.Parent.Parent;
			Mediachase.UI.Web.Modules.PageViewMenu secHeader = prj.ToolBar;
			if (secHeader != null)
			{
				secHeader.Title = LocRM.GetString("Projects");

				ComponentArt.Web.UI.MenuItem subItem;
				#region Create
				if (_hasMyProj)
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.LookId = "TopItemLook";
					//					subItem.Look.LeftIconUrl = "~/Layouts/Images/Icons/Project_Create.gif";
					//					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					//					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.NavigateUrl = "~/Projects/ProjectEdit.aspx";
					subItem.Text = /*"<img border='0' src='../Layouts/Images/Icons/Project_Create.gif' width='16px' height='16px' align='absmiddle'/>&nbsp;" + */LocRM.GetString("AddProject");
					subItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/icons/Project_Create.gif");
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					secHeader.ActionsMenu.Items.Add(subItem);
				}
				#endregion

				ComponentArt.Web.UI.MenuItem topMenuItem;

				#region View Menu Items
				topMenuItem = new ComponentArt.Web.UI.MenuItem();
				topMenuItem.Text = LocRM.GetString("tView");
				topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
				topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
				topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
				topMenuItem.LookId = "TopItemLook";

				string sCurrentView = _pc["pl_ViewStyle"];

				subItem = new ComponentArt.Web.UI.MenuItem();
				if (sCurrentView == FieldSetName.tProjectsDefault.ToString())
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewDef, "");
				subItem.Text = LocRM.GetString(FieldSetName.tProjectsDefault.ToString());
				topMenuItem.Items.Add(subItem);

				subItem = new ComponentArt.Web.UI.MenuItem();
				if (sCurrentView == FieldSetName.tProjectsLight.ToString())
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewDates, "");
				subItem.Text = LocRM.GetString(FieldSetName.tProjectsLight.ToString());
				topMenuItem.Items.Add(subItem);
				#endregion

				secHeader.ActionsMenu.Items.Add(topMenuItem);

				topMenuItem = new ComponentArt.Web.UI.MenuItem();
				topMenuItem.Text = LocRM.GetString("Export");
				topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
				topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
				topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
				topMenuItem.LookId = "TopItemLook";

				#region xlsexport
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/Icons/xlsexport.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Projects/default.aspx?Export=1";
				subItem.Text = LocRM.GetString("ExcelExport");
				topMenuItem.Items.Add(subItem);
				#endregion

				#region xmlexport
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/Icons/xmlexport.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Projects/default.aspx?Export=2";
				subItem.Text = LocRM.GetString("XMLExport");
				topMenuItem.Items.Add(subItem);
				#endregion

				secHeader.ActionsMenu.Items.Add(topMenuItem);

			}
		}
		#endregion

		#region BindSavedData
		private void BindSavedData()
		{
			// Dates
			if (_pc["pl_SDType"] != null)
				CommonHelper.SafeSelect(ddSDType, _pc["pl_SDType"]);

			if (_pc["pl_StartDate"] != null && _pc["pl_StartDate"] != "")
			{
				DateTime _date = DateTime.Parse(_pc["pl_StartDate"], _culture);
				//txtStartDate.Text = _date.ToShortDateString();
				dtcStartDate.SelectedDate = _date;
			}

			if (_pc["pl_FDType"] != null)
				CommonHelper.SafeSelect(ddFDType, _pc["pl_FDType"]);

			if (_pc["pl_EndDate"] != null && _pc["pl_EndDate"] != "")
			{
				DateTime _date = DateTime.Parse(_pc["pl_EndDate"], _culture);
				//txtEndDate.Text = _date.ToShortDateString();
				dtcEndDate.SelectedDate = _date;
			}

			// Status
			if (_pc["pl_Status"] != null)
				CommonHelper.SafeSelect(ddStatus, _pc["pl_Status"]);

			// Phase
			if (_pc["pl_Phase"] != null)
				CommonHelper.SafeSelect(ddPrjPhases, _pc["pl_Phase"]);

			// Type
			if (_pc["pl_Type"] != null)
				CommonHelper.SafeSelect(ddType, _pc["pl_Type"]);

			// Manager
			if (_pc["pl_Manager"] != null)
				CommonHelper.SafeSelect(ddManager, _pc["pl_Manager"]);

			// ExeManager
			if (_pc["pl_ExeManager"] != null)
				CommonHelper.SafeSelect(ddExeManager, _pc["pl_ExeManager"]);

			// Priority
			if (_pc["pl_Priority"] != null)
				CommonHelper.SafeSelect(ddPriority, _pc["pl_Priority"]);

			// Texts
			if (_pc["pl_Title"] != null)
				txtTitle.Value = _pc["pl_Title"];

			// Project Groups
			if (_pc["pl_PrjGrpType"] != null)
				CommonHelper.SafeSelect(ddPrjGrpType, _pc["pl_PrjGrpType"]);

			if (ddPrjGrpType.SelectedItem.Value == "0")
				lbPrjGrps.Style["Display"] = "none";
			else
				lbPrjGrps.Style["Display"] = "block";

			using (IDataReader reader = Project.GetListProjectGroupsByUser())
			{
				while (reader.Read())
					CommonHelper.SafeMultipleSelect(lbPrjGrps, reader["ProjectGroupId"].ToString());
			}

			// General Categories
			if (_pc["pl_GenCatType"] != null)
				CommonHelper.SafeSelect(ddGenCatType, _pc["pl_GenCatType"]);

			if (ddGenCatType.SelectedItem.Value == "0")
				lbGenCats.Style["Display"] = "none";
			else
				lbGenCats.Style["Display"] = "block";

			using (IDataReader reader = Project.GetListCategoriesByUser())
			{
				while (reader.Read())
					CommonHelper.SafeMultipleSelect(lbGenCats, reader["CategoryId"].ToString());
			}

			// Project Categories
			if (_pc["pl_PrjCatType"] != null)
				CommonHelper.SafeSelect(ddPrjCatType, _pc["pl_PrjCatType"]);

			if (ddPrjCatType.SelectedItem.Value == "0")
				lbPrjCats.Style["Display"] = "none";
			else
				lbPrjCats.Style["Display"] = "block";

			using (IDataReader reader = Project.GetListProjectCategoriesByUser())
			{
				while (reader.Read())
					CommonHelper.SafeMultipleSelect(lbPrjCats, reader["CategoryId"].ToString());
			}

			// OnlyForUser
			if (_pc["pl_OnlyForUser"] != null)
			{
				try
				{
					chkOnlyForUser.Checked = bool.Parse(_pc["pl_OnlyForUser"]);
				}
				catch { }
			}

			//Client
			if (_pc["pl_ClientNew"] != null && (!Page.IsPostBack || ClientControl.ObjectId == PrimaryKeyId.Empty))
			{
				string ss = _pc["pl_ClientNew"];
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

			// Other
			if (_pc["pl_ProjectSortColumn"] == null)
				_pc["pl_ProjectSortColumn"] = _sortColumn;
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			if (ddSDType.SelectedItem != null)
				_pc["pl_SDType"] = ddSDType.SelectedValue;
			if (ddSDType.SelectedItem.Value != "0")
			{
				_pc["pl_StartDate"] = dtcStartDate.SelectedDate.ToString(_culture);
			}

			if (ddFDType.SelectedItem != null)
				_pc["pl_FDType"] = ddFDType.SelectedValue;
			if (ddFDType.SelectedItem.Value != "0")
			{
				_pc["pl_EndDate"] = dtcEndDate.SelectedDate.ToString(_culture);
			}

			if (ddStatus.SelectedItem != null)
				_pc["pl_Status"] = ddStatus.SelectedValue;

			if (ddPrjPhases.SelectedItem != null)
				_pc["pl_Phase"] = ddPrjPhases.SelectedValue;

			if (ddType.SelectedItem != null)
				_pc["pl_Type"] = ddType.SelectedValue;

			if (ddManager.SelectedItem != null)
				_pc["pl_Manager"] = ddManager.SelectedValue;

			if (ddExeManager.SelectedItem != null)
				_pc["pl_ExeManager"] = ddExeManager.SelectedValue;

			if (ddPriority.SelectedItem != null)
				_pc["pl_Priority"] = ddPriority.SelectedValue;

			_pc["pl_Title"] = txtTitle.Value;

			// Project Groups
			_pc["pl_PrjGrpType"] = ddPrjGrpType.SelectedValue;
			ArrayList alPrjGrps = new ArrayList();
			foreach (ListItem liItem in lbPrjGrps.Items)
			{
				if (liItem.Selected && !alPrjGrps.Contains(int.Parse(liItem.Value)))
					alPrjGrps.Add(int.Parse(liItem.Value));
			}
			Project.SaveProjectGroups(alPrjGrps);

			// General Categories
			_pc["pl_GenCatType"] = ddGenCatType.SelectedValue;
			ArrayList alGenCats = new ArrayList();
			foreach (ListItem liItem in lbGenCats.Items)
			{
				if (liItem.Selected && !alGenCats.Contains(int.Parse(liItem.Value)))
					alGenCats.Add(int.Parse(liItem.Value));
			}
			Project.SaveGeneralCategories(alGenCats);

			// Project Categories
			_pc["pl_PrjCatType"] = ddPrjCatType.SelectedValue;
			ArrayList alPrjCats = new ArrayList();
			foreach (ListItem liItem in lbPrjCats.Items)
			{
				if (liItem.Selected && !alPrjCats.Contains(int.Parse(liItem.Value)))
					alPrjCats.Add(int.Parse(liItem.Value));
			}
			Project.SaveProjectCategories(alPrjCats);

			// OnlyForUser
			_pc["pl_OnlyForUser"] = chkOnlyForUser.Checked.ToString();

			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc["pl_ClientNew"] = "org_" + ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc["pl_ClientNew"] = "contact_" + ClientControl.ObjectId;
			else
				_pc["pl_ClientNew"] = "_";
		}
		#endregion

		#region BindDataGrid (2 overloads)
		private void BindDataGrid()
		{
			BindDataGrid(dgProjects);
		}

		private DataTable BindDataGrid(DataGrid dg)
		{

			int state = 0;

			// Project Status
			int status_id = 0;
			if (Request.QueryString["PrjStatus"] != null)
				status_id = int.Parse(Request.QueryString["PrjStatus"]);
			else if (ddStatus.SelectedItem != null)
				status_id = int.Parse(ddStatus.SelectedItem.Value);

			// Project Phase
			int phase_id = 0;
			if (Request.QueryString["PrjPhs"] != null)
				phase_id = int.Parse(Request.QueryString["PrjPhs"]);
			else if (ddPrjPhases.SelectedItem != null)
				phase_id = int.Parse(ddPrjPhases.SelectedValue);

			// Project Type
			int type_id = 0;
			if (Request.QueryString["PrjType"] != null)
				type_id = int.Parse(Request.QueryString["PrjType"]);
			else if (ddType.SelectedItem != null)
				type_id = int.Parse(ddType.SelectedItem.Value);

			// Project Priority
			int priority_id = 0;
			if (Request.QueryString["PrjPrty"] != null)
				priority_id = int.Parse(Request.QueryString["PrjPrty"]);
			else if (ddPriority.SelectedItem != null)
				priority_id = int.Parse(ddPriority.SelectedItem.Value);

			// Start Date
			int sdc = 0;
			if (ddSDType.SelectedItem != null)
				sdc = int.Parse(ddSDType.SelectedItem.Value);

			DateTime startdate = DateTime.Now;
			try
			{
				if (sdc != 0)
					startdate = dtcStartDate.SelectedDate;
			}
			catch
			{
			}

			// Finish Date
			int fdc = 0;
			if (ddFDType.SelectedItem != null)
				fdc = int.Parse(ddFDType.SelectedItem.Value);

			DateTime finshdate = DateTime.Now;
			try
			{
				if (fdc != 0)
					finshdate = dtcEndDate.SelectedDate;
			}
			catch
			{
			}

			// Manager
			int manager_id = 0;
			if (_pc["ProjectList_CurrentTab"] == "MyProjects")
				manager_id = Security.CurrentUser.UserID;
			else
				if (ddManager.SelectedItem != null)
					manager_id = int.Parse(ddManager.SelectedValue);

			// ExeManager
			int exemanager_id = 0;
			if (ddExeManager.SelectedItem != null)
				exemanager_id = int.Parse(ddExeManager.SelectedValue);

			// Project Groups
			int prjGroup_type = 0;
			if (Request.QueryString["PrjGrp"] != null)
				prjGroup_type = -int.Parse(Request.QueryString["PrjGrp"]);
			else if (ddPrjGrpType.SelectedItem != null)
				prjGroup_type = int.Parse(ddPrjGrpType.SelectedValue);

			// GeneralCategory Type
			int genCategory_type = 0;
			if (Request.QueryString["GenCat"] != null)
				genCategory_type = -int.Parse(Request.QueryString["GenCat"]);
			else if (ddGenCatType.SelectedItem != null)
				genCategory_type = int.Parse(ddGenCatType.SelectedValue);

			// ProjectCategory Type
			int prjCategory_type = 0;
			if (Request.QueryString["PrjCat"] != null)
				prjCategory_type = -int.Parse(Request.QueryString["PrjCat"]);
			else if (ddPrjCatType.SelectedItem != null)
				prjCategory_type = int.Parse(ddPrjCatType.SelectedValue);

			bool OnlyForUser = chkOnlyForUser.Checked;

			// Client
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName())
				orgUid = ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName())
				contactUid = ClientControl.ObjectId;

			DataTable dt = Project.GetListProjectsByFilterDataTable(
				txtTitle.Value,
				state,
				status_id,
				type_id,
				priority_id,
				contactUid, 
				orgUid,
				sdc, startdate,
				fdc, finshdate,
				manager_id,
				exemanager_id,
				genCategory_type,
				prjCategory_type,
				prjGroup_type,
				phase_id,
				OnlyForUser);

			DataView dv = dt.DefaultView;
			dv.Sort = _pc["pl_ProjectSortColumn"];
			dg.DataSource = dt.DefaultView;

			if (_pc["pl_PageSize"] != null)
				dg.PageSize = int.Parse(_pc["pl_PageSize"]);

			if (_pc["pl_Page"] != null)
			{
				int pageindex = int.Parse(_pc["pl_Page"]);
				int ppi = dv.Count / dg.PageSize;
				if (dv.Count % dg.PageSize == 0)
					ppi = ppi - 1;

				if (pageindex <= ppi)
					dg.CurrentPageIndex = pageindex;
				else dg.CurrentPageIndex = 0;
			}

			// FieldSet
			if (dg != dgExport)
			{
				string fieldSet = _pc["pl_ViewStyle"];

				switch (fieldSet)
				{
					case "tProjectsLight":
						dg.Columns[1].Visible = false;
						dg.Columns[2].Visible = true;
						break;
					default:	//"tProjectsDefault"
						dg.Columns[1].Visible = true;
						dg.Columns[2].Visible = false;
						break;
				}
			}

			dg.DataBind();

			return dt;
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
			this.dgProjects.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgProjects_PageChange);
			this.dgProjects.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgProjects_Sort);
			this.dgProjects.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dgProjects_PageSizeChange);
			this.lbChangeViewDef.Click += new EventHandler(lbChangeViewDef_Click);
			this.lbChangeViewDates.Click += new EventHandler(lbChangeViewDates_Click);
		}
		#endregion

		#region Apply_ServerClick
		protected void Apply_ServerClick(object sender, System.EventArgs e)
		{
			//pc["ShowProjectFilter"] = "False";
			SaveValues();
			BindSavedData();
			BindInfoTable();
			BindDataGrid();
		}
		#endregion

		#region Reset_ServerClick
		protected void Reset_ServerClick(object sender, System.EventArgs e)
		{
			//pc["ShowProjectFilter"] = "False";
			BindDefaultValues();
			SaveValues();
			BindInfoTable();
			BindDataGrid();
		}
		#endregion

		#region dgProjects_PageChange
		private void dgProjects_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			_pc["pl_Page"] = e.NewPageIndex.ToString();
			BindDataGrid();
		}
		#endregion

		#region dgProjects_PageSizeChange
		private void dgProjects_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			_pc["pl_PageSize"] = e.NewPageSize.ToString();
			BindDataGrid();
		}
		#endregion

		#region dgProjects_Sort
		private void dgProjects_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if (_pc["pl_ProjectSortColumn"] != null && _pc["pl_ProjectSortColumn"].ToString() == (string)e.SortExpression)
				_sortColumn = (string)e.SortExpression + " DESC";
			else
				_sortColumn = (string)e.SortExpression;

			_pc["pl_ProjectSortColumn"] = _sortColumn;
			BindDataGrid();
		}
		#endregion

		#region GetBool
		protected bool GetBool(int val)
		{
			if (val == 1)
				return true;
			else
				return false;
		}
		#endregion

		#region lblDeleteProjectAll_Click
		protected void lblDeleteProjectAll_Click(object sender, System.EventArgs e)
		{
			int projectId = int.Parse(hdnProjectId.Value);
			ListManager.DeleteProjectRoot(projectId);
			Project.Delete(projectId);
			BindDataGrid();
		}
		#endregion

		#region lbExport_Click
		protected void lbExport_Click(object sender, System.EventArgs e)
		{
			ExportGrid();
		}
		#endregion

		#region lbHideFilter_Click
		protected void lbHideFilter_Click(object sender, System.EventArgs e)
		{
			_pc["ShowProjectFilter"] = "False";
			BindDefaultValues();
			BindSavedData();
			BindInfoTable();
			BindDataGrid();
		}
		#endregion

		#region lbShowFilter_Click
		protected void lbShowFilter_Click(object sender, System.EventArgs e)
		{
			_pc["ShowProjectFilter"] = "True";
			BindDefaultValues();
			BindSavedData();
			BindDataGrid();
		}
		#endregion

		#region Change View
		void lbChangeViewDates_Click(object sender, EventArgs e)
		{
			_pc["pl_ViewStyle"] = FieldSetName.tProjectsLight.ToString();
			BindDataGrid();
		}

		void lbChangeViewDef_Click(object sender, EventArgs e)
		{
			_pc["pl_ViewStyle"] = FieldSetName.tProjectsDefault.ToString();
			BindDataGrid();
		}
		#endregion

		#region ExportGrid
		private void ExportGrid()
		{
			int i = 0;
			dgExport.Columns[i++].HeaderText = LocRM.GetString("Title");
			dgExport.Columns[i++].HeaderText = LocRM.GetString("PercentCompleted");
			dgExport.Columns[i++].HeaderText = LocRM.GetString("Status");
			dgExport.Columns[i++].HeaderText = LocRM.GetString("Priority");
			dgExport.Columns[i++].HeaderText = LocRM.GetString("StartDate");
			dgExport.Columns[i++].HeaderText = LocRM.GetString("FinishDate");
			dgExport.Columns[i++].HeaderText = LocRM.GetString("Manager");

			dgExport.Visible = true;
			BindDataGrid(dgExport);
			CommonHelper.ExportExcel(dgExport, "Projects.xls", null);
		}
		#endregion

		#region ExportXMLTable
		private void ExportXMLTable()
		{
			CommonHelper.SaveXML(BindDataGrid(dgExport), "Projects.xml");
		}
		#endregion
	}
}
