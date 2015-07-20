<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResourceWorkControl.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ProjectManagement.Modules.ResourceWorkControl" %>
<%@ Reference Control="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Register TagPrefix="mc" TagName="McGrid" Src="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGridAction" Src="~/Apps/MetaUI/Grid/MetaGridServerEventAction.ascx" %>
<link type="text/css" rel="stylesheet" href='<%= Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/grid.css", this.Page) %>' />
<div style="zoom: 1;" class="text ibn-propertysheet">
	<mc:McGrid runat="server" ID="ctrlGrid" ClassName="ResourceWork" PlaceName="ManagementCenter" ShowCheckboxes="false" ShowPaging="true" DashboardMode="false" PrimaryKeyIdField="PrimaryKeyId" />
	<asp:Label ID="InfoLabel" Runat="server" CssClass="text"></asp:Label>
</div>
<asp:Button runat="server" ID="RefreshGridButton" onclick="RefreshGridButton_Click" style="display:none;" />
