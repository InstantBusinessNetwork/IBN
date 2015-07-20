<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.HelpDeskManagement.ColumnTemplates.Integer_Grid_Incident_IncidentCode_IncidentList" %>
<script type="text/C#" runat="server">
	protected string GetTicket(object IssId, object IssBoxId, object sIdentifier)
	{
		if (IssId == DBNull.Value || IssBoxId == DBNull.Value)
			return "";
		if (sIdentifier != DBNull.Value && sIdentifier.ToString() != "")
			return sIdentifier.ToString();
		else
		{
			Mediachase.IBN.Business.EMail.IncidentBox box = Mediachase.IBN.Business.EMail.IncidentBox.Load((int)IssBoxId);
			return Mediachase.IBN.Business.EMail.TicketUidUtil.Create(box.IdentifierMask, (int)IssId);
		}
	}
</script>
<%# GetTicket(Eval("IncidentId"), Eval("IncidentBoxId"), Eval("Identifier"))%>