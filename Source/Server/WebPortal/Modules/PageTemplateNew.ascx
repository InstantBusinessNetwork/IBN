<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PageTemplateNew.ascx.cs" Inherits="Mediachase.UI.Web.Modules.PageTemplateNew" %>
<%@ Register TagPrefix="uc1" TagName="bc" Src="BottomCopyright.ascx" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="Ibn" TagName="Ie6Popup" Src="~/Modules/ie6updatePopup.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=Title%></title>
	<link id="iconIBN" runat="server" type='image/x-icon' rel="shortcut icon" />
</head>
<body class="UserBackground" id="bodyTag" runat="server">
	<form id="frmMain" method="post" runat="server" onkeypress="return disableEnterKey(event);">
		<div id='ibn_divWithLoadingRss' style="position: absolute; left: 0px; top: 0px; height: 100%; width: 100%; background-color: White; z-index: 10000">
			<div style="left: 40%; top: 40%; height: 30px; width: 200px; position: absolute; z-index: 10001">
				<div style="position: relative; z-index: 10002">
					<img alt="" style="position: absolute; left: 40%; top: 40%; z-index: 10003; border:0" src='<%= ResolveClientUrl("~/Images/IbnFramework/loading_rss.gif") %>' />
				</div>
			</div>
		</div>
		<Ibn:Ie6Popup ID="Ie6Popup1" runat="server"></Ibn:Ie6Popup>
		<asp:ScriptManager ID="ScriptManager1" runat="server" ScriptMode="Release" EnablePartialRendering="true" EnableViewState="true" EnableScriptLocalization="true" EnableScriptGlobalization="true">
			<Services>
				<asp:ServiceReference InlineScript="true" Path="~/Apps/Shell/WebServices/LayoutHandler.asmx" />
			</Services>
		</asp:ScriptManager>
		<mc:CommandManager ID="cm" runat="server" ContainerId="divContainer" UsePageHeaderForStyles="true" />
		<div id="divContainer" runat="server"></div>
		<table cellspacing="0" cellpadding="0" border="0" class="ibn-propertysheet" style="width:100%; height:100%">
			<tr>
				<td valign="top" style="height:95%; vertical-align:top; padding-right: 5px; padding-left: 5px; padding-top: 5px;">
					<asp:PlaceHolder ID="phMain" runat="server"></asp:PlaceHolder>
				</td>
			</tr>
			<tr>
				<td class="ibn-propertysheet" style="vertical-align:bottom; text-align: center; padding: 5px 5px 0 5px">
					<uc1:bc ID="bottomline" runat="server" ShowTopLine="true" />
				</td>
			</tr>
		</table>

		<script type="text/javascript" defer="defer">
		//<![CDATA[
		document.onclick = HideFrames2;

		//dvs: Opera fix
		function pageLoad1() {
			pageLoad();
		}
		Sys.Application.add_init(function() { pageLoad1(); });

		function pageLoad()
		{
			var obj = document.getElementById('ibn_divWithLoadingRss');
			if (obj)
			{
				obj.style.display = 'none';
			}

			if (window.location.href.toLowerCase().indexOf('workspace/default.aspx') > 0)
			{
				Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(ibnBeginRequestHandler)
			}
		}

		function ibnBeginRequestHandler(sender, args)
		{
			var obj = args._postBackElement;
			var parent = null;

			if (obj)
			{
				parent = obj.parentNode;
				while(parent && parent.nodeType == 1 && (parent.tagName == "TD" || parent.tagName == "TR"  || parent.tagName == "TBODY" ||parent.tagName == "TABLE"))
				{
					obj = parent;
					parent = obj.parentNode;
				}
			}
			else
				return;

			if (parent)
			{
				var div = document.createElement("DIV");
				var imgSrc = '<%= ResolveClientUrl("~/Images/IbnFramework/loading_rss.gif") %>';
				var imgStyle = 'padding-Top: ' + (obj.clientHeight - 8) / 2 + 'px; padding-Left: ' + (obj.clientWidth - 8) / 2 + 'px;';

				div.style.height = obj.clientHeight + 'px';
				div.style.width = '40%';//obj.clientWidth + 'px';
				div.innerHTML = '<img align="absmiddle" style=" ' + imgStyle + '" src="' + imgSrc + '" border="0" alt="loading"/>';

				parent.removeChild(obj);
				parent.appendChild(div);
			}
		}

		function BeforePrint()
		{
			var coll = document.all;
			if (coll!=null)
			{
				for (var i=0; i<coll.length; i++)
				{
					if (coll[i].Printable == "0")
						coll[i].style.display = "none";
					if (coll[i].Printable == "1")
						coll[i].style.display = "block";
				}
			}
		}

		function AfterPrint()
		{
			var coll = document.all;
			if (coll!=null)
			{
				for (var i=0; i<coll.length; i++)
				{
					if (coll[i].Printable == "0")
						coll[i].style.display = "block";
					if (coll[i].Printable == "1")
						coll[i].style.display = "none";
				}
			}
		}

		if (browseris.ie5up)
		{
			window.onbeforeprint = BeforePrint;
			window.onafterprint = AfterPrint;
		}
		//]]>
		</script>
	</form>
</body>
</html>
