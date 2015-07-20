<%@ Reference Control="~/FileStorage/Modules/FilesList.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Common.Modules.LatestFiles" Codebehind="LatestFiles.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="FileStorage" src="..\..\FileStorage\Modules\FilesList.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px;"><tr><td>
<ibn:blockheader id="tbLatestDocuments" runat="server" />
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="1" width="100%" border="0">
	<tr>
		<td><ibn:FileStorage id="fsControl" top="5" runat="server" /></td>
	</tr>
</table>
</td></tr></table>