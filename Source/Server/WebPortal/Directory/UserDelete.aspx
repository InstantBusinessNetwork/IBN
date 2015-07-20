<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Directory.UserDelete" CodeBehind="UserDelete.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title><%=LocRM.GetString("DeleteUser")%></title>

	<script type="text/javascript">
		function disableEnterKey() {
			try {
				if (window.event.keyCode == 13 && window.event.srcElement.type != "textarea")
					window.event.keyCode = 0;
			}
			catch (e) { }
		} 
	</script>

</head>
<body class="UserBackground" id="pT_body" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
	<form id="frmMain" runat="server">
	<asp:PlaceHolder ID="phDelete" runat="server" />
	</form>
</body>
</html>
