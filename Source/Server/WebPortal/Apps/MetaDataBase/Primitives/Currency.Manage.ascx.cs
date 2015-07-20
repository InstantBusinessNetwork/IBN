using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class Currency_Manage : System.Web.UI.UserControl, IManageControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members

		public string GetDefaultValue(bool AllowNulls)
		{
			if (txtDefaultValue.Text.Trim() == String.Empty)
				return String.Empty;
			CultureInfo invariantCulture = new CultureInfo("");
			return Double.Parse(txtDefaultValue.Text).ToString(invariantCulture);
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				Attr.Add(McDataTypeAttribute.CurrencyAllowNegative, chkAllowNegative.Checked);
				return Attr;
			}
		}

		public void BindData(MetaClass mc, string FieldType)
		{
			decimal defaultValue = 0;
			txtDefaultValue.Text = defaultValue.ToString("f");
		}

		public void BindData(MetaField mf)
		{
			if (mf.DefaultValue != String.Empty)
				txtDefaultValue.Text = ((decimal)DefaultValue.Evaluate(mf)).ToString("f");
			else
				txtDefaultValue.Text = "";

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.CurrencyAllowNegative))
				chkAllowNegative.Checked = (bool)mf.Attributes[McDataTypeAttribute.CurrencyAllowNegative];
		}
		#endregion
	}
}