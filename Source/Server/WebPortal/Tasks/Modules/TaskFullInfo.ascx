<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.TaskFullInfo" Codebehind="TaskFullInfo.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px">
	<tr>
		<td><ibn:blockheader id="tbView" title="" runat="server"></ibn:blockheader></td>
	</tr>
</table>
<table class="ibn-stylebox-light" cellspacing="0" cellpadding="5" width="100%" border="0">
	<tr>
		<td>
			<table class="ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%">
				<tr>
					<td width="130px" valign="top"><b><%=LocRM.GetString("Title") %>:</b></td>
					<td valign="top"><asp:label id="lblTitle" runat="server" CssClass="ibn-value"></asp:label></td>
					<td valign="top"><b><asp:label id="lblParentLabel" runat="server"></asp:label></b></td>
					<td valign="top" width="30%"><asp:label id="lblParentTitle" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td width="130px" valign="top"><b><%=LocRM.GetString("BelongsTo") %>:</b></td>
					<td class="ibn-value" valign="top"><asp:label id="lblBelongsTo" runat="server"></asp:label></td>
					<td valign="top"><b><%=LocRM.GetString("Manager") %>:</b></td>
					<td valign="top" class="ibn-value"><asp:label id="lblManager" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td valign="top" id="tdPriority" runat="server"><b><%=LocRM.GetString("Priority") %>:</b></td>
					<td class="ibn-value" valign="top" id="tdPriority2" runat="server"><asp:label id="lblPriority" runat="server"></asp:label></td>
					<td valign="top"><b><%=LocRM.GetString("Created") %>:</b></td>
					<td valign="top" class="ibn-value"><asp:label id="lblCreated" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("StartDate") %>:</b></td>
					<td valign="top" class="ibn-value">
						<asp:label id="lblStartDate" runat="server"></asp:label>
						<a style='display: <%=Mediachase.IBN.Business.Security.CurrentUser.IsExternal? "none" : "" %>;' href="javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId=<% =(int)Mediachase.IBN.Business.ObjectTypes.Task%>&ObjectId=<% =TaskID%>', 420, 150)"><asp:Image ImageUrl="~/Layouts/Images/reminder.gif" Width="16" Height="16" Runat="server" ID="imgReminderStart" ImageAlign="AbsMiddle"></asp:Image></a>
					</td>
					<td valign="top"><b><%=LocRM.GetString("OutlineNumber") %>:</b></td>
					<td valign="top" class="ibn-value"><asp:label id="lblOutlineNumber" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("DueDate") %>:</b></td>
					<td valign="top" class="ibn-value">
						<asp:label id="lblDueDate" runat="server"></asp:label>
						<a style='display: <%=Mediachase.IBN.Business.Security.CurrentUser.IsExternal? "none" : "" %>;' href="javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId=<% =(int)Mediachase.IBN.Business.ObjectTypes.Task%>&ObjectId=<% =TaskID%>', 420, 150)"><asp:Image ImageUrl="~/Layouts/Images/reminder.gif" Width="16" Height="16" Runat="server" ID="imgReminderFinish" ImageAlign="AbsMiddle"></asp:Image></a>
						&nbsp;
						<asp:label id="lblOverdue" runat="server" CssClass="ibn-alerttext" Visible="False" Font-Bold="True"></asp:label></td>
					<td valign="top"><b><%=LocRM.GetString("OutlineLevel") %>:</b></td>
					<td valign="top" class="ibn-value"><asp:Label id="lblOutlineLevel" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("ActualStartDate") %>:</b></td>
					<td valign="top" class="ibn-value"><asp:label id="lblActualStartDate" runat="server"></asp:label></td>
					<td valign="top" id="tdActivation" runat="server"><b><%=LocRM.GetString("ActivationType") %>:</b></td>
					<td valign="top" class="ibn-value" id="tdActivation2" runat="server"><asp:Label id="lblActivationType" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("ActualFinishDate") %>:</b></td>
					<td valign="top" class="ibn-value"><asp:label id="lblActualFinishDate" runat="server"></asp:label></td>
					<td valign="top" id="tdCompletion" runat="server"><asp:Label id="lblComplTypeTitle" runat="server" CssClass="boldtext"></asp:Label></td>
					<td valign="top" class="ibn-value" id="tdCompletion2" runat="server"><asp:Label id="lblCompletionType" runat="server"></asp:Label></td>				
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("Completed") %>:</b></td>
					<td valign="top" class="ibn-value"><asp:label id="lblCompleted" runat="server"></asp:label>&nbsp;</td>
					<td valign="top" id="tdTaskTime" runat="server"><b><%=LocRM3.GetString("taskTime") %>:</b></td>
					<td valign="top" class="ibn-value" id="tdTaskTime2" runat="server"><asp:label id="lblTaskTime" runat="server">OverallStatus</asp:label></td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("OverallStatus") %>:</b></td>
					<td valign="top" class="ibn-value"><asp:label id="lblOverallStatus" runat="server">OverallStatus</asp:label></td>
					<td valign="top"><b><asp:Label runat="server" ID="SpentTimeLabel"></asp:Label></b></td>
					<td valign="top" class="ibn-value"><asp:Label id="lblSpentTime" runat="server"></asp:Label></td>
				</tr>
				<tr runat="server" id="trCategories">
					<td valign="top"><b><%=LocRM.GetString("Category") %>:</b></td>
					<td valign="top" class="ibn-value" colspan="3"><asp:label id="lblCategory" runat="server"></asp:label></td>
				</tr>
				<tr style="PADDING-TOP: 8px">
					<td valign="top"><b><%=LocRM.GetString("Description") %>:</b></td>
					<td valign="top" colspan="3" class="ibn-description"><asp:label id="lblDescription" runat="server"></asp:label></td>
				</tr>
			</table>
		</td>
	</tr>
</table>