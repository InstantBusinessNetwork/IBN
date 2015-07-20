<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Modules.ControlLoader" CodeBehind="ControlLoader.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title>ControlLoader</title>
</head>
<body id="pT_body" class="topMenuFrameBody">
	<form id="frmMain" runat="server">
	<table cellpadding="0" cellspacing="0" border="0" id="tbImg" runat="server" width="100%" height="100%">
		<tr>
			<td align="center" valign="middle" width="100%">
				<img alt="" src="../layouts/images/cl-loading.gif" border="0" height="27" width="94" />
			</td>
		</tr>
	</table>
	<table cellpadding="0" cellspacing="0" border="0" style="display: none;" id="tbMain" runat="server" width="100%">
		<tr>
			<td width="100%">
				<asp:PlaceHolder ID="phItems" runat="server" />
			</td>
		</tr>
	</table>
	</form>

	<script type="text/javascript">
		//<![CDATA[
		fnStartInitProcess();
		//]]>
	</script>

</body>
</html>
