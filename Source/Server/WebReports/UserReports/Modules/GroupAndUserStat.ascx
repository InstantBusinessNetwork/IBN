<%@ Control Language="c#" Inherits="Mediachase.UI.Web.UserReports.Modules.GroupAndUserStat" CodeBehind="GroupAndUserStat.ascx.cs" %>
<%@ Reference Control="~/UserReports/GlobalModules/ReportHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="up" Src="~/UserReports/GlobalModules/ReportHeader.ascx" %>
<%@ Reference Control="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<div id="filter" runat="server" style="border-bottom: #cccccc 1px solid" printable="0">

	<script type="text/javascript">
	//<![CDATA[
	function ApplyFilter()
	{
		ChangeFormAction("");
	}
	function ChangeFormAction(sAction)
	{
		var sCurAction = window.document.forms[0].action;
		var pos = sCurAction.indexOf("?");
		if (pos > 0)
			sCurAction = sCurAction.substring(0, pos);
		window.document.forms[0].action = sCurAction + sAction;
		
		if (Page_ClientValidate())
			<%= Page.GetPostBackClientEvent(btnAplly, "")%>
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
				<select class="text" id="ddPeriod" style="width: 150px" onchange="ChangeModify(this);" runat="server" name="ddPeriod">
				</select>
			</td>
			<td>
				<table id="tableDate" cellspacing="2" cellpadding="0" runat="server">
					<tr>
						<td class="text">
							&nbsp;<b><%=LocRM.GetString("tDateFrom")%>:</b>&nbsp;
						</td>
						<td>
							<mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
						</td>
						<td class="text" style="padding-left: 10px">
							<b>
								<%=LocRM.GetString("tDateTo")%>:</b>&nbsp;
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
				<asp:Button ID="btnAplly" runat="server" CssClass="text" Text="Show" Width="80px" OnClick="btnAplly_Click"></asp:Button>
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
<div id="dHeaderPrint" style="display: none; margin-bottom: 20px" printable="1">
	<ibn:up ID="_header" runat="server"></ibn:up>
</div>
<table>
	<tr>
		<td>
		</td>
	</tr>
</table>
<table width="100%" cellspacing="0" cellpadding="0" border="0" class="text">
	<tr>
		<td class="ibn-navframe" style="padding-left: 5px; padding-top: 2px" valign="top" width="250px" printable="0">
			<div>
				<strong class="ibn-sectionheader" style="left: 5px; position: relative; top: 2px">
					<%=LocRM.GetString("tGrsAndUss")%>
				</strong>
			</div>
			<br/>
			<ComponentArt:TreeView ID="GUTree" Width="247px" AutoScroll="True" BackColor="#E1ECFC" BorderWidth="0px" DragAndDropEnabled="false" DefaultTarget="right" NodeEditingEnabled="false" CssClass="TreeView" NodeCssClass="TreeNode" SelectedNodeCssClass="SelectedTreeNode SelTrNode" HoverNodeCssClass="HoverTreeNode" NodeEditCssClass="NodeEdit" NodeLabelPadding="2" ShowLines="true" ClientScriptLocation="~/Scripts/componentart_webui_client/" EnableViewState="True" LineImagesFolderUrl="~/Layouts/Images/lines/" runat="server">
			</ComponentArt:TreeView>
		</td>
		<td valign="top">
			<asp:Panel ID="exportPanel" runat="server">
				<div id="dHeader" style="margin-bottom: 20px" runat="server" visible="false" printable="0">
					<table class="text" cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td align="middle">
								<b>
									<%= ProductName %></b>
							</td>
						</tr>
						<tr>
							<td align="middle" class="text">
								<b>
									<%=LocRM.GetString("tGrAndUs")%></b>
							</td>
						</tr>
						<tr>
							<td align="middle">
								<asp:Label ID="lblInterval" runat="server" CssClass="text" Font-Bold="True"></asp:Label>
							</td>
						</tr>
					</table>
				</div>
				<table class="text" id="tblGroupStat" cellspacing="0" cellpadding="5" width="100%" border="0" runat="server">
					<tr>
						<td colspan="5">
							<strong class="ibn-sectionheader">
								<%=LocRM.GetString("tGroupStatToDate")%></strong>
						</td>
					</tr>
					<tr>
						<td width="185px">
							&nbsp;
						</td>
						<td width="100px">
							&nbsp;
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="185px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr>
						<td width="185px" class="text">
							<strong>
								<%=LocRM.GetString("tGroupName")%>:</strong>
						</td>
						<td width="100px">
							<div align="left">
								<asp:Label EnableViewState="false" ID="lblGroupName" runat="server" CssClass="text"></asp:Label>
							</div>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="185px">
							<strong>
								<%=LocRM.GetString("tAccNum")%>:</strong>
						</td>
						<td>
							<div align="left">
								<asp:Label EnableViewState="false" ID="lblNumberAccounts" runat="server" CssClass="text"></asp:Label>
							</div>
						</td>
					</tr>
					<tr>
						<td width="185px" class="text">
							<strong>
								<%=LocRM.GetString("tActAccs")%>:</strong>
						</td>
						<td width="100px">
							<div align="left">
								<asp:Label EnableViewState="false" ID="lblActiveAccounts" runat="server" CssClass="text"></asp:Label>
							</div>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="185px" class="text">
							<strong>
								<%=LocRM.GetString("tDeActAccs")%>:</strong>
						</td>
						<td>
							<div align="left">
								<asp:Label EnableViewState="false" ID="lblDeActiveAccounts" runat="server" CssClass="text"></asp:Label>
							</div>
						</td>
					</tr>
					<tr>
						<td width="185px">
							&nbsp;
						</td>
						<td width="100px">
							<div align="left">
							</div>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="185px">
							&nbsp;
						</td>
						<td>
							<div align="left">
							</div>
						</td>
					</tr>
					<tr>
						<td colspan="5">
							<strong class="ibn-sectionheader">
								<%=LocRM.GetString("tBusinessObjects")%></strong>
						</td>
					</tr>
					<tr>
						<td width="185px">
							&nbsp;
						</td>
						<td width="100px">
							<div align="left">
							</div>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="185px">
							&nbsp;
						</td>
						<td>
							<div align="left">
							</div>
						</td>
					</tr>
					<tr id="trPM" runat="server">
						<td width="185px" class="text">
							<b>
								<%=LocRM.GetString("tNewPrjs")%>:</b>
						</td>
						<td width="100px">
							<asp:Label EnableViewState="false" ID="lblProjectsCreated" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="185px" class="text">
							<b>
								<%=LocRM.GetString("tNewTasks")%>:</b>
						</td>
						<td>
							<asp:Label EnableViewState="false" ID="lblTasksCreated" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="185px" class="text">
							<b>
								<%=LocRM.GetString("tNewToDo")%>:</b>
						</td>
						<td width="100px">
							<asp:Label EnableViewState="false" ID="lblToDoCreated" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="185px" class="text">
							<b>
								<%=LocRM.GetString("tCalEntrs")%>:</b>
						</td>
						<td>
							<asp:Label EnableViewState="false" ID="lblCalendarEntries" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr id="trHDM" runat="server">
						<td width="185px" class="text">
							<b>
								<%=LocRM.GetString("tNewIss")%>:</b>
						</td>
						<td width="100px">
							<asp:Label EnableViewState="false" ID="lblIssuesCreated" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="185px" class="text">
							<b><%=LocRM.GetString("tActiveReopenIss")%>:</b>
						</td>
						<td>
							<asp:Label EnableViewState="false" ID="lblIssuesReopen" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr id="trLibrary1" runat="server">
						<td width="185px" class="text">
							<b>
								<%=LocRM.GetString("tFilesPublish")%>:</b>
						</td>
						<td width="100px">
							<asp:Label EnableViewState="false" ID="lblFilesPublished" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="185px" class="text">
							<b>
								<%=LocRM.GetString("tNewVersions")%>:</b>
						</td>
						<td>
							<asp:Label EnableViewState="false" ID="lblVersionsFilesPublished" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
				</table>
				<table id="tblUser1" class="text" cellspacing="0" cellpadding="5" width="100%" border="0" runat="server">
					<tr>
						<td colspan="5">
							<strong class="ibn-sectionheader">
								<%=LocRM.GetString("tUsrInf")%></strong>
						</td>
					</tr>
					<tr>
						<td width="150px">
							&nbsp;
						</td>
						<td width="200px">
							<div align="left">
							</div>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr>
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tAccName")%>: </strong>
						</td>
						<td width="200px">
							<div align="left">
								<asp:Label EnableViewState="false" ID="lblUserAccountName" runat="server" CssClass="text"></asp:Label></div>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px" class="text">
						</td>
						<td>
						</td>
					</tr>
					<tr>
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tFirstName")%>:</strong>
						</td>
						<td width="200">
							<div align="left">
								<asp:Label EnableViewState="false" ID="lblUserFirstName" runat="server" CssClass="text"></asp:Label></div>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tLastName")%>:</strong>
						</td>
						<td>
							<asp:Label EnableViewState="false" ID="lblUserLastName" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tEMail")%>:</strong>
						</td>
						<td width="200px">
							<div align="left">
								<asp:Label EnableViewState="false" ID="lblUserEmail" runat="server" CssClass="text"></asp:Label></div>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tStatus")%>:</strong>
						</td>
						<td>
							<asp:Label EnableViewState="false" ID="lblUserStatus" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="150px" class="text" valign="top">
							<strong>
								<%=LocRM.GetString("tSecLevel")%>:</strong>
						</td>
						<td width="200px" valign="top">
							<div align="left">
								<asp:Label EnableViewState="false" ID="lblUserSecurityLevel" runat="server" CssClass="text"></asp:Label></div>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px" class="text" valign="top">
							<strong>
								<%=LocRM.GetString("tPortalLogins")%>:</strong>
						</td>
						<td valign="top">
							<asp:Label EnableViewState="false" ID="lblPortalLoginsCount" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr runat="server" id="trIMGroup">
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tIMGroupName")%>:</strong>
						</td>
						<td valign="top">
							<div align="left">
								<asp:Label EnableViewState="false" ID="lblUserGroupName" runat="server" CssClass="text"></asp:Label></div>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tClientLogins")%>:</strong>
						</td>
						<td>
							<asp:Label EnableViewState="false" ID="lblClientLoginsCount" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
				</table>
				<table id="tblUser2" class="text" cellspacing="0" cellpadding="5" width="100%" border="0" runat="server">
					<tr>
						<td colspan="6">
							<p>
								<strong></strong>&nbsp;</p>
							<p>
								<strong class="ibn-sectionheader">
									<asp:Label ID="lblIMStat" runat="server"></asp:Label></strong></p>
						</td>
					</tr>
					<tr>
						<td width="150px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td width="45px">
							&nbsp;
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr>
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tMessages")%></strong>
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotalSent")%>:
						</td>
						<td width="45px">
							<asp:Label EnableViewState="false" ID="lblUserMessagesSent" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr>
						<td width="150px">
							&nbsp;
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotalReceived")%>:
						</td>
						<td width="45px">
							<asp:Label EnableViewState="false" ID="lblUserMessagesReceived" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr>
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tConferences")%></strong>
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotalCreated")%>:
						</td>
						<td width="45px">
							<asp:Label EnableViewState="false" ID="lblUserConfCreated" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr>
						<td width="150px">
							&nbsp;
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotMesSent")%>:
						</td>
						<td width="45px">
							<asp:Label EnableViewState="false" ID="lblUserConfMessagesSent" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr>
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tFiles")%></strong>
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotFilesSent")%>:
						</td>
						<td width="45px">
							<asp:Label EnableViewState="false" ID="lblUserTotalFilesSent" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tBytes")%>:
						</td>
						<td>
							<asp:Label EnableViewState="false" ID="lblUserTotalFilesSentBytes" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td width="150px">
							&nbsp;
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotFilesReceived")%>:
						</td>
						<td width="45px">
							<asp:Label EnableViewState="false" ID="lblUserTotalFilesReceived" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tBytes")%>:
						</td>
						<td>
							<asp:Label EnableViewState="false" ID="lblUserTotalFilesReceivedBytes" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
				</table>
				<table id="tblUser3" class="text" cellspacing="0" cellpadding="5" width="100%" border="0" runat="server">
					<tr>
						<td colspan="6">
							<p>
								<strong></strong>&nbsp;</p>
							<p>
								<strong class="ibn-sectionheader">
									<%=LocRM.GetString("tBusinessObjects")%></strong></p>
						</td>
					</tr>
					<tr>
						<td width="150px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td width="45px">
							&nbsp;
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr id="trPrjs" runat="server">
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tProjects")%></strong>
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotPrjsCreate")%>:
						</td>
						<td width="45px">
							<asp:Label EnableViewState="false" ID="lblUserProjectTotalCreated" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr id="trIss" runat="server">
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tIssues")%></strong>
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotIsssCreate")%>:
						</td>
						<td width="45px">
							<asp:Label EnableViewState="false" ID="lblUserIssuesTotalCreated" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr>
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tCalendar")%></strong>
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotEntrsCreate")%>:
						</td>
						<td width="45px">
							<asp:Label EnableViewState="false" ID="lblUserCalendarTotalEntriesCreated" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr>
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tToDo")%></strong>
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotToDoCreate")%>:
						</td>
						<td width="45px">
							<asp:Label EnableViewState="false" ID="lblUserToDoTotalCreated" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr id="trTasks" runat="server">
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tTasks")%></strong>
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotTasksCreate")%>:
						</td>
						<td width="45px">
							<asp:Label EnableViewState="false" ID="lblUserTasksTotalCreated" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px">
							&nbsp;
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr runat="server" id="trLibrary">
						<td width="150px" class="text">
							<strong>
								<%=LocRM.GetString("tLibrary")%></strong>
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotFilesPublished")%>:
						</td>
						<td width="45px">
							<asp:Label EnableViewState="false" ID="lblUserTotalFilesPublished" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="30px">
							&nbsp;
						</td>
						<td width="150px" class="text">
							<%=LocRM.GetString("tTotVerFilesPublished")%>:
						</td>
						<td>
							<asp:Label EnableViewState="false" ID="lblUserTotalVerFilesPublished" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
				</table>
			</asp:Panel>
		</td>
	</tr>
</table>
<table style="border-bottom: #cccccc 1px solid" printable="0" width="100%">
	<tr>
		<td>
		</td>
	</tr>
</table>
