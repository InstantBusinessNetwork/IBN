<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.Reference_ViewEntity_All_ProjectId"%>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.IBN.Business" %>
<%@ Import Namespace="Mediachase.UI.Web.Util" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null && DataItem[FieldName] != null)
		{
			retVal = CommonHelper.GetObjectLinkAndTitle((int)ObjectTypes.Project, (int)(PrimaryKeyId)DataItem[FieldName], this.Page);
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
