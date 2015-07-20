<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PickerControl.ascx.cs" Inherits="Mediachase.UI.Web.UserReports.GlobalModules.PickerControl" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellpadding="0" cellspacing="0" border="0" id="tblCalendar" runat="server">
	<tr>
		<td id="tdDate" runat="server">
			<asp:TextBox runat="server" ID="txtDate" Width="180px" autocomplete="off" />
		</td>
		<td runat="server" id="imgCell">
			<asp:Image ID="img1" runat="server" AlternateText=" " ImageAlign="Middle" Width="25" Height="19" CssClass="ajax__calendar_btnImage" />
		</td>
		<td id="tdSeparator" runat="server" style="width: 10px;">
			&nbsp;
		</td>
		<td runat="server" id="tdTime">
			<asp:TextBox ID="txtTime" runat="server" Width="90px" autocomplete="off"></asp:TextBox>
		</td>
		<td>
			<asp:RequiredFieldValidator ID="rfDate" runat="server" ControlToValidate="txtDate" CssClass="text" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
		</td>
	</tr>
</table>
<ibn:CalendarExtender ID="calendarButtonExtender" runat="server" TargetControlID="txtDate" PopupButtonID="img1" />
<ibn:TimePickerExtender ID="tpExt" runat="server" TargetControlID="txtTime" CssClass="text"></ibn:TimePickerExtender>
<asp:HiddenField runat="server" ID="hfUpdateCalendar" />
