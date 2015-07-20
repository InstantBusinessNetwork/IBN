<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResourceUtilGraphForObject.ascx.cs" Inherits="Mediachase.UI.Web.Common.Modules.ResourceUtilGraphForObject" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Reference Control="~/Projects/Modules/ResourceUtilGraphControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="GraphControl" Src="~/Projects/Modules/ResourceUtilGraphControl.ascx" %>
<script type="text/javascript">
function resizeTable()
{
	var obj = document.getElementById('MainDiv');
	var toolbarRow = document.getElementById('ToolbarRow');

	var intHeight = 0;
	if (typeof(window.innerWidth) == "number") 
	{
		intHeight = window.innerHeight;
	} 
	else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) 
	{
		intHeight = document.documentElement.clientHeight;
	} 
	else if (document.body && (document.body.clientWidth || document.body.clientHeight)) 
	{
		intHeight = document.body.clientHeight;
	}
	obj.style.height = (intHeight - toolbarRow.offsetHeight - 2) + "px";
}
window.onresize=resizeTable;
window.onload=resizeTable;
</script>
<table class="ibn-stylebox text" style="margin-top: 0px" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr id="ToolbarRow">
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<div id="MainDiv" style="overflow-y:auto;">
				<div style="height:100%">
					<ibn:GraphControl runat="server" id="GraphControlMain"></ibn:GraphControl>
				</div>
			</div>
		</td>
	</tr>
</table>
<asp:Button ID="ViewButton" runat="server" Visible="false" OnCommand="ViewButton_Command"/>