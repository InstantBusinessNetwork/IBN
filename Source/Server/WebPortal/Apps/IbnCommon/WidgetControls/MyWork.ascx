<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyWork.ascx.cs" Inherits="Mediachase.UI.Web.Apps.Shell.Modules.MyWork" %>
<%@ Register Src="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" TagPrefix="Ibn" TagName="McGrid" %>
<div style="zoom: 1;" class="text ibn-propertysheet">
	<Ibn:McGrid runat="server" ID="ctrlGrid" ClassName="MyWork" PlaceName="Workspace" DashboardMode="true" ShowCheckboxes="false" />
	<asp:Label ID="NoCalendarLabel" Runat="server" CssClass="text"></asp:Label>
</div>
<asp:Button runat="server" ID="RefreshGridButton" onclick="RefreshGridButton_Click" style="display:none;" />