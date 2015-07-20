<%@ Reference Control="~/Modules/PageTemplateNext.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="~/Modules/PageTemplateNext.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Events.EventEdit" Codebehind="EventEdit.aspx.cs" %>
<ibn:PageTemplate enctype="multipart/form-data" runat="server" id="pT" Title="Event Edit" ControlName="../Events/Modules/EventEdit.ascx"></ibn:PageTemplate>
