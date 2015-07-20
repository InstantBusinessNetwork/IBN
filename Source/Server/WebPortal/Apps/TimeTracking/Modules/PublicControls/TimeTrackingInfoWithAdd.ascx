<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeTrackingInfoWithAdd.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls.TimeTrackingInfoWithAdd" %>
<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Time" src="~/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px;">
	<tr>
		<td><ibn:BlockHeaderLight2 id="MainBlockHeader" runat="server" HeaderCssClass="ibn-toolbar-light"/></td>
	</tr>
	<tr>
		<td class="ibn-stylebox-light">
			<table class="ibn-propertysheet" cellspacing="3" cellpadding="0" width="100%" border="0">
				<tr>
					<td align="right" valign="top" style="width:40%;">
						<%=LocRM2.GetString("SpentHours") %>: &nbsp;&nbsp;
					</td>
					<td>
						<ibn:Time id="TimesheetHours" ShowTime="HM" HourSpinMaxValue="24" ViewStartDate="false" runat="server" />
						<asp:customvalidator id="TimesheetHoursValidator" runat="server" Display="Dynamic"/>
					</td>
				</tr>
				<tr>
					<td align="right">
						<%=LocRM2.GetString("Date") %>: &nbsp;&nbsp;
					</td>
					<td>
						<mc:Picker ID="dtc" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
					</td>
				</tr>
				<tr>
					<td align="right"></td>
					<td><ibn:IMButton class="text" id="UpdateButton" Runat="server" onserverclick="UpdateButton_Click"></ibn:IMButton></td>
				</tr>
			</table>
			<table cellpadding="0" cellspacing="0" width="100%" class="ibn-bottomBlock text" style="height:30px;">
				<tr>
					<td align="center" style="width:50%;" nowrap="nowrap" id="tdTaskTime" runat="server">
						<span class="ibn-legend-greyblack"><%= (EventId > 0) ? LocRM.GetString("duration") : LocRM.GetString("taskTime")%>:</span> 
						<asp:Label Runat="server" ID="TaskTimeLabel"></asp:Label>
					</td>
					<td align="center" style="width:50%;" nowrap="nowrap">
						<span class="ibn-legend-greyblack"><%= LocRM.GetString("spentTime")%>:</span> 
						<asp:Label Runat="server" ID="SpentTimeLabel"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>

