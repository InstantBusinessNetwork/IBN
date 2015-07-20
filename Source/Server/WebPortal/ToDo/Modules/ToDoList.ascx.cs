namespace Mediachase.UI.Web.ToDo.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Core.Business;
	using Mediachase.Ibn.Clients;

	/// <summary>
	///		Summary description for ToDoList.
	/// </summary>
	public partial class ToDoList : System.Web.UI.UserControl
	{
		private string _strPref = "ToDoGroup";
		private Hashtable _hash = new Hashtable();
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strToDoGeneral", Assembly.GetExecutingAssembly());
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", Assembly.GetExecutingAssembly());
		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if (!Page.IsPostBack)
				BindDefaultValues();
			BindSavedValues();
			BindInfoTable();
			BindDataGrid();

			BindToolBar();
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			FilterTable.Visible = (_pc[_strPref + "ShowFilter"] != null && bool.Parse(_pc[_strPref + "ShowFilter"]));
			tblFilterInfo.Visible = !FilterTable.Visible;

			foreach (DataGridItem dgi in dgGroupToDoByClient.Items)
			{
				if (int.Parse(dgi.Cells[0].Text) <= 0)
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

			dgGroupToDoByClient.Columns[3].HeaderText = "<span class='text' style='padding-left:30px'>" + LocRM.GetString("Title") + "</span>";
			dgGroupToDoByClient.Columns[4].HeaderText = LocRM.GetString("Priority");
			dgGroupToDoByClient.Columns[5].HeaderText = LocRM.GetString("Created");
			dgGroupToDoByClient.Columns[6].HeaderText = LocRM.GetString("Manager");

			lbShowFilter.Text = "<img align='absmiddle' border='0' title='" + LocRM.GetString("ShowFilter") + "' src='../Layouts/Images/scrolldown_hover.GIF' />";
			lbHideFilter.Text = "<img align='absmiddle' border='0' title='" + LocRM.GetString("HideFilter") + "' src='../Layouts/Images/scrollup_hover.GIF' />";

			lblFilterNotSet.Text = LocRM.GetString("FilterNotSet");
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			//Managers
			ddManager.DataSource = ToDo.GetManagers();
			ddManager.DataTextField = "DisplayName";
			ddManager.DataValueField = "PrincipalId";
			ddManager.DataBind();

			ListItem lItem = new ListItem(LocRM.GetString("All"), "0");
			ddManager.Items.Insert(0, lItem);

			//Priorities
			ddPriority.DataSource = ToDo.GetListPriorities();
			ddPriority.DataTextField = "PriorityName";
			ddPriority.DataValueField = "PriorityId";
			ddPriority.DataBind();
			lItem = new ListItem(LocRM.GetString("All"), "-1");
			ddPriority.Items.Insert(0, lItem);

			//Keyword
			tbKeyword.Text = String.Empty;

			//Projects
			ddlProject.DataSource = Project.GetListProjects();
			ddlProject.DataTextField = "Title";
			ddlProject.DataValueField = "ProjectId";
			ddlProject.DataBind();
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

			//Client
			ClientControl.ObjectType = String.Empty;
			ClientControl.ObjectId = PrimaryKeyId.Empty;
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			//Manager
			if (_pc[_strPref + "Manager"] != null)
				Util.CommonHelper.SafeSelect(ddManager, _pc[_strPref + "Manager"]);

			//Priority
			if (_pc[_strPref + "Priority"] != null)
				Util.CommonHelper.SafeSelect(ddPriority, _pc[_strPref + "Priority"]);

			//Keyword
			if (_pc[_strPref + "Keyword"] != null)
				tbKeyword.Text = _pc[_strPref + "Keyword"];

			//Project
			if (_pc[_strPref + "Project"] != null)
				Util.CommonHelper.SafeSelect(ddlProject, _pc[_strPref + "Project"]);

			// General Categories
			if (_pc[_strPref + "GenCatType"] != null)
				Util.CommonHelper.SafeSelect(ddGenCatType, _pc[_strPref + "GenCatType"]);

			if (ddGenCatType.SelectedItem.Value == "0")
				lbGenCats.Style["Display"] = "none";
			else
				lbGenCats.Style["Display"] = "block";

			using (IDataReader reader = ToDo.GetListCategoriesByUser())
			{
				while (reader.Read())
					Util.CommonHelper.SafeMultipleSelect(lbGenCats, reader["CategoryId"].ToString());
			}

			//Client
			if (_pc[_strPref + "ClientNew"] != null && (!Page.IsPostBack || ClientControl.ObjectId == PrimaryKeyId.Empty))
			{
				string ss = _pc[_strPref + "ClientNew"];
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
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			_pc[_strPref + "Manager"] = ddManager.SelectedValue;
			_pc[_strPref + "Priority"] = ddPriority.SelectedValue;
			_pc[_strPref + "Keyword"] = tbKeyword.Text;
			_pc[_strPref + "Project"] = ddlProject.SelectedValue;

			// General Categories
			_pc[_strPref + "GenCatType"] = ddGenCatType.SelectedValue;
			ArrayList alGenCats = new ArrayList();
			foreach (ListItem liItem in lbGenCats.Items)
			{
				if (liItem.Selected && !alGenCats.Contains(int.Parse(liItem.Value)))
					alGenCats.Add(int.Parse(liItem.Value));
			}
			ToDo.SaveGeneralCategories(alGenCats);

			//Client
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc[_strPref + "ClientNew"] = "org_" + ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc[_strPref + "ClientNew"] = "contact_" + ClientControl.ObjectId;
			else
				_pc[_strPref + "ClientNew"] = "_";
		}
		#endregion

		#region BindInfoTable
		private void BindInfoTable()
		{
			int rowCount = 0;
			tblFilterInfoSet.Rows.Clear();

			// Manager
			if (_pc[_strPref + "Manager"] != null)
			{
				ListItem li = ddManager.Items.FindByValue(_pc[_strPref + "Manager"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Manager")), li.Text);
					rowCount++;
				}
			}

			// Project
			if (_pc[_strPref + "Project"] != null)
			{
				ListItem li = ddlProject.Items.FindByValue(_pc[_strPref + "Project"]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Project")), li.Text);
					rowCount++;
				}
			}

			// Priority
			if (_pc[_strPref + "Priority"] != null)
			{
				ListItem li = ddPriority.Items.FindByValue(_pc[_strPref + "Priority"]);
				if (li != null && li.Value != "-1")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Priority")), li.Text);
					rowCount++;
				}
			}

			// Keyword
			if (_pc[_strPref + "Keyword"] != null && _pc[_strPref + "Keyword"] != "")
			{
				AddRow(
					String.Format("{0}:&nbsp; ", LocRM.GetString("Keyword")),
					String.Format("'{0}'", _pc[_strPref + "Keyword"]));
				rowCount++;
			}

			//Client
			if (_pc[_strPref + "ClientNew"] != null)
			{
				string ss = _pc[_strPref + "ClientNew"];
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

			// General Categories
			if (_pc[_strPref + "GenCatType"] != null)
			{
				ListItem li = ddGenCatType.Items.FindByValue(_pc[_strPref + "GenCatType"]);
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
			td1.Width = "120px";
			td1.InnerHtml = filterName;
			HtmlTableCell td2 = new HtmlTableCell();
			td2.InnerHtml = filterValue;
			tr.Cells.Add(td1);
			tr.Cells.Add(td2);
			tblFilterInfoSet.Rows.Add(tr);
		}
		#endregion

		#region BindDataGrid (2 overload)
		private void BindDataGrid()
		{
			BindDataGrid(dgGroupToDoByClient);
		}

		private DataTable BindDataGrid(DataGrid dg)
		{
			_hash.Clear();

			//Project
			int prjId = int.Parse(ddlProject.SelectedValue);

			//Manager
			int manId = int.Parse(ddManager.SelectedValue);

			//Resource
			int resId = 0;

			//Keyword
			string keyword = tbKeyword.Text;

			// Priority
			int priority_id = int.Parse(ddPriority.SelectedValue);

			// Client
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName())
				orgUid = ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName())
				contactUid = ClientControl.ObjectId;

			// General Category Type
			int genCategory_type = int.Parse(ddGenCatType.SelectedValue);

			DataTable dt = ToDo.GetListTodoByFilterGroupedByClient(prjId, manId,
				resId, priority_id, keyword, genCategory_type, orgUid, contactUid);

			DataView dv = dt.DefaultView;

			dg.DataSource = dv;

			if (_pc[_strPref + "PageSize"] != null)
				dg.PageSize = int.Parse(_pc[_strPref + "PageSize"]);

			if (_pc[_strPref + "Page"] != null)
			{
				int pageIndex = int.Parse(_pc[_strPref + "Page"]);
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

		protected string GetIcon(bool isExpand)
		{
			if (isExpand)
				return "<img border=0 src='" + ResolveUrl("~/Layouts/images/minus.gif") + "'>";
			else
				return "<img border=0 src='" + ResolveUrl("~/Layouts/images/plus.gif") + "'>";
		}

		protected string GetTitleClient(bool isClient, bool isCollapsed, PrimaryKeyId contactUid, PrimaryKeyId orgUid,
			string clientName, int toDoId, string title)
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
				_hash.Add(key, "CollapseExpand(" + (isCollapsed ? "1" : "0") + ",'" + contactUid.ToString() + "','" + orgUid.ToString() + "', event)");
				return "<b>" + "&nbsp;" + GetIcon(!isCollapsed) + "&nbsp;&nbsp;" + client + "</b>";
			}
			else
				return String.Format("<span class='text' style='padding-left:25px'><a href='ToDoView.aspx?ToDoId={0}'>{1}</a></span>", toDoId, title);
		}

		protected string GetOptionsString(bool isClient, int toDoId, int canEdit, int canDelete)
		{
			string retval = "";
			if (!isClient)
			{
				if (GetBool(canEdit))
				{
					retval = String.Format("<a href='../ToDo/ToDoEdit.aspx?ToDoID={0}' title='{1}'><img border='0' src='../layouts/images/Edit.GIF' /></a>", toDoId.ToString(), LocRM.GetString("Edit"));
				}
				if (GetBool(canDelete))
				{
					retval += "&nbsp;" + String.Format("<a href='javascript:DeleteToDo({0})' title='{1}'><img border='0' src='../layouts/images/delete.GIF' /></a>", toDoId.ToString(), LocRM.GetString("Delete"));
				}
			}
			return retval;
		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			secHeader.Title = LocRM.GetString("tToDos");
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
			this.lbDeleteToDoAll.Click += new EventHandler(lbDeleteToDoAll_Click);

			this.dgGroupToDoByClient.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageIndexChanged);
			this.dgGroupToDoByClient.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChanged);
		}
		#endregion

		#region dg Events
		private void dg_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			_pc[_strPref + "Page"] = e.NewPageIndex.ToString();
			BindDataGrid();
		}

		private void dg_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			_pc[_strPref + "PageSize"] = e.NewPageSize.ToString();
			BindDataGrid();
		}
		#endregion

		#region Collapse_Expand
		protected void Collapse_Expand_Click(object sender, System.EventArgs e)
		{
			string sType = hdnColType.Value;
			string ceType = hdnCollapseExpand.Value;
			if (sType.ToLower() == "contact")
			{
				PrimaryKeyId contactUid = PrimaryKeyId.Parse(hdnToDoId.Value);
				if (ceType == "0")
					ToDo.CollapseByClient(contactUid, PrimaryKeyId.Empty);
				else
					ToDo.ExpandByClient(contactUid, PrimaryKeyId.Empty);
			}
			else if (sType.ToLower() == "org")
			{
				PrimaryKeyId orgUid = int.Parse(hdnToDoId.Value);
				if (ceType == "0")
					ToDo.CollapseByClient(PrimaryKeyId.Empty, orgUid);
				else
					ToDo.ExpandByClient(PrimaryKeyId.Empty, orgUid);
			}
			else if (sType.ToLower() == "noclient")
			{
				if (ceType == "0")
					ToDo.CollapseByClient(PrimaryKeyId.Empty, PrimaryKeyId.Empty);
				else
					ToDo.ExpandByClient(PrimaryKeyId.Empty, PrimaryKeyId.Empty);
			}
			hdnColType.Value = "";
			hdnToDoId.Value = "";
			BindDataGrid();
		}
		#endregion

		#region Show - Hide
		protected void lbHideFilter_Click(object sender, System.EventArgs e)
		{
			_pc[_strPref + "ShowFilter"] = "False";
			BindDefaultValues();
			BindSavedValues();
			BindDataGrid();
		}

		protected void lbShowFilter_Click(object sender, System.EventArgs e)
		{
			_pc[_strPref + "ShowFilter"] = "True";
			BindDefaultValues();
			BindSavedValues();
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
		#endregion

		#region Delete
		private void lbDeleteToDoAll_Click(object sender, EventArgs e)
		{
			int toDoId = int.Parse(hdnToDoId.Value);
			ToDo.Delete(toDoId);
			BindDataGrid();
		}
		#endregion
	}
}
