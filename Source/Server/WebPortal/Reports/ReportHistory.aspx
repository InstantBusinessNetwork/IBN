<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Reports.ReportHistory" Codebehind="ReportHistory.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="Reports: Groups & Users Stat" ControlName="../Reports/Modules/ReportHistory.ascx"></ibn:PageTemplate>