<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EditControls.IntValue" Codebehind="IntValue.ascx.cs" %>
<asp:TextBox CssClass="text" id="txtValue" runat="server" MaxLength="9" Wrap="False" Width="100"></asp:TextBox>
<asp:RequiredFieldValidator id="txtValueRFValidator" runat="server" ErrorMessage="*" ControlToValidate="txtValue"	Display="Dynamic"></asp:RequiredFieldValidator>
<asp:RangeValidator id="txtValueRangeValidator" runat="server" ErrorMessage="Wrong Range" ControlToValidate="txtValue" Display="Dynamic" Type="Integer" MaximumValue="999999999" MinimumValue="-999999999"></asp:RangeValidator>
