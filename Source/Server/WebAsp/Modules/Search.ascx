<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.Search" Codebehind="Search.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\Modules\BlockHeader.ascx" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox">
	<tr>
		<td>
			<ibn:blockheader id="secH" runat="server" title='Companies' />
		</td>
	</tr>
	<tr>
		<td>
			<asp:datagrid AllowSorting=False style="MARGIN-TOP: 10px; MARGIN-LEFT: 5px; MARGIN-RIGHT: 5px" id="dgCompanies" width="98%" autogeneratecolumns="False" borderwidth="0px" runat="server" GridLines="Horizontal" CellPadding="1" AllowPaging=false EnableViewState=False>
				<HeaderStyle Wrap="False"></HeaderStyle>
				<Columns>
					<asp:HyperLinkColumn DataNavigateUrlField="company_uid" DataNavigateUrlFormatString="../Pages/SiteView.aspx?id={0}" DataTextField="company_name" SortExpression="company_name" HeaderText="Company">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:HyperLinkColumn>
					<asp:BoundColumn DataField="domain" SortExpression="domain" HeaderText="Domain">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn SortExpression="company_type" HeaderText="Type">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# GetCompanyType((byte)Eval("company_type"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="Is_Active" HeaderText="Activity">
						<HeaderStyle HorizontalAlign="Center" CssClass="ibn-vh2" Width="50px"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center" CssClass="ibn-vb2" Width="50px"></ItemStyle>
						<ItemTemplate>
							<%# GetActivity((bool)Eval("Is_Active"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="creation_date" SortExpression="creation_date" DataFormatString="{0:d}">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn SortExpression="start_date" HeaderText="Start Date">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# GetDate(Eval("start_date"), (byte)Eval("company_type"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="end_date" HeaderText="End Date">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# GetDate(Eval("end_date"), (byte)Eval("company_type"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Right" CssClass="text" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
		</td>
	</tr>
</table>
<br>
<table class=ibn-stylebox cellspacing="0" cellpadding="0" width="100%" border=0>
	<tr>
		<td><ibn:blockheader id=secH1 runat="server" Title="Trial Requests"></ibn:blockheader></td>
	</tr>
	<tr>
		<td><asp:datagrid id=dgTrialRequests style="MARGIN-TOP: 10px; MARGIN-LEFT: 3px" runat="server" width="100%" autogeneratecolumns="False" borderwidth="0px" GridLines="Horizontal" CellPadding="1" EnableViewState=False>
				<HeaderStyle Wrap="False"></HeaderStyle>
				<Columns>
					<asp:HyperLinkColumn SortExpression="CompanyName" DataNavigateUrlField="RequestId" DataNavigateUrlFormatString="../Pages/TrialReqView.aspx?id={0}" DataTextField="CompanyName" HeaderText="Title">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle Width="10%" CssClass="ibn-vb2"></ItemStyle>
					</asp:HyperLinkColumn>
					<asp:BoundColumn SortExpression="Domain" DataField="domain" HeaderText="Domain">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn SortExpression="FirstName" HeaderText="Contact Name">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# GetName((string)Eval("FirstName"), (string)Eval("LastName"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn SortExpression="Email" DataField="Email" HeaderText="e-Mail">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn SortExpression="Phone" DataField="Phone" HeaderText="Phone">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn SortExpression="ResellerTitle" DataField="ResellerTitle" HeaderText="Reseller">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn HeaderText="Status" SortExpression="IsActive">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# (bool)Eval("IsActive") ? "Active" : "Pending" %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d}">
						<HeaderStyle CssClass="ibn-vh2" Width=90px></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width=90px></ItemStyle>
					</asp:BoundColumn>
				</Columns>
			</asp:datagrid></td>
	</tr>
</table>
<br>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" title="Resellers" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td><asp:datagrid id="dgResellers" ShowHeader="True" Width="100%" borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="3" AllowPaging="False" AutoGenerateColumns="False" Runat="server" EnableViewState=False>
				<Columns>
					<asp:BoundColumn Visible="False" HeaderText="ID" DataField="ResellerId" ReadOnly="True"></asp:BoundColumn>
					<asp:HyperLinkColumn DataNavigateUrlField="ResellerId" DataTextField="Title" DataNavigateUrlFormatString="../Pages/ResellerView.aspx?ResellerID={0}" HeaderText="Title" SortExpression="Title">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
					</asp:HyperLinkColumn>
					<asp:BoundColumn DataField="ContactName" ReadOnly="True" SortExpression="ContactName" HeaderText="Contact Name">
						<ItemStyle CssClass="ibn-vb2" Width="100"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
					</asp:BoundColumn>
					<asp:BoundColumn DataField="ContactEmail" ReadOnly="True" SortExpression="ContactEmail" HeaderText="Contact E-mail">
						<ItemStyle CssClass="ibn-vb2" Width="120"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
					</asp:BoundColumn>
					<asp:BoundColumn DataField="Guid" ReadOnly="True" SortExpression="Guid" HeaderText="Unique Id">
						<ItemStyle CssClass="ibn-vb2" Width="250"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
					</asp:BoundColumn>
					<asp:TemplateColumn HeaderText="Commission Percentage" SortExpression="CommissionPercentage">
						<HeaderStyle CssClass="ibn-vh2" Width="100px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="100px"></ItemStyle>
						<ItemTemplate>
							<%# GetPercentage((int)Eval("CommissionPercentage"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid></td>
	</tr>
</table>
