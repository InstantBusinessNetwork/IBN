<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RdlReportPropertyPage.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Shell.Modules.RdlReportPropertyPage" %>
<table cellpadding="0" cellspacing="0" width="100%" style="padding:10px;">
	<tr runat="server" id="ReportsRow">
		<td style="width:100px;">
			<asp:Literal runat="server" ID="ReportLiteral" Text="<%$ Resources: IbnFramework.Report, RdlReportBlockName %>"></asp:Literal>:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="ReportList" Width="350px" DataTextField="Title" DataValueField="PrimaryKeyId"></asp:DropDownList>
		</td>
	</tr>
	<tr runat="server" id="NoReportsRow">
		<td colspan="2">
			<asp:Literal runat="server" ID="NoReportsLiteral" Text="<%$ Resources: IbnFramework.Report, NoReports %>"></asp:Literal>:
		</td>
	</tr>
</table>
