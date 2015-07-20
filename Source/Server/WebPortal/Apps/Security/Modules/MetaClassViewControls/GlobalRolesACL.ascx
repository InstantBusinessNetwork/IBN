<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GlobalRolesACL.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Security.Modules.MetaClassViewControls.GlobalRolesACL" %>
<br />
<table width="100%">
	<tr>
		<td class="section"><asp:Literal ID="Literal3" Text="<%$Resources : IbnFramework.Security, GlobalSecurity%>" runat="server"></asp:Literal></td>
		<td align="right"><asp:HyperLink runat="server" ID="NewLink"></asp:HyperLink></td>
	</tr>
</table>
<asp:GridView runat="server" ID="MainGrid" AutoGenerateColumns="false" Width="100%" BorderWidth="1" BorderColor="lightgray"
	CellPadding="4" GridLines="Both" AllowPaging="false" AllowSorting="false" OnRowCommand="MainGrid_RowCommand" OnRowDeleting="MainGrid_RowDeleting">
	<RowStyle HorizontalAlign="Center" />
	<Columns></Columns>
</asp:GridView>
<asp:Button runat="server" ID="RefreshButton" OnClick="RefreshButton_Click" Text="Refresh" style="display:none" />
