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

using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class EMail_Manage : System.Web.UI.UserControl, IManageControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members
		public string GetDefaultValue(bool AllowNulls)
		{
			string retval = String.Empty;
			if (!AllowNulls)
				retval = "''";
			return retval;
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				Attr.Add(McDataTypeAttribute.StringMaxLength, int.Parse(txtMaxLen.Text));
				Attr.Add(McDataTypeAttribute.StringRegexPattern, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
				Attr.Add(McDataTypeAttribute.StringIsUnique, chkUnique.Checked);
				return Attr;
			}
		}

		public void BindData(MetaClass mc, string FieldType)
		{
			using (SkipSecurityCheckScope scope = Mediachase.Ibn.Data.Services.Security.SkipSecurityCheck())
			{
				int count = (mc == null) ? 0 : MetaObject.GetTotalCount(mc);
				if (count > 1)
				{
					chkUnique.Checked = false;
					chkUnique.Visible = false;
				}
			}
		}

		public void BindData(MetaField mf)
		{
			if (mf.Attributes.ContainsKey(McDataTypeAttribute.StringMaxLength))
				txtMaxLen.Text = mf.Attributes[McDataTypeAttribute.StringMaxLength].ToString();
			txtMaxLen.Enabled = false;

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.StringIsUnique))
				chkUnique.Checked = (bool)mf.Attributes[McDataTypeAttribute.StringIsUnique];
			chkUnique.Enabled = false;
		}
		#endregion
	}
}