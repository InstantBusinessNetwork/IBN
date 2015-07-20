<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Transitions.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.StateMachine.Modules.MetaClassViewControls.Transitions" %>
<br />
<table width="100%">
	<tr>
		<td class="section"><asp:Literal ID="Literal2" Text="<%$Resources : IbnFramework.GlobalMetaInfo, AllowedTransitions%>" runat="server"></asp:Literal></td>
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
	</tr>
</table>
<asp:GridView runat="server" ID="grdMain" AutoGenerateColumns="false" Width="100%" BorderWidth="1" BorderColor="lightgray"
	CellPadding="4" GridLines="Both" AllowPaging="false" AllowSorting="false">
	<Columns>
	</Columns>
</asp:GridView>
<asp:Button runat="server" ID="btnRefresh" OnClick="btnRefresh_Click" Text="Refresh" style="display:none" />
