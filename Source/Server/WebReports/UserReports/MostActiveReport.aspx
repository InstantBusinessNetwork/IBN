<%@ Reference Control="~/UserReports/GlobalModules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.UserReports.MostActiveReport" Codebehind="MostActiveReport.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="GlobalModules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="Reports: Most Active Group & Users" ControlName="../../UserReports/Modules/MostActiveReport.ascx"></ibn:PageTemplate>
