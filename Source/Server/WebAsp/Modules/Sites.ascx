<%@ Reference Control="~/Modules/SiteErrorLog.ascx" %>
<%@ Reference Control="~/Modules/SiteEdit.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="../Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.Sites" CodeBehind="Sites.ascx.cs" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox ibn-propertysheet" style="margin-top: 0px;">
	<tr>
		<td>
			<ibn:BlockHeader ID="secH" runat="server" Title='<%= LocRM.GetString("tbTitle")%>' />
		</td>
	</tr>
	<tr>
		<td>
			<table class="ibn-navline ibn-alternating text" cellspacing="0" cellpadding="4" width="100%" border="0">
				<tr>
					<td style="padding-left: 15px; width: 40px" class="text">
						<b>Type:</b>
					</td>
					<td style="width: 160px">
						<asp:DropDownList ID="ddType" runat="server" CssClass="text" Width="150px">
						</asp:DropDownList>
					</td>
					<td class="text" style="width: 40px; padding-left: 15px">
						<b>Activity:</b>&nbsp;
					</td>
					<td style="width:160px">
						<asp:DropDownList ID="ddActivity" runat="server" CssClass="text" Width="190px">
						</asp:DropDownList>
					</td>
					<td style="padding-left: 15px">
						<asp:Button ID="btnApply" runat="server" CssClass="text" Width="80px" Text="Apply" OnClick="btnApply_Click"></asp:Button>
					</td>
					<td>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<asp:DataGrid AllowSorting="true" Style="margin-top: 10px; margin-left: 5px; margin-right: 5px" ID="SitesList" Width="98%" AutoGenerateColumns="False" BorderWidth="0px" runat="server" GridLines="Horizontal" CellPadding="1" AllowPaging="true">
				<HeaderStyle Wrap="False"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn SortExpression="company_name" HeaderText="Company">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:HyperLink runat="server" ID="hl1" NavigateUrl='<%#"../Pages/SiteView.aspx?id="+Eval("company_uid").ToString()%>'>
								<%#DataBinder.Eval(Container.DataItem,"company_name")%>
							</asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="domain" SortExpression="domain" HeaderText="Domain">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn SortExpression="company_type" HeaderText="Type">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# GetCompanyType((byte)Eval("company_type")) %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="Is_Active" HeaderText="Activity">
						<HeaderStyle HorizontalAlign="Center" CssClass="ibn-vh2" Width="50px"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center" CssClass="ibn-vb2" Width="50px"></ItemStyle>
						<ItemTemplate>
							<%# GetActivity((bool)Eval("Is_Active")) %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="creation_date" SortExpression="creation_date" DataFormatString="{0:d}" HeaderText="Created">
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
					<asp:TemplateColumn SortExpression="Balance" HeaderText="Balance">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# ((decimal)Eval("Balance")).ToString("f") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="MonthlyCost" HeaderText="Monthly Cost">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# Eval("MonthlyCost") != DBNull.Value 
								? ((decimal)Eval("MonthlyCost")).ToString("f") 
								: ""
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" Width="25px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="25px"></ItemStyle>
						<ItemTemplate>
							<%# GetErrorLogPath(Eval("company_uid").ToString())%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" Width="25px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="25px"></ItemStyle>
						<ItemTemplate>
							<%# GetEditPath(Eval("company_uid").ToString()) %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" Width="25px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="25px"></ItemStyle>
						<ItemTemplate>
							<%# GetDeletePath(Eval("company_uid").ToString(), Eval("company_name").ToString(), Eval("domain").ToString())%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Right" CssClass="text" Mode="NumericPages"></PagerStyle>
			</asp:DataGrid>
			<div style="position: relative; top: -5px; left: 10px; width: 50%;">
				Page Size:
				<asp:DropDownList runat="server" ID="PageSizeList" AutoPostBack="true" OnSelectedIndexChanged="PageSizeList_SelectedIndexChanged">
					<asp:ListItem Text="10" Value="10"></asp:ListItem>
					<asp:ListItem Text="25" Value="25"></asp:ListItem>
					<asp:ListItem Text="50" Value="50"></asp:ListItem>
					<asp:ListItem Text="100" Value="100"></asp:ListItem>
				</asp:DropDownList>
			</div>
		</td>
	</tr>
</table>
<asp:Panel ID="exportPanel" runat="server" Visible="False">
	<table cellpadding="2" cellspacing="1">
		<tr height="30px">
			<td class="boldtext">
				<font class="boldtext" size="3">Companies/Sites List</font>
			</td>
		</tr>
		<tr>
			<td>
				<asp:DataGrid ID="dgExport" runat="server" Width="100%" AutoGenerateColumns="False" BorderWidth="1px" GridLines="Horizontal" CellPadding="1">
					<Columns>
						<asp:BoundColumn DataField="company_name" HeaderText="Company Name">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="domain" HeaderText="Domain">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						</asp:BoundColumn>
						<asp:TemplateColumn HeaderText="Type">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
							<ItemTemplate>
								<%# GetCompanyType((byte)Eval("company_type"))%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="Is_Active" HeaderText="Activity">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="creation_date" HeaderText="Creation Date" DataFormatString="{0:d}">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						</asp:BoundColumn>
						<asp:TemplateColumn HeaderText="Start Date">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
							<ItemTemplate>
								<%# GetDate(Eval("start_date"),(byte)Eval("company_type"))%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="End Date">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
							<ItemTemplate>
								<%# GetDate(Eval("end_date"), (byte)Eval("company_type"))%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Balance">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
							<ItemTemplate>
								<%# ((decimal)Eval("Balance")).ToString("f") %>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Monthly Cost">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
							<ItemTemplate>
								<%# Eval("MonthlyCost") != DBNull.Value 
								? ((decimal)Eval("MonthlyCost")).ToString("f") 
								: ""
								%>
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</asp:DataGrid>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:LinkButton ID="lbExport" runat="server" Visible="False" OnClick="lbExport_Click"></asp:LinkButton>
<asp:LinkButton ID="lbDelete" runat="server" Visible="False" OnClick="lbDelete_Click">lb</asp:LinkButton>
<input id="txtID" name="txtID" type="hidden" runat="server" />

<script type="text/javascript">
	//<![CDATA[
	function DeleteSite(id, tit, domain)
	{
		document.forms[0].<%= txtID.ClientID%>.value = id;
		if (confirm('<%=LocRM.GetString("WarningS")%> "' + tit + '"\r\n<%=LocRM.GetString("WarningS2")%> "' + domain + '" ?'))
			<%=Page.GetPostBackClientEvent(lbDelete,"") %>
	}
	//]]>
</script>
