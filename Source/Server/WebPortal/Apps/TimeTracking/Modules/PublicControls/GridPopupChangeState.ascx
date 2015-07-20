<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GridPopupChangeState.ascx.cs" Inherits="Mediachase.UI.Web.Apps.TimeTracking.Modules.PublicControls.GridPopupChangeState" %>
<div>
	<table cellspacing="3" cellpadding="0" width="100%" border="0">
		<tr>
			<td valign="top" style="padding-left:10px; padding-top: 3px;" class="ibn-label">
				<asp:Literal runat="server" ID="Literal1" Text="<%$Resources: IbnFramework.TimeTracking, Project %>"></asp:Literal>:
			</td>
			<td style="width:310px; padding-top: 3px;">
				<asp:Literal runat="server" ID="ttbTitle"></asp:Literal>
			</td>
		</tr>
		<tr>
			<td valign="top" style="padding-left:10px; padding-top: 5px;" class="ibn-label">
				<asp:Literal runat="server" ID="Literal2" Text="<%$Resources: IbnFramework.TimeTracking, _mc_Comment %>"></asp:Literal>:
			</td>
			<td style="width:310px; padding-top: 5px;">
				<textarea runat="server" id="TTBlockComment" rows="4" cols="50" style="width: 300px;height:55px;"></textarea>
			</td>
		</tr>
		<tr>
			<td valign="top" style="padding-left:10px; padding-top: 5px;" class="ibn-label">
				<asp:Literal runat="server" ID="Literal3" Text="<%$Resources: IbnFramework.Global, Action %>"></asp:Literal>:
			</td>
			<td style="width:310px; padding-top: 5px;">
				<div style="height:80px; overflow-y: auto; border:solid 1px #eeeeee; width:305px;">
					<asp:RadioButtonList runat="server" ID="TransitionList" CellPadding="0" CellSpacing="0"></asp:RadioButtonList>
				</div>
			</td>
		</tr>
	</table>
	<table cellspacing="0" cellpadding="2" width="100%" border="0">
		<tr>
			<td align="center" style="padding-top:5px;">
				<asp:Button runat="server" ID="SubmitButton" CausesValidation="false" OnClick="TransitionButton_Click" Text="<%$Resources: IbnFramework.Common, Submit %>" Width="100"/>
				&nbsp;&nbsp;
				<asp:Button ID="CancelButton" runat="server" Text="<%$Resources : IbnFramework.TimeTracking, _mc_Close%>" Width="100" />	
			</td>
		</tr>
	</table>
</div>