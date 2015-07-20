<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EmailLog" Codebehind="EmailLog.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<script language=javascript>
function ClearLog()
{
	if(confirm('<%=LocRM.GetString("tClearLog")%>'+'?'))
	{
		<%= Page.ClientScript.GetPostBackEventReference(lbClearLog, "")%>
	}
}
function ChangeLogState()
{
	<%= Page.ClientScript.GetPostBackEventReference(lbChangeLogState, "")%>
}
</script>

<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td style="padding: 5px">
			<dg:DataGridExtended id="dgLog" runat="server" allowpaging="True" 
				pagesize="10" allowsorting="True" cellpadding="0" gridlines="None" 
				CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" LayoutFixed="False">
				<columns>
					<asp:TemplateColumn sortexpression="Direction">
						<headerstyle CssClass="ibn-vh2" width="85px"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Wrap="false" width="85px"></itemstyle>
						<itemtemplate>
							<%#DataBinder.Eval(Container.DataItem, "Direction")%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:templatecolumn sortexpression="Subject">
						<headerstyle CssClass="ibn-vh2" Width="280px"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Wrap="True" Width="280px"></itemstyle>
						<itemtemplate>
							<%#DataBinder.Eval(Container.DataItem, "Subject")%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn sortexpression="From">
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
						<div style="overflow:hidden">
							<%# "<a href='mailto:" + DataBinder.Eval(Container.DataItem, "From").ToString() + "'>" + 
									DataBinder.Eval(Container.DataItem, "From").ToString() + "</a>"%>
						</div>			
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn sortexpression="To">
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
							<div style="overflow:hidden">
							<%# "<a href='mailto:" + DataBinder.Eval(Container.DataItem, "To").ToString() + "'>" + 
									DataBinder.Eval(Container.DataItem, "To").ToString() + "</a>"%>
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
					<asp:TemplateColumn sortexpression="EmailBoxId">
					<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Wrap="True"></itemstyle>
						<itemtemplate>
							<%#GetEmailBoxLink((int)DataBinder.Eval(Container.DataItem, "EmailBoxId"))%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn sortexpression="AntiSpamResult">
					<headerstyle CssClass="ibn-vh2" Wrap="true"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Wrap=True></itemstyle>
						<itemtemplate>
							<%#DataBinder.Eval(Container.DataItem, "AntiSpamResult")%>
						</itemtemplate>
					</asp:TemplateColumn>
				</columns>
			</dg:DataGridExtended>
		</td>
	</tr>
</table>
<asp:LinkButton ID="lbClearLog" Runat="server" Visible="False"></asp:LinkButton>
<asp:LinkButton runat="server" ID="lbChangeLogState" Visible="false"></asp:LinkButton>