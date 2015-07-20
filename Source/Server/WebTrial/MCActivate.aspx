<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MCActivate.aspx.cs" Inherits="Mediachase.Ibn.WebTrial.MCActivate" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Activation</title>
		<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
		<script src='<%=ResolveUrl("~/layouts/buttons.js") %>'></script>
		<script src='<%=ResolveUrl("~/layouts/browser.js") %>'></script>
		<link rel="stylesheet" type="text/css" href="http://ibn.qa.mediachase.com/App_Themes/Everything/style.css"/>
		<script>
		<!--
		if ((browseris.mac) && !browseris.ie5up)
		{
			var macstyle = "../layouts/<%=System.Threading.Thread.CurrentThread.CurrentUICulture.Name%>/styles/mac.css";
			document.write("<link rel='stylesheet' Type='text/css' href='" + macstyle + "'>");
		}
		//-->
		</script>
	</HEAD>
	<body style="TEXT-ALIGN: center" class="pmreg_body">
		<form id="mcSignupForm" method="post" runat="server">
			<table cellspacing="0" cellpadding="3" width="725" border="0" align="center">
				<tr>
					<td>
						<div ID="divTitle" Runat="server" class="ibn-sectionheader" style="DISPLAY: inline"></div>
						<br />
						<div class="ibn-propertysheet" id="divMess" runat="server"></div>
						<div class="ibn-propertysheet" id="divFailed" runat="server" style="display:none;"></div>
					</td>
				</tr>
			</table>
			<script type="text/javascript">
        function ActivatePortal(feedUrl, containerId, containerTitleId, errorContainerId)
        {
          var errorContainer = null;
          if(errorContainerId != null)
            errorContainer = document.getElementById(errorContainerId);
          document.getElementById(containerId).innerHTML = "<div><img align='absmiddle' border='0' src='Layouts/loading8.gif' /></div>";
          document.getElementById(containerTitleId).innerHTML = '<%= LocRM.GetString("MC_ActProgress")%>';
          var ajaxRequest;
            
          try
          {
            // Opera 8.0+, Firefox, Safari
            ajaxRequest = new XMLHttpRequest();
          } 
          catch (e)
          {
	          // Internet Explorer Browsers
	          try
	          {
		          ajaxRequest = new ActiveXObject("Msxml2.XMLHTTP");
	          } 
	          catch (e) 
	          {
		          try
		          {
			          ajaxRequest = new ActiveXObject("Microsoft.XMLHTTP");
		          } 
		          catch (e)
		          {
			          // Something went wrong
			          alert("Your browser broke!");
			          return false;
		          }
	          }
          }
          // Create a function that will receive data sent from the server
          ajaxRequest.onreadystatechange = function()
          {
            if(ajaxRequest.readyState == 4)
            {
              fl = true;
              if(ajaxRequest.responseText=="0")
              {
                document.getElementById(containerId).style.display = "none";
                document.getElementById('<%= divFailed.ClientID%>').style.display = "";
                document.getElementById(containerTitleId).innerHTML = '<%= LocRM.GetString("MC_ActivationError")%>';
              }
              else
              {
                document.getElementById(containerId).innerHTML = ajaxRequest.responseText;
                document.getElementById(containerTitleId).innerHTML = '<%= LocRM.GetString("MC_Congratulations")%>';
              }
            }
	        }
	        ajaxRequest.open("GET", feedUrl, true);
	        ajaxRequest.send(null); 
        }
        </script>
		</form>
        <script type="text/javascript">
var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
</script>
<script type="text/javascript">
try {
var pageTracker = _gat._getTracker("UA-490257-27");
pageTracker._trackPageview();
} catch(err) {}</script>
      		
	</body>
</HTML>
