<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListTimeTrackingReport.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.Lists.ListTimeTrackingReport" %>
<%@ Register TagPrefix="Ibn" TagName="DTCC" src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="up" src="~/Modules/ReportHeader.ascx"%>
<%@ register TagPrefix="mc" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<link type="text/css" rel="stylesheet" href='<%= ResolveUrl("~/styles/ibnframework/calendar.css")%>' />

<script type="text/javascript">
    function ExportScript()
    {
		<%= Page.ClientScript.GetPostBackEventReference(btnExport, "") %>
    }
</script>  
<table cellpadding="0" cellspacing="0" style="width: 100%;">
	<tr>
		<td>
			<asp:UpdatePanel runat="server" ID="PanelFilters" ChildrenAsTriggers="true">
				<ContentTemplate>
					<div runat="server" id="divFiters" style="BORDER-BOTTOM: #cccccc 1px solid;" Printable="0">
						<table cellpadding="0" cellspacing="0" class="FilterTable" id="tblFilters">
							<tr>
								<td colspan="2" style="vertical-align: top;">
									<table cellpadding="2" cellspacing="0">
										<tr>
											<td><b><asp:Label ID="lblGroupText" runat="server"></asp:Label>:</b></td>
											<td><asp:DropDownList runat="server" ID="ddPrimary" AutoPostBack="true" Width="120px" /></td>
										</tr>
										<tr id="trSecondGroup" runat="server">
											<td><b><%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_SecondaryGroup}")%>: </b> </td>
											<td><asp:DropDownList runat="server" ID="ddSecondary" AutoPostBack="true" Width="120px" /></td>
										</tr>
									</table>
								
								</td>
								<td>
									<table cellpadding="2" cellspacing="0">
										<tr id="trUsers" runat="server">
											<td><b><%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.Global:_mc_User}")%>:</b> </td>
											<td><asp:DropDownList runat="server" ID="ddUsers" AutoPostBack="true" Width="270px" /> <br /></td>
										</tr>
										<tr>
											<td><b><%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.Global:_mc_Project}")%>:</b> </td>
											<td><mc:IndentedDropDownList ID="ddProjects" AutoPostBack="true" Width="270px" runat="server" /><br /></td>
										</tr>
										<tr>
											<td><b><%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_State}")%>:</b> </td>
											<td><asp:DropDownList runat="server" ID="ddState" AutoPostBack="true" Width="270px" /></td>
										</tr>
										<tr>
											<td><b><%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_IsRejected}")%>:</b> </td>
											<td> <asp:CheckBox runat="server" ID="tbRejected" AutoPostBack="true" /> </td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td><b><%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.Global:_mc_TimePeriod}")%>:</b>&nbsp;&nbsp;<asp:DropDownList ID="ddPeriod" runat="server" AutoPostBack="true" /></td>
								<td runat="server" id="tdDate1" style="visibility: hidden"><Ibn:DTCC ID="Dtc1" runat="server" AutoPostBack="true" SelectedMode="Week" ShowImageButton="false" DateCssClass="IbnCalendarText" ReadOnly="true" DateFormat="dd MMM yyyy"/></td>
								<td runat="server" id="tdDate2" style="visibility: hidden"><Ibn:DTCC ID="Dtc2" runat="server" AutoPostBack="true" SelectedMode="Week" ShowImageButton="false" DateCssClass="IbnCalendarText" ReadOnly="true" DateFormat="dd MMM yyyy"/></td>
							</tr>
							<tr>
								<td> <asp:CheckBox runat="server" ID="cbShowWeekNumber" AutoPostBack="true"  />								
								</td>
								<td colspan="2" align="right" style="padding:0px 20px 5px 0px;">
									<input type="button" class="text" id="btnPrint" runat="server" style="WIDTH: 80px" onclick="javascript:window.print()" />&nbsp;
									<input type="button" class="text" id="btnExport2" runat="server" style="WIDTH: 80px" onclick="javascript:ExportScript()" />
								</td>
							</tr>
						</table>
					</div>
					<div id="dHeader" style="DISPLAY: none;" Printable="1">
						<ibn:up id="_header" runat="server"></ibn:up>
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
								<ItemStyle HorizontalAlign="Right" CssClass="TdTextClass"/>
								<ItemTemplate>
									<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day1")) / 60, Convert.ToInt32(Eval("Day1")) % 60)%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<ItemStyle HorizontalAlign="Right" CssClass="TdTextClass"/>
								<ItemTemplate>
									<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day2")) / 60, Convert.ToInt32(Eval("Day2")) % 60)%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<ItemStyle HorizontalAlign="Right" CssClass="TdTextClass"/>
								<ItemTemplate>
									<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day3")) / 60, Convert.ToInt32(Eval("Day3")) % 60)%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<ItemStyle HorizontalAlign="Right" CssClass="TdTextClass"/>
								<ItemTemplate>
									<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day4")) / 60, Convert.ToInt32(Eval("Day4")) % 60)%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<ItemStyle HorizontalAlign="Right" CssClass="TdTextClass"/>
								<ItemTemplate>
									<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day5")) / 60, Convert.ToInt32(Eval("Day5")) % 60)%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<ItemStyle HorizontalAlign="Right" CssClass="TdTextClass"/>
								<ItemTemplate>
									<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day6")) / 60, Convert.ToInt32(Eval("Day6")) % 60)%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<ItemStyle HorizontalAlign="Right" CssClass="TdTextClass"/>
								<ItemTemplate>
									<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day7")) / 60, Convert.ToInt32(Eval("Day7")) % 60)%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<ItemStyle HorizontalAlign="Right" CssClass="TdTextClass" />
								<ItemTemplate>
									<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Convert.ToInt32(Eval("DayT")) / 60), Convert.ToInt32(Convert.ToInt32(Eval("DayT")) % 60))%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:BoundColumn DataField="StateFriendlyName"></asp:BoundColumn>
						</Columns>
					</asp:DataGrid>			    
				</ContentTemplate>
			</asp:UpdatePanel>
			<asp:LinkButton ID="btnExport" runat="server" Visible="false" OnClick="btnExport_Click"></asp:LinkButton>
		</td>
	</tr>
</table>



