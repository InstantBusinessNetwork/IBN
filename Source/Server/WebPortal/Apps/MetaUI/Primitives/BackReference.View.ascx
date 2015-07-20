<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.BackReference_View" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
	
    protected string GetValue(MetaObject DataItem, string FieldName)
    {
        string retVal = "";
        
        if (DataItem != null)
        {
            string sReferencedClass = DataItem.Properties[FieldName].GetMetaType().Attributes[McDataTypeAttribute.BackReferenceMetaClassName].ToString();
            MetaClass mc = DataContext.Current.MetaModel.MetaClasses[sReferencedClass];

            MetaObject[] objList = (MetaObject[])DataItem.Properties[FieldName].Value;
			if (mc.Attributes[MetaClassAttribute.IsBridge] !=null && (bool)mc.Attributes[MetaClassAttribute.IsBridge])
			{
				MetaField refField = null;
				foreach (MetaField field in mc.Fields)
				{
					if (field.GetMetaType().McDataType == McDataType.Reference
						&& (bool)field.Attributes[MetaClassAttribute.IsSystem]
						//&& field.Attributes[MetaClassAttribute.IsSystem].ToString() == "1"
						&& field.Name != DataItem.Properties[FieldName].GetMetaType().Attributes[McDataTypeAttribute.BackReferenceMetaFieldName].ToString()
						)
					{
						refField = field;
						break;
					}
				}

				if (refField == null)
					throw new Exception("Referenced field is not found");

				string refClassName = refField.Attributes[McDataTypeAttribute.ReferencedFieldMetaClassName].ToString();


				foreach (MetaObject obj in objList)
				{
					mc = DataContext.Current.MetaModel.MetaClasses[refClassName];
					MetaObject refObj = new MetaObject(mc, obj.PrimaryKeyId.Value);
					//retVal += refObj.Properties[mc.TitleFieldName].Value.ToString();
					string sUrl = ResolveClientUrl(CHelper.GetLinkObjectView_Edit(sReferencedClass, obj.PrimaryKeyId.ToString()));
					if (retVal.Length > 0) retVal += "<br />";
					retVal += String.Format("<a href='{0}'>{1}</a>", sUrl, refObj.Properties[mc.TitleFieldName].Value.ToString());
				}
			}
			else
			{
				foreach (MetaObject obj in objList)
				{
					if (obj.Properties[mc.TitleFieldName].Value.ToString() == string.Empty) continue;
					if (retVal != String.Empty)
						retVal += "<br />";

					string sUrl = ResolveClientUrl(CHelper.GetLinkObjectView_Edit(sReferencedClass, obj.PrimaryKeyId.ToString()));
					retVal += String.Format("<a href='{0}'>{1}</a>", sUrl, obj.Properties[mc.TitleFieldName].Value.ToString());
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