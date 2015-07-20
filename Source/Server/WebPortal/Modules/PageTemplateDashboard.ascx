<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageTemplateDashboard.ascx.cs" Inherits="Mediachase.UI.Web.Modules.PageTemplateDashboard" %>
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

		<asp:ScriptManager ID="ScriptManager2" runat="server" EnablePartialRendering="true" ScriptMode="Release" EnableScriptGlobalization = "true" EnableScriptLocalization="true">
			<Services>
				<asp:ServiceReference Path="~/Apps/MetaUI/WebServices/ListHandler.asmx" InlineScript="true" />
				<asp:ServiceReference Path="~/Apps/Shell/WebServices/LayoutHandler.asmx" InlineScript="true" />
			</Services>
		</asp:ScriptManager>
		<Ibn:Ie6Popup runat="server"></Ibn:Ie6Popup>
		<asp:UpdatePanel ID="CommandManagerUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
			<ContentTemplate>
				<ibn:CommandManager ID="cmdManager" runat="server" ContainerId="containerDiv" UsePageHeaderForStyles="true" />
			</ContentTemplate>
		</asp:UpdatePanel>
		
		<table cellspacing="0" cellpadding="0" border="0" width="100%" style="height:97%" class="ibn-propertysheet">
			<tr>
				<td style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px; PADDING-TOP: 5px" valign="top" height="100%">
					<asp:placeholder id="phMain" runat="server"></asp:placeholder>
				</td>
			</tr>
			
		</table>
		<div id="containerDiv" runat="server" style="display: none"></div>
	<script type="text/javascript" defer="defer">
	//<![CDATA[
	document.onclick=HideFrames2;
	function pageLoad()
	{
	    if (_loadedFlag === true)
	        return;
	    _loadedFlag = true;
	    
		var obj = document.getElementById('<%= containerDiv.ClientID %>');
		if (obj)
		{
			obj.style.display = 'block';
		}
		Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(ibnBeginRequestHandler)
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
	function pageLoad1()
	{
	    pageLoad();
	}
	//]]>
	</script>
	</form>
</body>
</html>