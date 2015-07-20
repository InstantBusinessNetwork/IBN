namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using MetaDataPlus.Configurator;	

	/// <summary>
	///		Summary description for MasterDataManager.
	/// </summary>
	public partial class MasterDataManager : System.Web.UI.UserControl
	{
		#region HTML Vars
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strCalendarList", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			BindToolBar();
			if (!Page.IsPostBack)
			{
				BindShowTypes();
				BindDataTypes();
				BindAvailableFields();
			}
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			dgAvailableFields.Columns[1].HeaderText = LocRM.GetString("Field");
			dgAvailableFields.Columns[2].HeaderText = LocRM.GetString("FieldDescription");
			dgAvailableFields.Columns[3].HeaderText = LocRM.GetString("DataType");
			btnApply.Value = LocRM.GetString("tApply");
			btnReset.Value = LocRM.GetString("tReset");
		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			secHeader.Title = LocRM.GetString("tGloballyFields");
			string text = String.Format("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/NewItem.gif") + "' border='0' width='16' height='16' align='absmiddle' title='{0}'> {0}", LocRM.GetString("CreateField"));
			secHeader.AddLink(text, ResolveUrl("~/Admin/FieldEdit.aspx?Back=All"));
			secHeader.AddLink("<img alt='' src='" + ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM2.GetString("BusData"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin2"));
		}
		#endregion

		#region BindShowTypes
		private void BindShowTypes()
		{
			ddlShow.Items.Add(new ListItem(LocRM.GetString("tAll"), "0"));
			ddlShow.Items.Add(new ListItem(LocRM.GetString("tInUse"), "1"));
			ddlShow.Items.Add(new ListItem(LocRM.GetString("tNotInUse"), "-1"));
			if (pc["MDataMan_Show"] != null)
				CommonHelper.SafeSelect(ddlShow, pc["MDataMan_Show"].ToString());
		}
		#endregion

		#region BindDataTypes
		private void BindDataTypes()
		{
			Mediachase.UI.Web.Util.CommonHelper.BindMetaTypesItemCollections(ddlType.Items, true);

			if (pc["MDataMan_Type"] != null)
				CommonHelper.SafeSelect(ddlType, pc["MDataMan_Type"].ToString());
		}
		#endregion

		#region BindAvailableFields
		private void BindAvailableFields()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("FieldId", typeof(int));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("Description", typeof(string));
			dt.Columns.Add("DataType", typeof(string));
			dt.Columns.Add("DataTypeId", typeof(int));
			dt.Columns.Add("sortDataType", typeof(int));
			dt.Columns.Add("CanDelete", typeof(bool));

			MetaFieldCollection mfc = MetaField.GetList(MetaDataPlus.MetaNamespace.UserRoot, true);
			DataRow row;
			foreach (MetaField field in mfc)
			{
				bool fl = true;
				if (ddlShow.SelectedValue == "1" && field.OwnerMetaClassIdList.Count == 0)
					fl = false;
				if (ddlShow.SelectedValue == "-1" && field.OwnerMetaClassIdList.Count != 0)
					fl = false;
				if (fl)
				{
					row = dt.NewRow();
					row["FieldId"] = field.Id;
					row["FriendlyName"] = field.FriendlyName;
					row["Description"] = field.Description;
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
			if (pc["Cust_AvailableFields_Sort"] == null)
				pc["Cust_AvailableFields_Sort"] = "sortDataType";
			dv.Sort = pc["Cust_AvailableFields_Sort"];
			if (int.Parse(ddlType.SelectedValue) >= 0)
				dv.RowFilter = "DataTypeId = " + ddlType.SelectedValue;
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
			this.dgAvailableFields.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAvailableFields_DeleteCommand);
			this.dgAvailableFields.SortCommand += new DataGridSortCommandEventHandler(dgAvailableFields_SortCommand);
			this.dgAvailableFields.EditCommand += new DataGridCommandEventHandler(dgAvailableFields_EditCommand);
			this.dgAvailableFields.UpdateCommand += new DataGridCommandEventHandler(dgAvailableFields_UpdateCommand);
			this.dgAvailableFields.CancelCommand += new DataGridCommandEventHandler(dgAvailableFields_CancelCommand);
			this.btnApply.ServerClick += new EventHandler(btnApply_ServerClick);
			this.btnReset.ServerClick += new EventHandler(btnReset_ServerClick);
		}
		#endregion

		#region DataGrid Events
		private void dgAvailableFields_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int FieldId = int.Parse(e.Item.Cells[0].Text);
			MetaField field = MetaField.Load(FieldId);
			if (field.OwnerMetaClassIdList.Count == 0)
				MetaField.Delete(FieldId);

			Response.Redirect("~/Admin/MasterDataManager.aspx", true);
		}

		private void dgAvailableFields_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if ((pc["Cust_AvailableFields_Sort"] != null) && (pc["Cust_AvailableFields_Sort"] == e.SortExpression))
				pc["Cust_AvailableFields_Sort"] += " DESC";
			else pc["Cust_AvailableFields_Sort"] = e.SortExpression;
			BindAvailableFields();
		}

		private void dgAvailableFields_EditCommand(object source, DataGridCommandEventArgs e)
		{
			dgAvailableFields.EditItemIndex = e.Item.ItemIndex;
			dgAvailableFields.DataKeyField = "FieldId";
			BindAvailableFields();
		}

		private void dgAvailableFields_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			int ItemID = (int)dgAvailableFields.DataKeys[e.Item.ItemIndex];
			TextBox tbName = (TextBox)e.Item.FindControl("tbName");
			TextBox tbDescription = (TextBox)e.Item.FindControl("tbDescription");
			if (tbDescription != null && tbName != null && tbName.Text.Length > 0)
			{
				MetaField mf = MetaField.Load(ItemID);
				if (mf != null)
				{
					mf.FriendlyName = tbName.Text;
					mf.Description = tbDescription.Text;
				}
			}

			dgAvailableFields.EditItemIndex = -1;
			BindAvailableFields();
		}

		private void dgAvailableFields_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			dgAvailableFields.EditItemIndex = -1;
			BindAvailableFields();
		}
		#endregion

		#region Apply - Reset
		private void btnApply_ServerClick(object sender, EventArgs e)
		{
			pc["MDataMan_Type"] = ddlType.SelectedValue;
			pc["MDataMan_Show"] = ddlShow.SelectedValue;
			BindAvailableFields();
		}

		private void btnReset_ServerClick(object sender, EventArgs e)
		{
			pc["MDataMan_Type"] = "-1";
			pc["MDataMan_Show"] = "0";
			ddlType.ClearSelection();
			CommonHelper.SafeSelect(ddlType, pc["MDataMan_Type"].ToString());
			ddlShow.ClearSelection();
			CommonHelper.SafeSelect(ddlShow, pc["MDataMan_Show"].ToString());
			BindAvailableFields();
		}
		#endregion

	}
}
