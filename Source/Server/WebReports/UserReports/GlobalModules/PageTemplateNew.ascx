<%@ Control Language="c#" Inherits="Mediachase.UI.Web.UserReports.GlobalModules.PageTemplateNew" CodeBehind="PageTemplateNew.ascx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<title><%=Title%></title>

	<script type="text/javascript">
		//<![CDATA[
		function OnRedirect() {
			if (parent.newFrameSrc)
				parent.newFrameSrc = location.href;
			if (parent.gotFromTemp != 'undefined')
				parent.gotFromTemp = true;
		}
		//]]>
	</script>

</head>
<body class="UserBackground" id="bodyTag" runat="server">
	<form id="frmMain" method="post" runat="server" onkeypress="return disableEnterKey(event);">
	<asp:ScriptManager ID="ScriptManager1" runat="server" ScriptMode="Release" EnablePartialRendering="true" EnableViewState="true" EnableScriptLocalization="true" EnableScriptGlobalization="true">
		<Services>
			<asp:ServiceReference InlineScript="true" Path="~/Apps/Shell/WebServices/LayoutHandler.asmx" />
		</Services>
	</asp:ScriptManager>
	<table cellspacing="0" cellpadding="0" border="0" class="ibn-propertysheet" style="width: 100%; height: 97%">
		<tr>
			<td style="" valign="top" height="100%">
				<asp:PlaceHolder ID="phMain" runat="server"></asp:PlaceHolder>
			</td>
		</tr>
	</table>

	<script type="text/javascript">
		//<![CDATA[
		document.onclick = HideFrames2;
		function BeforePrint() {
			var coll = document.all;
			if (coll != null) {
				for (var i = 0; i < coll.length; i++) {
					if (coll[i].Printable == "0" || coll[i].printable == "0")
						coll[i].style.display = "none";
					if (coll[i].Printable == "1" || coll[i].printable == "1")
						coll[i].style.display = "block";
				}
			}
		}
		function AfterPrint() {
			var coll = document.all;
			if (coll != null) {
				for (var i = 0; i < coll.length; i++) {
					if (coll[i].Printable == "0" || coll[i].printable == "0")
						coll[i].style.display = "block";
					if (coll[i].Printable == "1" || coll[i].printable == "1")
						coll[i].style.display = "none";
				}
			}
		}
		if (browseris.ie5up) {
			window.onbeforeprint = BeforePrint;
			window.onafterprint = AfterPrint;
		}
		//]]>
	</script>

	</form>
</body>
</html>
