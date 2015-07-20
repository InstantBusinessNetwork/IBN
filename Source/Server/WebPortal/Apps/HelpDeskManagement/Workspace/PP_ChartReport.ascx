<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PP_ChartReport.ascx.cs" Inherits="Mediachase.UI.Web.Apps.HelpDeskManagement.Workspace.PP_ChartReport" %>


<table class="text" cellspacing="0" width="100%" border="0" style="padding-left: 10px;">
	<tr>
		<td style="width: 200px;"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.Incident, _mc_ChartReportPP %>" />:</td>
		<td><asp:DropDownList ID="ddChartView" Runat=server CssClass="text" Width="170px"></asp:DropDownList></td>
	</tr>
	<tr>
		<td style="width: 200px;"><%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.Incident:tSelectDiagram}")%>:</td>
		<td ><asp:RadioButtonList CellPadding="10" ID="rbChartType" CssClass="text" Runat=server RepeatDirection=Horizontal></asp:RadioButtonList></td>
	</tr>
</table>
