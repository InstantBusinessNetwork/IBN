<%@ Reference Control="~/Modules/PageTemplateNext.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="~/Modules/PageTemplateNext.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Tasks.TaskEdit" Codebehind="TaskEdit.aspx.cs" %>
<ibn:PageTemplate runat="server" id="pT" Title="Task Edit" ControlName="../Tasks/Modules/TaskEdit.ascx" enctype="multipart/form-data"></ibn:PageTemplate>
