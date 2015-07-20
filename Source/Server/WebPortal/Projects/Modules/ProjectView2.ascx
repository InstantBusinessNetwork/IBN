<%@ Reference Control="~/Modules/Favorites.ascx" %>
<%@ Reference Control="~/Modules/Reminder.ascx" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Reference Control="~/Modules/TopTabs.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectView2" Codebehind="ProjectView2.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="TopTab" src="~/Modules/TopTabs.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageViewMenu" src="~/Modules/PageViewMenu.ascx" %>
<script language="javascript">
function ShowWizard2(sLink,w,h,scroll)
{
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
	if (scroll) winprops+=',scrollbars=1';
	var f = window.open(sLink, "_blank", winprops);
}
function _XMLReqForClip(sUrl, sError)
{
	var req = window.XMLHttpRequest? 
		new XMLHttpRequest() : 
		new ActiveXObject("Microsoft.XMLHTTP");
	req.onreadystatechange = function() 
	{
		if (req.readyState != 4 ) return ;
		if (req.readyState == 4)
		{
			var oCurDiv = document.getElementById("divAlert");
			var oCurSpan = document.getElementById("spanAlert");
			if (req.status == 200)
				oCurSpan.innerHTML = req.responseText.toString();
			else
				oCurSpan.innerHTML = sError + ": " + req.statusText;
			oCurDiv.style.display = "block";
		}
	}
	var dt = new Date();
	var sID = dt.getMinutes() + "_" + dt.getSeconds() + "_" + dt.getMilliseconds();
	req.open("GET", '../Modules/XmlForTreeView.aspx?'+sUrl+"&sID="+sID, true);
	req.send(null);
}

function DeleteProject()
{
	if(confirm('<%=LocRM2.GetString("Warning")%>'))
		<%=Page.ClientScript.GetPostBackEventReference(lblDeleteProjectAll,"") %>
}

function OpenWindow(query,w,h,scroll)
{
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	
	winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
	if (scroll) winprops+=',scrollbars=1';
	var f = window.open(query, "_blank", winprops);
}
</script>

<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:PageViewMenu id="secHeader" title="" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="ibn-light">
			<div id="divAlert" align="center" style="PADDING: 5px; DISPLAY: none; ">
				<span id="spanAlert" class="ibn-alerttext" style="padding: 3px; border:1px solid red"></span>
			</div>
			<asp:PlaceHolder runat="server" ID="InfoPlaceHolder"></asp:PlaceHolder>
		</td>
	</tr>
	<tr>
		<td class="ibn-light">
			<ibn:TopTab id="ctrlTopTab" runat="server" />
		</td>
	</tr>
	<tr>
		<td style="padding:0">
			<asp:PlaceHolder ID="phItems" Runat="server"></asp:PlaceHolder>
		</td>
	</tr>
</table>

<asp:Button ID="btnSaveSnapshot" Runat="server" Visible="False" />
<asp:Button ID="btnAddToFavorites" Runat="server" Visible="False" />
<asp:Button ID="btnAddRelatedPrj" Runat="server" Visible="False" />
<asp:linkbutton id="lblDeleteProjectAll" runat="server" Visible="False" onclick="lblDeleteProjectAll_Click"></asp:linkbutton>
<asp:Button ID="btnToMSPrjSynch" Runat="server" Visible="False" onclick="btnToMSPrjSynch_Click" />
<asp:Button runat="server" ID="btnDeactivateFinance" Visible="false" />