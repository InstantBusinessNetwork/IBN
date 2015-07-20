<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\Modules\BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.ResellerEdit" Codebehind="ResellerEdit.ascx.cs" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px;">
	<tr>
		<td><ibn:blockheader id="secH" Title="Add/Edit Reseller" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="PADDING-RIGHT: 8px; PADDING-LEFT: 8px; PADDING-TOP: 8px">
			<!-- body -->
			<table class="ms-propertysheet text" cellspacing="4" cellpadding="4" border="0">
				<tr>
					<td><b>Title:</b></td>
					<td align="left" width="250px">
						<asp:TextBox ID="tbTitle" Runat="server" Width="90%"></asp:TextBox></td>
				</tr>
				<tr>
					<td><b>Contact Name:</b></td>
					<td align="left">
						<asp:TextBox ID="tbContactName" Runat="server" Width="90%"></asp:TextBox></td>
				</tr>
				<tr>
					<td><b>Contact Phone:</b></td>
					<td align="left">
						<asp:TextBox ID="tbContactPhone" Runat="server" Width="90%"></asp:TextBox></td>
				</tr>
				<tr>
					<td><b>Contact E-mail:</b></td>
					<td align="left">
						<asp:TextBox ID="tbContactEmail" Runat="server" Width="90%"></asp:TextBox></td>
				</tr>
				<tr>
					<td><b>Commission Percentage (%):</b></td>
					<td align="left">
						<asp:TextBox ID="tbCommPerc" Runat="server" Width="90%"></asp:TextBox>
						<asp:RequiredFieldValidator ID="rfPerc" CssClass="text" ControlToValidate="tbCommPerc" Runat=server ErrorMessage="*"></asp:RequiredFieldValidator>
						<asp:RangeValidator ID="rvPerc" Runat=server CssClass="text" ControlToValidate="tbCommPerc" ErrorMessage="*" MaximumValue="100" MinimumValue="0" Type="Integer"></asp:RangeValidator>
					</td>
				</tr>
				<tr>
					<td></td>
					<td align="right">
						<asp:button id="btnSave" CssClass="text" Runat="server" Text="Save" Width="80px" onclick="btnSave_Click"></asp:button>&nbsp;&nbsp;
						<input class="text" id="btnCancel" style="WIDTH: 80px" onclick="window.parent.history.back()" type="button" value='Cancel' name="btnCancel">
					</td>
				</tr>
			</table>
			<!-- end body --></td>
	</tr>
</table>
