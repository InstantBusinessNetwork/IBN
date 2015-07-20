<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.LatestTasks" Codebehind="LatestTasks.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../modules/BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="tbTasks" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<asp:DataGrid Runat="server" ID="dgTasks" AutoGenerateColumns="False" AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="3" borderwidth="0px" Width="100%" ShowHeader="True">
				<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn HeaderText="Title">
						<ItemTemplate>
							<%# GetTaskStatus(
									(DateTime)DataBinder.Eval(Container.DataItem, "FinishDate"),
									(bool)DataBinder.Eval(Container.DataItem, "IsCompleted"),
									(int)DataBinder.Eval(Container.DataItem, "TaskId"),
								(string)DataBinder.Eval(Container.DataItem, "Title")) 
							%>
							</a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Completed" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="30">
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "PercentCompleted") %> %
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="PriorityName" HeaderText="Priority" ItemStyle-Width="70"></asp:BoundColumn>
					<asp:TemplateColumn HeaderText="Start Date" HeaderStyle-Width="190" ItemStyle-Width="190">
						<ItemTemplate>
							<%# ((DateTime)DataBinder.Eval(Container.DataItem, "FinishDate")).ToShortDateString() %>
							<%# ((DateTime)DataBinder.Eval(Container.DataItem, "FinishDate")).ToShortTimeString() %>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle Visible="False"></PagerStyle>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:LinkButton id="lbViewAll" runat="server" Visible="False" onclick="lbViewAll_Click"></asp:LinkButton>

