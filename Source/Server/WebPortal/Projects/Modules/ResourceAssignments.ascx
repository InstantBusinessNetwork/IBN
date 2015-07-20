<%@ Reference Control="~/Modules/Separator1.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx"%>
<%@ Register TagPrefix="ibn" TagName="sep" src="~/Modules/Separator1.ascx"%>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ResourceAssignments" Codebehind="ResourceAssignments.ascx.cs" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="MARGIN-TOP:1px; margin-bottom:7px;">
	<tr>
		<td>
			<ibn:blockheader id="tbHeader" title="" runat="server"></ibn:blockheader>
		</td>
	</tr>
	<tr>
		<td>
			
			<ibn:sep id="sep2" runat="server" title="" />
			<asp:Panel ID="Pan2" Runat="server">
				<asp:datagrid id="dgIncident" Runat="server" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False"
					AllowPaging="true" AllowSorting="False" CellSpacing="0" PageSize="5" PagerStyle-Visible="true"
					PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2"
					ShowHeader="true"  EnableViewState="False">
					<ItemStyle CssClass="ibn-propertysheet" HorizontalAlign="Right"></ItemStyle>
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
						<asp:TemplateColumn HeaderText="Title">
							<ItemTemplate>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetIncidentTitle
									(
										(string)DataBinder.Eval(Container.DataItem, "Title"),
										(int)DataBinder.Eval(Container.DataItem, "IncidentId"),
										(bool)DataBinder.Eval(Container.DataItem, "IsOverdue"),
										(int)DataBinder.Eval(Container.DataItem, "StateId"),
										(string)DataBinder.Eval(Container.DataItem, "StateName")
									)
								%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn ItemStyle-Width="180px" ItemStyle-VerticalAlign="Top">
						<ItemTemplate>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus
									(
										(int)DataBinder.Eval(Container.DataItem, "CreatorId")
									)
								%>
								</a>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="CreationDate" ItemStyle-Width="80" HeaderText="Status" ItemStyle-HorizontalAlign="left" DataFormatString="{0:d}"
							HeaderStyle-HorizontalAlign="left"></asp:BoundColumn>
						<asp:BoundColumn DataField="StateName" ItemStyle-Width="70" HeaderText="Status" ItemStyle-HorizontalAlign="left"
							HeaderStyle-HorizontalAlign="left" Visible="False"></asp:BoundColumn>
					</Columns>
				</asp:datagrid>
			</asp:Panel>
			<ibn:sep id="sep3" runat="server" title="" />
			<asp:Panel ID="Pan3" Runat="server">
				<asp:DataGrid id="dgTaskToDo" Runat="server" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False"
					AllowPaging="true" AllowSorting="False" CellSpacing="0" PageSize="5" PagerStyle-Visible="true"
					PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2"
					ShowHeader="true" EnableViewState="False" >
					<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
					<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
					<PagerStyle CssClass="text ibn-TPHeader" HorizontalAlign="Right"></PagerStyle>
					<Columns>
						<asp:BoundColumn DataField="ItemId" Visible="False"></asp:BoundColumn>
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
						<asp:BoundColumn DataField="FinishDate" ItemStyle-Width="80" DataFormatString="{0:d}"></asp:BoundColumn>
					</Columns>
				</asp:DataGrid>
			</asp:Panel>
			<ibn:sep id="sep4" runat="server" title="" />
			<asp:Panel ID="Pan4" Runat="server" >
				<asp:DataGrid id="dgAssignments" Runat="server" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False"
					AllowPaging="true" AllowSorting="False" CellSpacing="0" PageSize="5" PagerStyle-Visible="true"
					PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2"
					ShowHeader="true"  EnableViewState="False">
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
						<asp:BoundColumn DataField="ItemId" Visible="False"></asp:BoundColumn>
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
			</asp:Panel>
			<ibn:sep id="sep8" runat="server" title="" />
			<asp:Panel ID="Pan8" Runat=server>
				<asp:DataGrid id="dgIncPending" Runat="server" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False" AllowPaging="true" AllowSorting="False" CellSpacing="0" PageSize="5" PagerStyle-Visible="true" PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2" ShowHeader="true" EnableViewState="False" >
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
						<asp:HyperLinkColumn DataNavigateUrlField="IncidentId" DataNavigateUrlFormatString="~/Incidents/IncidentView.aspx?IncidentId={0}" DataTextField="Title" HeaderText=""></asp:HyperLinkColumn>
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
					</Columns>
				</asp:DataGrid>
			</asp:Panel>
			<ibn:sep id="sep7" runat="server" title="" />
			<asp:Panel ID="Pan7" Runat="server" >
				<asp:DataGrid id="dgPendingAssignments" Runat="server" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False" AllowPaging="true" AllowSorting="False" CellSpacing="0" PageSize="5" PagerStyle-Visible="true" PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2" ShowHeader="true" EnableViewState="False" >
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
						<asp:HyperLinkColumn DataNavigateUrlField="EventId" DataNavigateUrlFormatString="~/Events/EventView.aspx?EventId={0}" DataTextField="Title" HeaderText=""></asp:HyperLinkColumn>
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
						<asp:BoundColumn DataField="StartDate" HeaderText="" DataFormatString="{0:d}" ItemStyle-Width="120" HeaderStyle-Width="120"></asp:BoundColumn>
						<asp:BoundColumn DataField="FinishDate" HeaderText="" DataFormatString="{0:d}" ItemStyle-Width="120" HeaderStyle-Width="120"></asp:BoundColumn>
					</Columns>
				</asp:DataGrid>
			</asp:Panel>
			
			<ibn:sep id="sep5" runat="server" title="" />
			<asp:Panel ID="Pan5" Runat="server">
				<asp:DataGrid id="dgNotAssigned" Runat="server" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False"
					AllowPaging="true" AllowSorting="False" CellSpacing="0" PageSize="5" PagerStyle-Visible="true"
					PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2"
					ShowHeader="true" EnableViewState="False" >
					<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
					<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
					<PagerStyle CssClass="text ibn-TPHeader" HorizontalAlign="Right"></PagerStyle>
					<Columns>
						<asp:BoundColumn DataField="ItemId" Visible="False"></asp:BoundColumn>
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
						<asp:BoundColumn DataField="FinishDate" ItemStyle-Width="80" DataFormatString="{0:d}"></asp:BoundColumn>
					</Columns>
				</asp:DataGrid>
			</asp:Panel>
		</td>
	</tr>
</table>
