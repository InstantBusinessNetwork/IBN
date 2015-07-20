<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TariffGroupEdit.ascx.cs" Inherits="Mediachase.Ibn.WebAsp.Modules.TariffGroupEdit" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin:0px;">
	<tr>
		<td><ibn:BlockHeader id="secHeader" runat="server"></ibn:BlockHeader></td>
	</tr>
	<tr>
		<td>
			<table cellpadding="7" class="text">
				<tr>
					<td><b><%=LocRM.GetString("TariffTypeName") %>:</b></td>
					<td><asp:TextBox ID="txtName" runat="server" CssClass="text" Width="300px"></asp:TextBox>
						<asp:RequiredFieldValidator ID="rf1" runat="server" ControlToValidate="txtName" ErrorMessage="*"
						Display="Dynamic" CssClass="text"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td></td>
					<td><asp:CheckBox ID="cbIsActive" runat="server" CssClass="text" /></td>
				</tr>
				<tr>
					<td colspan="2" align="center">
						<asp:Button ID="btnSave" runat="server" CssClass="text" />&nbsp;&nbsp;
						<asp:Button ID="btnCancel" runat="server" CssClass="text" CausesValidation="false" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
