<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserReportPersonal.ascx.cs" Inherits="Mediachase.UI.Web.Apps.ReportManagement.Modules.UserReportPersonal" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<table cellpadding="5" cellspacing="0" width="100%">
	<tr>
		<td valign="top" width="50%">
			<ibn:blockheader id="tbLightPers" runat="server"></ibn:blockheader>
			<table cellpadding="3" cellspacing="3" border="0" width="100%" class="text ibn-stylebox-light">
				<tr>
					<td>
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr id="trPersReps" runat="server">
								<td style="padding-left: 6px" valign="top">
									<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/listset.gif") %>' />
								</td>
								<td width="100%" style="padding: 0px 0px 10px 5px;">
									<span class="text"><b>
										<%=LocRM.GetString("tSystemReports")%></b></span>
									<asp:Repeater ID="repPerc" runat="server">
										<HeaderTemplate>
											<table cellpadding="5" cellspacing="0" border="0" width="100%" class="text">
										</HeaderTemplate>
										<ItemTemplate>
											<tr>
												<td>
													<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' />&nbsp;<%# DataBinder.Eval(Container.DataItem, "Name")%>
												</td>
											</tr>
										</ItemTemplate>
										<FooterTemplate>
											</table>
										</FooterTemplate>
									</asp:Repeater>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td>
						<dg:datagridextended id="dgRepPers" runat="server" allowsorting="True" allowpaging="True" width="100%" autogeneratecolumns="False" pagesize="10" borderwidth="0" gridlines="None" cellpadding="1">
							<headerstyle></headerstyle>
							<columns>
								<asp:boundcolumn datafield="TemplateId" Visible="False"></asp:boundcolumn>
								<asp:Templatecolumn sortExpression="IsGlobal">
									<headerstyle cssclass="ibn-vh2" width="23px"></headerstyle>
									<itemstyle cssclass="ibn-vb2" width="23px"></itemstyle>
									<itemtemplate>
										<%# GetType((bool)DataBinder.Eval(Container.DataItem, "IsGlobal"))%>
									</itemtemplate>
								</asp:Templatecolumn>
								<asp:templatecolumn sortexpression="TemplateName" headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2">
									<itemtemplate>
										<asp:LinkButton id="ibView" runat="server" borderwidth="0" commandname="View" causesvalidation="False">
											<%# DataBinder.Eval(Container.DataItem, "TemplateName")%>
										</asp:LinkButton>
									</itemtemplate>
								</asp:templatecolumn>
								<asp:Templatecolumn sortExpression="ObjectType">
									<headerstyle cssclass="ibn-vh2" width="90px"></headerstyle>
									<itemstyle cssclass="ibn-vb2" width="90px"></itemstyle>
									<itemtemplate>
										<%# DataBinder.Eval(Container.DataItem, "ObjectType")%>
									</itemtemplate>
								</asp:Templatecolumn>
								<asp:templatecolumn itemstyle-cssclass="ibn-vb2" headerstyle-width="69px" itemstyle-width="69px">
									<itemtemplate>
										<asp:imagebutton ImageAlign="AbsMiddle" id="ibEdit" runat="server" borderwidth="0" imageurl="~/layouts/images/edit.gif" commandname="Edit" causesvalidation="False">
										</asp:imagebutton>&nbsp;
										<a href='../../../Reports/ReportHistory.aspx?TemplateId=<%# DataBinder.Eval(Container.DataItem, "TemplateId")%>'>
											<img align=absmiddle border=0 src='<%= Page.ResolveUrl("~/layouts/images/icon-search.gif") %>' title='<% =LocRM.GetString("tViewReport")%>' /></a>&nbsp;
										<asp:imagebutton ImageAlign="AbsMiddle" id="ibDelete" runat="server" borderwidth="0" imageurl="~/layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
										</asp:imagebutton>
									</itemtemplate>
								</asp:templatecolumn>
							</columns>
						</dg:datagridextended>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
