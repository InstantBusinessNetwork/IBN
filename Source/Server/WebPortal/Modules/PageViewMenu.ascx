<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.PageViewMenu" Codebehind="PageViewMenu.ascx.cs" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<table cellspacing="0" cellpadding="0" class="ibn-toolbar" style="width:100%; border:0">
	<tr>
		<td class="ibn-toolbar">
			<table cellspacing="0" cellpadding="0" style="margin-left:0px; border:0">
				<tr id="trMain" runat="server" style="height:23px">
					<td style="padding-bottom:2px; white-space:nowrap">
						<asp:Label CssClass="ibn-WPTitle" runat="server" ID="lblBlockTitle" EnableViewState="False"></asp:Label>
					</td>
					<td style="width:100%"></td>
					<td class="ibn-toolbar" style="padding: 0 1 0 0; white-space:nowrap">
						<div class="text">
	<ComponentArt:Menu id="AcMenu" 
      Orientation="Horizontal"
      CssClass="BTopGroup"
      DefaultGroupCssClass="BMenuGroup"
      DefaultItemLookID="DefaultItemLook"
      DefaultGroupItemSpacing="1"
      EnableViewState="false"
      ExpandDelay="100"
      ExpandOnClick="true"
      ClientScriptLocation="~/Scripts/componentart_webui_client/"
      runat="server">
    <ItemLooks>
      <ComponentArt:ItemLook LookID="TopItemLook" CssClass="BTopMenuItem" HoverCssClass="BTopMenuItemHover" ExpandedCssClass="BTopMenuItemExpanded" LabelPaddingLeft="10" LabelPaddingRight="10" LabelPaddingTop="2" LabelPaddingBottom="2" />
      <ComponentArt:ItemLook LookID="DefaultItemLook" CssClass="BMenuItem" HoverCssClass="BMenuItemHover" ExpandedCssClass="BMenuItemHover" LeftIconWidth="20" LeftIconHeight="18" LabelPaddingLeft="10" LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="4" />
      <ComponentArt:ItemLook LookID="BreakItem" CssClass="BMenuBreak" />
    </ItemLooks>
    </ComponentArt:Menu>
						</div>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>