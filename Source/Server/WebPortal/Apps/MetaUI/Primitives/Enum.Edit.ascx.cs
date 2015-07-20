using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class Enum_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

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
					CHelper.SafeSelect(ddlValue, value.ToString());
			}
			get
			{
				if (ddlValue.Items.Count > 0 && ddlValue.SelectedValue != "0")
					return int.Parse(ddlValue.SelectedValue);
				else
					return null;
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
			set { }
			get { return 1; }
		}
		#endregion

		#region ReadOnly
		public bool ReadOnly
		{
			set
			{
				ddlValue.Enabled = !value;
				if (value)
					tdEdit.Visible = false;
			}
			get
			{
				return !ddlValue.Enabled;
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
			RebuildList(sTypeName);

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
			set
			{
				ddlValue.TabIndex = value;
			}
		}
		#endregion
		#endregion

		#region btnRefresh_Click
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			RebuildList(ViewState["FieldType"].ToString());
		}
		#endregion

		#region RebuildList
		private void RebuildList(string sFieldType)
		{
			object savedValue = Value;

			ddlValue.Items.Clear();
			if (AllowNulls)
				ddlValue.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.GlobalFieldManageControls:NoValue}"), "0"));


			foreach (MetaEnumItem item in MetaEnum.GetItems(DataContext.Current.MetaModel.RegisteredTypes[sFieldType]))
			{
				string text = CHelper.GetResFileString(item.Name);
				ddlValue.Items.Add(new ListItem(text, item.Handle.ToString()));
			}

			Value = savedValue;
		}
		#endregion
	}
}