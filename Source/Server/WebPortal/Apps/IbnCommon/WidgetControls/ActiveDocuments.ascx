<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActiveDocuments.ascx.cs" Inherits="Mediachase.UI.Web.Apps.Shell.Modules.ActiveDocuments" %>
<%@ Register Src="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" TagPrefix="Ibn" TagName="McGrid" %>
<div style="zoom: 1;" class="text ibn-propertysheet">
	<Ibn:McGrid runat="server" ID="ctrlGrid" ClassName="Document" PlaceName="Workspace" DashboardMode="true" ShowCheckboxes="false" />
</div>
<div id="divNoObjects" runat="server" style="padding:7px;">
	<asp:Label ID="lblNoObjects" runat="server" CssClass="text"></asp:Label>
</div>