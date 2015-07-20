<%@ Control Language="c#" Inherits="Mediachase.UI.Web.UserReports.Modules.PortalQuickSnapshot" CodeBehind="PortalQuickSnapshot.ascx.cs" %>
<%@ Reference Control="~/UserReports/GlobalModules/ReportHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="up" Src="~/UserReports/GlobalModules/ReportHeader.ascx" %>
<%@ Reference Control="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/UserReports/GlobalModules/PickerControl.ascx" %>
<div id="filter" runat="server" style="border-bottom: #cccccc 1px solid" printable="0">

	<script type="text/javascript">
	//<![CDATA[
	function PrintReport()
	{
		ChangeFormAction("?action=print");
	}

	function ExportReport(sReportName)
	{
		ChangeFormAction("?action=export&filename=" + sReportName + ".xls");
	}
	
	function ChangeFormAction(sAction)
	{
		var sCurAction = window.document.forms[0].action;
		var pos = sCurAction.indexOf("?");
		if (pos > 0)
			sCurAction = sCurAction.substring(0, pos);
		window.document.forms[0].action = sCurAction + sAction;
		<%= Page.GetPostBackClientEvent(btnApplySelectPeriod, "")%>
	}
	function ChangeModify(obj)
	{
		objTbl = document.getElementById('<%=tableDate.ClientID %>');
		id=obj.value;
		if(id=="9")
		{
			objTbl.style.display = 'block';
		}
		else
		{
			objTbl.style.display = 'none';
		}
	}
	//]]>
	</script>

	<table cellpadding="5" cellspacing="0" border="0">
		<tr>
			<td width="80px" class="text">
				<b>
					<%=LocRM.GetString("tPeriod")%>:</b>&nbsp;
			</td>
			<td valign="middle">
				<select class="text" id="ddPeriod" style="width: 150px" onchange="ChangeModify(this);" runat="server">
				</select>
			</td>
			<td>
				<table id="tableDate" cellspacing="2" cellpadding="0" runat="server">
					<tr>
						<td class="text">
							&nbsp;<b><%=LocRM.GetString("tFrom")%>:</b>&nbsp;
						</td>
						<td>
							<mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
						</td>
						<td class="text" style="padding-left: 10px">
							<b>
								<%=LocRM.GetString("tTo")%>:</b>&nbsp;
						</td>
						<td>
							<mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<table>
		<tr class="ibn-descriptiontext">
			<td width="80px">
			</td>
			<td>
				<asp:Button ID="btnApplySelectPeriod" runat="server" CssClass="text" Text="Show" Width="80px"></asp:Button>
			</td>
			<td>
				<input type="button" class="text" value='<%=LocRM.GetString("tPrint")%>' style="width: 80px" onclick="javascript:window.print()" />
			</td>
			<td>
				<asp:Button ID="btnExport" runat="server" CssClass="text" Width="80px" OnClick="btnExport_Click"></asp:Button>
			</td>
		</tr>
	</table>
	<br/>
</div>
<div id="dHeader" style="display: none; margin-bottom: 20px" printable="1">
	<ibn:up ID="_header" runat="server"></ibn:up>
