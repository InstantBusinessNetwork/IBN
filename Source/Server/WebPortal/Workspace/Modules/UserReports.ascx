<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Workspace.Modules.UserReports" Codebehind="UserReports.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<script language="javascript">
function OpenWindow(query,w,h,scroll)
	{
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		
		winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
		if (scroll) winprops+=',scrollbars=1';
		var f = window.open(query, "_blank", winprops);
	}
</script>
<table cellpadding="5" cellspacing="0" width="100%">
	<tr>
		<td valign="top" width="50%">
			<ibn:blockheader id="tbLightPers" runat="server"></ibn:blockheader>
			<table cellpadding="3" cellspacing="3" border="0" width="100%" class="text ibn-stylebox-light">
				<tr>
					<td>
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr id="trPersReps" runat="server">
								<td style="PADDING-LEFT: 6px" vAlign="top"><IMG height="24" align="absmiddle" src="../layouts/images/listset.gif" width="34"></td>
								<td width="100%" style="PADDING: 0px 0px 10px 5px;">
									<span class="text"><b><%=LocRM.GetString("tSystemReports")%></b></span>
									<asp:Repeater ID="repPerc" Runat=server>
										<HeaderTemplate>
											<table cellpadding="5" cellspacing="0" border="0" width="100%" class="text">
										</HeaderTemplate>
										<ItemTemplate>
											<tr><td>
											<img height="6" src="../layouts/images/rect.gif" width="6" align=absmiddle>&nbsp;<%# DataBinder.Eval(Container.DataItem, "Name")%>
											</td></tr>
										</ItemTemplate>
										<FooterTemplate>
											</table>
										</FooterTemplate>
									</asp:Repeater>
								</td>
							</tr>
							<tr id="trHide1" runat=server>
								<td style="PADDING-LEFT: 6px" vAlign="top"><IMG height="24" align="absmiddle" src="../layouts/images/listset.gif" width="34"></td>
								<td width="100%" style="PADDING: 5px 0px 10px 5px;">
									<span class="text"><b><%=LocRM.GetString("tCustomReports")%></b></span></td>
							</tr>
							<tr id="trHide2" runat=server>
								<td style="PADDING-TOP: 10px;" colspan="2">
									<dg:datagridextended id="dgRepPers" runat="server" allowsorting="True" 
										allowpaging="True" width="100%" autogeneratecolumns="False" 
										PageSize="10" borderwidth="0" gridlines="None" cellpadding="1">
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
													<asp:linkbutton id="ibView" runat="server" borderwidth="0" commandname="View" causesvalidation="False">
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
													<asp:imagebutton ImageAlign="AbsMiddle" id="ibEdit" runat="server" borderwidth="0" imageurl="../../layouts/images/edit.gif" commandname="Edit" causesvalidation="False">
													</asp:imagebutton>&nbsp;
													<a href='../Reports/ReportHistory.aspx?TemplateId=<%# DataBinder.Eval(Container.DataItem, "TemplateId")%>'>
														<img align=absmiddle border=0 src='../Layouts/Images/icon-search.gif' title='<% =LocRM.GetString("tViewReport")%>' /></a>&nbsp;
													<asp:imagebutton ImageAlign="AbsMiddle" id="ibDelete" runat="server" borderwidth="0" imageurl="../../layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
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
		</td>
		<td valign="top" width="50%">
			<ibn:blockheader id="tbLightGeneral" runat="server"></ibn:blockheader>
			<table cellpadding="3" cellspacing="3" border="0" width="100%" class="text ibn-stylebox-light">
				<tr>
					<td>
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr id="trGenReps" runat="server">
								<td style="PADDING-LEFT: 6px" vAlign="top"><IMG height="24" align="absmiddle" src="../layouts/images/listset.gif" width="34"></td>
								<td width="100%" style="PADDING: 0 0 10 5">
									<span class="text"><b><%=LocRM.GetString("tSystemReports")%></b></span>
									<asp:Repeater ID="repGeneral" Runat=server>
										<HeaderTemplate>
											<table cellpadding="5" cellspacing="0" border="0" width="100%" class="text">
										</HeaderTemplate>
										<ItemTemplate>
											<tr><td>
											<img height="6" src="../layouts/images/rect.gif" width="6" align=absmiddle>&nbsp;<%# DataBinder.Eval(Container.DataItem, "Name")%>
											</td></tr>
										</ItemTemplate>
										<FooterTemplate>
											</table>
										</FooterTemplate>
									</asp:Repeater>
								</td>
							</tr>
							<tr id="trHide3" runat=server>
								<td style="PADDING-LEFT: 6px" vAlign="top"><IMG height="24" align="absmiddle" src="../layouts/images/listset.gif" width="34"></td>
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
													<asp:imagebutton ImageAlign="AbsMiddle" id="ibEdit1" runat="server" borderwidth="0" imageurl="../../layouts/images/edit.gif" commandname="Edit" causesvalidation="False">
													</asp:imagebutton>&nbsp;
													<a href='../Reports/ReportHistory.aspx?TemplateId=<%# DataBinder.Eval(Container.DataItem, "TemplateId")%>'>
														<img align=absmiddle border=0 src='../Layouts/Images/icon-search.gif' title='<% =LocRM.GetString("tViewReport")%>' /></a>&nbsp;
													<asp:imagebutton ImageAlign="AbsMiddle" id="ibDelete1" runat="server" borderwidth="0" imageurl="../../layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
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
		</td>
	</tr>
</table>