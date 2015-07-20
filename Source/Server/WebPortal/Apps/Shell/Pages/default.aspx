<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Mediachase.Ibn.Web.UI.Shell.Pages._default" %>
<%@ register TagPrefix="mc" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Reference Control="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Register TagPrefix="mc" TagName="MetaToolbar" Src="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Reference Control="~/Modules/upTemplate.ascx" %>
<%@ Register TagPrefix="mc" TagName="upTemplate" Src="~/Modules/upTemplate.ascx" %>
<%@ Reference Control="~/Apps/Shell/Modules/leftTemplate.ascx" %>
<%@ Register TagPrefix="mc" TagName="leftTemplate" Src="~/Apps/Shell/Modules/leftTemplate.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=sTitle%></title>
	<link id="iconIBN" runat="server" type='image/x-icon' rel="shortcut icon" />
	<style type="text/css">
		table.imageMiddle tbody tr td em button { background-position: 4pt 2px !important; }
	</style>
</head>
<body>
	<iframe name="frmList" id="frmList" frameborder="0" class="listFrame" style="display:none; z-index:99; position:absolute; left:1px; top:24px; width:800px;"></iframe>
	<form id="form1" runat="server">
		<asp:ScriptManager ID="sm1" runat="server" EnablePartialRendering="true" ScriptMode="Release" EnableScriptGlobalization = "true" EnableScriptLocalization="true" EnableHistory="true">
		</asp:ScriptManager>
		<div id="up_div" class="noprint">
			<div id="TopPlaceDiv">
				<mc:upTemplate id="upCtrl" runat="server"></mc:upTemplate>
			</div>
			<mc:MetaToolbar runat="server" ID="MainMetaToolbar" ClassName="" PlaceName="TopMenu" />
		</div>
		<div id="left_div" class="noprint">
			<mc:leftTemplate id="leftCtrl" runat="server"></mc:leftTemplate>
		</div>
		<div id="center_div" style="height:100%; width:100%;">
			<iframe frameborder="0" scrolling="auto" name="right" id="right" width="100%" marginheight="0" marginwidth="0" src='<%=ResolveUrl("~/Apps/Shell/Pages/Empty.html") %>'></iframe>
		</div>
		<mc:CommandManager ID="cm1" runat="server" ContainerId="containerDiv" EnableViewState="false" UsePageHeaderForStyles="true" />
		<div id="containerDiv" runat="server" class="noprint"></div>

	</form>
	<script type="text/javascript" defer="defer">
	//<![CDATA[
	function HideMenu(e)
	{
		var toolbar = $find("<%= MainMetaToolbar.GetJsToolbar().ClientID%>");
		if (toolbar)
		{
			toolbar.hideAllMenus();
		}
	}
	document.onclick = HideFrames2;
	//]]>
	</script>

	<script type="text/javascript">
	//<![CDATA[
	//dvs: Opera fix
	function pageLoad1() {
		pageLoad(true);
	}

	//        if (Sys.Browser.name == "Opera" || Sys.Browser.name == "Firefox")
	Sys.Application.add_init(function() { pageLoad1(); });

	function pageLoad(_force) {
		if (_force === true) {
			if ((Ext && !Ext.isIE && top.location.href.indexOf("#right") < 0) || (!Ext && top.location.href.indexOf("#right") < 0)) {
				var newLocation = '<%=ResolveClientUrl("~/Apps/Shell/Pages/default.aspx")%>' + '?p=ibn#right=' + escapeWithAmp('<%=defaultLink%>');

				if (top.location.replace)
					top.location.replace(newLocation);
				else
					top.location.href = newLocation;

				return;
			}
			else {
				// O.R.: Sys.Application._state.right doesn't work correctly in FireFox, 
				// because it doesn't return any info after "&".
				// So we introduce a small hack to use hash value instead of Sys.Application._state
				var useHash = false;
				var hash = window.location.hash;

				if (Sys.Browser.agent === Sys.Browser.Firefox && (hash.length > 0) && (hash.indexOf('#right=') == 0)) {
					hash = hash.substring(7);
					useHash = true;
				}
				
				if (Sys.Application._state.right && Sys.Application._state.right.indexOf("%3A//") < 0
					|| !Sys.Application._state.right)
				{
					if (useHash)
					{
						Sys.Application.addHistoryPoint({ right: replaceAmp(hash) });
						
					}
					else if (Sys.Application._state.right)
					{
						var newFrameSrc = Sys.Application._state.right;
						Sys.Application.addHistoryPoint({ right: replaceAmp(newFrameSrc) });
					}
					
				}

				initLayout();
				initIframe();

				//frame load
				var bookmarkedFrameSrc = Sys.Application._state.right;
				if (bookmarkedFrameSrc) {
					if (useHash) {
						mainLayout_initialFrameSrc = hash;
					}
					else {
						mainLayout_initialFrameSrc = bookmarkedFrameSrc;
					}
				}
				else {
					mainLayout_initialFrameSrc = escapeWithAmp('<%=defaultLink%>');
				}

				mainLayout_initialize();
			}
		}
	}

	function escapeWithAmp(str) {
		var re = /&/gi;
		var ampEncoded = "%26";
		return escape(str).replace(re, ampEncoded);
	}

	function replaceAmp(str)
	{
		var re = /&/gi;
		var ampEncoded = "%26";
		return str.replace(re, ampEncoded);
	}

	function RefreshRightFrame(params) {
		window.top.frames.right.location.href = window.top.frames.right.location.href;
	}

	var globalImageBaseUrl = '<%= this.ResolveUrl("~/Images/IbnFramework/") %>';
	//]]>
	</script>
</body>
</html>
