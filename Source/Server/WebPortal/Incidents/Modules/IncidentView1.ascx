<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.IncidentView1" Codebehind="IncidentView1.ascx.cs" %>
<%@ Reference Control="~/Modules/TopTabs.ascx" %>
<%@ Reference Page="~/Incidents/IncidentView.aspx" %>
<%@ Reference Control="~/Modules/Favorites.ascx" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="TopTab" src="~/Modules/TopTabs.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageViewMenu" src="~/Modules/PageViewMenu.ascx" %>
<script type="text/javascript">
//<![CDATA[
function DeleteIncident()
{
	if(confirm('<%=LocRM.GetString("Warning") %>'))
		<%=Page.ClientScript.GetPostBackEventReference(lbDeleteIncidentAll,"") %>
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
//]]>
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:PageViewMenu id="secHeader" title="" runat="server" />
			<asp:Panel ID="apShared" Runat="server" CssClass="ibn-propertysheet ibn-navline ibn-alternating" style="padding-right:5px; padding-left:5px; padding-bottom:5px; padding-top:5px"><img alt='' src="../Layouts/images/caution.gif" align="absMiddle" border="0"/>
				&nbsp;<%=LocRM.GetString("SharedIssue") %> 
				<asp:Label id="lblEntryOwner" Runat="server"></asp:Label>
			</asp:Panel>
		</td>
	</tr>
	<tr>
		<td class="ibn-light">
			<div id="divAlert" align="center" style="padding: 5px; display: none; ">
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
		<td style="padding-left:7px; padding-right:7px; padding-bottom:7px">
			<asp:PlaceHolder ID="phItems" Runat="server"></asp:PlaceHolder>
		</td>
	</tr>
</table>

<asp:Button ID="btnAddToFavorites" Runat="server" Visible="False" />
<asp:Button ID="btnAddRelatedIss" Runat="server" Visible="False" />
<asp:LinkButton id="lbDeleteIncidentAll" runat="server" Visible="False" onclick="lbDeleteIncidentAll_Click"></asp:LinkButton>