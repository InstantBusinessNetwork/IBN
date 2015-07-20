<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.Shell.ColumnTemplates.Document.Title" %>
<%# 
	(Eval("DocumentId") != DBNull.Value)
	?
	Mediachase.UI.Web.Apps.Shell.Modules.ActiveDocuments.GetDocumentTitle(Eval("Title").ToString(),
					(int)Eval("DocumentId"),
					(int)Eval("ReasonId"))
	:""
%>