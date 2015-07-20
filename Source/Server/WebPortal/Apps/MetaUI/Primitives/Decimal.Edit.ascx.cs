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
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Data.Meta.Management;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class Decimal_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IEditControl Members

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

		public void BindData(Mediachase.Ibn.Data.Meta.Management.MetaField field)
		{
			Decimal dbl;

			// Minimum value
			dbl = -10000000000;
			if (field.Attributes.ContainsKey(McDataTypeAttribute.DecimalMinValue))
			{
				dbl = (Decimal)field.Attributes[McDataTypeAttribute.DecimalMinValue];
			}
			vldValue_Range.MinimumValue = dbl.ToString();

			// Maximum value
			dbl = 100000000000;
			if (field.Attributes.ContainsKey(McDataTypeAttribute.DecimalMaxValue))
			{
				dbl = (Decimal)field.Attributes[McDataTypeAttribute.DecimalMaxValue];
			}
			vldValue_Range.MaximumValue = dbl.ToString();
		}

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

		#region TabIndex
		public short TabIndex
		{
			set
			{
				txtValue.TabIndex = value;
			}
		}
		#endregion

		public object Value
		{
			get
			{
				if (AllowNulls && (txtValue.Text.Trim() == string.Empty))
				{
					return null;
				}
				Decimal obj;
				if (!Decimal.TryParse(txtValue.Text.Trim(), out obj))
					return Decimal.Parse(txtValue.Text.Trim(), CultureInfo.InvariantCulture);
				return obj;
			}
			set
			{
				if (value != null)
				{
					Decimal dbl = (Decimal)value;
					txtValue.Text = dbl.ToString();
				}
			}
		}

		#endregion
	}
}