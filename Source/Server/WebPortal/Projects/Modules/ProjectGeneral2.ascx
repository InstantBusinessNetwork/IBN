<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectGeneral2" Codebehind="ProjectGeneral2.ascx.cs" %>
<%@ Reference Control="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ProjectGeneralInfo" src="ProjectGeneralInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ProjectStateInfo" src="ProjectStateInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ProjectTimelineInfo" src="ProjectTimelineInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ProjectClientInfo" src="ProjectClientInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataViewControl" src="~/Modules/MetaDataViewControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MdpCustomization" src="~/Admin/Modules/MdpCustomization.ascx" %>
<%@ Register TagPrefix="ibn" TagName="RelatedProjects" src="RelatedProjects.ascx" %>
<%@ Register TagPrefix="ibn" TagName="SystemReminders" src="~/Directory/Modules/SystemRemindersForObject.ascx" %>
<%@ Register TagPrefix="ibn" TagName="TimeTracking" src="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>

<table cellpadding="0" cellspacing="7" width="100%">
	<tr>
		<td valign="top" style="width:50%;">
			<ibn:ProjectTimelineInfo id="ucProjectTimelineInfo" runat="server"/>
			<ibn:ProjectClientInfo id="ucProjectclientinfo" runat="server"/>
		</td>
		<td valign="top" style="width:50%;">
			<ibn:ProjectStateInfo id="ucProjectStateInfo" runat="server"/>
		</td>
	</tr>
	<tr>
		<td colspan="2">
			<ibn:ProjectGeneralInfo id="ucProjectGeneralInfo" runat="server"/>
		</td>
	</tr>
	<tr>
		<td colspan="2">
			<ibn:MdpCustomization id="mdView" EnableCustomize="false" runat="server" ClassName="Projects" />
		</td>
	</tr>
	<tr>	
		<td valign="top">
			<ibn:RelatedProjects id="usRelatedProjects" runat="server" />
		</td>
		<td valign="top">
			<ibn:TimeTracking runat="server" ID="ibnTimeTracking" />
		</td>
	</tr>
</table>


