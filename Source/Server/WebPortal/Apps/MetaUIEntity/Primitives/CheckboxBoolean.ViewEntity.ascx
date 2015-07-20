<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.CheckboxBoolean_ViewEntity" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<script runat="server" language="c#">
	public string GetValue()
	{
		string retval = string.Empty;
		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			if ((bool)DataItem[FieldName])
			{
				MetaField mf = MetaDataWrapper.GetMetaFieldByName(DataItem.MetaClassName, FieldName);
				if (mf.IsReferencedField)
				{
					string refClassName = mf.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName);
					string refFieldName = mf.Attributes.GetValue<string>(McDataTypeAttribute.ReferencedFieldMetaFieldName);
					mf = MetaDataWrapper.GetMetaFieldByName(refClassName, refFieldName);
				}
				retval = CHelper.GetResFileString(mf.Attributes.GetValue<string>(McDataTypeAttribute.BooleanLabel));
			}
		}
		return retval;
	}
</script>
<%# GetValue() %>
