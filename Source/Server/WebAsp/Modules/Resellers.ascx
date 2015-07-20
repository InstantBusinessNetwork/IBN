<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\Modules\BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.Resellers" Codebehind="Resellers.ascx.cs" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px;">
	<tr>
		<td><ibn:blockheader id="secHeader" title="Resellers List" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td><asp:datagrid id="dgResellers" ShowHeader="True" Width="100%" borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="3" AllowSorting="True" AllowPaging="False" AutoGenerateColumns="False" Runat="server">
				<Columns>
					<asp:BoundColumn Visible="False" HeaderText="ID" DataField="ResellerId" ReadOnly="True"></asp:BoundColumn>
					<asp:HyperLinkColumn DataNavigateUrlField="ResellerId" DataTextField="Title" DataNavigateUrlFormatString="../Pages/ResellerView.aspx?ResellerID={0}" HeaderText="Title" SortExpression="Title">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
					</asp:HyperLinkColumn>
					<asp:BoundColumn DataField="ContactName" ReadOnly="True" SortExpression="ContactName" HeaderText="Contact Name">
						<ItemStyle CssClass="ibn-vb2" Width="100"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
					</asp:BoundColumn>
					<asp:BoundColumn DataField="ContactEmail" ReadOnly="True" SortExpression="ContactEmail" HeaderText="Contact E-mail">
						<ItemStyle CssClass="ibn-vb2" Width="120"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
					</asp:BoundColumn>
					<asp:BoundColumn DataField="Guid" ReadOnly="True" SortExpression="Guid" HeaderText="Unique Id">
						<ItemStyle CssClass="ibn-vb2" Width="250"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
					</asp:BoundColumn>
					<asp:TemplateColumn HeaderText="Commission Percentage" SortExpression="CommissionPercentage">
						<HeaderStyle CssClass="ibn-vh2" Width="100px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="100px"></ItemStyle>
						<ItemTemplate>
							<%# GetPercentage((int)DataBinder.Eval(Container.DataItem,"CommissionPercentage"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Right" Width="85px" CssClass="ibn-vh-right"></HeaderStyle>
						<ItemStyle Width="85px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:HyperLink Runat="server"  NavigateUrl='<%# "../Pages/ResellerView.aspx?ResellerID=" + DataBinder.Eval(Container.DataItem, "ResellerId").ToString() %>' ID="Hyperlink1"><img alt="" src="../layouts/images/icon-search.gif" />
							</asp:HyperLink>&nbsp;
							<asp:HyperLink ImageUrl="../layouts/images/edit.gif" Runat="server" NavigateUrl='<%# "../Pages/ResellerEdit.aspx?ResellerID=" + DataBinder.Eval(Container.DataItem, "ResellerId").ToString() %>' ID="Hyperlink2"><img alt="" src="../layouts/images/icon-search.gif" />
							</asp:HyperLink>&nbsp;
							<asp:ImageButton ImageUrl="../layouts/images/delete.gif" Runat="server" CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ResellerId") %>' ID="ibDelete" Visible='<%# (bool)Eval("CanDelete") %>' style="vertical-align:middle">
							</asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid></td>
	</tr>
</table>
