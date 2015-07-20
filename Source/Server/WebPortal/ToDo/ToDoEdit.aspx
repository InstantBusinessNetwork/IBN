<%@ Reference Control="~/Modules/PageTemplateNext.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.ToDo.ToDoEdit" EnableEventValidation="true" Codebehind="ToDoEdit.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="~/Modules/PageTemplateNext.ascx" %>
<ibn:PageTemplate enctype="multipart/form-data" runat="server" id="pT" Title="ToDo Edit" ControlName="../ToDo/Modules/ToDoEdit.ascx"></ibn:PageTemplate>
