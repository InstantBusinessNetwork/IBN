<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.HelpDeskManagement.ColumnTemplates.Text_Grid_Incident_StateName_IncidentList" %>
<script type="text/C#" runat="server">
	protected string GetStateName(object stateName, object isOverdue)
	{
		if (stateName == DBNull.Value || isOverdue == DBNull.Value)
			return String.Empty;
		else
		{
			if (!(bool)isOverdue)
				return stateName.ToString();
			else
				return String.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}, {1}",
					stateName,
					GetGlobalResourceObject("IbnFramework.Incident", "Overdue"));
		}
	}
</script>
<%# GetStateName(Eval("StateName"), Eval("IsOverdue"))%>