<%@ Reference Control="~/Modules/Separator1.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.WorkspacePTTI" Codebehind="WorkspacePTTI.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="..\..\Modules\Separator1.ascx"%>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx"%>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-bottom:7px;">
	<tr>
		<td><ibn:blockheader id="tbHeader" title="" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<!-- Task/ToDo -->
			<ibn:sep id="sep2" runat="server" title="" /><asp:panel id="Pan2" Runat="server">
				<asp:DataGrid id="dgToDo" ShowHeader="true" cellpadding="2" borderwidth="0px" GridLines="None"
					Width="100%" PagerStyle-HorizontalAlign="Right" PagerStyle-Visible="true" PageSize="5" CellSpacing="0"
					AllowSorting="False" AllowPaging="true" AutoGenerateColumns="False" PagerStyle-Mode="NumericPages"
					Runat="server" EnableViewState="False">
					<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
					<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
					<PagerStyle CssClass="text ibn-TPHeader" HorizontalAlign="Right"></PagerStyle>
					<Columns>
						<asp:TemplateColumn ItemStyle-Width="18">
							<ItemTemplate>
								<%# GetTaskToDoStatus (
									(int)DataBinder.Eval(Container.DataItem, "PriorityId"),
									(string)DataBinder.Eval(Container.DataItem, "PriorityName")
									)
								%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn>
							<ItemTemplate>
								<%#
									Mediachase.UI.Web.Util.CommonHelper.GetTaskToDoLink 
									(
										(int)DataBinder.Eval(Container.DataItem, "ItemId"),
										(int)DataBinder.Eval(Container.DataItem, "IsToDo"),
										DataBinder.Eval(Container.DataItem, "Title").ToString(),
										(int)DataBinder.Eval(Container.DataItem, "StateId")
										
									)
								%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn ItemStyle-Width="180px" ItemStyle-VerticalAlign="Top">
						<ItemTemplate>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus
									(
										(int)DataBinder.Eval(Container.DataItem, "ManagerId")
									)
								%>
								</a>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="FinishDate" ItemStyle-Width="80" DataFormatString="{0:d}"></asp:BoundColumn>
					</Columns>
				</asp:DataGrid>
			</asp:panel>
			<!-- End Task/ToDo -->
			</td>
	</tr>
</table>
