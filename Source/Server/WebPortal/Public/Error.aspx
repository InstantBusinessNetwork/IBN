<?xml version='1.0' encoding='utf-8' ?>
<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Public.Error" CodeBehind="Error.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="error" Src="~/public/modules/error.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title>Error</title>
</head>
<body class="UserBackground" id="body" runat="server">
	<form id="frmMain" method="post" runat="server">
	<ibn:error runat="server" ID="error" title="Error" />
	</form>
</body>
</html>
