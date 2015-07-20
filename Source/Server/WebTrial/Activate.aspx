<%@ Page language="c#" Inherits="Mediachase.Ibn.WebTrial.Activate" Codebehind="Activate.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Activation</title>
		<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
		<script src='<%=ResolveUrl("~/layouts/buttons.js") %>'></script>
		<script src='<%=ResolveUrl("~/layouts/browser.js") %>'></script>
		<link href='<%=ResolveUrl("~/layouts/windows.css") %>' type=text/css rel=stylesheet>
		<link href='<%=ResolveUrl("~/layouts/XP/theme.css") %>' rel=stylesheet>
		<link href='<%=ResolveUrl("~/layouts/XP/color1.css") %>' rel=stylesheet>
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
	<body style="TEXT-ALIGN: center">
		<form id="mcSignupForm" method="post" runat="server">
			<table cellspacing="0" cellpadding="3" width="725" border="0" align="center">
				<tr>
					<td>
					  <asp:image id="imgHeader" EnableViewState="False" ImageUrl="Layouts/ibn2003_masthead.gif" BorderWidth="0" Runat="server"></asp:image>
					  <br />
						<br />
						<div ID="divTitle" Runat="server" class="ibn-sectionheader" style="DISPLAY: inline"></div>
						<br />
						<br />
						<div class="ibn-propertysheet" id="divMess" runat="server"></div>
						<div class="ibn-propertysheet" id="divFailed" runat="server" style="display:none;"></div>
					</td>
				</tr>
			</table>
			<script type="text/javascript">
        function ActivatePortal(feedUrl, containerId, containerTitleId)
        {
          document.getElementById(containerId).innerHTML = "<div style='padding:10px;text-align:center;'><img align='absmiddle' border='0' src='Layouts/loading8.gif' /></div>";
          document.getElementById(containerTitleId).innerHTML = '<%= LocRM.GetString("ActProgress")%>';
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
                document.getElementById(containerTitleId).innerHTML = '<%= LocRM.GetString("ActivationError")%>';
              }
              else
              {
                document.getElementById(containerId).innerHTML = ajaxRequest.responseText;
                document.getElementById(containerTitleId).innerHTML = '<%= LocRM.GetString("Congratulations")%>';
              }
            }
	        }
	        ajaxRequest.open("GET", feedUrl, true);
	        ajaxRequest.send(null); 
        }
        </script>
		</form>
	</body>
</HTML>
