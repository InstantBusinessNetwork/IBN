<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.ToDoShortInfoTask" Codebehind="ToDoShortInfoTask.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px">
	<tr>
		<td>
			<ibn:blockheader id="tbView" title="" runat="server"></ibn:blockheader></td>
	</tr>
</table>
<table class="ibn-propertysheet ibn-stylebox-light"  cellspacing="7" cellpadding="0" width="100%" border="0">
	<tr>
		<td class="ibn-value" valign="top">
			<asp:Label id="lblTitle" runat="server"></asp:Label>
		</td>
		<td valign="top" align="right">
			<asp:label id="lblState" runat="server" Font-Bold="True"></asp:label>
		</td>
	</tr>
	<tr>
		<td class="ibn-legend-greyblack" valign="top">
			<asp:label id="lblTimeline" runat="server"></asp:label>
		</td>
		<td class="ibn-value" valign="top" align="right">
			<asp:Label id="lblPriority" runat="server"></asp:Label>
		</td>
	</tr>
	<tr>
		<td colspan="2" class="ibn-description" valign="top">
			<asp:Label id="lblDescription" runat="server"></asp:Label>
		</td>
	</tr>
</table>
		