<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeTrackingInfo.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls.TimeTrackingInfo" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px;">
	<tr>
		<td><ibn:BlockHeader id="blockHeaderMain" runat="server" /></td>
	</tr>
	<tr>
		<td class="ibn-stylebox-light" runat="server" id="tdTotal" style="height:30px;">
			<table cellpadding="0" cellspacing="0" width="100%" class="text">
				<tr>
					<td align="center" style="width:50%;" id="tdTaskTime" runat="server">
						<span class="ibn-legend-greyblack"><%= (EventId > 0) ? LocRM.GetString("duration") : LocRM.GetString("taskTime")%>:</span> 
						<asp:Label Runat="server" ID="labelTaskTime" EnableViewState="false"></asp:Label>
					</td>
					<td align="center" style="width:50%;">
						<span class="ibn-legend-greyblack"><%= LocRM.GetString("spentTime")%>:</span> 
						<asp:Label Runat="server" ID="labelSpentTime" EnableViewState="false"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>

