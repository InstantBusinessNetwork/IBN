<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\Modules\BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.ResellerView" Codebehind="ResellerView.ascx.cs" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px;">
	<TBODY>
		<tr>
			<td><ibn:blockheader id="secH" title="View Reseller" runat="server"></ibn:blockheader></td>
		</tr>
		<tr>
			<td style="PADDING-RIGHT: 8px; PADDING-LEFT: 8px; PADDING-TOP: 8px">
				<!-- body -->
				<table class="ms-propertysheet text" cellspacing="4" cellpadding="4" border="0" style="WIDTH: 777px; HEIGHT: 128px">
					<TBODY>
						<tr>
							<td width=130><b>Title:</b></td>
							<td align="left"><asp:label id="lbTitle" Runat="server" CssClass="text"></asp:label></td>
						</tr>
						<tr>
							<td width=130><b>Contact Name:</b></td>
							<td align="left"><asp:label id="lbContactName" Runat="server" Width="100%" CssClass="text"></asp:label></td>
						</tr>
						<tr>
							<td width=130><b>Contact Phone:</b></td>
							<td align="left"><asp:label id="lbContactPhone" Runat="server" Width="100%" CssClass="text"></asp:label></td>
						</tr>
						<tr>
							<td width=130><b>Contact E-mail:</b></td>
							<td align="left"><asp:label id="lbContactEmail" Runat="server" Width="100%" CssClass="text"></asp:label></td>
						</tr>
						<tr>
							<td width=130><STRONG>Unique ID:</STRONG></td>
							<td align=left>
								<asp:label id=lblGuid CssClass="text" Runat="server" Width="100%"></asp:label></td>
						</tr>
						<tr>
							<td width=130><STRONG>Commission Percentage:</STRONG></td>
							<td align=left>
								<asp:label id="lblPercentage" CssClass="text" Runat="server" Width="100%"></asp:label></td>
						</tr>
					</TBODY>
				</table>
				<!-- end body --></td>
		</tr>
	</TBODY>
</table>
</tr></TBODY></table></tr></TBODY></table>
