<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RdlReport.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Shell.Modules.RdlReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<table cellpadding="0" cellspacing="0" width="100%">
	<tr runat="server" id="TitleRow">
		<td class="ibn-alternating ibn-navline" style="padding:3px;">
			<asp:Label runat="server" ID="ReportLabel"></asp:Label>
		</td>
	</tr>
	<tr runat="server" id="FilterRow">
		<td style="padding:5px;" class="ibn-alternating ibn-navline">
			<asp:PlaceHolder ID="phFilter" runat="server"></asp:PlaceHolder>
		</td>
	</tr>
	<tr>
		<td>
			<rsweb:ReportViewer ID="rvMain" runat="server" Width="100%" HyperlinkTarget="right"></rsweb:ReportViewer>
			<asp:Literal runat="server" ID="NoReportsLiteral" Text="<%$ Resources: IbnFramework.Report, NoReports %>"></asp:Literal>
			<asp:Label runat="server" ID="ErrorLabel" Text="<%$ Resources: IbnFramework.Report, ErrorOccurred %>" CssClass="ibn-error"></asp:Label>
		</td>
	</tr>
</table>