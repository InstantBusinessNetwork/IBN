<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.ToDoShortInfoDoc" Codebehind="ToDoShortInfoDoc.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px">
	<tr>
		<td>
			<ibn:blockheader id="tbView" runat="server"></ibn:blockheader></td>
	</tr>
</table>
<table class="ibn-propertysheet ibn-stylebox-light" cellspacing="7" cellpadding="0" width="100%" border="0">
	<tr>
		<td valign="top" class="ibn-legend-greyblack" align="right" width="120">
			<%= LocRM.GetString("Title")%>:
		</td>
		<td valign="top">
			<asp:Label id="lblTitle" runat="server"></asp:Label>
		</td>
	</tr>
	<tr>
		<td valign="top" class="ibn-legend-greyblack" align="right">
			<%= LocRM.GetString("State")%>:
		</td>
		<td valign="top">
			<asp:Label id="lblState" runat="server"></asp:Label>
		</td>
	</tr>
	<tr>
		<td valign="top" class="ibn-legend-greyblack" align="right">
			<%= LocRM.GetString("Priority")%>:
		</td>
		<td valign="top">
			<asp:Label id="lblPriority" runat="server"></asp:Label>
		</td>
	</tr>
	<tr>
		<td valign="top" class="ibn-legend-greyblack" align="right">
			<%= LocRM.GetString("LatestVersion")%>:
		</td>
		<td valign="top">
			<asp:label id="lblLatestVersion" runat="server"></asp:label>
		</td>
	</tr>
	<tr>
		<td class="ibn-description" colspan="2" align="left">
			<asp:Label id="lblDescription" runat="server"></asp:Label>
		</td>
	</tr>
</table>
