<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageTemplateNew.ascx.cs" Inherits="Mediachase.Ibn.Apps.ClioSoft.Modules.PageTemplateNew" %>
<%@ Register Assembly="System.Web.Extensions" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="headTag" runat="server">
	<title>
		<%=Title%>
	</title>
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
	<script type="text/javascript">
		function OnRedirect()
		{
			if(parent.newFrameSrc)
				parent.newFrameSrc = location.href;
			if(parent.gotFromTemp != 'undefined')
				parent.gotFromTemp = true;
		}
	</script>

	<script type="text/javascript">
		if (parent == window)
		{
			if (location.replace)
				location.replace('<%=ResolveClientUrl("~/Apps/Shell/Pages/default.aspx")%>' + '#right=' + escape(location.href));
			else
				location.href = '<%=ResolveClientUrl("~/Apps/Shell/Pages/default.aspx")%>' + '#right=' + escape(location.href);
		}
		else
		{
			if (parent && parent.document)
			{
				var td = parent.document.getElementById("onetidPageTitle");
				if (td)
					td.innerHTML = self.document.title;
			}
			top.document.title = self.document.title + " | Instant Business Network 4.7";
		}
	</script>
</head>
<body class="UserBackground" id="bodyTag" runat="server">
	<form id="frmMain" method="post" runat="server" onkeypress="return disableEnterKey(event);">
		<div id='ibn_divWithLoadingRss' style="position: absolute; left: 0px; top: 0px; height: 100%; width: 100%; background-color: White; z-index: 10000">
			<div style="left: 40%; top: 40%; height: 30px; width: 200px; position: absolute; z-index: 10001">
				<div style="position: relative; z-index: 10002">
					<img style="position: absolute; left: 40%; top: 40%; z-index: 10003" src='<%= ResolveClientUrl("~/Images/IbnFramework/loading_rss.gif") %>' border='0' alt="" />
				</div>
			</div>
		</div>
		<asp:ScriptManager ID="ScriptManager1" runat="server" ScriptMode="Release" EnablePartialRendering="true" EnableViewState="true" EnableScriptLocalization="true" EnableScriptGlobalization="true">
			<Services>
				<asp:ServiceReference InlineScript="true" Path="~/Apps/Shell/WebServices/LayoutHandler.asmx" />
			</Services>
		</asp:ScriptManager>
		<table cellspacing="0" cellpadding="0" width="100%" border="0" height="97%" class="ibn-propertysheet">
			<tr>
				<td style="" valign="top" height="100%">
					<asp:PlaceHolder ID="phMain" runat="server"></asp:PlaceHolder>
				</td>
			</tr>
		</table>

		<script type="text/javascript">
		document.onclick=HideFrames2;
		
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
				parent = obj.parentNode;
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
					if (coll[i].Printable == "0" || coll[i].printable == "0")
						coll[i].style.display = "none";
					if (coll[i].Printable == "1" || coll[i].printable == "1")
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
					if (coll[i].Printable == "0" || coll[i].printable == "0")
						coll[i].style.display = "block";
					if (coll[i].Printable == "1" || coll[i].printable == "1")
						coll[i].style.display = "none";
				}
			}
		}
		if (browseris.ie5up)
		{
			window.onbeforeprint = BeforePrint;
			window.onafterprint = AfterPrint;	
		}
		</script>

	</form>
</body>
</html>
