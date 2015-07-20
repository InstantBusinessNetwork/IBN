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

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class Link_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region IEditControl Members

		public object Value
		{
			set
			{
				if (value != null)
					txtValue.Text = value.ToString();
			}
			get
			{
				if (AllowNulls && txtValue.Text.Trim() == String.Empty)
					return null;
				else
					return int.Parse(txtValue.Text.Trim());
			}
		}

		public bool ShowLabel
		{
			set { }
			get { return true; }
		}

		public string Label
		{
			set { }
			get { return ""; }
		}

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

		public int RowCount
		{
			set { }
			get { return 1; }
		}

		public bool ReadOnly
		{
			set
			{
				txtValue.Enabled = !value;
				if (value)
				{
					txtValue.CssClass = "text-readonly";
				}
			}
			get
			{
				return !txtValue.Enabled;
			}
		}

		public string LabelWidth
		{
			set { }
			get { return ""; }
		}

		public void BindData(Mediachase.Ibn.Data.Meta.Management.MetaField field)
		{
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