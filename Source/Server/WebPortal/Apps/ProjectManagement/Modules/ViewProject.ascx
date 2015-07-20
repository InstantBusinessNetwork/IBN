<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewProject.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ProjectManagement.Modules.ViewProject" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Reference Control="~/Apps/IbnCommon/Modules/ViewObjectNoBind.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageViewMenu" Src="~/Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ViewObjectNoBind" Src="~/Apps/IbnCommon/Modules/ViewObjectNoBind.ascx" %>

<script type="text/javascript">
//<![CDATA[
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
//]]>
</script>

<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-stylebox2 ibn-propertysheet">
	<tr>
		<td>
			<ibn:PageViewMenu ID="secHeader" Title="" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="ibn-light">
			<div id="divAlert" align="center" style="padding: 5px; display: none;">
				<span id="spanAlert" class="ibn-alerttext" style="padding: 3px; border: 1px solid red"></span>
			</div>
			<asp:PlaceHolder runat="server" ID="InfoPlaceHolder"></asp:PlaceHolder>
		</td>
	</tr>
	<tr>
		<td valign="top">
			<ibn:ViewObjectNoBind runat="server" ID="ViewObjectControl" ClassName="Project" MenuWidth="150"></ibn:ViewObjectNoBind>
		</td>
	</tr>
</table>
<asp:Button ID="btnSaveSnapshot" runat="server" Visible="False" />
<asp:Button ID="btnAddToFavorites" runat="server" Visible="False" />
<asp:Button ID="btnAddRelatedPrj" runat="server" Visible="False" />
<asp:LinkButton ID="lblDeleteProjectAll" runat="server" Visible="False" OnClick="lblDeleteProjectAll_Click"></asp:LinkButton>
<asp:Button ID="btnToMSPrjSynch" runat="server" Visible="False" OnClick="btnToMSPrjSynch_Click" />
<asp:Button runat="server" ID="btnDeactivateFinance" Visible="false" />
