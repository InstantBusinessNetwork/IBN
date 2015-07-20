<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.ReminderTemplates" Codebehind="ReminderTemplates.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox2 text" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secH" runat="server" /></td>
	</tr>
	<tr>
		<td class="ibn-navline ibn-alternating text" style="PADDING: 3 14 3 14">
			<table width="100%" cellpadding=7 cellspacing=0 border="0">
				<tr class="text">
					<td width="100px"><b><%=LocRM.GetString("TemplateFor") %>:</b></td>
					<td><asp:DropDownList ID="ddTemplateFor" Runat="server" AutoPostBack="True"></asp:DropDownList>&nbsp;&nbsp;</td>
					<td><b><%=LocRM.GetString("Language") %>:</b></td>
					<td><asp:DropDownList ID="ddLanguage" Runat="server" AutoPostBack="True" Width="80px" /></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<table width="75%" cellpadding="7" cellspacing="0" border="0" class="text">
				<tr id="trSubject" runat="server">
					<td width="80" class="ibn-label"><b><%=LocRM.GetString("Subject") %>:</b></td>
					<td>
						<asp:Label ID="lblSubject" Runat="server" CssClass="text"></asp:Label>
					</td>
				</tr>
				<tr id="Tr1" runat="server">
					<td valign="top" class="ibn-label"><b><%=LocRM.GetString("Body") %>:</b></td>
					<td>
						<asp:Label ID="lblBody" Runat="server" CssClass="text"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<asp:LinkButton ID="btnReset" CausesValidation=False Runat=server Visible=False />