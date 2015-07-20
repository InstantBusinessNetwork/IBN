<%@ Reference Control="~/Modules/Favorites.ascx" %>
<%@ Reference Control="~/Modules/Reminder.ascx" %>
<%@ Reference Control="~/Modules/History.ascx" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Reference Control="~/Modules/TopTabs.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.ToDoView2" Codebehind="ToDoView2.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="TopTab" src="../../Modules/TopTabs.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageViewMenu" src="../../Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ToDoInfo" src="ToDoInfo.ascx" %>
<script type="text/javascript">
//<![CDATA[
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

function DeleteToDoModal2()
{
	if(confirm('<%=LocRM.GetString("Warning")%>'))
		<%=Page.ClientScript.GetPostBackEventReference(lbDeleteToDoAll,"") %>
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

<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:PageViewMenu id="secHeader" title="" runat="server" />
			<asp:Panel ID="apShared" Runat="server" CssClass="ibn-propertysheet ibn-navline ibn-alternating" style="PADDING-RIGHT:5px; PADDING-LEFT:5px; PADDING-BOTTOM:5px; PADDING-TOP:5px"><IMG height="16" src="../Layouts/images/caution.gif" width="16" align="absMiddle" border="0">
				&nbsp;<%=LocRM.GetString("SharedToDo") %> 
				<asp:Label id="lblEntryOwner" Runat="server"></asp:Label>
			</asp:Panel>
		</td>
	</tr>
	<tr>
		<td class="ibn-light">
			<div id="divAlert" align="center" style="PADDING: 5px; DISPLAY: none; ">
				<span id="spanAlert" class="ibn-alerttext" style="padding: 3px; border:1px solid red"></span>
			</div>
			<ibn:ToDoInfo id="ucToDoInfo" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="ibn-light">
			<ibn:TopTab id="ctrlTopTab" runat="server" />
		</td>
	</tr>
	<tr>
		<td style="padding-left:7px; padding-right:7px; padding-bottom:7px">
			<asp:PlaceHolder ID="phItems" Runat="server" ></asp:PlaceHolder>
		</td>
	</tr>
</table>
<asp:LinkButton id="lbDeleteToDoAll" runat="server" Visible="False" onclick="lbDeleteToDoAll_Click"></asp:LinkButton>
<asp:Button ID="btnAddToFavorites" Runat="server" Visible="False" />