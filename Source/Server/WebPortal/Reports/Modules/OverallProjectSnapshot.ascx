<%@ Reference Control="~/Modules/ReportHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx"%>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Reports.Modules.OverallProjectSnapshot" Codebehind="OverallProjectSnapshot.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="..\..\Modules\Separator1.ascx"%>
<%@ Register TagPrefix="ibn" TagName="PrintHeader" src="..\..\Modules\ReportHeader.ascx"%>
<div runat="server" id="filter" style="BORDER-BOTTOM: #cccccc 1px solid;" Printable="0">
	<table cellpadding="5" cellspacing="0" border="0" width="100%">
		<tr class="ibn-descriptiontext">
			<td align="left" width="70">
				<asp:Label ID="lblProjTitle" Runat="server" CssClass="boldtext"></asp:Label>
			</td>
			<td>
				<asp:DropDownList ID="ddProject" Runat="server" Width="250px" CssClass="text"></asp:DropDownList>
			</td>
			<td align="right">
				<asp:Button ID="btnApply" Runat="server" CssClass="text" Width="80px" onclick="btnAplly_Click"></asp:Button>
				&nbsp;<input type="button" class="text" style="WIDTH: 80px" value='<%=LocRM.GetString("tPrint")%>' onclick="javascript:window.print()" />
			</td>
		</tr>
	</table>
</div>
<ibn:printheader id="Migrated_Printheader1" runat="server" ForPrintOnly="false"></ibn:printheader>
<ibn:sep id="Sep0" runat="server" IsClickable="false" title='<%# LocRM.GetString("tProjInf")%>'>
</ibn:sep>
<table border="0" width="100%" cellpadding="2">
	<tr>
		<td class="text" width="120"><b><%=LocRM.GetString("tStartDate")%>:</b></td>
		<td align="left" width="80"><asp:Label ID="lblStartDate" Runat="server" CssClass="text"></asp:Label></td>
		<td width="15"></td>
		<td class="text" width="120"><b><%=LocRM.GetString("tProjectStatus")%>:</b></td>
		<td width="150" align="left"><asp:Label ID="lblProjectStatus" Runat="server" CssClass="text"></asp:Label></td>
		<td></td>
	</tr>
	<tr>
		<td class="text" width="120"><b><%=LocRM.GetString("tTargetEndDate")%>:</b></td>
		<td align="left" width="80"><asp:Label ID="lblTargetEndDate" Runat="server" CssClass="text"></asp:Label></td>
		<td width="15"></td>
		<td class="text" width="120"><b><%=LocRM.GetString("tCustomer")%>:</b></td>
		<td align="left" width="150"><asp:Label ID="lblCustomer" Runat="server" CssClass="text"></asp:Label></td>
		<td width="15"></td>
		<td class="text" width="120"><b><%=LocRM.GetString("tProjectType")%>:</b></td>
		<td align="left" width="200"><asp:Label ID="lblProjectType" Runat="server" CssClass="text"></asp:Label></td>
		<td></td>
	</tr>
	<tr>
		<td class="text" width="120" valign="top"><b><%=LocRM.GetString("tActualEndDate")%>:</b></td>
		<td align="left" width="80" valign="top"><asp:Label ID="lblActualEndDate" Runat="server" CssClass="text"></asp:Label></td>
		<td width="15"></td>
		<td class="text" width="120" valign="top"><b><%=LocRM.GetString("tSponsors")%>:</b></td>
		<td align="left" valign="top" width="150"><asp:Label ID="lblSponsors" Runat="server" CssClass="text"></asp:Label></td>
		<td width="15"></td>
		<td class="text" width="120" valign="top"><b><%=LocRM.GetString("tStakeholders")%>:</b></td>
		<td align="left" valign="top" width="200"><asp:Label ID="lblStakeholders" Runat="server" CssClass="text"></asp:Label></td>
		<td></td>
	</tr>
	<tr height="25">
		<td></td>
	</tr>
