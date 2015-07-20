<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailAntiSpamList" Codebehind="EMailAntiSpamList.ascx.cs" %>
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
			<asp:DataGrid id="dgRules" runat="server" allowpaging="False" 
				allowsorting="False" cellpadding="0" gridlines="None" 
				CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%">
				<columns>
					<asp:boundcolumn DataField="Id" visible="false"></asp:boundcolumn>
					<asp:boundcolumn HeaderStyle-Width="50px" DataField="Weight" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:boundcolumn>
					<asp:templatecolumn>
						<headerstyle CssClass="ibn-vh2" width="60px"></headerstyle>
						<itemstyle CssClass="ibn-vb2" width="60px"></itemstyle>
						<itemtemplate>
							<%# GetIcon((bool)DataBinder.Eval(Container.DataItem, "IsAccept"))%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
							<%# GetLink((int)DataBinder.Eval(Container.DataItem, "Id"), DataBinder.Eval(Container.DataItem, "Key").ToString())%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
							<%# DataBinder.Eval(Container.DataItem, "Type")%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
							<%# DataBinder.Eval(Container.DataItem, "Value")%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<headerstyle CssClass="ibn-vh-right" Width="52px"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Width="52px"></itemstyle>
						<itemtemplate>
							<%#GetEditButton((int)DataBinder.Eval(Container.DataItem, "Id"), LocRM.GetString("tEdit"))%>&nbsp;
							<asp:ImageButton ImageAlign="AbsMiddle" ID="ibDelete" title="Delete" Runat=server CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>' CommandName="Delete" ImageUrl="~/layouts/Images/Delete.gif"></asp:ImageButton>
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>