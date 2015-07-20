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

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class Integer_Manage : System.Web.UI.UserControl, IManageControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members

		public string GetDefaultValue(bool AllowNulls)
		{
			return txtDefaultValue.Text;
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				Attr.Add(McDataTypeAttribute.IntegerMinValue, int.Parse(txtMinValue.Text));
				Attr.Add(McDataTypeAttribute.IntegerMaxValue, int.Parse(txtMaxValue.Text));
				return Attr;
			}
		}

		public void BindData(MetaClass mc, string FieldType)
		{

		}

		public void BindData(MetaField mf)
		{
			txtMinValue.Text = mf.Attributes.GetValue<int>(McDataTypeAttribute.IntegerMinValue,int.MinValue).ToString();
			txtMaxValue.Text = mf.Attributes.GetValue<int>(McDataTypeAttribute.IntegerMaxValue,int.MaxValue).ToString();
			txtDefaultValue.Text = mf.DefaultValue;
		}
		#endregion
	}
}