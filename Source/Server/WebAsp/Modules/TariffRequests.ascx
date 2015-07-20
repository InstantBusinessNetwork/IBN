<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TariffRequests.ascx.cs" Inherits="Mediachase.Ibn.WebAsp.Modules.TariffRequests" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin:0px;">
	<tr>
		<td><ibn:BlockHeader id="secHeader" runat="server"></ibn:BlockHeader></td>
	</tr>
	<tr>
		<td><asp:datagrid AllowSorting="False" id="dgTariffReqs" runat="server" width="100%" 
			autogeneratecolumns="False" borderwidth="0px" GridLines="Horizontal" CellPadding="3">
				<HeaderStyle Wrap="False"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="Company_Name">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "TariffName")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Description")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "dt")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="26"></ItemStyle>
						<ItemTemplate>
							<asp:ImageButton id="ibDelete" ImageUrl="~/Layouts/images/delete.gif" ImageAlign="AbsMiddle" Runat="server" BorderWidth="0" CommandName="Delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RequestId") %>'></asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid></td>
	</tr>
</table>
