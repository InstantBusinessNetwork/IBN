<%@ Page Language="c#" Inherits="Mediachase.IBN.UI.Web.FileLibrary.FileDownload" EnableViewState="false" CodeBehind="FileDownload.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	<title>FileDownload</title>
</head>
<body>
	<form method="post" runat="server">
	<input type="text" id="Login" runat="server">
	<input id="Password" type="text" runat="server">
	<input id="FolderID" type="text" runat="server">
	<input id="VersionID" type="text" runat="server">
	<input type="button" id="btnSubmit" runat="server" onserverclick="btnSubmit_Click">
	</form>
</body>
</html>
