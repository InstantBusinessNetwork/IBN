namespace Mediachase.UI.Web.Documents.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using ComponentArt.Web.UI;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Core.Business;
	using Mediachase.Ibn.Clients;

	/// <summary>
	///		Summary description for DocumentList.
	/// </summary>
	public partial class DocumentList : System.Web.UI.UserControl
	{
		private string _strPref = "Doc";
		private Hashtable _hash = new Hashtable();
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(DocumentList).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(DocumentList).Assembly);
		UserLightPropertyCollection _pc = Security.CurrentUser.Properties;

		#region ProjectID
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
			if (ProjectID != 0)
				_strPref = "Prj";

			ApplyLocalization();
			if (!Page.IsPostBack)
				BindDefaultValues();
			BindSavedValues();
			BindInfoTable();

			if (Request["Export"] == "1")
				ExportGrid();
			else if (Request["Export"] == "2")
				ExportXMLTable();
			else
				BindDataGrid();

			BindToolBar();
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			FilterTable.Visible = (_pc[_strPref + "ShowDocumentFilter"] != null && bool.Parse(_pc[_strPref + "ShowDocumentFilter"]));
			tblFilterInfo.Visible = !FilterTable.Visible;
			tblGroupInfo.Visible = ((_pc[_strPref + "ShowDocumentGroup"] != null && bool.Parse(_pc[_strPref + "ShowDocumentGroup"])) || Request.QueryString["IssGroup"] != null);

			DataGrid dg = (dgGroupDocs.Visible) ? dgGroupDocs : ((dgGroupDocsByClient.Visible) ? dgGroupDocsByClient : dgDocuments);
			foreach (DataGridItem dgi in dg.Items)
			{
				if (int.Parse(dgi.Cells[0].Text) <= 0)
				{
					if (dg == dgGroupDocsByClient)
					{
						string s1 = dgi.Cells[1].Text;
						string s2 = dgi.Cells[2].Text;
						string key = "";
						if (s1 != PrimaryKeyId.Empty.ToString())
							key = "contact_" + s1;
						else if (s2 != PrimaryKeyId.Empty.ToString())
							key = "org_" + s2;
						else
							key = "noclient";

						dgi.Attributes.Add("onclick", _hash[key].ToString());
						dgi.BackColor = Color.FromArgb(238, 238, 238);
						dgi.Cells[3].ColumnSpan = 5;
						dgi.Cells[4].Visible = dgi.Cells[5].Visible = dgi.Cells[6].Visible = dgi.Cells[7].Visible = false;
					}
					else
					{
						dgi.Attributes.Add("onclick", _hash[int.Parse(dgi.Cells[1].Text)].ToString());
						dgi.BackColor = Color.FromArgb(238, 238, 238);
						dgi.Cells[2].ColumnSpan = 5;
						dgi.Cells[3].Visible = dgi.Cells[4].Visible = dgi.Cells[5].Visible = dgi.Cells[6].Visible = false;
					}
				}
			}
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnApply.Value = LocRM.GetString("tApply");
			btnReset.Value = LocRM.GetString("tReset");
			btnReset2.Value = LocRM.GetString("ResetFilter");
			btnReset3.Value = LocRM.GetString("ResetGroup");

			dgGroupDocs.Columns[2].HeaderText = "<span class='text' style='padding-left:30px'>" + LocRM.GetString("Title") + "</span>";
			dgGroupDocs.Columns[3].HeaderText = LocRM.GetString("Priority");
			dgGroupDocs.Columns[4].HeaderText = LocRM.GetString("Status");
			dgGroupDocs.Columns[5].HeaderText = LocRM.GetString("CreatedDate");
			dgGroupDocs.Columns[6].HeaderText = LocRM.GetString("Manager");

			dgGroupDocsByClient.Columns[3].HeaderText = "<span class='text' style='padding-left:30px'>" + LocRM.GetString("Title") + "</span>";
			dgGroupDocsByClient.Columns[4].HeaderText = LocRM.GetString("Priority");
			dgGroupDocsByClient.Columns[5].HeaderText = LocRM.GetString("Status");
			dgGroupDocsByClient.Columns[6].HeaderText = LocRM.GetString("CreatedDate");
			dgGroupDocsByClient.Columns[7].HeaderText = LocRM.GetString("Manager");

			lblGroupInfo.Text = "<table><tr><td class='text' width='120px' align='right'>" + LocRM.GetString("tGroupBy") + ":&nbsp;" + "</td><td class='text'>" + LocRM.GetString("tProjects") + "</td></tr></table>";

			lbShowFilter.Text = "<img align='absmiddle' border='0' title='" + LocRM.GetString("ShowFilter") + "' src='../Layouts/Images/scrolldown_hover.GIF' />";
			lbHideFilter.Text = "<img align='absmiddle' border='0' title='" + LocRM.GetString("HideFilter") + "' src='../Layouts/Images/scrollup_hover.GIF' />";

			lblFilterNotSet.Text = LocRM.GetString("FilterNotSet");
		}
		#endregion

		#region BindInfoTable
		private void BindInfoTable()
		{
			int rowCount = 0;
			tblFilterInfoSet.Rows.Clear();

			// Status
			if (Request.QueryString["DocStatus"] != null)
			{
				ListItem li = ddStatus.Items.FindByValue(Request.QueryString["DocStatus"]);
				if (li != null && li.Value != "0")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("Status")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
			}
			else if (_pc[_strPref + "DocumentList_Status"] != null)
			{
				ListItem li = ddStatus.Items.FindByValue(_pc[_strPref + "DocumentList_Status"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Status")), li.Text);
					rowCount++;
				}
			}

			// Manager
			if (Request.QueryString["ManDoc"] != null)
			{
				ListItem li = ddManager.Items.FindByValue(Security.CurrentUser.UserID.ToString());
				if (li != null && li.Value != "-1")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("Manager")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
			}
			else if (_pc[_strPref + "DocumentList_Manager"] != null)
			{
				ListItem li = ddManager.Items.FindByValue(_pc[_strPref + "DocumentList_Manager"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Manager")), li.Text);
					rowCount++;
				}
			}

			//Resource
			if (Request.QueryString["AssDoc"] != null)
			{
				AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tMyRole")),
					String.Format("<span style='color:red'>{0}</span>", LocRM.GetString("tResource")));
			}

			// Project
			if (ProjectID > 0)
			{
				AddRow(
					String.Format("{0}:&nbsp; ", LocRM.GetString("Project")),
					String.Format("<span style='color:red'>{0}</span>", Project.GetProjectTitle(ProjectID)));
			}
			else if (_pc[_strPref + "DocumentList_Project"] != null)
			{
				ListItem li = ddlProject.Items.FindByValue(_pc[_strPref + "DocumentList_Project"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Project")), li.Text);
					rowCount++;
				}
			}

			// Priority
			if (Request.QueryString["DocPrty"] != null)
			{
				ListItem li = ddPriority.Items.FindByValue(Request.QueryString["DocPrty"]);
				if (li != null && li.Value != "-1")
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("Priority")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
			}
			else if (_pc[_strPref + "DocumentList_Priority"] != null)
			{
				ListItem li = ddPriority.Items.FindByValue(_pc[_strPref + "DocumentList_Priority"]);
				if (li != null && li.Value != "-1")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Priority")), li.Text);
					rowCount++;
				}
			}

			//Client
			if (_pc[_strPref + "DocumentList_ClientNew"] != null)
			{
				string ss = _pc[_strPref + "DocumentList_ClientNew"];
				if (ss.IndexOf("_") >= 0)
				{
					string stype = ss.Substring(0, ss.IndexOf("_"));

					string sName = "";
					if (stype.ToLower() == "org")
					{
						EntityObject entity = BusinessManager.Load(OrganizationEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1)));
						if (entity != null)
							sName = ((OrganizationEntity)entity).Name;
						AddRow(String.Format("{0}:&nbsp; ", LocRM2.GetString("Client")), sName);
						rowCount++;
					}
					else if (stype.ToLower() == "contact")
					{
						EntityObject entity = BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1)));
						if (entity != null)
							sName = ((ContactEntity)entity).FullName;
						AddRow(String.Format("{0}:&nbsp; ", LocRM2.GetString("Client")), sName);
						rowCount++;
					}
				}
			}

			// Keyword
			if (_pc[_strPref + "DocumentList_Keyword"] != null && _pc[_strPref + "DocumentList_Keyword"] != "")
			{
				AddRow(
					String.Format("{0}:&nbsp; ", LocRM.GetString("Keyword")),
					String.Format("'{0}'", _pc[_strPref + "DocumentList_Keyword"]));
				rowCount++;
			}

			// General Categories
			if (Request.QueryString["GenCat"] != null)
			{
				ListItem li = lbGenCats.Items.FindByValue(Request.QueryString["GenCat"]);
				if (li != null)
					AddRow(
						String.Format("{0}:&nbsp; ", LocRM.GetString("Category")),
						String.Format("<span style='color:red'>{0}</span>", li.Text));
			}
			else
			{
				if (_pc[_strPref + "DocumentList_GenCatType"] != null)
				{
					ListItem li = ddGenCatType.Items.FindByValue(_pc[_strPref + "DocumentList_GenCatType"]);
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
							AddRow(String.Format("{0} ({1}):&nbsp; ", LocRM.GetString("Category"), li.Text), str);
						else
							AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Category")), str);
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
			if (Request.QueryString["DocGroup"] != null)
			{
				if (Request.QueryString["IssGroup"] == "Prj")
				{
					lblGroupInfo.Text = "<table><tr><td class='text' width='120px' align='right'>" + LocRM.GetString("tGroupBy") + ":&nbsp;" + "</td><td class='text'><font color='red'>" + LocRM.GetString("tProjects") + "</font></td></tr></table>";
					btnReset3.Visible = false;
				}
				if (Request.QueryString["IssGroup"] == "Client")
				{
					lblGroupInfo.Text = "<table><tr><td class='text' width='120px' align='right'>" + LocRM.GetString("tGroupBy") + ":&nbsp;" + "</td><td class='text'><font color='red'>" + LocRM.GetString("tClients") + "</font></td></tr></table>";
					btnReset3.Visible = false;
				}
			}
			else if (_pc[_strPref + "ShowDocumentGroup"] != null && bool.Parse(_pc[_strPref + "ShowDocumentGroup"]))
			{
				btnReset3.Visible = true;
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

		#region BindToolBar
		private void BindToolBar()
		{
			if (this.Parent is IPageViewMenu)
			{
				PageViewMenu secHeader = (PageViewMenu)((IPageViewMenu)this.Parent).GetToolBar();
				secHeader.Title = LocRM.GetString("tbDocumentsList");

				GenerateMenu(secHeader.ActionsMenu);
			}
			else if (this.Parent is IToolbarLight)
			{
				BlockHeaderLightWithMenu secHeader = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent).GetToolBar();
				secHeader.AddText(LocRM.GetString("tbDocumentsList"));

				GenerateMenu(secHeader.ActionsMenu);
			}

			else if (this.Parent.Parent is IToolbarLight)
			{
				BlockHeaderLightWithMenu secHeader = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent.Parent).GetToolBar();
				GenerateMenu(secHeader.ActionsMenu);
			}
		}

		private void GenerateMenu(ComponentArt.Web.UI.Menu menu)
		{
			ComponentArt.Web.UI.MenuItem subItem;

			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.LookId = "TopItemLook";
			subItem.NavigateUrl = "../Documents/DocumentEdit.aspx?ProjectID=" + ProjectID;
			subItem.Text = LocRM.GetString("tDocumentAdd");
			subItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/icons/document_create.gif");
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			menu.Items.Add(subItem);

			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = LocRM2.GetString("Export");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/xlsexport.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			subItem.NavigateUrl = "../Documents/default.aspx?Export=1&ProjectID=" + ProjectID;
			subItem.Text = LocRM.GetString("Export");
			topMenuItem.Items.Add(subItem);

			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/xmlexport.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			subItem.NavigateUrl = "../Documents/default.aspx?Export=2&ProjectID=" + ProjectID;
			subItem.Text = LocRM.GetString("XMLExport");
			topMenuItem.Items.Add(subItem);

			menu.Items.Add(topMenuItem);
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			//Managers
			ddManager.DataSource = Document.GetListDocumentManagers();
			ddManager.DataTextField = "UserName";
			ddManager.DataValueField = "UserId";
			ddManager.DataBind();

			ListItem lItem = new ListItem(LocRM.GetString("All"), "0");
			ddManager.Items.Insert(0, lItem);

			//Priorities
			ddPriority.DataSource = Document.GetListPriorities();
			ddPriority.DataTextField = "PriorityName";
			ddPriority.DataValueField = "PriorityId";
			ddPriority.DataBind();
			lItem = new ListItem(LocRM.GetString("All"), "-1");
			ddPriority.Items.Insert(0, lItem);

			//Keyword
			tbKeyword.Text = String.Empty;


			//Statuses
			ddStatus.DataSource = Document.GetListDocumentStatus();
			ddStatus.DataTextField = "StatusName";
			ddStatus.DataValueField = "StatusId";
			ddStatus.DataBind();

			lItem = new ListItem(LocRM.GetString("All"), "0");
			ddStatus.Items.Insert(0, lItem);

			//Projects
			if (ProjectID == 0)
			{
				if (!Configuration.ProjectManagementEnabled)
				{
					tdProject.Visible = false;
					_pc[_strPref + "DocumentList_Project"] = "0";
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

			//Client
			ClientControl.ObjectType = String.Empty;
			ClientControl.ObjectId = PrimaryKeyId.Empty;

			if (!PortalConfig.GeneralAllowPriorityField)
			{
				tblPriority.Visible = false;
				_pc[_strPref + "DocumentList_Priority"] = "-1";
			}
			if (!PortalConfig.GeneralAllowClientField)
			{
				tblClient.Visible = false;
				_pc[_strPref + "DocumentList_ClientNew"] = "_";
			}
			if (!PortalConfig.GeneralAllowGeneralCategoriesField)
			{
				tdCategories.Visible = false;
				_pc[_strPref + "DocumentList_GenCatType"] = "0";
			}

			if (ddGenCatType.SelectedItem.Value == "0")
				lbGenCats.Style["Display"] = "none";
			else
				lbGenCats.Style["Display"] = "block";
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			//Manager
			if (_pc[_strPref + "DocumentList_Manager"] != null)
				CommonHelper.SafeSelect(ddManager, _pc[_strPref + "DocumentList_Manager"]);

			//Priority
			if (_pc[_strPref + "DocumentList_Priority"] != null)
				CommonHelper.SafeSelect(ddPriority, _pc[_strPref + "DocumentList_Priority"]);

			//Keyword
			if (_pc[_strPref + "DocumentList_Keyword"] != null)
				tbKeyword.Text = _pc[_strPref + "DocumentList_Keyword"];

			//Project
			if (_pc[_strPref + "DocumentList_Project"] != null)
				CommonHelper.SafeSelect(ddlProject, _pc[_strPref + "DocumentList_Project"]);

			//Status
			if (_pc[_strPref + "DocumentList_Status"] != null)
				CommonHelper.SafeSelect(ddStatus, _pc[_strPref + "DocumentList_Status"]);

			//Client
			if (_pc[_strPref + "DocumentList_ClientNew"] != null && (!Page.IsPostBack || ClientControl.ObjectId == PrimaryKeyId.Empty))
			{
				string ss = _pc[_strPref + "DocumentList_ClientNew"];
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
			if (_pc[_strPref + "DocumentList_GenCatType"] != null)
				CommonHelper.SafeSelect(ddGenCatType, _pc[_strPref + "DocumentList_GenCatType"]);

			if (ddGenCatType.SelectedItem.Value == "0")
				lbGenCats.Style["Display"] = "none";
			else
				lbGenCats.Style["Display"] = "block";

			using (IDataReader reader = Document.GetListCategoriesByUser())
			{
				while (reader.Read())
					CommonHelper.SafeMultipleSelect(lbGenCats, reader["CategoryId"].ToString());
			}
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			_pc[_strPref + "DocumentList_Manager"] = ddManager.SelectedValue;
			_pc[_strPref + "DocumentList_Priority"] = ddPriority.SelectedValue;
			_pc[_strPref + "DocumentList_Keyword"] = tbKeyword.Text;
			try
			{
				_pc[_strPref + "DocumentList_Project"] = ddlProject.SelectedValue;
			}
			catch
			{
			}
			_pc[_strPref + "DocumentList_Status"] = ddStatus.SelectedValue;

			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc[_strPref + "DocumentList_ClientNew"] = "org_" + ClientControl.ObjectId.ToString();
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc[_strPref + "DocumentList_ClientNew"] = "contact_" + ClientControl.ObjectId.ToString();
			else
				_pc[_strPref + "DocumentList_ClientNew"] = "_";

			// General Categories
			_pc[_strPref + "DocumentList_GenCatType"] = ddGenCatType.SelectedValue;
			ArrayList alGenCats = new ArrayList();
			foreach (ListItem liItem in lbGenCats.Items)
			{
				if (liItem.Selected && !alGenCats.Contains(int.Parse(liItem.Value)))
					alGenCats.Add(int.Parse(liItem.Value));
			}
			Document.SaveGeneralCategories(alGenCats);
		}
		#endregion

		#region BindDataGrid (2 overload)
		private void BindDataGrid()
		{
			if ((_pc[_strPref + "ShowDocumentGroup"] != null && bool.Parse(_pc[_strPref + "ShowDocumentGroup"])) || (Request.QueryString["DocGroup"] != null && Request.QueryString["DocGroup"] == "Prj"))
			{
				dgDocuments.Visible = false;
				dgGroupDocs.Visible = true;
				dgGroupDocsByClient.Visible = false;
				BindDataGrid(dgGroupDocs);
			}
			else if ((_pc[_strPref + "ShowDocumentGroup"] != null && bool.Parse(_pc[_strPref + "ShowDocumentGroup"])) || (Request.QueryString["DocGroup"] != null && Request.QueryString["DocGroup"] == "Client"))
			{
				dgDocuments.Visible = false;
				dgGroupDocs.Visible = false;
				dgGroupDocsByClient.Visible = true;
				BindDataGrid(dgGroupDocsByClient);
			}
			else
			{
				dgDocuments.Visible = true;
				dgGroupDocs.Visible = false;
				dgGroupDocsByClient.Visible = false;
				BindDataGrid(dgDocuments);
			}
		}

		private DataTable BindDataGrid(DataGrid dg)
		{
			_hash.Clear();

			#region Filter Params
			int projId = ProjectID;
			//Project
			if (ProjectID == 0)
				projId = int.Parse(ddlProject.SelectedValue);

			//Manager
			int manId = 0;
			if (Request.QueryString["ManDoc"] != null)
				manId = Security.CurrentUser.UserID;
			manId = int.Parse(ddManager.SelectedValue);

			//Resource
			int resId = 0;
			if (Request.QueryString["AssDoc"] != null)
				resId = Security.CurrentUser.UserID;

			//Keyword
			string keyword = tbKeyword.Text;

			// Priority
			int priority_id = 0;
			if (Request.QueryString["DocPrty"] != null)
				priority_id = int.Parse(Request.QueryString["DocPrty"]);
			else if (ddPriority.SelectedItem != null)
				priority_id = int.Parse(ddPriority.SelectedValue);

			// Status
			int status_id = 0;
			if (Request.QueryString["DocStatus"] != null)
				status_id = int.Parse(Request.QueryString["DocStatus"]);
			else if (ddStatus.SelectedItem != null)
				status_id = int.Parse(ddStatus.SelectedValue);

			// General Category Type
			int genCategory_type = 0;
			if (Request.QueryString["GenCat"] != null)
				genCategory_type = -int.Parse(Request.QueryString["GenCat"]);
			else if (ddGenCatType.SelectedItem != null)
				genCategory_type = int.Parse(ddGenCatType.SelectedValue);

			// Client
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName())
				orgUid = ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName())
				contactUid = ClientControl.ObjectId;
			#endregion

			DataTable dt = new DataTable();
			DataView dv = new DataView();
			if ((_pc[_strPref + "ShowDocumentGroup"] != null && bool.Parse(_pc[_strPref + "ShowDocumentGroup"])) || (Request.QueryString["DocGroup"] != null && Request.QueryString["DocGroup"] == "Prj"))
			{
				dt = Document.GetListDocumentsByFilterGroupedByProject(projId, manId,
					resId, priority_id, status_id, keyword, genCategory_type, orgUid, contactUid);
				dv = dt.DefaultView;
			}
			else if ((_pc[_strPref + "ShowDocumentGroup"] != null && bool.Parse(_pc[_strPref + "ShowDocumentGroup"])) || (Request.QueryString["DocGroup"] != null && Request.QueryString["DocGroup"] == "Client"))
			{
				dt = Document.GetListDocumentsByFilterGroupedByClient(projId, manId,
					resId, priority_id, status_id, keyword, genCategory_type, orgUid, contactUid);
				dv = dt.DefaultView;
			}
			else
			{
				dt = Document.GetListDocumentsByFilterDataTable(projId, manId, resId, priority_id, status_id, keyword, genCategory_type, contactUid, orgUid);
				dv = dt.DefaultView;
				if (_pc[_strPref + "DocumentList_SortColumn"] == null)
					_pc[_strPref + "DocumentList_SortColumn"] = "CreationDate DESC";
				try
				{
					dv.Sort = _pc[_strPref + "DocumentList_SortColumn"];
				}
				catch { }
			}

			dg.DataSource = dv;

			if (_pc[_strPref + "DocumentList_PageSize"] != null)
				dg.PageSize = int.Parse(_pc[_strPref + "DocumentList_PageSize"]);

			if (_pc[_strPref + "DocumentList_Page"] != null)
			{
				int pageIndex = int.Parse(_pc[_strPref + "DocumentList_Page"]);
				int ppi = dv.Count / dg.PageSize;
				if (dv.Count % dg.PageSize == 0)
					ppi = ppi - 1;
				if (pageIndex <= ppi)
					dg.CurrentPageIndex = pageIndex;
				else dg.CurrentPageIndex = 0;
			}
			dg.DataBind();
			return dt;
		}
		#endregion

		#region Grid - Strings
		protected bool GetBool(int val)
		{
			if (val == 1)
				return true;
			else
				return false;
		}

		protected string GetPriority(int pId, string name)
		{
			string color = "PriorityNormal.gif";
			if (pId < 100) color = "PriorityLow.gif";
			if (pId > 500 && pId < 800) color = "PriorityHigh.gif";
			if (pId >= 800 && pId < 1000) color = "PriorityVeryHigh.gif";
			if (pId >= 1000) color = "PriorityUrgent.gif";
			name = LocRM.GetString("Priority") + ":" + name;
			return String.Format("<img width='16' height='16' align='absmiddle' src='../layouts/images/icons/{0}' alt='{1}' title='{1}'/>", color, name);
		}

		protected string GetIcon(bool isExpand)
		{
			if (isExpand)
				return "<img border=0 src='" + ResolveUrl("~/Layouts/images/minus.gif") + "'>";
			else
				return "<img border=0 src='" + ResolveUrl("~/Layouts/images/plus.gif") + "'>";
		}

		protected string GetTitle(bool isPrj, bool isCollapsed, int prjId, int docId, string title)
		{
			if (isPrj)
			{
				string prjStatus = "";
				if (prjId > 0)
					prjStatus = CommonHelper.GetProjectStatus(prjId);
				else
					prjStatus = "<span style='width:20px'>&nbsp;</span><font color='#003399'>" + LocRM.GetString("tNoProject") + "</font>";
				_hash.Add(prjId, "CollapseExpand(" + (isCollapsed ? "1" : "0") + "," + prjId.ToString() + ", event)");
				return "<b>" + "&nbsp;" + GetIcon(!isCollapsed) + "&nbsp;&nbsp;" + prjStatus + "</b>";
			}
			else
				return String.Format("<span class='text' style='padding-left:25px'><a href='DocumentView.aspx?DocumentId={0}'>{1}</a></span>", docId, title);
		}

		protected string GetTitleClient(bool isClient, bool isCollapsed,
			PrimaryKeyId contactUid, PrimaryKeyId orgUid,
			string clientName, int docId, string title)
		{
			if (isClient)
			{
				string client = "";
				string key = "";
				if (contactUid != PrimaryKeyId.Empty)
				{
					client = CommonHelper.GetContactLink(this.Page, contactUid, clientName);
					key = "contact_" + contactUid.ToString();
				}
				else if (orgUid != PrimaryKeyId.Empty)
				{
					client = CommonHelper.GetOrganizationLink(this.Page, orgUid, clientName);
					key = "org_" + orgUid.ToString();
				}
				else
				{
					client = "<span style='width:4px'>&nbsp;</span><font color='#003399'>" + LocRM.GetString("tNoClient") + "</font>";
					key = "noclient";
				}

				_hash.Add(key, "CollapseExpand2(" + (isCollapsed ? "1" : "0") + ",'" + contactUid.ToString() + "','" + orgUid.ToString() + "', event)");
				return "<b>" + "&nbsp;" + GetIcon(!isCollapsed) + "&nbsp;&nbsp;" + client + "</b>";
			}
			else
				return String.Format("<span class='text' style='padding-left:25px'><a href='DocumentView.aspx?DocumentId={0}'>{1}</a></span>", docId, title);
		}

		protected string GetOptionsString(bool isPrj, int docId, int canEdit, int canDelete)
		{
			string retval = "";
			if (!isPrj)
			{
				if (GetBool(canEdit))
				{
					retval = String.Format("<a href='../Documents/DocumentEdit.aspx?DocumentID={0}' title='{1}'><img border='0' src='../layouts/images/Edit.GIF' /></a>", docId.ToString(), LocRM2.GetString("Edit"));
				}
				if (GetBool(canDelete))
				{
					retval += "&nbsp;" + String.Format("<a href='javascript:DeleteDocument({0})' title='{1}'><img border='0' src='../layouts/images/delete.GIF' /></a>", docId.ToString(), LocRM2.GetString("Delete"));
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

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgDocuments.PageIndexChanged += new DataGridPageChangedEventHandler(dgDocuments_PageIndexChanged);
			this.dgDocuments.SortCommand += new DataGridSortCommandEventHandler(dgDocuments_SortCommand);
			this.dgDocuments.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(dgDocuments_PageSizeChanged);

			this.dgGroupDocs.PageIndexChanged += new DataGridPageChangedEventHandler(dgDocuments_PageIndexChanged);
			this.dgGroupDocs.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(dgDocuments_PageSizeChanged);

			this.dgGroupDocsByClient.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgDocuments_PageIndexChanged);
			this.dgGroupDocsByClient.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dgDocuments_PageSizeChanged);
			this.lblDeleteDocumentAll.Click += new EventHandler(lblDeleteDocumentAll_Click);
		}
		#endregion

		#region Collapse_Expand
		protected void Collapse_Expand_Click(object sender, System.EventArgs e)
		{
			string sType = hdnColType.Value;
			string ceType = hdnCollapseExpand.Value;
			if (sType.ToLower() == "prj")
			{
				int prjId = int.Parse(hdnDocumentId.Value);
				if (ceType == "0")
					Document.Collapse(prjId);
				else
					Document.Expand(prjId);
			}
			else if (sType.ToLower() == "contact")
			{
				PrimaryKeyId contactUid = PrimaryKeyId.Parse(hdnDocumentId.Value);
				if (ceType == "0")
					Document.CollapseByClient(contactUid, PrimaryKeyId.Empty);
				else
					Document.ExpandByClient(contactUid, PrimaryKeyId.Empty);
			}
			else if (sType.ToLower() == "org")
			{
				PrimaryKeyId orgUid = PrimaryKeyId.Parse(hdnDocumentId.Value);
				if (ceType == "0")
					Document.CollapseByClient(PrimaryKeyId.Empty, orgUid);
				else
					Document.ExpandByClient(PrimaryKeyId.Empty, orgUid);
			}
			else if (sType.ToLower() == "noclient")
			{
				if (ceType == "0")
					Document.CollapseByClient(PrimaryKeyId.Empty, PrimaryKeyId.Empty);
				else
					Document.ExpandByClient(PrimaryKeyId.Empty, PrimaryKeyId.Empty);
			}
			hdnColType.Value = "";
			hdnDocumentId.Value = "";
			BindDataGrid();
		}
		#endregion

		#region dgDocuments Events
		private void dgDocuments_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			_pc[_strPref + "DocumentList_Page"] = e.NewPageIndex.ToString();
			BindDataGrid();
		}

		private void dgDocuments_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if ((_pc[_strPref + "DocumentList_SortColumn"] != null) && (_pc[_strPref + "DocumentList_SortColumn"].ToString() == (string)e.SortExpression))
				_pc[_strPref + "DocumentList_SortColumn"] = (string)e.SortExpression + " DESC";
			else
				_pc[_strPref + "DocumentList_SortColumn"] = (string)e.SortExpression;
			BindDataGrid();
		}

		private void dgDocuments_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			_pc[_strPref + "DocumentList_PageSize"] = e.NewPageSize.ToString();
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
			_pc[_strPref + "ShowDocumentGroup"] = "False";
			BindDataGrid();
		}
		#endregion

		#region Show - Hide
		protected void lbShowGroup_Click(object sender, System.EventArgs e)
		{
			if (_strPref == "Doc")
				_pc[_strPref + "ShowDocumentGroup"] = "True";
			BindDataGrid();
		}

		protected void lbHideFilter_Click(object sender, System.EventArgs e)
		{
			_pc[_strPref + "ShowDocumentFilter"] = "False";
			BindDefaultValues();
			BindSavedValues();
			BindDataGrid();
		}

		protected void lbShowFilter_Click(object sender, System.EventArgs e)
		{
			_pc[_strPref + "ShowDocumentFilter"] = "True";
			BindDefaultValues();
			BindSavedValues();
			BindDataGrid();
		}
		#endregion

		#region Delete
		private void lblDeleteDocumentAll_Click(object sender, EventArgs e)
		{
			int documentId = int.Parse(hdnDocumentId.Value);
			Document.Delete(documentId);
			BindDataGrid();
		}
		#endregion

		#region Export
		private void ExportGrid()
		{
			dgExport.Columns[0].HeaderText = LocRM.GetString("Title");
			dgExport.Columns[1].HeaderText = LocRM.GetString("Priority");
			dgExport.Columns[2].HeaderText = LocRM.GetString("Status");
			dgExport.Columns[3].HeaderText = LocRM.GetString("CreatedDate");
			dgExport.Columns[4].HeaderText = LocRM.GetString("Manager");

			dgExport.Visible = true;
			BindDataGrid(dgExport);
			CommonHelper.ExportExcel(dgExport, "Documents.xls", null);
		}

		private void ExportXMLTable()
		{
			CommonHelper.SaveXML(BindDataGrid(dgExport), "Documents.xml");
		}
		#endregion

	}
}
