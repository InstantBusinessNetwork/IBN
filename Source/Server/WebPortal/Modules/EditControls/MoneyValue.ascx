<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EditControls.MoneyValue" Codebehind="MoneyValue.ascx.cs" %>
<asp:TextBox id="txtValue" runat="server" Wrap="False" CssClass="text" Width="150"></asp:TextBox>
<asp:RequiredFieldValidator id="txtValueRFValidator" runat="server" ErrorMessage="*" ControlToValidate="txtValue"	Display="Dynamic"></asp:RequiredFieldValidator>
<asp:RangeValidator MinimumValue="0" MaximumValue="1000000000" Type=Currency ID="txtValueRgValidator" Runat=server ErrorMessage="*" ControlToValidate="txtValue" Display=Dynamic></asp:RangeValidator>