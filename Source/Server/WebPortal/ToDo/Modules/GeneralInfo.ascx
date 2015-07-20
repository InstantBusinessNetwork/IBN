<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.GeneralInfo" Codebehind="GeneralInfo.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:3px;"><tr><td>
<ibn:blockheader id="tbView" runat="server"></ibn:blockheader>
</td></tr></table>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="1" width="100%" border="0">
	<tr>
		<td style="PADDING-LEFT: 7px">
			<table width="100%" cellpadding="5" cellspacing="0" class="ibn-propertysheet">
				<tr>
					<td width="160" valign="top"><b><%=LocRM.GetString("Title") %>:</b></td>
					<td class="ibn-value" width="30%" valign="top"><asp:Label id="lblTitle" runat="server"></asp:Label></td>
					<td width="130" class="ibn-label" valign="top" id="tdParentTitle" runat="server"><asp:Label id="lblParentTitle" runat="server"></asp:Label>:</td>
					<td class="ibn-value" valign="top" id="tdParentName" runat="server"><asp:Label id="lblParent" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td valign="top" id="tdPriority" runat="server"><b><%=LocRM.GetString("Priority") %>:</b></td>
					<td class="ibn-value" valign="top" id="tdPriority2" runat="server"><asp:Label id="lblPriority" runat="server"></asp:Label></td>
					<td valign="top"><b><%=LocRM.GetString("Manager") %>:</b></td>
					<td valign="top" class="ibn-value"><asp:Label id="lblManager" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("StartDate") %>:</b></td>
					<td class="ibn-value" valign="top">
						<asp:Label id="lblStartDate" runat="server"></asp:Label>
						<a style='display: <%=Mediachase.IBN.Business.Security.CurrentUser.IsExternal? "none" : "" %>;' href="javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId=<% =(int)Mediachase.IBN.Business.ObjectTypes.ToDo%> &ObjectId=<% =ToDoID%>', 420, 150)"><asp:Image ImageUrl="~/Layouts/Images/reminder.gif" Width="16" Height="16" Runat="server" ID="imgReminderStart" ImageAlign="AbsMiddle"></asp:Image></a>
					</td>
					<td valign="top"><b><%=LocRM.GetString("Created") %>:</b></td>
					<td class="ibn-value" valign="top"><asp:Label id="lblCreated" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("DueDate") %>:</b></td>
					<td class="ibn-value" valign="top">
						<asp:Label id="lblDueDate" runat="server"></asp:Label>
						<a style='display: <%=Mediachase.IBN.Business.Security.CurrentUser.IsExternal? "none" : "" %>;' href="javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId=<% =(int)Mediachase.IBN.Business.ObjectTypes.ToDo%> &ObjectId=<% =ToDoID%>', 420, 150)"><asp:Image ImageUrl="~/Layouts/Images/reminder.gif" Width="16" Height="16" Runat="server" ID="imgReminderFinish" ImageAlign="AbsMiddle"></asp:Image></a>
						&nbsp;
						<asp:label id="lblOverdue" runat="server" CssClass="ibn-alertboldtext" Visible="False"></asp:label>
					</td>
					<td valign="top"><b><%=LocRM.GetString("OverallStatus") %>:</b></td>
					<td class="ibn-value" valign="top"><asp:Label id="lblOverallStatus" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("Completed") %>:</b></td>
					<td valign="top" class="ibn-value"><asp:label id="lblCompleted" runat="server"></asp:label></td>
					<td valign="top" id="tdCompletion" runat="server"><b><%=LocRM.GetString("CompletionType") %>:</b></td>
					<td class="ibn-value" valign="top" id="tdCompletion2" runat="server"><asp:Label id="lblCompletionType" runat="server"></asp:Label></td>
				</tr>
				<tr runat="server" id="trActualFinishDate">
					<td valign="top"><b><%=LocRM.GetString("ActualFinishDate") %>:</b></td>
					<td colspan="3" class="ibn-value" valign="top"><asp:label id="lblActualFinishDate" runat="server"></asp:label></td>
				</tr>
				<tr id="trCategoriesClient" runat="server">
					<td valign="top" id="tdCategories" runat="server"><b><%=LocRM.GetString("Category") %>:</b></td>
					<td class="ibn-value" valign="top" id="tdCategories2" runat="server"><asp:label id="lblCategory" runat="server"></asp:label></td>
					<td valign="top" id="tdClient" runat="server"><b><%=LocRM.GetString("tClient") %>:</b></td>
					<td class="ibn-value" valign="top" id="tdClient2" runat="server"><asp:Label id="lblClient" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td valign="top" id="tdTaskTime" runat="server"><b><%=LocRM3.GetString("taskTime")%>:</b></td>
					<td class="ibn-value" valign="top" id="tdTaskTime2" runat="server"><asp:label id="lblTaskTime" runat="server"></asp:label></td>
					<td valign="top"><b><asp:Label runat="server" ID="SpentTimeLabel"></asp:Label></b></td>
					<td class="ibn-value" valign="top"><asp:Label id="lblSpentTime" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("Description") %>:</b></td>
					<td colspan="3" class="ibn-description" valign="top"><asp:Label id="lblDescription" runat="server"></asp:Label></td>
				</tr>
			</table>
		</td>
	</tr>
</table>