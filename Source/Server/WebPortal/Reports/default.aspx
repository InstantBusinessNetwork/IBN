<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Reports._default" Codebehind="default.aspx.cs" %>
<ibn:PageTemplate runat="server" SelectedMenu="1_0_0" id="pT" Title="Reports" ControlName="../Reports/Modules/DefaultReports.ascx"></ibn:PageTemplate>
