<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Directory.UserView" Codebehind="UserView.aspx.cs" %>
<ibn:PageTemplate runat="server" id="pT" Title="User View" ControlName="../Directory/Modules/UserViewTabbed.ascx"></ibn:PageTemplate>