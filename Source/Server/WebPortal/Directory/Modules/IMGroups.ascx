<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.IMGroups" CodeBehind="IMGroups.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\PageViewMenu.ascx" %>

<script type="text/javascript">
//<![CDATA[
function DeleteUser(UserID)
{
	var w = 450;
	var h = 250;
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
	window.open('UserDelete.aspx?BtnID=<%=btnRefresh.ClientID %>&UserID=' + UserID, "UserDelete", winprops);
}
function DeleteGroup(GroupId)
{
	if (confirm('<%=LocRM.GetString("DelConfGroup") %>'))
	{
		document.forms[0].<%=deleteId.ClientID%>.value = GroupId;
		<%= Page.ClientScript.GetPostBackEventReference(btnDeleteGroup, "")%>
	}
}
//]]>
</script>

<table cellspacing="0" cellpadding="0" border="0" class="ibn-stylebox text" style="width:100%">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td style="padding-top: 5px; padding-bottom: 10px">
			<asp:DataGrid ID="dgUsers" runat="server" CellPadding="1" GridLines="Horizontal" BorderWidth="0" AutoGenerateColumns="False" Width="100%" AllowPaging="False" AllowSorting="False">
				<HeaderStyle></HeaderStyle>
				<Columns>
					<asp:BoundColumn Visible="false" DataField="ID"></asp:BoundColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2 IconAndText" HeaderText="Email" SortExpression="sortSize">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetName((int)DataBinder.Eval(Container.DataItem, "Type"),(int)DataBinder.Eval(Container.DataItem, "ID"), DataBinder.Eval(Container.DataItem, "GroupName").ToString()) %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderText="Email" HeaderStyle-Width="40%" ItemStyle-Width="40%" SortExpression="sortSize">
						<ItemTemplate>
							<a href='mailto:<%# DataBinder.Eval(Container.DataItem, "Email") %>'>
								<%# DataBinder.Eval(Container.DataItem, "Email") %>
							</a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle Width="26px" CssClass="ibn-vb2" HorizontalAlign="left"></ItemStyle>
						<ItemTemplate>
							<%# (int)DataBinder.Eval(Container.DataItem, "Type")==1 ?
							"<a href='../Directory/UserView.aspx?UserID="+DataBinder.Eval(Container.DataItem, "ID")+"' title='"+LocRM.GetString("tViewDetails")+"'>"+
								"<img alt='' src='../layouts/images/icon-search.GIF'/></a>"
								: ""  %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2" DataField="ActionEdit" ItemStyle-Width="26px" HeaderStyle-Width="26px"></asp:BoundColumn>
					<asp:BoundColumn ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2" DataField="ActionDelete" ItemStyle-Width="26px" HeaderStyle-Width="26px"></asp:BoundColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:Button ID="btnRefresh" runat="server" Style="display: none" OnClick="btnRefresh_Click">
</asp:Button>
<asp:LinkButton runat="server" ID="btnDeleteGroup" Visible="False" OnClick="DeleteGroup"></asp:LinkButton>
<input type="hidden" id="deleteId" runat="server" />