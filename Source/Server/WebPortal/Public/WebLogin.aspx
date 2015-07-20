<%@ Reference Control="~/Modules/PageTemplatePublic.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Public.WebLogin" Codebehind="WebLogin.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/PageTemplatePublic.ascx" %>
<ibn:pagetemplate runat="server" id="pT" title="Secure Portal Access" controlname="../Public/Modules/Login.ascx" SelectedMenu="1"></ibn:pagetemplate>