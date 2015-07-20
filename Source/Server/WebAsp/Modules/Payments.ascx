<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Payments.ascx.cs" Inherits="Mediachase.Ibn.WebAsp.Modules.Payments" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin:0px;">
	<tr>
		<td><ibn:BlockHeader id="secHeader" runat="server"></ibn:BlockHeader></td>
	</tr>
	<tr>
		<td><asp:datagrid AllowSorting="False" id="dgPayments" runat="server" width="100%" 
			autogeneratecolumns="False" borderwidth="0px" GridLines="Horizontal" CellPadding="3" 
				AllowPaging="true" PageSize="50" PagerStyle-Mode="NumericPages" 
				onpageindexchanged="dgPayments_PageIndexChanged" 
				oncancelcommand="dgPayments_CancelCommand">
				<HeaderStyle Wrap="False"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<a href="SiteView.aspx?id=<%# DataBinder.Eval(Container.DataItem, "CompanyUid")%>"><%# DataBinder.Eval(Container.DataItem, "Company_Name")%></a>
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
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# ((decimal)DataBinder.Eval(Container.DataItem, "Amount")).ToString("f") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# ((decimal)DataBinder.Eval(Container.DataItem, "Bonus")).ToString("f")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="orderNo">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="50"></ItemStyle>
						<ItemTemplate>
							<asp:ImageButton id="ibUndo" ImageUrl="~/Layouts/images/Undo.png" Runat="server" BorderWidth="0" CommandName="Cancel" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"PaymentId") %>' ToolTip="Undo"></asp:ImageButton>&nbsp;
							<asp:ImageButton id="ibDelete" ImageUrl="~/Layouts/images/delete.gif" Runat="server" BorderWidth="0" CommandName="Delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"PaymentId") %>' ToolTip="Delete"></asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Right" CssClass="text" Mode="NumericPages"></PagerStyle>
			</asp:datagrid></td>
	</tr>
</table>