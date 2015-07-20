<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.Shell.ColumnTemplates.ToDo.Title" %>
<%# (Eval("ItemId") != System.DBNull.Value) ?
Mediachase.UI.Web.Util.CommonHelper.GetTaskToDoLink 
(
	(int)Eval("ItemId"),
	(int)Eval("IsToDo"),
	Eval("Title").ToString(),
	(int)Eval("StateId")
)
:
""
%>