<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListTimeTrackingWeek.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.Lists.ListTimeTrackingWeek" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Reference Control="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Register Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" TagPrefix="mc2" %>
<%@ Reference Control="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Register TagPrefix="mc" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGrid" Src="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Register TagPrefix="mc" TagName="MetaToolbar" Src="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>

<link rel="stylesheet" type="text/css" href='<%= Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/grid.css", this.Page) %>' />
<script type="text/javascript">
var resizeFlag = false;

function LayoutResizeHandler(sender, eventArgs)
{
}

function OpenWindow(query,w,h,scroll)
{
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	
	winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
	if (scroll) winprops+=',scrollbars=1';
	var f = window.open(query, "_blank", winprops);
}
</script>
<style type="text/css">
	.hundred
	{
		height: 100%;
		width: 100%;
		position: absolute;
	}
	.innerArea
	{
		background-color: #D6E8FF;
		border-top:1px solid #A6A6A6;
		border-left: 1px solid #6B92CE;
		border-right: 1px solid #6B92CE;
		border-bottom: 1px solid #6B92CE;
	}
	.innerContentArea
	{
		background-color: White;
		border:1px solid #BBD4F6;
	}
</style>
<mc2:McDock ID="DockTop" runat="server" Anchor="top" EnableSplitter="False" DefaultSize="58">
	<DockItems>
		<table cellspacing="0" cellpadding="0" border="0" width="100%" class="filter">
			<tr>
				<td class="bottomBorder">
					<mc:BlockHeader2 runat="server" ID="BlockHeaderMain" />
				</td>
			</tr>
			<tr>
				<td style="padding:0px;">
					&nbsp;
				</td>
			</tr>
			<tr>
				<td style="padding-left: 5px; padding-right: 5px;">
					<div class="noBottomBorder">
						<mc:MetaToolbar runat="server" ID="MainMetaToolbar" GridId="grdMain" />
					</div>		
				</td>
			</tr>	
		</table>
	</DockItems>
</mc2:McDock>
<table style="margin-top:0px; padding-top: 0px; table-layout: fixed;" cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td style="padding-left: 5px;" class="filter">
			<asp:UpdatePanel ID="grdMainPanel" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<mc:MCGrid ID="grdMain" runat="server" />
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>
</table>