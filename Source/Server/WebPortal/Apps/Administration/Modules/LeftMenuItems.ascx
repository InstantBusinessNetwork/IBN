<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeftMenuItems.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Administration.Modules.LeftMenuItems" %>
<%@ Reference Control="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Reference Control="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Reference Control="~/Apps/MetaUI/Grid/MetaGridServerEventAction.ascx" %>
<%@ Register TagPrefix="mc" TagName="MetaToolbar" Src="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGrid" Src="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGridAction" Src="~/Apps/MetaUI/Grid/MetaGridServerEventAction.ascx" %>
<div style="padding:5px;"> 
<table class="ibn-propertysheet" cellspacing="0" cellpadding="0" border="0" width="100%" style="table-layout: fixed">
	<tr runat="server" id="ToolbarRow">
		<td class="ibn-stylebox2noBottom">
			<mc:MetaToolbar runat="server" ID="MainMetaToolbar" GridId="grdMain" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:UpdatePanel runat="server" ID="GridUpdatePanel" UpdateMode="Always">
				<ContentTemplate>
					<mc:MCGrid ID="grdMain" runat="server" ShowPaging="false" GetCssFromColumn="true"/>
					<mc:MCGridAction runat="server" ID="ctrlGridEventUpdater" />
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>
</table>
</div>