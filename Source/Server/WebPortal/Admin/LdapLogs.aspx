<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Admin.LdapLogs" Codebehind="LdapLogs.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="~/Modules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="Ldap Synchronization Logs" ControlName="~/Admin/Modules/LdapLogs.ascx"></ibn:PageTemplate>