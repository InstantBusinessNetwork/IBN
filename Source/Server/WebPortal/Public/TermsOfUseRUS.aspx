<?xml version="1.0" encoding="utf-8" ?>
<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Public.TermsOfUseRUS" CodeBehind="TermsOfUseRUS.aspx.cs" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=LocRM.GetString("TermsOfUse")%></title>
</head>
<body class="UserBackground" style="background-color: #ECE9D8">
	<table class="mainLayout" cellspacing="0" style="background-color: #ECE9D8">
		<tr>
			<td class="mainContent">
				<ComponentArt:TabStrip ID="TabStrip1" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook" DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook" DefaultGroupTabSpacing="1" ImagesBaseUrl="../layouts/Images/" MultiPageId="MultiPage1" runat="server">
					<ItemLooks>
						<ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover" LabelPaddingLeft="10" LabelPaddingRight="10" LabelPaddingTop="5" LabelPaddingBottom="4" LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif" HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3" LeftIconHeight="21" RightIconWidth="3" RightIconHeight="21" />
						<ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedTab" LabelPaddingLeft="10" LabelPaddingRight="10" LabelPaddingTop="4" LabelPaddingBottom="4" LeftIconUrl="selected_tab_left_icon.gif" RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3" LeftIconHeight="21" RightIconWidth="3" RightIconHeight="21" />
					</ItemLooks>
				</ComponentArt:TabStrip>
				<ComponentArt:MultiPage ID="MultiPage1" CssClass="MultiPage" runat="server" Width="100%" Height="100%">
					<ComponentArt:PageView CssClass="text" runat="server" ID="Pageview1">
						<table cellspacing="0" cellpadding="4" border="0">
							<tr>
								<td class="text">
									<div id="lblTrialVersion" runat="server" class="text">
									</div>
								</td>
							</tr>
						</table>
					</ComponentArt:PageView>
					<ComponentArt:PageView CssClass="text" runat="server" ID="Pageview2">
						<table cellspacing="0" cellpadding="4" border="0">
							<tr>
								<td class="text">
									<div id="lblBillableVersion" runat="server" class="text">
									</div>
								</td>
							</tr>
						</table>
					</ComponentArt:PageView>
				</ComponentArt:MultiPage>
			</td>
		</tr>
		<tr class="mainFooter">
			<td>
				<div style="text-align: center; padding-bottom: 5px">
					<input type="button" value='<%=LocRM.GetString("Close") %>' onclick="window.close();" class="text" />
				</div>
			</td>
		</tr>
	</table>
</body>
</html>
