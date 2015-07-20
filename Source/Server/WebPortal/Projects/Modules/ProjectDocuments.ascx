<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectDocuments" Codebehind="ProjectDocuments.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="Documents" src="~/Documents/Modules/DocumentList.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<table cellpadding="0" cellspacing="7" width="100%">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server"/>
			<table cellpadding="0" cellspacing="0" width="100%" class="ibn-stylebox-light">
				<tr>
					<td valign="top">
						<ibn:Documents id="ucDocuments" runat="server"/>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