</table>
<table width="100%" border="0" cellpadding="2" cellspacing="2">
	<tr>
		<!-- Images -->
		<td>
			<table width="100%" border="0" cellpadding="2" cellspacing="2" class="text">
				<tr>
					<td width="50%" valign="top">
						<ibn:sep id="Sep2" title='<%#LocRM.GetString("oIssuesGraph")%>' runat="server" IsClickable="false"/><br />
						<asp:Image id="imgIssuesGraph" Runat="server" BorderWidth="0" />
					</td>
					<td valign="top">
						<ibn:sep id="Sep3" title='<%#LocRM.GetString("oTaskGraph")%>' runat="server" IsClickable="false"/><br />
						<asp:Image id="imgTaskTodosGraph" Runat="server" BorderWidth="0" />
					</td>
				</tr>
			</table>
			<!-- END Images -->
		</td>
	</tr>
	<tr>
		<td>
			<!-- Project Statistics & Resources/Team Breakdown-->
			<table width="100%" border="0" cellpadding="2" cellspacing="2" class="text">
				<tr>
					<td width="50%" valign="top">
						<ibn:sep id="Sep1" title='<%#LocRM.GetString("oProjectStatistics")%>' runat="server" IsClickable="false"/>
						<b>
							<%#LocRM.GetString("oTaskTodoTrackingCounts")%>
						</b>
						<table width="100%" border="0" cellpadding="2" cellspacing="2" class="text">
							<tr>
								<td>
									<%#LocRM.GetString("oTotal")%>:
								</td>
								<td width="30">
									<asp:Label id="lblTaskTotal" runat="server" CssClass="boldtext"></asp:Label>
								</td>
							</tr>
							<tr>
								<td>&nbsp;&nbsp;&nbsp;<%#LocRM.GetString("oCompleted")%>:
								</td>
								<td>
									<asp:Label id="lblTaskCompleted" runat="server" CssClass="boldtext"></asp:Label>
								</td>
							</tr>
							<tr>
								<td>&nbsp;&nbsp;&nbsp;<%#LocRM.GetString("oActive")%>:
								</td>
								<td>
									<asp:Label id="lblTaskActive" runat="server" CssClass="boldtext"></asp:Label>
								</td>
							</tr>
							<tr>
								<td>&nbsp;&nbsp;&nbsp;<%#LocRM.GetString("oPastDue")%>:
								</td>
								<td>
									<asp:Label id="lblTaskPastdue" runat="server" CssClass="boldtext"></asp:Label>
								</td>
							</tr>
							<tr>
								<td height="30">
									<%#LocRM.GetString("oPercentsCompleted")%>:
								</td>
								<td>
									<asp:Label id="lblTaskPCompleted" runat="server" CssClass="boldtext"></asp:Label>
								</td>
							</tr>
						</table>
						<br />
						<b>
							<%#LocRM.GetString("oIssuesTrackingCount")%>
						</b>
						<table width="100%" border="0" cellpadding="2" cellspacing="2" class="text">
							<tr>
								<td>
									<%#LocRM.GetString("oTotal")%>:
								</td>
								<td width="30">
									<asp:Label id="lblIssuesTotal" runat="server" CssClass="boldtext"></asp:Label>
								</td>
							</tr>
							<tr>
								<td>&nbsp;&nbsp;&nbsp;<%#LocRM.GetString("oClosed")%>:
								</td>
								<td>
									<asp:Label id="lblIssuesClosed" runat="server" CssClass="boldtext"></asp:Label>
								</td>
							</tr>
							<tr>
								<td>&nbsp;&nbsp;&nbsp;<%#LocRM.GetString("oActive")%>:
								</td>
								<td>
									<asp:Label id="lblIssuesActive" runat="server" CssClass="boldtext"></asp:Label>
								</td>
							</tr>
							<tr>
								<td>&nbsp;&nbsp;&nbsp;<%#LocRM.GetString("oOpen")%>:
								</td>
								<td>
									<asp:Label id="lblIssuesOpen" runat="server" CssClass="boldtext"></asp:Label>
								</td>
							</tr>
							<tr>
								<td height="30">
									<%#LocRM.GetString("oPercentsClosed")%>:
								</td>
								<td>
									<asp:Label id="lblIssuesPClosed" runat="server" CssClass="boldtext"></asp:Label>
								</td>
							</tr>
						</table>
						<br />
						<b>
							<%#LocRM.GetString("oOther")%>
						</b>
						<table width="100%" border="0" cellpadding="2" cellspacing="2" class="text">
							<tr>
								<td>
									<%#LocRM.GetString("oDiscussions")%>:
								</td>
								<td width="30">
									<asp:Label id="lblOtherDiscussions" runat="server" CssClass="boldtext"></asp:Label>
								</td>
							</tr>
							<tr>
								<td>
									<%#LocRM.GetString("oFiles")%>:
								</td>
								<td>
									<asp:Label id="lblOtherFiles" runat="server" CssClass="boldtext"></asp:Label>
								</td>
							</tr>
						</table>
					</td>
					<td valign="top">
						<ibn:sep id="Sep4" title='<%#LocRM.GetString("oResourcesTeam")%>' runat="server" IsClickable="false"/>
						<asp:DataGrid Runat="server" ID="dgResources" AutoGenerateColumns="False" AllowPaging="False" 
							AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" 
							Width="100%" ShowHeader="True" EnableViewState="False" ShowFooter="False">
							<FooterStyle CssClass="text ibn-styleheader" Font-Bold="True"></FooterStyle>
							<Columns>
								<asp:TemplateColumn>
									<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemTemplate>
										<%# DataBinder.Eval(Container.DataItem, "LastName") + " " +
											DataBinder.Eval(Container.DataItem, "FirstName")%>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn itemstyle-width="30" HeaderStyle-Width="30">
									<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemTemplate>
										<%# DataBinder.Eval(Container.DataItem, "Code") %>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn itemstyle-width="30" HeaderStyle-Width="30">
									<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemTemplate>
										<%# ((decimal)DataBinder.Eval(Container.DataItem, "Rate")).ToString("f") %>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn itemstyle-width="90" HeaderStyle-Width="90">
									<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemTemplate>
										<%# Mediachase.UI.Web.Util.CommonHelper.GetHours((int)DataBinder.Eval(Container.DataItem, "Minutes")) %>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</asp:DataGrid>
					</td>
				</tr>
			</table>
			<br />
			<!-- END Project Statistics & Resources/Team Breakdown-->
			<!-- Past due activities --><br />
			<ibn:sep id="Sep6" title='<%# LocRM.GetString("oPastDueActivities")%>' runat="server" IsClickable="false"/>
			<asp:DataGrid Runat="server" ID="dgPastDue" AutoGenerateColumns="False" AllowPaging="False" 
				AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" 
				Width="100%" ShowHeader="True" EnableViewState="False" ShowFooter="True">
				<Columns>
					<asp:TemplateColumn ItemStyle-VerticalAlign="Top">
						<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Title") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn itemstyle-width="250" HeaderStyle-Width="250" ItemStyle-VerticalAlign="Top">
						<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Description") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn itemstyle-width="250" HeaderStyle-Width="250" ItemStyle-VerticalAlign="Top">
						<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%#
							GetResourcesList
							(
								(int)DataBinder.Eval(Container.DataItem, "ItemId"),
								(int)DataBinder.Eval(Container.DataItem, "IsToDo")
							)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:boundcolumn itemstyle-width="110" HeaderStyle-Width="110" DataField="FinishDate" DataFormatString="{0:d}" ItemStyle-VerticalAlign="Top" HeaderStyle-CssClass="ibn-vh2" />
				</Columns>
			</asp:DataGrid>
			<!-- END Past due activities -->
			<!-- Open Issues -->
			<ibn:sep id="Sep7" title='<%#LocRM.GetString("oOpenIssues")%>' runat="server" IsClickable="false"/>
			<asp:DataGrid Runat="server" ID="dgOpenIssues" AutoGenerateColumns="False" AllowPaging="False" 
				AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" 
				Width="100%" ShowHeader="True" EnableViewState="False" ShowFooter="True">
				<Columns>
					<asp:TemplateColumn ItemStyle-VerticalAlign="Top">
						<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Title") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn itemstyle-width="100" HeaderStyle-Width="100" ItemStyle-VerticalAlign="Top">
						<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "TypeName") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn itemstyle-width="250" HeaderStyle-Width="250" ItemStyle-VerticalAlign="Top">
						<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName((int)DataBinder.Eval(Container.DataItem, "ManagerId"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
			<!-- END Open Issues -->
		</td>
	</tr>
</table>
