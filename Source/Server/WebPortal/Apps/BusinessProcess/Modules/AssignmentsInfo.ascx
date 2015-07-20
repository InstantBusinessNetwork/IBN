<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignmentsInfo.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BusinessProcess.Modules.AssignmentsInfo" %>
<style type="text/css">
	.assignmentHeader
	{
		border: solid 1px #ADADAD; 
		background-color: #F8F8F8;
	}
	.assignmentBody
	{
		padding-left:3px;
		padding-top:3px;
		padding-bottom:10px;
		font-style:normal;
		min-height:1px;
	}
	.resultAccepted
	{
		font-weight:bold;
		color: Green;
	}
	.resultAccepted
	{
		font-weight:bold;
		color: Green;
	}
	.resultDeclined
	{
		font-weight:bold;
		color: Red;
	}
	.resultCanceled
	{
		font-weight:bold;
		color: Gray;
	}
</style>
<div style="padding:10px;">
	<asp:Repeater runat="server" ID="AssignmentList">
		<ItemTemplate>
			<table cellpadding="0" cellspacing="0" width="100%" border="0" style="table-layout:fixed">
				<tr>
					<td style='padding-left:<%# Eval("Indent") %>px' class="assignmentHeader">
						<table cellpadding="0" cellspacing="3" width="100%">
							<tr>
								<td style="font-weight:bold"><%# GetSubject((string)Eval("Subject")) %></td>
								<td style="width:200px;"><%# Eval("User") %></td>
								<td style="width:120px;"><%# Eval("FinishDate") %></td>
								<td style="width:80px;"><%# GetResult(Eval("Result"))%></td>
								<td style="width:170px;"><%# Eval("ClosedBy") %></td>
							</tr>
						</table>
				</tr>
				<tr>
					<td style='padding-left:<%# Eval("Indent") %>px'>
						<div class="assignmentBody"><%# Eval("Comment")%></div>
					</td>
				</tr>
			</table>
		</ItemTemplate>
	</asp:Repeater>
	<asp:Label runat="server" ID="NoAssignmentsLabel"></asp:Label>
</div>