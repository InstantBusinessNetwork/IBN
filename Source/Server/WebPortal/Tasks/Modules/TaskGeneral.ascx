<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.TaskGeneral" Codebehind="TaskGeneral.ascx.cs" %>
<%@ Reference Control="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Comments" src="~/Common/Modules/LatestComments.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Docs" src="~/Common/Modules/LatestFiles.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Resources" src="~/Tasks/Modules/TaskGeneralResources.ascx" %>
<%@ Register TagPrefix="ibn" TagName="QTracker" src="~/Tasks/Modules/QTracker.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ToDo" src="~/Tasks/Modules/TasksToDo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="TimeTracking" src="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>

<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td valign="top" style="width:49%;">
			<ibn:QTracker id="qtrk" runat="server" />
		</td>
		<td valign="top" style="width:10px;"></td>
		<td style="width:50%;" valign="top">
			<ibn:Resources id="res" runat="server" />
			<ibn:ToDo id="ToDoList" runat="server" />
			<ibn:Comments id="LatestCommentsTemplate" runat="server" />
			<ibn:Docs id="DocsList" runat="server" />
			<ibn:TimeTracking runat="server" ID="ibnTimeTracking" />
		</td>
	</tr>
</table>
