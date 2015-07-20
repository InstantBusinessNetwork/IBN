<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.UI.Web.Workspace.Modules.MCNews"
	Codebehind="MCNews.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%"
	border="0" style="margin: 0;">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" Title="" runat="server" Visible="false"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<div id="divMCNews" style="padding: 5px;">
			</div>
		</td>
	</tr>
</table>
<asp:LinkButton ID="lbHide" runat="server" Visible="False"></asp:LinkButton>

<script type="text/javascript">
<!--
var rsslastTime = null;
var waitTime = 9000;
var fl = false;
window.setTimeout("CheckTime()", 1000);
function CheckTime()
{
  if(!fl && rsslastTime)
  {
    var rsscurTime = (new Date()).getTime();
    if (rsscurTime - rsslastTime > waitTime)
    {
      document.getElementById('divMCNews').innerHTML = "<div style='text-align:center;padding:10px;color:red;' class='text'>"+'<%= LocRM3.GetString("tRSSProblems")%>'+"</div>";
      return;
    }
    else
    {
      window.setTimeout("CheckTime()", 1000);
    }
  }
}
function CreateRssNews(feedUrl, feedCount, containerId)
{
  rsslastTime = (new Date()).getTime();
  var img = '<%= ResolveClientUrl("~/Layouts/Images/loading_rss.gif") %>';
  document.getElementById(containerId).innerHTML = "<div style='padding:10px;text-align:center;'><img alt='Loading...' style='vertical-align:middle;border:0' src='" + img + "' /></div>";
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
  	if (ajaxRequest.readyState == 4)
  	{
  		fl = true;
  		var cont = document.getElementById(containerId);
  		if (cont)
  			cont.innerHTML = ajaxRequest.responseText;
  	}
  }
	ajaxRequest.open("GET", '<%= this.ResolveUrl("~/Modules/XmlForTreeView.aspx") %>' + '?RssPath='+feedUrl+'&RssCount='+feedCount, true);
	ajaxRequest.send(null); 
}
//-->
</script>
