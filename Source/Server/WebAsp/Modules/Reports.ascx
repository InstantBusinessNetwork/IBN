<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.Reports" Codebehind="Reports.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\Modules\BlockHeader.ascx" %>
<table id="Table1" width="100%" cellpadding="4" cellspacing="2" border="0">
	<tr>
		<td valign="top" style="width:50%">
			<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="padding-bottom:5px; margin-top:0px;">
				<tr>
					<td class="ibn-sectionheader">
						<ibn:BlockHeader id="Header2" runat="server"></ibn:BlockHeader>
					</td>
				</tr>
				<tr>
					<td style="width:100%;padding: 5px">
						<asp:datagrid AllowPaging="True" PageSize="25" runat="server" id="dgErrors" autogeneratecolumns="False" borderstyle="None" gridlines="Horizontal" borderwidth="0px" cellpadding="3" width="100%">
							<columns>
								<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headertext="Domain" datafield="Domain"></asp:boundcolumn>
								<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headertext="Error Code" datafield="Error" itemstyle-width="100" headerstyle-width="150"></asp:boundcolumn>
								<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headertext="Date/Time" datafield="CreationTime" itemstyle-width="150" headerstyle-width="150"></asp:boundcolumn>
							</columns>
							<PagerStyle HorizontalAlign="Right" CssClass="text" Mode="NumericPages"></PagerStyle>
						</asp:datagrid>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top" style="width: 50%;padding-right:0px">
			<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px;padding-bottom:5px">
				<tr>
					<td class="ibn-sectionheader" style="padding:0px;margin:0px">
						<ibn:BlockHeader id="Header3" runat="server"></ibn:BlockHeader>
					</td>
				</tr>
				<tr>
					<td style="width: 100%; padding: 5px; padding-top: 10px">
						<table width="100%" cellpadding="3px" cellspacing="0" class="text" style="border-collapse:collapse;border-color:#E4E4E4" border="1px">
							<tr>
								<td style="width:30%"></td>
								<td align="center">Active</td>
								<td align="center">Inactive</td>
								<td align="center">Total</td>
							</tr>
							<tr>
								<td>Billable Companies:</td>
								<td align="center"><asp:Label ID="ActiveBillableCountValue" Runat="server" CssClass="boldtext"></asp:Label></td>
								<td align="center"><asp:Label ID="InactiveBillableCountValue" Runat="server" CssClass="boldtext"></asp:Label></td>
								<td align="center"><asp:Label ID="BillableCountValue" Runat="server" CssClass="boldtext"></asp:Label></td>
							</tr>
							<tr>
								<td>Trial Companies:</td>
								<td align="center"><asp:Label ID="ActiveTrialCountValue" Runat="server" CssClass="boldtext"></asp:Label></td>
								<td align="center"><asp:Label ID="InactiveTrialCountValue" Runat="server" CssClass="boldtext"></asp:Label></td>
								<td align="center"><asp:Label ID="TrialCountValue" Runat="server" CssClass="boldtext"></asp:Label></td>
							</tr>
							<tr>
								<td>All Companies:</td>
								<td align="center"><asp:Label ID="ActiveCountValue" Runat="server" CssClass="boldtext"></asp:Label></td>
								<td align="center"><asp:Label ID="InactiveCountValue" Runat="server" CssClass="boldtext"></asp:Label></td>
								<td align="center"><asp:Label ID="CompaniesCountValue" Runat="server" CssClass="boldtext"></asp:Label></td>
							</tr>
						</table>
						<br />
						<div class="text">
							Total Pending Trials: <asp:Label ID="PendingTrialsCountValue" Runat="server" CssClass="boldtext"></asp:Label>
						</div>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
