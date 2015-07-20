<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.IncidentMetrics" Codebehind="IncidentMetrics.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
</table>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
	<tr>
		<td>
			<table cellpadding="5" cellspacing="0" border="0" class="text">
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("Created")%>:</td>
					<td class="ibn-value"><asp:label id="CreationDateLabel" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("ActualOpenDate")%>:</td>
					<td class="ibn-value"><asp:label id="ActualOpenDateLabel" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("ModifiedDate")%>:</td>
					<td class="ibn-value"><asp:label id="ModifiedDateLabel" runat="server"></asp:label></td>
				</tr>
				<tr id="trExpAssign" runat="server">
					<td class="ibn-label"><%=LocRM.GetString("tExpectedAssignDate")%>:</td>
					<td class="ibn-value"><asp:label id="ExpAssignDateLabel" runat="server"></asp:label></td>
				</tr>
				<tr id="trExpReply" runat="server">
					<td class="ibn-label"><%=LocRM.GetString("tExpectedResponseDate")%>:</td>
					<td class="ibn-value"><asp:label id="ExpResponseDateLabel" runat="server"></asp:label></td>
				</tr>
				<tr id="trExpResolution" runat="server">
					<td class="ibn-label"><%=LocRM.GetString("tExpectedResolveDate")%>:</td>
					<td class="ibn-value"><asp:label id="ExpResolveDateLabel" runat="server"></asp:label></td>
				</tr>
				<tr runat="server" id="trFinish">
					<td class="ibn-label"><%=LocRM.GetString("tActualFinish")%>:</td>
					<td class="ibn-value"><asp:label id="ActualFinishLabel" runat="server"></asp:label></td>
				</tr>
			</table>
		</td>
	</tr>
</table>