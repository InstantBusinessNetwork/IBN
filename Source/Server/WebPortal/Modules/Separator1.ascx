<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.Separator1" Codebehind="Separator1.ascx.cs" %>
<table class="ibn-styleheader ibn-underline" cellspacing="0" cellpadding="3" width="100%" border="0">
	<tr>
		<td>
			<span id="spFont" runat="server" style="font-size: 10px" enableviewstate="false">
				<asp:LinkButton id="lbTitle" Runat="server" BorderWidth="0" EnableViewState="False" onclick="lbTitle_Click"></asp:LinkButton>
				<asp:label id="lblTitle" Runat="server" EnableViewState="False"></asp:label>
			</span>
		</td>
		<td align="right">&nbsp;
			<asp:label id="lblLinks" runat="server" EnableViewState="False"></asp:label></td>
		<td align="right" width="16"><asp:imagebutton id="ibCompact" ImageUrl="../layouts/images/minusxp.gif" runat="server" Height="16" Width="16" EnableViewState="False"></asp:imagebutton></td>
	</tr>
</table>
