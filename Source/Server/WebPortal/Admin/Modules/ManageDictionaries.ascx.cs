namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Drawing;
	using System.Globalization;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.SpreadSheet;
	using Mediachase.Ibn.Data.Meta;
	using Mediachase.Ibn.Data.Meta.Management;
	using Mediachase.Ibn.Lists;
	using Mediachase.Ibn.Web.UI;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for ManageDictionaries.
	/// </summary>
	public partial class ManageDictionaries : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strManageDictionaries", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strCalendarList", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			lblError.Visible = false;

			BindToolbar();
			if (!IsPostBack)
			{
				btnAddNewItem.Text = LocRM.GetString("AddNewItem");
				BindDictionariesTypes();
				BindDG();
			}
		}

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			dgDic.Columns[2].Visible = false;

			if (ddDictionaries.SelectedValue == DictionaryTypes.ProjectPhases.ToString() ||
				ddDictionaries.SelectedValue == DictionaryTypes.RiskLevels.ToString())
			{
				dgDic.Columns[2].Visible = true;
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tTitle");
			secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/newitem.gif") + "'/> " + LocRM.GetString("AddNewItem"), Page.ClientScript.GetPostBackClientHyperlink(btnAddNewItem, ""));
			secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM2.GetString("BusData"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin2"));
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			if (ddDictionaries.SelectedValue == "0")//Business Score
			{
				dgScore.Visible = true;
				dgPlanSlot.Visible = false;
				dgDic.Visible = false;
				dgEnum.Visible = false;
				dgScore.Columns[1].HeaderText = LocRM.GetString("tKey");
				dgScore.Columns[2].HeaderText = LocRM.GetString("Name");

				dgScore.DataSource = GetTable(BusinessScore.List()).DefaultView;
				dgScore.DataBind();

				foreach (DataGridItem dgi in dgScore.Items)
				{
					ImageButton ib = (ImageButton)dgi.FindControl("ibDelete2");
					if (ib != null)
						ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");

					RequiredFieldValidator rf = (RequiredFieldValidator)dgi.FindControl("rfKey");
					if (rf != null)
						rf.ErrorMessage = LocRM.GetString("Required");

					RequiredFieldValidator rf2 = (RequiredFieldValidator)dgi.FindControl("rfName2");
					if (rf2 != null)
						rf2.ErrorMessage = LocRM.GetString("Required");
				}
			}
			else if (ddDictionaries.SelectedValue == "-1")//Base Plan Slot
			{
				dgScore.Visible = false;
				dgDic.Visible = false;
				dgPlanSlot.Visible = true;
				dgEnum.Visible = false;
				dgPlanSlot.Columns[1].HeaderText = LocRM.GetString("Name");
				dgPlanSlot.Columns[2].HeaderText = LocRM.GetString("tIsDefault");

				dgPlanSlot.DataSource = GetTableSlot(BasePlanSlot.List()).DefaultView;
				dgPlanSlot.DataBind();

				foreach (DataGridItem dgi in dgPlanSlot.Items)
				{
					ImageButton ib = (ImageButton)dgi.FindControl("ibDelete3");
					if (ib != null)
						ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");

					RequiredFieldValidator rf3 = (RequiredFieldValidator)dgi.FindControl("rfName3");
					if (rf3 != null)
						rf3.ErrorMessage = LocRM.GetString("Required");
				}
			}
			else if (ddDictionaries.SelectedValue == ListManager.ListTypeEnumName)	//List Types
			{
				dgScore.Visible = false;
				dgPlanSlot.Visible = false;
				dgDic.Visible = false;
				dgEnum.Visible = true;

				dgEnum.DataSource = GetListTable().DefaultView;
				dgEnum.DataBind();

				foreach (DataGridItem row in dgEnum.Items)
				{
					ImageButton ib = (ImageButton)row.FindControl("ibDelete");
					if (ib != null)
						ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Delete").ToString() + "?')");
					TextBox tb = (TextBox)row.FindControl("txtName");
					HtmlImage im = (HtmlImage)row.FindControl("imResourceTemplate");
					RequiredFieldValidator rfv = (RequiredFieldValidator)row.FindControl("rfName");
					if (im != null && tb != null && rfv != null)
					{
						im.Attributes.Add("onclick", "SetText('" + tb.ClientID + "','{ResourceName:ResourceKey}','" + rfv.ClientID + "')");
					}
				}

				if (dgEnum.EditItemIndex >= 0)
				{
					DropDownList ddl = (DropDownList)dgEnum.Items[dgEnum.EditItemIndex].FindControl("ddlOrder");
					if (ddl != null)
					{
						for (int i = 1; i <= dgEnum.Items.Count; i++)
						{
							ddl.Items.Add(i.ToString());
						}
						ddl.SelectedIndex = dgEnum.EditItemIndex;
					}
				}
			}
			else
			{
				dgScore.Visible = false;
				dgPlanSlot.Visible = false;
				dgDic.Visible = true;
				dgEnum.Visible = false;
				dgDic.Columns[1].HeaderText = LocRM.GetString("Name");
				dgDic.Columns[2].HeaderText = LocRM.GetString("tWeight");
				//dgDic.Columns[3].HeaderText = LocRM.GetString("Options");

				DictionaryTypes dic = (DictionaryTypes)Enum.Parse(typeof(DictionaryTypes), ddDictionaries.SelectedItem.Value);
				DataView dv = Dictionaries.GetList(dic).DefaultView;
				//			dv.Sort = "ItemName";
				dgDic.DataSource = dv;
				dgDic.DataBind();

				foreach (DataGridItem dgi in dgDic.Items)
				{
					ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
					if (ib != null)
						ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");

					RequiredFieldValidator rf = (RequiredFieldValidator)dgi.FindControl("rfName");
					if (rf != null)
						rf.ErrorMessage = LocRM.GetString("Required");
				}
			}
		}
		#endregion

		#region BindDictionariesTypes
		private void BindDictionariesTypes()
		{
			ddDictionaries.Items.Add(new ListItem(LocRM.GetString("Categories"),
				DictionaryTypes.Categories.ToString()));
			if (Configuration.ProjectManagementEnabled)
			{
				ddDictionaries.Items.Add(new ListItem(LocRM.GetString("ProjectTypes"),
					DictionaryTypes.ProjectTypes.ToString()));
				ddDictionaries.Items.Add(new ListItem(LocRM.GetString("ProjectCategories"),
					DictionaryTypes.ProjectCategories.ToString()));
				ddDictionaries.Items.Add(new ListItem(LocRM.GetString("Currency"),
					DictionaryTypes.Currency.ToString()));
				ddDictionaries.Items.Add(new ListItem(LocRM.GetString("ProjectPhases"),
					DictionaryTypes.ProjectPhases.ToString()));
				ddDictionaries.Items.Add(new ListItem(LocRM.GetString("RiskLevels"),
					DictionaryTypes.RiskLevels.ToString()));
			}
			if (Configuration.HelpDeskEnabled)
			{
				ddDictionaries.Items.Add(new ListItem(LocRM.GetString("IncidentSeverities"),
					DictionaryTypes.IncidentSeverities.ToString()));
				ddDictionaries.Items.Add(new ListItem(LocRM.GetString("IncidentTypes"),
					DictionaryTypes.IncidentTypes.ToString()));
				ddDictionaries.Items.Add(new ListItem(LocRM.GetString("IncidentCategories"),
					DictionaryTypes.IncidentCategories.ToString()));
			}
			ddDictionaries.Items.Add(new ListItem(LocRM.GetString("DocumentStatus"),
				DictionaryTypes.DocumentStatus.ToString()));
			ddDictionaries.Items.Add(new ListItem(LocRM.GetString("ListTypes"),
				ListManager.ListTypeEnumName));

			if (Configuration.ProjectManagementEnabled)
			{
				ddDictionaries.Items.Add(new ListItem(LocRM.GetString("tBusinessScore"), "0"));
				ddDictionaries.Items.Add(new ListItem(LocRM.GetString("tBasePlanSlot"), "-1"));
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

			this.dgDic.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_cancel);
			this.dgDic.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_edit);
			this.dgDic.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_update);
			this.dgDic.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);

			this.dgScore.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_cancel2);
			this.dgScore.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_edit2);
			this.dgScore.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_update2);
			this.dgScore.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete2);

			this.dgPlanSlot.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_cancel3);
			this.dgPlanSlot.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_edit3);
			this.dgPlanSlot.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_update3);
			this.dgPlanSlot.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete3);

			this.dgEnum.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_cancel);
			this.dgEnum.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_edit);
			this.dgEnum.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_update);
			this.dgEnum.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);
		}
		#endregion

		#region GetVisibleStatus
		protected bool GetVisibleStatus(object val)
		{
			int status = (int)val;
			if (status == 0)
				return false;
			else
				return true;
		}
		#endregion

		#region ddDic_ChangeDictionary
		protected void ddDic_ChangeDictionary(object sender, System.EventArgs e)
		{
			dgDic.EditItemIndex = -1;
			dgEnum.EditItemIndex = -1;
			dgPlanSlot.EditItemIndex = -1;
			dgScore.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region dg_delete
		private void dg_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string dictionaryName = ddDictionaries.SelectedItem.Value;
			if (dictionaryName == ListManager.ListTypeEnumName)
			{
				MetaFieldType mft = Mediachase.Ibn.Core.MetaDataWrapper.GetEnumByName(dictionaryName);
				if (mft != null)
				{
					MetaEnum.RemoveItem(mft, int.Parse(e.CommandArgument.ToString()));
				}
				dgEnum.EditItemIndex = -1;
			}
			else
			{
				int ItemID = int.Parse(e.Item.Cells[0].Text);
				DictionaryTypes dic = (DictionaryTypes)Enum.Parse(typeof(DictionaryTypes), dictionaryName);
				Dictionaries.DeleteItem(ItemID, dic);
				dgDic.EditItemIndex = -1;
			}

			BindDG();
		}
		#endregion

		#region dg_edit
		private void dg_edit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string dictionaryName = ddDictionaries.SelectedItem.Value;
			if (dictionaryName == ListManager.ListTypeEnumName)
			{
				dgEnum.EditItemIndex = e.Item.ItemIndex;
			}
			else
			{
				dgDic.EditItemIndex = e.Item.ItemIndex;
			}
			BindDG();
		}
		#endregion

		#region dg_cancel
		private void dg_cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string dictionaryName = ddDictionaries.SelectedItem.Value;
			if (dictionaryName == ListManager.ListTypeEnumName)
			{
				dgEnum.EditItemIndex = -1;
			}
			else
			{
				dgDic.EditItemIndex = -1;
			}
			BindDG();
		}
		#endregion

		#region dg_update
		private void dg_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string dictionaryName = ddDictionaries.SelectedItem.Value;
			if (dictionaryName == ListManager.ListTypeEnumName)
			{
				MetaFieldType mft = Mediachase.Ibn.Core.MetaDataWrapper.GetEnumByName(dictionaryName);

				int ItemId = int.Parse(e.CommandArgument.ToString());

				TextBox tb = (TextBox)e.Item.FindControl("txtName");
				DropDownList ddl = (DropDownList)e.Item.FindControl("ddlOrder");
				int OrderId = int.Parse(ddl.SelectedValue);

				if (tb != null && tb.Text.Trim() != String.Empty)
				{
					if (ItemId > 0)
						MetaEnum.UpdateItem(mft, ItemId, tb.Text.Trim(), OrderId);
					else
						MetaEnum.AddItem(mft, tb.Text.Trim(), OrderId);
				}
				dgEnum.EditItemIndex = -1;
			}
			else
			{
				int ItemID = int.Parse(e.Item.Cells[0].Text);
				DictionaryTypes dic = (DictionaryTypes)Enum.Parse(typeof(DictionaryTypes), ddDictionaries.SelectedItem.Value);
				TextBox tb = (TextBox)e.Item.FindControl("tbName");
				int _weight = 0;
				TextBox tbW = (TextBox)e.Item.FindControl("tbWeight");
				if (tbW != null && tbW.Text.Length > 0)
				{
					_weight = int.Parse(tbW.Text);
				}
				if (tb != null)
				{
					try
					{
						if (ItemID > 0)
							Dictionaries.UpdateItem(ItemID, tb.Text, _weight, dic);
						else
							Dictionaries.AddItem(tb.Text, _weight, dic);
					}
					catch (SqlException ex)
					{
						if (ex.Number == 2627)	// Violation of UNIQUE KEY
						{
							lblError.Visible = true;
							lblError.Text = "<br><br>" + LocRM.GetString("Duplication");
						}
					}
				}
				dgDic.EditItemIndex = -1;
			}

			BindDG();
		}
		#endregion

		#region dg_delete2
		private void dg_delete2(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int ItemID = int.Parse(e.Item.Cells[0].Text);
			BusinessScore.Delete(ItemID);
			dgScore.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region dg_edit2
		private void dg_edit2(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgScore.EditItemIndex = e.Item.ItemIndex;
			BindDG();
		}
		#endregion

		#region dg_cancel2
		private void dg_cancel2(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgScore.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region dg_update2
		private void dg_update2(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int ItemID = int.Parse(e.Item.Cells[0].Text);
			TextBox tbK = (TextBox)e.Item.FindControl("tbKey");
			TextBox tbN = (TextBox)e.Item.FindControl("tbName2");
			if (tbK != null && tbN != null)
			{
				if (ItemID > 0)
				{
					BusinessScore bs = BusinessScore.Load(ItemID);
					bs.Key = tbK.Text;
					bs.Name = tbN.Text;
					BusinessScore.Update(bs);
				}
				else
					BusinessScore.Create(tbK.Text, tbN.Text);
			}

			dgScore.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region dg_delete3
		private void dg_delete3(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int ItemID = int.Parse(e.Item.Cells[0].Text);
			BasePlanSlot.Delete(ItemID);
			dgPlanSlot.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region dg_edit3
		private void dg_edit3(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgPlanSlot.EditItemIndex = e.Item.ItemIndex;
			BindDG();
		}
		#endregion

		#region dg_cancel3
		private void dg_cancel3(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgPlanSlot.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region dg_update3
		private void dg_update3(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int ItemID = int.Parse(e.Item.Cells[0].Text);
			TextBox tbN = (TextBox)e.Item.FindControl("tbName3");
			CheckBox cb = (CheckBox)e.Item.FindControl("cbIsDef");
			if (cb != null && tbN != null)
			{
				if (ItemID > 0)
				{
					BasePlanSlot bs = BasePlanSlot.Load(ItemID);
					bs.IsDefault = cb.Checked;
					bs.Name = tbN.Text;
					BasePlanSlot.Update(bs);
				}
				else
					BasePlanSlot.Create(tbN.Text, cb.Checked);
			}

			dgPlanSlot.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region btnAddNewItem_Click
		protected void btnAddNewItem_Click(object sender, System.EventArgs e)
		{
			if (ddDictionaries.SelectedValue == "0")
			{
				DataTable dt = GetTable(BusinessScore.List());

				DataRow dr = dt.NewRow();
				dr["BusinessScoreId"] = -1;
				dr["Key"] = "";
				dr["Name"] = "";
				dt.Rows.Add(dr);

				dgScore.EditItemIndex = dt.Rows.Count - 1;
				dgScore.DataSource = dt.DefaultView;
				dgScore.DataBind();
			}
			else if (ddDictionaries.SelectedValue == "-1")
			{
				DataTable dt = GetTableSlot(BasePlanSlot.List());

				DataRow dr = dt.NewRow();
				dr["BasePlanSlotId"] = -1;
				dr["Name"] = "";
				dr["IsDefault"] = false;
				dt.Rows.Add(dr);

				dgPlanSlot.EditItemIndex = dt.Rows.Count - 1;
				dgPlanSlot.DataSource = dt.DefaultView;
				dgPlanSlot.DataBind();
			}
			else if (ddDictionaries.SelectedValue == ListManager.ListTypeEnumName)	//List Types
			{
				DataTable dt = GetListTable();

				DataRow dr = dt.NewRow();
				dr["Id"] = -1;
				dr["OrderId"] = dt.Rows.Count + 1;
				dr["Name"] = "";
				dt.Rows.Add(dr);

				dgEnum.EditItemIndex = dt.Rows.Count - 1;
				dgEnum.DataSource = dt.DefaultView;
				dgEnum.DataBind();

				DropDownList ddl = (DropDownList)dgEnum.Items[dgEnum.EditItemIndex].FindControl("ddlOrder");
				if (ddl != null)
				{
					for (int i = 1; i <= dgEnum.Items.Count; i++)
					{
						ddl.Items.Add(i.ToString());
					}
					ddl.SelectedIndex = dgEnum.EditItemIndex;
				}
			}
			else
			{
				DictionaryTypes dic = (DictionaryTypes)Enum.Parse(typeof(DictionaryTypes), ddDictionaries.SelectedItem.Value);

				DataTable dt = Dictionaries.GetList(dic);

				DataRow dr = dt.NewRow();
				dr["ItemId"] = -1;
				dr["ItemName"] = "";
				dr["Weight"] = 0;
				dr["CanDelete"] = 0;
				dt.Rows.Add(dr);

				dgDic.EditItemIndex = dt.Rows.Count - 1;
				dgDic.DataSource = dt.DefaultView;
				dgDic.DataBind();
			}
		}
		#endregion

		#region GetListTable
		private DataTable GetListTable()
		{
			MetaFieldType mft = Mediachase.Ibn.Core.MetaDataWrapper.GetEnumByName(ddDictionaries.SelectedItem.Value);

			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("OrderId", typeof(int));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("DisplayName", typeof(string));

			foreach (MetaEnumItem item in MetaEnum.GetItems(mft))
			{
				DataRow row = dt.NewRow();
				row["Id"] = item.Handle;
				row["OrderId"] = item.OrderId;
				row["Name"] = item.Name;
				row["DisplayName"] = CHelper.GetResFileString(item.Name);
				dt.Rows.Add(row);
			}
			return dt;
		}
		#endregion

		#region GetTable
		private DataTable GetTable(BusinessScore[] _list)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("BusinessScoreId", typeof(int)));
			dt.Columns.Add(new DataColumn("Key", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			DataRow dr;
			foreach (BusinessScore bs in _list)
			{
				dr = dt.NewRow();
				dr["BusinessScoreId"] = bs.BusinessScoreId;
				dr["Key"] = bs.Key;
				dr["Name"] = bs.Name;
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		#region GetTableSlot
		private DataTable GetTableSlot(BasePlanSlot[] _list)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("BasePlanSlotId", typeof(int)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("IsDefault", typeof(bool)));
			DataRow dr;
			foreach (BasePlanSlot bs in _list)
			{
				dr = dt.NewRow();
				dr["BasePlanSlotId"] = bs.BasePlanSlotId;
				dr["Name"] = bs.Name;
				dr["IsDefault"] = bs.IsDefault;
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion
	}
}
