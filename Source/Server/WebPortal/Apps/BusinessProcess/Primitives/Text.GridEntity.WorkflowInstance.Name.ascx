<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.BusinessProcess.Primitives.Text_GridEntity_WorkflowInstance_Name" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.UI.Web.Util" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retval = "";

		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			string url = "~/Apps/BusinessProcess/Pages/WorkflowInstanceView.aspx";
			
			retval = String.Format(CultureInfo.InvariantCulture,
				"<a href='{0}?ObjectId={1}&OwnerName={2}&OwnerId={3}'>{4}</a>",
				ResolveClientUrl(url), 
				DataItem.PrimaryKeyId.Value,
				CommonHelper.GetFromContext("OwnerName"),
				CommonHelper.GetFromContext("OwnerId"),
				DataItem[FieldName].ToString());
		}
		return retval;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
