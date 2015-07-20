<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.SystemNotificationForObject" Codebehind="SystemNotificationForObject.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="MARGIN-TOP:0px">
	<tr>
		<td><ibn:blockheader id="secHeader" title="" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td class="ibn-navline ibn-alternating text" style="PADDING-RIGHT:14px;PADDING-LEFT:14px;PADDING-BOTTOM:14px;PADDING-TOP:14px">
			<asp:Label Runat="server" ID="lblObjectType" CssClass="ibn-label"></asp:Label>: 
			<asp:Label Runat="server" ID="lnkObject"></asp:Label>
		</td>
	</tr>
	<tr>
		<td style="padding:7">
			<asp:DataGrid Runat="server" ID="grdMain" AllowSorting="False" AllowPaging="False" 
				Width="100%" AutoGenerateColumns="False" BorderWidth="0" CellPadding="0" CellSpacing="0" CssClass="text">
				<Columns>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
