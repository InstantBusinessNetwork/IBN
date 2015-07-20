<%@ Reference Page="~/Admin/EditTemplate.aspx" %>
<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Admin.EmailSignatureEdit" Codebehind="EmailSignatureEdit.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="~/Modules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" SelectedMenu="1_0_0" id="pT" Title="Projects" ControlName="~/Admin/Modules/EmailSignatureEdit.ascx"></ibn:PageTemplate>