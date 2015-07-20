<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GlobalRoleAclEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.Security.Modules.ManageControls.GlobalRoleAclEdit" %>
<%@ Reference Control="~/Apps/MetaUI/Primitives/Reference.Edit.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Reference" Src="~/Apps/MetaUI/Primitives/Reference.Edit.ascx" %>
<table style="width:100%;"><tr><td style="padding:10px;">
	<table cellpadding="3" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
		<tr>
			<td style='width:<%= labelColumnWidth%>'>
				<asp:Literal ID="Literal1" Text="<%$Resources : IbnFramework.Security, Principal%>" runat="server"></asp:Literal>:
			</td>
			<td>
				<asp:UpdatePanel runat="server" ID="updatePanel" RenderMode="Inline">
					<ContentTemplate>
						<ibn:Reference runat="server" AllowNulls="false" ID="principal" ShowLabel="false" />
					</ContentTemplate>
				</asp:UpdatePanel>
			</td>
		</tr>
		<tr>
			<td valign="top">
				<asp:Literal ID="Literal4" Text="<%$Resources : IbnFramework.Security, Rights%>" runat="server"></asp:Literal>:
			</td>
			<td style="padding-right:62px;">
				<div style="border:2px inset;overflow-y:auto;height:180px;">
					<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed" runat="server" id="tblRights">
					</table>
				</div>
			</td>
		</tr>
	</table>
	<div style="padding-left: <%= labelColumnWidth%>; padding-top:10px;">
	<asp:Button runat="server" ID="btnSave" Text="<%$Resources : IbnFramework.Global, _mc_Save%>" OnClick="btnSave_Click" Width="80" />
	<asp:Button runat="server" ID="btnCancel" Text="<%$Resources : IbnFramework.Global, _mc_Cancel%>" OnClientClick="window.close();" Width="80" style="margin-left:15px;" />
	</div>
</td></tr></table>