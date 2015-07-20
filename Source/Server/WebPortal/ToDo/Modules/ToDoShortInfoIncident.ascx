<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.ToDoShortInfoIncident" Codebehind="ToDoShortInfoIncident.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px">
	<tr>
		<td>
			<ibn:blockheader id="tbView" runat="server"></ibn:blockheader></td>
	</tr>
</table>
<table class="ibn-propertysheet ibn-stylebox-light" cellspacing="7" cellpadding="0" width="100%" border="0">
	<tr>
		<td class="ibn-value" valign="top">
			<asp:Label id="lblTitle" runat="server"></asp:Label>
		</td>
		<td valign="top" align="right">
			<asp:Label id="lblStatus" runat="server" Font-Bold="True"></asp:Label>
		</td>
	</tr>
	<tr>
		<td class="ibn-legend-greyblack" valign="top">
			<asp:Label id="lblSeverity" runat="server"></asp:Label>
		</td>
		<td valign="top" align="right">
			<asp:Label id="lblPriority" runat="server"></asp:Label>
		</td>
	</tr>
	<tr>
		<td class="ibn-legend-greyblack" colspan="2" valign="top">
			<asp:Label id="lblType" runat="server"></asp:Label>
		</td>
	</tr>
	<tr>
		<td class="ibn-description" colspan="2" valign="top">
			<asp:Label id="lblDescription" runat="server"></asp:Label>
		</td>
	</tr>
</table>
