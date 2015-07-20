<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Reference" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
    protected string GetValue(MetaObject DataItem, string FieldName)
    {
        string retVal = "";

        if (DataItem != null && DataItem.Properties[FieldName].Value!=null)
        {
			string sReferencedClass = DataItem.Properties[FieldName].GetMetaType().Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(sReferencedClass);

			MetaObject obj = new MetaObject(mc, (PrimaryKeyId)DataItem.Properties[FieldName].Value);

			string sUrl = ResolveClientUrl(CHelper.GetLinkObjectView_Edit(sReferencedClass, obj.PrimaryKeyId.ToString()));
			retVal = String.Format("<a href='{0}'>{1}</a>", sUrl, obj.Properties[mc.TitleFieldName].Value.ToString());
        }
        return retVal;
    }
</script>
<%# GetValue(DataItem, FieldName) %>
