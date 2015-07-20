<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.LdapLogView" Codebehind="LdapLogView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td class="ibn-alternating ibn-navline">
			<table cellpadding="7" cellspacing="0" border="0" class="text">
				<tr>
					<td><b><%= LocRM.GetString("tLDAPSettings")%>:</b></td>
					<td width="300px"><asp:Label ID="lblTitle" Runat="server"></asp:Label></td>
					<td><b><%= LocRM.GetString("tSynchDate")%>:</b></td>
					<td><asp:Label ID="lblSynchDate" Runat="server"></asp:Label></td>
				</tr>
			</table>
		</td>
	<tr>
		<td style="padding: 5px">
			<asp:DataGrid Runat="server" ID="dgFields" AutoGenerateColumns="False" 
				AllowPaging="False" AllowSorting="False" cellpadding="5" 
				gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" 
				ShowHeader="True">
				<Columns>
					<asp:BoundColumn DataField="UserId" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"/>
						<ItemTemplate>
							<%# GetTitle(
									DataBinder.Eval(Container.DataItem, "UserId"),
									DataBinder.Eval(Container.DataItem, "UserName"),
									DataBinder.Eval(Container.DataItem, "FieldName")
								)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"/>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "NewValue")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"/>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "OldValue")%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>