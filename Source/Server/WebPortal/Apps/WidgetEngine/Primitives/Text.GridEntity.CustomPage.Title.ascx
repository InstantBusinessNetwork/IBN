<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.WidgetEngine.Primitives.Text_GridEntity_CustomPage_Title" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retval = String.Empty;
		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			string title = CHelper.GetResFileString(DataItem[FieldName].ToString());
			
			retval = String.Format(CultureInfo.InvariantCulture,
				"<a href=\"{0}?id={1}&amp;ClassName={2}&amp;ObjectId={3}&amp;PageUid={4}\">{5}</a>",
				ResolveClientUrl("~/Apps/WidgetEngine/Pages/CustomPageDesign.aspx"),
				DataItem.PrimaryKeyId.Value.ToString(),
				Request["ClassName"],
				Request["ObjectId"],
				DataItem["Uid"],
				title);
		}
		return retval;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
