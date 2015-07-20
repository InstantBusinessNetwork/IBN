<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.GlobalReminders" Codebehind="GlobalReminders.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="MARGIN-TOP:0px">
	<tr>
		<td><ibn:blockheader id="secHeader" title="" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="padding:10px">
			<table cellpadding="3" cellspacing="0" class="text">
				<tr id="trPrj1" runat="server">
					<td colspan="2" style="padding-top:10px" class="ibn-label"><%= LocRM.GetString("Project")%></td>
				</tr>
				<tr id="trPrj2" runat="server">
					<td width="220px" align="right"><%= LocRM.GetString("Start")%>:</td>
					<td style="padding-left:10px">
						<asp:DropDownList Runat="server" ID="ddProjectStart" Width="200px"></asp:DropDownList>
					</td>
				</tr>
				<tr id="trPrj3" runat="server">
					<td align="right"><%= LocRM.GetString("Finish")%>:</td>
					<td style="padding-left:10px">
						<asp:DropDownList Runat="server" ID="ddProjectFinish" Width="200px"></asp:DropDownList>
					</td>
				</tr>
				<tr id="trTask1" runat="server">
					<td colspan="2" style="padding-top:10px" class="ibn-label"><%= LocRM.GetString("Task")%></td>
				</tr>
				<tr id="trTask2" runat="server">
					<td align="right"><%= LocRM.GetString("Start")%>:</td>
					<td style="padding-left:10px">
						<asp:DropDownList Runat="server" ID="ddTaskStart" Width="200px"></asp:DropDownList>
					</td>
				</tr>
				<tr id="trTask3" runat="server">
					<td align="right"><%= LocRM.GetString("Finish")%>:</td>
					<td style="padding-left:10px">
						<asp:DropDownList Runat="server" ID="ddTaskFinish" Width="200px"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td colspan="2" style="padding-top:10px" class="ibn-label"><%= LocRM.GetString("Todo")%></td>
				</tr>
				<tr>
					<td align="right"><%= LocRM.GetString("Start")%>:</td>
					<td style="padding-left:10px">
						<asp:DropDownList Runat="server" ID="ddToDoStart" Width="200px"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td align="right"><%= LocRM.GetString("Finish")%>:</td>
					<td style="padding-left:10px">
						<asp:DropDownList Runat="server" ID="ddToDoFinish" Width="200px"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td colspan="2" style="padding-top:10px" class="ibn-label"><%= LocRM.GetString("CalendarEntry")%></td>
				</tr>
				<tr>
					<td align="right"><%= LocRM.GetString("Start")%>:</td>
					<td style="padding-left:10px">
						<asp:DropDownList Runat="server" ID="ddCalendarEntryStart" Width="200px"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td align="right"><%= LocRM.GetString("Finish")%>:</td>
					<td style="padding-left:10px">
						<asp:DropDownList Runat="server" ID="ddCalendarEntryFinish" Width="200px"></asp:DropDownList>
					</td>
				</tr>
				<tr runat="server" id="AssignmentRow1">
					<td colspan="2" style="padding-top:10px" class="ibn-label"><%= LocRM.GetString("Assignment")%></td>
				</tr>
				<tr runat="server" id="AssignmentRow2">
					<td align="right"><%= LocRM.GetString("Finish")%>:</td>
					<td style="padding-left:10px">
						<asp:DropDownList Runat="server" ID="ddAssignmentFinish" Width="200px"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td colspan="2" align="right" height="80"> 	
						<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" id="btnCancel" Runat="server" style="width:110px; CausesValidation="false"></btn:imbutton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>