<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ObjectRoles_StateTransitions.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Security.Modules.MetaClassViewControls.ObjectRoles_StateTransitions" %>
<br />
<table width="100%">
	<tr>
		<td class="section">
			<asp:Literal ID="Literal4" Text="<%$Resources : IbnFramework.Security, StateTransitions%>" runat="server"></asp:Literal>
		</td>
	</tr>
</table>
<table>
	<tr>
		<td>
			<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, StateMachine%>" />:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="ddlStateMachine" AutoPostBack="true" OnSelectedIndexChanged="ddlStateMachine_SelectedIndexChanged"></asp:DropDownList>
		</td>
		<td>
			<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, State%>" />:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="ddlState" AutoPostBack="true" OnSelectedIndexChanged="ddlState_SelectedIndexChanged"></asp:DropDownList>
		</td>
	</tr>
</table>
<asp:GridView runat="server" ID="grdMain" AutoGenerateColumns="false" Width="100%" BorderWidth="1" BorderColor="lightgray"
	CellPadding="4" GridLines="Both" AllowPaging="false" AllowSorting="false">
	<RowStyle HorizontalAlign="Center" />
	<Columns></Columns>
</asp:GridView>
<asp:LinkButton runat="server" ID="lnkRefresh" Text="Refresh" OnClick="lnkRefresh_Click" style="display:none;"></asp:LinkButton>
