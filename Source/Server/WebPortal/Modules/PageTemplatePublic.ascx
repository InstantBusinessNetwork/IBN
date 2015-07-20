<?xml version="1.0" encoding="utf-8"?>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.PageTemplatePublic" Codebehind="PageTemplatePublic.ascx.cs" %>
<%@ Register TagPrefix="uc1" TagName="bc" Src="BottomCopyright.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=Title + sTitle%></title>
	<link id="iconIBN" runat="server" type='image/x-icon' rel="shortcut icon" />

	<script type="text/javascript">
	//<![CDATA[
	if (top != window)
	{
		if (location.replace)
			top.location.replace(self.location.href);
		else
			top.location.href = self.location.href;
	}
	//]]>
	</script>
</head>

<body id="body" runat="server" class="ibn-propertysheet">
	<form id="frmMain" runat="server" class="mainLayout" method="post">
		<table class="mainLayout" cellspacing="0">
			<tr class="mainHeader">
				<td colspan="2">
					<div class="ibn-bannerframe">
						<asp:Label ID="lblProductName" runat="server" CssClass="topBannerText"></asp:Label>
					</div>
					<div class="pageHeader">
						<img alt="" src="<%=Mediachase.UI.Web.Util.CommonHelper.GetCompanyLogoUrl(Page)%>" class="companyLogo" />
						<asp:Label id="lblTitle" runat="server" CssClass="ibn-pagetitle"></asp:Label>
					</div>
					<div class="ibn-titlearealine">
						<img alt="" src="../layouts/images/blank.gif" width="1" height="1" />
					</div>
				</td>
			</tr>
			<tr class="mainLayout">
				<td class="mainMenu">
					<div class="ibn-navline"><%=LocRM.GetString("Actions")%></div>
					<table class="ibn-propertysheet">
						<tr>
							<td><img alt="" src='<%=Page.ResolveUrl("~/layouts/images/rect.gif")%>' /></td>
							<td class="mainMenu"><a href="<%=Page.ResolveUrl("~/Public/default.aspx")%>"><%=LocRM.GetString("Home")%></a></td>
						</tr>
						<tr>
							<td><img alt="" src='<%=Page.ResolveUrl("~/layouts/images/rect.gif")%>' /></td>
							<td class="mainMenu"><a href="<%=Page.ResolveUrl("~/Public/SignUp.aspx")%>"><%=LocRM.GetString("SignUp")%></a></td>
						</tr>
						<tr id="tableContactUs" runat="server">
							<td><img alt="" src='<%=Page.ResolveUrl("~/layouts/images/rect.gif")%>' /></td>
							<td class="mainMenu"><a href="<%=Page.ResolveUrl("~/Public/ContactUs.aspx")%>"><%=LocRM.GetString("ContactUs")%></a></td>
						</tr>
						<tr>
							<td><img alt="" src='<%=Page.ResolveUrl("~/layouts/images/rect.gif")%>' /></td>
							<td class="mainMenu"><a href="<%=Page.ResolveUrl("~/Public/Download.aspx")%>"><%=LocRM.GetString("Download")%></a></td>
						</tr>
						<tr id="tableFeedback" runat="server">
							<td><img alt="" src='<%=Page.ResolveUrl("~/layouts/images/rect.gif")%>' /></td>
							<td class="mainMenu"><a href="<%=Page.ResolveUrl("~/Public/Help.aspx")%>"><%=LocRM.GetString("Feedback")%></a></td>
						</tr>
					</table>
				</td>
				<td class="mainContent">
					<asp:placeholder id="phMain" runat="server"></asp:placeholder>
				</td>
			</tr>
			<tr class="mainFooter">
				<td class="mainMenu" />
				<td><uc1:bc ID="bottomline" runat="server" ShowTopLine="false" /></td>
			</tr>
		</table>
		<div>
			<input id="PageX" runat="server" type="hidden" name="PageX" value="0" />
			<input id="PageY" runat="server" type="hidden" name="PageY" value="0" />
		</div>
	</form>
	<div id="printScript" runat="server">
		<script type="text/javascript">function window.onload(){window.print();}</script>
	</div>
	<script type="text/javascript">
	//<![CDATA[
		window.onscroll = function () {setcoords();}
		// Netscape submit fix
		if(browseris.nav6up)
			document.<%=frmMain.ClientID%> = document.forms['<%=frmMain.ClientID%>'];
	//]]>
	</script>
</body>
</html>
