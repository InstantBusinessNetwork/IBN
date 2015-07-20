<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Role.Manage.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Security.Primitives.Role_Manage" %>
<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td class="ibn-label" valign="top" style="width:120px;">
			<asp:Literal ID="Literal3" Text="<%$Resources : IbnFramework.Security, PermittedTypes%>" runat="server"></asp:Literal>:
		</td>
		<td>
			<div style="border: 2px inset;">
				<asp:DataGrid id="MainGrid" runat="server" allowsorting="False" allowpaging="False" width="100%" autogeneratecolumns="False" borderwidth="0" gridlines="None" cellpadding="0" CellSpacing="0" ShowFooter="False" ShowHeader="False">
					<columns>
						<asp:boundcolumn visible="false" datafield="Id"></asp:boundcolumn>
						<asp:templatecolumn itemstyle-cssclass="text">
							<itemtemplate>
								<asp:checkbox runat="server" id="checkItem" Text='<%# Eval("Name")%>' Checked=<%# Eval("Checked")%>></asp:checkbox>
							</itemtemplate>
						</asp:templatecolumn>
					</columns>
				</asp:DataGrid>
			</div>
		</td>
		<td style="width:20px;"></td>
	</tr>
</table>