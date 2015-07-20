<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.IncidentGeneral" Codebehind="IncidentGeneral.ascx.cs" %>
<%@ Reference Control="~/Incidents/Modules/RelatedIncidents.ascx" %>
<%@ Reference Control="~/Incidents/Modules/IncidentsToDo.ascx" %>
<%@ Reference Control="~/Incidents/Modules/IncidentResources.ascx" %>
<%@ Reference Control="~/Incidents/Modules/ExternalRecipients.ascx" %>
<%@ Reference Control="~/Incidents/Modules/IncidentMetrics.ascx" %>
<%@ Reference Control="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="IncidentResources" src="~/Incidents/Modules/IncidentResources.ascx" %>
<%@ Register TagPrefix="ibn" TagName="IncidentRecipients" src="~/Incidents/Modules/ExternalRecipients.ascx" %>
<%@ Register TagPrefix="ibn" TagName="IncidentsToDo" src="~/Incidents/Modules/IncidentsToDo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="AdditionalInfo" src="~/Incidents/Modules/AdditionalInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MdpCustomization" src="~/Admin/Modules/MdpCustomization.ascx" %>
<%@ Register TagPrefix="ibn" TagName="RelatedIncidents" src="~/Incidents/Modules/RelatedIncidents.ascx" %>
<%@ Register TagPrefix="ibn" TagName="IncidentMetrics" src="~/Incidents/Modules/IncidentMetrics.ascx" %>
<%@ Register TagPrefix="ibn" TagName="TimeTracking" src="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfoWithAdd.ascx" %>

<table cellpadding="0" cellspacing="0" width="100%" border="0">
	<tr>
		<td valign="top" runat="server" id="tdMain" style="width: 50%; padding-right:7px">
			<ibn:IncidentRecipients id="ucIncidentRecipients" runat="server" />
			<ibn:IncidentResources id="ucIncidentResources" runat="server" />
			<ibn:IncidentsToDo id="ucIncidentsToDo" runat="server" />
			<ibn:TimeTracking runat="server" ID="ucTimeTracking" />
			<ibn:RelatedIncidents id="ucRelatedIncidents" runat="server" />
		</td>
		<td valign="top">
			<ibn:AdditionalInfo id="ucAdditionalInfo" runat="server"/>
			<ibn:IncidentMetrics id="ucIncidentMetrics" runat="server" />
		</td>
	</tr>
	<tr>
		<td colspan="2">
			<ibn:MdpCustomization id="mdView" EnableCustomize="false" runat="server" ClassName="Incidents" />
		</td>
	</tr>
</table>
