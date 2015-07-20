<%@ Reference Control="~/Modules/PageTemplateExternal.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.External.ExternalToDo" Codebehind="ExternalToDo.aspx.cs" %>
<%@Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/PageTemplateExternal.ascx" %>
<ibn:pagetemplate id="pT" enctype="multipart/form-data" title="" runat="server" ControlName="../ToDo/Modules/ToDoView2.ascx"></ibn:pagetemplate>
