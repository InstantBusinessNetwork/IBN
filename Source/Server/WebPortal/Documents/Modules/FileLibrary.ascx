<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/BlockControl.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.FileLibrary" Codebehind="FileLibrary.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockControl" src="../../Modules/BlockControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="DocumentVersions" src="DocumentVersions.ascx" %>
<table cellpadding="0" cellspacing="1" width="100%" style="margin-top:7px">
	<tr>
		<td><ibn:BlockControl id="blockControl" runat="server" /></td>
	</tr>
</table>