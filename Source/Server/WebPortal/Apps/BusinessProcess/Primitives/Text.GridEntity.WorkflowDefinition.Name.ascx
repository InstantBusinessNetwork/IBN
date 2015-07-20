<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.BusinessProcess.Primitives.Text_GridEntity_WorkflowDefinition_Name" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.UI.Web.Util" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retval = "";

		if (DataItem != null && DataItem.Properties.Contains(FieldName))
		{
			retval = DataItem.Properties[FieldName].Value.ToString();
		}
		return retval;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
