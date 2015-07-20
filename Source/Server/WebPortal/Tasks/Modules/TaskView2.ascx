<%@ Reference Control="~/Modules/Favorites.ascx" %>
<%@ Reference Control="~/Modules/Reminder.ascx" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Reference Control="~/Modules/TopTabs.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.TaskView2" Codebehind="TaskView2.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="TopTab" src="../../Modules/TopTabs.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageViewMenu" src="../../Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="TaskShortInfo" src="TaskShortInfo.ascx" %>
<script type="text/javascript">

function DeleteTask()
{
	if(confirm('<%=LocRM.GetString("Warning")%>'))
		<%=Page.ClientScript.GetPostBackEventReference(lbDeleteTaskAll,"") %>
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
			<asp:Panel ID="apShared" Runat="server" CssClass="ibn-propertysheet ibn-navline ibn-alternating" style="PADDING-RIGHT:5px; PADDING-LEFT:5px; PADDING-BOTTOM:5px; PADDING-TOP:5px"><IMG height="16" src="../Layouts/images/caution.gif" width="16" align="absMiddle" border="0">
				&nbsp;<%=LocRM.GetString("SharedTask") %> 
				<asp:Label id="lblEntryOwner" Runat="server"></asp:Label>
			</asp:Panel>
		</td>
	</tr>
	<tr>
		<td class="ibn-light">
			<ibn:TaskShortInfo id="ucTaskShortInfo" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="ibn-light">
			<ibn:TopTab id="ctrlTopTab" runat="server" />
		</td>
	</tr>
	<tr>
		<td style="padding-left:7px; padding-right:7px; padding-bottom:7px">
			<asp:PlaceHolder ID="phItems" Runat="server" />
		</td>
	</tr>
</table>

<asp:LinkButton id="lbDeleteTaskAll" runat="server" Visible="False"></asp:LinkButton>
<asp:Button ID="btnAddToFavorites" Runat="server" Visible="False"></asp:Button>