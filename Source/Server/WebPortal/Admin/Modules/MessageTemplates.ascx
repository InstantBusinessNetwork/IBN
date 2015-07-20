<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.MessageTemplates" Codebehind="MessageTemplates.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox2 text" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secH" runat="server" /></td>
	</tr>
	<tr>
		<td class="ibn-navline ibn-alternating text" style="PADDING: 3 14 3 14">
			<table width="100%" cellpadding=7 cellspacing=0 border="0">
				<tr class="text">
					<td width="100px">
						<b><%=LocRM.GetString("TemplateFor") %>:</b>
					</td>
					<td>
						<asp:DropDownList ID="ddTemplateFor" Runat=server AutoPostBack="True" onselectedindexchanged="ddT_PostBack"></asp:DropDownList>&nbsp;&nbsp;
						<b><%=LocRM.GetString("EventName") %></b>
						:&nbsp;
						<asp:DropDownList ID="ddMessages" Runat="server" Width="260px"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td class="text">
						<b><%=LocRM.GetString("Language") %></b>
					</td>
					<td>
						<asp:DropDownList ID="ddLanguage" Runat="server" Width="80px"></asp:DropDownList>&nbsp;&nbsp;
						<asp:Button CssClass="text" Runat="server" Text='<%#LocRM.GetString("Apply") %>' ID="btnApply" onclick="btnApply_Click">
						</asp:Button>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<table width="50%" cellpadding="7" cellspacing="0" border="0" class="text">
				<tr id="trSubject" runat="server">
					<td width="80" class="ibn-label"><%=LocRM.GetString("Subject") %>:</td>
					<td>
						<asp:Label ID="lblSubject" Runat="server" CssClass="text"></asp:Label>
					</td>
				</tr>
				<tr id="Tr1" runat="server">
					<td valign="top" class="ibn-label"><%=LocRM.GetString("Body") %>:</td>
					<td>
						<asp:Label ID="lblBody" Runat="server" CssClass="text"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
