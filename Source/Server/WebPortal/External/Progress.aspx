<%@ Page Language="c#" Inherits="Mediachase.UI.Web.External.Progress" CodeBehind="Progress.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title><%=Percents%></title>

	<style type="text/css">
		.ibn-vb
		{
			font-family: verdana;
			font-size: .68em;
			vertical-align: top;
		}
	</style>

	<script type="text/javascript">
		//<![CDATA[
		function refresh() {
			document.forms[0].submit();
		}
		//]]>
	</script>

</head>
<body class="text" style="padding: 15px" id="body" runat="server">
	<div id="dWinClose" runat="server" visible="false">
		<script type="text/javascript">
			//<![CDATA[
			window.close();
			//]]>
		</script>
	</div>
	<form method="post" runat="server">
	<%=LocRM.GetString("tUploading")%>:<br>
	<table cellspacing="0" cellpadding="0" align="Center" border="1" bordercolor="Black" border="0" style="border-color: Black; border-style: None; width: 100%; border-collapse: collapse;">
		<tr style="height: 18px;">
			<td class="ibn-vb" style="background-color: #000099; width: <%=PercentsAbs %>;">
			</td>
			<td class="ibn-vb" style="background-color: White; width: 60%;">
			</td>
		</tr>
	</table>
	<br>
	<asp:Label ID="lblTransferred" runat="server" />
	<br>
	<asp:CheckBox AutoPostBack="True" Checked="True" EnableViewState="True" ID="cbClose" runat="server" CssClass="text" /><br>
	<br>
	<table width="100%" id="dClose" runat="server">
		<tr>
			<td align="center">
				<input type="button" class="text" value='<%=LocRM.GetString("tBtnClose")%>' style="width: 80" onclick="window.close();" />
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
