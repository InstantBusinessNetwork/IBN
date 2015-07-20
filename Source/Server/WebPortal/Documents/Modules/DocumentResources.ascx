<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.DocumentResources" Codebehind="DocumentResources.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<script language=javascript>
function ModifyResources(IncidentID)
{
	var obj = document.getElementById('<%=frManageResources.ClientID%>');
	if(obj!=null)
	{
		obj.style.display = "";
	}
}
</script>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px;"><tr><td>
<ibn:blockheader id="secHeader" runat="server" />
</td></tr></table>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="1" width="100%" border="0">
	<tr>
		<td>
			<asp:DataGrid Runat="server" ID="dgMembers" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="3" borderwidth="0px" Width="100%" ShowHeader="True">
				<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="PrincipalId" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemTemplate>
							<%# GetLink( (int)DataBinder.Eval(Container.DataItem, "PrincipalId"),(bool)DataBinder.Eval(Container.DataItem, "IsGroup") )%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText='Can Manage' ItemStyle-Width="60" HeaderStyle-Width="60">
						<ItemTemplate>
							<%# GetManageType( (bool)DataBinder.Eval(Container.DataItem, "CanManage") )%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn ItemStyle-Width="110px" HeaderStyle-Width="110px">
						<ItemTemplate>
							<%# GetStatus
								(
									DataBinder.Eval(Container.DataItem, "MustBeConfirmed"),
									DataBinder.Eval(Container.DataItem, "ResponsePending"),
									DataBinder.Eval(Container.DataItem, "IsConfirmed")
								)%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:Button id="btnRefresh" runat="server" Text="Button" style="DISPLAY:none"></asp:Button>
<iframe id="frManageResources" frameborder=0 scrolling=no runat=server style="padding:2px;border:1px solid black;position:absolute;top:80px;left:100px; width:650px;height:350px; z-index:255;display:none">
</iframe>