<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Directory.UserWebApps" Codebehind="UserWebApps.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" ControlName="../Directory/Modules/UserStubs.ascx"></ibn:PageTemplate>