<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.LatestToDo" Codebehind="LatestToDo.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="tbToDo" title="Latest ToDo" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<asp:datagrid id="dgToDo" runat="server" GridLines="None" AutoGenerateColumns="False" AllowSorting="False" cellpadding="3" CellSpacing="3" borderwidth="0px" Width="100%">
				<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn HeaderText="Title">
						<ItemTemplate>
							<%# GetToDoStatus(
									DataBinder.Eval(Container.DataItem, "FinishDate"),
									(bool)DataBinder.Eval(Container.DataItem, "IsCompleted"),
									(int)DataBinder.Eval(Container.DataItem, "ToDoId"),
								(string)DataBinder.Eval(Container.DataItem, "Title")) 
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Completed" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="30">
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "PercentCompleted") %>
							%
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="PriorityName" HeaderText="Priority"></asp:BoundColumn>
					<asp:TemplateColumn HeaderText="Start Date">
						<ItemTemplate>
							<%# FormatDate(DataBinder.Eval(Container.DataItem, "FinishDate")) %>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid></td>
	</tr>
</table>
<asp:LinkButton id="lbViewAll" runat="server" Visible="False" onclick="lbViewAll_Click"></asp:LinkButton>
