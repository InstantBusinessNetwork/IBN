<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.LatestIncidents" Codebehind="LatestIncidents.ascx.cs" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="Migrated_tbIncidents" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td><asp:datagrid id="dgIncidents" runat="server" allowsorting="False" cellpadding="1" gridlines="None" CellSpacing="3" borderwidth="0px" autogeneratecolumns="False" width="100%">
				<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:BoundColumn Visible="False" DataField="IncidentId"></asp:BoundColumn>
					<asp:TemplateColumn SortExpression="Title" HeaderText="Title">
						<ItemStyle Width="60%"></ItemStyle>
						<ItemTemplate>
							<a href='../Incidents/IncidentView.aspx?IncidentId=<%# DataBinder.Eval(Container.DataItem, "IncidentId")%>'>
								<%# DataBinder.Eval(Container.DataItem, "Title")%>
							</a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="PriorityName" HeaderText="Priority">
						<ItemStyle></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "PriorityName")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="CreationDate" SortExpression="CreationDate" DataFormatString="{0:d}" HeaderText="Creation Date"></asp:BoundColumn>
				</Columns>
				<PagerStyle Visible="False"></PagerStyle>
			</asp:datagrid></td>
	</tr>
</table>
<asp:LinkButton id="lbViewAll" runat="server" Visible="False" onclick="lbViewAll_Click"></asp:LinkButton>
