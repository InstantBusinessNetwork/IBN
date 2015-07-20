<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlockHeader.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BlockHeader" %>
<table cellspacing="0" cellpadding="0" class="ibn-toolbar" style="width:100%; border:0">
	<tr>
		<td class="ibn-toolbar">
			<table cellpadding="2" cellspacing="0" style="margin-left: 0px" border="0">
				<tr runat="server" id="trMain" style="height:20px">
					<td style="height:16px; white-space:nowrap">
						<asp:Label CssClass="ibn-WPTitle" Runat="server" ID="lblBlockTitle" EnableViewState="False"></asp:Label>
					</td>
					<td style="width:100%"></td>
				</tr>
			</table>
		</td>
	</tr>
</table>