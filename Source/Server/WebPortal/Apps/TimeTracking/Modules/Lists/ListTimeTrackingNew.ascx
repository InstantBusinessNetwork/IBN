<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListTimeTrackingNew.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.Lists.ListTimeTrackingNew" %>
<%@ Reference Control="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" TagPrefix="mcWc" %>
<%@ Register Namespace="Mediachase.Ibn.Web.UI.Controls.Util" Assembly="Mediachase.Ibn.Web.UI.Controls.Util" TagPrefix="mc" %>
<%@ Register
    Assembly="Mediachase.UI.Web"
    Namespace="Mediachase.Ibn.Web.UI.TimeTracking"
    TagPrefix="mc2" %>
<%@ Register Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" TagPrefix="mc2" %>
<%@ Register Src="~/Apps/TimeTracking/Modules/PublicControls/TTFilterPopupEdit.ascx" TagName="TTFilterMain" TagPrefix="Ibn" %>
<%@ Register Src="~/Apps/TimeTracking/Modules/PublicControls/ObjectDetails.ascx" TagName="ObjDetails" TagPrefix="Ibn" %>
<%@ Register Src="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" TagPrefix="ibn" TagName="MetaToolbar" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MetaGridServer" Src="~/Apps/MetaUI/Grid/MetaGridServer.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MetaGridServerEventAction" Src="~/Apps/MetaUI/Grid/MetaGridServerEventAction.ascx" %>

<link type="text/css" rel="stylesheet" href='<%= Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/grid.css", this.Page) %>' />    
<link type="text/css" rel="stylesheet" href='<%= Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/calendar.css", this.Page)%>' />

<link type="text/css" rel="stylesheet" href='<%= Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/TimeSheetStyle.css", this.Page)%>' />
<script type="text/javascript">
    var resizeFlag = false;
    function LayoutResizeHandler(sender, eventArgs)
    {
		//alert('resized fired: '+eventArgs._name+':'+eventArgs._blockHeight+'x'+eventArgs._blockWidth);

		if (eventArgs._name == "spDetailsH")
		{
			var s = new Object();
			s.ViewName = Request.QueryString.Item("viewName").toString();
			
			Mediachase.Ibn.Web.UI.WebServices.ListHandler.LayoutResized(Sys.Serialization.JavaScriptSerializer.serialize(s), "south", eventArgs._blockHeight);
		}
    }
    
    function LayoutEndResizeHadler(sender, eventArgs)
    {
		//resizeFlag = true;
    }
    
    function executeEvalScript(scriptToExecute)
	{
		eval(scriptToExecute);
	}
</script>   

<mc2:McDock ID="DockTop" runat="server" Anchor="top" EnableSplitter="False" DefaultSize="95">
	<DockItems>	
		<div class="bottomBorder">
			<ibn:BlockHeader2 runat="server" ID="BlockHeaderMain" Title="" />
		</div>
		<asp:UpdatePanel runat="server" id="FiltersControls" UpdateMode="Always">
			<ContentTemplate>
				<table style="margin-top:0px;" cellspacing="0" cellpadding="0" border="0" width="100%">
					<tr>
						<td class="filter" style="height:5px;">
							<Ibn:TTFilterMain ID="ctrlTTFilterMain" runat="server" />
						</td>
					</tr>
					<tr >
						<td class="filter" style="padding-left: 5px; padding-right: 5px;">
							<div class="noBottomBorder">
								<ibn:MetaToolbar runat="server" ID="MainMetaToolbar" GridId="ctrlGrid"  />
							</div>		
						</td>
					</tr>	
				</table>
			</ContentTemplate>
		</asp:UpdatePanel>
	</DockItems>
</mc2:McDock>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="panelGridGeneral" EnableViewState="true" DynamicLayout="true">
	<ProgressTemplate>
		<div style="height: 20%; width: 30%; position: absolute; left: 35%; top: 40%; z-index: 10000; background-color: White; border: solid 1px #AAAAAA;">
			<div style="position: absolute; left: 45%; top: 40%;">
				<img src='<%= this.ResolveClientUrl("~/Images/IbnFramework/loading_rss.gif") %>' alt='loading' height="16" width="16" />
			</div>
		</div>
	</ProgressTemplate>
</asp:UpdateProgress>
<table style="margin-top:0px; padding-top: 0px; table-layout: fixed;" cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td style="padding-left: 5px;" class="filter">
		<asp:UpdatePanel runat="server" ID="panelGridGeneral" UpdateMode="Conditional" EnableViewState="true">
			<ContentTemplate>
				<div style="position: absolute; left: 10px; top: 10px; width: 100px; height: 100px; border:solid 1px Black; display: none;">
					<asp:Button runat="server" ID="btnOk" Text="PostBack" />
				</div>
				<ibn:MetaGridServer runat="server" id="MetaGridControl" PlaceName=""></ibn:MetaGridServer>
				<ibn:MetaGridServerEventAction runat="server" ID="ctrlGridEventUpdater" PlaceName="" ViewName="" ClassName=""  />
			</ContentTemplate>
		</asp:UpdatePanel>
		</td>
		<td style="width: 5px" class="filter">&nbsp;</td>
	</tr>
</table>

<%--<Ibn:Dock ID="DockBottom" runat="server" Anchor="Bottom" FriendlyName="spDetailsH" ShowScrolling="true" CssClass="DockContainer2" InnerCssClass="DockContainerInner" DefaultBorderSize="4">
	<DockItems>
			<div id="Div1" runat="server" class="filter" style="position: relative;" >
				<div style="background-color:White; position: absolute; bottom: 2px; top: 2px; left:2px; right: 2px;">
					<asp:UpdatePanel runat="server" ID="DetailsPanel" UpdateMode="Conditional">
						<ContentTemplate>
							<Ibn:ObjDetails ID="ctrlObjDet" runat="server" />
							<asp:HiddenField runat="server" ID="testHF" />
						</ContentTemplate>
					</asp:UpdatePanel>	
				</div>
			</div>
	</DockItems>
</Ibn:Dock>
<Ibn:Dock runat="server" ID="dockBottomMain2" Anchor="bottom" EnableSplitter="False" DefaultSize="5">
	<DockItems>
		<div style="height:4px; background-color: #DFE8F6;">&nbsp;</div>
	</DockItems>
</Ibn:Dock>	--%>
