<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.RelatedProjects" Codebehind="RelatedProjects.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>

<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
	<tr>
		<td>
			<asp:DataGrid Runat="server" ID="dgRelatedPrjs" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" ShowHeader="True">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="ProjectId" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# (bool)DataBinder.Eval(Container.DataItem, "CanView") ?
							 Mediachase.UI.Web.Util.CommonHelper.GetProjectStatus((int)DataBinder.Eval(Container.DataItem, "ProjectId")) :
							 DataBinder.Eval(Container.DataItem, "Title")
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="170px"></ItemStyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "ManagerId"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Right" Width="26px" CssClass="ibn-vh-right"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="26px"></ItemStyle>
						<ItemTemplate>
							<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" imageurl="../../layouts/images/DELETE.GIF" commandname="Delete" causesvalidation="False"></asp:imagebutton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:Button ID="btnAddRelatedPrj" Runat="server" Visible="False" />