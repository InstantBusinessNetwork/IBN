<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Events.Modules.EventShortInfo" Codebehind="EventShortInfo.ascx.cs" %>
<asp:Panel ID="apShared" Runat="server" CssClass="ibn-propertysheet ibn-navline ibn-alternating" style="PADDING-RIGHT:5px; PADDING-LEFT:5px; PADDING-BOTTOM:5px; PADDING-TOP:5px"><IMG height="16" src="../Layouts/images/caution.gif" width="16" align="absMiddle" border="0">
	&nbsp;<%=LocRM.GetString("SharedEntry") %> 
	<asp:Label id="lblEntryOwner" Runat="server"></asp:Label>
</asp:Panel>

<table class="ibn-propertysheet" width="100%" border="0" cellpadding="5" cellspacing="0" style="margin-bottom:10;">
	<tr>
		<td class="ibn-label" style="padding-left:10" width="130"><%=LocRM.GetString("Title") %>:</td>
		<td class="ibn-value">
			<asp:Label id="lblTitle" runat="server"></asp:Label>
			<span class="ibn-legend-greyblack">(<asp:Label id="lblType" runat="server"></asp:Label>)</span>
		</td>
		<td align="right">
			<asp:label id="lblState" runat="server" Font-Bold="True"></asp:label>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" style="padding-left:10"><%=LocRM2.GetString("timeline") %>:</td>
		<td class="ibn-legend-greyblack">
			<asp:Label id="lblStartDate" runat="server"></asp:Label>&nbsp;-&nbsp;<asp:Label id="lblEndDate" runat="server"></asp:Label>&nbsp;<asp:Label ID="lblTimeZone" runat="server"></asp:Label>
		</td>
		<td align="right">
			<asp:label id="lblPriority" runat="server"></asp:label>
		</td>
	</tr>
	<tr>
	<td colspan="3" class="ibn-description" style="padding-left:10" align="left">
		<asp:label id="lblDescription" runat="server"></asp:label>
	</td>
</tr>
</table>
