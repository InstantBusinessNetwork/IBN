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

using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class Date_Manage : System.Web.UI.UserControl, IManageControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members

		public string GetDefaultValue(bool AllowNulls)
		{
			string retval = String.Empty;
			if (chkCurrentDateAsDefault.Checked)
			{
				retval = "getdate()";
			}
			return retval;
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				Attr.Add(McDataTypeAttribute.DateTimeMinValue, DateTime.Parse(txtMinValue.Text));
				Attr.Add(McDataTypeAttribute.DateTimeMaxValue, DateTime.Parse(txtMaxValue.Text));
				Attr.Add(McDataTypeAttribute.DateTimeUseUserTimeZone, false);
				return Attr;
			}
		}

		public void BindData(MetaClass mc, string FieldType)
		{

		}

		public void BindData(MetaField mf)
		{
			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DateTimeMinValue))
				txtMinValue.Text = ((DateTime)mf.Attributes[McDataTypeAttribute.DateTimeMinValue]).ToString("yyyy-MM-dd");
			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DateTimeMaxValue))
				txtMaxValue.Text = ((DateTime)mf.Attributes[McDataTypeAttribute.DateTimeMaxValue]).ToString("yyyy-MM-dd");
			chkCurrentDateAsDefault.Checked = (DefaultValue.Evaluate(mf) != null);
		}
		#endregion
	}
}