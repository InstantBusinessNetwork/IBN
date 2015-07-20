<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.RelatedIncidents" Codebehind="RelatedIncidents.ascx.cs" %>
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
			<asp:DataGrid Runat="server" ID="dgRelatedIss" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" ShowHeader="True">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="IncidentId" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# (bool)DataBinder.Eval(Container.DataItem, "CanView") ?
							 Mediachase.UI.Web.Util.CommonHelper.GetIncidentTitle((int)DataBinder.Eval(Container.DataItem, "IncidentId")) :
							 DataBinder.Eval(Container.DataItem, "Title")
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="170px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "ManagerId"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Right" Width="26px" CssClass="ibn-vh-right"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="26px"></ItemStyle>
						<ItemTemplate>
							<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" imageurl="../../layouts/images/DELETE.GIF" commandname="Delete" causesvalidation="False"></asp:imagebutton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:Button ID="btnAddRelatedIss" Runat="server" Visible="False" />