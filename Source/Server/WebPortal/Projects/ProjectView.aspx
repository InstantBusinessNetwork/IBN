<%@ Reference Control="~/Modules/PageTemplateNext.ascx" %>
<%@ Register TagName="PageTemplate" TagPrefix="ibn" src="~/Modules/PageTemplateNext.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Projects.ProjectView" Codebehind="ProjectView.aspx.cs" %>
<ibn:pagetemplate id="pT" enctype="multipart/form-data" title="" runat="server" ControlName="../Projects/Modules/ProjectView2.ascx"></ibn:pagetemplate>