<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.IncidentDetails" Codebehind="IncidentDetails.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="MdpCustomization" src="..\..\Admin\Modules\MdpCustomization.ascx" %>
<%@ Register TagPrefix="ibn" TagName="IncidentGeneralInfo" src="IncidentGeneralInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="AdditionalInfo" src="AdditionalInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="RelIss" src="..\..\Incidents\modules\RelatedIncidents.ascx" %>
<table cellpadding="0" cellspacing="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:IncidentGeneralInfo id="ucIncidentGeneralInfo" runat="server"/>
			<ibn:AdditionalInfo id="ucAdditionalInfo" runat="server"/>
		</td>
	</tr>
	<tr>
		<td style="padding-top:5px">
			<ibn:MdpCustomization id="mdView" EnableCustomize="false" runat="server" ClassName="Incidents" />
		</td>
	</tr>
	<tr>
		<td>
			<ibn:RelIss id="RelIssues" runat="server" />
		</td>
	</tr>
</table>
