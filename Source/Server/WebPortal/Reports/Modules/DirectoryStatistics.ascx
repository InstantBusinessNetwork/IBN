<%@ Reference Control="~/Modules/ReportHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Reports.Modules.DirectoryStatistics" Codebehind="DirectoryStatistics.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PrintHeader" src="..\..\Modules\ReportHeader.ascx"%>
<div style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px; PADDING-BOTTOM: 5px; WIDTH: 100%; PADDING-TOP: 5px" align="right" Printable="0"><button id="btnPrint" value='' onclick="window.print();" type="button">
		<%#LocRM.GetString("Print")%>
	</button>&nbsp;
	<asp:button id=btnExport Text='<%#LocRM.GetString("Export")%>' runat="server" onclick="btnExport_Click">
	</asp:button>&nbsp;
</div>
<ibn:printheader id="Migrated_Printheader1" runat="server" ForPrintOnly="true" Filter=''></ibn:printheader><asp:panel id="exportPanel" Runat="server">
	<div id="dHeader" style="DISPLAY: none; MARGIN-BOTTOM: 20px" Printable="1" runat="server">
		<table class="text" cellspacing="0" cellpadding="0" width="100%" border="0">
			<tr>
				<td align="middle"><B><%=LocRM.GetString("IBN35")%></B></td>
			</tr>
			<tr>
				<td align="middle"><B><%=LocRM.GetString("UsersStatistics")%></B></td>
			</tr>
			<tr>
				<td align="middle">
					<asp:label id="lblStartedBy" runat="server" font-bold="True" cssclass="text"></asp:label></td>
			</tr>
			<tr>
				<td align="middle">
					<asp:label id="lblStartedAt" runat="server" font-bold="True" cssclass="text"></asp:label></td>
			</tr>
			<tr>
				<td align="middle"></td>
			</tr>
		</table>
	</div>
	<table cellspacing="0" cellpadding="7" width="100%" border="0">
		<tr>
			<td vAlign="top">
				<table class="ibn-propertysheet" id="Table1" cellspacing="0" cellpadding="3" width="100%" border="0">
					<tr>
						<td width="310"><B><%# LocRM.GetString("TotalUserCount")%>:</B></td>
						<td align="left">
							<asp:label id="lblTotalUserCount" runat="server"></asp:label></td>
					</tr>
					<tr>
						<td width="310"><B><%# LocRM.GetString("ActiveUserCount")%>:</B></td>
						<td align="left">
							<asp:label id="lblActiveUserCount" runat="server"></asp:label></td>
					</tr>
					<tr>
						<td width="310"><B><%# LocRM.GetString("InactiveUserCount")%>:</B></td>
						<td align="left">
							<asp:Label id="lblInactiveUserCount" runat="server"></asp:Label></td>
					</tr>
					<tr>
						<td width="310"><B><%# LocRM.GetString("ExternalUserCount")%>:</B></td>
						<td align="left">
							<asp:Label id="lblExternalUserCount" runat="server"></asp:Label></td>
					</tr>
					<tr>
						<td width="310"><B><%# LocRM.GetString("PendingUserCount")%>:</B></td>
						<td align="left">
							<asp:Label id="lblPendingUserCount" runat="server"></asp:Label></td>
					</tr>
				</table>
				<BR>
				<table class="ibn-propertysheet" id="Table2" cellspacing="0" cellpadding="3" width="100%" border="0">
					<tr>
						<td width="310"><B><%# LocRM.GetString("TotalSequreGroupCount")%>:</B></td>
						<td>
							<asp:Label id="lblTotalSequreGroupCount" runat="server"></asp:Label></td>
					</tr>
					<tr id="trTotalIM" runat="server">
						<td width="310"><B><%# LocRM.GetString("TotalImGroupCount")%>:</B></td>
						<td>
							<asp:Label id="lblTotalImGroupCount" runat="server"></asp:Label></td>
					</tr>
					<tr>
						<td width="310"><B><%# LocRM.GetString("AVCountUserPerGroup")%>:</B></td>
						<td>
							<asp:Label id="lblAVCountUserPerGroup" runat="server"></asp:Label></td>
					</tr>
					<tr id="trAvIM" runat="server">
						<td width="310"><B><%# LocRM.GetString("AVCountUserPerImGroup")%>:</B></td>
						<td>
							<asp:Label id="lblAVCountUserPerImGroup" runat="server"></asp:Label></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:panel><br>
<br>
