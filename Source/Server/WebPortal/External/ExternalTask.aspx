<%@ Reference Control="~/Modules/PageTemplateExternal.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.External.ExternalTask" Codebehind="ExternalTask.aspx.cs" %>
<%@ Register TagName="PageTemplate" TagPrefix="ibn" src="../Modules/PageTemplateExternal.ascx" %>
<ibn:pagetemplate id="pT" enctype="multipart/form-data" title="" runat="server" ControlName="../Tasks/Modules/TaskView2.ascx"></ibn:pagetemplate>
