<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectHistory" Codebehind="ProjectHistory.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr runat=server id="trHistory">
		<td style="PADDING:8px">
			<asp:datagrid id="dgHistory" Runat="server" AutoGenerateColumns="False" AllowPaging=True PageSize=10
				cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" ShowHeader="True">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="SnapshotId"></asp:BoundColumn>
					<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="70px" DataFormatString="{0:d}" DataField="CreationDate"></asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL((int)DataBinder.Eval(Container.DataItem,"CreatorId")) %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" Width="70px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem,"StatusName") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" Width="70px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem,"PhaseName") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" Width="55px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# ((decimal)DataBinder.Eval(Container.DataItem,"TargetBudget")).ToString("f") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" Width="55px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# ((decimal)DataBinder.Eval(Container.DataItem,"EstimatedBudget")).ToString("f") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" Width="55px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# ((decimal)DataBinder.Eval(Container.DataItem,"ActualBudget")).ToString("f") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="70px" DataFormatString="{0:d}" DataField="StartDate"></asp:BoundColumn>
					<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="70px" DataFormatString="{0:d}" DataField="FinishDate"></asp:BoundColumn>
					<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="70px" DataFormatString="{0:d}" DataField="TargetFinishDate"></asp:BoundColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Right" Mode="NumericPages" CssClass="text"></PagerStyle>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:LinkButton id="lbShowTable" runat="server" Visible="False" onclick="lbShowTable_Click"></asp:LinkButton>
<asp:LinkButton id="lbHideTable" runat="server" Visible="False" onclick="lbHideTable_Click"></asp:LinkButton>
<asp:LinkButton id="lb3Items" runat="server" Visible="False" onclick="lb3Items_Click"></asp:LinkButton>