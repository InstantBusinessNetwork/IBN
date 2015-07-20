<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.ProjectManagement.ColumnTemplates.ProjectId_ProjectList" %>
<%# Eval("ProjectId") == DBNull.Value || Eval("ProjectCode") == DBNull.Value 
	? "" 
	: "#" + CHelper.GetProjectNum((int)Eval("ProjectId"), Eval("ProjectCode").ToString())%>