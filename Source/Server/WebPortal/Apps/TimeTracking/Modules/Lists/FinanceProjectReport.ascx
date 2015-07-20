<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FinanceProjectReport.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.Lists.FinanceProjectReport" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Reference Control="~/Modules/ReportHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="DTCC" src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="up" src="~/Modules/ReportHeader.ascx"%>
<script type="text/javascript">
function ExportScript()
{
	<%= Page.ClientScript.GetPostBackEventReference(btnExport, "") %>
}
</script>
<div runat="server" id="NoProjectsDiv" style="padding:20px;">
	<asp:Label runat="server" ID="NoProjectsLabel" CssClass="ibn-error" Text="<%$Resources: IbnFramework.TimeTracking, NoProjectsOrPermissions %>"></asp:Label>
</div>
<asp:UpdatePanel runat="server" ID="PanelFilters" ChildrenAsTriggers="true">
	<ContentTemplate>
		<div style="BORDER-BOTTOM: #cccccc 1px solid;" Printable="0">
			<table cellpadding="3" cellspacing="0" class="FilterTable" border="0">
				<tr>
					<td class="ibn-label">
						<asp:Literal runat="server" ID="GroupingLiteral" Text="<%$Resources: IbnFramework.TimeTracking, _mc_Group %>"></asp:Literal>:
					</td>
					<td>
						<asp:DropDownList runat="server" ID="GroupingList" AutoPostBack="true" Width="270px" OnSelectedIndexChanged="GroupingList_SelectedIndexChanged" />
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Literal runat="server" ID="ProjectLiteral" Text="<%$Resources: IbnFramework.Global, _mc_Project %>"></asp:Literal>:
					</td>
					<td>
						<asp:DropDownList runat="server" ID="ProjectList" AutoPostBack="true" Width="270px" OnSelectedIndexChanged="ProjectList_SelectedIndexChanged" />
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Literal runat="server" ID="UsersLiteral" Text="<%$Resources: IbnFramework.Global, _mc_User %>"></asp:Literal>:
					</td>
					<td>
						<asp:DropDownList runat="server" ID="UserList" AutoPostBack="true" Width="270px" OnSelectedIndexChanged="UserList_SelectedIndexChanged" />
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Literal runat="server" ID="PeriodLiteral" Text="<%$Resources: IbnFramework.Global, _mc_TimePeriod %>"></asp:Literal>:
					</td>
					<td>
						<table cellpadding="0" cellspacing="0">
							<tr>
								<td><asp:DropDownList ID="PeriodList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PeriodList_SelectedIndexChanged" /></td>
								<td><Ibn:DTCC ID="Dtc1" runat="server" AutoPostBack="true" SelectedMode="Week" ShowImageButton="false" DateCssClass="IbnCalendarText" ReadOnly="true" DateFormat="dd MMM yyyy" OnValueChange="Dtc1_ValueChange"/></td>
								<td><Ibn:DTCC ID="Dtc2" runat="server" AutoPostBack="true" SelectedMode="Week" ShowImageButton="false" DateCssClass="IbnCalendarText" ReadOnly="true" DateFormat="dd MMM yyyy" OnValueChange="Dtc2_ValueChange"/></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td class="ibn-label"><%= Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_ShowWeekNumbers}")%></td>
					<td><asp:CheckBox runat="server" ID="cbShowWeekNumber" AutoPostBack="true"  /></td>
				</tr>
				<tr>
					<td>&nbsp;</td>
					<td>
						<input type="button" class="text" id="PrintButton" runat="server" style="WIDTH: 80px" onclick="javascript:window.print()" />&nbsp;
						<input type="button" class="text" id="ExportButton" runat="server" style="WIDTH: 80px" onclick="javascript:ExportScript()" />
					</td>
				</tr>
			</table>
		</div>
		<div id="HeaderDiv" style="DISPLAY: none;" Printable="1">
			<ibn:up id="HeaderControl" runat="server"></ibn:up>
		</div>

		<asp:DataGrid runat="server" ID="MainGrid"
			AllowSorting="true"
			AllowPaging="False"
			AutoGenerateColumns="False"
			ShowFooter="False"
			EnableViewState="False"
			Width="100%"
			CssClass="SectionTable"
			CellPadding="3">
			<ItemStyle CssClass="SectionTableRow" />
			<AlternatingItemStyle CssClass="SectionTableRowAlt" />
			<HeaderStyle CssClass="SectionTableHeader" BackColor="#EDE9E0" />	
			<Columns>
				<asp:TemplateColumn>											
					<ItemTemplate>
						<%#  Eval("Title") %>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemStyle HorizontalAlign="Right" />
					<ItemTemplate>
						<%# Eval("Total")%>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemStyle HorizontalAlign="Right" />
					<ItemTemplate>
						<%# Eval("TotalApproved")%>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemStyle HorizontalAlign="Right" CssClass="TdTextClass" />
					<ItemTemplate>
						<%# Eval("Rate")%>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemStyle HorizontalAlign="Right" CssClass="TdTextClass" />
					<ItemTemplate>
						<%# Eval("Cost")%>
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>				
		</asp:DataGrid>
	</ContentTemplate>
</asp:UpdatePanel>
<asp:LinkButton ID="btnExport" runat="server" Visible="false" OnClick="btnExport_Click"></asp:LinkButton>
