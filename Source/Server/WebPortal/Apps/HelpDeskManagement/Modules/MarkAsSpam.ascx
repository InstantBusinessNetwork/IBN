<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MarkAsSpam.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules.MarkAsSpam" %>
<table cellpadding="5" cellspacing="0" width="100%">
	<tr>
		<td style="padding:10px">
			<%= GetGlobalResourceObject("IbnFramework.Incident", "MarkAsSpamText").ToString() %>
		</td>
	</tr>
	<tr>
		<td align="center">
			<asp:RadioButtonList ID="rbAction" runat="server" CellPadding="5" CellSpacing="0" RepeatDirection="Vertical"></asp:RadioButtonList>
		</td>
	</tr>
	<tr>
		<td align="center"><asp:Button ID="btnMark" runat="server" /></td>
	</tr>
</table>