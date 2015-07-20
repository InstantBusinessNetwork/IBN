<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ResourceUtilProjects" Codebehind="ResourceUtilProjects.ascx.cs" %>
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
								(bool)DataBinder.Eval(Container.DataItem,"IsGroup"),
								(int)DataBinder.Eval(Container.DataItem,"UserId"),
								(int)DataBinder.Eval(Container.DataItem,"ProjectId"),
								(int)DataBinder.Eval(Container.DataItem,"StatusId"),
								DataBinder.Eval(Container.DataItem,"Title").ToString())
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetRole(
								(bool)DataBinder.Eval(Container.DataItem,"IsGroup"),
								(bool)DataBinder.Eval(Container.DataItem,"IsTeamMember"),
								(bool)DataBinder.Eval(Container.DataItem,"IsSponsor"),
								(bool)DataBinder.Eval(Container.DataItem,"IsStakeHolder"),
								(bool)DataBinder.Eval(Container.DataItem,"IsManager"),
								(bool)DataBinder.Eval(Container.DataItem,"IsExecutiveManager"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetPriority(
								(bool)DataBinder.Eval(Container.DataItem,"IsGroup"),
								DataBinder.Eval(Container.DataItem,"PriorityName").ToString())
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetStatus(
								(bool)DataBinder.Eval(Container.DataItem,"IsGroup"),
								DataBinder.Eval(Container.DataItem,"StatusName").ToString())
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetPhase(
								(bool)DataBinder.Eval(Container.DataItem,"IsGroup"),
								DataBinder.Eval(Container.DataItem,"PhaseName").ToString())
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