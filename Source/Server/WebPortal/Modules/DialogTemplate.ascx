<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.DialogTemplate" CodeBehind="DialogTemplate.ascx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=Title%></title>
	<link id="iconIBN" runat="server" type='image/x-icon' rel="shortcut icon" />

	<script type="text/javascript">
		//<![CDATA[
		function disableEnterKey() {
			try {
				if (window.event.keyCode == 13 && window.event.srcElement.type != "textarea")
					window.event.keyCode = 0;
			}
			catch (e) { }
		}
		//]]>
	</script>

</head>
<body class="UserBackground">
	<form id="mainForm" method="post" runat="server" onkeypress="return disableEnterKey(event);">
		<asp:ScriptManager ID="sm" runat="server" ScriptMode="Debug" EnablePartialRendering="true" EnableScriptGlobalization="true" EnableScriptLocalization="true"></asp:ScriptManager>
		<asp:PlaceHolder ID="phMain" runat="server"></asp:PlaceHolder>
	</form>
</body>
</html>
