<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.Shell.ColumnTemplates.ToDo.Resources" %>
<%# (Eval("ItemId") != System.DBNull.Value) ?
	Mediachase.UI.Web.Apps.Shell.Modules.OutActivities.GetResources
	(
		(int)Eval("CompletionTypeId"),
		(int)Eval("IsToDo"),
		(int)Eval("ItemId"),
		(int)Eval("PercentCompleted")					
	)
	: "" 
%>