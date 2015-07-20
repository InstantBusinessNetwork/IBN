<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DialogTemplateNext.ascx.cs" Inherits="Mediachase.UI.Web.Modules.DialogTemplateNext" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title></title>
	<link id="iconIBN" runat="server" type='image/x-icon' rel="shortcut icon" />
</head>
<body class="ibn-WhiteBg">
	<form id="frmMain" runat="server" method="post">
	<div>
		<asp:ScriptManager ID="ScriptManager1" runat="server" ScriptMode="Release" EnablePartialRendering="true" EnableScriptGlobalization="true" EnableScriptLocalization="true"></asp:ScriptManager>
		<asp:PlaceHolder ID="phMain" runat="server"></asp:PlaceHolder>
		<mc:CommandManager ID="cm" runat="server" ContainerId="divContainer" UsePageHeaderForStyles="true" />
		<div id="divContainer" runat="server"></div>
	</div>
	</form>
</body>
</html>
