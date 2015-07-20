<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovalViewControl.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BusinessProcess.ActivityControls.ApprovalViewControl" %>
<table width="100%" border="0" class="ibn-propertysheet pad5">
	<tr>
		<td style="width:100px;">
			<asp:Literal runat="server" ID="SubjectLiteral" Text="<%$ Resources: IbnFramework.BusinessProcess, Subject %>"></asp:Literal>:
		</td>
		<td>
			<asp:Label runat="server" ID="SubjectLabel"></asp:Label>
		</td>
	</tr>
	<tr>
		<td style="width:100px;">
			<asp:Literal runat="server" ID="UserLiteral" Text="<%$ Resources: IbnFramework.Global, _mc_User %>"></asp:Literal>:
		</td>
		<td>
			<asp:Label runat="server" ID="UserLabel"></asp:Label>
		</td>
	</tr>
</table>