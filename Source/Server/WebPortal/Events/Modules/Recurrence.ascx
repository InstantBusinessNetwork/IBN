<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Events.Modules.Recurrence" Codebehind="Recurrence.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px">
	<tr>
		<td><ibn:blockheader id="Migrated_secHeader" title="Recurrence" runat="server"></ibn:blockheader></td>
	</tr>
</table>
<table class="ibn-stylebox-light" cellspacing="0" cellpadding="0" width="100%" border="0">	
	<tr>
		<td>
			<table class="text" id="ShowRecurrence" cellspacing="3" cellpadding="3" width="100%" border="0" runat="server">
				<tr>
					<td class="ibn-label" align="right"><%= LocRM.GetString("StartTime")%>:</td>
					<td class="ibn-value"><asp:label id="txtStartTime" runat="server"></asp:label></td>
					<td class="ibn-label" align="right"><%#LocRM.GetString("EndTime")%>:</td>
					<td class="ibn-value"><asp:label id="txtEndTime" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td valign="top" class="ibn-label" align="right"><%#LocRM.GetString("Pattern")%>:</td>
					<td colspan="3" class="ibn-value">
						<asp:label id="txtInfo11" runat="server" Visible="False"><%#LocRM.GetString("Daily")%>. &nbsp;<%#LocRM.GetString("Every")%>&nbsp;<%#Frequency%>&nbsp;<%#LocRM.GetString("Day").ToLower()%></asp:label>
						<asp:label id="txtInfo12" runat="server" Visible="False"><%#LocRM.GetString("Daily")%>. &nbsp;<%#LocRM.GetString("Every")%>&nbsp;<%#Frequency%>&nbsp;<%#LocRM.GetString("weekday")%></asp:label>
						<asp:label id="txtInfo21" runat="server" Visible="False"><%#LocRM.GetString("Weekly")%>. <%#LocRM.GetString("Recurevery")%>&nbsp;<%#Frequency%>&nbsp;<%#LocRM.GetString("week(s)on")%>: <%#Weekdays%>.</asp:label>
						<asp:label id="txtInfo31" runat="server" Visible="False"><%#LocRM.GetString("Monthly")%>. <%#LocRM.GetString("Day")%> &nbsp;<%#MonthDay%>&nbsp;<%#LocRM.GetString("ofevery")%>&nbsp;<%#Frequency%>&nbsp;<%#LocRM.GetString("month(s)")%></asp:label>
						<asp:label id="txtInfo32" runat="server" Visible="False"><%#LocRM.GetString("Monthly")%>. <%#LocRM.GetString("The")%>&nbsp;<%#WeekNumber%>&nbsp;<%#Weekdays%>&nbsp;<%#LocRM.GetString("ofevery")%>&nbsp;<%#Frequency%>&nbsp;<%#LocRM.GetString("month(s)")%></asp:label>
						<asp:label id="txtInfo41" runat="server" Visible="False"><%#LocRM.GetString("Yearly")%>. <%#LocRM.GetString("Every")%>&nbsp;<%#MonthName%>&nbsp;<%#MonthDay%></asp:label>
						<asp:label id="txtInfo42" runat="server" Visible="False"><%#LocRM.GetString("Yearly")%>. <%#LocRM.GetString("The")%>&nbsp;<%#WeekNumber%>&nbsp;<%#Weekdays%>&nbsp;<%#LocRM.GetString("of")%>&nbsp;<%#MonthName%></asp:label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label" align="right" nowrap><asp:Label Runat="server" ID="lblEnd"></asp:Label></td>
					<td colSpan="3" class="ibn-value"><asp:label id="txtEnd" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td class="ibn-label" align="right"><%#LocRM.GetString("TimeZone")%>:</td>
					<td colSpan="3" class="ibn-value"><asp:label id="txtTimeZone" runat="server"></asp:label></td>
				</tr>
			</table>
			<asp:Label id="lblNoRecurrence" runat="server" CssClass="text"></asp:Label>
			<asp:Button id="btnDelete" runat="server" style="DISPLAY:none" onclick="btnDelete_Click"></asp:Button></td>
	</tr>
</table>
