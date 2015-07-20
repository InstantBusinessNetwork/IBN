<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.Shell.ColumnTemplates.ToDo.CreationDate" %>
<%# (Eval("CreationDate") != DBNull.Value)
		? ((DateTime)Eval("CreationDate")).ToShortDateString()
	: ""
%>