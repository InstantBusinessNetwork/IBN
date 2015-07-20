namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Collections;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using ComponentArt.Web.UI;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Core.Business;
	using Mediachase.Ibn.Clients;
	using System.Globalization;

	/// <summary>
	///		Summary description for Comments.
	/// </summary>
	public partial class IncidentsList : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentsList", typeof(IncidentsList).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidents", typeof(IncidentsList).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(IncidentsList).Assembly);
		protected UserLightPropertyCollection _pc = Security.CurrentUser.Properties;
		private int _projId = 0;
		private string _currentTab = "AllIncidents";
		private string _strPref = "IncIncidentList";
		private Hashtable _hash = new Hashtable();
		
		protected enum FieldSetName
		{
			tIncidentsDefault,
			tIncidentsLight
		}

		#region ProjectId
		protected int ProjectID
		{
			get
			{
				try
				{
					return Request["ProjectID"] != null ? int.Parse(Request["ProjectID"]) : 0;
				}
				catch
				{
					return 0;
				}
			}
		}
		#endregion

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (ProjectID > 0)
			{
				if (Request["Tab"] == "My")
					_currentTab = "MyIncidents";
			}
			else
				_currentTab = _pc[_strPref + "_CurrentTab"];

			ApplyLocalization();
			if (!Page.IsPostBack)
			{
				if (_pc["IncidentList_ViewStyle"] == null)
					_pc["IncidentList_ViewStyle"] = FieldSetName.tIncidentsDefault.ToString();

				BindDefaultValues();
			}
			BindSavedValues();
			BindInfoTable();

			if (Request["Export"] == "1")
				ExportGrid();
			else
				if (Request["Export"] == "2")
					ExportXMLTable();
				else
					BindDataGrid();
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			FilterTable.Visible = (_pc[_strPref + "ShowIncidentFilter"] != null && bool.Parse(_pc[_strPref + "ShowIncidentFilter"]));
			tblFilterInfo.Visible = !FilterTable.Visible;
			tblGroupInfo.Visible = ((_pc[_strPref + "ShowIncidentGroup"] != null && bool.Parse(_pc[_strPref + "ShowIncidentGroup"])) || Request.QueryString["IssGroup"] != null);

			if (_currentTab == "MyIncidents")
				tdCreatedBy.Visible = false;

			DataGrid dg = (dgGroupIncs.Visible) ? dgGroupIncs : ((dgGroupIncsByClient.Visible) ? dgGroupIncsByClient : dgIncidents);
			foreach (DataGridItem dgi in dg.Items)
			{
				if (int.Parse(dgi.Cells[0].Text) <= 0)
				{
					if (dg == dgGroupIncsByClient)
					{
						string s1 = dgi.Cells[1].Text;
						string s2 = dgi.Cells[2].Text;
						string _key = "";
						if (s1 != PrimaryKeyId.Empty.ToString())
							_key = "contact_" + s1;
						else if (s2 != PrimaryKeyId.Empty.ToString())
							_key = "org_" + s2;
						else
							_key = "noclient";

						dgi.Attributes.Add("onclick", _hash[_key].ToString());
						dgi.CssClass = "eecolor";
					}
					else
					{
						dgi.Attributes.Add("onclick", _hash[int.Parse(dgi.Cells[1].Text)].ToString());
						dgi.CssClass = "eecolor";
					}
				}
			}

			BindToolbar();
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblProject.Text = LocRM.GetString("Project");
			btnApply.Value = LocRM.GetString("tApply");
			btnReset.Value = LocRM.GetString("tReset");
			btnReset2.Value = LocRM.GetString("ResetFilter");
			btnReset3.Value = LocRM.GetString("ResetGroup");

			lblGenCats.Text = LocRM.GetString("GeneralCats") + ":";
			lblIssCats.Text = LocRM.GetString("IssuesCats") + ":";
			
			dgGroupIncs.Columns[2].HeaderText = "<span class='text' style='padding-left:30px'>" + LocRM.GetString("Title") + "</span>";
			dgGroupIncs.Columns[3].HeaderText = LocRM.GetString("tClient");
			dgGroupIncs.Columns[4].HeaderText = LocRM.GetString("tResponsible");
			dgGroupIncs.Columns[5].HeaderText = LocRM.GetString("tIssBox");

			dgGroupIncsByClient.Columns[3].HeaderText = "<span class='text' style='padding-left:30px'>" + LocRM.GetString("Title") + "</span>";
			dgGroupIncsByClient.Columns[4].HeaderText = LocRM.GetString("tClient");
			dgGroupIncsByClient.Columns[5].HeaderText = LocRM.GetString("tResponsible");
			dgGroupIncsByClient.Columns[6].HeaderText = LocRM.GetString("tIssBox");

			lblGroupInfo.Text = "<table><tr><td class='text' width='120px' align='right'>" + LocRM.GetString("tGroupBy") + ":&nbsp;" + "</td><td class='text'>" + LocRM.GetString("tProjects") + "</td></tr></table>";

			lbShowFilter.Text = String.Format(CultureInfo.InvariantCulture,
				"<img align='absmiddle' border='0' title='{0}' src='{1}' />",
				LocRM.GetString("tShowFilter"),
				ResolveClientUrl("~/Layouts/Images/scrolldown_hover.GIF"));
			lbHideFilter.Text = String.Format(CultureInfo.InvariantCulture,
				"<img align='absmiddle' border='0' title='{0}' src='{1}' />",
				LocRM.GetString("tHideFilter"),
				ResolveClientUrl("~/Layouts/Images/scrollup_hover.GIF"));

			lblFilterNotSet.Text = LocRM.GetString("FilterNotSet");

			lblOnlyNew.Text = LocRM.GetString("tUnansweredOnly");
		}
		#endregion

		#region BindInfoTable
		private void BindInfoTable()
		{
			int rowCount = 0;
			tblFilterInfoSet.Rows.Clear();

			// Status
			if (Request.QueryString["IssState"] != null)
			{
				ListItem li = ddState.Items.FindByValue(Request.QueryString["IssState"]);
				if (li != null && li.Value != "0")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("Status")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
			}
			else if (_pc[_strPref + "IncidentList_Status"] != null)
			{
				ListItem li = ddState.Items.FindByValue(_pc[_strPref + "IncidentList_Status"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Status")), li.Text);
					rowCount++;
				}
			}

			// Type
			if (Request.QueryString["IssType"] != null)
			{
				ListItem li = ddType.Items.FindByValue(Request.QueryString["IssType"]);
				if (li != null && li.Value != "0")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("Type")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
			}
			else if (_pc[_strPref + "IncidentList_Type"] != null)
			{
				ListItem li = ddType.Items.FindByValue(_pc[_strPref + "IncidentList_Type"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Type")), li.Text);
					rowCount++;
				}
			}

			// Manager
			if (_pc[_strPref + "IncidentList_Manager"] != null)
			{
				ListItem li = ddManager.Items.FindByValue(_pc[_strPref + "IncidentList_Manager"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Manager")), li.Text);
					rowCount++;
				}
			}

			// Creator
			if (_currentTab == "MyIncidents")
			{
				AddRow(
					String.Format("{0}:&nbsp; ", LocRM.GetString("CreatedBy")),
					String.Format("<span style='color:red'>{0} {1}</span>", Security.CurrentUser.LastName, Security.CurrentUser.FirstName));
			}
			else if (_pc[_strPref + "IncidentList_CreatedBy"] != null)
			{
				ListItem li = ddCreatedBy.Items.FindByValue(_pc[_strPref + "IncidentList_CreatedBy"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("CreatedBy")), li.Text);
					rowCount++;
				}
			}

			// Responsible
			if (Request.QueryString["RespIss"] != null)
			{
				AddRow(
					String.Format("{0}:&nbsp; ", LocRM.GetString("tResponsible")),
					String.Format("<span style='color:red'>{0} {1}</span>", Security.CurrentUser.LastName, Security.CurrentUser.FirstName));
			}
			else if (_pc[_strPref + "IncidentList_Responsible"] != null)
			{
				ListItem li = ddResponsible.Items.FindByValue(_pc[_strPref + "IncidentList_Responsible"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tResponsible")), li.Text);
					rowCount++;
				}
			}

			//Client
			if (_pc[_strPref + "IncidentList_ClientNew"] != null)
			{
				string ss = _pc[_strPref + "IncidentList_ClientNew"];
				if (ss.IndexOf("_") >= 0)
				{
					string stype = ss.Substring(0, ss.IndexOf("_"));

					string sName = "";
					if (stype.ToLower() == "org")
					{
						EntityObject entity = BusinessManager.Load(OrganizationEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1)));
						if (entity != null)
							sName = ((OrganizationEntity)entity).Name;
						AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tClient")), sName);
						rowCount++;
					}
					else if (stype.ToLower() == "contact")
					{
						EntityObject entity = BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1)));
						if (entity != null)
							sName = ((ContactEntity)entity).FullName;
						AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tClient")), sName);
						rowCount++;
					}
				}
			}

			//Resource
			if (Request.QueryString["AssIss"] != null)
			{
				AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tMyRole")),
					   String.Format("<span style='color:red'>{0}</span>", LocRM.GetString("tResource")));
			}

			//New / Not Assigned Internal Issues
			if (Request.QueryString["NewNotAss"] != null)
			{
				ObjectStates os = ObjectStates.Upcoming;
				ListItem li = ddState.Items.FindByValue(((int)os).ToString());
				if (li != null && li.Value != "0")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("Status")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
			}

			// Project
			if (ProjectID > 0)
			{
				AddRow(
					String.Format("{0}:&nbsp; ", LocRM.GetString("Project")),
					String.Format("<span style='color:red'>{0}</span>", Task.GetProjectTitle(ProjectID)));
			}
			else if (_pc[_strPref + "IncidentList_Project"] != null)
			{
				ListItem li = ddlProject.Items.FindByValue(_pc[_strPref + "IncidentList_Project"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Project")), li.Text);
					rowCount++;
				}
			}

			// Priority
			if (Request.QueryString["IssPrty"] != null)
			{
				ListItem li = ddPriority.Items.FindByValue(Request.QueryString["IssPrty"]);
				if (li != null && li.Value != "-1")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("Priority")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
			}
			else if (_pc[_strPref + "IncidentList_Priority"] != null)
			{
				ListItem li = ddPriority.Items.FindByValue(_pc[_strPref + "IncidentList_Priority"]);
				if (li != null && li.Value != "-1")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Priority")), li.Text);
					rowCount++;
				}
			}

			// Issue Box
			if (Request.QueryString["IssBox"] != null)
			{
				ListItem li = ddIssBox.Items.FindByValue(Request.QueryString["IssBox"]);
				if (li != null && li.Value != "-1")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("tIssBox")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
			}
			else if (_pc[_strPref + "IncidentList_IssBox"] != null)
			{
				ListItem li = ddIssBox.Items.FindByValue(_pc[_strPref + "IncidentList_IssBox"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tIssBox")), li.Text);
					rowCount++;
				}
			}

			// Keyword
			if (_pc[_strPref + "IncidentList_Keyword"] != null && _pc[_strPref + "IncidentList_Keyword"] != "")
			{
				AddRow(
					String.Format("{0}:&nbsp; ", LocRM.GetString("Keyword")),
					String.Format("'{0}'", _pc[_strPref + "IncidentList_Keyword"]));
				rowCount++;
			}

			// Severity
			if (Request.QueryString["IssSev"] != null)
			{
				ListItem li = ddSeverity.Items.FindByValue(Request.QueryString["IssSev"]);
				if (li != null && li.Value != "0")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("tSeverity")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
			}
			else if (_pc[_strPref + "IncidentList_Severity"] != null)
			{
				ListItem li = ddSeverity.Items.FindByValue(_pc[_strPref + "IncidentList_Severity"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tSeverity")), li.Text);
					rowCount++;
				}
			}

			// Unanswered
			if (_pc[_strPref + "IncidentList_Unanswered"] != null && bool.Parse(_pc[_strPref + "IncidentList_Unanswered"]))
			{
				AddRow("", LocRM.GetString("tUnansweredOnly"));
				rowCount++;
			}

			// General Categories
			if (Request.QueryString["GenCat"] != null)
			{
				ListItem li = lbGenCats.Items.FindByValue(Request.QueryString["GenCat"]);
				if (li != null)
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("GeneralCats")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
			}
			else
			{
				if (_pc[_strPref + "IncidentList_GenCatType"] != null)
				{
					ListItem li = ddGenCatType.Items.FindByValue(_pc[_strPref + "IncidentList_GenCatType"]);
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

			// Issue Categories
			if (Request.QueryString["IssCat"] != null)
			{
				ListItem li = lbIssCats.Items.FindByValue(Request.QueryString["IssCat"]);
				if (li != null)
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("IssuesCats")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
			}
			else
			{
				if (_pc[_strPref + "IncidentList_IssCatType"] != null)
				{
					ListItem li = ddIssCatType.Items.FindByValue(_pc[_strPref + "IncidentList_IssCatType"]);
					if (li != null && li.Value != "0")
					{
						string str = "";
						foreach (ListItem item in lbIssCats.Items)
						{
							if (item.Selected)
							{
								if (str != "")
									str += ", ";
								str += item.Text;
							}
						}

						if (li.Value == "2")
							AddRow(String.Format("{0} ({1}):&nbsp; ", LocRM.GetString("IssuesCats"), li.Text), str);
						else
							AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("IssuesCats")), str);
						rowCount++;
					}
				}
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

			// Grouping
			if (Request.QueryString["IssGroup"] != null)
			{
				if (Request.QueryString["IssGroup"] == "1")
				{
					lblGroupInfo.Text = "<table><tr><td class='text' width='120px' align='right'>" + LocRM.GetString("tGroupBy") + ":&nbsp;" + "</td><td class='text'><font color='red'>" + LocRM.GetString("tProjects") + "</font></td></tr></table>";
					btnReset3.Visible = false;
				}
				if (Request.QueryString["IssGroup"] == "2")
				{
					lblGroupInfo.Text = "<table><tr><td class='text' width='120px' align='right'>" + LocRM.GetString("tGroupBy") + ":&nbsp;" + "</td><td class='text'><font color='red'>" + LocRM.GetString("tClients") + "</font></td></tr></table>";
					btnReset3.Visible = false;
				}
			}
			else if (_pc[_strPref + "ShowIncidentGroup"] != null && bool.Parse(_pc[_strPref + "ShowIncidentGroup"]))
			{
				btnReset3.Visible = true;
			}
		}

		private void AddRow(string filterName, string filterValue)
		{
			HtmlTableRow tr = new HtmlTableRow();
			HtmlTableCell td1 = new HtmlTableCell();
			td1.Align = "Right";
			td1.Width = "140px";
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
			if (this.Parent.Parent is IToolbarLight)
			{
				BlockHeaderLightWithMenu secHeaderLight = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent.Parent).GetToolBar();

				secHeaderLight.ActionsMenu.Items.Clear();
				secHeaderLight.ClearRightItems();

				ComponentArt.Web.UI.MenuItem subItem;

				#region Issue Create
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.LookId = "TopItemLook";
				subItem.NavigateUrl = String.Format("~/Incidents/IncidentEdit.aspx?ProjectId={0}", ProjectID);
				subItem.Text = /*"<img border='0' src='../Layouts/Images/icons/incident_create.gif' width='16px' height='16px' align='absmiddle'/>&nbsp;" + */LocRM2.GetString("tbIncidentAdd");
				subItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/icons/incident_create.gif");
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				secHeaderLight.ActionsMenu.Items.Add(subItem);
				#endregion

				ComponentArt.Web.UI.MenuItem topMenuItem;

				if (Request.QueryString["IssGroup"] == null)
				{
					#region View Menu Items
					topMenuItem = new ComponentArt.Web.UI.MenuItem();
					topMenuItem.Text = LocRM.GetString("tView");
					topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
					topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
					topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
					topMenuItem.LookId = "TopItemLook";

					string sCurrentView = _pc["IncidentList_ViewStyle"];

					subItem = new ComponentArt.Web.UI.MenuItem();
					if (sCurrentView == FieldSetName.tIncidentsDefault.ToString())
					{
						subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
					}
					subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewDef, "");
					subItem.Text = LocRM.GetString(FieldSetName.tIncidentsDefault.ToString());
					topMenuItem.Items.Add(subItem);

					subItem = new ComponentArt.Web.UI.MenuItem();
					if (sCurrentView == FieldSetName.tIncidentsLight.ToString())
					{
						subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
					}
					subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewDates, "");
					subItem.Text = LocRM.GetString(FieldSetName.tIncidentsLight.ToString());
					topMenuItem.Items.Add(subItem);
					#endregion

					secHeaderLight.ActionsMenu.Items.Add(topMenuItem);
				}

				topMenuItem = new ComponentArt.Web.UI.MenuItem();
				topMenuItem.Text = LocRM2.GetString("ImportExport");
				topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
				topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
				topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
				topMenuItem.LookId = "TopItemLook";

				#region Excel Export
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/Icons/xlsexport.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = String.Format("~/Incidents/default.aspx?Export=1&ProjectId={0}", ProjectID);
				subItem.Text = LocRM2.GetString("Export");
				topMenuItem.Items.Add(subItem);
				#endregion

				#region XML Export
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/Icons/xmlexport.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = String.Format("~/Incidents/default.aspx?Export=2&ProjectId={0}", ProjectID);
				subItem.Text = LocRM2.GetString("XMLExport");
				topMenuItem.Items.Add(subItem);
				#endregion

				secHeaderLight.ActionsMenu.Items.Add(topMenuItem);

				secHeaderLight.EnsureRender();
			}
			else if (this.Parent.Parent is Incidents)
			{
				PageViewMenu secHeader = ((Incidents)this.Parent.Parent).ToolBar;
				if (secHeader != null)
				{
					secHeader.Title = LocRM2.GetString("tbIncidentsList");

					ComponentArt.Web.UI.MenuItem subItem;

					#region Issue Create
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.LookId = "TopItemLook";
					subItem.NavigateUrl = String.Format("~/Incidents/IncidentEdit.aspx?ProjectId={0}", ProjectID);
					subItem.Text = /*"<img border='0' src='../Layouts/Images/icons/incident_create.gif' width='16px' height='16px' align='absmiddle'/>&nbsp;" + */LocRM2.GetString("tbIncidentAdd");
					subItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/icons/incident_create.gif");
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					secHeader.ActionsMenu.Items.Add(subItem);
					#endregion

					ComponentArt.Web.UI.MenuItem topMenuItem;

					if (Request.QueryString["IssGroup"] == null)
					{
						#region View Menu Items
						topMenuItem = new ComponentArt.Web.UI.MenuItem();
						topMenuItem.Text = LocRM.GetString("tView");
						topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
						topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
						topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
						topMenuItem.LookId = "TopItemLook";

						string sCurrentView = _pc["IncidentList_ViewStyle"];

						subItem = new ComponentArt.Web.UI.MenuItem();
						if (sCurrentView == FieldSetName.tIncidentsDefault.ToString())
						{
							subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
							subItem.Look.LeftIconWidth = Unit.Pixel(16);
							subItem.Look.LeftIconHeight = Unit.Pixel(16);
						}
						subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewDef, "");
						subItem.Text = LocRM.GetString(FieldSetName.tIncidentsDefault.ToString());
						topMenuItem.Items.Add(subItem);

						subItem = new ComponentArt.Web.UI.MenuItem();
						if (sCurrentView == FieldSetName.tIncidentsLight.ToString())
						{
							subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
							subItem.Look.LeftIconWidth = Unit.Pixel(16);
							subItem.Look.LeftIconHeight = Unit.Pixel(16);
						}
						subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewDates, "");
						subItem.Text = LocRM.GetString(FieldSetName.tIncidentsLight.ToString());
						topMenuItem.Items.Add(subItem);
						#endregion

						secHeader.ActionsMenu.Items.Add(topMenuItem);
					}

					topMenuItem = new ComponentArt.Web.UI.MenuItem();
					topMenuItem.Text = LocRM2.GetString("ImportExport");
					topMenuItem.DefaultSubGroupExpandDirection = GroupExpandDirection.BelowRight;
					topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
					topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
					topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
					topMenuItem.LookId = "TopItemLook";

					#region Import
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/Layouts/Images/import.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.ClientSideCommand = "javascript:ImportWizard();";
					subItem.Text = LocRM2.GetString("tImport");
					topMenuItem.Items.Add(subItem);
					#endregion

					#region xlsexport
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/Layouts/Images/Icons/xlsexport.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.NavigateUrl = "~/Incidents/default.aspx?Export=1";
					subItem.Text = LocRM2.GetString("Export");
					topMenuItem.Items.Add(subItem);
					#endregion

					#region xmlexport
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/Layouts/Images/Icons/xmlexport.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.NavigateUrl = "~/Incidents/default.aspx?Export=2";
					subItem.Text = LocRM2.GetString("XMLExport");
					topMenuItem.Items.Add(subItem);
					#endregion

					secHeader.ActionsMenu.Items.Add(topMenuItem);
				}
			}
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			//Managers
			ddManager.DataSource = Incident.GetListIncidentManagers();
			ddManager.DataValueField = "UserId";
			ddManager.DataTextField = "UserName";
			ddManager.DataBind();

			ListItem lItem = new ListItem(LocRM.GetString("All"), "0");
			ddManager.Items.Insert(0, lItem);

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

			//Keyword
			tbKeyword.Text = String.Empty;

			//Creators
			ddCreatedBy.DataSource = Incident.GetListIncidentCreators();
			ddCreatedBy.DataTextField = "UserName";
			ddCreatedBy.DataValueField = "UserId";
			ddCreatedBy.DataBind();

			lItem = new ListItem(LocRM.GetString("All"), "0");
			ddCreatedBy.Items.Insert(0, lItem);

			//Responsibles
			ddResponsible.DataSource = User.GetListActive();
			ddResponsible.DataValueField = "PrincipalId";
			ddResponsible.DataTextField = "DisplayName";
			ddResponsible.DataBind();

			lItem = new ListItem(LocRM.GetString("All"), "0");
			ddResponsible.Items.Insert(0, lItem);

			lItem = new ListItem(LocRM.GetString("tRespNotAssigned"), "-1");
			ddResponsible.Items.Insert(1, lItem);

			//Client
			ClientControl.ObjectType = String.Empty;
			ClientControl.ObjectId = PrimaryKeyId.Empty;

			//States
			ddState.Items.Clear();
			using (IDataReader reader = Mediachase.IBN.Business.Incident.GetListIncidentStates())
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
			if (ProjectID == 0)
			{
				if (!Configuration.ProjectManagementEnabled)
				{
					tdProject.Visible = false;
					_pc[_strPref + "IncidentList_Project"] = "0";
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
			}
			else
				tdProject.Visible = false;

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

			if (!PortalConfig.GeneralAllowClientField)
			{
				tblClient.Visible = false;
				_pc[_strPref + "IncidentList_ClientNew"] = "_";
			}
			if (!PortalConfig.GeneralAllowPriorityField)
			{
				tblPriority.Visible = false;
				_pc[_strPref + "IncidentList_Priority"] = "-1";
			}
			if (!PortalConfig.GeneralAllowGeneralCategoriesField)
			{
				trCategories.Visible = false;
				_pc[_strPref + "IncidentList_GenCatType"] = "0";
			}
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			//Manager
			if (_pc[_strPref + "IncidentList_Manager"] != null)
				CommonHelper.SafeSelect(ddManager, _pc[_strPref + "IncidentList_Manager"]);

			//Responsible
			if (_pc[_strPref + "IncidentList_Responsible"] != null)
				CommonHelper.SafeSelect(ddResponsible, _pc[_strPref + "IncidentList_Responsible"]);

			//Priority
			if (_pc[_strPref + "IncidentList_Priority"] != null)
				CommonHelper.SafeSelect(ddPriority, _pc[_strPref + "IncidentList_Priority"]);

			//Issue Box
			if (_pc[_strPref + "IncidentList_IssBox"] != null)
				CommonHelper.SafeSelect(ddIssBox, _pc[_strPref + "IncidentList_IssBox"]);

			//Type
			if (_pc[_strPref + "IncidentList_Type"] != null)
				CommonHelper.SafeSelect(ddType, _pc[_strPref + "IncidentList_Type"]);

			//Keyword
			if (_pc[_strPref + "IncidentList_Keyword"] != null)
				tbKeyword.Text = _pc[_strPref + "IncidentList_Keyword"];

			//Project
			if (_pc[_strPref + "IncidentList_Project"] != null)
				CommonHelper.SafeSelect(ddlProject, _pc[_strPref + "IncidentList_Project"]);

			//Creator
			if (_pc[_strPref + "IncidentList_CreatedBy"] != null)
				CommonHelper.SafeSelect(ddCreatedBy, _pc[_strPref + "IncidentList_CreatedBy"]);

			//Status
			if (_pc[_strPref + "IncidentList_Status"] != null)
				CommonHelper.SafeSelect(ddState, _pc[_strPref + "IncidentList_Status"]);

			//Severity
			if (_pc[_strPref + "IncidentList_Severity"] != null)
				CommonHelper.SafeSelect(ddSeverity, _pc[_strPref + "IncidentList_Severity"]);

			//Client
			if (_pc[_strPref + "IncidentList_ClientNew"] != null && (!Page.IsPostBack || ClientControl.ObjectId == PrimaryKeyId.Empty))
			{
				string ss = _pc[_strPref + "IncidentList_ClientNew"];
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

			// General Categories
			if (_pc[_strPref + "IncidentList_GenCatType"] != null)
				CommonHelper.SafeSelect(ddGenCatType, _pc[_strPref + "IncidentList_GenCatType"]);

			if (ddGenCatType.SelectedItem.Value == "0")
				lbGenCats.Style["Display"] = "none";
			else
				lbGenCats.Style["Display"] = "block";

			using (IDataReader reader = Incident.GetListCategoriesByUser())
			{
				while (reader.Read())
					CommonHelper.SafeMultipleSelect(lbGenCats, reader["CategoryId"].ToString());
			}

			// Issue Categories
			if (_pc[_strPref + "IncidentList_IssCatType"] != null)
				CommonHelper.SafeSelect(ddIssCatType, _pc[_strPref + "IncidentList_IssCatType"]);

			if (ddIssCatType.SelectedItem.Value == "0")
				lbIssCats.Style["Display"] = "none";
			else
				lbIssCats.Style["Display"] = "block";

			using (IDataReader reader = Incident.GetListIncidentCategoriesByUser())
			{
				while (reader.Read())
					CommonHelper.SafeMultipleSelect(lbIssCats, reader["CategoryId"].ToString());
			}

			//Unanswered
			if (_pc[_strPref + "IncidentList_Unanswered"] != null)
				cbOnlyNewMess.Checked = bool.Parse(_pc[_strPref + "IncidentList_Unanswered"]);
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			_pc[_strPref + "IncidentList_Unanswered"] = cbOnlyNewMess.Checked.ToString();
			_pc[_strPref + "IncidentList_Manager"] = ddManager.SelectedValue;
			_pc[_strPref + "IncidentList_Responsible"] = ddResponsible.SelectedValue;
			_pc[_strPref + "IncidentList_Priority"] = ddPriority.SelectedValue;
			_pc[_strPref + "IncidentList_IssBox"] = ddIssBox.SelectedValue;
			_pc[_strPref + "IncidentList_Type"] = ddType.SelectedValue;
			_pc[_strPref + "IncidentList_Keyword"] = tbKeyword.Text;
			try
			{
				_pc[_strPref + "IncidentList_Project"] = ddlProject.SelectedValue;
			}
			catch
			{
			}
			_pc[_strPref + "IncidentList_CreatedBy"] = ddCreatedBy.SelectedValue;
			_pc[_strPref + "IncidentList_Status"] = ddState.SelectedValue;
			_pc[_strPref + "IncidentList_Severity"] = ddSeverity.SelectedValue;

			//Client
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc[_strPref + "IncidentList_ClientNew"] = "org_" + ClientControl.ObjectId.ToString();
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc[_strPref + "IncidentList_ClientNew"] = "contact_" + ClientControl.ObjectId.ToString();
			else
				_pc[_strPref + "IncidentList_ClientNew"] = "_";

			// General Categories
			_pc[_strPref + "IncidentList_GenCatType"] = ddGenCatType.SelectedValue;
			ArrayList alGenCats = new ArrayList();
			foreach (ListItem liItem in lbGenCats.Items)
			{
				if (liItem.Selected && !alGenCats.Contains(int.Parse(liItem.Value)))
					alGenCats.Add(int.Parse(liItem.Value));
			}
			Incident.SaveGeneralCategories(alGenCats);

			// Issue Categories
			_pc[_strPref + "IncidentList_IssCatType"] = ddIssCatType.SelectedItem.Value;
			ArrayList alIssCats = new ArrayList();
			foreach (ListItem liItem in lbIssCats.Items)
			{
				if (liItem.Selected && !alIssCats.Contains(int.Parse(liItem.Value)))
					alIssCats.Add(int.Parse(liItem.Value));
			}
			Incident.SaveIncidentCategories(alIssCats);
		}
		#endregion

		#region BindDataGrid (2 overload)
		private void BindDataGrid()
		{
			if ((_pc[_strPref + "ShowIncidentGroup"] != null && bool.Parse(_pc[_strPref + "ShowIncidentGroup"])) || (Request.QueryString["IssGroup"] != null && Request.QueryString["IssGroup"] == "1"))
			{
				dgIncidents.Visible = false;
				dgGroupIncs.Visible = true;
				dgGroupIncsByClient.Visible = false;
				BindDataGrid(dgGroupIncs);
			}
			else if ((_pc[_strPref + "ShowIncidentGroup"] != null && bool.Parse(_pc[_strPref + "ShowIncidentGroup"])) || (Request.QueryString["IssGroup"] != null && Request.QueryString["IssGroup"] == "2"))
			{
				dgIncidents.Visible = false;
				dgGroupIncs.Visible = false;
				dgGroupIncsByClient.Visible = true;
				BindDataGrid(dgGroupIncsByClient);
			}
			else
			{
				dgIncidents.Visible = true;
				dgGroupIncs.Visible = false;
				dgGroupIncsByClient.Visible = false;
				BindDataGrid(dgIncidents);
			}
		}

		private DataTable BindDataGrid(DataGrid dg)
		{
			_hash.Clear();

			#region Filter Params
			//Project
			if (ProjectID == 0)
				_projId = int.Parse(ddlProject.SelectedValue);
			else
				_projId = ProjectID;

			//Manager
			int iManId = int.Parse(ddManager.SelectedValue);

			//Responsible
			int iRespId = 0;
			if (Request.QueryString["RespIss"] != null)
			{
				iRespId = Security.CurrentUser.UserID;
				tdResponsible.Visible = false;
			}
			else if (ddResponsible.SelectedItem != null)
				iRespId = int.Parse(ddResponsible.SelectedValue);

			//Creator
			int iCreatorId = 0;
			if (_currentTab == "MyIncidents")
				iCreatorId = Security.CurrentUser.UserID;
			else if (ddCreatedBy.SelectedItem != null)
				iCreatorId = int.Parse(ddCreatedBy.SelectedValue);

			//Resource
			int iResId = 0;
			if (Request.QueryString["AssIss"] != null)
				iResId = Security.CurrentUser.UserID;

			//Keyword
			string sKeyword = tbKeyword.Text;

			// Priority
			int priority_id = 0;
			if (Request.QueryString["IssPrty"] != null)
				priority_id = int.Parse(Request.QueryString["IssPrty"]);
			else if (ddPriority.SelectedItem != null)
				priority_id = int.Parse(ddPriority.SelectedValue);

			// Issue Box
			int issbox_id = 0;
			if (Request.QueryString["IssBox"] != null)
			{
				issbox_id = int.Parse(Request.QueryString["IssBox"]);
				tdIssBox.Visible = false;
			}
			else if (ddIssBox.SelectedItem != null)
				issbox_id = int.Parse(ddIssBox.SelectedValue);

			// Issue Type
			int type_id = 0;
			if (Request.QueryString["IssType"] != null)
				type_id = int.Parse(Request.QueryString["IssType"]);
			else if (ddType.SelectedItem != null)
				type_id = int.Parse(ddType.SelectedValue);

			// Client
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName())
				orgUid = ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName())
				contactUid = ClientControl.ObjectId;

			// Issue State
			int state_id = 0;
			if (Request.QueryString["IssState"] != null)
				state_id = int.Parse(Request.QueryString["IssState"]);
			else if (Request.QueryString["NewNotAss"] != null)
				state_id = (int)ObjectStates.Upcoming;
			else if (ddState.SelectedItem != null)
				state_id = int.Parse(ddState.SelectedValue);

			// Issue Severity
			int severity_id = 0;
			if (Request.QueryString["IssSev"] != null)
				severity_id = int.Parse(Request.QueryString["IssSev"]);
			else if (ddSeverity.SelectedItem != null)
				severity_id = int.Parse(ddSeverity.SelectedValue);

			// General Category Type
			int genCategory_type = 0;
			if (Request.QueryString["GenCat"] != null)
				genCategory_type = -int.Parse(Request.QueryString["GenCat"]);
			else if (ddGenCatType.SelectedItem != null)
				genCategory_type = int.Parse(ddGenCatType.SelectedValue);

			// Issue Category Type
			int issCategory_type = 0;
			if (Request.QueryString["IssCat"] != null)
				issCategory_type = -int.Parse(Request.QueryString["IssCat"]);
			else if (ddIssCatType.SelectedItem != null)
				issCategory_type = int.Parse(ddIssCatType.SelectedValue);
			#endregion

			DataTable dt = new DataTable();
			DataView dv = new DataView();
			if ((_pc[_strPref + "ShowIncidentGroup"] != null && bool.Parse(_pc[_strPref + "ShowIncidentGroup"])) || (Request.QueryString["IssGroup"] != null && Request.QueryString["IssGroup"] == "1"))
			{
				dt = Incident.GetListIncidentsByFilterGroupedByProject
					(_projId, iManId, iCreatorId, iResId, iRespId, orgUid, contactUid, issbox_id, priority_id, type_id, state_id,
					severity_id, sKeyword, genCategory_type, issCategory_type);
				dv = dt.DefaultView;
			}
			else if ((_pc[_strPref + "ShowIncidentGroup"] != null && bool.Parse(_pc[_strPref + "ShowIncidentGroup"])) || (Request.QueryString["IssGroup"] != null && Request.QueryString["IssGroup"] == "2"))
			{
				dt = Incident.GetListIncidentsByFilterGroupedByClient
					(_projId, iManId, iCreatorId, iResId, iRespId, orgUid, contactUid, issbox_id, priority_id, type_id, state_id,
					severity_id, sKeyword, genCategory_type, issCategory_type);
				dv = dt.DefaultView;
			}
			else
			{
				dt = Incident.GetListIncidentsByFilterDataTable
					(_projId, iManId, iCreatorId, iResId, iRespId, orgUid, contactUid, issbox_id, priority_id, type_id, state_id,
					severity_id, sKeyword, genCategory_type, issCategory_type);
				foreach (DataRow dr in dt.Rows)
					if (dr["ResponsibleName"] == DBNull.Value)
					{
						if ((ObjectStates)dr["StateId"] == ObjectStates.Upcoming ||
							(ObjectStates)dr["StateId"] == ObjectStates.Suspended ||
							(ObjectStates)dr["StateId"] == ObjectStates.Completed)
							dr["ResponsibleName"] = dr["ManagerName"].ToString();
						else if ((ObjectStates)dr["StateId"] == ObjectStates.OnCheck)
							dr["ResponsibleName"] = dr["ControllerName"].ToString();
						else if (dr["IsResponsibleGroup"] != DBNull.Value && (bool)dr["IsResponsibleGroup"])
						{
							if ((int)dr["ResponsibleGroupState"] == 1)
								dr["ResponsibleName"] = "03";
							else
								dr["ResponsibleName"] = "02";

						}
						else
							dr["ResponsibleName"] = "01";
					}

				dv = dt.DefaultView;
				if (cbOnlyNewMess.Checked)
					dv.RowFilter = "IsNewMessage = 1";
				if (_pc[_strPref + "IncidentList_SortColumn"] == null)
					_pc[_strPref + "IncidentList_SortColumn"] = "ModifiedDate DESC";
				try
				{
					dv.Sort = _pc[_strPref + "IncidentList_SortColumn"];
				}
				catch { }
			}

			dg.DataSource = dv;

			if (_pc[_strPref + "IncidentList_PageSize"] != null)
				dg.PageSize = int.Parse(_pc[_strPref + "IncidentList_PageSize"]);

			if (_pc[_strPref + "IncidentList_Page"] != null)
			{
				int iPageIndex = int.Parse(_pc[_strPref + "IncidentList_Page"]);
				int ppi = dv.Count / dg.PageSize;
				if (dv.Count % dg.PageSize == 0)
					ppi = ppi - 1;
				if (iPageIndex <= ppi)
					dg.CurrentPageIndex = iPageIndex;
				else dg.CurrentPageIndex = 0;
			}

			if (Request.QueryString["IssGroup"] == null)
			{
				string fieldSet = _pc["IncidentList_ViewStyle"];

				switch (fieldSet)
				{
					case "tIncidentsLight":
						dg.Columns[2].Visible = false;
						dg.Columns[3].Visible = true;
						break;
					default:	//"tIncidentsDefault"
						dg.Columns[2].Visible = true;
						dg.Columns[3].Visible = false;
						break;
				}
			}

			dg.DataBind();

			if (dg != dgExport)
			{
				ArrayList incList = new ArrayList();
				foreach (DataGridItem dgi in dg.Items)
					if (int.Parse(dgi.Cells[0].Text) > 0)
						incList.Add(int.Parse(dgi.Cells[0].Text));
			}
			return dt;
		}
		#endregion

		#region Grid - Strings
		protected string GetTitleLink(string sTitle, int iIncId, int iStateId, string sStateName, bool isNewMessage, bool isOverdue)
		{
			if (isNewMessage)
				return CommonHelper.GetIncidentTitle(sTitle, iIncId, isOverdue, iStateId, sStateName, true);
			else
				return CommonHelper.GetIncidentTitle(sTitle, iIncId, isOverdue, iStateId, sStateName, false);
		}

		protected string GetClientLink(object OrgUid, object ContactUid, object ClientName)
		{
			string retVal = "";
			if (OrgUid != DBNull.Value && PrimaryKeyId.Parse(OrgUid.ToString()) != PrimaryKeyId.Empty)
				retVal = CommonHelper.GetOrganizationLink(this.Page, PrimaryKeyId.Parse(OrgUid.ToString()), ClientName.ToString());
			else if (ContactUid != DBNull.Value && PrimaryKeyId.Parse(ContactUid.ToString()) != PrimaryKeyId.Empty)
				retVal = CommonHelper.GetContactLink(this.Page, PrimaryKeyId.Parse(ContactUid.ToString()), ClientName.ToString());
			return retVal;
		}

		protected string GetIssBoxLink(object IssId, object IssBoxId, object IssBoxName)
		{
			return String.Format(CultureInfo.InvariantCulture,
				"<a href=\"javascript:OpenWindow(&quot;{0}?IssBoxId={1}&IncidentId={2}&quot;,500,375,false)\">{3}</a>",
				ResolveClientUrl("~/Incidents/IncidentBoxView.aspx"),
				IssBoxId.ToString(),
				IssId.ToString(),
				IssBoxName.ToString());
		}

		protected string GetTicket(int IssId, int IssBoxId, object sIdentifier)
		{
			if (sIdentifier != DBNull.Value && sIdentifier.ToString() != "")
				return sIdentifier.ToString();
			else
			{
				IncidentBox box = IncidentBox.Load(IssBoxId);
				return TicketUidUtil.Create(box.IdentifierMask, IssId);
			}
		}

		protected string GetResponsibleLink(object IncidentId, object StateId, object ResponsibleId, object ResponsibleName,
			object IsResponsibleGroup, object ResponsibleGroupState, object ManagerId, object ControllerId)
		{
			if ((ObjectStates)StateId == ObjectStates.Upcoming ||
				(ObjectStates)StateId == ObjectStates.Suspended ||
				(ObjectStates)StateId == ObjectStates.Completed)
				return Util.CommonHelper.GetUserStatus((int)ManagerId);
			if ((ObjectStates)StateId == ObjectStates.OnCheck)
			{
				if (ControllerId != DBNull.Value)
					return Util.CommonHelper.GetUserStatus((int)ControllerId);
				else
					return "";
			}
			if (ResponsibleId != DBNull.Value && (int)ResponsibleId > 0)
				return Util.CommonHelper.GetUserStatus((int)ResponsibleId);
			else if (IsResponsibleGroup != DBNull.Value && (bool)IsResponsibleGroup)
			{
				if ((int)ResponsibleGroupState == 1)
					return String.Format(CultureInfo.InvariantCulture,
						"<a href=\"javascript:OpenWindow(&quot;{0}?IncidentId={1}&notchange=1&quot;,600,{2},false)\"><img align='absmiddle' border='0' src='{3}' />&nbsp;{4}</a>",
						ResolveClientUrl("~/Incidents/ResponsiblePool.aspx"),
						IncidentId.ToString(), 
						(Request.Browser.Browser.ToLower().Contains("ie")) ? "320" : "310",
						ResolveUrl("~/layouts/images/waiting.gif"),
						LocRM3.GetString("tRespGroup"));
				else
					return String.Format(CultureInfo.InvariantCulture, 
						"<img align='absmiddle' border='0' src='{0}' />&nbsp;{1}",
						ResolveUrl("~/layouts/images/red_denied.gif"),
						LocRM3.GetString("tRespGroup"));
			}
			else
				return String.Format(CultureInfo.InvariantCulture, 
					"<img align='absmiddle' border='0' src='{0}' />&nbsp;{1}",
					ResolveUrl("~/layouts/images/not_set.png"),
					LocRM3.GetString("tRespNotSet"));
		}

		protected bool CanIncidentEdit(int IncidentId)
		{
			return Incident.CanUpdate(IncidentId);
		}

		protected bool CanIncidentDelete(int IncidentId)
		{
			return Incident.CanDelete(IncidentId);
		}

		protected string GetTaskToDoStatus(int PID, string Name)
		{
			string color = "PriorityNormal.gif";
			if (PID < 100) color = "PriorityLow.gif";
			if (PID > 500 && PID < 800) color = "PriorityHigh.gif";
			if (PID >= 800 && PID < 1000) color = "PriorityVeryHigh.gif";
			if (PID >= 1000) color = "PriorityUrgent.gif";
			Name = LocRM.GetString("Priority") + ":" + Name;
			return String.Format(CultureInfo.InvariantCulture,
				"<img width='16' height='16' src='{0}' alt='{1}' title='{1}'/>", 
				ResolveClientUrl("~/layouts/images/icons/" + color), 
				Name);
		}

		protected string GetIcon(bool IsExpand)
		{
			if (IsExpand)
				return "<img class='mousepointer' border=0 src='" + ResolveClientUrl("~/Layouts/images/minus.gif") + "'>";
			else
				return "<img class='mousepointer' border=0 src='" + ResolveClientUrl("~/Layouts/images/plus.gif") + "'>";
		}

		protected string GetTitle(bool IsPrj, bool IsCollapsed, int PrjId, int IncId, string IncTitle, int StatusId, string StatusName, bool isOverdue)
		{
			if (IsPrj)
			{
				string PrjStatus = "";
				if (PrjId > 0)
					PrjStatus = CommonHelper.GetProjectStatus(PrjId);
				else
					PrjStatus = "<span style='width:20px'>&nbsp;</span><font color='#003399'>" + LocRM.GetString("tNoProject") + "</font>";
				_hash.Add(PrjId, "CollapseExpand(" + (IsCollapsed ? "1" : "0") + "," + PrjId.ToString() + ", event)");
				return "<b>" + "&nbsp;" + GetIcon(!IsCollapsed) + "&nbsp;&nbsp;" + PrjStatus + "</b>";
			}
			else
			{
				return "<span style='width:40px'>&nbsp;</span>" + Util.CommonHelper.GetIncidentTitle(IncTitle, IncId, isOverdue, StatusId, StatusName);
			}
		}

		protected string GetTitleClient(bool IsClient, bool IsCollapsed, PrimaryKeyId contactUid, PrimaryKeyId orgUid,
			string ClientName, int IncId, string IncTitle, int StatusId, string StatusName, bool isOverdue)
		{
			if (IsClient)
			{
				string _client = "";
				string _key = "";
				if (contactUid != PrimaryKeyId.Empty)
				{
					_client = CommonHelper.GetContactLink(this.Page, contactUid, ClientName);
					_key = "contact_" + contactUid.ToString();
				}
				else if (orgUid != PrimaryKeyId.Empty)
				{
					_client = CommonHelper.GetOrganizationLink(this.Page, orgUid, ClientName);
					_key = "org_" + orgUid.ToString();
				}
				else
				{
					_client = "<span style='width:4px'>&nbsp;</span><font color='#003399'>" + LocRM.GetString("tNoClient") + "</font>";
					_key = "noclient";
				}

				_hash.Add(_key, "CollapseExpand2(" + (IsCollapsed ? "1" : "0") + ",'" + contactUid.ToString() + "','" + orgUid.ToString() + "', event)");
				return "<b>" + "&nbsp;" + GetIcon(!IsCollapsed) + "&nbsp;&nbsp;" + _client + "</b>";
			}
			else
				return "<span style='width:24px'>&nbsp;</span>" + Util.CommonHelper.GetIncidentTitle(IncTitle, IncId, isOverdue, StatusId, StatusName);
		}

		protected string GetOptionsString(bool IsPrj, int IncId)
		{
			string retval = "";
			if (!IsPrj)
			{
				if (Incident.CanUpdate(IncId))
				{
					retval = string.Format(CultureInfo.InvariantCulture,
						"<a href='{0}?IncidentID={1}' title='{2}'><img border='0' src='{3}' /></a>",
						ResolveClientUrl("~/Incidents/IncidentEdit.aspx"),
						IncId, 
						LocRM.GetString("tEdit"),
						ResolveClientUrl("~/layouts/images/Edit.GIF"));
				}
				if (Incident.CanDelete(IncId))
				{
					retval += string.Format(CultureInfo.InvariantCulture,
						"&nbsp;<a href='javascript:DeleteIncident({0})' title='{1}'><img border='0' src='{2}' /></a>", 
						IncId, 
						LocRM.GetString("tDelete"),
						ResolveClientUrl("~/layouts/images/delete.GIF"));
				}
			}
			return retval;
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
			//this.dgIncidents.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgIncidents_DeleteCommand);
			this.dgIncidents.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgIncidents_PageIndexChanged);
			this.dgIncidents.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgIncidents_SortCommand);
			this.dgIncidents.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dgIncidents_PageSizeChange);

			//this.dgGroupIncs.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgIncidents_DeleteCommand);
			this.dgGroupIncs.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgIncidents_PageIndexChanged);
			this.dgGroupIncs.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dgIncidents_PageSizeChange);

			this.dgGroupIncsByClient.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgIncidents_PageIndexChanged);
			this.dgGroupIncsByClient.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dgIncidents_PageSizeChange);

			this.lbChangeViewDef.Click += new EventHandler(lbChangeViewDef_Click);
			this.lbChangeViewDates.Click += new EventHandler(lbChangeViewDates_Click);
		}
		#endregion

		#region Collapse_Expand
		protected void Collapse_Expand_Click(object sender, System.EventArgs e)
		{
			string sType = hdnColType.Value;
			if (sType.ToLower() == "prj")
			{
				int PrjId = int.Parse(hdnIncidentId.Value);
				string CEType = hdnCollapseExpand.Value;
				if (CEType == "0")
					Incident.Collapse(PrjId);
				else
					Incident.Expand(PrjId);
			}
			else if (sType.ToLower() == "contact")
			{
				PrimaryKeyId contactUid = PrimaryKeyId.Parse(hdnIncidentId.Value);
				string CEType = hdnCollapseExpand.Value;
				if (CEType == "0")
					Incident.CollapseByClient(contactUid, PrimaryKeyId.Empty);
				else
					Incident.ExpandByClient(contactUid, PrimaryKeyId.Empty);
			}
			else if (sType.ToLower() == "org")
			{
				PrimaryKeyId orgUid = PrimaryKeyId.Parse(hdnIncidentId.Value);
				string CEType = hdnCollapseExpand.Value;
				if (CEType == "0")
					Incident.CollapseByClient(PrimaryKeyId.Empty, orgUid);
				else
					Incident.ExpandByClient(PrimaryKeyId.Empty, orgUid);
			}
			else if (sType.ToLower() == "noclient")
			{
				string CEType = hdnCollapseExpand.Value;
				if (CEType == "0")
					Incident.CollapseByClient(PrimaryKeyId.Empty, PrimaryKeyId.Empty);
				else
					Incident.ExpandByClient(PrimaryKeyId.Empty, PrimaryKeyId.Empty);
			}
			hdnColType.Value = "";
			hdnIncidentId.Value = "";
			BindDataGrid();
		}
		#endregion

		#region dgIncidents Events
		private void dgIncidents_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			_pc[_strPref + "IncidentList_PageSize"] = e.NewPageSize.ToString();
			BindDataGrid();
		}

		private void dgIncidents_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			_pc[_strPref + "IncidentList_Page"] = e.NewPageIndex.ToString();
			BindDataGrid();
		}

		private void dgIncidents_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if ((_pc[_strPref + "IncidentList_SortColumn"] != null) && (_pc[_strPref + "IncidentList_SortColumn"].ToString() == (string)e.SortExpression))
				_pc[_strPref + "IncidentList_SortColumn"] = (string)e.SortExpression + " DESC";
			else
				_pc[_strPref + "IncidentList_SortColumn"] = (string)e.SortExpression;
			BindDataGrid();
		}
		#endregion

		#region Apply - Reset
		protected void btnApply_ServerClick(object sender, System.EventArgs e)
		{
			SaveValues();
			BindSavedValues();
			BindInfoTable();
			BindDataGrid();
		}


		protected void btnReset_ServerClick(object sender, System.EventArgs e)
		{
			BindDefaultValues();
			SaveValues();
			BindInfoTable();
			BindDataGrid();
		}

		protected void btnResetGroup_ServerClick(object sender, System.EventArgs e)
		{
			_pc[_strPref + "ShowIncidentGroup"] = "False";
			BindDataGrid();
		}
		#endregion

		#region Show - Hide
		protected void lbHideFilter_Click(object sender, System.EventArgs e)
		{
			_pc[_strPref + "ShowIncidentFilter"] = "False";
			BindDefaultValues();
			BindSavedValues();
			BindDataGrid();
		}

		protected void lbShowFilter_Click(object sender, System.EventArgs e)
		{
			_pc[_strPref + "ShowIncidentFilter"] = "True";
			BindDefaultValues();
			BindSavedValues();
			BindDataGrid();
		}

		protected void lbShowGroup_Click(object sender, System.EventArgs e)
		{
			if (_strPref == "Inc")
				_pc[_strPref + "ShowIncidentGroup"] = "True";
			BindDataGrid();
		}
		#endregion

		#region Change View
		void lbChangeViewDates_Click(object sender, EventArgs e)
		{
			_pc["IncidentList_ViewStyle"] = FieldSetName.tIncidentsLight.ToString();
			BindDataGrid();
		}

		void lbChangeViewDef_Click(object sender, EventArgs e)
		{
			_pc["IncidentList_ViewStyle"] = FieldSetName.tIncidentsDefault.ToString();
			BindDataGrid();
		}
		#endregion

		#region Delete
		protected void lblDeleteIncidentAll_Click(object sender, System.EventArgs e)
		{
			int IncidentId = int.Parse(hdnIncidentId.Value);
			Incident.Delete(IncidentId);
			BindDataGrid();
		}
		#endregion

		#region Export
		private void ExportGrid()
		{
			dgExport.Columns[0].HeaderText = LocRM.GetString("Title");
			dgExport.Columns[1].HeaderText = LocRM.GetString("Priority");
			dgExport.Columns[2].HeaderText = LocRM.GetString("Status");
			dgExport.Columns[3].HeaderText = LocRM.GetString("ModifiedDate");
			dgExport.Columns[4].HeaderText = LocRM.GetString("CreatedBy");
			dgExport.Columns[5].HeaderText = LocRM.GetString("Manager");

			dgExport.Visible = true;
			BindDataGrid(dgExport);
			CommonHelper.ExportExcel(dgExport, "Issues.xls", null);
		}

		private void ExportXMLTable()
		{
			CommonHelper.SaveXML(BindDataGrid(dgExport), "Issues.xml");
		}
		#endregion
	}
}
