<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ActivitiesByPhase" Codebehind="ActivitiesByPhase.ascx.cs" %>
<table class="text" cellspacing="0" cellpadding="0" border="0" width="100%">
	<tr>
		<td style="PADDING-TOP: 5px; PADDING-BOTTOM:5px" class="ibn-propertysheet" valign="top">
			<asp:DataGrid EnableViewState=False id="dgProjects" runat="server" cellpadding="0" gridlines="Horizontal" CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%">
				<columns>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetTitle(
								(bool)DataBinder.Eval(Container.DataItem,"IsHeader"),
								(int)DataBinder.Eval(Container.DataItem,"ProjectId"),
								DataBinder.Eval(Container.DataItem,"PhaseName").ToString(),
								(int)DataBinder.Eval(Container.DataItem,"ProjectsCount"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetStatus(
								(bool)DataBinder.Eval(Container.DataItem,"IsHeader"),
								DataBinder.Eval(Container.DataItem,"StatusName").ToString(),
								(int)DataBinder.Eval(Container.DataItem,"ActiveProjectsCount"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetResources(
								(bool)DataBinder.Eval(Container.DataItem,"IsHeader"),
								(int)DataBinder.Eval(Container.DataItem,"Resources"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetTasksToDos(
								(bool)DataBinder.Eval(Container.DataItem,"IsHeader"),
								(int)DataBinder.Eval(Container.DataItem,"TasksTodos"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetIssues(
								(bool)DataBinder.Eval(Container.DataItem,"IsHeader"),
								(int)DataBinder.Eval(Container.DataItem,"Issues"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
			<span id="spanLbl" runat="server" style="padding-left:20px;">
				<asp:Label CssClass="ibn-alerttext" ID="lblNoItems" Runat=server></asp:Label>
			</span>
		</td>
	</tr>
</table>