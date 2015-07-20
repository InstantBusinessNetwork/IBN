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
using System.Globalization;

namespace Mediachase.UI.Web.Apps.MetaUI.Primitives
{
	public partial class FloatPercent_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IEditControl
		public void BindData(MetaField field)
		{
			if (field.Attributes.ContainsKey(McDataTypeAttribute.DoubleMinValue))
			{
				vldValue_Range.MinimumValue = field.Attributes[McDataTypeAttribute.DoubleMinValue].ToString();
			}
			else
			{
				double dbl = 0;
				vldValue_Range.MinimumValue = dbl.ToString();
			}
			if (field.Attributes.ContainsKey(McDataTypeAttribute.DoubleMaxValue))
			{
				vldValue_Range.MaximumValue = field.Attributes[McDataTypeAttribute.DoubleMaxValue].ToString();
			}
			else
			{
				double dbl = 100.00;
				vldValue_Range.MaximumValue = dbl.ToString();
			}
		}

		// Properties
		public bool AllowNulls
		{
			get
			{
				return !vldValue_Required.Visible;
			}
			set
			{
				vldValue_Required.Visible = !value;
			}
		}
		public string Label
		{
			get
			{
				return "";
			}
			set
			{

			}
		}
		public string LabelWidth
		{
			get
			{
				return "";
			}
			set
			{

			}
		}
		public bool ReadOnly
		{
			get
			{
				return !txtValue.Enabled;
			}
			set
			{
				txtValue.Enabled = !value;
				vldValue_Required.Enabled = !value;
				if (value)
				{
					txtValue.CssClass = "text-readonly";
				}
			}
		}
		public int RowCount
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}
		public bool ShowLabel
		{
			get
			{
				return true;
			}
			set
			{

			}
		}
		public object Value
		{
			get
			{
				if (AllowNulls && (txtValue.Text.Trim() == string.Empty))
				{
					return null;
				}
				return double.Parse(txtValue.Text.Trim(), CultureInfo.InvariantCulture);
			}
			set
			{
				if (value != null)
				{
					txtValue.Text = value.ToString();
				}
			}
		}

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
				txtValue.TabIndex = value;
			}
		}
		#endregion
		#endregion
	}
}