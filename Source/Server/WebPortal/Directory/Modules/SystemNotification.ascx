<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.SystemNotification" CodeBehind="SystemNotification.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<div style="margin-top: 5px">
	<ibn:BlockHeader ID="secHeader" runat="server"></ibn:BlockHeader>
</div>
<table class="ibn-stylebox-light" cellspacing="0" cellpadding="0" style="width:100%">
	<tr>
		<td class="ibn-navline text" style="padding-right: 14px; padding-left: 14px; padding-bottom: 14px; padding-top: 14px">
			<span class="ibn-label"><%=LocRM.GetString("ObjectType") %>:</span>&nbsp;<asp:DropDownList ID="ddType" Width="200px" CssClass="text" runat="server" AutoPostBack="True"></asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td style="padding: 7">
			<asp:DataGrid runat="server" ID="grdMain" AllowSorting="False" AllowPaging="False"
				Width="100%" AutoGenerateColumns="False" BorderWidth="0" CellPadding="0" CellSpacing="0"
				CssClass="text">
				<Columns>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
