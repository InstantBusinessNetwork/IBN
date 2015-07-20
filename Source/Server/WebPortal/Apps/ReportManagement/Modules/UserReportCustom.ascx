<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserReportCustom.ascx.cs" Inherits="Mediachase.UI.Web.Apps.ReportManagement.Modules.UserReportCustom" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>

<ibn:blockheader id="tbLightGeneral" runat="server"></ibn:blockheader>
<table cellpadding="3" cellspacing="3" border="0" width="100%" class="text ibn-stylebox-light">
	<tr>
		<td>
			<table cellpadding="0" cellspacing="0" border="0" width="100%">
				<tr id="trHide3" runat=server>
					<td style="PADDING-LEFT: 6px" valign="top"><img alt="" src='<%= Page.ResolveUrl("~/layouts/images/listset.gif") %>' /></td>
					<td width="100%" style="PADDING: 5 0 10 5">
						<span class="text"><b><%=LocRM.GetString("tCustomReports")%></b></span></td>
				</tr>
				<tr id="trHide4" runat=server>
					<td style="PADDING-TOP: 10px" colspan="2">
						<dg:datagridextended id="dgRepGeneral" runat="server" allowsorting="True" allowpaging="True" width="100%" autogeneratecolumns="False" PageSize="10" borderwidth="0" gridlines="None" cellpadding="1">
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
										<asp:linkbutton id="ibView1" runat="server" borderwidth="0" commandname="View" causesvalidation="False">
											<%# DataBinder.Eval(Container.DataItem, "TemplateName")%>
										</asp:linkbutton>
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
										<asp:imagebutton ImageAlign="AbsMiddle" id="ibEdit1" runat="server" borderwidth="0" imageurl="~/layouts/images/edit.gif" commandname="Edit" causesvalidation="False">
										</asp:imagebutton>&nbsp;
										<a href='../../../Reports/ReportHistory.aspx?TemplateId=<%# DataBinder.Eval(Container.DataItem, "TemplateId")%>'>
											<img align=absmiddle border=0 src='<%= Page.ResolveUrl("~/layouts/images/icon-search.gif") %>' title='<% =LocRM.GetString("tViewReport")%>' /></a>&nbsp;
										<asp:imagebutton ImageAlign="AbsMiddle" id="ibDelete1" runat="server" borderwidth="0" imageurl="~/layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
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