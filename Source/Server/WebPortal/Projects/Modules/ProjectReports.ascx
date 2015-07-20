<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" EnableViewState="false" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectReports" Codebehind="ProjectReports.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="..\..\Modules\Separator1.ascx"%>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx"%>
<%@ Register TagPrefix="ibn" TagName="SnapHistory" src="..\..\Projects\Modules\ProjectHistory.ascx"%>
<style type="text/css">
.top { BORDER-RIGHT: #999999 thin solid; BORDER-TOP: #999999 1px solid; BORDER-LEFT: #999999 thin solid; BORDER-BOTTOM: #999999 thin }
.leftright { BORDER-RIGHT: #999999 thin solid; BORDER-TOP: #999999 1px; BORDER-LEFT: #999999 thin solid; BORDER-BOTTOM: #999999 thin }
.bottom { BORDER-RIGHT: #999999 thin solid; BORDER-TOP: #999999 1px; BORDER-LEFT: #999999 thin solid; BORDER-BOTTOM: #999999 thin solid }
.all { BORDER-RIGHT: #999999 thin solid; BORDER-TOP: #999999 1px solid; BORDER-LEFT: #999999 thin solid; BORDER-BOTTOM: #999999 thin solid }
</style>
<script type="text/javascript">
function OpenWindow(query,w,h,scroll)
	{
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		
		winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
		if (scroll) winprops+=',scrollbars=1';
		var f = window.open(query, "_blank", winprops);
	}
</script>
<table class="ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td class="ibn-propertysheet">
			<table id="Table1" width="100%" cellpadding="4" cellspacing="2" border="0">
				<tr>
					<td valign="top" width="45%" valign="top">
						<table id="tblIssues" runat="server" class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="MARGIN-TOP:0px;PADDING-BOTTOM:5px">
							<tr>
								<td class="ibn-sectionheader">
									<ibn:BlockHeader id="Header2" runat="server"></ibn:BlockHeader></td>
							</tr>
							<tr>
								<td width="100%" valign="middle" align="center">
									<asp:Image id="imgIssuesGraph" runat="server"></asp:Image>
								</td>
							</tr>
						</table>
					</td>
					<td valign="top" width="55%" style="PADDING-RIGHT:0px">
						<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="MARGIN-TOP:0px;PADDING-BOTTOM:5px">
							<tr>
								<td><ibn:BlockHeader id="Header3" runat="server"></ibn:BlockHeader></td>
							</tr>
							<tr>
								<td width="100%" valign="middle" align="center">
									<asp:Image id="imgTasksToDosGraph" runat="server" BorderWidth="0"></asp:Image>
								</td>
							</tr>
						</table>
						<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="PADDING-BOTTOM:5px">
							<tr>
								<td><ibn:BlockHeader id="Header4" runat="server"></ibn:BlockHeader></td>
							</tr>
							<tr>
								<td width="100%" style="padding:10px">
									<table border="0" cellpadding="3" cellspacing="0" class="text" width="100%">
										<asp:label id="lblTable" runat="server" CssClass="text"></asp:label>
										<tr id="trLegend" runat="server">
											<td>&nbsp;
											</td>
											<td align="left" width="40" class="text">0
											</td>
											<td align="center" class="text"><%=LocRM.GetString("ToDoTasksAssigned")%>
											</td>
											<td align="right" width="40"><asp:label id="lblMaxValue" runat="server" CssClass="text">1281</asp:label>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr id="trSnapshots" runat="server">
		<td class="ibn-propertysheet" style="padding:5px">
			<ibn:SnapHistory id="SH" runat="server"></ibn:SnapHistory>
		</td>
	</tr>
</table>
