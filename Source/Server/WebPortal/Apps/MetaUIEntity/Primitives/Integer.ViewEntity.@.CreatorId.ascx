<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.Integer_ViewEntity_All_CreatorId" %>
<%@ Import Namespace="Mediachase.UI.Web.Util" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			retVal = CommonHelper.GetUserStatusUL((int)DataItem[FieldName]);
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
