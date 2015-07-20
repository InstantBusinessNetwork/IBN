<%@ Control Language="c#" Inherits="Mediachase.UI.Web.FileStorage.Modules.FileLibrary" Codebehind="FileLibrary.ascx.cs" %>
<%@ Reference Control="~/Modules/TopTabs.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ctrl" TagName="TopTab" src="../../Modules/TopTabs.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/BlockHeaderLightWithMenu.ascx" %>
<ctrl:TopTab id="ctrlTopTab" runat="server" />
<table cellpadding="7" cellspacing="0" border="0" width="100%" class="ibn-WPBorder">
	<tr>
		<td valign="top" align=center style="PADDING-RIGHT:6px">
			<ibn:blockheader id="tbLightFS" runat="server"></ibn:blockheader>
			<table cellpadding="3" cellspacing="3" border="0" width="100%" class="text ibn-stylebox-light">
			<tr><td>
			<asp:PlaceHolder ID="phItems" Runat="server" />
			</td></tr></table>
		</td>
	</tr>
</table>