<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DialogTemplateLayout.ascx.cs" Inherits="Mediachase.UI.Web.Modules.DialogTemplateLayout" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="Ibn" TagName="Ie6Popup" Src="~/Modules/ie6updatePopup.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=Title%></title>
	<link id="iconIBN" runat="server" type='image/x-icon' rel="shortcut icon" />

	<script type="text/javascript" defer="defer">
		//<![CDATA[
		function pageLoad() {
			var obj = document.getElementById('ibn_divWithLoadingRss');
			if (obj) {
				obj.style.display = 'none';
			}
		}
		//]]>
	</script>

</head>
<body class="ibn-WhiteBg">
	<form id="frmMain" runat="server" method="post">
	<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" EnableScriptGlobalization="true" EnableScriptLocalization="true" LoadScriptsBeforeUI="false">
		<Services>
			<asp:ServiceReference Path="~/Apps/MetaUI/WebServices/ListHandler.asmx" InlineScript="true" />
		</Services>
	</asp:ScriptManager>
	<div id='ibn_divWithLoadingRss' style="position: absolute; left: 0px; top: 0px; height: 100%; width: 100%; background-color: White; z-index: 10000">
		<div style="left: 40%; top: 40%; height: 30px; width: 200px; position: absolute; z-index: 10001">
			<div style="position: relative; z-index: 10002">
				<img alt='' style="position: absolute; left: 40%; top: 40%; z-index: 10003" src='<%= ResolveClientUrl("~/Images/IbnFramework/loading_rss.gif") %>' border='0' />
			</div>
		</div>
	</div>
	<Ibn:Ie6Popup ID="Ie6Popup1" runat="server"></Ibn:Ie6Popup>
	<mc:LayoutExtender ID="LayoutExtender1" runat="server" TargetControlID="IbnMainLayout">
	</mc:LayoutExtender>
	<div style="height: 100%;">
		<mc:McLayout runat="server" ID="IbnMainLayout" ClientOnResize="LayoutResizeHandler">
			<Items>
				<asp:PlaceHolder ID="phMain" runat="server"></asp:PlaceHolder>
			</Items>
		</mc:McLayout>
	</div>
	<asp:UpdatePanel ID="CommandManagerUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
		<ContentTemplate>
			<mc:CommandManager ID="cm" runat="server" ContainerId="divContainer" UsePageHeaderForStyles="true" />
		</ContentTemplate>
	</asp:UpdatePanel>
	<div id="divContainer" runat="server">
	</div>
	</form>
</body>
</html>
