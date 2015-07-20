<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkFlowInstanceView.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BusinessProcess.Modules.WorkFlowInstanceView" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<style type="text/css">
	.assignmentHeader
	{
		border: dotted 1px #ADADAD; 
		background-color: #F8F8F8;
	}
	.assignmentSubject
	{
		font-weight: bold;
	}
	.assignmentBody
	{
		padding-bottom:5px;
		color: #333333;
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
	.readonly
	{
		color: Gray;
	}
</style>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:1px">
	<tr>
		<td><ibn:blockheader id="MainHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td class="ibn-alternating ibn-navline">
			<table cellpadding="0" cellspacing="7" width="100%">
				<tr>
					<td class="ibn-label" style="width:120px;">
						<asp:Literal runat="server" ID="WorkFlowLiteral" Text="<%$Resources: IbnFramework.BusinessProcess, Workflow %>"></asp:Literal>:
					</td>
					<td class="ibn-value">
						<asp:Label runat="server" ID="WorkFlowLabel"></asp:Label>
					</td>
					<td class="ibn-label" style="width:100px;">
						<asp:Literal runat="server" ID="StateLiteral" Text="<%$Resources: IbnFramework.BusinessProcess, State %>"></asp:Literal>:
					</td>
					<td class="ibn-value">
						<asp:Label runat="server" ID="StateLabel"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Literal runat="server" ID="OwnerLiteral"></asp:Literal>:
					</td>
					<td class="ibn-value">
						<asp:HyperLink runat="server" ID="OwnerLink"></asp:HyperLink>
					</td>
					<td class="ibn-label">
						<asp:Literal runat="server" ID="CurrentDateLiteral" Text="<%$Resources: IbnFramework.BusinessProcess, State %>"></asp:Literal>:
					</td>
					<td class="ibn-value">
						<asp:Label runat="server" ID="CurrentDateLabel"></asp:Label>
					</td>
				</tr>
				<td class="ibn-label">
						<asp:Literal runat="server" ID="FilesLiteral" Text="<%$Resources: IbnFramework.BusinessProcess, Files %>"></asp:Literal>:
					</td>
					<td class="ibn-value" colspan="3">
						<asp:Label runat="server" ID="FilesLabel"></asp:Label>
					</td>
			</table>
		</td>
	</tr>
	<tr>
		<td style="padding:5px">
			<table cellpadding="4" cellspacing="0" width="100%" style="table-layout:fixed">
				<tr>
					<td><asp:Literal runat="server" ID="Literal1" Text="<%$Resources: IbnFramework.BusinessProcess, Subject %>"></asp:Literal></td>
					<td style="width:170px;"><asp:Literal runat="server" ID="Literal2" Text="<%$Resources: IbnFramework.BusinessProcess, Participant %>"></asp:Literal></td>
					<td style="width:110px;"><asp:Literal runat="server" ID="Literal3" Text="<%$Resources: IbnFramework.BusinessProcess, DueDate %>"></asp:Literal></td>
					<td style="width:80px;"><asp:Literal runat="server" ID="Literal4" Text="<%$Resources: IbnFramework.BusinessProcess, State %>"></asp:Literal></td>
					<td style="width:170px;"><asp:Literal runat="server" ID="Literal5" Text="<%$Resources: IbnFramework.BusinessProcess, ClosedBy %>"></asp:Literal></td>
					<td style="width:110px;"><asp:Literal runat="server" ID="Literal6" Text="<%$Resources: IbnFramework.BusinessProcess, FinishDate %>"></asp:Literal></td>
				</tr>
			</table>
			<asp:Repeater runat="server" ID="AssignmentList">
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" border="0">
						<tr>
							<td style='padding-left:<%# Eval("Indent") %>px'>
								<table cellpadding="4" cellspacing="0" width="100%" class="assignmentHeader" style="table-layout:fixed">
									<tr>
										<td><%# GetSubject((string)Eval("Subject"), (bool)Eval("ReadOnly"))%></td>
										<td style="width:170px;"><%# Eval("User") %></td>
										<td style="width:110px;"><%# Eval("PlanFinishDate") %></td>
										<td style="width:80px; font-weight:bold;"><%# GetResult(Eval("State"), Eval("Result"))%></td>
										<td style="width:170px;"><%# Eval("ClosedBy") %></td>
										<td style="width:110px;"><%# Eval("FinishDate") %></td>
									</tr>
								</table>
							</td>
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
		</td>
	</tr>
</table>