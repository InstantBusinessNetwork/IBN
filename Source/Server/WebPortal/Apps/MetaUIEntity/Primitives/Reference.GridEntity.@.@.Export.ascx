<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.Reference_GridEntity_All_All_Export" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			MetaField field = MetaDataWrapper.GetMetaFieldByName(DataItem.MetaClassName, FieldName);
			string sReferencedClass = field.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName);

			EntityObject obj = BusinessManager.Load(sReferencedClass, (PrimaryKeyId)DataItem[FieldName]);

			MetaClass mc = MetaDataWrapper.GetMetaClassByName(sReferencedClass);
			retVal = obj.Properties[mc.TitleFieldName].Value.ToString();
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
