<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectGeneralSponsors" Codebehind="ProjectGeneralSponsors.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
	<tr>
		<td>
			<asp:DataGrid Runat="server" ID="dgMembers" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="3" borderwidth="0px" Width="100%" ShowHeader="False" EnableViewState="False">
				<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
				<HeaderStyle CssClass="text"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="PrincipalId" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus( (int)DataBinder.Eval(Container.DataItem, "PrincipalId"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
