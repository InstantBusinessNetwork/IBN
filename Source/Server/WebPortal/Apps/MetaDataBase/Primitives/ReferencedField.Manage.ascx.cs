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

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class ReferencedField_Manage : System.Web.UI.UserControl, IManageControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members
		public string GetDefaultValue(bool AllowNulls)
		{
			return String.Format("{0},{1}", ddlClass.SelectedValue, ddlField.SelectedValue);
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				string[] str = ddlClass.SelectedValue.Split(',');
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				Attr.Add(McDataTypeAttribute.ReferencedFieldMetaClassName, str[0]);
				Attr.Add(McDataTypeAttribute.ReferencedFieldMetaFieldName, ddlField.SelectedValue);
				Attr.Add(McDataTypeAttribute.ReferencedFieldReferenceName, str[1]);
				return Attr;
			}
		}

		public void BindData(MetaClass mc, string FieldType)
		{
			if (ddlClass.Items.Count == 0 && mc != null)
			{
				foreach (MetaField field in mc.GetReferences())
				{
					string RefClassName = field.Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
					string RefFieldName = field.Name;
					MetaClass refmc = MetaDataWrapper.GetMetaClassByName(RefClassName);

					ddlClass.Items.Add(
						new ListItem(
							String.Format("{0} ({1})", CHelper.GetResFileString(refmc.FriendlyName), CHelper.GetResFileString(field.FriendlyName)),
							String.Format("{0},{1}", RefClassName, RefFieldName))
						);
				}
				BindFields();
			}
		}

		public void BindData(MetaField mf)
		{
			string RefClassName = mf.Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
			string ReferenceName = mf.Attributes[McDataTypeAttribute.ReferencedFieldReferenceName].ToString();
			string RefFieldName = mf.Attributes[McDataTypeAttribute.ReferencedFieldMetaFieldName].ToString();

			MetaClass mc = MetaDataWrapper.GetMetaClassByName(RefClassName);

			ddlClass.Items.Add(
				new ListItem(
					String.Format(CultureInfo.InvariantCulture, "{0} ({1})", CHelper.GetResFileString(mf.Owner.Fields[ReferenceName].FriendlyName), CHelper.GetResFileString(mc.FriendlyName)),
					String.Format(CultureInfo.InvariantCulture, "{0},{1}", RefClassName, ReferenceName))
				);
			ddlClass.Enabled = false;

			ddlField.Items.Add(new ListItem(CHelper.GetResFileString(mc.Fields[RefFieldName].FriendlyName)));
			ddlField.Enabled = false;
		}
		#endregion

		#region ddlClass_SelectedIndexChanged
		protected void ddlClass_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindFields();
		}
		#endregion

		#region BindFields
		private void BindFields()
		{
			ddlField.Items.Clear();

			string[] class_field = ddlClass.SelectedValue.Split(',');
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(class_field[0]);
			foreach (MetaField field in mc.Fields)
			{
				McDataType type = field.GetMetaType().McDataType;
				McDataType originalType = field.GetOriginalMetaType().McDataType;
				// forbid reference and referenced fields
				if (type != McDataType.Reference && type != McDataType.ReferencedField && originalType != McDataType.ReferencedField)
				{
					ddlField.Items.Add(new ListItem(String.Format("{0} ({1})", CHelper.GetResFileString(field.FriendlyName), field.Name), field.Name));
				}
			}
		}
		#endregion
	}
}