<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailIssueBoxes" Codebehind="EMailIssueBoxes.ascx.cs" %>
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
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td style="padding: 5px">
			<div id="divMessage" runat="server" style="padding:5px" class="text"><asp:Label ID="lblCantDelete" Runat="server" CssClass="text" ForeColor="Red"></asp:Label></div>
			<asp:DataGrid Runat="server" ID="dgBoxes" AutoGenerateColumns="False" 
				AllowPaging="False" AllowSorting="False" cellpadding="5" 
				gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" 
				ShowHeader="True">
				<Columns>
					<asp:BoundColumn DataField="IncidentBoxId" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="50px" ></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="50px" />
						<ItemTemplate>
						<div align="center">
							<asp:Image Runat="server" ID="imIsDefault" ImageUrl='<%#(bool)DataBinder.Eval(Container.DataItem, "IsDefault")?this.Page.ResolveUrl("~/layouts/images/accept_1.gif"):this.Page.ResolveUrl("~/layouts/images/deny_1.gif")%>'></asp:Image>
						</div>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<a href='EMailIssueBoxView.aspx?IssBoxId=<%#DataBinder.Eval(Container.DataItem, "IncidentBoxId")%>'>
								<%#DataBinder.Eval(Container.DataItem, "Name").ToString()%>
							</a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "IdentifierMask").ToString()%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="50"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh-right" Width="50" />
						<ItemTemplate>
							<%#GetRuleButton((int)DataBinder.Eval(Container.DataItem, "IncidentBoxId"))%>&nbsp;
							<asp:imagebutton ImageAlign="AbsMiddle" id="ibDelete" runat="server" borderwidth="0" imageurl="~/layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
							</asp:imagebutton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>