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
	public partial class DropDownBoolean_Manage : System.Web.UI.UserControl, IManageControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members
		public string GetDefaultValue(bool AllowNulls)
		{
			return ddlDefaultValue.SelectedValue;
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				Attr.Add(McDataTypeAttribute.BooleanTrueText, txtYesText.Text);
				Attr.Add(McDataTypeAttribute.BooleanFalseText, txtNoText.Text);
				return Attr;
			}
		}

		public void BindData(MetaClass mc, string FieldType)
		{
			if (ddlDefaultValue.Items.Count == 0)
			{
				ddlDefaultValue.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.GlobalFieldManageControls", "Yes").ToString(), "1"));
				ddlDefaultValue.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.GlobalFieldManageControls", "No").ToString(), "0"));
			}
		}

		public void BindData(MetaField mf)
		{
			if (mf.Attributes.ContainsKey(McDataTypeAttribute.BooleanTrueText))
				txtYesText.Text = mf.Attributes[McDataTypeAttribute.BooleanTrueText].ToString();
			if (mf.Attributes.ContainsKey(McDataTypeAttribute.BooleanFalseText))
				txtNoText.Text = mf.Attributes[McDataTypeAttribute.BooleanFalseText].ToString();

			if (ddlDefaultValue.Items.Count == 0)
			{
				ddlDefaultValue.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.GlobalFieldManageControls", "Yes").ToString(), "1"));
				ddlDefaultValue.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.GlobalFieldManageControls", "No").ToString(), "0"));
			}

			try
			{
				if ((bool)DefaultValue.Evaluate(mf))
					CHelper.SafeSelect(ddlDefaultValue, "1");
				else
					CHelper.SafeSelect(ddlDefaultValue, "0");
			}
			catch
			{
			}
		}
		#endregion
	}
}