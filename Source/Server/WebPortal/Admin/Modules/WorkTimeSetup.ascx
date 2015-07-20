<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.WorkTimeSetup" Codebehind="WorkTimeSetup.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="mc" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td>
			<table border="0" cellpadding="9" cellspacing="0" class="text" width="100%">
				<tr>
					<td width="180px" class="ibn-label"><%=LocRM.GetString("WorkTimeStart")%>:</td>
					<td><ibn:Picker ID="startTime" runat="server" TimeOnly="true" TimeWidth="80px" /></td>
				</tr>
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("WorkTimeFinish") %>:</td>
					<td><ibn:Picker ID="endTime" runat="server" TimeOnly="true" TimeWidth="80px" /></td>
				</tr>
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("DefaultCalendar")%>:</td>
					<td><asp:DropDownList runat="server" ID="DefaultCalendarList"></asp:DropDownList></td>
				</tr>
				<tr>
					<td colspan="2">
						<mc:IMButton id="btnSave" class="text" runat="server" />&nbsp;&nbsp;
						<mc:IMButton id="btnCancel" class="text" runat="server" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>