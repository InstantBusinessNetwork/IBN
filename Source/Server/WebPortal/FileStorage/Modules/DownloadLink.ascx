<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DownloadLink.ascx.cs" Inherits="Mediachase.UI.Web.FileStorage.Modules.DownloadLink" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td>
			<ibn:BlockHeader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td style="padding: 15px;" align="center">
			<asp:Label ID="lblDownloadLink" runat="server"></asp:Label>
		</td>
	</tr>
</table>