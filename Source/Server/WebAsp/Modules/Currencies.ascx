<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Currencies.ascx.cs" Inherits="Mediachase.Ibn.WebAsp.Modules.Currencies" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin:0px;">
	<tr>
		<td><ibn:BlockHeader id="secHeader" runat="server"></ibn:BlockHeader></td>
	</tr>
	<tr>
		<td><asp:datagrid AllowSorting="False" id="dgCurrencies" runat="server" width="100%" 
			autogeneratecolumns="False" borderwidth="0px" GridLines="Horizontal" CellPadding="3">
				<HeaderStyle Wrap="False"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="CurrencyName">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="Symbol">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width=51></ItemStyle>
						<ItemTemplate>
							<asp:ImageButton id="ibEdit" ImageUrl="~/Layouts/images/edit.gif" ImageAlign="AbsMiddle" Runat="server" BorderWidth="0" PostBackUrl='<%# "~/Pages/CurrencyEdit.aspx?CurrencyId=" + DataBinder.Eval(Container.DataItem,"CurrencyId").ToString() %>'></asp:ImageButton>&nbsp;&nbsp;
							<asp:ImageButton id="ibDelete" ImageUrl="~/Layouts/images/delete.gif" ImageAlign="AbsMiddle" Runat="server" BorderWidth="0" CommandName="Delete" Visible='<%#DataBinder.Eval(Container.DataItem,"CanDelete") %>' CommandArgument='<%#DataBinder.Eval(Container.DataItem,"CurrencyId") %>'></asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid></td>
	</tr>
</table>
