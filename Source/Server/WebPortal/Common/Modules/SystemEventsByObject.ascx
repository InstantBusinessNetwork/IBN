<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Common.Modules.SystemEventsByObject" Codebehind="SystemEventsByObject.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~\Modules\BlockHeader.ascx" %>
<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
<div style="height:445px; overflow-y:scroll; overflow:-moz-scrollbars-vertical;"> <!-- -->
	<asp:DataGrid id="dgUpdates" runat="server" cellpadding="1" gridlines="Horizontal" borderwidth="0" autogeneratecolumns="False" allowsorting="False" Width="730">
		<ItemStyle Height="21px"></ItemStyle>
		<Columns>
			<asp:TemplateColumn ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2" HeaderStyle-Width="120px">
				<HeaderTemplate>
					<%#LocRM.GetString("Updated")%>
				</HeaderTemplate>
				<ItemTemplate>
					<%# ((DateTime)DataBinder.Eval(Container.DataItem, "Dt")).ToString("g")%>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2">
				<HeaderTemplate>
					<%#LocRM.GetString("Action")%>
				</HeaderTemplate>
				<ItemTemplate>
					<%# Mediachase.IBN.Business.SystemEvents.GetSystemEventName(DataBinder.Eval(Container.DataItem,"SystemEventTitle").ToString()) %>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2">
				<HeaderTemplate>
					<%#LocRM.GetString("Info")%>
				</HeaderTemplate>
				<ItemTemplate>
					<%# DataBinder.Eval(Container.DataItem,"RelObjectTitle") %>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2" HeaderStyle-Width="140px">
				<HeaderTemplate>
					<%#LocRM.GetString("UpdatedBy")%>
				</HeaderTemplate>
				<ItemTemplate>
					<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL((int)DataBinder.Eval(Container.DataItem, "UserId"))%>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>
</div>