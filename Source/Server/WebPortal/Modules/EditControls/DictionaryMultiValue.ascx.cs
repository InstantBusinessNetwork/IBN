namespace Mediachase.UI.Web.Modules.EditControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using System.Resources;
	using MetaDataPlus;
	using MetaDataPlus.Configurator;
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.Interfaces;
	using System.Globalization;
	using System.Web.UI;
	using System.Linq;
	using System.Collections.Generic;

	/// <summary>
	///		Summary description for DictionaryMultivalue.
	/// </summary>
	public partial class DictionaryMultivalue : System.Web.UI.UserControl, ICustomField
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(DictionaryMultivalue).Assembly);

		private int fieldId = -1;
		private MetaDictionaryItem[] savedItems = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			
		}

		#region BindGrid
		private void BindGrid()
		{
			MetaField field = MetaField.Load(fieldId);

			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("Name", typeof(string));

			foreach (MetaDictionaryItem item in field.Dictionary)
			{
				DataRow row = dt.NewRow();
				row["Id"] = item.Id;
				row["Name"] = "&nbsp;" + item.Value;
				dt.Rows.Add(row);
			}
			grdMain.DataSource = dt.DefaultView;
			grdMain.DataBind();
		} 
		#endregion

		#region RestoreValues
		private void RestoreValues()
		{
			if (savedItems != null && savedItems.Length > 0)
			{
				foreach (DataGridItem dgi in grdMain.Items)
				{
					foreach (Control control in dgi.Cells[1].Controls)
					{
						if (control is CheckBox)
						{
							CheckBox checkBox = (CheckBox)control;
							int itemId = int.Parse(dgi.Cells[0].Text);

							if (savedItems.Any(item => item.Id == itemId))
								checkBox.Checked = true;
							else
								checkBox.Checked = false;
						}
					}
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
		}
		#endregion

		#region InitControl
		public void InitControl(int FieldId)
		{
			fieldId = FieldId;
			MetaField field = MetaField.Load(fieldId);

			btnEditItems.Visible = false;
			if (Security.IsManager() && field.DataType == MetaDataType.DictionaryMultivalue)
			{
				btnEditItems.Visible = true;
				btnEditItems.Attributes.Add("onclick", String.Format("EditItems({0})", fieldId));

				if (!Page.ClientScript.IsClientScriptBlockRegistered("EditItems"))
				{
					String scriptString = "<script language=JavaScript>\r\n";
					scriptString += "function EditItems(FieldId)\r\n";
					scriptString += "{\r\n";
					scriptString += "	var w = 640;\r\n";
					scriptString += "	var h = 350;\r\n";
					scriptString += "	var l = (screen.width - w) / 2;\r\n";
					scriptString += "	var t = (screen.height - h) / 2;\r\n";
					scriptString += "	var winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;\r\n";
					scriptString += String.Format("	window.open('{0}?Id=' + FieldId, 'EditItems', winprops);\r\n", ResolveUrl("~/Common/MetaDictionary.aspx"));
					scriptString += "}\r\n";
					scriptString += "</script>\r\n";

					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EditItems", scriptString);
				}
			}
		} 
		#endregion

		#region Implementation of ICustomField
		public object Value
		{
			set
			{
				savedItems = (MetaDictionaryItem[])value;
			}
			get
			{
				object retval = null;
				if (fieldId > 0)
				{
					ArrayList items = new ArrayList();
					MetaField field = MetaField.Load(fieldId);

					foreach (DataGridItem dgi in grdMain.Items)
					{
						foreach (Control control in dgi.Cells[1].Controls)
						{
							if (control is CheckBox)
							{
								CheckBox checkBox = (CheckBox)control;
								if (checkBox.Checked)
								{
									int itemId = int.Parse(dgi.Cells[0].Text);
									MetaDictionaryItem item = field.Dictionary.GetItem(itemId);
									if (item != null)
										items.Add(item);
								}
							}
						}
					}
					retval = items.ToArray(typeof(MetaDictionaryItem));
				}
				return retval;
			}
		}

		private string fieldName = "";
		public string FieldName
		{
			set
			{
				fieldName = value;
			}
			get
			{
				return fieldName;
			}
		}

		private bool allowEmptyValues = false;
		public bool AllowEmptyValues
		{
			set
			{
				allowEmptyValues = value;
				vldCustom.Visible = !allowEmptyValues;
			}
			get
			{
				return allowEmptyValues;
			}
		}
		#endregion

		#region vldCustom_ServerValidate
		protected void vldCustom_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (!AllowEmptyValues)
			{
				if (((Array)Value).Length <= 0)
					args.IsValid = false;
			}
		}
		#endregion

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (IsPostBack)
			{
				// ReGet checked items to savedItems
				Value = Value;
			}

			BindGrid();
			RestoreValues();
		}
	}
}
