<%@ Reference Page="~/Admin/EditTemplate.aspx" %>
<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Admin.EmailIssueBoxTemplateEdit" Codebehind="EmailIssueBoxTemplateEdit.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="~/Modules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" SelectedMenu="1_0_0" id="pT" Title="Projects" ControlName="~/Admin/Modules/EmailIssueBoxTemplateEdit.ascx"></ibn:PageTemplate>