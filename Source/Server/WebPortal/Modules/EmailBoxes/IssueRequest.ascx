<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EmailBoxes.IssueRequest" Codebehind="IssueRequest.ascx.cs" %>
<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
	<tr>
		<td style="PADDING-TOP:15px">
			<asp:CheckBox ID="cbAutoApprove" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
		</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox ID="cbUseExternal" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
		</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox ID="cbAutoDelete" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
		</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox ID="cbSaveMessageBodyAsEml" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
		</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox ID="cbSaveMessageBodyAsMsg" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
		</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox ID="cbSaveMessageBodyAsMht" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
		</td>
	</tr>
</table>