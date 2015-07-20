<%@ Reference Control="~/Modules/TrialReqEdit.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.TrialReq" CodeBehind="TrialReq.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\Modules\BlockHeader.ascx" %>

<script type="text/javascript">
	//<![CDATA[
	function ExcelExport()
	{
		<%=Page.GetPostBackClientEvent(lbExport,"") %>
	}

function OpenWindow(query,w,h,scroll)
		{
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			
			winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
			if (scroll) winprops+=',scrollbars=1';
			var f = window.open(query, "_blank", winprops);
		}
	//]]>
</script>

<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:BlockHeader ID="secH" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<table class="ibn-navline ibn-alternating text" cellspacing="0" cellpadding="4" width="100%" border="0">
				<tr>
					<td style="width: 55px; padding-left: 15px" class="text">
						<b>Reseller:</b>
					</td>
					<td style="width: 160px">
						<asp:DropDownList ID="ddReseller" runat="server" CssClass="text" Width="150px">
						</asp:DropDownList>
					</td>
					<td class="text" style="width: 40px; padding-left: 15px">
						<b>Status:</b>&nbsp;
					</td>
					<td style="width: 160px">
						<asp:DropDownList ID="ddStatus" runat="server" CssClass="text" Width="190px">
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
			<asp:DataGrid AllowSorting="True" ID="SitesList" Style="margin-top: 10px; margin-left: 3px" runat="server" Width="100%" AutoGenerateColumns="False" BorderWidth="0px" GridLines="Horizontal" CellPadding="1">
				<HeaderStyle Wrap="False"></HeaderStyle>
				<Columns>
					<asp:HyperLinkColumn SortExpression="CompanyName" DataNavigateUrlField="RequestId" DataNavigateUrlFormatString="../Pages/TrialReqView.aspx?id={0}" DataTextField="CompanyName" HeaderText="Title">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle Width="10%" CssClass="ibn-vb2"></ItemStyle>
					</asp:HyperLinkColumn>
					<asp:BoundColumn SortExpression="Domain" DataField="domain" HeaderText="Domain">
						<HeaderStyle CssClass="ibn-vh2" Width="200px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="200px"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn SortExpression="FirstName" HeaderText="Contact Name">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# GetName(
								(string)DataBinder.Eval(Container.DataItem,"FirstName"),
								(string)DataBinder.Eval(Container.DataItem,"LastName")
							)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="Email" HeaderText="e-Mail">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<a href='mailto:<%#DataBinder.Eval(Container.DataItem,"Email")%>'>
								<%#DataBinder.Eval(Container.DataItem,"Email")%></a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn SortExpression="Phone" DataField="Phone" HeaderText="Phone">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn SortExpression="ResellerTitle" DataField="ResellerTitle" HeaderText="Reseller">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn HeaderText="Activity" SortExpression="IsActive">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%#
								(bool)DataBinder.Eval(Container.DataItem,"IsActive") ? "Active" : "Pending"
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d}">
						<HeaderStyle CssClass="ibn-vh2" Width="90px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="90px"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# GetEditPath(
								(int)DataBinder.Eval(Container.DataItem,"RequestId")
							)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# GetDeletePath(
								(int)DataBinder.Eval(Container.DataItem,"RequestId")
							)%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>

<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:BlockHeader ID="Blockheader1" Title="Failed Trial Requests" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<asp:DataGrid AllowSorting="True" ID="dgFailed" Style="margin-top: 10px; margin-left: 5px" runat="server" Width="100%" AutoGenerateColumns="False" BorderWidth="0px" GridLines="Horizontal" CellPadding="1">
				<HeaderStyle Wrap="False"></HeaderStyle>
				<Columns>
					<asp:BoundColumn SortExpression="CompanyName" DataField="CompanyName" HeaderText="Company Name">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn SortExpression="domain" DataField="domain" HeaderText="Domain">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn SortExpression="FirstName" HeaderText="Contact Name">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# GetName(
								(string)DataBinder.Eval(Container.DataItem,"FirstName"),
								(string)DataBinder.Eval(Container.DataItem,"LastName")
							)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn SortExpression="Email" DataField="Email" HeaderText="E-mail">
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
					<asp:BoundColumn SortExpression="CreationDate" DataField="CreationDate" HeaderText="Created">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn SortExpression="ErrorCode" HeaderText="Error Type">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%#
							GetError(
								(int)DataBinder.Eval(Container.DataItem,"ErrorCode")
							)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="30"></ItemStyle>
						<ItemTemplate>
							<asp:ImageButton ID="ibDelete" ImageUrl="~/Layouts/images/delete.gif" runat="server" BorderWidth="0" ToolTip="Delete Failed Request Record" CommandName="Delete"></asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:Panel ID="exportPanel" runat="server" Visible="False">
	<table cellpadding="2" cellspacing="1">
		<tr height="30px">
			<td class="boldtext">
				<font class="boldtext" size="3">Trial Requests List</font>
			</td>
		</tr>
		<tr>
			<td>
				<asp:DataGrid ID="dgExport" runat="server" Width="100%" AutoGenerateColumns="False" BorderWidth="1px" GridLines="Horizontal" CellPadding="1">
					<Columns>
						<asp:BoundColumn DataField="CompanyName" HeaderText="Company Name">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="domain" HeaderText="Domain">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						</asp:BoundColumn>
						<asp:TemplateColumn HeaderText="Contact Name">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
							<ItemTemplate>
								<%# GetName(
								(string)DataBinder.Eval(Container.DataItem,"FirstName"),
								(string)DataBinder.Eval(Container.DataItem,"LastName")
							)%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="Email" HeaderText="e-Mail">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="Phone" HeaderText="Phone">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="ResellerTitle" HeaderText="Reseller">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						</asp:BoundColumn>
						<asp:TemplateColumn HeaderText="Status">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
							<ItemTemplate>
								<%#
								(bool)DataBinder.Eval(Container.DataItem,"IsActive") ? "Active" : "Pending"
								%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="CreationDate" HeaderText="Created" DataFormatString="{0:d}">
							<HeaderStyle CssClass="ibn-vh2" Font-Bold="True"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						</asp:BoundColumn>
					</Columns>
				</asp:DataGrid>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:LinkButton ID="lbDelete" runat="server" Visible="False" OnClick="lbDelete_Click">lb</asp:LinkButton>
<asp:LinkButton ID="lbExport" runat="server" Visible="False" OnClick="lbExport_Click">lb</asp:LinkButton>
<input id="txtID" type="hidden" name="txtID" runat="server">

<script type="text/javascript">
	//<![CDATA[
	function DeleteRequest(id)
	{
		document.forms[0].<%= txtID.ClientID%>.value = id;
		if (confirm('<%=LocRM.GetString("Warning") %>' ))
			<%=Page.GetPostBackClientEvent(lbDelete,"") %>
	}
	//]]>
</script>
