<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Text.Edit.DocumentType.Name.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.DocumentManagement.Primitives.Text_Edit_DocumentType_Name" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
	<colgroup>
		<col width="65px" />
		<col />
		<col />
	</colgroup>
	<tr>
		<td align="right" style="width:65px;">
			Document_
		</td>
		<td>
			<asp:TextBox id="txtValue" runat="server" CssClass="text" Wrap="False" Width="99%"></asp:TextBox>
			<asp:CustomValidator id="uniqValue_Required" OnServerValidate="uniqValue_Required_ServerValidate" runat="server" ValidateEmptyText="true" ControlToValidate="txtValue" Display="Dynamic"></asp:CustomValidator>
		</td>
		<td style="width:20px;">
			<asp:RequiredFieldValidator id="vldValue_Required" runat="server" ErrorMessage="*" ControlToValidate="txtValue"	Display="Dynamic"></asp:RequiredFieldValidator>
		</td>
	</tr>
</table>
