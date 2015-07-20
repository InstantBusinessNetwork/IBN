<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.ToDoGeneral2" Codebehind="ToDoGeneral2.ascx.cs" %>
<%@ Reference Control="~/ToDo/Modules/QTracker.ascx" %>
<%@ Reference Control="~/ToDo/Modules/ToDoGeneralResources.ascx" %>
<%@ Reference Control="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Doc" src="~/Common/Modules/LatestFiles.ascx" %>
<%@ Register TagPrefix="ibn" TagName="QTracker" src="~/ToDo/Modules/QTracker.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Resources" src="~/ToDo/Modules/ToDoGeneralResources.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Comments" src="~/Common/Modules/LatestComments.ascx" %>
<%@ Register TagPrefix="ibn" TagName="TimeTracking" src="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>

<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td valign="top" style="width:49%;">
			<ibn:qtracker id="qtrack" runat="server" />
		</td>
		<td valign="top" style="width:10px;"></td>
		<td style="width:50%;" valign="top">
			<ibn:Resources id="res" HideIfEmpty="True" runat="server" />
			<ibn:Comments runat="server" id="LatestCommentsTemplate" />
			<ibn:Doc id="DocsList" HideIfEmpty="True" runat="server" />
			<ibn:TimeTracking runat="server" ID="ibnTimeTracking" />
		</td>
	</tr>
</table>