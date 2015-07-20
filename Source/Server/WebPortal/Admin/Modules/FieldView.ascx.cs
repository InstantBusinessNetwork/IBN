namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using MetaDataPlus.Configurator;
	using MetaDataPlus;
	using System.Reflection;

	/// <summary>
	///		Summary description for FieldView.
	/// </summary>
	public partial class FieldView : System.Web.UI.UserControl
	{

		protected ResourceManager LocRMDict = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strManageDictionaries", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRMList = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Lists.Resources.strLists", Assembly.GetExecutingAssembly());

		private MetaField _field;
		private MetaClass _mc;
		private int _fieldId = -1;
		private int _classId = -1;

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (Request["ID"] != null)
			{
				try
				{
					_fieldId = int.Parse(Request["ID"]);
				}
				catch { }
			}

			if (Request["ClassId"] != null)
			{
				try
				{
					_classId = int.Parse(Request["ClassId"]);
					_mc = MetaClass.Load(_classId);
				}
				catch { }
			}

			_field = MetaField.Load(_fieldId);

			//Checking
			if (_mc != null)
			{
				bool fl = false;
				foreach (int i in _field.OwnerMetaClassIdList)
				{
					if (i == _mc.Id)
					{
						fl = true;
						break;
					}
				}
				if (!fl)
					_mc = null;
			}

			ApplyLocalization();
			if (!Page.IsPostBack)
				BindValues();
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
			this.dgDic.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_cancel);
			this.dgDic.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_edit);
			this.dgDic.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_update);
			this.dgDic.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnAddNewItem.Text = LocRMDict.GetString("AddNewItem");

			secHeader.Title = LocRM.GetString("FieldDetails");
			secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("BackToCustomization"),
				ResolveUrl("~/Admin/Customization.aspx"));

			secItemsHeader.Title = LocRMDict.GetString("DictItems");
			secItemsHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/Sort-Ascending.png") + "'/> " + LocRMDict.GetString("tSortAsc"), Page.ClientScript.GetPostBackClientHyperlink(lbSortAsc, ""));
			secItemsHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/Sort-Descending.png") + "'/> " + LocRMDict.GetString("tSortDesc"), Page.ClientScript.GetPostBackClientHyperlink(lbSortDesc, ""));
			secItemsHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/newitem.gif") + "'/> " + LocRMDict.GetString("AddNewItem"), Page.ClientScript.GetPostBackClientHyperlink(btnAddNewItem, ""));
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			string FieldName = _field.Name;
			lblFieldName.Text = FieldName;
			if (FieldName.ToLower() == "lasteditorid")
				lblFieldFriendlyName.Text = LocRMList.GetString("UpdatedBy");
			else if (FieldName.ToLower() == "lasteditorid")
				lblFieldFriendlyName.Text = LocRMList.GetString("LastUpdated");
			else if (FieldName.ToLower() == "creatorid")
				lblFieldFriendlyName.Text = LocRMList.GetString("CreatedBy");
			else if (FieldName.ToLower() == "lastsaveddate")
				lblFieldFriendlyName.Text = LocRMList.GetString("Created");
			else
				lblFieldFriendlyName.Text = _field.FriendlyName;
			lblFieldDescription.Text = _field.Description;
			MetaType mdType = MetaType.Load(_field.DataType);
			lblFieldType.Text = mdType.FriendlyName;

			trAllowNulls.Visible = true;
			if (_field.AllowNulls)
			{
				if (_mc == null)
					trAllowNulls.Visible = false;
				else if (!_mc.GetFieldIsRequired(_field))
					lblAllowNulls.Text = LocRM.GetString("Yes");
				else
					lblAllowNulls.Text = LocRM.GetString("No");
			}
			else
				lblAllowNulls.Text = LocRM.GetString("No");

			if (_field.SaveHistory)
				lblSaveHistory.Text = LocRM.GetString("Yes");
			else
				lblSaveHistory.Text = LocRM.GetString("No");

			if (_field.AllowSearch)
				lblAllowSearch.Text = LocRM.GetString("Yes");
			else
				lblAllowSearch.Text = LocRM.GetString("No");

			tblDictItems.Visible = false;
			if (_field.DataType == MetaDataType.DictionarySingleValue
				|| _field.DataType == MetaDataType.DictionaryMultivalue
				|| _field.DataType == MetaDataType.EnumSingleValue
				|| _field.DataType == MetaDataType.EnumMultivalue)
			{
				tblDictItems.Visible = true;
				BindDG();
			}
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			dgDic.Columns[1].HeaderText = "#";
			dgDic.Columns[2].HeaderText = LocRMDict.GetString("Name");

			dgDic.DataSource = _field.Dictionary;
			dgDic.DataBind();

			foreach (DataGridItem dgi in dgDic.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRMDict.GetString("Warning") + "')");

				RequiredFieldValidator rf = (RequiredFieldValidator)dgi.FindControl("rfName");
				if (rf != null)
					rf.ErrorMessage = LocRMDict.GetString("Required");

				DropDownList ddl = (DropDownList)dgi.FindControl("ddIndex");
				if (ddl != null)
				{
					for (int i = 1; i <= _field.Dictionary.Count; i++)
						ddl.Items.Add(new ListItem(i.ToString(), i.ToString()));
					ddl.SelectedValue = (dgDic.EditItemIndex + 1).ToString();
				}
			}
		}
		#endregion

		#region btnAddNewItem_Click
		protected void btnAddNewItem_Click(object sender, System.EventArgs e)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Index", typeof(int)));
			dt.Columns.Add(new DataColumn("Value", typeof(string)));

			DataRow row;
			foreach (MetaDictionaryItem item in _field.Dictionary)
			{
				row = dt.NewRow();
				row["Id"] = item.Id;
				row["Value"] = item.Value;
				row["Index"] = item.Index;
				dt.Rows.Add(row);
			}

			row = dt.NewRow();
			row["Id"] = -1;
			row["Value"] = "";
			row["Index"] = -1;
			dt.Rows.Add(row);

			dgDic.EditItemIndex = dt.Rows.Count - 1;
			dgDic.DataSource = dt.DefaultView;
			dgDic.DataBind();

			foreach (DataGridItem dgi in dgDic.Items)
			{
				DropDownList ddl = (DropDownList)dgi.FindControl("ddIndex");
				if (ddl != null)
				{
					for (int i = 1; i <= dt.Rows.Count; i++)
						ddl.Items.Add(new ListItem(i.ToString(), i.ToString()));
					ddl.SelectedValue = dt.Rows.Count.ToString();
				}
			}
		}
		#endregion

		#region SortingDictionary
		protected void lbSortAsc_Click(object sender, System.EventArgs e)
		{
			_field.Dictionary.SortByValue(true);
			BindDG();
		}

		protected void lbSortDesc_Click(object sender, System.EventArgs e)
		{
			_field.Dictionary.SortByValue(false);
			BindDG();
		}
		#endregion

		#region DataGrid Event Handlers
		private void dg_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int itemId = int.Parse(e.Item.Cells[0].Text);
			_field.Dictionary.Delete(itemId);

			dgDic.EditItemIndex = -1;
			BindDG();
		}

		private void dg_edit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgDic.EditItemIndex = e.Item.ItemIndex;
			BindDG();
		}

		private void dg_cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgDic.EditItemIndex = -1;
			BindDG();
		}

		private void dg_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int itemId = int.Parse(e.Item.Cells[0].Text);
			TextBox tb = (TextBox)e.Item.FindControl("tbName");
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddIndex");
			if (tb != null && ddl != null)
			{
				if (itemId > 0)
					_field.Dictionary.Update(itemId, int.Parse(ddl.SelectedValue) - 1, tb.Text);
				else
					_field.Dictionary.Insert(int.Parse(ddl.SelectedValue) - 1, tb.Text);
			}

			dgDic.EditItemIndex = -1;
			BindDG();
		}
		#endregion
	}
}
