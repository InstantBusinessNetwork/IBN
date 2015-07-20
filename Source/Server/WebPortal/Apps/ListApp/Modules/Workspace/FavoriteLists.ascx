<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FavoriteLists.ascx.cs" Inherits="Mediachase.UI.Web.Apps.ListApp.Modules.Workspace.FavoriteLists" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<asp:DataGrid id="grdMain" Runat="server" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False"
	AllowPaging="true" AllowSorting="False" CellSpacing="0" PageSize="10" PagerStyle-Visible="true"
	PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2"
	ShowHeader="true" EnableViewState="True">
	<PagerStyle CssClass="text ibn-TPHeader" HorizontalAlign="Right"></PagerStyle>	
	<Columns>
		<asp:BoundColumn DataField="ObjectId" Visible="False"></asp:BoundColumn>
		<asp:BoundColumn HeaderStyle-Width="70%" HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" DataField="Name" ></asp:BoundColumn>
		<%--<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" DataField="UserName" ></asp:BoundColumn>--%>
	</Columns>
</asp:DataGrid>
<asp:Panel runat="server" ID="panelNoFavList" Visible="False">
	<div style="padding: 7px; color: Gray; font-family: Verdana;">
		<asp:Literal runat="server" Text="<%$Resources: IbnFramework.ListInfo, NoFavoriteLists %>"></asp:Literal>
	</div>
</asp:Panel>
