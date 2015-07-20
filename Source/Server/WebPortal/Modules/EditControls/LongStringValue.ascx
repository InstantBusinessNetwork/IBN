<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EditControls.LongStringValue" Codebehind="LongStringValue.ascx.cs" %>
<asp:TextBox id="txtValue" runat="server" Wrap="False" width="350px" cssclass="text" textmode="MultiLine" rows="5"></asp:TextBox>
<asp:RequiredFieldValidator id="txtValueRFValidator" runat="server" ErrorMessage="*" ControlToValidate="txtValue"	Display="Dynamic"></asp:RequiredFieldValidator>
