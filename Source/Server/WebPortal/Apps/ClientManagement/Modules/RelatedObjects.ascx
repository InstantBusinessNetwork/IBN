<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RelatedObjects.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ClientManagement.Modules.RelatedObjects" %>
<%@ Reference Control="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Reference Control="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Reference Control="~/Apps/MetaUI/Grid/MetaGridServerEventAction.ascx" %>
<%@ Register TagPrefix="mc" TagName="MetaToolbar" Src="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGrid" Src="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGridAction" Src="~/Apps/MetaUI/Grid/MetaGridServerEventAction.ascx" %>
<script type="text/javascript">
function GetSelectedIds()
{
	var obj = $find('<%= grdMain.GridClientContainerId %>');
	if(obj && obj.isCheckboxes() && obj.isChecked())
		return obj.getCheckedCollection();
	return "";
}
</script>
<div style="padding:5px;"> 
<table class="ibn-propertysheet" cellspacing="0" cellpadding="0" border="0" width="100%" style="table-layout: fixed">
	<tr runat="server" id="ToolbarRow">
		<td class="ibn-stylebox2noBottom">
			<mc:MetaToolbar runat="server" ID="MainMetaToolbar" GridId="grdMain" />
		</td>
	</tr>
	<tr>
		<td >
			<mc:MCGrid ID="grdMain" runat="server" />
			<mc:MCGridAction runat="server" ID="ctrlGridEventUpdater" />
		</td>
	</tr>
</table>
</div>