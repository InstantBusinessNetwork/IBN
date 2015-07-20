<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageList.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules.PageList" %>
<%@ Register Src="~/Apps/MetaUIEntity/Grid/EntityGrid.ascx" TagName="Grid" TagPrefix="mc" %>
<%@ Register Src="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" TagName="Toolbar" TagPrefix="mc" %>
<%@ Register Src="~/Apps/MetaUI/Grid/MetaGridServerEventAction.ascx" TagName="GridEventAction" TagPrefix="mc" %>
<%@ Register TagPrefix="mc2" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<script type="text/javascript">
function LayoutResizeHandler(sender, eventArgs)
{
}
</script>
<table cellspacing="0" cellpadding="0" border="0" width="100%">
	<tr>
		<td style="padding-left:5px; padding-right:5px; padding-top:5px;">
			<div class="noBottomBorder">
				<mc:Toolbar runat="server" ID="ctrlToolbar" ClassName="CustomPage" ViewName="" PlaceName="PageList" ToolbarMode="ListViewUI" />
			</div>		
		</td>
	</tr>
	<tr>
		<td style="padding-bottom:2px;">
			<div style="margin-left:5px; margin-right:5px;">
				<mc:Grid runat="server" ID="ctrlGrid" ShowPaging="false" DashboardMode="false" ClassName="CustomPage" ViewName="" PlaceName="PageList" />
			</div>
		</td>
	</tr>
</table>
