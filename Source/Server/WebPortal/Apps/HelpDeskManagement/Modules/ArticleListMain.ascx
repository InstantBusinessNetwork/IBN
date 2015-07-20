<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArticleListMain.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules.ArticleListMain" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Reference Control="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Reference Control="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Register TagPrefix="mc" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGrid" Src="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Register TagPrefix="mc" TagName="MetaToolbar" Src="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Register Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" TagPrefix="mc2" %>
<%@ Reference Control="~/Modules/TagCloud.ascx"  %>

<%@ Register TagPrefix="ibn" TagName="TagCloud" Src="~/Modules/TagCloud.ascx" %>
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
<mc2:McDock ID="DockMainTop" runat="server" Anchor="top" EnableSplitter="False" DefaultSize="21">
	<DockItems>
		<table cellspacing="0" cellpadding="0" border="0" width="100%" class="filter">
			<tr>
				<td class="bottomBorder">
					<mc:BlockHeader2 runat="server" ID="BlockHeaderMain" />
				</td>
			</tr>
		</table>
	</DockItems>
</mc2:McDock>

<mc2:McDock ID="DockTop" runat="server" Anchor="top" EnableSplitter="False" DefaultSize="60">
	<DockItems>
		<asp:UpdatePanel ID="upToolbarSearch" runat="server" UpdateMode="Conditional">
			<ContentTemplate>
				<table cellspacing="0" cellpadding="0" border="0" width="100%" class="filter">
					<tr>
						<td style="padding:7px;">
							<asp:TextBox ID="txtSearch" runat="server" Width="150px" CssClass="text"></asp:TextBox>
							<asp:Button CssClass="text" ID="btnSearch" runat="server" OnClick="btnSearch_Click" />
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
			</ContentTemplate>
		</asp:UpdatePanel>
	</DockItems>
</mc2:McDock>	
<mc2:McDock ID="DockLeft" InnerCssClass="hundred" runat="server" Anchor="Left" EnableSplitter="false" DefaultSize="250">
	<DockItems>
		<div style="height: 100%; width: 100%; position: relative;" class="filter ibn-propertysheet">
			<div style="position: absolute; left:5px; bottom: 3px; top: 0px; right:0px;" class="innerArea">
				<div style="position:absolute;left:9px;right:9px;top:9px;bottom:9px;" class="innerContentArea">
					<div style="padding:5px;">
						<asp:UpdatePanel ID="upTagCloud" runat="server" UpdateMode="Conditional">
							<ContentTemplate>
								<ibn:TagCloud runat="server" id="ctrlTagCloud" ObjectType="20" TagCount="30" TagSizeCount="13" OnTagClick="ctrlTagCloud_TagClick"></ibn:TagCloud>
							</ContentTemplate>
						</asp:UpdatePanel>
					</div>
				</div>
			</div>
		</div>
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