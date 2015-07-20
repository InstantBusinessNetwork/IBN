<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.TimeTracking.Primitives.AllowEdit_Grid_TimeTrackingEntry" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.IbnNext" %>
<%@ Import Namespace="Mediachase.IbnNext.TimeTracking" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Services" %>
<script language="c#" runat="server">
protected string GetValue(MetaObject DataItem)
{
	string retVal = "false";

	if (DataItem != null && DataItem.Properties["ParentBlockId"] != null)
	{
		Mediachase.Ibn.Data.PrimaryKeyId? oParentBlockId = (Mediachase.Ibn.Data.PrimaryKeyId?)DataItem.Properties["ParentBlockId"].Value;
		if (oParentBlockId != null && (int)oParentBlockId > 0)
		{
			retVal = TimeTrackingBlock.CheckUserRight((int)oParentBlockId, Security.RightWrite).ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture);
		}
	}
	
	return retVal;
}
</script>
<%# GetValue(DataItem) %>