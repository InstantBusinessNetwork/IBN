<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.SecureGroupsPartners" CodeBehind="SecureGroupsPartners.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" style="width:100%">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td style="padding-top: 5px; padding-bottom: 10px; padding-left: 5px">
			<asp:DataGrid ID="dgUsers" runat="server" ItemStyle-Height="21px" AllowSorting="False" AllowPaging="False" Width="100%" AutoGenerateColumns="False" BorderWidth="0" GridLines="Horizontal" CellPadding="1">
				<Columns>
					<asp:BoundColumn Visible="false" DataField="ObjectId"></asp:BoundColumn>
					<asp:BoundColumn Visible="false" DataField="Type"></asp:BoundColumn>
					<asp:BoundColumn DataField="Title" HeaderText="Group/User" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn DataField="Email" HeaderText="Email" HeaderStyle-Width="170px" ItemStyle-Width="170px" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn DataField="ActionView" HeaderStyle-Width="35px" ItemStyle-Width="35px" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
