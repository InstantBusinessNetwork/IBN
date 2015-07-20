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

namespace Mediachase.UI.Web.Apps.MetaUI.Primitives
{
	public partial class Duration_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IEditControl
		public void BindData(MetaField field)
		{
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
				return ConvertFromString(txtValue.Text.Trim());
			}
			set
			{
				if (value != null)
				{
					int hours, minutes;
					int _value = Convert.ToInt32(value.ToString());
					hours = Convert.ToInt32(_value / 60);
					minutes = _value % 60;

					txtValue.Text = String.Format("{0:D2}:{1:D2}", hours, minutes);
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

		int ConvertFromString(string value)
		{
			int hours, minutes;

			value = value.Replace(" ", ":");

			hours = Convert.ToInt32(value.Split(':')[0]);
			if (value.Split(':').Length > 1)
				minutes = Convert.ToInt32(value.Split(':')[1]);
			else
				minutes = 0;

			return hours * 60 + minutes;
		}
	}
}