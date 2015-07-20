<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ObjectRoleStateEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.Security.Modules.ManageControls.ObjectRoleStateEdit" %>
<table style="width:100%;"><tr><td style="padding:10px;">
	<table cellpadding="3" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
		<tr>
			<td style='width:<%= labelColumnWidth%>'>
				<asp:Literal ID="Literal1" Text="<%$Resources : IbnFramework.Security, RoleName%>" runat="server"></asp:Literal>:
			</td>
			<td>
				<asp:Label runat="server" ID="RoleNameLabel"></asp:Label>
			</td>
		</tr>
		<tr>
			<td valign="top">
				<asp:Literal ID="Literal5" Text="<%$Resources : IbnFramework.Security, Rights%>" runat="server"></asp:Literal>:
			</td>
			<td>
				<div style="border:2px inset;overflow-y:auto;height:120px;">
					<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed;" runat="server" id="tblRights">
					</table>
				</div>
			</td>
		</tr>
	</table>
	<div style='padding-left: <%= labelColumnWidth%>; padding-top:10px;'>
	<asp:Button runat="server" ID="btnSave" Text="<%$Resources : IbnFramework.Global, _mc_Save%>" OnClick="btnSave_Click" Width="80" />
	<asp:Button runat="server" ID="btnCancel" Text="<%$Resources : IbnFramework.Global, _mc_Cancel%>" OnClientClick="window.close();" Width="80" style="margin-left:15px;" />
	</div>
</td></tr></table>