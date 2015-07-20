<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.BackReference_ViewEntity" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<script language="c#" runat="server">
	
	protected string GetValue(EntityObject DataItem, string FieldName)
    {
        string retVal = "";

		if (DataItem != null && DataItem.Properties[FieldName] != null)
        {
			string sReferencedClass = MetaDataWrapper.GetMetaFieldByName(DataItem.MetaClassName, FieldName).Attributes.GetValue<string>(McDataTypeAttribute.BackReferenceMetaClassName);
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(sReferencedClass);

			EntityObject[] objList = (EntityObject[])DataItem[FieldName];
			if (mc.Attributes[MetaClassAttribute.IsBridge] != null && (bool)mc.Attributes[MetaClassAttribute.IsBridge])
			{
				MetaField refField = null;
				foreach (MetaField field in mc.Fields)
				{
					if (field.GetMetaType().McDataType == McDataType.Reference
						&& (bool)field.Attributes[MetaClassAttribute.IsSystem]
						//&& field.Attributes[MetaClassAttribute.IsSystem].ToString() == "1"
						&& field.Name != MetaDataWrapper.GetMetaFieldByName(DataItem.MetaClassName, FieldName).Attributes.GetValue<string>(McDataTypeAttribute.BackReferenceMetaFieldName)
						)
					{
						refField = field;
						break;
					}
				}

				if (refField == null)
					throw new Exception("Referenced field is not found");

				string refClassName = refField.Attributes.GetValue<string>(McDataTypeAttribute.ReferencedFieldMetaClassName);
				mc = MetaDataWrapper.GetMetaClassByName(refClassName);
				foreach (EntityObject obj in objList)
				{
					EntityObject refObj = BusinessManager.Load(refClassName, obj.PrimaryKeyId.Value);
					string sUrl = ResolveClientUrl(CHelper.GetLinkEntityView_Edit(sReferencedClass, obj.PrimaryKeyId.ToString()));
					if (retVal.Length > 0) retVal += "<br />";
					retVal += String.Format("<a href='{0}'>{1}</a>", sUrl, refObj[mc.TitleFieldName].ToString());
				}
			}
			else
			{
				foreach (EntityObject obj in objList)
				{
					if (obj[mc.TitleFieldName].ToString() == string.Empty) continue;
					if (retVal != String.Empty)
						retVal += "<br />";

					string sUrl = ResolveClientUrl(CHelper.GetLinkEntityView_Edit(sReferencedClass, obj.PrimaryKeyId.ToString()));
					retVal += String.Format("<a href='{0}'>{1}</a>", sUrl, obj[mc.TitleFieldName].ToString());
				}
			}
        }
        return retVal;
    }
</script>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td valign='top'>
		  <%# GetValue(DataItem, FieldName) %>
		</td>
	</tr>
</table>