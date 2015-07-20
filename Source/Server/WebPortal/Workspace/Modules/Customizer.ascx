<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Workspace.Modules.Customizer" Codebehind="Customizer.ascx.cs" %>
<%@ register TagPrefix="btn" namespace="Mediachase.UI.Web.Modules" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<script language="javascript">
function SetMyUpdatesEnabled()
{
	cb = document.forms[0].<%=cbShowMyUpdates.ClientID%>;
	document.forms[0].<%=cbShowMyProjects.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowMyIncidents.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowMyDocument.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowMyCalendarEntries.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowMyTaskToDo.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowMyList.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=ddMyDays.ClientID%>.disabled = !cb.checked;
}

function SetUpdatesEnabled()
{
	cb = document.forms[0].<%=cbShowAllUpdates.ClientID%>;
	document.forms[0].<%=cbShowProjects.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowIncidents.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowDocument.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowCalendarEntries.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowTaskToDo.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowList.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=ddDays.ClientID%>.disabled = !cb.checked;
}
function SetShowActivities()
{
	cb = document.forms[0].<%=cbShowActivities.ClientID%>;
	
	try{
	document.forms[0].<%=cbShowNewPending.ClientID%>.disabled = !cb.checked;
	} catch(e)
	{}
	
	try{
	document.forms[0].<%=cbShowIssuesYouNeed.ClientID%>.disabled = !cb.checked;
	} catch(e)
	{}
	
	try
	{
	document.forms[0].<%=cbShowExternalEmail.ClientID%>.disabled = !cb.checked;
	} catch (e)
	{}
	document.forms[0].<%=cbShowCompletedTasksToDos.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowTasksToDosAwaiting.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowEventsAssignments.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowTasksToDosAssign.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowIssuesAssignments.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowPendingTimesheets.ClientID%>.disabled = !cb.checked;
}

function SetShowActiveProjects()
{
	cb = document.forms[0].<%=cbShowActiveProjects.ClientID%>;
	document.forms[0].<%=cbShowActiveProjectsInvolved.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowTasksToDosInvolved.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowIssuesResponsible.ClientID%>.disabled = !cb.checked;
	document.forms[0].<%=cbShowDocumentsTasks.ClientID%>.disabled = !cb.checked;
}

