<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListAppList.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.EntityControls.ListAppList" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Reference Control="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Grid/EntityGrid.ascx" %>
<%@ Register TagPrefix="mc2" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" Assembly="Mediachase.UI.Web" Namespace="Mediachase.UI.Web.Modules" %>
<%@ Register TagPrefix="mc" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="mc" TagName="EntityGrid" Src="~/Apps/MetaUIEntity/Grid/EntityGrid.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGridAction" Src="~/Apps/MetaUI/Grid/MetaGridServerEventAction.ascx" %>
<%@ Register TagPrefix="mc" TagName="MetaToolbar" Src="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>

<link rel="stylesheet" type="text/css" href='<%= Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/grid.css", this.Page) %>' />
<script type="text/javascript">
var resizeFlag = false;

function LayoutResizeHandler(sender, eventArgs)
{
}

function checkKey(e) 
{ 
	var _key = e.keyCode ? e.keyCode : e.which ? e.which : e.charCode;
	try {
		if (_key == 13)
			<%= Page.ClientScript.GetPostBackEventReference(btnSearch, "") %>
		else
			return true;
	}
	catch (e) {return true;}
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
<mc2:McDock ID="DockTop" runat="server" Anchor="top" EnableSplitter="False" DefaultSize="101">
	<DockItems>
		<table cellspacing="0" cellpadding="0" border="0" width="100%" class="filter">
			<tr>
				<td class="bottomBorder">
					<mc:BlockHeader2 runat="server" ID="BlockHeaderMain" />
				</td>
			</tr>
			<tr>
				<td style="padding:0px;">
					<asp:UpdatePanel ID="upFilters" runat="server" UpdateMode="Conditional">
						<ContentTemplate>
							<table cellpadding="5" cellspacing="0" width="100%" border="0" class="ibn-propertysheet" style="table-layout:fixed;">
								<colgroup>
									<col width="320px" />
									<col width="21px" />
									<col width="24px" />
									<col align="right" />
								</colgroup>
								<tr>
									<td width="320px" style="padding:5px 5px 4px 12px;">
										<asp:TextBox ID="txtSearch" runat="server" Width="300px" CssClass="text" onkeypress="return checkKey(event);"></asp:TextBox>			
									</td>
									<td width="21px" style="padding:5px 5px 4px 0px;"><asp:ImageButton Runat="server" id="btnSearch" Width="16" Height="16" ImageUrl="~/layouts/images/search.gif" ImageAlign="AbsMiddle" OnClick="btnSearch_Click" /></td>
									<td width="24px"style="padding:5px 5px 4px 0px;"><asp:ImageButton runat="server" ID="btnClear" Width="19" Height="17" ImageUrl="~/Layouts/Images/reset17.gif" ImageAlign="AbsMiddle" OnClick="btnClear_Click" /></td>
									<td align="right" style="padding:5px 5px 4px 0px;">
										&nbsp;
									</td>
								</tr>
								<tr id="trFilterText" runat="server">
									<td style="padding:2px 0px 6px 12px;" colspan="4">
										<div id="spanFilters" runat="server" style="height:12px;padding:3px;overflow:hidden;font-size:smaller;">
											<nobr><asp:Label ID="FilterIsSet" runat="server"></asp:Label></nobr>
										</div>
									</td>
								</tr>
							</table>
						</ContentTemplate>
					</asp:UpdatePanel>
				</td>
			</tr>
			<tr>
				<td style="padding-left: 5px; padding-right: 5px;">
					<div class="noBottomBorder">
						<asp:UpdatePanel ID="upToolbar" runat="server" UpdateMode="conditional">
							<ContentTemplate>
								<mc:MetaToolbar runat="server" ID="MainMetaToolbar" GridId="grdMain" />
							</ContentTemplate>
						</asp:UpdatePanel>
					</div>		
				</td>
			</tr>	
		</table>
	</DockItems>
</mc2:McDock>
	
<mc2:McDock ID="DockLeft" InnerCssClass="hundred" runat="server" Anchor="Left" EnableSplitter="false" DefaultSize="220">
	<DockItems>
		<div style="height: 100%; width: 100%; position: relative;" class="filter">
			<div style="position: absolute; left:5px; bottom: 3px; top: 0px; right:0px;" class="innerArea">
			<asp:UpdatePanel ID="upLeftArea" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<div style="position:absolute;left:9px;right:9px;top:9px;bottom:9px;" class="innerContentArea">
						<div style="height:100%;">
							<mc2:JsTreePanel FillToContainer="true" RootVisible="false" AutoScroll="true" id="jsTreeExt" runat="server" IconCss="iconStdCls" NodeTextCss="nodeCls"></mc2:JsTreePanel>	
						</div>
					</div>
				</ContentTemplate>
			</asp:UpdatePanel>
			</div>
		</div>
	</DockItems>
</mc2:McDock>
<table style="margin-top:0px; padding-top: 0px; table-layout: fixed;" cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td style="padding-left: 5px;" class="filter">
			<asp:UpdatePanel ID="grdMainPanel" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<mc:EntityGrid ID="grdMain" runat="server" />	
					<mc:MCGridAction runat="server" ID="ctrlGridEventUpdater"  />
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>
</table>