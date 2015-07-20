<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.HelpDeskManagement.ColumnTemplates.Priority_Grid_IncidentList" %>
<script type="text/C#" runat="server">
	protected string GetIncPriority(object priorityId, object priorityName)
	{
		if (priorityId == DBNull.Value || priorityName == DBNull.Value)
			return String.Empty;
		int pid = (int)priorityId;
		string color = "PriorityNormal.gif";
		if (pid < 100) color = "PriorityLow.gif";
		if (pid > 500 && pid < 800) color = "PriorityHigh.gif";
		if (pid >= 800 && pid < 1000) color = "PriorityVeryHigh.gif";
		if (pid >= 1000) color = "PriorityUrgent.gif";
		string name = priorityName.ToString();
		return String.Format("<img width='16' height='16' src='{2}/{0}' alt='{1}' title='{1}'/>", color, name, this.Page.ResolveClientUrl("~/layouts/images/icons"));
	}
</script>
<%# GetIncPriority(Eval("PriorityId"), Eval("PriorityName"))%>