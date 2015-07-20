<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TodoPredecessors.ascx.cs" Inherits="Mediachase.UI.Web.ToDo.Modules.TodoPredecessors" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" style="MARGIN-TOP:5px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server"/>
		</td>
	</tr>
</table>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox-light">
	<tr>
		<td>
			<asp:DataGrid Runat="server" ID="dgPredecessors" AutoGenerateColumns="False" AllowPaging="False" 
					AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" 
					Width="100%" ShowHeader="True">
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:BoundColumn Visible="False" DataField="ToDoId" ReadOnly="True"></asp:BoundColumn>
					<asp:BoundColumn Visible="False" DataField="LinkId" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2 "></ItemStyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetTaskToDoLink(
								(int)DataBinder.Eval(Container.DataItem, "ToDoId"),
								1,
								DataBinder.Eval(Container.DataItem, "Title").ToString(),
								(int)DataBinder.Eval(Container.DataItem, "StateId")
							)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="FinishDate" DataFormatString="{0:d}" ReadOnly="True" HeaderStyle-Width="150px">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Right" Width="26px" CssClass="ibn-vh-right"></HeaderStyle>
						<ItemStyle Width="26px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" imageurl="../../layouts/images/DELETE.GIF" title='<%# LocRM.GetString("Delete")%>' commandname="Delete" causesvalidation="False"></asp:imagebutton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>