<%@ Reference Control="~/Modules/Separator1.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.LatestMyUpdates" Codebehind="LatestMyUpdates.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="~/Modules/Separator1.ascx"%>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx"%>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-bottom:7px;">
	<tr>
		<td>
			<ibn:blockheader id="tbHeader" title="" runat="server"></ibn:blockheader>
		</td>
	</tr>
	<tr>
		<td>
			<ibn:sep id="sep2" title="" runat="server"></ibn:sep>
			<asp:panel id="Pan2" Runat="server">
				<asp:DataGrid id="dgIncidents" Runat="server" EnableViewState="False" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False" AllowPaging="true" AllowSorting="False" CellSpacing="0" PageSize="5" PagerStyle-Visible="true" PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2" ShowHeader="true">
					<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
					<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
					<PagerStyle CssClass="text ibn-TPHeader" HorizontalAlign="Right"></PagerStyle>
					<Columns>
						<asp:TemplateColumn>
							<ItemTemplate>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetIncidentTitle 
									(
										(int)DataBinder.Eval(Container.DataItem, "IncidentId")
									)
								%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="LastSavedDate" ItemStyle-Width="120" DataFormatString="{0:g}"></asp:BoundColumn>
					</Columns>
				</asp:DataGrid>
			</asp:panel>
			<ibn:sep id="sep3" title="" runat="server"></ibn:sep>
			<asp:panel id="Pan3" Runat="server">
				<asp:DataGrid id="dgDocuments" Runat="server" EnableViewState="False" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False" AllowPaging="true" AllowSorting="False" CellSpacing="0" PageSize="5" PagerStyle-Visible="true" PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2" ShowHeader="true">
					<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
					<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
					<PagerStyle CssClass="text ibn-TPHeader" HorizontalAlign="Right"></PagerStyle>
					<Columns>
						<asp:TemplateColumn>
							<ItemTemplate>
								<%# 
									String.Format(System.Globalization.CultureInfo.InvariantCulture, 
										"<a href='{0}?DocumentId={1}'> {2}</a>",
										ResolveClientUrl("~/Documents/DocumentView.aspx"),
										Eval("DocumentId"),
										Eval("Title"))
								%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="LastSavedDate" ItemStyle-Width="120" DataFormatString="{0:g}"></asp:BoundColumn>
					</Columns>
				</asp:DataGrid>
			</asp:panel>
			<ibn:sep id="sep4" title="" runat="server"></ibn:sep>
			<asp:panel id="Pan4" Runat="server">
				<asp:DataGrid id="dgEvents" Runat="server" EnableViewState="False" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False" AllowPaging="true" AllowSorting="False" CellSpacing="0" PageSize="5" PagerStyle-Visible="true" PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2" ShowHeader="true">
					<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
					<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
					<PagerStyle CssClass="text ibn-TPHeader" HorizontalAlign="Right"></PagerStyle>
					<Columns>
						<asp:TemplateColumn>
							<ItemTemplate>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetEventTitle 
									(
										(int)DataBinder.Eval(Container.DataItem, "EventId"),
										(int)DataBinder.Eval(Container.DataItem, "StateId"),
										(string)DataBinder.Eval(Container.DataItem, "Title")
									)
								%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="LastSavedDate" ItemStyle-Width="120" DataFormatString="{0:g}"></asp:BoundColumn>
					</Columns>
				</asp:DataGrid>
			</asp:panel>
			<ibn:sep id="sep5" title="" runat="server"></ibn:sep>
			<asp:panel id="Pan5" Runat="server">
				<asp:DataGrid id="dgTasks" Runat="server" EnableViewState="False" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False" AllowPaging="true" AllowSorting="False" CellSpacing="0" PageSize="5" PagerStyle-Visible="true" PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2" ShowHeader="true">
					<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
					<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
					<PagerStyle CssClass="text ibn-TPHeader" HorizontalAlign="Right"></PagerStyle>
					<Columns>
						<asp:TemplateColumn>
							<ItemTemplate>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetTaskToDoLink 
									(
										(int)DataBinder.Eval(Container.DataItem, "ItemId"),
										(int)DataBinder.Eval(Container.DataItem, "IsToDo"),
										DataBinder.Eval(Container.DataItem, "Title").ToString(),
										(int)DataBinder.Eval(Container.DataItem, "StateId")
									)
								%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="LastSavedDate" ItemStyle-Width="120" DataFormatString="{0:g}"></asp:BoundColumn>
					</Columns>
				</asp:DataGrid>
			</asp:panel>
		</td>
	</tr>
</table>
