<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.MailIncidentsList" Codebehind="MailIncidentsList.ascx.cs" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<dg:DataGridExtended id="dgIncidents" runat="server" allowpaging="True" pagesize="10" allowsorting="True" cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" enableviewstate="False">
	<COLUMNS>
		<ASP:BOUNDCOLUMN DataField="Pop3MailRequestId" Visible="False">
			<HEADERSTYLE CssClass="ibn-vh2"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2"></ITEMSTYLE>
		</ASP:BOUNDCOLUMN>
		
		<ASP:TEMPLATECOLUMN HeaderText="Subject" SortExpression="Subject">
			<HEADERSTYLE CssClass="ibn-vh2"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2"></ITEMSTYLE>
			<ITEMTEMPLATE>
				<A href='../Incidents/MailRequestView.aspx?RequestId=<%# DataBinder.Eval(Container.DataItem, "Pop3MailRequestId")%>'>
					<%# DataBinder.Eval(Container.DataItem, "Subject")%>
				</A>
			</ITEMTEMPLATE>
		</ASP:TEMPLATECOLUMN>
		
		<ASP:BOUNDCOLUMN DataField="Subject" Visible="False"></ASP:BOUNDCOLUMN>
		
		<ASP:TEMPLATECOLUMN HeaderText="Sender" SortExpression="Sender">
			<HEADERSTYLE CssClass="ibn-vh2" Width="190px"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2" Width="190px"></ITEMSTYLE>
			<ITEMTEMPLATE>
				<%# GetSender
					(
						(string)DataBinder.Eval(Container.DataItem, "Sender")
					)
				%>
			</ITEMTEMPLATE>
		</ASP:TEMPLATECOLUMN>
		
		<ASP:BOUNDCOLUMN DataField="Sender" Visible="False"></ASP:BOUNDCOLUMN>
				
		<ASP:TEMPLATECOLUMN HeaderText="Type" SortExpression="SenderType">
			<HEADERSTYLE CssClass="ibn-vh2" Width="100px"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2" Width="100px"></ITEMSTYLE>
			<ITEMTEMPLATE>
				<%# GetSenderType 
					(
						(int)DataBinder.Eval(Container.DataItem, "SenderType")
					)
				%>
			</ITEMTEMPLATE>
		</ASP:TEMPLATECOLUMN>
		<ASP:TEMPLATECOLUMN HeaderText="Priority" SortExpression="PriorityName">
			<HEADERSTYLE CssClass="ibn-vh2" Width="85px"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2" Width="85px"></ITEMSTYLE>
			<ITEMTEMPLATE>
				<%# DataBinder.Eval(Container.DataItem, "PriorityName")%>
			</ITEMTEMPLATE>
		</ASP:TEMPLATECOLUMN>
		<ASP:BOUNDCOLUMN DataField="Received" HeaderText="Creation Date" SortExpression="Received" DataFormatString="{0:d}">
			<HEADERSTYLE CssClass="ibn-vh2" Width="105px"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2" Width="105px"></ITEMSTYLE>
		</ASP:BOUNDCOLUMN>
		<ASP:TEMPLATECOLUMN>
			<HEADERSTYLE CssClass="ibn-vh-right" Width="60px" HorizontalAlign="Right"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2" Width="60px" HorizontalAlign="Right"></ITEMSTYLE>
			<ITEMTEMPLATE>
				<table cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td width="50%">
							<asp:HyperLink id="Hyperlink1" Runat="server" NavigateUrl='<%# "~/Incidents/MailRequestView.aspx?RequestId=" + DataBinder.Eval(Container.DataItem, "Pop3MailRequestId").ToString() %>' ImageUrl="../../layouts/images/icon-search.GIF">
							</asp:HyperLink></td>
						<td>
							<asp:imagebutton id="ibDelete" width="16" borderwidth="0" runat="server" causesvalidation="False" commandname="Delete" imageurl="../../layouts/images/DELETE.GIF"  height="16" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Pop3MailRequestId")%>'></asp:imagebutton></td>
					</tr>
				</table>
			</ITEMTEMPLATE>
		</ASP:TEMPLATECOLUMN>
	</COLUMNS>
</dg:DataGridExtended>
