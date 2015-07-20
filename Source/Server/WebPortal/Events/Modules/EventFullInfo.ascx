<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Events.Modules.EventFullInfo" Codebehind="EventFullInfo.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px">
	<tr>
		<td><ibn:blockheader id="secHeader" title="" runat="server"></ibn:blockheader></td>
	</tr>
</table>
<table class="ibn-stylebox-light" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<table width="100%" class="ibn-propertysheet" cellspacing="0" cellpadding="5">
				<tr>
					<td width="120"><b><%=LocRM.GetString("Title") %>:</b></td>
					<td class="ibn-value" colspan="3"><asp:Label id="lblTitle" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td id="tdPrjLabel" runat="server"><b><%=LocRM.GetString("Project") %>:</b></td>
					<td class="ibn-value" width="35%" id="tdPrjName" runat="server"><asp:Label id="lblProject" runat="server"></asp:Label></td>
					<td width="120"><b><%=LocRM.GetString("Manager") %>:</b></td>
					<td class="ibn-value"><asp:Label id="lblManager" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("Location") %>:</b></td>
					<td class="ibn-value"><asp:Label id="lblLocation" runat="server"></asp:Label></td>
					<td><b><%=LocRM.GetString("Created") %>:</b></td>
					<td class="ibn-value"><asp:Label id="lblCreated" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("StartDate") %>:</b></td>
					<td class="ibn-value">
						<asp:Label id="lblStartDate" runat="server"></asp:Label>
						<a style='display: <%=Mediachase.IBN.Business.Security.CurrentUser.IsExternal? "none" : "" %>;' href="javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId=<% =(int)Mediachase.IBN.Business.ObjectTypes.CalendarEntry%>&ObjectId=<% =EventID%>', 420, 150)"><asp:Image ImageUrl="~/Layouts/Images/reminder.gif" Width="16" Height="16" Runat="server" ID="imgReminderStart" ImageAlign="AbsMiddle"></asp:Image></a>
					</td>
					<td><b><%=LocRM.GetString("Type") %>:</b></td>
					<td class="ibn-value"><asp:Label id="lblType" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("EndDate") %>:</b></td>
					<td class="ibn-value">
						<asp:Label id="lblEndDate" runat="server"></asp:Label>
						<a style='display: <%=Mediachase.IBN.Business.Security.CurrentUser.IsExternal? "none" : "" %>;' href="javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId=<% =(int)Mediachase.IBN.Business.ObjectTypes.CalendarEntry%>&ObjectId=<% =EventID%>', 420, 150)"><asp:Image ImageUrl="~/Layouts/Images/reminder.gif" Width="16" Height="16" Runat="server" ID="imgReminderFinish" ImageAlign="AbsMiddle"></asp:Image></a>
					</td>
					<td id="tdPriority" runat="server"><b><%=LocRM.GetString("Priority") %>:</b></td>
					<td class="ibn-value" id="tdPriority2" runat="server"><asp:Label id="lblPriority" runat="server"></asp:Label></td>
				</tr>
				<tr id="trCategoriesClient" runat="server">
					<td valign="top" id="tdCategories" runat="server"><b><%=LocRM.GetString("Category") %>:</b></td>
					<td valign="top" class="ibn-value" id="tdCategories2" runat="server"><asp:Label id="lblCategory" runat="server"></asp:Label></td>
					<td valign="top" id="tdClient" runat="server"><b><%=LocRM.GetString("tClient") %>:</b></td>
					<td valign="top" class="ibn-value" id="tdClient2" runat="server"><asp:Label id="lblClient" runat="server"></asp:Label></td>
				</tr>
				<tr style="PADDING-TOP: 8px">
					<td valign="top"><b><%=LocRM.GetString("Description") %>:</b></td>
					<td colspan="3" vAlign="top" class="ibn-description">
						<asp:Label id="lblDescription" runat="server"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>