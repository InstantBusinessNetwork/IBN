<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Common.Modules.LatestVisitors" Codebehind="LatestVisitors.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~\Modules\BlockHeader.ascx" %>
<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
<div style="height:240px;overflow:-moz-scrollbars-vertical;overflow-y:scroll;">
	<asp:DataGrid id="dgMain" runat="server" cellpadding="0" CellSpacing="0" gridlines="Horizontal" borderwidth="0" autogeneratecolumns="False" allowsorting="False" Width="433">
		<ItemStyle Height="21px"></ItemStyle>
		<Columns>
			<asp:TemplateColumn ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2" HeaderStyle-Width="120px">
				<HeaderTemplate>
					<%#LocRM.GetString("Date")%>
				</HeaderTemplate>
				<ItemTemplate>
					<%# ((DateTime)DataBinder.Eval(Container.DataItem, "Dt")).ToString("g")%>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2">
				<HeaderTemplate>
					<%#LocRM.GetString("Visitor")%>
				</HeaderTemplate>
				<ItemTemplate>
					<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL((int)DataBinder.Eval(Container.DataItem, "UserId"))%>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>
</div>