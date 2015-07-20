<%@ Reference Control="~/UserReports/GlobalModules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.UserReports.PortalQuickSnapshot" Codebehind="PortalQuickSnapshot.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="GlobalModules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="Reports: Portal Quick Snapshot" ControlName="../../UserReports/Modules/PortalQuickSnapshot.ascx"></ibn:PageTemplate>
