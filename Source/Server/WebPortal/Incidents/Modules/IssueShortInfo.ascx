<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.IssueShortInfo" Codebehind="IssueShortInfo.ascx.cs" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="padding:5px">
	<tr>
		<td>
			<table class="text" width="100%" border="0" cellpadding="5" cellspacing="0">
				<colgroup>
					<col width="80px" />
					<col width="40%" />
					<col width="80px" />
					<col />
					<col />
				</colgroup>
				<tr>
					<td class="ibn-label" width="80px">
						<%=LocRM.GetString("Title")%>:
					</td>
					<td class="ibn-value" colspan="3">
						<asp:label id="lblTitle" runat="server"></asp:label>
					</td>
					<td align="right">
						<asp:label id="lblState" runat="server" Font-Bold="True"></asp:label>
					</td>
				</tr>
				<tr id="trAdd" runat="server">
					<td class="ibn-label" width="80px"><div id="divType" runat="server"><%=LocRM.GetString("Type")%>:</div></td>
					<td class="ibn-value" width="40%"><asp:label id="lblType" runat="server"></asp:label></td>
					<td class="ibn-label" width="80px"><div id="divSeverity" runat="server"><%=LocRM.GetString("Severity")%>:</div></td>
					<td class="ibn-value"><asp:label id="lblSeverity" runat="server"></asp:label></td>
					<td align="right">
						<asp:Label ID="lblPriority" Runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td colspan="5" class="ibn-value"><asp:Label ID="lblDescription" Runat="server" Font-Italic="True"></asp:Label></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
