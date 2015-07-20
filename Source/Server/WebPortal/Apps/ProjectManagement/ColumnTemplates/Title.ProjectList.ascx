<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.ProjectManagement.ColumnTemplates.Title_ProjectList" %>
<%# (Eval("ProjectId") == DBNull.Value) ? "&nbsp;" : Mediachase.UI.Web.Util.CommonHelper.GetProjectStatus
(
	this.Page,
	(int)Eval("ProjectId"),
	Eval("Title").ToString(),
	(int)Eval("StatusId")
)
%>