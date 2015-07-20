namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using MetaDataPlus.Configurator;
	using Mediachase.UI.Web.Util;
	using Mediachase.IBN.Business;
	using System.Reflection;

	/// <summary>
	///		Summary description for FieldEdit.
	/// </summary>
	public partial class FieldEdit : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRMDict = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strManageDictionaries", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		private int _classId = -1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			LoadRequestVariables();
			ApplyLocalization();

			if (!Page.IsPostBack)
				BindValues();

			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			lblErrorMessage.Visible = false;
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
		}

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request["ClassID"] != null)
			{
				try
				{
					_classId = int.Parse(Request["ClassID"]);
				}
				catch { }
			}
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			Mediachase.UI.Web.Util.CommonHelper.BindMetaTypesItemCollections(ddlType.Items, false);

			if (_classId > 0)
			{
				MetaClass mc = MetaClass.Load(_classId);
				lblElementName.Text = mc.Parent.FriendlyName;
				if (mc.Parent.TableName.ToLower() == "projects" || mc.Parent.TableName.ToLower() == "list_items")
					lblElementName.Text += " - " + mc.FriendlyName;
				trElement.Visible = true;

				trAllowNulls.Visible = true;
			}
			else
			{
				trElement.Visible = false;
				trAllowNulls.Visible = false;
			}

			ShowHide();

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Index", typeof(int)));
			dt.Columns.Add(new DataColumn("Value", typeof(string)));

			ViewState["DicItems"] = dt;

			BindDG();
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			DataTable dt = (DataTable)ViewState["DicItems"];
			dgDic.Columns[1].HeaderText = "#";
			dgDic.Columns[2].HeaderText = LocRM.GetString("tValue");

			DataView dv = dt.DefaultView;
			dv.Sort = "Index";
			dgDic.DataSource = dv;
			dgDic.DataBind();

			foreach (DataGridItem dgi in dgDic.Items)
			{
				DropDownList ddl = (DropDownList)dgi.FindControl("ddIndex");
				if (ddl != null)
				{
					for (int i = 1; i <= dt.Rows.Count; i++)
						ddl.Items.Add(new ListItem(i.ToString(), i.ToString()));
					ddl.SelectedValue = (dgDic.EditItemIndex + 1).ToString();
				}
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
			this.dgDic.ItemCommand += new DataGridCommandEventHandler(dgDic_ItemCommand);
			this.dgDic.CancelCommand += new DataGridCommandEventHandler(dgDic_CancelCommand);
			this.dgDic.EditCommand += new DataGridCommandEventHandler(dgDic_EditCommand);
			this.dgDic.UpdateCommand += new DataGridCommandEventHandler(dgDic_UpdateCommand);
			this.dgDic.DeleteCommand += new DataGridCommandEventHandler(dgDic_DeleteCommand);
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			secHeader.Title = LocRM.GetString("CreateField");
			//lblDicItems.Text = LocRM.GetString("DictItems");
			lgdDicItems.InnerText = LocRM.GetString("DictItems");

			chkAllowNulls.Text = LocRM.GetString("FieldAllowNulls");
			chkSaveHistory.Text = LocRM.GetString("FieldSaveHistory");
			chkAllowSearch.Text = LocRM.GetString("FieldAllowSearch");
			chkEditable.Text = LocRM.GetString("Editable");
			chkMultiline.Text = LocRM.GetString("Multiline");
			reNameVal.ErrorMessage = LocRM.GetString("tOnlyLettersAndDigits");
		}
		#endregion

		#region btnCancel_ServerClick
		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			if (Request["Back"] == "All")
				Response.Redirect("~/Admin/MasterDataManager.aspx", true);
			else
				Response.Redirect("~/Admin/Customization.aspx", true);
		}
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			if (tbName.Text.Length == 0)
				tbName.Text = tbFriendlyName.Text;
			tbName.Text = cyr2lat(tbName.Text);
			tbName.Text = tbName.Text.Replace(" ", "_").Replace("-", "_").Replace("_&_", "_").Replace("/", "_").Replace("~", "_").Replace("`", "_");
			reNameVal.Validate();
			if (!reNameVal.IsValid)
			{
				if (int.Parse(ddlType.SelectedValue) == 0)
					tbName.Text = "Dictionary_Field";
				else
					tbName.Text = ((MetaDataType)(int.Parse(ddlType.SelectedValue))).ToString() + "_Field";
			}
			Page.Validate();
			if (!Page.IsValid)
				return;

			int DataTypeId = int.Parse(ddlType.SelectedItem.Value);
			MetaDataType type;
			if (DataTypeId != 0)
			{
				type = MetaType.Load(DataTypeId).MetaDataType;
			}
			else
			{
				if (chkMultiline.Checked && chkEditable.Checked)
					type = MetaDataType.DictionaryMultivalue;
				else if (!chkMultiline.Checked && chkEditable.Checked)
					type = MetaDataType.DictionarySingleValue;
				else if (chkMultiline.Checked && !chkEditable.Checked)
					type = MetaDataType.EnumMultivalue;
				else
					type = MetaDataType.EnumSingleValue;
			}

			bool SaveHistory = false;
			if (chkSaveHistory.Visible)
				SaveHistory = chkSaveHistory.Checked;

			bool AllowSearch = false;
			if (chkAllowSearch.Visible)
				AllowSearch = chkAllowSearch.Checked;

			MetaField field = null;
			string sNameSpace = Mediachase.MetaDataPlus.MetaNamespace.UserRoot;
			try
			{
				field = MetaField.Create(sNameSpace, MetaFieldFix.GetUniqueName(tbName.Text), tbFriendlyName.Text, tbDescription.Text,
					type, true, SaveHistory, AllowSearch);
			}
			catch (SqlException sqlException)
			{
				if (sqlException.Number == 2627 || sqlException.Number == 50000)
				{
					lblErrorMessage.Text = LocRM.GetString("FieldNameDuplictaion");
					lblErrorMessage.Visible = true;
					return;
				}
			}
			int iClassId = -1;
			if (_classId > 0)
				iClassId = _classId;

			if (iClassId > 0)
			{
				MetaClass mc = MetaClass.Load(iClassId);

				mc.AddField(field);

				if (trAllowNulls.Visible)
				{
					mc.SetFieldIsRequired(field.Name, !chkAllowNulls.Checked);
				}
			}
			if (field.Dictionary != null)
			{
				DataView dv = ((DataTable)ViewState["DicItems"]).DefaultView;
				dv.Sort = "Index";
				foreach (DataRowView dr in dv)
					field.Dictionary.Add(dr["Value"].ToString());
			}

			if (Request["Back"] == "All")
				Response.Redirect("~/Admin/MasterDataManager.aspx", true);
			else
				Response.Redirect("~/Admin/Customization.aspx", true);
		}
		#endregion

		#region ddlType_SelectedIndexChanged
		protected void ddlType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ShowHide();

			BindDG();
		}
		#endregion

		#region ShowHide
		private void ShowHide()
		{
			int SelectedValue = int.Parse(ddlType.SelectedItem.Value);
			if (SelectedValue == 0)
			{
				chkMultiline.Visible = true;
				chkEditable.Visible = true;
			}
			else
			{
				chkMultiline.Visible = false;
				chkEditable.Visible = false;
			}

			if (SelectedValue == (int)MetaDataType.LongHtmlString
				|| SelectedValue == (int)MetaDataType.LongString
				|| SelectedValue == (int)MetaDataType.File
				|| SelectedValue == (int)MetaDataType.ImageFile
				|| SelectedValue == 0)
				chkSaveHistory.Visible = false;
			else
				chkSaveHistory.Visible = true;

			if (SelectedValue == (int)MetaDataType.Email
				|| SelectedValue == (int)MetaDataType.Url
				|| SelectedValue == (int)MetaDataType.ShortString
				|| SelectedValue == (int)MetaDataType.LongString
				|| SelectedValue == (int)MetaDataType.LongHtmlString)
				chkAllowSearch.Visible = true;
			else
				chkAllowSearch.Visible = false;
		}
		#endregion

		#region dgDic_ItemCommand
		private void dgDic_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Add")
			{
				DataTable dt = ((DataTable)ViewState["DicItems"]).Copy();
				DataRow dr = dt.NewRow();
				dr["Id"] = -1;
				dr["Value"] = "";
				dr["Index"] = dt.Rows.Count;
				dt.Rows.Add(dr);

				dgDic.EditItemIndex = dt.Rows.Count - 1;
				dgDic.DataKeyField = "Id";
				DataView dv = dt.DefaultView;
				dv.Sort = "Index";
				dgDic.DataSource = dv;
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
			else if (e.CommandName == "SortAsc")
			{
				DataTable dt = ((DataTable)ViewState["DicItems"]).Copy();
				DataView dv = dt.DefaultView;
				dv.Sort = "Value";
				for (int i = 0; i < dv.Count; i++)
					dv[i]["Index"] = i;

				ViewState["DicItems"] = dt;
				BindDG();
			}
			else if (e.CommandName == "SortDesc")
			{
				DataTable dt = ((DataTable)ViewState["DicItems"]).Copy();
				DataView dv = dt.DefaultView;
				dv.Sort = "Value DESC";
				for (int i = 0; i < dv.Count; i++)
					dv[i]["Index"] = i;

				ViewState["DicItems"] = dt;
				BindDG();
			}
		}
		#endregion

		#region dgDic_CancelCommand
		private void dgDic_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			dgDic.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region dgDic_EditCommand
		private void dgDic_EditCommand(object source, DataGridCommandEventArgs e)
		{
			dgDic.EditItemIndex = e.Item.ItemIndex;
			dgDic.DataKeyField = "Id";
			BindDG();
		}
		#endregion

		#region dgDic_UpdateCommand
		private void dgDic_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			int ItemID = (int)dgDic.DataKeys[e.Item.ItemIndex];
			TextBox tbValue = (TextBox)e.Item.FindControl("tbValue");
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddIndex");
			DataTable dt = (DataTable)ViewState["DicItems"];
			if (tbValue != null && tbValue.Text.Length > 0 && ddl != null)
			{
				if (ItemID > 0)
				{
					DataRow[] dr = dt.Select("Id = " + ItemID);
					if (dr.Length > 0)
					{
						dt.Rows.Remove(dr[0]);
						DataRow drNew = dt.NewRow();
						drNew["Id"] = ItemID;
						drNew["Value"] = tbValue.Text;
						drNew["Index"] = int.Parse(ddl.SelectedValue) - 1;
						dt.Rows.InsertAt(drNew, int.Parse(ddl.SelectedValue) - 1);
						for (int i = 0; i < dt.Rows.Count; i++)
							dt.Rows[i]["Index"] = i;
					}
				}
				else
				{
					int max = 0;
					foreach (DataRow dr in dt.Rows)
						if ((int)dr["Id"] > max)
							max = (int)dr["Id"];
					DataRow drNew = dt.NewRow();
					drNew["Id"] = max + 1;
					drNew["Value"] = tbValue.Text;
					drNew["Index"] = int.Parse(ddl.SelectedValue) - 1;
					dt.Rows.InsertAt(drNew, int.Parse(ddl.SelectedValue) - 1);
					for (int i = 0; i < dt.Rows.Count; i++)
						dt.Rows[i]["Index"] = i;
				}
			}
			ViewState["DicItems"] = dt;
			dgDic.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region dgDic_DeleteCommand
		private void dgDic_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			DataTable dt = (DataTable)ViewState["DicItems"];
			int ItemId = int.Parse(e.Item.Cells[0].Text);
			DataRow[] dr = dt.Select("Id = " + ItemId);
			if (dr.Length > 0)
				dt.Rows.Remove(dr[0]);
			for (int i = 0; i < dt.Rows.Count; i++)
				dt.Rows[i]["Index"] = i;
			ViewState["DicItems"] = dt;
			BindDG();
		}
		#endregion

		#region cyr2lat
		public static string cyr2lat(string str)
		{
			Hashtable eng = new Hashtable();
			eng["À"] = "A";
			eng["Á"] = "B";
			eng["Â"] = "V";
			eng["Ã"] = "G";
			eng["Ä"] = "D";
			eng["Å"] = "E";
			eng["¨"] = "YO";
			eng["Æ"] = "ZH";
			eng["Ç"] = "Z";
			eng["È"] = "I";
			eng["É"] = "J";
			eng["Ê"] = "K";
			eng["Ë"] = "L";
			eng["Ì"] = "M";
			eng["Í"] = "N";
			eng["Î"] = "O";
			eng["Ï"] = "P";
			eng["Ð"] = "R";
			eng["Ñ"] = "S";
			eng["Ò"] = "T";
			eng["Ó"] = "U";
			eng["Ô"] = "F";
			eng["Õ"] = "H";
			eng["Ö"] = "C";
			eng["×"] = "CH";
			eng["Ø"] = "SH";
			eng["Ù"] = "SCH";
			eng["Ú"] = "'";
			eng["Û"] = "Y";
			eng["Ü"] = "'";
			eng["Ý"] = "E'";
			eng["Þ"] = "YU";
			eng["ß"] = "YA";
			eng["à"] = "a";
			eng["á"] = "b";
			eng["â"] = "v";
			eng["ã"] = "g";
			eng["ä"] = "d";
			eng["å"] = "e";
			eng["¸"] = "e";
			eng["æ"] = "zh";
			eng["ç"] = "z";
			eng["è"] = "i";
			eng["é"] = "j";
			eng["ê"] = "k";
			eng["ë"] = "l";
			eng["ì"] = "m";
			eng["í"] = "n";
			eng["î"] = "o";
			eng["ï"] = "p";
			eng["ð"] = "r";
			eng["ñ"] = "s";
			eng["ò"] = "t";
			eng["ó"] = "u";
			eng["ô"] = "f";
			eng["õ"] = "h";
			eng["ö"] = "c";
			eng["÷"] = "ch";
			eng["ø"] = "sh";
			eng["ù"] = "sch";
			eng["ú"] = "'";
			eng["û"] = "y";
			eng["ü"] = "'";
			eng["ý"] = "e";
			eng["þ"] = "yu";
			eng["ÿ"] = "ya";

			string msg = "";
			for (int i = 0; i < str.Length; i++)
			{
				string tmp = str.Substring(i, 1);
				if (eng[tmp] == eng[" "]) msg += tmp;
				else msg += eng[tmp];
			}
			return msg;
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			if (ddlType.SelectedValue == "0")
			{
				fsItems.Visible = true;
			}
			else
			{
				fsItems.Visible = false;
			}
		}
		#endregion
	}
}
