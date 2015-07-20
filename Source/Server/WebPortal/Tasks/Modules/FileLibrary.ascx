<%@ Reference Control="~/Modules\BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/BlockControl.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.FileLibrary" Codebehind="FileLibrary.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockControl" src="../../Modules/BlockControl.ascx" %>
<table cellpadding="0" cellspacing="7" width="100%">
	<tr>
		<td><ibn:BlockControl id="blockControl" runat="server" /></td>
	</tr>
</table>