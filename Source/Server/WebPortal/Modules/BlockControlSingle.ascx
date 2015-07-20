<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlockControlSingle.ascx.cs" Inherits="Mediachase.UI.Web.Modules.BlockControlSingle" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>

<table cellpadding="0" cellspacing="7" width="100%">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
			<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td>
						<asp:PlaceHolder runat="server" ID="MainPlaceHolder"></asp:PlaceHolder>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
