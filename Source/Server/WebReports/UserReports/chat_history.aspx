<%@ Reference Control="~/UserReports/GlobalModules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.UserReports.chat_history" Codebehind="chat_history.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="GlobalModules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="Reports: Message History" ControlName="../../UserReports/Modules/ChatHistory.ascx"></ibn:PageTemplate>
