<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectGeneralTeam" Codebehind="ProjectGeneralTeam.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
	<tr>
		<td>
			<asp:DataGrid Runat="server" ID="dgMembers" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="3" borderwidth="0px" Width="100%" ShowHeader="True" EnableViewState="False">
				<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "UserId"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn itemstyle-width="70">
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
						<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Code") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn itemstyle-width="70">
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
						<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
						<ItemTemplate>
							<%#(_isManager || (Mediachase.IBN.Business.Security.CurrentUser.UserID == (int)DataBinder.Eval(Container.DataItem, "UserId"))) ? ((decimal)DataBinder.Eval(Container.DataItem, "Rate")).ToString("f") : ""%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
