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

using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class DropDownBoolean_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IEditControl Members

		#region Value
		private bool _value = false;
		public object Value
		{
			set
			{
				if (value != null)
				{
					_value = (bool)value;
					CHelper.SafeSelect(ddlValue, _value.ToString().ToLower());
				}
			}
			get
			{
				return bool.Parse(ddlValue.SelectedValue);
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
			}
			get
			{
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
			set { ddlValue.Enabled = !value; }
			get { return !ddlValue.Enabled; }
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
			ddlValue.Items.Clear();
			ddlValue.Items.Add(new ListItem(CHelper.GetResFileString(field.Attributes[McDataTypeAttribute.BooleanTrueText].ToString()), "true"));
			ddlValue.Items.Add(new ListItem(CHelper.GetResFileString(field.Attributes[McDataTypeAttribute.BooleanFalseText].ToString()), "false"));
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
	}
}