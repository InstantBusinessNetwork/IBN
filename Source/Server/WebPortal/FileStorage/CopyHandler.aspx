<%@ Page Language="c#" Inherits="Mediachase.UI.Web.FileStorage.CopyHandler" CodeBehind="CopyHandler.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title>IBN</title>

	<script type="text/javascript">
		//<![CDATA[
		setTimeout('window.close()', 1500);
		//]]>
	</script>

</head>
<body class="UserBackground" id="pT_body">
	<form id="copyForm" method="post" runat="server" enctype="multipart/form-data">
	<table width="100%" height="100%">
		<tr>
			<td valign="middle" align="center">
				<asp:Label ID="lblResult" runat="server" CssClass="text"></asp:Label>
			</td>
		</tr>
		<tr valign="bottom">
			<td align="center" style="padding-bottom: 10px">
				<input type="button" value='<%=LocRM.GetString("tClose")%>' id='btnClose' onclick='javascript:window.close();' />
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
