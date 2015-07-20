<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.Administration.Primitives.Text_GridEntity_OutgoingMessageQueue_Email" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null && DataItem.Properties["Email"] != null && DataItem["Email"] != null)
		{
			retVal = String.Format("{0}", DataItem["Email"].ToString());
		}
		else if (DataItem != null && DataItem.Properties["IbnClientMessage"] != null && DataItem["IbnClientMessage"] != null)
		{
			retVal = String.Format("{0}", DataItem["IbnClientMessage"]);
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>