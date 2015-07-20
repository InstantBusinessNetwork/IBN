<?xml version="1.0" encoding="utf-8"?>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.PageTemplatePublic" CodeBehind="PageTemplate.ascx.cs" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.WebAsp.Modules" Assembly="Mediachase.Ibn.WebAsp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<title><%=Title%> | Instant Business Network</title>

	<asp:PlaceHolder ID="phPrintScripts" runat="server">

		<script type="text/javascript">
			//<![CDATA[
			function disableEnterKey() {
				try {
					if (window.event.keyCode == 13 && window.event.srcElement.type != "textarea")
						window.event.keyCode = 0;
				}
				catch (e) { }
			}

			function BeforePrint() {
				var coll = document.all;
				if (coll != null) {
					for (var i = 0; i < coll.length; i++) {
						if (coll[i].Printable == "0")
							coll[i].style.display = "none";
						if (coll[i].Printable == "1")
							coll[i].style.display = "block";
					}
				}
			}

			function AfterPrint() {
				var coll = document.all;
				if (coll != null) {
					for (var i = 0; i < coll.length; i++) {
						if (coll[i].Printable == "0")
							coll[i].style.display = "block";
						if (coll[i].Printable == "1")
							coll[i].style.display = "none";
					}
				}
			}
			//]]>
		</script>

	</asp:PlaceHolder>
</head>
<body id="body" runat="server" class="UserBackground">

	<script type="text/javascript">
		//<![CDATA[

		if (browseris.ie5up) {
			window.onbeforeprint = BeforePrint;
			window.onafterprint = AfterPrint;
		}
		//]]>
	</script>

	<mc:FixedForm ID="frmMain" method="post" runat="server" onkeypress="disableEnterKey()">
		<div class="header1">
			<span><img alt="" src="../layouts/images/ibn_logo.gif" /></span>
			<span><%=LocRM.GetString("IBN")%></span>
		</div>
		<table class="header2" cellspacing="0">
			<tr>
				<td>
					<img alt="" src="../Layouts/Images/Home.gif" />
				</td>
				<td>
					<table cellspacing="0">
						<tr>
							<td class="ibn-titlearea">
								<asp:Label ID="lblSiteName" runat="server"></asp:Label>
							</td>
						</tr>
						<tr>
							<td class="ibn-pagetitle">
								<asp:Label ID="lblTitle" runat="server"></asp:Label>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
		<table class="mainLayout" cellspacing="0">
			<tr>
				<td class="mainMenu ibn-propertysheet">
					<div class="mainMenu-Item">
						Search:<br/>
						<asp:TextBox ID="tbSearchstr" runat="server" CssClass="text" Width="90px"></asp:TextBox>
						<asp:ImageButton ID="btnSearch" runat="server" ImageUrl="../layouts/images/cancel.gif" CausesValidation="False"></asp:ImageButton>
					</div>
					<div class="mainMenu-Separator"></div>
					<div class="mainMenu-Item">
						<img alt="" src="../Layouts/Images/Rect.gif" />
						<a href="../Pages/AspHome.aspx">Home Page</a>
					</div>
					<div class="mainMenu-Separator"></div>
					<div class="mainMenu-Item">
						<img alt="" src="../Layouts/Images/Rect.gif" />
						<a href="../pages/sites.aspx"><%=LocRM.GetString("ManageSites") %></a>
					</div>
					<div class="mainMenu-Item">
						<img alt="" src="../Layouts/Images/Rect.gif" />
						<a href="../pages/TrialRequests.aspx"><%=LocRM.GetString("TrialReqs") %></a>
					</div>
					<div class="mainMenu-Item">
						<img alt="" src="../Layouts/Images/Rect.gif" />
						<a href="../pages/Settings.aspx"><%=LocRM.GetString("Settings") %></a>
					</div>
					<div class="mainMenu-Item">
						<img alt="" src="../Layouts/Images/Rect.gif" />
						<a href="../pages/Resellers.aspx">Resellers</a>
					</div>
					<div class="mainMenu-Item">
						<img alt="" src="../Layouts/Images/Rect.gif" />
						<a href="../pages/TrialTemplates.aspx">Notification Templates</a>
					</div>
					<div class="mainMenu-Item">
						<img alt="" src="../Layouts/Images/Rect.gif" />
						<a href="../Pages/LoginUser.aspx">Login on Behalf of User</a>
					</div>
					<div class="mainMenu-Separator"></div>
					<div class="mainMenu-Item">
						<img alt="" src="../Layouts/Images/Rect.gif" />
						<a href="../pages/ASPHome.aspx?Tab=1">Reports</a>
					</div>
					<div class="mainMenu-Separator"></div>
					<div class="mainMenu-Item">
						<img alt="" src="../Layouts/Images/Rect.gif" />
						<a href="../pages/ASPHome.aspx?Tab=4">Tariffs</a>
					</div>
					<div class="mainMenu-Item">
						<img alt="" src="../Layouts/Images/Rect.gif" />
						<a href="../pages/Payments.aspx">Payments</a>
					</div>
					<div class="mainMenu-Separator"></div>
					<div class="mainMenu-Item">
						<img alt="" src="../Layouts/Images/Rect.gif" />
						<a href="http://www.mediachase.com" target="_blank"><%=LocRM.GetString("ContactUs") %></a>
					</div>
				</td>
				<td class="mainContent">
					<asp:PlaceHolder ID="phMain" runat="server"></asp:PlaceHolder>
				</td>
			</tr>
		</table>
		<div>
			<input id="PageX" type="hidden" value="0" name="PageX" runat="server" />
			<input id="PageY" type="hidden" value="0" name="PageY" runat="server" />
		</div>
	</mc:FixedForm>
	<div id="printScript" runat="server">

		<script type="text/javascript">
			//<![CDATA[
			function window.onload() { window.print(); }
			//]]>
		</script>

	</div>

	<script type="text/javascript">
		//<![CDATA[
		window.onscroll = function() { setcoords(); }
		// Netscape submit fix
		if (browseris.nav6up)
			document.<%=frmMain.ClientID%> = document.forms["<%=frmMain.ClientID%>"];
		//]]>
	</script>

</body>
</html>
