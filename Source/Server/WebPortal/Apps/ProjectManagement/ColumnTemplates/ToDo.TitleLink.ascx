<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.ProjectManagement.ColumnTemplates.ToDo_TitleLink" %>
<script type="text/C#" runat="server">
	protected string GetToDoTitleLink(object id, object title, object stateId)
	{
		if (id == DBNull.Value || stateId == DBNull.Value)
			return String.Empty;
		else
			return Mediachase.UI.Web.Util.CommonHelper.GetToDoLink((int)id, title.ToString(), (int)stateId);
	}
</script>
<%# GetToDoTitleLink(Eval("ToDoId"), Eval("Title"), Eval("StateId"))%>