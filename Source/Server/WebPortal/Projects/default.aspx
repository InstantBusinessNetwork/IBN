<%@ Reference Control="~/Modules/PageTemplateNext.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Projects._default" Codebehind="default.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="~/Modules/PageTemplateNext.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="Projects" ControlName="../Projects/Modules/Projects.ascx"></ibn:PageTemplate>