<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Incidents.EMailRouterPush" CodeBehind="EMailRouterPush.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	<title>EMailRouterPush</title>
</head>
<body>
	<form id="Form1" method="post" runat="server">
	<asp:Button ID="routeEMailBoxesButton" runat="server" Text="Route EMail Boxes" Width="312px" OnClick="routeEMailBoxesButton_Click"></asp:Button>
	</form>
</body>
</html>
