<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.SecureGroups" CodeBehind="SecureGroups.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\PageViewMenu.ascx" %>

<script type="text/javascript">
//<![CDATA[
function DeleteUser(UserID, SGroupID)
{
	var w = 450;
	var h = 250;
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
	var f = window.open('UserDelete.aspx?BtnID=<%=btnRefresh.ClientID %>&UserID=' + UserID + '&SGroupID='+SGroupID, "UserDelete", winprops);
	return false;
}
function DeleteGroup()
{
	if (confirm('<%=LocRM.GetString("DelConfGroup") %>'))
	<%= Page.ClientScript.GetPostBackEventReference(btnDeleteGroup, "")%>
}
	//]]>
</script>

<table class="ibn-stylebox" cellspacing="0" cellpadding="0" style="width:100%">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td style="padding-top: 5px; padding-bottom: 10px">
			<asp:DataGrid ID="dgGroupsUsers" runat="server" ItemStyle-Height="21" AllowSorting="False" AllowPaging="False" Width="100%" AutoGenerateColumns="False" BorderWidth="0" GridLines="Horizontal" CellPadding="1">
				<Columns>
					<asp:BoundColumn Visible="false" DataField="ObjectId"></asp:BoundColumn>
					<asp:BoundColumn Visible="false" DataField="Type"></asp:BoundColumn>
					<asp:BoundColumn DataField="Title" HeaderText="Group/User" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn DataField="Email" HeaderText="Email" HeaderStyle-Width="40%" ItemStyle-Width="40%" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn DataField="ActionView" HeaderStyle-Width="25px" ItemStyle-Width="25px" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:BoundColumn>
					<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" DataField="ActionEdit" HeaderStyle-Width="25px" ItemStyle-Width="25px" ItemStyle-CssClass="ibn-vb2"></asp:BoundColumn>
					<asp:TemplateColumn ItemStyle-CssClass="ibn-vh2" HeaderStyle-Width="25px">
						<ItemStyle Width="25px" HorizontalAlign="left" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:ImageButton ID="ibDelete" runat="server" BorderWidth="0" Width="16" Height="16" ImageUrl="../../layouts/images/DELETE.GIF" CommandName="Delete" CausesValidation="False" title='<%#LocRM.GetString("tDelete")%>'></asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn Visible="false" DataField="HasChildren"></asp:BoundColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:Label runat="server" ID="PartnerLabel" CssClass="text ibn-error"></asp:Label>
<asp:Button ID="btnRefresh" runat="server" Style="display: none" OnClick="btnRefresh_Click"></asp:Button>
<asp:LinkButton ID="btnDeleteGroup" runat="server" Visible="False" OnClick="DeleteGroup"></asp:LinkButton>