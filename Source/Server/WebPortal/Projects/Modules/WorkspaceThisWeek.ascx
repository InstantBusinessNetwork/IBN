<%@ Reference Control="~/Modules/Separator1.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="sep" Src="..\..\Modules\Separator1.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.WorkspaceThisWeek" CodeBehind="WorkspaceThisWeek.ascx.cs" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top: 1px">
	<tr>
		<td>
			<ibn:BlockHeader ID="tbHeader" Title="" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<asp:DataList ID="dlWeek" runat="server" Width="100%" CssClass="ibn-propertysheet" EnableViewState="False">
				<ItemTemplate>
					<ibn:sep ID="sep1" runat="server" Title='<%# DataBinder.Eval(Container.DataItem, "DayTitle")%>' IsClickable="false" />
					<div style="padding: 0px"><%# DataBinder.Eval(Container.DataItem, "EventList")%></div>
				</ItemTemplate>
			</asp:DataList>
		</td>
	</tr>
</table>
