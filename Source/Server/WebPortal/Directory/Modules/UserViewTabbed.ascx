<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Reference Control="~/Directory/Modules/UserView.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Reference Control="~/Modules/TopTabs.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.UserViewTabbed" Codebehind="UserViewTabbed.ascx.cs" %>
<%@ Register TagPrefix="ctrl" TagName="TopTab" src="../../Modules/TopTabs.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageViewMenu" src="../../Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="UserShortInfo" src="UserShortInfo.ascx" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" style="width:100%">
	<tr>
		<td>
			<ibn:PageViewMenu id="secHeader" title="" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="ibn-light">
			<ibn:UserShortInfo id="ucUserShortInfo" title="" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="ibn-light">
			<ctrl:TopTab id="ctrlTopTab" runat="server" BaseUrl="UserView.aspx" />
		</td>
	</tr>
	<tr>
		<td style="padding-left:7px; padding-right:7px; padding-bottom:7px">
			<asp:PlaceHolder ID="phItems" Runat="server" />
		</td>
	</tr>
</table>
<asp:Button ID="AddToFavoritesButton" Runat="server" Visible="False" onclick="AddToFavoritesButton_Click" />