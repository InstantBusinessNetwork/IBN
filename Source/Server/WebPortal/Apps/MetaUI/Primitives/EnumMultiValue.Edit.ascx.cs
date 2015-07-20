using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class EnumMultiValue_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (CHelper.GetFromContext(String.Format("{0}_MultiValue", FieldName)) != null &&
				Value != CHelper.GetFromContext(String.Format("{0}_MultiValue", FieldName)))
			{
				List<int> lst = new List<int>((int[])CHelper.GetFromContext(String.Format("{0}_MultiValue", FieldName)));

				foreach (DataGridItem dgi in grdMain.Items)
				{
					foreach (Control control in dgi.Cells[1].Controls)
					{
						if (control is CheckBox)
						{
							CheckBox checkBox = (CheckBox)control;
							int itemId = int.Parse(dgi.Cells[0].Text);

							if (lst.Contains(itemId))
								checkBox.Checked = true;
						}
					}
				}
			}
		}

		private string _guid = "718E6421-0312-4323-9454-E0B85BF2C169";
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (!this.Page.ClientScript.IsStartupScriptRegistered(this.Page.GetType(), _guid))
				this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), _guid,
					@"function enumEdit_OpenWindow(query, w, h, resize){
						var l = (screen.width - w) / 2;
						var t = (screen.height - h) / 2;
						
						winprops = 'height='+h+',width='+w+',top='+t+',left='+l;
						if (scroll) winprops+=',scrollbars=1';
						if (resize) 
							winprops+=',resizable=1';
						else
							winprops+=',resizable=0';
						var f = window.open(query, '_blank', winprops);
					}", true);
		}

		#region IEditControl Members

		#region Value
		public object Value
		{
			set
			{
				if (value != null)
				{
					CHelper.AddToContext(String.Format("{0}_MultiValue", FieldName), value);
					
					List<int> lst = new List<int>((int[])value);

					foreach (DataGridItem dgi in grdMain.Items)
					{
						foreach (Control control in dgi.Cells[1].Controls)
						{
							if (control is CheckBox)
							{
								CheckBox checkBox = (CheckBox)control;
								int itemId = int.Parse(dgi.Cells[0].Text);

								if (lst.Contains(itemId))
									checkBox.Checked = true;

								checkBox.TabIndex = TabIndex;
							}
						}
					}
				}
			}
			get
			{
				List<int> lst = new List<int>();

				foreach (DataGridItem dgi in grdMain.Items)
				{
					foreach (Control control in dgi.Cells[1].Controls)
					{
						if (control is CheckBox)
						{
							CheckBox checkBox = (CheckBox)control;
							if (checkBox.Checked)
							{
								lst.Add(int.Parse(dgi.Cells[0].Text));
							}
						}
					}
				}
				return lst.ToArray();
			}
		}
		#endregion

		#region ShowLabel
		public bool ShowLabel
		{
			set { }
			get { return true; }
		}
		#endregion

		#region Label
		public string Label
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region AllowNulls
		public bool AllowNulls
		{
			set
			{
				ViewState["AllowNulls"] = value;
			}
			get
			{
				if (ViewState["AllowNulls"] != null)
					return (bool)ViewState["AllowNulls"];
				else
					return false;
			}
		}
		#endregion

		#region RowCount
		public int RowCount
		{
			set
			{
				int iHeight = 20 * value + 10 * (value - 1);
				divBlock.Style.Add(HtmlTextWriterStyle.Height, iHeight.ToString() + "px");
			}
			get
			{
				Unit h = Unit.Parse(divBlock.Style[HtmlTextWriterStyle.Height]);
				return (int)((h.Value + 10) / 30);
			}
		}
		#endregion

		#region ReadOnly
		public bool ReadOnly
		{
			set
			{
				grdMain.Enabled = !value;
				if (value)
					tdEdit.Visible = false;
			}
			get
			{
				return !grdMain.Enabled;
			}
		}
		#endregion

		#region LabelWidth
		public string LabelWidth
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region BindData
		public void BindData(MetaField field)
		{
			string sTypeName = field.TypeName;
			ViewState["FieldType"] = sTypeName;
			RebuildList(sTypeName, false);

			if (!ReadOnly && field.Attributes.ContainsKey(McDataTypeAttribute.EnumEditable)
				&& (bool)field.Attributes[McDataTypeAttribute.EnumEditable])
			{
				tdEdit.Visible = true;
				string url = this.Page.ResolveUrl(String.Format("~/Apps/MetaDataBase/Pages/Public/EnumView.aspx?type={0}&btn={1}", sTypeName, Page.ClientScript.GetPostBackEventReference(btnRefresh, "")));

				btnEditItems.Attributes.Add("onclick", String.Format("enumEdit_OpenWindow(\"{0}\", 750, 500, 1)", url));
			}
			else
			{
				tdEdit.Visible = false;
			}
		}
		#endregion

		#region FieldName
		public string FieldName
		{
			get
			{
				if (ViewState["FieldName"] != null)
					return ViewState["FieldName"].ToString();
				else
					return "";
			}
			set
			{
				ViewState["FieldName"] = value;
			}
		}
		#endregion

		#region TabIndex
		public short TabIndex
		{
			get
			{
				if (ViewState["TabIndex"] != null)
					return (short)ViewState["TabIndex"];
				else
					return 0;
			}
			set
			{
				ViewState["TabIndex"] = value;
			}
		}
		#endregion
		#endregion

		#region btnRefresh_Click
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			RebuildList(ViewState["FieldType"].ToString(), true);
		}
		#endregion

		#region RebuildList
		private void RebuildList(string sFieldType, bool saveValue)
		{
			object savedValue = new object();
			
			if(saveValue)
				savedValue = Value;

			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("Name", typeof(string));
			
			foreach (MetaEnumItem item in MetaEnum.GetItems(DataContext.Current.MetaModel.RegisteredTypes[sFieldType]))
			{
				DataRow row = dt.NewRow();
				row["Id"] = item.Handle;
				row["Name"] = "&nbsp;" + CHelper.GetResFileString(item.Name);
				dt.Rows.Add(row);
			}

			grdMain.DataSource = dt.DefaultView;
			grdMain.DataBind();

			if(saveValue)
				Value = savedValue;
		}
		#endregion

		#region vldCustom_ServerValidate
		protected void vldCustom_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (!AllowNulls)
			{
				if (((int[])Value).Length <= 0)
					args.IsValid = false;
			}
		}
		#endregion
	}
}