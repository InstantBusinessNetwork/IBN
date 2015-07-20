<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.HelpDeskManagement.ColumnTemplates.Responsible_IncidentList" %>
<%@ Import Namespace="Mediachase.IBN.Business" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%# GetResponsibleLink(Eval("IncidentId"), Eval("CurrentResponsibleId")) %>
	
<script type="text/C#" runat="server">
protected string GetResponsibleLink(object incidentObj, object currentResponsibleObj)
{
	string retval = "";
	if (currentResponsibleObj != DBNull.Value)
	{
		int currentResponsibleId = (int)currentResponsibleObj;
		if (currentResponsibleId > 0)
		{
			retval = Mediachase.UI.Web.Util.CommonHelper.GetUserStatus(currentResponsibleId);
		}
		else if (currentResponsibleId == -2)	// no responsible
		{
			retval = CHelper.GetIconText(
				CHelper.GetResFileString("{IbnFramework.Incident:tRespNotSet}"),
				this.Page.ResolveUrl("~/Layouts/Images/not_set.png"));
		}
		else if (currentResponsibleId == -3)	// group
		{
			System.Collections.Generic.Dictionary<string, string> dic = new System.Collections.Generic.Dictionary<string, string>();
			dic.Add("IncidentId", incidentObj.ToString());
			CommandParameters cp = new CommandParameters("MC_HDM_GroupResponsibilityList", dic);
			string cmd = CommandManager.GetCurrent(this.Page).AddCommand("Issue", "", "IssueListNew", cp);

			retval = CHelper.GetIconLink(
				String.Concat("javascript:", cmd),
				CHelper.GetResFileString("{IbnFramework.Incident:tRespGroup}"),
				this.Page.ResolveUrl("~/Layouts/Images/waiting.gif"),
				"");
 		}
		else if (currentResponsibleId == -4)	// all denied
		{
			retval = CHelper.GetIconText(
				CHelper.GetResFileString("{IbnFramework.Incident:tRespGroup}"),
				this.Page.ResolveUrl("~/Layouts/Images/red_denied.gif"));
		}
	}
	return retval;
}
</script>