function SetShowThisWeek()
{
	cb = document.forms[0].<%=cbShowThisWeek.ClientID%>;
	document.forms[0].<%=ddTWDaysBefore.ClientID%>.disabled = !cb.checked;
}
</script>
<TABLE class="ibn-stylebox" cellSpacing="0" cellPadding="0" width="100%" border="0" style="margin-top:0px;margin-left:2px;margin-bottom:6px;margin-right:-6px">
	<tr>
		<td><ibn:blockheader id="tbHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<div class="text" style="PADDING-RIGHT: 10px; PADDING-LEFT: 25px; PADDING-BOTTOM: 5px; PADDING-TOP: 10px">
				<%=LocRM.GetString("RecsPerPage")%>
				&nbsp;<asp:DropDownList ID="ddRecsPerPage" Runat="server" CssClass="text" Width="40"></asp:DropDownList></div>
			<table class="ibn-propertysheet" cellPadding="4" width="100%" border="0">
				<tr>
					<td vAlign="top" width="49%">
						<fieldset><LEGEND>
								<asp:checkbox id="cbShowAllUpdates" Runat="server" Text='<%# LocRM.GetString("ShowAllUpdates")%>'>
								</asp:checkbox><BR>
							</LEGEND>
							<DIV style="PADDING-RIGHT: 10px; PADDING-LEFT: 25px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
								<asp:checkbox id="cbShowProjects" Runat="server" Text='<%# LocRM.GetString("ShowProjects")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowIncidents" Runat="server" Text='<%# LocRM.GetString("ShowIncidents")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowDocument" Runat="server" Text='<%# LocRM.GetString("ShowDocuments")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowCalendarEntries" Runat="server" Text='<%# LocRM.GetString("ShowEvents")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowTaskToDo" Runat="server" Text='<%# LocRM.GetString("ShowTasks")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowList" Runat="server" Text='<%# LocRM.GetString("ShowLists")%>'>
								</asp:checkbox><BR>
								<BR>
								<%=LocRM.GetString("DisplayItems")%>
								&nbsp;&nbsp;
								<asp:DropDownList id="ddDays" Width="40" CssClass="text" Runat="server"></asp:DropDownList>&nbsp;&nbsp;<%=LocRM.GetString("Days")%></DIV>
						</fieldset>
						<BR>
						<fieldset><LEGEND>
								<asp:checkbox id="cbShowMyUpdates" Runat="server" Text='<%# LocRM.GetString("ShowMyUpdates")%>'>
								</asp:checkbox></LEGEND>
							<DIV style="PADDING-RIGHT: 10px; PADDING-LEFT: 25px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
								<asp:checkbox id="cbShowMyProjects" Runat="server" Text='<%# LocRM.GetString("ShowProjects")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowMyIncidents" Runat="server" Text='<%# LocRM.GetString("ShowIncidents")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowMyDocument" Runat="server" Text='<%# LocRM.GetString("ShowDocuments")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowMyCalendarEntries" Runat="server" Text='<%# LocRM.GetString("ShowEvents")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowMyTaskToDo" Runat="server" Text='<%# LocRM.GetString("ShowTasks")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowMyList" Runat="server" Text='<%# LocRM.GetString("ShowLists")%>'>
								</asp:checkbox><BR>
								<BR>
								<%=LocRM.GetString("DisplayItems")%>
								&nbsp;&nbsp;
								<asp:DropDownList id="ddMyDays" Width="40" CssClass="text" Runat="server"></asp:DropDownList>&nbsp;&nbsp;<%=LocRM.GetString("Days")%></DIV>
						</fieldset><br>
						<fieldset>
						<legend><%=LocRM.GetString("Other")%></legend>
						<DIV style="PADDING-RIGHT: 10px; PADDING-LEFT: 25px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
							<asp:checkbox id="cbShowGettingStarted" Runat="server" Text='<%# LocRM.GetString("ShowGettingStarted")%>'>
							</asp:checkbox>						
							<br>
							<asp:checkbox id="cbShowMostRecentTimesheet" Runat="server" Text='<%# LocRM.GetString("ShowMostRecentTimesheet")%>'>
							</asp:checkbox><BR>
							<asp:CheckBox id=cbCollapseBlocks runat="server" Text='<%# LocRM.GetString("AutomaticallyCollapseBlocks")%>' Checked="True"></asp:CheckBox>
						</DIV>					
						</fieldset>
					</td>
					<td width="10">&nbsp;</td>
					<td vAlign="top" width="50%">
						<FIELDSET><LEGEND>
								<asp:checkbox id="cbShowActivities" Runat="server" Text='<%# LocRM.GetString("ShowActivities")%>'>
								</asp:checkbox></LEGEND>
							<DIV style="PADDING-RIGHT: 10px; PADDING-LEFT: 25px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
								<asp:checkbox id="cbShowNewPending" Runat="server" Text='<%# LocRM.GetString("ShowNewPending")+"<BR>"%>'>
								</asp:checkbox>
								<asp:checkbox id="cbShowIssuesYouNeed" Runat="server" Text='<%# LocRM.GetString("ShowIssuesYouNeed")+"<BR>"%>'>
								</asp:checkbox>
								<asp:checkbox id="cbShowExternalEmail" Runat="server" Text='<%# LocRM.GetString("ShowExternalEmail")+"<BR>"%>'>
								</asp:checkbox>
								<asp:checkbox id="cbShowCompletedTasksToDos" Runat="server" Text='<%# LocRM.GetString("ShowCompletedTasksToDos")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowTasksToDosAwaiting" Runat="server" Text='<%# LocRM.GetString("ShowTasksToDosAwaiting")%>'>
								</asp:checkbox><br>
								<asp:checkbox id="cbShowEventsAssignments" Runat="server" Text='<%# LocRM.GetString("ShowEventsAssignments ")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowTasksToDosAssign" Runat="server" Text='<%# LocRM.GetString("ShowTasksToDosAssign")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowIssuesAssignments" Runat="server" Text='<%# LocRM.GetString("ShowIssuesAssignments")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowPendingTimesheets" Runat="server" Text='<%# LocRM.GetString("ShowPendingTimesheets")%>'>
								</asp:checkbox><BR>
							</DIV>
						</FIELDSET>
						<BR>
						<FIELDSET><LEGEND>
								<asp:checkbox id="cbShowActiveProjects" Runat="server" Text='<%# LocRM.GetString("ShowActiveProjects")%>'>
								</asp:checkbox></LEGEND>
							<DIV style="PADDING-RIGHT: 10px; PADDING-LEFT: 25px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
								<asp:checkbox id="cbShowActiveProjectsInvolved" Runat="server" Text='<%# LocRM.GetString("ShowActiveProjectsInvolved")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowTasksToDosInvolved" Runat="server" Text='<%# LocRM.GetString("ShowTasksToDosInvolved")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowIssuesResponsible" Runat="server" Text='<%# LocRM.GetString("ShowIssuesResponsible")%>'>
								</asp:checkbox><BR>
								<asp:checkbox id="cbShowDocumentsTasks" Runat="server" Text='<%# LocRM.GetString("ShowDocumentsTasks")%>'>
								</asp:checkbox></DIV>
						</FIELDSET>
						<br>
						<FIELDSET><LEGEND>
								<asp:checkbox id="cbShowThisWeek" Runat="server" Text='<%# LocRM.GetString("ShowThisWeek")%>'>
								</asp:checkbox></LEGEND>
							<DIV style="PADDING-RIGHT: 10px; PADDING-LEFT: 15px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
								<%=LocRM.GetString("ThisWeekDaysBefore") %>
								&nbsp;
								<asp:DropDownList ID="ddTWDaysBefore" Runat="server" Width="160px"></asp:DropDownList>
							</DIV>
						</FIELDSET>

					</td>
				</tr>
				<tr>
					<td vAlign="center" align="right" height="60" colspan="3"><btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;" CustomImage="../layouts/images/saveitem.gif" onserverclick="btnSave_Click">cc</btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true" onserverclick="btnCancelClick">cc</btn:imbutton></td>
				</tr>
			</table>
		</td>
	</tr>
</TABLE>
