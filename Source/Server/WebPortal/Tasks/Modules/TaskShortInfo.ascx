<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.TaskShortInfo" Codebehind="TaskShortInfo.ascx.cs" %>
<table cellspacing="0" cellpadding="5" width="100%" border="0" class="ibn-propertysheet" style="margin-bottom:10;">
	<tr>
		<td class="ibn-label" width="130" style="padding-left:10"><%=LocRM.GetString("Title") %>:</td>
		<td class="ibn-value">
			<asp:Label id="lblTitle" runat="server"></asp:Label>
			<asp:Label id="lblSummaryMilestone" runat="server" Visible="False" CssClass="ibn-legend-greyblack"></asp:Label>
		</td>
		<td align="right">
			<asp:label id="lblState" runat="server" Font-Bold="True"></asp:label>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" style="padding-left:10"> 
			<nobr><%= LocRM2.GetString("timeline")%>:</nobr>
		</td>
		<td class="ibn-legend-greyblack">
			<asp:label id="lblTimeline" runat="server"></asp:label>
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
