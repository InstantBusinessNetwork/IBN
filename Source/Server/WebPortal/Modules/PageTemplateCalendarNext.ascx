<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageTemplateCalendarNext.ascx.cs" Inherits="Mediachase.UI.Web.Modules.PageTemplateCalendarNext" %>
<%@ Register TagPrefix="ibn2" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="Ibn" TagName="Ie6Popup" Src="~/Modules/ie6updatePopup.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=Title%></title>
	<link id="iconIBN" runat="server" type='image/x-icon' rel="shortcut icon" />
</head>
<body class="UserBackground" id="bodyTag" runat="server">
	<form id="frmMain" method="post" runat="server" onkeypress="return disableEnterKey(event);">
	<asp:ScriptManager ID="ScriptManager2" runat="server" EnablePartialRendering="true" LoadScriptsBeforeUI="true" ScriptMode="Release" EnableScriptGlobalization="true" EnableScriptLocalization="true">
		<Services>
			<asp:ServiceReference Path="~/Apps/Shell/WebServices/LayoutHandler.asmx" InlineScript="true" />
			<asp:ServiceReference Path="~/Apps/Calendar/WebServices/Default.asmx" InlineScript="true" />
		</Services>
	</asp:ScriptManager>
	<Ibn:Ie6Popup ID="Ie6Popup1" runat="server"></Ibn:Ie6Popup>
	<ibn2:LayoutExtender ID="LayoutExtender1" runat="server" TargetControlID="IbnMainLayout"></ibn2:LayoutExtender>
	<div style="border-left: 1px solid #6B92CE; height: 100%;">
		<ibn2:McLayout runat="server" ID="IbnMainLayout" ClientOnResize="LayoutResizeHandler">
			<Items>
				<asp:PlaceHolder ID="phMain" runat="server"></asp:PlaceHolder>
			</Items>
		</ibn2:McLayout>
	</div>
	<asp:UpdatePanel ID="CommandManagerUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
		<ContentTemplate>
			<ibn2:CommandManager runat="server" ContainerId="divContainer" ID="CM" UsePageHeaderForStyles="true" />
		</ContentTemplate>
	</asp:UpdatePanel>
	<div id="divContainer" runat="server" />
	<input type="hidden" id="PageX" name="PageX" value="0" runat="server" />
	<input type="hidden" id="PageY" name="PageY" value="0" runat="server" />
	</form>

	<script type="text/javascript">
		//<![CDATA[
		document.onclick = HideFrames2;
		//]]>
	</script>

</body>
</html>
