<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.ListView" CodeBehind="ListView.ascx.cs" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeader.ascx" %>

<script type="text/javascript">
	//<![CDATA[
	function DeleteUser(UserID) {
		var w = 450;
		var h = 250;
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l;
		var f = window.open('UserDelete.aspx?BtnID=<%=btnRefresh.ClientID %>&UserID=' + UserID, "UserDelete", winprops);
		return false;
	}

	function ImportUsersWizard() {
		var w = 700;
		var h = 500;
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l;
		var f = window.open('../wizards/ADConvertWizard.aspx', "Wizard", winprops);
	}
	//]]>
</script>

<table class="ibn-stylebox text" cellspacing="0" cellpadding="0" style="width:100%">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" Title="" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td style="padding-top: 5px" valign="top">
			<dg:DataGridExtended ID="dgUsers" runat="server" CellPadding="1" GridLines="None" BorderWidth="0" AutoGenerateColumns="False" Width="100%" AllowPaging="True" AllowSorting="True" PageSize="10" LayoutFixed="false">
				<Columns>
					<asp:BoundColumn Visible="false" DataField="ObjectId"></asp:BoundColumn>
					<asp:BoundColumn DataField="LastName" SortExpression="sortLastName" HeaderText="Last Name" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn DataField="FirstName" SortExpression="sortFirstName" HeaderText="First Name" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn DataField="Email" SortExpression="sortEMail" HeaderText="Email" HeaderStyle-Width="140px" ItemStyle-Width="140px" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn DataField="Location" SortExpression="sortLocation" HeaderText="Location" HeaderStyle-Width="140px" ItemStyle-Width="140px" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn DataField="Phone" SortExpression="sortPhone" HeaderText="Phone" HeaderStyle-Width="140px" ItemStyle-Width="140px" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn DataField="ActionView" HeaderStyle-Width="25px" ItemStyle-Width="25px" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" DataField="ActionEdit" HeaderStyle-Width="25px" ItemStyle-Width="25px" ItemStyle-CssClass="ibn-vb2"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle Width="25px" HorizontalAlign="left" CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle Width="25px"></HeaderStyle>
						<ItemTemplate>
							<asp:ImageButton ID="ibDelete" Visible="False" runat="server" BorderWidth="0" Width="16" Height="16" ImageUrl="../../layouts/images/DELETE.GIF" CommandName="Delete" CausesValidation="False" title='<%#LocRM.GetString("tDelete")%>'></asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</dg:DataGridExtended>
		</td>
	</tr>
</table>
<asp:Button ID="btnRefresh" runat="server" Style="display: none" OnClick="btnRefresh_Click"></asp:Button>
