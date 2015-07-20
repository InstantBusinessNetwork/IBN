<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.ToDo.ToDoList" Codebehind="ToDoList.aspx.cs" %>
<%@ Register TagName="PageTemplate" TagPrefix="ibn" src="../Modules/PageTemplateNew.ascx" %>
<ibn:pagetemplate id="pT" title="" runat="server" ControlName="../ToDo/Modules/ToDoList.ascx" />