</div>
<asp:Panel ID="exportPanel" runat="server">
	<div id="exportHeader" runat="server" visible="false" printable="0">
		<table width="100%" border="0" cellpadding="0" cellspacing="0" class="text">
			<tr>
				<td align="center">
					<b>
						<%= ProductName %></b>
				</td>
			</tr>
			<tr>
				<td align="center">
					<b>
						<%=LocRM.GetString("tPrtQSnap")%></b>
				</td>
			</tr>
			<tr>
				<td align="middle">
					<asp:Label CssClass="text" Font-Bold="True" runat="server" ID="lblInterval"></asp:Label>
				</td>
			</tr>
		</table>
	</div>
	<table>
		<tr>
			<td>
			</td>
		</tr>
	</table>
	<table width="100%" border="0" cellspacing="15" cellpadding="0">
		<tr>
			<td width="30%" class="ibn-sectionheader">
				<%=LocRM.GetString("tTotalsTD")%>
			</td>
			<td width="20%">
			</td>
			<td width="50%" class="ibn-sectionheader">
				<%=LocRM.GetString("tPeriodUsage")%>
			</td>
		</tr>
		<tr>
			<td valign="top">
				<table class="text" width="300px" runat="server" id="tableIMTotal">
					<tr>
						<td class="text">
							<%=LocRM.GetString("tMesStat")%>
						</td>
					</tr>
					<tr>
						<td>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotMesSent")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalMessages" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotIMMes")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalIMMessages" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotConfMes")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalConfMessages" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotFilesTrans")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalFilesTransferred" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
				</table>
			</td>
			<td>
			</td>
			<td valign="top">
				<table width="300px" class="text" runat="server" id="tableIMPeriod">
					<tr>
						<td class="text">
							<%=LocRM.GetString("tMesStat")%>
						</td>
					</tr>
					<tr>
						<td>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tAuthUs")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblAuthenticatedUsers" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotIMMes")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblPerTotalIMMaessages" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotConfMes")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblPerTotalChatMessages" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotFilesTrans")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblPerTotalFilesTransferred" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td class="text">
				<%=LocRM.GetString("tGrAndUs")%>
			</td>
			<td>
			</td>
			<td class="text">
				<%=LocRM.GetString("tOther")%>
			</td>
		</tr>
		<tr>
			<td valign="top">
				<table width="300px" class="text">
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotGr")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalGroups" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotUs")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalUsers" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tNumActUs")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalActiveUsers" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tNumPendUs")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalPendingUsers" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tNumExtUs")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalExternalUsers" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tNumInActUs")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalInactiveUsers" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
				</table>
			</td>
			<td>
			</td>
			<td valign="top">
				<table width="300px" class="text">
					<tr id="trProjects" runat="server">
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tNewPrjs")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblNewProjectsCreated" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr id="trIssues" runat="server">
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tNewIss")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblNewIssuesCreated" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tNewCalEntr")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblNewCalendarEntries" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tNewToDo")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblNewToDo" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr id="trTasks" runat="server">
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tNewTask")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblNewTask" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr style="visibility: hidden">
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tNewFiles")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblNewFiles" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr style="visibility: hidden">
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tNewVerFiles")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblNewFileVersions" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td class="text">
				<%=LocRM.GetString("tOther")%>
			</td>
			<td>
			</td>
			<td>
				<div class="text" runat="server" id="divDGTitle">
					<%=LocRM.GetString("tTop10")%></div>
			</td>
		</tr>
		<tr>
			<td valign="top">
				<table width="300px" class="text">
					<tr id="trTotProjects" runat="server">
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotPrj")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalProjects" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr id="trTotIssues" runat="server">
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotIss")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalIssues" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotCalEntr")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalCalendarEntries" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotToDo")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalToDo" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr id="trTotTasks" runat="server">
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotTask")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalTasks" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr style="visibility: hidden">
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotFilesLib")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalLibraryFiles" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr style="visibility: hidden">
						<td width="200px" class="text">
							<strong>
								<%=LocRM.GetString("tTotFilesVerLib")%>:</strong>
						</td>
						<td>
							<asp:Label ID="lblTotalLibraryFileVersions" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
				</table>
			</td>
			<td>
			</td>
			<td valign="top">
				<asp:DataGrid ID="dgTop10" runat="server" EnableViewState="False" AutoGenerateColumns="False" BorderStyle="None" GridLines="Horizontal" BorderWidth="0px" CellPadding="3" Width="100%">
					<Columns>
						<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderText="Place" DataField="Place"></asp:BoundColumn>
						<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderText="Group" DataField="Group"></asp:BoundColumn>
						<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderText="Name" DataField="Name"></asp:BoundColumn>
						<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderText="Total Messages" DataField="TotalMessages"></asp:BoundColumn>
					</Columns>
				</asp:DataGrid>
			</td>
		</tr>
	</table>
</asp:Panel>
