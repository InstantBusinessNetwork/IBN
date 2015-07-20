<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.Search" CodeBehind="Search.ascx.cs" %>

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
	//]]>
</script>

<table class="ibn-stylebox text" style="width:100%" cellspacing="0" cellpadding="0">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" Title="" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td class="ibn-navline ibn-alternating text" style="padding-right: 14px; padding-left: 14px; padding-bottom: 14px; padding-top: 14px">
			<%=LocRM.GetString("SearchString") %>:
			<asp:TextBox ID="tbSerchStr" Width="120" CssClass="text" runat="server"></asp:TextBox><asp:RequiredFieldValidator ID="Requiredfieldvalidator1" CssClass="text" runat="server" ControlToValidate="tbSerchStr" Display="Static" ErrorMessage="&nbsp;*"></asp:RequiredFieldValidator>
			&nbsp;&nbsp;
			<%=LocRM.GetString("Type") %>:
			<asp:DropDownList ID="ddType" Width="95px" CssClass="text" runat="server"></asp:DropDownList>
			&nbsp;&nbsp;
			<asp:Button CssClass="text" ID="btnSearch" runat="server" OnClick="btnSearch_Click"></asp:Button>
		</td>
	</tr>
	<tr>
		<td style="padding-top: 5px" valign="top">
			<dg:DataGridExtended ID="dgGroupsUsers" runat="server" CellPadding="1" GridLines="None" BorderWidth="0" AutoGenerateColumns="False" Width="100%" AllowPaging="True" AllowSorting="False" PageSize="10" LayoutFixed="false">
				<Columns>
					<asp:BoundColumn Visible="false" DataField="ObjectId"></asp:BoundColumn>
					<asp:BoundColumn Visible="false" DataField="Type"></asp:BoundColumn>
					<asp:BoundColumn DataField="Title" HeaderText="Group/User" SortExpression="sortTitle" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn DataField="Email" HeaderText="Email" SortExpression="sortSize" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn DataField="ActionView" HeaderStyle-Width="25px" ItemStyle-Width="25px" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" DataField="ActionEdit" HeaderStyle-Width="25px" ItemStyle-Width="25px" ItemStyle-CssClass="ibn-vb2"></asp:BoundColumn>
					<asp:TemplateColumn ItemStyle-CssClass="ibn-vh2">
						<ItemStyle Width="25px" HorizontalAlign="left" CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle Width="25px"></HeaderStyle>
						<ItemTemplate>
							<asp:ImageButton ID="ibDelete" runat="server" BorderWidth="0" Width="16" Height="16" ImageUrl="../../layouts/images/DELETE.GIF" CommandName="Delete" CausesValidation="False" title='<%#LocRM.GetString("tDelete")%>'></asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn Visible="false" DataField="HasChildren"></asp:BoundColumn>
				</Columns>
			</dg:DataGridExtended>
		</td>
	</tr>
</table>
<asp:Button ID="btnRefresh" runat="server" Style="display: none" OnClick="btnRefresh_Click"></asp:Button>
