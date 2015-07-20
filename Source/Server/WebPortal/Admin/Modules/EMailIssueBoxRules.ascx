<%@ Reference Page="~/Admin/EMailIssueRuleEdit.aspx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailIssueBoxRules" Codebehind="EMailIssueBoxRules.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<script type="text/javascript">
	function OpenWindow(query,w,h,scroll)
		{
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			
			winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
			if (scroll) winprops+=',scrollbars=1';
			var f = window.open(query, "_blank", winprops);
		}
	function ChangeBox(obj)
	{
		window.location.href = '<%=ResolveClientUrl("~/Admin/EMailIssueBoxRules.aspx?IssBoxId=") %>' + obj.value;
	}
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td class="ibn-alternating ibn-navline">
			<table cellpadding="7" cellspacing="0" border="0" class="text">
				<tr>
					<td><b><%= LocRM.GetString("tName")%>:</b></td>
					<td><asp:DropDownList onchange="ChangeBox(this);"  ID="ddIssBox" Runat="server" Width="200px"></asp:DropDownList></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="padding: 5px">
			<asp:DataGrid Runat="server" ID="dgRules" AutoGenerateColumns="False" 
				AllowPaging="False" AllowSorting="False" cellpadding="5" 
				gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" 
				ShowHeader="True">
				<Columns>
					<asp:BoundColumn DataField="IncidentBoxRuleId" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="40px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="40px" />
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "OutlineIndex")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<%# GetLabel(DataBinder.Eval(Container.DataItem, "RuleType"),
											DataBinder.Eval(Container.DataItem, "Key"),
											DataBinder.Eval(Container.DataItem, "Value"),
											DataBinder.Eval(Container.DataItem, "OutlineIndex"),
											DataBinder.Eval(Container.DataItem, "OutlineLevel"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle HorizontalAlign=Right CssClass="ibn-vb2" Width="75"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh-right" Width="75" />
						<ItemTemplate>
							<asp:ImageButton Visible='<%#((int)DataBinder.Eval(Container.DataItem, "RuleType")==4 || (int)DataBinder.Eval(Container.DataItem, "RuleType")==5) %>' ImageAlign="AbsMiddle" ID="ibNew" Runat="server" BorderWidth="0" ImageUrl="~/layouts/images/newitem.gif" CausesValidation="False" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "IncidentBoxRuleId")%>'></asp:ImageButton>
							&nbsp;
							<a href='javascript:OpenWindow("EMailIssueRuleEdit.aspx?IssRuleId=<%#DataBinder.Eval(Container.DataItem, "IncidentBoxRuleId")%>&IssBoxId=<%#IssBoxId%>", 400, 300, false)' title='<%#LocRM.GetString("tEdit")%>'><img align='absmiddle' border='0' src='<%# ResolveUrl("~/layouts/images/edit.gif")%>' /></a>
							&nbsp;
							<asp:imagebutton ImageAlign="AbsMiddle" id="ibDelete" runat="server" borderwidth="0" imageurl="~/layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
							</asp:imagebutton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>