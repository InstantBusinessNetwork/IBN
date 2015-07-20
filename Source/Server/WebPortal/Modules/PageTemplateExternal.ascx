<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.PageTemplateExternal" Codebehind="PageTemplateExternal.ascx.cs" %>
<%@ register TagPrefix="mc" namespace="Mediachase.UI.Web.Modules" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="mc2" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head runat="server">
		
		<title><%= Title + sTitle %></title>
		<link id="iconIBN" runat="server" type='image/x-icon' rel="shortcut icon" />

	</head>
	<body class="UserBackground" leftmargin="0" topmargin="0" marginheight="0" marginwidth="0" id="body" runat="server">
		<form id="frmMain" method="post" runat="server" onkeypress="disableEnterKey(event)">
		<asp:ScriptManager ID="sm1" runat="server" EnablePartialRendering="true" ScriptMode="Release" EnableScriptGlobalization = "true" EnableScriptLocalization="true">
		</asp:ScriptManager>
		<table height="100%" border="0" cellpadding="0" cellspacing="0" width="100%"><tr><td valign="top">
			<table cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td>
						<table width="100%" class="ibn-bannerframe" cellspacing="0" cellpadding="3">
							<tr>
								<td valign="middle" nowrap="nowrap" width="22" style="padding-left:2">
									<img alt="" id="onetidHeadbnnr0" src="../layouts/images/ibn_logo.gif" width="16" height="16"></td>
								<td class="ibn-banner" valign="middle" nowrap="nowrap" width="99%" style="color:#FFFFFF">
									<b><asp:Label ID="lblProductName" runat="server"></asp:Label></b>
								</td>
								<td class="ibn-banner" style="PADDING-RIGHT: 7px;color:#FFFFFF" nowrap="nowrap">&nbsp;
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			
			<table cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td class="ibn-titleareaframe" colspan="3">
						<div class="ibn-titleareaframe">
							<table class="ibn-titleareaframe" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td style="PADDING-BOTTOM: 0px">
										<table style="PADDING-LEFT: 2px; PADDING-TOP: 0px" cellspacing="0" cellpadding="0" border="0" width="100%">
											<tr>
												<td style="PADDING-TOP: 4px" nowrap="nowrap" align="center" width="132" height="50">
													<img alt="" src="<%=Mediachase.UI.Web.Util.CommonHelper.GetCompanyLogoUrl(Page)%>" width="100" height="50">
												</td>
												<td width="12"><img height="1" alt="" src="../layouts/images/blank.gif" width="7"></td>
												<td style="PADDING-TOP: 2px" nowrap="nowrap">
													<table cellspacing="0" cellpadding="0" border="0">
														<tr>
															<td class="ibn-pagetitle" id="onetidPageTitle">
																<asp:label runat="server" id="lblTitle"></asp:label>
															</td>
														</tr>
													</table>
												</td>
												<td valign="bottom" align="right" style="PADDING-RIGHT:5px; PADDING-BOTTOM:3px">
													<asp:label runat="server" id="lblUser" cssclass="ibn-descriptiontext"></asp:label><br>
