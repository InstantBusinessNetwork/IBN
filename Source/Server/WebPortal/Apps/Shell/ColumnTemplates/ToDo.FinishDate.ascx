<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.Shell.ColumnTemplates.ToDo.FinishDate" %>
<%# (Eval("FinishDate") != DBNull.Value)
	? ((DateTime)Eval("FinishDate")).ToShortDateString()
	: ""
%>