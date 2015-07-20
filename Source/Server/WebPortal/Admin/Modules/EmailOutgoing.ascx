<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailOutgoing.ascx.cs" Inherits="Mediachase.UI.Web.Admin.Modules.EmailOutgoing" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td style="padding: 5px">
			<dg:DataGridExtended id="dgLog" runat="server" allowpaging="True" 
				pagesize="10" allowsorting="True" cellpadding="0" gridlines="None" 
				CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" LayoutFixed="True">
				<columns>
					<asp:templatecolumn sortexpression="Subject">
						<headerstyle CssClass="ibn-vh2" Width="280px"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Wrap=True Width="280px"></itemstyle>
						<itemtemplate>
							<%#DataBinder.Eval(Container.DataItem, "Subject")%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn sortexpression="MailFrom">
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
						<div style="overflow:hidden">
							<%# "<a href='mailto:" + DataBinder.Eval(Container.DataItem, "MailFrom").ToString() + "'>" + 
									DataBinder.Eval(Container.DataItem, "MailFrom").ToString() + "</a>"%>
						</div>			
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn sortexpression="RcptTo">
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
							<div style="overflow:hidden">
							<%# "<a href='mailto:" + DataBinder.Eval(Container.DataItem, "RcptTo").ToString() + "'>" + 
									DataBinder.Eval(Container.DataItem, "RcptTo").ToString() + "</a>"%>
							</div>		
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn sortexpression="Created">
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Wrap="True"></itemstyle>
						<itemtemplate>
							<%# ((DateTime)DataBinder.Eval(Container.DataItem, "Created")).ToString("g")%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn sortexpression="Generation">
						<headerstyle CssClass="ibn-vh2" Width="80px"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Wrap=True Width="80px"></itemstyle>
						<itemtemplate>
							<%#DataBinder.Eval(Container.DataItem, "Generation")%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:TemplateColumn sortexpression="ErrorMsg">
					<headerstyle CssClass="ibn-vh2" Wrap="true" Width="185px"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Wrap=True Width="185px"></itemstyle>
						<itemtemplate>
							<%#DataBinder.Eval(Container.DataItem, "ErrorMsg")%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn sortexpression="ErrorMsg">
						<headerstyle CssClass="ibn-vh2" Wrap="true" Width="50px"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Wrap=True Width="50px"></itemstyle>
						<itemtemplate>
							<asp:ImageButton ID="ibReset" runat="server" CommandName="Reset" ImageUrl="~/Layouts/images/reset.gif" CommandArgument='<%# Eval("PrimaryKeyId")%>' />				
							<asp:ImageButton ID="ibDelete" runat="server" CommandName="Delete" ImageUrl="~/Layouts/images/delete.gif" CommandArgument='<%# Eval("PrimaryKeyId")%>' />
						</itemtemplate>
					</asp:TemplateColumn>
				</columns>
			</dg:DataGridExtended>
		</td>
	</tr>
</table>