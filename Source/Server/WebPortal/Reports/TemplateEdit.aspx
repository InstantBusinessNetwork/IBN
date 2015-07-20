<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Reports.TemplateEdit" Codebehind="TemplateEdit.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" SelectedMenu="1_0_0" id="pT" Title="Template Edit" ControlName="../Reports/Modules/TemplateEdit.ascx"></ibn:PageTemplate>