<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StickObject.ascx.cs" Inherits="Mediachase.UI.Web.Shell.Modules.StickObject" %>
<div class="ibn-navline" style="padding:5px;">
	<asp:Label runat="server" ID="ObjectLabel"></asp:Label>
</div>
<table cellpadding="0" cellspacing="0" width="100%" class="ibn-propertysheet tablePadding5">
	<tr>
		<td style="width:50%" valign="top">
			<asp:DataList runat="server" ID="PositionList" CellPadding="0" CellSpacing="0" 
				RepeatDirection="Vertical" RepeatLayout="Table" HeaderStyle-Font-Bold="true" 
				onitemcommand="PositionList_ItemCommand">
				<HeaderTemplate>
					<asp:Literal runat="server" ID="PositionHeader" Text="<%$ Resources: IbnFramework.Calendar, Pin %>"></asp:Literal>:
				</HeaderTemplate>
				<ItemTemplate>
					<asp:LinkButton runat="server" ID="PositionButton" Text='<%# Eval("PositionName") %>' CommandArgument='<%# Eval("PositionId") %>'></asp:LinkButton>
				</ItemTemplate>
			</asp:DataList>
		</td>
		<td style="width:50%" valign="top">
			<asp:DataList runat="server" ID="PriorityList" CellPadding="0" CellSpacing="0" 
				RepeatDirection="Vertical" RepeatLayout="Table" HeaderStyle-Font-Bold="true" 
				onitemcommand="PriorityList_ItemCommand">
				<HeaderTemplate>
					<asp:Literal runat="server" ID="PriorityHeader" Text="<%$ Resources: IbnFramework.Global, _mc_Priority %>"></asp:Literal>:
				</HeaderTemplate>
				<ItemTemplate>
					<span class="IconAndText">
					<%# Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)Eval("PriorityId"), this.Page)%>
					<asp:LinkButton runat="server" ID="PriorityButton" CommandArgument='<%# Eval("PriorityId") %>'><%# Eval("PriorityName") %></asp:LinkButton>
					</span>
				</ItemTemplate>
			</asp:DataList>
		</td>
	</tr>
</table>
