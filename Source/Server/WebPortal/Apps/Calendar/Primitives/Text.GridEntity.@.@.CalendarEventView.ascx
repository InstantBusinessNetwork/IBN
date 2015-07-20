<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.Calendar.Primitives.Text_GridEntity_Any_Any_CalendarEventView" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retval = String.Empty;
		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			retval = DataItem[FieldName].ToString();
		}
		return retval;
	}
</script>
<%# GetValue(DataItem, FieldName) %>