<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.ToDoGeneralTaskLayout" Codebehind="ToDoGeneralTaskLayout.ascx.cs" %>
<%@ Reference Control="~/ToDo/Modules/ToDoGeneralResources.ascx" %>
<%@ Reference Control="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="QTracker" src="~/ToDo/Modules/QTracker.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Resources" src="~/ToDo/Modules/ToDoGeneralResources.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Comments" src="~/Common/Modules/LatestComments.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Docs" src="~/Common/Modules/LatestFiles.ascx" %>
<%@ Register TagPrefix="ibn" TagName="task" src="~/ToDo/Modules/ToDoShortInfoTask.ascx" %>
<%@ Register TagPrefix="ibn" TagName="TimeTracking" src="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>

<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td valign="top" style="width:49%;">
			<ibn:qtracker id="qtracker" runat="server" />
		</td>
		<td style="padding-left:5px; width:51%;" valign="top">
			<ibn:resources id="res" runat="server" />
			<ibn:task id="task" runat="server" />
			<ibn:Comments runat="server" id="LatestCommentsTemplate" />
			<ibn:docs id="DocsList" runat="server"></ibn:docs>
			<ibn:TimeTracking runat="server" ID="ibnTimeTracking" />
		</td>
	</tr>
</table>
