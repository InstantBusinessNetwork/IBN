<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Events.Modules.EventGeneral" Codebehind="EventGeneral.ascx.cs" %>
<%@ Reference Control="~/Common/Modules/LatestComments.ascx" %>
<%@ Reference Control="~/Events/Modules/QTAcceptDeny.ascx" %>
<%@ Reference Control="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfoWithAdd.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Docs" src="~/Common/Modules/LatestFiles.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Recurrence" src="~/Events/Modules/Recurrence.ascx" %>
<%@ Register TagPrefix="ibn" TagName="EventParticipants" src="~/Events/Modules/EventGeneralParticipants.ascx" %>
<%@ Register TagPrefix="ibn" TagName="QT" src="~/Events/Modules/QTAcceptDeny.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Comments" src="~/Common/Modules/LatestComments.ascx" %>
<%@ Register TagPrefix="ibn" TagName="TimeTracking" src="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfoWithAdd.ascx" %>

<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td valign="top" style="width: 50%;">
			<!-- EventParticipants -->
			<ibn:EventParticipants id="ctrlEP" runat="server" />
			<!-- end EventParticipants -->
			<ibn:QT id="ctrlQT" runat="server" />
			<ibn:Recurrence id="Recurrence" runat="server" />
		</td>
		<td style="width: 10px;">&nbsp;</td>
		<td valign="top">
			<ibn:Comments runat="server" id="LatestCommentsTemplate"></ibn:Comments>
			<ibn:TimeTracking runat="server" ID="ibnTimeTracking" />
			<ibn:docs id="DocsList" runat="server"></ibn:docs>
		</td>
	</tr>
</table>
