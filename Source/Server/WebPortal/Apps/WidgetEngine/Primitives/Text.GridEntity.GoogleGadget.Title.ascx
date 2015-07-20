<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.UI.Web.Apps.WidgetEngine.Primitives.Text_GridEntity_GoogleGadget_Title" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retval = String.Empty;
		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			string title = Mediachase.Ibn.Web.UI.CHelper.GetResFileString(DataItem[FieldName].ToString());
			return title;
		}
		return retval;
	}
</script>
<%# GetValue(DataItem, FieldName) %>