<span id="timeSpan" class="ibn-propertysheet"></span><asp:Label ID="lblTime" Runat="server" cssclass="ibn-propertysheet"></asp:Label>
												</td>
											</tr>
										</table>
										<table cellspacing="0" cellpadding="0" width="100%" border="0">
											<tr>
												<td colspan="5" height="2"><img height="1" alt="" src="../layouts/images/blank.gif" width="1"></td>
											</tr>
											<tr>
												<td class="ibn-titlearealine" colspan="5" height="1"><img height="1" alt="" src="../layouts/images/blank.gif" width="1"></td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</div>
					</td>
				</tr>
			</table>
			
			<table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0" style="TABLE-LAYOUT: fixed">
				<col width="125">
				<col width="5">
				<col>

				<tr valign="top" height="100%">
					<td class="ibn-nav" valign="top" height="100%" width="120">
						<!-- Left Navigation -->
						<table class="ibn-navframe" style="PADDING-TOP: 8px" height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td valign="top" width="4"><img height="1" alt="" src="../layouts/images/blank.gif" width="1"></td>
								<td class="ibn-viewselect" id="onetidSelectView" valign="top">
									<table style="MARGIN-LEFT: 3px" cellspacing="2" cellpadding="0" width="115" border="0">
										<tr>
											<td id="L_SelectView" width="100%"><%=LocRM.GetString("Actions") %></td>
										</tr>
										<tr>
											<td class="ibn-navline"><img height="1" alt="" src="../layouts/images/blank.gif" width="1"></td>
										</tr>
									</table>
									<table width="100%" cellpadding="1" cellspacing="0" border="0" style="MARGIN-TOP:5px" id="tableContactUs" runat="server">
										<tr>
											<td style="PADDING-LEFT: 2px; PADDING-BOTTOM: 2px" width="100%">
												<table cellspacing="0" cellpadding="0" width="100%" border="0">
													<tr>
														<td class="ibn-propertysheet" width="100%" colspan="2">
															<table cellspacing="0" cellpadding="0" border="0">
																<tr>
																	<td valign="top"><img alt="" src="../layouts/images/rect.gif">&nbsp;
																	</td>
																	<td><a class="boldtext" href='../Public/ContactUs.aspx'><%=LocRM.GetString("ContactUs") %></a>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
									<table width="100%" cellpadding="1" cellspacing="0" border="0" style="MARGIN-TOP:5px">
										<tr>
											<td style="PADDING-LEFT: 2px; PADDING-BOTTOM: 2px" width="100%">
												<table cellspacing="0" cellpadding="0" width="100%" border="0">
													<tr>
														<td class="ibn-propertysheet" width="100%" colspan="2">
															<table cellspacing="0" cellpadding="0" border="0">
																<tr>
																	<td valign="top"><img alt="" src="../layouts/images/rect.gif">&nbsp;
																	</td>
																	<td><a class="boldtext" href='../Public/Download.aspx'><%=LocRM.GetString("Download") %></a>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<!-- End Left Navigation -->
					</td>
					<!-- Contents -->
					<td width="5"><img height="1" alt="" src="../layouts/images/blank.gif" width="5"></td>
					<td class="ibn-bodyareaframe" valign="top" width="100%">
						<table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px; PADDING-TOP: 5px" valign="top" height="100%">
									<asp:placeholder id="phMain" runat="server"></asp:placeholder>
								</td>
							</tr>
						</table>
						<!-- FooterBanner closes the TD, TR, TABLE, BODY, And HTML regions opened above -->
						&nbsp; 
						<!-- Close the TD, TR, TABLE, BODY, And HTML from Header -->
					</td>
				</tr>
			</table>
			<input type="hidden" id="PageX" name="PageX" value="0" runat="server"> <input type="hidden" id="PageY" name="PageY" value="0" runat="server">
			</td></tr></table>
			<mc2:CommandManager ID="cm" runat="server" ContainerId="divContainer" UsePageHeaderForStyles="true" />
			<div id="divContainer" runat="server"></div>
		</form>
		<div id="printScript" runat="server">
			<script type="text/javascript">function window.onload(){window.print();}</script>
		</div>
		<script type="text/javascript">
		//<![CDATA[
		// Netscape submit fix
		if(browseris.nav6up)
			document.<%=frmMain.ClientID%> = document.forms['<%=frmMain.ClientID%>'];
		
		var Is24Hours = <%=(Is24Hours=="1")? "true" : "false" %>;
		var TimeOffset = <%=TimeOffset %>;
		
		function showTheHours(theHour) 
		{
			if (Is24Hours || (theHour > 0 && theHour < 13)) {
				return (theHour)
			}
			if (theHour == 0) {
				return (12)
			}
			return (theHour-12) 
		}
		
		function showZeroFilled(inValue) 
		{
			if (inValue > 9) {
				return ":" + inValue
			}
			return ":0" + inValue
		}
		
		function showAmPm(theHour) 
		{
			if (Is24Hours) {
				return ("")
			}
		
			if (theHour < 12) {
				return (" AM")
			}
			return (" PM")
		}
		
		function showTheTime() {
			var now = new Date();
			
			var cHours = now.getUTCHours();
			var cMinutes = now.getUTCMinutes();
			
			var MinutesOffset = cHours*60 + cMinutes - TimeOffset;
			var Hours = Math.floor(MinutesOffset / 60);
			var Minutes = MinutesOffset - Hours * 60;
			
			if (Hours > 23) Hours = Hours - 24;
			if (Hours < 0 ) Hours = 24 + Hours;
			
			var spn = document.getElementById('timeSpan');
			spn.innerHTML = showTheHours(Hours) + showZeroFilled(Minutes) + showAmPm(Hours)
			setTimeout("showTheTime()",10000)
		}
		
		setTimeout("showTheTime()",100)
		//]]>
		</script>
	</body>
</html>
