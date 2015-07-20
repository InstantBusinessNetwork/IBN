<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.Link_ViewEntity" %>
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
			MetaLink mlink = (MetaLink)DataItem[FieldName];
			MetaField field = MetaDataWrapper.GetMetaFieldByName(DataItem.MetaClassName, FieldName);
			string linkField = field.LinkInformation.TitleField;
			object obj_value = DataItem[linkField];
			if (obj_value != null)
				retVal = obj_value.ToString();
			if (mlink.IsEmpty)
				return retVal;
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(mlink.MetaClassName);
			MetaObject obj = new MetaObject(mc, mlink.MetaObjectId.Value);
			string sUrl = ResolveClientUrl(CHelper.GetLinkEntityView_Edit(mc.Name, obj.PrimaryKeyId.ToString()));
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