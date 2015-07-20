<%@ Reference Control="~/Modules/Separator1.ascx" %>
<%@ Reference Control="~/Modules/ReportHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Reports.Modules.ResourcesTracking" Codebehind="ResourcesTracking.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="..\..\Modules\Separator1.ascx"%>
<%@ Register TagPrefix="ibn" TagName="PrintHeader" src="..\..\Modules\ReportHeader.ascx"%>
<div runat="server" id="filter" style="BORDER-BOTTOM: #cccccc 1px solid" Printable="0">
	<table cellpadding="5" cellspacing="0" border="0" width="100%">
		<tr class="ibn-descriptiontext">
			<td align=left width="90px">
				<asp:Label ID="lblProjTitle" Runat=server CssClass="boldtext"></asp:Label>
			</td>
			<td>
				<asp:DropDownList ID="ddProject" Runat=server Width="250px" CssClass="text"></asp:DropDownList>
			</td>
			<td align=right>
				<asp:Button ID="btnApply" Runat=server CssClass="text" Width="80px" onclick="btnAplly_Click"></asp:Button>
				&nbsp;<input type="button" class="text" style="WIDTH: 80px" value='<%=LocRM.GetString("tPrint")%>' onclick=javascript:window.print()>
			</td>
		</tr>
	</table>
</div>
<ibn:printheader id="Migrated_Printheader1" runat="server" ForPrintOnly="true"></ibn:printheader>
<ibn:sep id="Sep0" runat="server" IsClickable="false">
</ibn:sep>
<table border=0 width="100%" cellpadding=2>
	<tr>
		<td class=text width="120px"><b><%=LocRM.GetString("tStartDate")%>:</b></td>
		<td align=left width="80px"><asp:Label ID="lblStartDate" Runat=server CssClass=text></asp:Label></td>
		<td width="15px"></td>
		<td class=text width="120px"><b><%=LocRM.GetString("tProjectStatus")%>:</b></td>
		<td width="150px" align=left><asp:Label ID="lblProjectStatus" Runat=server CssClass=text></asp:Label></td>
		<td></td>
	</tr>
	<tr>
		<td class=text width="120px"><b><%=LocRM.GetString("tTargetEndDate")%>:</b></td>
		<td align=left width="80px"><asp:Label ID="lblTargetEndDate" Runat=server CssClass=text></asp:Label></td>
		<td width="15px"></td>
		<td class=text width="120px"><b><%=LocRM.GetString("tCustomer")%>:</b></td>
		<td align=left width="150px"><asp:Label ID="lblCustomer" Runat=server CssClass=text></asp:Label></td>
		<td width="15px"></td>
		<td class=text width="120px"><b><%=LocRM.GetString("tProjectType")%>:</b></td>
		<td align=left width="200px"><asp:Label ID="lblProjectType" Runat=server CssClass=text></asp:Label></td>
		<td></td>
	</tr>
	<tr>
		<td class=text width="120px" valign=top><b><%=LocRM.GetString("tActualEndDate")%>:</b></td>
		<td align=left width="80px" valign=top><asp:Label ID="lblActualEndDate" Runat=server CssClass=text></asp:Label></td>
		<td width="15px"></td>
		<td class=text width="120px" valign=top><b><%=LocRM.GetString("tSponsors")%>:</b></td>
		<td align=left valign=top width="150px"><asp:Label ID="lblSponsors" Runat=server CssClass=text></asp:Label></td>
		<td width="15px"></td>
		<td class=text width="120px" valign=top><b><%=LocRM.GetString("tStakeholders")%>:</b></td>
		<td align=left valign=top width="200px"><asp:Label ID="lblStakeholders" Runat=server CssClass=text></asp:Label></td>
		<td></td>
	</tr>
	<tr height="25px">
		<td></td>
	</tr>
</table>
<ibn:sep id="Sep1" runat="server" IsClickable="false">
</ibn:sep>
<table cellpadding=2 cellspacing=2 border=0 width=100%>
	<tr>
		<td colspan=2 class=text width="16%"><b><%=LocRM.GetString("tActsCounts")%></b></td>
		<td width="12%"></td>
		<td colspan=3 class=text width="50%"><b><%=LocRM.GetString("tAddCounts")%></b></td>
	</tr>
	<tr>
		<td class=text width="10%"><%=LocRM.GetString("tTotalTasks")%>:</td>
		<td width="6%" align=right><b><asp:Label ID="lblTotalTasks" Runat=server CssClass=text></asp:Label></b></td>
		<td width="12%"></td>
		<td class=text width="10%"><%=LocRM.GetString("tTotalCalEntries")%>:</td>
		<td align=right width="6%"><b><asp:Label ID="lblTotalCalEntries" Runat=server CssClass=text></asp:Label></b></td>
		<td></td>
	</tr>
	<tr>
		<td class=text><%=LocRM.GetString("tTotalToDos")%>:</td>
		<td align=right><b><asp:Label ID="lblTotalToDos" Runat=server CssClass=text></asp:Label></b></td>
		<td width="12%"></td>
		<td class=text><%=LocRM.GetString("tTotalFiles")%>:</td>
		<td align=right><b><asp:Label ID="lblTotalFiles" Runat=server CssClass=text></asp:Label></b></td>
	</tr>
	<tr>
		<td class=text><%=LocRM.GetString("tTotalIssues")%>:</td>
		<td align=right><b><asp:Label ID="lblTotalIssues" Runat=server CssClass=text></asp:Label></b></td>
		<td width="12%"></td>
		<td class=text><%=LocRM.GetString("tTotalDiscussions")%>:</td>
		<td align=right><b><asp:Label ID="lblTotalDiscussions" Runat=server CssClass=text></asp:Label></b></td>
	</tr>
	<tr>
		<td class=text><%=LocRM.GetString("tTotalResources")%>:</td>
		<td align=right><b><asp:Label ID="lblTotalResources" Runat=server CssClass=text></asp:Label></b></td>
		<td width="12%"></td>
		<td class=text></td>
		<td align=right></td>
	</tr>
	<tr height="25px">
		<td></td>
	</tr>
