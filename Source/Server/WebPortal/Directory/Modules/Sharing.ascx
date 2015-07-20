<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.Sharing" CodeBehind="Sharing.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>

<script type="text/javascript">
	//<![CDATA[
	function ModifySharing(UserID) {
		var w = 580;
		var h = 350;
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l;
		var f = window.open('../Directory/SharingEditor.aspx?BtnID=<%=btnRefresh.ClientID %>&UserID=' + UserID, "ModifySharing", winprops);
	}
	//]]>
</script>

<div style="margin-top: 5px">
	<ibn:BlockHeader ID="Migrated_secHeader" runat="server"></ibn:BlockHeader>
</div>
<table class="ibn-stylebox-light" cellspacing="0" cellpadding="0" style="width:100%">
	<tr>
		<td>
			<asp:DataGrid ID="dgSharing" ShowHeader="False" Width="100%" BorderWidth="0px" CellSpacing="3" GridLines="None" CellPadding="3" AllowSorting="False" AllowPaging="False" AutoGenerateColumns="False" runat="server" EnableViewState="False">
				<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
				<HeaderStyle CssClass="text"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="UserId" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "UserId"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText='Name' ItemStyle-Width="50">
						<ItemTemplate>
							<%# GetLevel( (int)DataBinder.Eval(Container.DataItem, "Level"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid><asp:Button ID="btnRefresh" runat="server" Text="Button" Style="display: none" OnClick="btnRefresh_Click"></asp:Button>
		</td>
	</tr>
</table>
