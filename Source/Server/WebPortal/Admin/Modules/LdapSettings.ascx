<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.LdapSettings" Codebehind="LdapSettings.ascx.cs" %>
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
			<asp:DataGrid Runat="server" ID="dgSets" AutoGenerateColumns="False" 
				AllowPaging="False" AllowSorting="False" cellpadding="5" 
				gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" 
				ShowHeader="True">
				<Columns>
					<asp:BoundColumn DataField="LdapId" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"/>
						<ItemTemplate>
							<a href='LdapSettingsView.aspx?SetId=<%#DataBinder.Eval(Container.DataItem, "LdapId")%>'><%# DataBinder.Eval(Container.DataItem, "Title")%></a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"/>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Domain")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"/>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "IbnKey")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"/>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "LdapKey")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="150px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="150px"/>
						<ItemTemplate>
							<%# ((bool)DataBinder.Eval(Container.DataItem, "Autosync"))?
																"<img alt='checked' src='" + ResolveUrl("~/layouts/images/accept_1.gif") + "' />" :
																"<img alt='unchecked' src='" + ResolveUrl("~/layouts/images/deny_1.gif") + "' />"
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="160px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="160px"/>
						<ItemTemplate>
							<%# GetDate(DataBinder.Eval(Container.DataItem, "LastSynchronization"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle HorizontalAlign=Right CssClass="ibn-vb2" Width="26"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh-right" Width="26" />
						<ItemTemplate>
							<asp:imagebutton id="ibDelete" runat="server" imageurl="~/layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
							</asp:imagebutton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>