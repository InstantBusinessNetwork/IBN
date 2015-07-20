<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.HelpDeskManagement.ColumnTemplates.Text_Grid_PendingMessage_Subject" %>
<%# GetSubjectLink(Eval("PendingMessageId").ToString(), Eval("Subject").ToString()) %>
<script type="text/C#" runat="server">
	protected string GetSubjectLink(string sId, string sSubject)
	{
		string retVal = String.Empty;
		if (String.IsNullOrEmpty(sId))
			return retVal;
		if (sSubject == String.Empty || sSubject == "&nbsp;")
			sSubject = GetGlobalResourceObject("IbnFramework.Incident", "tNosubject").ToString();
		retVal = String.Format("<a href=\"javascript:OpenMessage({0})\">{1}</a>&nbsp;", sId, sSubject);
		return retVal;
	}
</script>