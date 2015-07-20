using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.ListApp.Primitives
{
	public partial class Enum_Manage_All_All_ListInfoImport : System.Web.UI.UserControl, IManageControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region IManageControl Members
		public string GetDefaultValue(bool AllowNulls)
		{
			string retVal = String.Empty;
			return retVal;
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				Attr.Add(McDataTypeAttribute.EnumEditable, chkEditable.Checked);

				//Temporary Attributes
				Attr.Add("EnumFriendlyName", txtFriendlyName.Text.Trim());

				MetaFieldType mft = DataContext.Current.MetaModel.RegisteredTypes[ViewState[this.ClientID + "_TypeName"].ToString()];
				if (mft == null)
				{
					Attr.Add("NewEnum", "1");
					Attr.Add("EnumName", txtEnumName.Text.Trim());
					Attr.Add("EnumPrivate", !chkPublic.Checked);
				}
				return Attr;
			}
		}

		public void BindData(MetaClass mc, string FieldType)
		{
			ViewState[this.ClientID + "_TypeName"] = FieldType;

			MetaFieldType mft = DataContext.Current.MetaModel.RegisteredTypes[FieldType];
			if (mft == null)
			{
				txtEnumName.Visible = true;
				txtEnumName.Attributes.Add("onblur", "SetName('" + txtEnumName.ClientID + "','" + txtFriendlyName.ClientID + "','" + vldFriendlyName_Required.ClientID + "')");
				chkPublic.Enabled = true;
				trName.Visible = true;
			}
			else
			{
				trName.Visible = false;
				txtFriendlyName.Text = mft.FriendlyName;

				if (mft.Attributes.ContainsKey(McDataTypeAttribute.EnumPrivate) &&
					mft.Attributes[McDataTypeAttribute.EnumPrivate].ToString() == mc.Name)
					chkPublic.Checked = false;
				else
					chkPublic.Checked = true;

				chkPublic.Enabled = false;
			}
		}

		public void BindData(MetaField mf)
		{
			ViewState[this.ClientID + "_TypeName"] = mf.TypeName;

			MetaFieldType mft = DataContext.Current.MetaModel.RegisteredTypes[mf.TypeName];

			trName.Visible = false;
			txtFriendlyName.Text = mft.FriendlyName;

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.EnumEditable))
				chkEditable.Checked = (bool)mf.Attributes[McDataTypeAttribute.EnumEditable];

			if (mft.Attributes.ContainsKey(McDataTypeAttribute.EnumPrivate) &&
				mft.Attributes[McDataTypeAttribute.EnumPrivate].ToString() == mf.Owner.Name)
				chkPublic.Checked = false;
			else
				chkPublic.Checked = true;

			chkPublic.Enabled = false;
		}
		#endregion
	}
}