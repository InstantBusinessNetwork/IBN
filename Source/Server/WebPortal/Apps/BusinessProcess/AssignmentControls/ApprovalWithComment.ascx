<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovalWithComment.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BusinessProcess.AssignmentControls.ApprovalWithComment" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<div style="text-align:center; padding-bottom:10px;">
	<table cellpadding="0" cellspacing="5" width="320px" border="0" style="text-align:left;">
		<tr>
			<td class="ibn-legend-greyblack" style="width:70px;"><asp:Literal runat="server" ID="SubjectLiteral" Text="<%$ Resources: IbnFramework.BusinessProcess, Subject %>"></asp:Literal>:</td>
			<td><asp:Label runat="server" ID="SubjectLabel"></asp:Label></td>
		</tr>
		<tr runat="server" id="DueDateRow">
			<td class="ibn-legend-greyblack"><asp:Literal runat="server" ID="DueDateLiteral" Text="<%$ Resources: IbnFramework.BusinessProcess, DueDate %>"></asp:Literal>:</td>
			<td><asp:Label runat="server" ID="DueDateLabel"></asp:Label></td>
		</tr>
	</table>

	<asp:TextBox runat="server" ID="CommentText" TextMode="MultiLine" Rows="4" Width="300px"></asp:TextBox>
	<br />
	<asp:RequiredFieldValidator runat="server" ID="CommentTextValidator" ControlToValidate="CommentText" ErrorMessage="<%$ Resources: IbnFramework.BusinessProcess, CommentRequired %>" EnableClientScript="true"></asp:RequiredFieldValidator>
	<br />
	<btn:imbutton class="text" id="ApproveButton" 
		Text="<%$ Resources: IbnFramework.BusinessProcess, Approve %>" Runat="server" 
		style="width:120px;" onserverclick="ApproveButton_ServerClick" CausesValidation="false"></btn:imbutton>
	&nbsp;
	<btn:imbutton class="text" id="DenyButton" 
		Text="<%$ Resources: IbnFramework.BusinessProcess, Deny %>" Runat="server" 
		IsDecline="true" style="width:120px;" onserverclick="DenyButton_ServerClick" CausesValidation="true"></btn:imbutton>
</div>