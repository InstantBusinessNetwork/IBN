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
	public partial class Image_Manage : System.Web.UI.UserControl, IManageControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members
		public string GetDefaultValue(bool AllowNulls)
		{
			return null;
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();

				if (txtWidth.Text.Trim() != String.Empty)
					Attr.Add(McDataTypeAttribute.ImageWidth, int.Parse(txtWidth.Text));
				if (txtHeight.Text.Trim() != String.Empty)
					Attr.Add(McDataTypeAttribute.ImageHeight, int.Parse(txtHeight.Text));
				Attr.Add(McDataTypeAttribute.ImageShowBorder, chkShowBorder.Checked);
				Attr.Add(McDataTypeAttribute.FileNameRegexPattern, txtRegexPattern.Text);
				return Attr;
			}
		}

		public void BindData(MetaClass mc, string FieldType)
		{

		}

		public void BindData(MetaField mf)
		{
			if (mf.Attributes.ContainsKey(McDataTypeAttribute.FileNameRegexPattern))
				txtRegexPattern.Text = mf.Attributes[McDataTypeAttribute.FileNameRegexPattern].ToString();

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.ImageWidth))
				txtWidth.Text = mf.Attributes[McDataTypeAttribute.ImageWidth].ToString();
			else
				txtWidth.Text = "";

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.ImageHeight))
				txtHeight.Text = mf.Attributes[McDataTypeAttribute.ImageHeight].ToString();
			else
				txtHeight.Text = "";

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.ImageShowBorder))
				chkShowBorder.Checked = (bool)mf.Attributes[McDataTypeAttribute.ImageShowBorder];
		}
		#endregion
	}
}