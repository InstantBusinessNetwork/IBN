<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReccurenceView.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Calendar.Modules.ReccurenceView" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls"%>
<style type="text/css">.pad5 {padding: 4px;}</style>
<div class="pad5">
<ibn:BlockHeaderLight2 HeaderCssClass="ibn-toolbar-light" ID="secHeader" runat="server" />
<table class="ibn-stylebox-light" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td class="pad5">
			<table class="text" id="ShowRecurrence" cellspacing="3" cellpadding="3" border="0" runat="server">
				<tr>
					<td class="ibn-label pad5" align="right"><%= LocRM.GetString("StartTime")%>:</td>
					<td class="ibn-value pad5"><asp:label id="txtStartTime" runat="server"></asp:label></td>
					<td class="ibn-label pad5" align="right"><%#LocRM.GetString("EndTime")%>:</td>
					<td class="ibn-value pad5"><asp:label id="txtEndTime" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td valign="top" class="ibn-label pad5" align="right"><%#LocRM.GetString("Pattern")%>:</td>
					<td colspan="3" class="ibn-value pad5">
						<asp:label id="txtInfo11" runat="server" Visible="False"><%#LocRM.GetString("Daily")%>. &nbsp;<%#LocRM.GetString("Every")%>&nbsp;<%#Frequency%>&nbsp;<%#LocRM.GetString("Day").ToLower()%></asp:label>
						<asp:label id="txtInfo12" runat="server" Visible="False"><%#LocRM.GetString("Daily")%>. &nbsp;<%#LocRM.GetString("Every")%>&nbsp;<%#Frequency%>&nbsp;<%#LocRM.GetString("weekday")%></asp:label>
						<asp:label id="txtInfo2" runat="server" Visible="False"><%#LocRM.GetString("Weekly")%>. <%#LocRM.GetString("Recurevery")%>&nbsp;<%#Frequency%>&nbsp;<%#LocRM.GetString("week(s)on")%>: <%#Weekdays%>.</asp:label>
						<asp:label id="txtInfo3" runat="server" Visible="False"><%#LocRM.GetString("Monthly")%>. <%#LocRM.GetString("Day")%> &nbsp;<%#MonthDay%>&nbsp;<%#LocRM.GetString("ofevery")%>&nbsp;<%#Frequency%>&nbsp;<%#LocRM.GetString("month(s)")%></asp:label>
						<asp:label id="txtInfo4" runat="server" Visible="False"><%#LocRM.GetString("Monthly")%>. <%#LocRM.GetString("The")%>&nbsp;<%#WeekNumber%>&nbsp;<%#Weekdays%>&nbsp;<%#LocRM.GetString("ofevery")%>&nbsp;<%#Frequency%>&nbsp;<%#LocRM.GetString("month(s)")%></asp:label>
						<asp:label id="txtInfo5" runat="server" Visible="False"><%#LocRM.GetString("Yearly")%>. <%#LocRM.GetString("Every")%>&nbsp;<%#MonthName%>&nbsp;<%#MonthDay%></asp:label>
						<asp:label id="txtInfo6" runat="server" Visible="False"><%#LocRM.GetString("Yearly")%>. <%#LocRM.GetString("The")%>&nbsp;<%#WeekNumber%>&nbsp;<%#Weekdays%>&nbsp;<%#LocRM.GetString("of")%>&nbsp;<%#MonthName%></asp:label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label pad5" align="right" nowrap><asp:Label Runat="server" ID="lblEnd"></asp:Label></td>
					<td colspan="3" class="ibn-value pad5"><asp:label id="txtEnd" runat="server"></asp:label></td>
				</tr>
			</table>
			<asp:Label id="lblNoRecurrence" runat="server" CssClass="text"></asp:Label>
			<asp:Button id="btnDelete" runat="server" style="DISPLAY:none" onclick="btnDelete_Click"></asp:Button>
		</td>
	</tr>
</table>
</div>