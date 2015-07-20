<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomPageDesign.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules.CustomPageDesign" %>
<%@ Reference VirtualPath="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register Src="~/Apps/WidgetEngine/Modules/WsViewer.ascx" TagName="WsViewer" TagPrefix="ibn" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:1px">
	<tr>
		<td><ibn:blockheader id="MainHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="padding:10px;" class="ibn-navline ibn-alternating">
			<table cellpadding="0" cellspacing="5" width="100%" border="0" style="table-layout:fixed;">
				<tr>
					<td class="ibn-label" style="width:80px;">
						<asp:Literal runat="server" ID="Literal2" Text="<%$Resources: IbnFramework.Admin, Page %>" />: 
					</td>
					<td class="ibn-value">
						<asp:Label runat="server" ID="PageLabel"></asp:Label>
					</td>
					<td class="ibn-label" style="width:130px;">
						<asp:Label runat="server" ID="LayerLabel"></asp:Label>
					</td>
					<td>
						<asp:HyperLink runat="server" ID="LayerLink"></asp:HyperLink>
					</td>
				</tr>
				<tr>
					<td colspan="4" style="padding-top:5px;">
						<asp:LinkButton runat="server" ID="ClearUserSettingsButton" onclick="ClearUserSettingsButton_Click" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<ibn:WsViewer runat="server" ID="WsViewerControl" IsAdmin="true" />
		</td>
	</tr>
</table>
