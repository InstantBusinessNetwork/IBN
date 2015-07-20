namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	
	using Mediachase.IBN.Business;
	using MetaDataPlus.Configurator;

	/// <summary>
	///		Summary description for Customization.
	/// </summary>
	public partial class Customization : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strCalendarList", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Lists.Resources.strLists", typeof(Customization).Assembly);

		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if (!IsPostBack)
			{
				BindData();
			}
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			fieldsHeader.Title = LocRM.GetString("AvailableFields");

			lblSelectedType.Text = LocRM.GetString("SelectType") + ":";

			dgSelectedFields.Columns[1].HeaderText = LocRM.GetString("Field");
			dgSelectedFields.Columns[2].HeaderText = LocRM.GetString("DataType");
			dgSelectedFields.Columns[3].HeaderText = LocRM.GetString("tIsRequired");

			dgAvailableFields.Columns[2].HeaderText = LocRM.GetString("Field");
			dgAvailableFields.Columns[3].HeaderText = LocRM.GetString("DataType");
		}
		#endregion

		#region BindToolBars
		private void BindToolBars()
		{
			// Left Toolbar Header
			int classId = -1;
			if (ddlSelectedType.Visible && ddlSelectedType.Items.Count > 0)
				classId = int.Parse(ddlSelectedType.SelectedItem.Value);
			else if (ddlSelectedElement.Items.Count > 0)
				classId = int.Parse(ddlSelectedElement.SelectedItem.Value);
			elementHeader.Title = LocRM.GetString("tCurrentlyUsed");

			// Left Toolbar Button
			string text = String.Format("<img alt='' src='{1}' border='0' width='16' height='16' align='absmiddle' title='{0}'> {0}", LocRM.GetString("NewField"), this.Page.ResolveUrl("~/Layouts/Images/NewItem.gif"));
			string link = this.Page.ResolveUrl("~/Admin/FieldEdit.aspx?ClassId=") + classId;
			elementHeader.AddLink(text, link);

			// Right Toolbar Button
			text = String.Format("<img alt='' src='{1}' border='0' width='16' height='16' align='absmiddle' title='{0}'> {0}", LocRM.GetString("NewField"), this.Page.ResolveUrl("~/Layouts/Images/NewItem.gif"));
			link = this.Page.ResolveUrl("~/Admin/FieldEdit.aspx");
			fieldsHeader.AddLink(text, link);
			fieldsHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM2.GetString("BusData"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin2"));
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
			this.dgSelectedFields.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSelectedFields_DeleteCommand);
			this.dgSelectedFields.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSelectedFields_EditCommand);
			this.dgSelectedFields.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSelectedFields_update);
			this.dgSelectedFields.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSelectedFields_cancel);
			this.dgAvailableFields.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAvailableFields_ItemCommand);
			this.dgAvailableFields.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAvailableFields_DeleteCommand);
			this.dgAvailableFields.SortCommand += new DataGridSortCommandEventHandler(dgAvailableFields_SortCommand);
			this.ddlType.SelectedIndexChanged += new EventHandler(ddType_SelectedIndexChanged);
		}
		#endregion

		#region BindData
		private void BindData()
		{
			using (IDataReader reader = MetaClass.GetDataReader())
			{
				while (reader.Read())
				{
					int metaClassId = (int)reader["MetaClassId"];
					bool isSystem = (bool)reader["IsSystem"];
					string tableName = reader["TableName"].ToString().ToLower();
					if ((tableName == "projects" || tableName == "taskex" || tableName == "portfolioex") &&
						!Configuration.ProjectManagementEnabled)
						continue;
					if (tableName == "incidentsex" && !Configuration.HelpDeskEnabled)
						continue;
					string friendlyName = reader["FriendlyName"].ToString();
					string parentTableName = "";
					string parentFriendlyName = "";
					if (reader["ParentTableName"] != DBNull.Value)
						parentTableName = reader["ParentTableName"].ToString().ToLower();
					if (reader["ParentFriendlyName"] != DBNull.Value)
						parentFriendlyName = reader["ParentFriendlyName"].ToString();
					bool isAbstract = false;
					if (reader["IsAbstract"] != DBNull.Value)
						isAbstract = (bool)reader["IsAbstract"];
					string namespaceName = reader["namespace"].ToString();
					if (!isSystem && parentTableName != "projects" && parentTableName != "list_items" && !isAbstract && namespaceName == MetaDataWrapper.UserRoot)
					{
						ddlSelectedElement.Items.Add(new ListItem(parentFriendlyName, metaClassId.ToString()));
					}
					else if (tableName == "projects")
					{
						ddlSelectedElement.Items.Add(new ListItem(friendlyName, metaClassId.ToString()));
					}
				}
			}

			// Current Element
			if (_pc["cust_SelectedElement"] != null)
			{
				int curClassId = int.Parse(_pc["cust_SelectedElement"]);
				MetaClass mc = MetaClass.Load(curClassId);
				if (mc != null)
				{
					if (mc.Parent != null && mc.Parent.TableName.ToLower() == "projects")
						Util.CommonHelper.SafeSelect(ddlSelectedElement, mc.Parent.Id.ToString());
					else
						Util.CommonHelper.SafeSelect(ddlSelectedElement, curClassId.ToString());
				}
			}

			BindSelectedType();
			BindAvailableType();
		}
		#endregion

		#region BindSelectedType
		private void BindSelectedType()
		{
			lblSelectedType.Visible = false;
			ddlSelectedType.Visible = false;

			if (ddlSelectedElement.Items.Count > 0)
			{
				int selectedClassId = int.Parse(ddlSelectedElement.SelectedItem.Value);
				MetaClass mc = MetaClass.Load(selectedClassId);
				if (mc.ChildClasses != null && mc.ChildClasses.Count > 0)
				{
					lblSelectedType.Visible = true;
					ddlSelectedType.Visible = true;

					ddlSelectedType.Items.Clear();

					MetaClassCollection children = mc.ChildClasses;
					foreach (MetaClass child in children)
						ddlSelectedType.Items.Add(new ListItem(child.FriendlyName, child.Id.ToString()));

					if (!IsPostBack && _pc["cust_SelectedElement"] != null)
						Util.CommonHelper.SafeSelect(ddlSelectedType, _pc["cust_SelectedElement"]);
				}
			}

			BindSelectedFields();
		}
		#endregion

		#region BindAvailableType
		private void BindAvailableType()
		{
			if (_pc["cust_AvailableElement"] == null)
				_pc["cust_AvailableElement"] = "-1";

			Util.CommonHelper.BindMetaTypesItemCollections(ddlType.Items, true);
			Util.CommonHelper.SafeSelect(ddlType, _pc["cust_AvailableElement"]);

			BindAvailableFields();
		}
		#endregion

		#region BindSelectedFields
		private void BindSelectedFields()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("FieldId", typeof(int));
			dt.Columns.Add("ClassId", typeof(int));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("DataType", typeof(string));
			dt.Columns.Add("IsRequired", typeof(bool));
			dt.Columns.Add("CanChangeIsRequired", typeof(bool));

			MetaClass selectedMetaClass = GetSelectedMetaClass();
			if (selectedMetaClass != null)
			{
				MetaFieldCollection mfc = selectedMetaClass.MetaFields;
				foreach (MetaField field in mfc)
				{
					if (field.IsUser)
					{
						DataRow row = dt.NewRow();
						row["FieldId"] = field.Id;
						row["ClassId"] = selectedMetaClass.Id;
						row["FriendlyName"] = field.FriendlyName;
						MetaType mdType = MetaType.Load(field.DataType);
						row["DataType"] = mdType.FriendlyName;

						row["CanChangeIsRequired"] = true;
						if (!field.AllowNulls)
						{
							row["IsRequired"] = true;
							row["CanChangeIsRequired"] = false;
						}
						else
							row["IsRequired"] = selectedMetaClass.GetFieldIsRequired(field);

						dt.Rows.Add(row);
					}
				}
				_pc["cust_SelectedElement"] = selectedMetaClass.Id.ToString();
			}

			dgSelectedFields.DataSource = new DataView(dt);
			dgSelectedFields.DataBind();

			foreach (DataGridItem dgi in dgSelectedFields.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("DeleteWarning") + "')");
			}
		}
		#endregion

		#region BindAvailableFields
		private void BindAvailableFields()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("FieldId", typeof(int));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("DataType", typeof(string));
			dt.Columns.Add("DataTypeId", typeof(int));
			dt.Columns.Add("sortDataType", typeof(int));
			dt.Columns.Add("CanDelete", typeof(bool));

			// Fields
			MetaFieldCollection mfc = MetaField.GetList(MetaDataPlus.MetaNamespace.UserRoot, true);

			foreach (MetaField field in mfc)
			{
				if (field.IsUser)
				{
					DataRow row = dt.NewRow();
					row["FieldId"] = field.Id;
					row["FriendlyName"] = field.FriendlyName;
					MetaType mdType = MetaType.Load(field.DataType);
					row["DataType"] = mdType.FriendlyName;
					int iType = mdType.Id;
					if (mdType.MetaDataType == MetaDataType.DictionaryMultivalue ||
						mdType.MetaDataType == MetaDataType.DictionarySingleValue ||
						mdType.MetaDataType == MetaDataType.EnumMultivalue ||
						mdType.MetaDataType == MetaDataType.EnumSingleValue ||
						mdType.MetaDataType == MetaDataType.StringDictionary)
						iType = 0;
					row["DataTypeId"] = iType;
					row["sortDataType"] = GetSortDataType(mdType);
					row["CanDelete"] = (field.OwnerMetaClassIdList.Count == 0);
					dt.Rows.Add(row);
				}
			}

			DataView dv = dt.DefaultView;
			if (_pc["Cust_AvailableFields_Sort"] == null)
				_pc["Cust_AvailableFields_Sort"] = "sortDataType";
			dv.Sort = _pc["Cust_AvailableFields_Sort"];
			if (int.Parse(_pc["cust_AvailableElement"]) >= 0)
				dv.RowFilter = "DataTypeId = " + _pc["cust_AvailableElement"];
			dgAvailableFields.DataSource = dv;
			dgAvailableFields.DataBind();

			foreach (DataGridItem dgi in dgAvailableFields.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("DeleteWarning") + "')");
			}
		}
		#endregion

		#region GetSortDataType
		private int GetSortDataType(MetaType mdType)
		{
			switch (mdType.Id)
			{
				case 31://ShortString
					return 0;
				case 32://LongString
					return 1;
				case 26://Integer
					return 2;
				case 9://Money
					return 3;
				case 28://Date
					return 4;
				case 4://DateTime
					return 5;
				case 34://DictionarySingleValue
					return 6;
				case 35://DictionaryMultivalue
					return 7;
				case 36://EnumSingleValue
					return 8;
				case 37://EnumMultivalue
					return 9;
				case 38://StringDictionary
					return 10;
				case 27://Boolean
					return 11;
				case 30://URL
					return 12;
				case 33://LongHtmlString
					return 13;
				case 29://Email
					return 14;
				case 39://File
					return 15;
				case 40://ImageFile
					return 16;
				default:
					return 100;
			}
		}
		#endregion

		#region SelectedIndexChanged
		protected void ddlSelectedElement_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			dgSelectedFields.EditItemIndex = -1;
			BindSelectedType();
		}

		protected void ddlSelectedType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			dgSelectedFields.EditItemIndex = -1;
			BindSelectedFields();
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolBars();

			// Disabling checkboxes for existing fields
			MetaClass selectedMetaClass = GetSelectedMetaClass();
			if (selectedMetaClass != null)
			{
				foreach (DataGridItem item in dgAvailableFields.Items)
				{
					bool checkBoxEnabled = true;
					int fieldId = int.Parse(item.Cells[0].Text);

					foreach (MetaField field in selectedMetaClass.UserMetaFields)
					{
						if (field.Id == fieldId)
						{
							checkBoxEnabled = false;
							break;
						}
					}

					foreach (Control control in item.Cells[1].Controls)
					{
						if (control is CheckBox)
						{
							CheckBox checkBox = (CheckBox)control;
							checkBox.Enabled = checkBoxEnabled;
						}
					}
					ImageButton ib = (ImageButton)item.FindControl("ibCopy");
					if (ib != null)
						ib.Visible = checkBoxEnabled;
				}
			}
		}
		#endregion

		#region dgAvailableFields_ItemCommand
		private void dgAvailableFields_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			MetaClass selectedMetaClass = GetSelectedMetaClass();
			int fieldsCount = selectedMetaClass.UserMetaFields.Count;
			if (e.CommandName == "Copy" && selectedMetaClass != null)
			{
				foreach (DataGridItem item in dgAvailableFields.Items)
				{
					foreach (Control control in item.Cells[1].Controls)
					{
						if (control is CheckBox)
						{
							CheckBox checkBox = (CheckBox)control;
							if (checkBox.Enabled && checkBox.Checked)
							{
								int fieldId = int.Parse(item.Cells[0].Text);
								fieldsCount++;
								selectedMetaClass.AddField(fieldId, fieldsCount);
								checkBox.Checked = false;
							}
						}
					}
				}
				Response.Redirect("~/Admin/Customization.aspx", true);
			}
			if (e.CommandName == "CopyOne" && selectedMetaClass != null)
			{
				foreach (Control control in e.Item.Cells[1].Controls)
				{
					if (control is CheckBox)
					{
						CheckBox checkBox = (CheckBox)control;
						int fieldId = int.Parse(e.Item.Cells[0].Text);
						fieldsCount++;
						selectedMetaClass.AddField(fieldId, fieldsCount);
						checkBox.Checked = false;
					}
				}
				Response.Redirect("~/Admin/Customization.aspx", true);
			}
		}
		#endregion

		#region GetSelectedMetaClass
		private MetaClass GetSelectedMetaClass()
		{
			int selectedClassId = -1;
			if (ddlSelectedType.Visible && ddlSelectedType.Items.Count > 0)
				selectedClassId = int.Parse(ddlSelectedType.SelectedItem.Value);
			else if (ddlSelectedElement.Items.Count > 0)
				selectedClassId = int.Parse(ddlSelectedElement.SelectedItem.Value);
			MetaClass selectedMetaClass = null;
			if (selectedClassId > 0)
				selectedMetaClass = MetaClass.Load(selectedClassId);
			return selectedMetaClass;
		}
		#endregion

		#region DeleteCommand
		private void dgSelectedFields_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int fieldId = int.Parse(e.Item.Cells[0].Text);
			MetaClass mc = GetSelectedMetaClass();
			int currentPos = mc.GetFieldWeight(fieldId);

			foreach (MetaField mf in mc.UserMetaFields)
			{
				if (mf.Weight > currentPos)
					mf.Weight = mf.Weight - 1;
			}
			mc.DeleteField(fieldId);

			Response.Redirect("~/Admin/Customization.aspx", true);
		}

		private void dgAvailableFields_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int FieldId = int.Parse(e.Item.Cells[0].Text);
			MetaField field = MetaField.Load(FieldId);
			if (field.OwnerMetaClassIdList.Count == 0)
				MetaField.Delete(FieldId);

			Response.Redirect("~/Admin/Customization.aspx", true);
		}
		#endregion

		#region dgSelectedFields_EditCommand
		private void dgSelectedFields_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgSelectedFields.EditItemIndex = e.Item.ItemIndex;
			BindSelectedFields();

			MetaClass mc = GetSelectedMetaClass();
			foreach (DataGridItem dgi in dgSelectedFields.Items)
			{
				//IsRequired
				DropDownList ddlIsRequired = (DropDownList)dgi.FindControl("ddlIsRequired");
				if (ddlIsRequired != null)
				{
					ddlIsRequired.Items.Add(new ListItem(LocRM3.GetString("Yes"), "True"));
					ddlIsRequired.Items.Add(new ListItem(LocRM3.GetString("No"), "False"));

					int fieldId = int.Parse(e.Item.Cells[0].Text);
					Util.CommonHelper.SafeSelect(ddlIsRequired, mc.GetFieldIsRequired(fieldId).ToString());
				}
			}
		}
		#endregion

		#region dgSelectedFields_cancel
		private void dgSelectedFields_cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgSelectedFields.EditItemIndex = -1;
			BindSelectedFields();
		}
		#endregion

		#region dgSelectedFields_update
		private void dgSelectedFields_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			MetaClass mc = GetSelectedMetaClass();
			int fieldId = int.Parse(e.CommandArgument.ToString());

			//IsRequired
			DropDownList ddlIsRequired = (DropDownList)e.Item.FindControl("ddlIsRequired");
			if (ddlIsRequired != null)
				mc.SetFieldIsRequired(fieldId, bool.Parse(ddlIsRequired.SelectedValue));

			dgSelectedFields.EditItemIndex = -1;
			BindSelectedFields();
		}
		#endregion

		#region GetBoolValue
		protected string GetBoolValue(bool bValue)
		{
			return bValue ? LocRM3.GetString("Yes") : LocRM3.GetString("No");
		}
		#endregion

		#region dgAvailableFields_SortCommand
		private void dgAvailableFields_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if ((_pc["Cust_AvailableFields_Sort"] != null) && (_pc["Cust_AvailableFields_Sort"] == e.SortExpression))
				_pc["Cust_AvailableFields_Sort"] += " DESC";
			else
				_pc["Cust_AvailableFields_Sort"] = e.SortExpression;
			BindAvailableFields();
		}
		#endregion

		#region ddType_SelectedIndexChanged
		private void ddType_SelectedIndexChanged(object sender, EventArgs e)
		{
			_pc["cust_AvailableElement"] = ddlType.SelectedValue;
			BindAvailableFields();
		}
		#endregion
	}
}