</table>
<asp:Repeater ID="ResourcesRep" runat=server>
	<HeaderTemplate>
		<table border="0" cellspacing="0" cellpadding="5" width="100%">
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td width=100% style="border-bottom: #9ebff6 1px solid">
				<ibn:sep id="Sep2" runat="server" IsClickable="false" Title=<%# DataBinder.Eval(Container.DataItem,"ResourceInf")%> >
				</ibn:sep>
				<input type=hidden id="hdnUserID" runat=server value='<%# DataBinder.Eval(Container.DataItem,"ResourceID")%>' />
				<table width=100% cellpadding=2 cellspacing=2 border=0 class="ibn-propertysheet" bordercolor="#afc9ef" style="background-color: #F0F8FF">
					<tr>
						<td width="100px" class=text><%=LocRM.GetString("tIssCreated")%>:</td>
						<td width="50px" class=text><%# DataBinder.Eval(Container.DataItem,"IssuesCreated")%></td>
						<td width="40px"></td>
						<td width="100px" class=text><%=LocRM.GetString("tIssModified")%>:</td>
						<td width="50px" class=text><%# DataBinder.Eval(Container.DataItem,"IssuesModified")%></td>
						<td width="40px"></td>
						<td width="100px" class=text><%=LocRM.GetString("tIssClosed")%>:</td>
						<td width="50px" class=text><%# DataBinder.Eval(Container.DataItem,"IssuesClosed")%></td>
						<td></td>
					</tr>
					<tr>
						<td width="100px" class=text><%=LocRM.GetString("tFilesPub")%>:</td>
						<td width="50px" class=text></td>
						<td width="40px"></td>
						<td width="100px" class=text><%=LocRM.GetString("tCalEntries")%>:</td>
						<td width="50px" class=text><%# DataBinder.Eval(Container.DataItem,"CalendarEntries")%></td>
						<td width="40px"></td>
						<td width="100px" class=text><%=LocRM.GetString("tDissAdded")%>:</td>
						<td width="50px" class=text><%# DataBinder.Eval(Container.DataItem,"DiscussionsAdded")%></td>
						<td></td>
					</tr>
				</table>
				<div class=text id="divCurrentGrid" runat=server><br><b><%=LocRM.GetString("tCurPrjActs")%></b></div>
				<asp:datagrid runat="server" id="grdCurPrjActs" enableviewstate="False" autogeneratecolumns="False" borderstyle="None" gridlines="Horizontal" borderwidth="0px" cellpadding="3" width="100%">
					<columns>
						<asp:boundcolumn itemstyle-width="60px" headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="PriorityName"></asp:boundcolumn>
						<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" itemstyle-width="100">
							<itemtemplate>
								<%# GetType((int)DataBinder.Eval(Container.DataItem, "IsToDo"))%>
							</itemtemplate>
						</asp:templatecolumn>
						<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="Title"></asp:boundcolumn>
						<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="CreationDate" dataformatstring="{0:MMMM dd, yyyy}" itemstyle-width="160"></asp:boundcolumn>
						<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="FinishDate" dataformatstring="{0:MMMM dd, yyyy}" itemstyle-width="160"></asp:boundcolumn>
					</columns>
				</asp:datagrid>
				<div class=text id="divComplGrid" runat=server><br><b><%=LocRM.GetString("tComplActs")%></b></div>
				<asp:datagrid runat="server" id="grdComplActs" enableviewstate="False" autogeneratecolumns="False" borderstyle="None" gridlines="Horizontal" borderwidth="0px" cellpadding="3" width="100%">
					<columns>
						<asp:boundcolumn itemstyle-width="60px" headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="PriorityName"></asp:boundcolumn>
						<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" itemstyle-width="100">
							<itemtemplate>
								<%# GetType((int)DataBinder.Eval(Container.DataItem, "IsToDo"))%>
							</itemtemplate>
						</asp:templatecolumn>
						<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="Title"></asp:boundcolumn>
						<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="CreationDate" dataformatstring="{0:MMMM dd, yyyy}" itemstyle-width="160"></asp:boundcolumn>
						<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ActualFinishDate" dataformatstring="{0:MMMM dd, yyyy}" itemstyle-width="160"></asp:boundcolumn>
					</columns>
				</asp:datagrid>
				<br>
			</td>
		</tr>
	</ItemTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
</asp:Repeater>
<br>