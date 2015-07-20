<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.IncidentResources" Codebehind="IncidentResources.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" style="MARGIN-TOP:5px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<table class="ibn-stylebox-light"  cellspacing="0" cellpadding="0" border="0" width="100%" 
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
		</td>
	</tr>
</table>
<asp:Button id="btnRefresh" runat="server" Text="Button" style="DISPLAY:none"></asp:Button>
