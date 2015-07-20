<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.Shell.ColumnTemplates.MyWork.PercentCompleted" %>
<%# 
	(Eval("PercentCompleted") != DBNull.Value)
	? Eval("PercentCompleted").ToString() + "%"
	: "" 
%>