<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.ToDoGeneralIncidentLayout" Codebehind="ToDoGeneralIncidentLayout.ascx.cs" %>
<%@ Reference Control="~/ToDo/Modules/ToDoGeneralResources.ascx" %>
<%@ Reference Control="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="QTracker" src="~/ToDo/Modules/QTracker.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Incident" src="~/ToDo/Modules/ToDoShortInfoIncident.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Resources" src="~/ToDo/Modules/ToDoGeneralResources.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Comments" src="~/Common/Modules/LatestComments.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Docs" src="~/Common/Modules/LatestFiles.ascx" %>
<%@ Register TagPrefix="ibn" TagName="TimeTracking" src="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>

<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td valign="top" style="width:50%;">
			<ibn:qtracker id="qtracker" runat="server" />
		</td>
		<td style="width:7px;">&nbsp;</td>
		<td valign="top">
			<ibn:resources id="res" runat="server" />
			<ibn:incident id="incident" runat="server" />
			<ibn:Comments runat="server" id="LatestCommentsTemplate" />
			<ibn:docs id="DocsList" runat="server"></ibn:docs>
			<ibn:TimeTracking runat="server" ID="ibnTimeTracking" />
		</td>
	</tr>
</table>
