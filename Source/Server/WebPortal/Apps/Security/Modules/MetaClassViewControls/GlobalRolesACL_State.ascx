<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GlobalRolesACL_State.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Security.Modules.MetaClassViewControls.GlobalRolesACL_State" %>
<br />
<table>
	<tr>
		<td class="section"><asp:Literal ID="Literal3" Text="<%$Resources : IbnFramework.Security, GlobalSecurityState%>" runat="server"></asp:Literal></td>
	</tr>
</table>
<table>
	<tr>
		<td>
			<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, StateMachine%>" />:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="StateMachineList" AutoPostBack="true" OnSelectedIndexChanged="StateMachineList_SelectedIndexChanged"></asp:DropDownList>
		</td>
		<td>
			<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, State%>" />:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="StateList" AutoPostBack="true" OnSelectedIndexChanged="StateList_SelectedIndexChanged"></asp:DropDownList>
		</td>
	</tr>
</table>
<asp:GridView runat="server" ID="MainGrid" AutoGenerateColumns="false" Width="100%" BorderWidth="1" BorderColor="lightgray"
	CellPadding="4" GridLines="Both" AllowPaging="false" AllowSorting="false" OnRowCommand="MainGrid_RowCommand" OnRowDataBound="MainGrid_RowDataBound">
	<RowStyle HorizontalAlign="Center" />
	<Columns></Columns>
</asp:GridView>
<asp:Button runat="server" ID="RefreshButton" OnClick="RefreshButton_Click" Text="Refresh" style="display:none" />
