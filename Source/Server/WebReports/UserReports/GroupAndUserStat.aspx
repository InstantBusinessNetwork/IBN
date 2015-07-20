<%@ Reference Control="~/UserReports/GlobalModules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.UserReports.GroupAndUserStat" Codebehind="GroupAndUserStat.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="GlobalModules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="Reports: Groups & Users Stat" ControlName="../../UserReports/Modules/GroupAndUserStat.ascx"></ibn:PageTemplate>
