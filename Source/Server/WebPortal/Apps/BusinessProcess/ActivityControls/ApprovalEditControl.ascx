<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovalEditControl.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BusinessProcess.ActivityControls.ApprovalEditControl" %>
<table border="0" class="ibn-propertysheet pad5" width="100%">
	<tr>
		<td class="ibn-label" style="width:150px;">
			<asp:Literal runat="server" ID="SubjectLiteral" Text="<%$ Resources: IbnFramework.BusinessProcess, Subject %>"></asp:Literal>:
		</td>
		<td>
			<asp:TextBox runat="server" ID="SubjectText" Width="350"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" style="width:150px;">
			<asp:Literal runat="server" ID="UserLiteral" Text="<%$ Resources: IbnFramework.Global, _mc_User %>"></asp:Literal>:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="UserList" Width="350" DataTextField="DisplayName" DataValueField="PrincipalId"></asp:DropDownList>
		</td>
	</tr>
</table>