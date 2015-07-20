<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.MyWork" Codebehind="MyWork.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockControl" src="~/Modules/BlockControl.ascx" %>
<table cellpadding="0" cellspacing="7" width="100%">
	<tr>
		<td><ibn:BlockControl id="blockControl" runat="server" /></td>
	</tr>
</table>
