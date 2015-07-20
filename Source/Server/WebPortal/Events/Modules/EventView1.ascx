<%@ Reference Control="~/Modules/Favorites.ascx" %>
<%@ Reference Control="~/Modules/Reminder.ascx" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Reference Control="~/Modules/TopTabs.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Events.Modules.EventView1" Codebehind="EventView1.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="TopTab" src="../../Modules/TopTabs.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageViewMenu" src="../../Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="EventShortInfo" src="EventShortInfo.ascx" %>
<script language="javascript">
function DeleteEvent()
{
	if(confirm('<%=LocRM.GetString("Warning")%>'))
		<%=Page.ClientScript.GetPostBackEventReference(lbDeleteEventAll,"") %>
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
			<ibn:EventShortInfo id="ucEventShortInfo" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="ibn-light">
			<ibn:TopTab id="ctrlTopTab" runat="server" />
		</td>
	</tr>
	<tr>
		<td style="padding-left:7px; padding-right:7px; padding-bottom:7px">
			<asp:PlaceHolder ID="phItems" Runat="server" /></asp:PlaceHolder>
		</td>
	</tr>
</table>

<asp:Button ID="btnAddToFavorites" Runat="server" Visible="False" />
<asp:LinkButton id="lbDeleteEventAll" runat="server" Visible="False" onclick="lbDeleteEventAll_Click"></asp:LinkButton>