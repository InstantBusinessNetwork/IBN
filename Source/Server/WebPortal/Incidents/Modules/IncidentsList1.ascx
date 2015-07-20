<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.IncidentsList1" Codebehind="IncidentsList1.ascx.cs" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<dg:DataGridExtended id="dgIncidents" runat="server" allowpaging="True" pagesize="10" allowsorting="True" cellpadding="0" gridlines="None" CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" enableviewstate="true">
	<COLUMNS>
		<ASP:BOUNDCOLUMN DataField="IncidentId" Visible="False"></ASP:BOUNDCOLUMN>
		<asp:TemplateColumn>
			<HEADERSTYLE CssClass="ibn-vh2" Width="18"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2" Width="18"></ITEMSTYLE>
			<ItemTemplate>
				<%# GetTaskToDoStatus (
					(int)DataBinder.Eval(Container.DataItem, "PriorityId"),
					(string)DataBinder.Eval(Container.DataItem, "PriorityName")
					)
					%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<ASP:TEMPLATECOLUMN sortexpression="Title">
			<HEADERSTYLE CssClass="ibn-vh2"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2"></ITEMSTYLE>
			<ITEMTEMPLATE>
				<A href='../Incidents/IncidentView.aspx?IncidentId=<%# DataBinder.Eval(Container.DataItem, "IncidentId")%>'>
					<%# DataBinder.Eval(Container.DataItem, "Title")%>
				</A>
			</ITEMTEMPLATE>
		</ASP:TEMPLATECOLUMN>
		<ASP:TEMPLATECOLUMN>
			<HEADERSTYLE CssClass="ibn-vh2" Width="190px"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2" Width="190px"></ITEMSTYLE>
			<ITEMTEMPLATE>
				<%# GetCreator((int)DataBinder.Eval(Container.DataItem, "CreatorId"))%>
			</ITEMTEMPLATE>
		</ASP:TEMPLATECOLUMN>
		<ASP:TEMPLATECOLUMN sortexpression="StateName">
			<HEADERSTYLE CssClass="ibn-vh2" Width="100px"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2" Width="100px"></ITEMSTYLE>
			<ITEMTEMPLATE>
				<%# DataBinder.Eval(Container.DataItem, "StateName")%>
			</ITEMTEMPLATE>
		</ASP:TEMPLATECOLUMN>
		<ASP:TEMPLATECOLUMN sortexpression="PriorityName">
			<HEADERSTYLE CssClass="ibn-vh2" Width="80px"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2" Width="80px"></ITEMSTYLE>
			<ITEMTEMPLATE>
				<%# DataBinder.Eval(Container.DataItem, "PriorityName")%>
			</ITEMTEMPLATE>
		</ASP:TEMPLATECOLUMN>
		<ASP:BOUNDCOLUMN DataField="CreationDate" DataFormatString="{0:d}" sortexpression="CreationDate">
			<HEADERSTYLE CssClass="ibn-vh2" Width="90px"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2" Width="90px"></ITEMSTYLE>
		</ASP:BOUNDCOLUMN>
		<ASP:TEMPLATECOLUMN>
			<HEADERSTYLE CssClass="ibn-vh-right" Width="60px" HorizontalAlign="Right"></HEADERSTYLE>
			<ITEMSTYLE CssClass="ibn-vb2" Width="60px" HorizontalAlign="Right"></ITEMSTYLE>
			<ITEMTEMPLATE>
				<table cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td width="50%">
							<asp:HyperLink id="Hyperlink1" Runat="server" NavigateUrl='<%# "~/Incidents/IncidentEdit.aspx?IncidentID=" + DataBinder.Eval(Container.DataItem, "IncidentId").ToString() %>' ImageUrl="../../layouts/images/edit.GIF" ToolTip='<%#LocRM.GetString("tEdit")%>'>
							</asp:HyperLink></td>
						<td>
							<asp:imagebutton id="ibDelete" width="16" borderwidth="0" runat="server" causesvalidation="False" commandname="Delete" imageurl="../../layouts/images/DELETE.GIF"  height="16" title='<%#LocRM.GetString("tDelete")%>'></asp:imagebutton>
						</td>
					</tr>
				</table>
			</ITEMTEMPLATE>
		</ASP:TEMPLATECOLUMN>
	</COLUMNS>
</dg:DataGridExtended>