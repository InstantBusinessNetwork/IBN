<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.CheckboxBoolean" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<script runat="server" language="c#">
	public string GetValue()
	{
		string retval = string.Empty;
		if (DataItem != null)
		{
			MetaObjectProperty property = DataItem.Properties[FieldName];
			if (property != null && property.Value != null)
			{
				if ((bool)property.Value)
				{
					MetaField mf = property.GetMetaType();
					if (mf.IsReferencedField)
					{
						string refClassName = mf.Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
						string refFieldName = mf.Attributes[McDataTypeAttribute.ReferencedFieldMetaFieldName].ToString();
						mf = MetaDataWrapper.GetMetaFieldByName(refClassName, refFieldName);
					}
					retval = CHelper.GetResFileString(mf.Attributes[McDataTypeAttribute.BooleanLabel].ToString());
				}
			}
		}
		return retval;
	}
</script>
<%# GetValue() %>
