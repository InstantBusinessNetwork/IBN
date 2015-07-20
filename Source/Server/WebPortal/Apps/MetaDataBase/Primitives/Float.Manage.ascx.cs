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
	public partial class Float_Manage : System.Web.UI.UserControl, IManageControl
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
				CultureInfo invariantCulture = new CultureInfo("");
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				if (txtMinValue.Text.Trim() != String.Empty)
					Attr.Add(McDataTypeAttribute.DoubleMinValue, Double.Parse(txtMinValue.Text));
				if (txtMaxValue.Text.Trim() != String.Empty)
					Attr.Add(McDataTypeAttribute.DoubleMaxValue, Double.Parse(txtMaxValue.Text));
				return Attr;
			}
		}

		public void BindData(MetaClass mc, string FieldType)
		{
			double minValue = -10000000000.00;
			double maxValue = 100000000000.00;
			double defaultValue = 0.00;

			txtMinValue.Text = minValue.ToString("f");
			txtMaxValue.Text = maxValue.ToString("f");
			txtDefaultValue.Text = defaultValue.ToString("f");
		}

		public void BindData(MetaField mf)
		{
			CultureInfo invariantCulture = new CultureInfo("");
			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DoubleMinValue))
				txtMinValue.Text = ((double)mf.Attributes[McDataTypeAttribute.DoubleMinValue]).ToString("f");
			else
				txtMinValue.Text = "";

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DoubleMaxValue))
				txtMaxValue.Text = ((double)mf.Attributes[McDataTypeAttribute.DoubleMaxValue]).ToString("f");
			else
				txtMaxValue.Text = "";

			if (mf.DefaultValue != String.Empty)
				txtDefaultValue.Text = ((double)DefaultValue.Evaluate(mf)).ToString("f");
			else
				txtDefaultValue.Text = "";
		}
		#endregion
	}
}