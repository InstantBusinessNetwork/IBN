<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageTemplateNext.ascx.cs" Inherits="Mediachase.UI.Web.Modules.PageTemplateNext" %>
<%@ Register TagPrefix="uc1" TagName="bc" Src="BottomCopyright.ascx" %>
<%@ register TagPrefix="mc" namespace="Mediachase.UI.Web.Modules" Assembly="Mediachase.UI.Web" %>
<%@ register TagPrefix="ibn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
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
				<div style="position: relative;  z-index: 10002">
					<img alt="" style="border:0; position: absolute; left: 40%; top: 40%; z-index: 10003" src='<%= ResolveClientUrl("~/Images/IbnFramework/loading_rss.gif") %>' />
				</div>
			</div>
		</div>
		<%--<ibn:AutoResizerExtender runat="server" ID="ibnAutoResizer" />--%>
		<asp:ScriptManager ID="ScriptManager2" runat="server" EnablePartialRendering="true" ScriptMode="Release" EnableScriptGlobalization = "true" EnableScriptLocalization="true">
			<Services>
				<asp:ServiceReference Path="~/Apps/MetaUI/WebServices/ListHandler.asmx" InlineScript="true" />
				<asp:ServiceReference Path="~/Apps/Shell/WebServices/LayoutHandler.asmx" InlineScript="true" />
				<asp:ServiceReference Path="~/Apps/Common/WebServices/WsResourceEditor.asmx" InlineScript="true" />
			</Services>
		</asp:ScriptManager>
		<Ibn:Ie6Popup runat="server"></Ibn:Ie6Popup>
		
		<ibn:CommandManager ID="cmdManager" runat="server" ContainerId="containerDiv" UsePageHeaderForStyles="true" />
		
		<div id="containerDiv" runat="server"></div>
		<table cellspacing="0" cellpadding="0" border="0" width="100%" style="height:97%" class="ibn-propertysheet">
			<tr>
				<td valign="top" style="height:100%" runat="server" id="MainCell">
					<asp:placeholder id="phMain" runat="server"></asp:placeholder>
				</td>
			</tr>
			
		</table>
	<script type="text/javascript" defer="defer">
	//<![CDATA[
	document.onclick=HideFrames2;

	function pageLoad()
	{
	    if (_loadedFlag === true)
	        return;
	    _loadedFlag = true;
	    
		var obj = document.getElementById('ibn_divWithLoadingRss');
		if (obj)
		{
			obj.style.display = 'none';
		}

		if (window.location.href.toLowerCase().indexOf('workspace/default.aspx') > 0)
		{
			Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(ibnBeginRequestHandler)
		}

		if (typeof (contentPageLoad) == "function")
			contentPageLoad();
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
			div.innerHTML = '<img style=" ' + imgStyle + '" src="' + imgSrc + '" border="0" alt="loading"/>';

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

	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
	function EndRequestHandler(sender, args)
	{
		if (args.get_error() != null)
		{
			var errorMessage = args.get_error().message;
			if (errorMessage && errorMessage.length > 0 )
			{
				args.set_errorHandled(true);
				alert(errorMessage.replace("Sys.WebForms.PageRequestManagerServerErrorException: ", ""));
			}
		}
	}

	var _loadedFlag = false;
	Sys.Application.add_init(function() { pageLoad1(); });
	function pageLoad1() {
	    pageLoad();
	}
	//]]>
	</script>
	</form>
</body>
</html>