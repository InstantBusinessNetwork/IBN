<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeTrackingBlockTypeEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.ManageControls.TimeTrackingBlockTypeEdit" %>
<table style="width:100%;"><tr><td style="padding:10px;">
	<table cellpadding="5" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
		<tr>
			<td style='width:<%= labelColumnWidth%>'>
				<asp:Literal ID="Literal1" Text="<%$Resources : IbnFramework.TimeTracking, BlockTypeName%>" runat="server"></asp:Literal>:
			</td>
			<td>
				<asp:TextBox runat="server" ID="txtName" Width="100%"></asp:TextBox>
			</td>
			<td style="width:20px;">
				<asp:RequiredFieldValidator runat="server" ID="rfvName" ControlToValidate="txtName" Display="dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td>
				<asp:Literal ID="Literal4" Text="<%$Resources : IbnFramework.GlobalMetaInfo, StateMachine%>" runat="server"></asp:Literal>:
			</td>
			<td>
				<asp:DropDownList runat="server" ID="ddlStateMachine" Width="100%"></asp:DropDownList>
			</td>
			<td></td>
		</tr>
		<tr>
			<td>
				<asp:Literal ID="Literal2" Text="<%$Resources : IbnFramework.TimeTracking, BlockCard%>" runat="server"></asp:Literal>:
			</td>
			<td>
				<asp:DropDownList runat="server" ID="ddlBlockCard" Width="100%"></asp:DropDownList>
			</td>
			<td></td>
		</tr>
		<tr>
			<td>
				<asp:Literal ID="Literal3" Text="<%$Resources : IbnFramework.TimeTracking, EntryCard%>" runat="server"></asp:Literal>:
			</td>
			<td>
				<asp:DropDownList runat="server" ID="ddlEntryCard" Width="100%"></asp:DropDownList>
			</td>
			<td></td>
		</tr>
		<tr>
			<td>
				<asp:Literal ID="Literal5" Text="<%$Resources : IbnFramework.TimeTracking, SuperType%>" runat="server"></asp:Literal>:
			</td>
			<td>
				<asp:DropDownList runat="server" ID="ddlSuperType" Width="100%"></asp:DropDownList>
			</td>
			<td></td>
		</tr>
	</table>
	<br />
	<div style="padding-top:10px; text-align:center;">
		<asp:Button runat="server" ID="btnSave" Text="<%$Resources : IbnFramework.Global, _mc_Save%>" OnClick="btnSave_Click" Width="80" />
		<asp:Button runat="server" ID="btnCancel" Text="<%$Resources : IbnFramework.Global, _mc_Cancel%>" OnClientClick="window.close();" Width="80" style="margin-left:15px;" CausesValidation="false" />
	</div>
</td></tr></table>