<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailIssueBoxRulesView" Codebehind="EMailIssueBoxRulesView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td class="ibn-alternating ibn-navline">
			<table cellpadding="7" cellspacing="0" border="0" class="text">
				<tr>
					<td><b><%= LocRM.GetString("tName")%>:</b></td>
					<td><asp:Label ID="lblIssBoxName" Runat="server"></asp:Label></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<div style="padding:5px;overflow:auto;padding-bottom:10px;">
			<asp:Label ID="lblRules" Runat="server"></asp:Label>
			</div>
		</td>
	</tr>
</table>