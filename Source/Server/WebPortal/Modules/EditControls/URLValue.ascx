<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EditControls.URLValue" Codebehind="URLValue.ascx.cs" %>
<asp:TextBox id="txtValue" runat="server" Wrap="False" CssClass="text" Width="300" MaxLength="512"></asp:TextBox>
<asp:RequiredFieldValidator id="txtValueRFValidator" runat="server" ErrorMessage="*" ControlToValidate="txtValue"	Display="Dynamic"></asp:RequiredFieldValidator>
