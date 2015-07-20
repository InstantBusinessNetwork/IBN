<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovalBlockEditControl.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BusinessProcess.ActivityControls.ApprovalBlockEditControl" %>
<table border="0" class="ibn-propertysheet pad5" width="100%">
	<tr>
		<td class="ibn-label" style="width:150px;">
			<asp:Literal runat="server" ID="BlockTypeLiteral" Text="<%$ Resources: IbnFramework.BusinessProcess, BlockActivityType %>"></asp:Literal>:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="BlockTypeList" Width="350">
				<asp:ListItem Text="<%$ Resources: IbnFramework.BusinessProcess, BlockActivityTypeAll %>" Value="0"></asp:ListItem>
				<asp:ListItem Text="<%$ Resources: IbnFramework.BusinessProcess, BlockActivityTypeAny %>" Value="1"></asp:ListItem>
			</asp:DropDownList>
		</td>
	</tr>
</table>