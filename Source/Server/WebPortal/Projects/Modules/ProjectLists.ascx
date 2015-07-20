<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectLists" Codebehind="ProjectLists.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Apps/ListApp/Modules/ManageControls/ListInfoList.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ListInfoList" Src="~/Apps/ListApp/Modules/ManageControls/ListInfoList.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellpadding="0" cellspacing="7" width="100%">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server"/>
			<table cellpadding="0" cellspacing="0" width="100%" class="ibn-stylebox-light">
				<tr>
					<td valign="top">
						<ibn:ListInfoList ID="ctrlLists" runat="server" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
