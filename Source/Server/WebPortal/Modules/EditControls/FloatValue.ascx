<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EditControls.FloatValue" Codebehind="FloatValue.ascx.cs" %>
<asp:TextBox id="MetaValueCtrl" runat="server" Wrap="False" CssClass="text" Width="150"></asp:TextBox>
<asp:RequiredFieldValidator id="MetaValueRFValidator" runat="server" ErrorMessage="*" ControlToValidate="MetaValueCtrl"
	Display="Dynamic"></asp:RequiredFieldValidator>
<asp:RangeValidator MinimumValue="-100000000000" MaximumValue="100000000000" Type="Double" ID="MetaValueRgValidator"
	Runat="server" ErrorMessage="*" ControlToValidate="MetaValueCtrl" Display="Dynamic"></asp:RangeValidator>