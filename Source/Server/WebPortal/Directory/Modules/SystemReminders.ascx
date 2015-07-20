<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.SystemReminders" Codebehind="SystemReminders.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<div style="margin-top:5px">
	<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
</div>
<table class="ibn-stylebox-light" cellspacing="0" cellpadding="0" style="width:100%">
	<tr>
		<td style="padding:10px">
			<table cellpadding="3" cellspacing="0" class="text">
				<tr id="trPrj1" runat="server">
					<td colspan="3" style="padding-top:10px" class="ibn-label"><%= LocRM.GetString("Project")%></td>
				</tr>
				<tr id="trPrj2" runat="server">
					<td align="right" width="220px"><%= LocRM.GetString("Start")%>:</td>
					<td width="150px" style="padding-left:10px" class="ibn-legend-greyblack">
						<asp:DropDownList Runat="server" ID="ddProjectStart" Width="150px" Visible="False"></asp:DropDownList>
						<asp:Label Runat="server" ID="lblProjectStart"></asp:Label>
					</td>
					<td>
						<asp:ImageButton Runat="server" ID="ibEditProjectStart" ImageUrl="~/layouts/images/edit.gif" BorderWidth="0" Width="16px" Height="16px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibRestoreProjectStart" ImageUrl="~/layouts/images/import.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibSaveProjectStart" ImageUrl="~/layouts/images/Saveitem.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibCancelProjectStart" ImageUrl="~/layouts/images/cancel.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
					</td>
				</tr>
				<tr id="trPrj3" runat="server">
					<td align="right"><%= LocRM.GetString("Finish")%>:</td>
					<td style="padding-left:10px" class="ibn-legend-greyblack">
						<asp:DropDownList Runat="server" ID="ddProjectFinish" Width="150px" Visible="False"></asp:DropDownList>
						<asp:Label Runat="server" ID="lblProjectFinish"></asp:Label>
					</td>
					<td>
						<asp:ImageButton Runat="server" ID="ibEditProjectFinish" ImageUrl="~/layouts/images/edit.gif" BorderWidth="0" Width="16px" Height="16px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibRestoreProjectFinish" ImageUrl="~/layouts/images/import.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibSaveProjectFinish" ImageUrl="~/layouts/images/Saveitem.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibCancelProjectFinish" ImageUrl="~/layouts/images/cancel.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
					</td>
				</tr>
				<tr id="trTask1" runat="server">
					<td colspan="3" style="padding-top:10px" class="ibn-label"><%= LocRM.GetString("Task")%></td>
				</tr>
				<tr id="trTask2" runat="server">
					<td align="right"><%= LocRM.GetString("Start")%>:</td>
					<td style="padding-left:10px" class="ibn-legend-greyblack">
						<asp:DropDownList Runat="server" ID="ddTaskStart" Width="150px" Visible="False"></asp:DropDownList>
						<asp:Label Runat="server" ID="lblTaskStart"></asp:Label>
					</td>
					<td>
						<asp:ImageButton Runat="server" ID="ibEditTaskStart" ImageUrl="~/layouts/images/edit.gif" BorderWidth="0" Width="16px" Height="16px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibRestoreTaskStart" ImageUrl="~/layouts/images/import.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibSaveTaskStart" ImageUrl="~/layouts/images/Saveitem.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibCancelTaskStart" ImageUrl="~/layouts/images/cancel.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
					</td>
				</tr>
				<tr id="trTask3" runat="server">
					<td align="right"><%= LocRM.GetString("Finish")%>:</td>
					<td style="padding-left:10px" class="ibn-legend-greyblack">
						<asp:DropDownList Runat="server" ID="ddTaskFinish" Width="150px" Visible="False"></asp:DropDownList>
						<asp:Label Runat="server" ID="lblTaskFinish"></asp:Label>
					</td>
					<td>
						<asp:ImageButton Runat="server" ID="ibEditTaskFinish" ImageUrl="~/layouts/images/edit.gif" BorderWidth="0" Width="16px" Height="16px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibRestoreTaskFinish" ImageUrl="~/layouts/images/import.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibSaveTaskFinish" ImageUrl="~/layouts/images/Saveitem.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibCancelTaskFinish" ImageUrl="~/layouts/images/cancel.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
					</td>
				</tr>
				<tr>
					<td colspan="3" style="padding-top:10px" class="ibn-label"><%= LocRM.GetString("Todo")%></td>
				</tr>
				<tr>
					<td align="right"><%= LocRM.GetString("Start")%>:</td>
					<td style="padding-left:10px" class="ibn-legend-greyblack">
						<asp:DropDownList Runat="server" ID="ddToDoStart" Width="150px" Visible="False"></asp:DropDownList>
						<asp:Label Runat="server" ID="lblToDoStart"></asp:Label>
					</td>
					<td>
						<asp:ImageButton Runat="server" ID="ibEditToDoStart" ImageUrl="~/layouts/images/edit.gif" BorderWidth="0" Width="16px" Height="16px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibRestoreToDoStart" ImageUrl="~/layouts/images/import.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibSaveToDoStart" ImageUrl="~/layouts/images/Saveitem.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibCancelToDoStart" ImageUrl="~/layouts/images/cancel.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
					</td>
				</tr>
				<tr>
					<td align="right"><%= LocRM.GetString("Finish")%>:</td>
					<td style="padding-left:10px" class="ibn-legend-greyblack">
						<asp:DropDownList Runat="server" ID="ddToDoFinish" Width="150px" Visible="False"></asp:DropDownList>
						<asp:Label Runat="server" ID="lblToDoFinish"></asp:Label>
					</td>
					<td>
						<asp:ImageButton Runat="server" ID="ibEditToDoFinish" ImageUrl="~/layouts/images/edit.gif" BorderWidth="0" Width="16px" Height="16px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibRestoreToDoFinish" ImageUrl="~/layouts/images/import.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibSaveToDoFinish" ImageUrl="~/layouts/images/Saveitem.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibCancelToDoFinish" ImageUrl="~/layouts/images/cancel.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
					</td>
				</tr>
				<tr>
					<td colspan="3" style="padding-top:10px" class="ibn-label"><%= LocRM.GetString("CalendarEntry")%></td>
				</tr>
				<tr>
					<td align="right"><%= LocRM.GetString("Start")%>:</td>
					<td style="padding-left:10px" class="ibn-legend-greyblack">
						<asp:DropDownList Runat="server" ID="ddCalendarEntryStart" Width="150px" Visible="False"></asp:DropDownList>
						<asp:Label Runat="server" ID="lblCalendarEntryStart"></asp:Label>
					</td>
					<td>
						<asp:ImageButton Runat="server" ID="ibEditCalendarEntryStart" ImageUrl="~/layouts/images/edit.gif" BorderWidth="0" Width="16px" Height="16px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibRestoreCalendarEntryStart" ImageUrl="~/layouts/images/import.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibSaveCalendarEntryStart" ImageUrl="~/layouts/images/Saveitem.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibCancelCalendarEntryStart" ImageUrl="~/layouts/images/cancel.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
					</td>
				</tr>
				<tr>
					<td align="right"><%= LocRM.GetString("Finish")%>:</td>
					<td style="padding-left:10px" class="ibn-legend-greyblack">
						<asp:DropDownList Runat="server" ID="ddCalendarEntryFinish" Width="150px" Visible="False"></asp:DropDownList>
						<asp:Label Runat="server" ID="lblCalendarEntryFinish"></asp:Label>
					</td>
					<td>
						<asp:ImageButton Runat="server" ID="ibEditCalendarEntryFinish" ImageUrl="~/layouts/images/edit.gif" BorderWidth="0" Width="16px" Height="16px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibRestoreCalendarEntryFinish" ImageUrl="~/layouts/images/import.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibSaveCalendarEntryFinish" ImageUrl="~/layouts/images/Saveitem.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibCancelCalendarEntryFinish" ImageUrl="~/layouts/images/cancel.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
					</td>
				</tr>
				<tr runat="server" id="AssignmentRow1">
					<td colspan="3" style="padding-top:10px" class="ibn-label"><%= LocRM.GetString("Assignment")%></td>
				</tr>
				<tr  runat="server" id="AssignmentRow2">
					<td align="right"><%= LocRM.GetString("Finish")%>:</td>
					<td style="padding-left:10px" class="ibn-legend-greyblack">
						<asp:DropDownList Runat="server" ID="ddAssignmentFinish" Width="150px" Visible="False"></asp:DropDownList>
						<asp:Label Runat="server" ID="lblAssignmentFinish"></asp:Label>
					</td>
					<td>
						<asp:ImageButton Runat="server" ID="ibEditAssignmentFinish" ImageUrl="~/layouts/images/edit.gif" BorderWidth="0" Width="16px" Height="16px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibRestoreAssignmentFinish" ImageUrl="~/layouts/images/import.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibSaveAssignmentFinish" ImageUrl="~/layouts/images/Saveitem.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibCancelAssignmentFinish" ImageUrl="~/layouts/images/cancel.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>