<%@ Control Language="C#" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.Enum_ViewEntity" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = String.Empty;

		if (DataItem != null && DataItem.Properties[FieldName] != null && 
			DataItem[FieldName] != null)
		{
			MetaField field = MetaDataWrapper.GetMetaFieldByName(DataItem.MetaClassName, FieldName);

			retVal = CHelper.GetResFileString(MetaEnum.GetFriendlyName(field.GetMetaType(), (int)DataItem[FieldName]));
		}
		return retVal;
	}
</script>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
	 	<td>
		  <%# GetValue(DataItem, FieldName) %>
		</td>
	</tr>
</table>