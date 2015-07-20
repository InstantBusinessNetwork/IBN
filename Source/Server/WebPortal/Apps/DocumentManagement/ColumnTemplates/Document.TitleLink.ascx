<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.DocumentManagement.ColumnTemplates.Document_TitleLink" %>
<script type="text/C#" runat="server">
	protected string GetDocTitleLink(object id, object stateId, object stateName, object title)
	{
		if (id == DBNull.Value || stateId == DBNull.Value)
			return String.Empty;
		else
			return String.Format("<a href='{2}?DocumentId={0}'><img alt='' width='16' height='16' src='{3}' align='absmiddle' border='0' />&nbsp;{1}</a>", 
				id.ToString(), title.ToString(),
				ResolveUrl("~/Documents/DocumentView.aspx"),
				ResolveUrl("~/layouts/images/icons/document.gif"));
	}
</script>
<%# GetDocTitleLink(Eval("DocumentId"), Eval("StatusId"), Eval("StatusName"), Eval("Title"))%>