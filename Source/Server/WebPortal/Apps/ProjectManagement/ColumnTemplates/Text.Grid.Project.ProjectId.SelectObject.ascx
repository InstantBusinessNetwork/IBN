<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.ProjectManagement.ColumnTemplates.Text_Grid_Project_ProjectId_SelectObject" %>
<%# Eval("ProjectId") == DBNull.Value || Eval("ProjectCode") == DBNull.Value 
	? "" 
	: "#" + CHelper.GetProjectNum((int)Eval("ProjectId"), Eval("ProjectCode").ToString())%>
