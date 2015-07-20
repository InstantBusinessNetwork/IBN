<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EditControls.BooleanValue" Codebehind="BooleanValue.ascx.cs" %>
<asp:RadioButtonList CellSpacing="5" ID="rbValue" Runat="server" CssClass="text" AutoPostBack="False" RepeatColumns="2" RepeatDirection="Horizontal">
</asp:RadioButtonList>
<asp:RequiredFieldValidator id="boolValueRFValidator" runat="server" ErrorMessage="*" ControlToValidate="rbValue"	Display="Dynamic"></asp:RequiredFieldValidator>