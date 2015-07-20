<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ObjectRoles.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Security.Modules.MetaClassViewControls.ObjectRoles" %>
<br />
<table width="100%">
	<tr>
		<td class="section"><asp:Literal ID="Literal3" Text="<%$Resources : IbnFramework.Security, ObjectRoles%>" runat="server"></asp:Literal></td>
	</tr>
</table>
<asp:GridView runat="server" ID="MainGrid" AutoGenerateColumns="false" Width="100%" BorderWidth="1" BorderColor="lightgray"
	CellPadding="4" GridLines="Both" AllowPaging="false" AllowSorting="false">
	<RowStyle HorizontalAlign="Center" />
	<Columns></Columns>
</asp:GridView>
<asp:LinkButton runat="server" ID="RefreshButton" Text="Refresh" OnClick="RefreshButton_Click" style="display:none;"></asp:LinkButton>
