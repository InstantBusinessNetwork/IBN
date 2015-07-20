<%@ Reference Control="~/UserReports/GlobalModules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.UserReports.User_History" Codebehind="User_History.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="GlobalModules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="Reports: Message History" ControlName="../../UserReports/Modules/MessageHistory.ascx"></ibn:PageTemplate>
