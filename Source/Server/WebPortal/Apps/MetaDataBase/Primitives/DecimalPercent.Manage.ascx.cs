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
using System.Globalization;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class DecimalPercent_Manage : System.Web.UI.UserControl, IManageControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members

		public void BindData(Mediachase.Ibn.Data.Meta.Management.MetaField mf)
		{
			CultureInfo invariantCulture = new CultureInfo("");
			if (mf.DefaultValue != String.Empty)
				txtDefaultValue.Text = ((Decimal)DefaultValue.Evaluate(mf)).ToString("f");
			else
				txtDefaultValue.Text = "";

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DecimalScale))
				txtMaxPointDigits.Text = ((int)mf.Attributes[McDataTypeAttribute.DecimalScale]).ToString();
			else
				txtMaxPointDigits.Text = "4";
		}

		public void BindData(Mediachase.Ibn.Data.Meta.Management.MetaClass mc, string FieldType)
		{
			Decimal defaultValue = 0.0000M;
			int maxPointDigits = 4;

			txtDefaultValue.Text = defaultValue.ToString();
			txtMaxPointDigits.Text = maxPointDigits.ToString();
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				if (!String.IsNullOrEmpty(txtMaxPointDigits.Text.Trim()))
				{
					int scale = int.Parse(txtMaxPointDigits.Text);
					Attr.Add(McDataTypeAttribute.DecimalScale, scale);
					Attr.Add(McDataTypeAttribute.DecimalPrecision, (scale+4));
				}

				Attr.Add(McDataTypeAttribute.DecimalMinValue, 0);
				Attr.Add(McDataTypeAttribute.DecimalMaxValue, 100);

				return Attr;
			}
		}

		public string GetDefaultValue(bool AllowNulls)
		{
			if (txtDefaultValue.Text.Trim() == String.Empty)
				return String.Empty;
			CultureInfo invariantCulture = new CultureInfo("");
			return Decimal.Parse(txtDefaultValue.Text).ToString(invariantCulture);
		}

		#endregion
	}
}