<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.BlockHeaderLightWithMenu" Codebehind="BlockHeaderLightWithMenu.ascx.cs" %>
<%@ Import Namespace="Mediachase.UI.Web.Util" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td valign="bottom" id="tdLeftCorner" runat="server"><img alt=' ' runat="server" id="LeftCornerImage" width="11" height="20"/></td>
		<td runat="server" id="CollapsingCell" style="width:16px;">
			<img runat="server" id="CollapsingImage" width="16" height="16" style="vertical-align:middle; cursor:pointer;" alt=''/>
		</td>
		<td runat="server" id="tdLeftItems"></td>
		<td style='background-image: url(<%=Page.ResolveUrl("~/Images/IbnFramework/linehz.gif")%>);background-repeat: repeat-x; background-position-y: bottom; width:100%'></td>
		<td runat="server" id="tdRightItems"></td>
		<td runat="server" id="tdMenu">
			<ComponentArt:Menu id="AcMenu" 
				Orientation="Horizontal"
				CssClass="BTopGroup"
				DefaultGroupCssClass="BMenuGroup"
				DefaultItemLookID="DefaultItemLook"
				DefaultGroupItemSpacing="1"
				EnableViewState="false"
				ExpandDelay="100"
				ExpandOnClick="true"
				runat="server">
			<ItemLooks>
				<ComponentArt:ItemLook LookID="TopItemLook" CssClass="BTopMenuItem" HoverCssClass="BTopMenuItemHover" ExpandedCssClass="BTopMenuItemExpanded" LabelPaddingLeft="10" LabelPaddingRight="10" LabelPaddingTop="2" LabelPaddingBottom="2" />
				<ComponentArt:ItemLook LookID="DefaultItemLook" CssClass="BMenuItem" HoverCssClass="BMenuItemHover" ExpandedCssClass="BMenuItemHover" LeftIconWidth="20" LeftIconHeight="18" LabelPaddingLeft="10" LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="4" />
				<ComponentArt:ItemLook LookID="BreakItem" CssClass="BMenuBreak" />
			</ItemLooks>
			</ComponentArt:Menu>
		</td>
		<td valign="bottom" id="tdRightCorner" runat="server"><img alt=' ' runat="server" id="RightCornerImage" width="11" height="20" /></td>
	</tr>
</table>
<input type="hidden" runat="server" id="CollapsedValue" value="0" />
