<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TariffGroups.ascx.cs" Inherits="Mediachase.Ibn.WebAsp.Modules.TariffGroups" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin:0px;">
	<tr>
		<td><ibn:BlockHeader id="secHeader" runat="server"></ibn:BlockHeader></td>
	</tr>
	<tr>
		<td><asp:datagrid id="dgGroups" runat="server" width="100%" 
			autogeneratecolumns="False" borderwidth="0px" GridLines="Horizontal" CellPadding="3">
				<HeaderStyle Wrap="False"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="TypeName">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="IsActive">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width=51></ItemStyle>
						<ItemTemplate>
							<asp:ImageButton id="ibEdit" ImageUrl="~/Layouts/images/edit.gif" ImageAlign="AbsMiddle" Runat="server" BorderWidth="0" PostBackUrl='<%# "~/Pages/TariffGroupEdit.aspx?TypeId=" + DataBinder.Eval(Container.DataItem,"TypeId").ToString() %>'></asp:ImageButton>&nbsp;&nbsp;
							<asp:ImageButton id="ibDelete" ImageUrl="~/Layouts/images/delete.gif" ImageAlign="AbsMiddle" Runat="server" BorderWidth="0" CommandName="Delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"TypeId") %>' Visible='<%#(bool)Eval("CanDelete") %>'></asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid></td>
	</tr>
</table>
