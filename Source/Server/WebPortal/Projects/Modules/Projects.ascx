<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.Projects" Codebehind="Projects.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\PageViewMenu.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="MARGIN-TOP:0px;margin-left:2px">
	<tbody>
		<tr>
			<td class="ms-toolbar">
				<ibn:blockheader id="secHeader" runat="server" title="ToolBar" />
			</td>
		</tr>
		<tr>
			<td>
				<asp:PlaceHolder ID="phItems" Runat="server" />
			</td>
		</tr>
	</tbody>
</table>
