<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.SubTasks" Codebehind="SubTasks.ascx.cs" %>
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
			<asp:DataGrid Runat="server" ID="dgSubTasks" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" ShowHeader="True">
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:BoundColumn Visible="False" DataField="TaskId" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2 "></ItemStyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetTaskToDoLink(
								(int)DataBinder.Eval(Container.DataItem, "TaskId"),
								0,
								DataBinder.Eval(Container.DataItem, "Title").ToString(),
								(int)DataBinder.Eval(Container.DataItem, "StateId")
							)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="StartDate" DataFormatString="{0:d}" ReadOnly="True" HeaderStyle-Width="150px">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="FinishDate" DataFormatString="{0:d}" ReadOnly="True" HeaderStyle-Width="150px">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn HeaderStyle-Width="210px">
						<ItemStyle CssClass="ibn-vb2 "></ItemStyle>
						<ItemTemplate>
							<%#  ((int)DataBinder.Eval(Container.DataItem,"PercentCompleted")).ToString()+"%" %>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
