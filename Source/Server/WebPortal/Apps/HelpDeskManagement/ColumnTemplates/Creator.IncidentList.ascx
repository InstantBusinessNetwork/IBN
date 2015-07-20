<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.IBN.Web.UI.HelpDeskManagement.ColumnTemplates.Creator_IncidentList" %>
<script type="text/C#" runat="server">
	protected string GetIncCreatedBy(object creatorId)
	{
		if (creatorId == DBNull.Value)
			return String.Empty;
		return Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)creatorId);
	}
</script>
<%# GetIncCreatedBy(Eval("CreatorId"))%>