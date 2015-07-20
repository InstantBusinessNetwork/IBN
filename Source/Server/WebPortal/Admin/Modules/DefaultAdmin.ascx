<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.DefaultAdmin" Codebehind="DefaultAdmin.ascx.cs" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%">
	<tr>
		<td valign="top">
			<asp:PlaceHolder ID="phItems" Runat="server" />
		</td>
	</tr>
</table>