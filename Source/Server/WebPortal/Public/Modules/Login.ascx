<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Public.Modules.Login" Codebehind="Login.ascx.cs" %>
<script type="text/javascript">
	//<![CDATA[
	function LoginFocusElement(elId) {
		var elem = document.getElementById(elId);
		if (!elem)
			return;
		elem.focus();
	}

	function mcInnerValidateLogin() {
		var obj1 = document.getElementById('<%= tbLogin.ClientID %>');
		var obj2 = document.getElementById('<%= tbPassword.ClientID %>');
		if (obj1.value == '') { document.getElementById('<%= lblLogin.ClientID %>').innerHTML = "*"; return false; }
		if (obj2.value == '') { document.getElementById('<%= lblPassword.ClientID %>').innerHTML = "*"; return false; }
		return true;
	}
	function mcInnerValidationClear() {
		document.getElementById('<%= lblLogin.ClientID %>').innerHTML = "";
		document.getElementById('<%= lblPassword.ClientID %>').innerHTML = "";
	}
	//]]>
</script>
<table width="100%" border="0" cellspacing="0" cellpadding="0" class="text">
	<tr>
		<td style="width: 250px; vertical-align: top">
			<table border="0" cellpadding="0" cellspacing="7" class="ibn-ToolPaneFrame">
				<tr class="text">
					<td colspan="2" valign="top"><strong><%=LocRM.GetString("PortalLogin")%></strong></td>
				</tr>
				<tr class="text">
					<td valign="top"><label for='<%=tbLogin.ClientID%>'><%=LocRM.GetString("Login")%></label></td>
					<td>
						<div style="white-space:nowrap">
							<asp:textbox ID="tbLogin" runat="server" CssClass='text' Width="100px" />
							<asp:Label ID="lblPortal" runat="server" CssClass="boldtext" />
							<asp:Label ID="lblLogin" runat="server" CssClass="ibn-alerttext" Width="20px" />
						</div>
					</td>
				</tr>
				<tr class="text">
					<td valign="top"><label for='<%=tbPassword.ClientID%>'><%=LocRM.GetString("Password")%></label></td>
					<td>
						<asp:textbox ID="tbPassword" runat="server" CssClass="text" width="100px" textmode="Password"></asp:textbox>
						<asp:Label ID="lblPassword" runat="server" CssClass="ibn-alerttext"/>
					</td>
				</tr>
				<tr class="text">
					<td>&nbsp;</td>
					<td><asp:checkbox id="cbRemember" runat="server" text="Remember Me" cssclass="text" checked="true"></asp:checkbox></td>
				</tr>
				<tr>
					<td>&nbsp;</td>
					<td><asp:button OnClientClick="mcInnerValidationClear(); return mcInnerValidateLogin()" id="btnLogin" runat="server" cssclass="text" text="Login" width="100" onclick="btnLogin_Click"></asp:button></td>
				</tr>
				<tr>
					<td colspan="2"><asp:hyperlink cssclass="text" runat="server" id="lbForgotPass" navigateurl="../Forgot.aspx"></asp:hyperlink></td>
				</tr>
			</table>
			<p class="UserGenericHeader"><%=LocRM.GetString("RealTimeServices")%></p><hr style="color:#aaa;" />
			<div style="padding-bottom:7px;"><asp:hyperlink id="hlDownload" cssclass="ibn-sectionheader" runat="server" navigateurl="../download.aspx">
				<img align="right" alt="" border="0" height="94" hspace="1" src="../Layouts/images/client-image.gif" vspace="1" width="52" /></asp:hyperlink><br />
			<%=LocRM.GetString("DownloadText")%></div>
			<div style="padding-bottom:7px;"><asp:hyperlink id="hlScreenCapture" cssclass="ibn-sectionheader" runat="server" navigateurl="../download.aspx">
				<img align="right" alt="" border="0" height="94" hspace="1" src="../Layouts/images/client-image.gif" vspace="1" width="52" /></asp:hyperlink><br />
			<asp:Label ID="lblScreenText" runat="server"></asp:Label></div>
			<div style="padding-bottom:7px;"><asp:hyperlink id="hlPluginIE" cssclass="ibn-sectionheader" runat="server" navigateurl="../download.aspx">
				<img align="right" alt="" border="0" height="94" hspace="1" src="../Layouts/images/client-image.gif" vspace="1" width="52" /></asp:hyperlink><br />
			<%=LocRM.GetString("DownloadTextPlugIE")%></div>
			<div style="padding-bottom:7px;"><asp:hyperlink id="hlPluginFF" cssclass="ibn-sectionheader" runat="server" navigateurl="../download.aspx">
				<img align="right" alt="" border="0" height="94" hspace="1" src="../Layouts/images/client-image.gif" vspace="1" width="52" /></asp:hyperlink><br />
			<%=LocRM.GetString("DownloadTextPlugFF")%></div>
			<div style="padding-bottom:7px;"><asp:hyperlink id="hlPluginOut" cssclass="ibn-sectionheader" runat="server" navigateurl="../download.aspx">
				<img align="right" alt="" border="0" height="94" hspace="1" src="../Layouts/images/client-image.gif" vspace="1" width="52" /></asp:hyperlink><br />
			<%=LocRM.GetString("DownloadTextPlugOut")%></div>
			<asp:hyperlink id="hlToolBox" cssclass="ibn-sectionheader" runat="server" navigateurl="../download.aspx">
				<img align="right" alt="" border="0" height="94" hspace="1" src="../Layouts/images/client-image.gif" vspace="1" width="52" /></asp:hyperlink><br />
			<%=LocRM.GetString("DownloadTextToolBox")%>
			<p><asp:hyperlink id="Hyperlink1" cssclass="Text" runat="server" navigateurl="../download.aspx"></asp:hyperlink></p>
			<input id="hdnOffset" type="hidden" runat="server" value="111" />
		</td>
		<td style="width: 70px">&nbsp;</td>
		<td valign="top">
			<img alt="Company Identity" src="../Common/CompanyIdentity.aspx" />
			<p class="UserGenericHeader"><asp:label id="lblTitle1" runat="server"></asp:label></p>
			<p><asp:label id="lblHPText1" runat="server" cssclass="text"></asp:label></p>
			<p class="UserGenericHeader"><asp:label id="lblTitle2" runat="server"></asp:label></p>
			<p><asp:label id="lblHPText2" runat="server" cssclass="text"></asp:label></p>
		</td>
	</tr>
</table>