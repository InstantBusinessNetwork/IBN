<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Link_View" %>
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
			MetaLink mlink = (MetaLink)DataItem.Properties[FieldName].Value;
			string linkField = DataItem.Properties[FieldName].GetMetaType().LinkInformation.TitleField;
			object obj_value = DataItem.Properties[linkField].Value;
			if (obj_value != null)
				retVal = obj_value.ToString();
			if (mlink.IsEmpty)
				return retVal;
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(mlink.MetaClassName);
			MetaObject obj = new MetaObject(mc, mlink.MetaObjectId.Value);
			string sUrl = ResolveClientUrl(CHelper.GetLinkObjectView_Edit(mc.Name, obj.PrimaryKeyId.ToString()));
			retVal = String.Format("<a href='{0}'>{1}</a>", sUrl, retVal);			
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