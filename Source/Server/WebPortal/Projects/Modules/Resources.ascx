<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.Resources" Codebehind="Resources.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="ProjectTeam" src="..\..\Projects\modules\ProjectGeneralTeam.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ProjectSponsors" src="..\..\Projects\modules\ProjectGeneralSponsors.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ProjectStakeholders" src="..\..\Projects\modules\ProjectGeneralStakeholders.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ProjectManagers" src="..\..\Projects\modules\ProjectManagers.ascx" %>
<table cellspacing=7 cellpadding=0 width="100%" border=0>
	<tr>
		<td valign="top" width="50%">
			<ibn:ProjectTeam id=ctrlProjectTeam runat="server"></ibn:ProjectTeam>
		</td>
		<td valign="top" width="50%">
			<ibn:ProjectManagers id="ucProjectManagers" runat="server" />
			<div style="margin-top:7px">
				<ibn:ProjectSponsors id="ucProjectSponsors" runat="server" />
			</div>
			<div style="margin-top:7px">
				<ibn:ProjectStakeholders id="ucProjectStakeholders" runat="server" />
			</div>
		</td>
	</tr>
</table>