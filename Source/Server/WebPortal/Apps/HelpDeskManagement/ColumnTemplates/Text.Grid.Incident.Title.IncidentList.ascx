<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.HelpDeskManagement.ColumnTemplates.Text_Grid_Incident_Title_IncidentList" %>
<script type="text/C#" runat="server">
	protected string GetIncTitleLink(object id, object isOverdue, object stateId, object stateName, object title, object newMessage)
	{
		if (id == DBNull.Value || stateId == DBNull.Value)
			return String.Empty;
		else
		{
			if (newMessage != DBNull.Value && (bool)newMessage)
				return Mediachase.UI.Web.Util.CommonHelper.GetIncidentTitle(this.Page, title.ToString(), (int)id, (bool)isOverdue, (int)stateId, stateName.ToString(), true);
			else
				return Mediachase.UI.Web.Util.CommonHelper.GetIncidentTitle(this.Page, title.ToString(), (int)id, (bool)isOverdue, (int)stateId, stateName.ToString());
		}
	}
</script>
<%# GetIncTitleLink(Eval("IncidentId"), Eval("IsOverdue"), Eval("StateId"), Eval("StateName"), Eval("Title"), Eval("IsNewMessage"))%>