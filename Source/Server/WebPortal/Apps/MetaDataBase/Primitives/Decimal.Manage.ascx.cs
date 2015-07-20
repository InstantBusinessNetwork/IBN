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
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class Decimal_Manage : System.Web.UI.UserControl, IManageControl
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

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DecimalPrecision))
				txtMaxDigits.Text = ((int)mf.Attributes[McDataTypeAttribute.DecimalPrecision]).ToString();
			else
				txtMaxDigits.Text = "18";

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DecimalScale))
				txtMaxPointDigits.Text = ((int)mf.Attributes[McDataTypeAttribute.DecimalScale]).ToString();
			else
				txtMaxPointDigits.Text = "4";

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DecimalMinValue))
				txtMinValue.Text = ((Decimal)mf.Attributes[McDataTypeAttribute.DecimalMinValue]).ToString("f");
			else
				txtMinValue.Text = "";

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DecimalMaxValue))
				txtMaxValue.Text = ((Decimal)mf.Attributes[McDataTypeAttribute.DecimalMaxValue]).ToString("f");
			else
				txtMaxValue.Text = "";
		}

		public void BindData(Mediachase.Ibn.Data.Meta.Management.MetaClass mc, string FieldType)
		{
			Decimal defaultValue = 0.0000M;
			int maxDigits = 18;
			int maxPointDigits = 4;
			double minValue = -10000000000.00;
			double maxValue = 100000000000.00;

			txtDefaultValue.Text = defaultValue.ToString();
			txtMinValue.Text = minValue.ToString("f");
			txtMaxValue.Text = maxValue.ToString("f");

			txtMaxDigits.Text = maxDigits.ToString();
			txtMaxPointDigits.Text = maxPointDigits.ToString();
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				if (txtMaxDigits.Text.Trim() != String.Empty)
					Attr.Add(McDataTypeAttribute.DecimalPrecision, Int32.Parse(txtMaxDigits.Text));
				if (txtMaxPointDigits.Text.Trim() != String.Empty)
					Attr.Add(McDataTypeAttribute.DecimalScale, Int32.Parse(txtMaxPointDigits.Text));

				if (txtMinValue.Text.Trim() != String.Empty)
					Attr.Add(McDataTypeAttribute.DecimalMinValue, Decimal.Parse(txtMinValue.Text));
				if (txtMaxValue.Text.Trim() != String.Empty)
					Attr.Add(McDataTypeAttribute.DecimalMaxValue, Decimal.Parse(txtMaxValue.Text));

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