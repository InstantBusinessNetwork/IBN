<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.BlockControl" Codebehind="BlockControl.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="BlockHeaderLightWithMenu.ascx" %>
<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<asp:PlaceHolder ID="phItems" Runat="server" />
		</td>
	</tr>
</table>