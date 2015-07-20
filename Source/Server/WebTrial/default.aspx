<%@ Page validateRequest=false language="c#" Inherits="Mediachase.Ibn.WebTrial.defaultpage" Codebehind="default.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="AntiRobot" src="AntiRobotControl.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<HTML>
	<HEAD>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta name="description" content='<%= LocRM.GetString("strMetaDescr") %>'>
		<title><%= LocRM.GetString("strTitle") %></title>
		<script type="text/javascript" src='<%=ResolveUrl("~/layouts/buttons.js") %>'></script>
		<script type="text/javascript" src='<%=ResolveUrl("~/layouts/browser.js") %>'></script>
		<link href='<%=ResolveUrl("~/layouts/windows.css") %>' type=text/css rel=stylesheet>
		<link href='<%=ResolveUrl("~/layouts/XP/theme.css") %>' rel=stylesheet>
		<link href='<%=ResolveUrl("~/layouts/XP/color1.css") %>' rel=stylesheet>
		<style type="text/css">
			.ibn-navMy {
				text-decoration: none;
				font-family: Verdana, sans-serif; 
				font-size: 1.5em; 
				font-weight: bold; 
				color: #003399;
				cursor: pointer;}
		</style>
		<script src="http://www.google-analytics.com/urchin.js" type="text/javascript">
		</script>
		<script type="text/javascript">
			_uacct = "UA-490257-2";
			urchinTracker();
		</script>
		<script type="text/javascript">
		<!--
		if ((browseris.mac) && !browseris.ie5up)
		{
			var macstyle = "../layouts/<%=System.Threading.Thread.CurrentThread.CurrentUICulture.Name%>/styles/mac.css";
			document.write("<link rel='stylesheet' Type='text/css' href='" + macstyle + "'>");
		}
		//-->
		</script>
		<script type="text/javascript">
			function FocusElement(elementId)
			{
				var element = document.getElementById(elementId);
				if(element)
					element.focus();
			}
		</script>
	</HEAD>
	<body>
		<form id="mcSignupForm" method="post" runat="server">
			<table cellspacing="0" cellpadding="3" width="725" border="0" align="center" class="text">
				<tr>
					<td>
						<asp:image id="imgHeader" EnableViewState="False" ImageUrl="Layouts/ibn2003_masthead.gif" BorderWidth="0" Runat="server"></asp:image>
						<br><br>
						<table border="0" width="100%" cellpadding="0" cellspacing="0" class="text">
							<tr>
								<td>
									<div class="ibn-sectionheader" style="DISPLAY: inline">
										<%= LocRM.GetString("NoObligation1") %><%= Mediachase.Ibn.WebTrial.TrialHelper.GetTrialPeriod()%><%= LocRM.GetString("NoObligation2") %>
									</div>
								</td>
								<td align="right">
									<span class="text" id="spanLanguage" runat="server">
									<%= LocRM.GetString("SelectLanguage") %>:
									<asp:DropDownList ID="ddLanguage" tabIndex="1" Runat="server" AutoPostBack="True" onselectedindexchanged="ddLanguage_change">
										<asp:ListItem Value="ru-RU">Russian</asp:ListItem>
										<asp:ListItem Value="en-US">English</asp:ListItem>
									</asp:DropDownList>
									</span>
								</td>
							</tr>
						</table>
						<BR>
					</td>
				</tr>
				<tr id="tr2" runat="server">
					<td>
						<div class="ibn-sectionheader" style="DISPLAY: inline">
							<%= LocRM.GetString("Step1") %>
						</div><BR><br>
						<table class="ibn-propertysheet" style="background-color: whitesmoke" cellspacing="0" cellpadding="4" width="100%" border="0">
							<tr>
								<td vAlign="top" align="right" width="170"><b>&nbsp;<%= LocRM.GetString("Name") %>:</b>
								</td>
								<td vAlign="top" width="10"><asp:requiredfieldvalidator id="Requiredfieldvalidator2" runat="server" ControlToValidate="tbLatName" ErrorMessage="*" CssClass="boldtext" Display="Dynamic"></asp:requiredfieldvalidator></td>
								<td><asp:textbox id="tbFirstName" tabIndex="2" runat="server" CssClass="text" Width="150px"></asp:textbox>&nbsp;&nbsp;&nbsp;&nbsp;<asp:requiredfieldvalidator id="Requiredfieldvalidator1" runat="server" ControlToValidate="tbFirstName" ErrorMessage="*" CssClass="boldtext" Display="Dynamic"></asp:requiredfieldvalidator>
									<asp:textbox id="tbLatName" tabIndex="3" runat="server" CssClass="text" Width="150px"></asp:textbox><BR>
								</td>
							</tr>
							<tr>
								<td vAlign="top" align="right" width="170"><b><%= LocRM.GetString("IBNPortalName") %>:</b>
								</td>
								<td vAlign="top" width="10"><asp:requiredfieldvalidator id="rf1" runat="server" ControlToValidate="tbPortalName" ErrorMessage="*" CssClass="textbold"></asp:requiredfieldvalidator></td>
								<td><asp:textbox id="tbPortalName" tabIndex="4" runat="server" CssClass="text" Width="320px"></asp:textbox>
								</td>
							</tr>
							<tr>
								<td vAlign="top" align="right"><STRONG><%= LocRM.GetString("Country") %>:</STRONG></td>
								<td></td>
								<td><select id="ddCountry" tabIndex="5" name="ddCountry" style="width:320px" runat="server">
										<option value="1" selected>Albania</option>
										<option value="71">Argentina</option>
										<option value="69">Armenia</option>
										<option value="3">Australia</option>
										<option value="4">Austria</option>
										<option value="68">Azerbaijan</option>
										<option value="5">Bahamas</option>
										<option value="62">Belarus</option>
										<option value="6">Belgium</option>
										<option value="7">Bosnia and Herzegovina</option>
										<option value="8">Brazil</option>
										<option value="9">Bulgaria</option>
										<option value="10">Canada</option>
										<option value="79">Chile</option>
										<option value="74">China</option>
										<option value="11">Croatia</option>
										<option value="12">Czech Republic</option>
										<option value="13">Denmark</option>
										<option value="77">Egypt</option>
										<option value="65">Estonia</option>
										<option value="14">Finland</option>
										<option value="15">France</option>
										<option value="67">Georgia</option>
										<option value="16">Germany</option>
										<option value="17">Greece</option>
										<option value="18">Greenland</option>
										<option value="19">Hong Kong</option>
										<option value="20">Hungary</option>
										<option value="21">Iceland</option>
										<option value="22">India</option>
										<option value="23">Indonesia</option>
										<option value="24">Ireland</option>
										<option value="25">Israel</option>
										<option value="26">Italy</option>
										<option value="27">Jamaica</option>
										<option value="28">Japan</option>
										<option value="66">Kazakhstan</option>
										<option value="29">Korea, Democratic People's Republic of</option>
										<option value="30">Korea, Republic of</option>
										<option value="31">Kuwait</option>
										<option value="64">Latvia</option>
										<option value="32">Liechtenstein</option>
										<option value="63">Lithuania</option>
										<option value="33">Luxembourg</option>
										<option value="34">Malaysia</option>
										<option value="35">Mexico</option>
										<option value="70">Moldova</option>
										<option value="36">Monaco</option>
										<option value="37">Morocco</option>
										<option value="38">Mozambique</option>
										<option value="39">Nepal</option>
										<option value="40">Netherlands</option>
										<option value="41">New Zealand</option>
										<option value="42">Norway</option>
										<option value="43">Poland</option>
										<option value="44">Portugal</option>
										<option value="45">Puerto Rico</option>
										<option value="59">Qatar</option>
										<option value="72">Romania</option>
										<option value="46">Russian Federation</option>
										<option value="47">Rwanda</option>
										<option value="48">San Marino</option>
										<option value="75">Saudi Arabia</option>
										<option value="60">Serbia and Montenegro</option>
										<option value="49">Singapore</option>
										<option value="50">Slovakia</option>
										<option value="73">Slovenia</option>
										<option value="51">South Africa</option>
										<option value="52">Spain</option>
										<option value="53">Sweden</option>
										<option value="54">Switzerland</option>
										<option value="78">Tunisia</option>
										<option value="55">Turkey</option>
										<option value="61">Ukraine</option>
										<option value="76">United Arab Emirates</option>
										<option value="56">United Kingdom</option>
										<option value="57">United States</option>
										<option value="58">Virgin Islands, British</option>
										<option value="80">Virgin Islands, U.S.</option>
									</select>
								</td>
							</tr>
							<tr>
								<td vAlign="top" align="right"><STRONG><%= LocRM.GetString("TimeZone") %>:</STRONG></td>
								<td></td>
								<td><asp:DropDownList ID="ddTimeZone" tabIndex="6" CssClass="text" Runat="server"></asp:DropDownList></td>
							</tr>
							<tr>
								<td vAlign="top" align="right"><STRONG><%= LocRM.GetString("Phone") %>:</STRONG>
								</td>
								<td vAlign="top">
									<asp:regularexpressionvalidator id="Regularexpressionvalidator4" runat="server" ErrorMessage="*" ControlToValidate="tbPhone" Display="Dynamic" ValidationExpression="^\+?(?:\d|\s|(?:\(\d+\))|-)+$"></asp:regularexpressionvalidator></td>
								<td><asp:textbox id="tbPhone" tabIndex="7" runat="server" CssClass="text" Width="320px"></asp:textbox></td>
							</tr>
							<tr>
								<td vAlign="top" align="right" width="170"><STRONG><%= LocRM.GetString("Email") %></STRONG></td>
								<td><asp:regularexpressionvalidator id="Regularexpressionvalidator2" runat="server" ControlToValidate="tbEmail" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:regularexpressionvalidator><asp:requiredfieldvalidator id="Requiredfieldvalidator3" runat="server" ControlToValidate="tbEmail" ErrorMessage="*" CssClass="boldtext" Display="Dynamic"></asp:requiredfieldvalidator></td>
								<td><asp:textbox id="tbEmail" tabIndex="8" runat="server" CssClass="text" Width="320px"></asp:textbox></td>
							</tr>
							<tr>
								<td colspan="3"><i><%= LocRM.GetString("strAttention") %></i></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr id="tr1" runat="server">
					<td>
						<br>
						<div class="ibn-sectionheader" style="DISPLAY: inline">
							<%= LocRM.GetString("Step2") %>
						</div><BR><br>
						<table class="ibn-propertysheet" style="BACKGROUND-COLOR: whitesmoke" cellspacing="0" cellpadding="4" width="100%" border="0">
							<tr>
								<td vAlign="top" align="right" width="170"><STRONG><%= LocRM.GetString("LoginName") %>:</STRONG></td>
								<td vAlign="top" width="10"><asp:requiredfieldvalidator id="Requiredfieldvalidator5" runat="server" ControlToValidate="tbLogin" ErrorMessage="*" CssClass="boldtext" Display="Dynamic"></asp:requiredfieldvalidator><asp:regularexpressionvalidator id="RegularExpressionValidator3" runat="server" ControlToValidate="tbLogin" ErrorMessage="*" ValidationExpression="^[\w-\.]+" Display="Dynamic"></asp:regularexpressionvalidator></td>
								<td><asp:textbox id="tbLogin" tabIndex="9" runat="server" CssClass="text" Width="150px"></asp:textbox><BR>
									<SPAN class="ibn-formdescription" style="FONT-SIZE: 10px">
										<%= LocRM.GetString("EnterAccount") %>
									</SPAN>
								</td>
							</tr>
							<tr>
								<td vAlign="top" align="right"><STRONG><%= LocRM.GetString("Password") %>:</STRONG></td>
								<td vAlign="top"><asp:requiredfieldvalidator id="Requiredfieldvalidator6" runat="server" ControlToValidate="tbPassword" ErrorMessage="*" CssClass="boldtext" Display="Dynamic"></asp:requiredfieldvalidator></td>
								<td><asp:textbox id="tbPassword" tabIndex="10" runat="server" CssClass="text" Width="150px" TextMode="Password"></asp:textbox><BR>
									<SPAN class="ibn-formdescription" style="FONT-SIZE: 10px">
										<%= LocRM.GetString("EnterPassword") %>
									</SPAN>
								</td>
							</tr>
							<tr>
								<td vAlign="top" align="right"><STRONG><%= LocRM.GetString("ConfirmPassword") %>:</STRONG></td>
								<td vAlign="top"><asp:requiredfieldvalidator id="Requiredfieldvalidator7" runat="server" ControlToValidate="tbConfirmPassword" ErrorMessage="*" CssClass="boldtext" Display="Dynamic" Height="20px"></asp:requiredfieldvalidator><asp:comparevalidator id="CompareValidator1" runat="server" ControlToValidate="tbConfirmPassword" ErrorMessage="*" CssClass="text" ControlToCompare="tbPassword" Display="Dynamic"></asp:comparevalidator></td>
								<td><asp:textbox id="tbConfirmPassword" tabIndex="11" runat="server" CssClass="text" Width="150px" TextMode="Password"></asp:textbox>&nbsp;</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr id="tr3" runat="server">
					<td>
						<br>
						<div class="ibn-sectionheader" style="DISPLAY: inline">
							<%= LocRM.GetString("Step3") %>
						</div><BR><br>
						<table class="ibn-propertysheet" style="BACKGROUND-COLOR: whitesmoke" cellspacing="0" cellpadding="4" width="100%" border="0">
							<tr>
								<td vAlign="top" align="right">
									<div style="PADDING-TOP: 5px"><b><%= LocRM.GetString("Domain") %>:</b></div>
								</td>
								<td vAlign="top"><asp:requiredfieldvalidator id="rf2" runat="server" ControlToValidate="tbDomain" ErrorMessage="*" CssClass="boldtext"></asp:requiredfieldvalidator><asp:regularexpressionvalidator id="RegularExpressionValidator1" runat="server" ControlToValidate="tbDomain" ErrorMessage="*" ValidationExpression="[\dA-Za-z]+(-*)[\dA-Za-z]+"></asp:regularexpressionvalidator></td>
								<td><b>http://</b>
									<asp:textbox id="tbDomain" onkeydown="ChangeText()" tabIndex="12" runat="server" CssClass="text" Width="150px"></asp:textbox>
									<b><asp:Label ID="lblDomain" Runat="server"></asp:Label></b>
									&nbsp;<span id="imgFree" style="display:none;"><img title='<%=LocRM.GetString("strDomainNotExists")%>' align="absmiddle" src="layouts/free-g.gif" />&nbsp;<%=LocRM.GetString("strDomainNotExists")%></span>
									<span id="imgBusy"  style="display:none;"><img title='<%=LocRM.GetString("strDomainExists")%>' align="absmiddle" src="layouts/busy.gif" />&nbsp;<%=LocRM.GetString("strDomainExists")%></span>
									<input type="button" id="btnCheck" tabindex="13" onclick="CheckDomain()" class="text" value='<%=LocRM.GetString("strCheck")%>'/>
									<img id="imgLoading" align="absmiddle" src="layouts/loading6.gif" style="display:none;" />
									<br>
									<span class="ibn-formdescription" style="FONT-SIZE: 10px">
										<%= LocRM.GetString("EnterTheWeb") %>
									</span>
								</td>
							</tr>
						</table><br /><br />
						<table class="ibn-propertysheet" style="BACKGROUND-COLOR: whitesmoke;" cellspacing="0" cellpadding="4" width="100%" border="0">
							<tr>
							  <td valign="top"><b><%= LocRM.GetString("strKeyword") %>:</b></td>
							  <td></td>
							  <td valign="top">
							    <ibn:AntiRobot id="ucARobot" runat="server"></ibn:AntiRobot>
							  </td>
							</tr>
							<tr>
								<td colspan="3" style="padding-left:85px;"><BR>
									<asp:checkbox id="cbConfirm" tabIndex="15" Runat="server" Text=""></asp:checkbox>
									&nbsp;<asp:Label ID="lblTerms" Runat="server" CssClass="text"></asp:Label>
								</td>
							</tr>
							<tr>
								<td colspan="3" style="padding-left:85px;">
									<asp:checkbox id="cbSendMe" tabIndex="16" Runat="server" Text=""></asp:checkbox>
								</td>
							</tr>
							<tr>
								<td colspan="3" style="padding-left:85px;">
									<asp:checkbox id="cbCallMe" tabIndex="17" Runat="server" Text=""></asp:checkbox>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr id="tr4" runat="server">
					<td><span class="ibn-discussiontitle" style="FONT-SIZE: medium">
							<asp:Label ID="lblStep4Header" Runat="server"></asp:Label>
						</span>
						<BR>
						<span class="text">
							<asp:Label ID="lblStep4SubHeader" Runat="server"></asp:Label></span>
						<BR>
						<table class="ibn-propertysheet" style="BACKGROUND-COLOR: whitesmoke" cellspacing="0" cellpadding="3" width="100%" border="0">
							<tr>
								<td align="left"><asp:Label ID="lblStep4Text" Runat="server"></asp:Label>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td align="center">
					  <hr>
						<input id="btnCreate" runat="server" tabindex="18" type="button" class="text" style="font-size:1.2em" />
						<asp:LinkButton ID="lbCreate" Runat="server" Visible="False" onclick="lbCreate_Click"></asp:LinkButton>&nbsp;
						<asp:LinkButton ID="lbRepeat" Runat="server" CssClass="ibn-navMy" onclick="Button2_ServerClick"></asp:LinkButton>
						<input type="button" id="btnCancel" Runat="server" Class="text" Value="Cancel" tabIndex="18" NAME="btnCancel" onserverclick="btnCancel_ServerClick">
					</td>
				</tr>
			</table>
		</form>
		<script type="text/javascript">
		<!--
		function CheckddCountry(source, args) {
			_ddCountry = document.forms[0].<%= ddCountry.ClientID %>
			if (_ddCountry.selectedIndex <= 0) {
				args.IsValid = false;
			}
			else {
				args.IsValid = true;
			}
		}
		
		function CheckddTimeZone(source, args) {
			_ddTimeZone = document.forms[0].<%= ddTimeZone.ClientID %>
			if (_ddTimeZone.selectedIndex <= 0) {
				args.IsValid = false;
			}
			else {
				args.IsValid = true;
			}
		}
		
		function OpenTerms()
		{
			var w = 650;
			var h = 400;
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			winprops = 'scrollbars=1, resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
			var f = window.open("TermsOfService.aspx", '', winprops);
		}
		
		function ChangeText()
		{
		  var objimgfree = document.getElementById('imgFree');
	    var objimgbusy = document.getElementById('imgBusy');
	    objimgfree.style.display = "none";
	    objimgbusy.style.display = "none";
	    var objbutton = document.getElementById('btnCheck');
	    objbutton.style.display = "";
		}
		
		function CheckDomain()
		{
			var obj = document.forms[0].<%= tbDomain.ClientID %>;
			if(obj.value=="")
			{
				alert('<%=LocRM.GetString("strDomainNotEmpty")%>');
				return;
			}
			var objbutton = document.getElementById('btnCheck');
			var objimg = document.getElementById('imgLoading');
			var req = window.XMLHttpRequest? 
				new XMLHttpRequest() : 
				new ActiveXObject("Microsoft.XMLHTTP");
			req.onreadystatechange = function() 
			{
				if (req.readyState != 4 ) return ;
				if (req.readyState == 4)
				{
				  var objimg = document.getElementById('imgLoading');
				  objimg.style.display = "none";
					if (req.status == 200)
					{
					  //alert(req.responseText.toString());
					  if(req.responseText.toString()=="0")
					  {
					    var objimgfree = document.getElementById('imgFree');
					    var objimgbusy = document.getElementById('imgBusy');
					    objimgfree.style.display = "";
					    objimgbusy.style.display = "none";
					  }
					  else
					  {
					    var objimgfree = document.getElementById('imgFree');
					    var objimgbusy = document.getElementById('imgBusy');
					    objimgfree.style.display = "none";
					    objimgbusy.style.display = "";
					    obj.value = "";
					  }
					}
					else
						alert("There was a problem retrieving the XML data:\n" + req.statusText);
				}
			}
			objbutton.style.display = "none";
			objimg.style.display = "";
			var objimgfree = document.getElementById('imgFree');
	    var objimgbusy = document.getElementById('imgBusy');
	    objimgfree.style.display = "none";
	    objimgbusy.style.display = "none";
			var locObj = document.getElementById('<%=ddLanguage.ClientID%>');
			var dt = new Date();
			var sID = dt.getMinutes() + "_" + dt.getSeconds() + "_" + dt.getMilliseconds();
			req.open("GET", 'CheckDomain.aspx?locale='+locObj.value+'&name='+obj.value+'&sID='+sID, true);
			req.send(null);
		}
		
		function EnableButtons(obj)
		{
			var _link = document.getElementById('<%= btnCreate.ClientID%>');
			_link.disabled = !obj.checked;
		}
		
		function CreateTrial()
		{
		  var _link = document.getElementById('<%= cbConfirm.ClientID%>');
			if(_link && _link.checked)
			{
				if(browseris.ie5up)
				{
					if (typeof(Page_ClientValidate) != 'function' || Page_ClientValidate())
						<%=Page.GetPostBackClientEvent(lbCreate, "")%>;
				}
				else
					<%=Page.GetPostBackClientHyperlink(lbCreate, "")%>;
			}
		}
// -->
		</script>
<script type="text/javascript">
var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
</script>
<script type="text/javascript">
try {
var pageTracker = _gat._getTracker("UA-490257-27");
pageTracker._trackPageview();
} catch(err) {}</script>		
	</body>
</HTML>