<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Incidents.EMailPendingMessages" Codebehind="EMailPendingMessages.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="Pending Messages" ControlName="../Admin/Modules/EMailPendingMessages.ascx"></ibn:PageTemplate>