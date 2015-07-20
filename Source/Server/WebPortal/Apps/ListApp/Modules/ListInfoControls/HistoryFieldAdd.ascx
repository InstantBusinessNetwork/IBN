<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HistoryFieldAdd.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.ListInfoControls.HistoryFieldAdd" %>
<table cellpadding="10" cellspacing="0" width="100%">
	<tr>
		<td class="ibn-label" style="width:120px;">
			<asp:Literal runat="server" ID="Literal1" Text="<%$Resources : IbnFramework.ListInfo, ChooseField %>"></asp:Literal>:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="FieldList" Width="100%"></asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td></td>
		<td>
			<asp:Button runat="server" ID="SaveButton" Text="<%$Resources : IbnFramework.Global, _mc_Save %>" OnClick="SaveButton_Click" />
			<asp:Button runat="server" ID="CancelButton" Text="<%$Resources : IbnFramework.Global, _mc_Cancel %>" />
		</td>
	</tr>
</table>
