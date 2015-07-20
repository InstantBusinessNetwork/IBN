<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.HelpDeskManagement.ColumnTemplates.IssueBox_IncidentList" %>
<%# GetIssBoxLink(Eval("IncidentId"), Eval("IncidentBoxId"), Eval("IncidentBoxName"))%>
<script type="text/C#" runat="server">
	protected string GetIssBoxLink(object IssId, object IssBoxId, object IssBoxName)
	{
		Mediachase.Ibn.Web.UI.WebControls.CommandParameters cp = new Mediachase.Ibn.Web.UI.WebControls.CommandParameters();
		cp.CommandName = "MC_HDM_IssueBoxView";
		cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
		cp.AddCommandArgument("IssBoxId", IssBoxId.ToString());
		cp.AddCommandArgument("IncidentId", IssId.ToString());

		string clientScript = Mediachase.Ibn.Web.UI.WebControls.CommandManager.GetCurrent(this.Page).AddCommand("Issue", "", "IssueListNew", cp);
		clientScript = clientScript.Replace("\"", "&quot;");
		return String.Format("<a href=\"javascript:{{{0}}}\">{1}</a>",
			clientScript, IssBoxName.ToString());
	}
</script>