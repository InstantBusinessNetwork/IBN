<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserReportGeneral.ascx.cs" Inherits="Mediachase.UI.Web.Apps.ReportManagement.Modules.UserReportGeneral" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<ibn:blockheader id="tbLightGeneral" runat="server"></ibn:blockheader>
<table cellpadding="3" cellspacing="3" border="0" width="100%" class="text ibn-stylebox-light">
	<tr>
		<td>
			<table cellpadding="0" cellspacing="0" border="0" width="100%">
				<tr id="trGenReps" runat="server">
					<td style="padding-left: 6px" valign="top">
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/listset.gif") %>' />
					</td>
					<td width="100%" style="padding: 0 0 10 5">
						<span class="text"><b><%=LocRM.GetString("tSystemReports")%></b></span>
						<asp:Repeater ID="repGeneral" runat="server">
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
</table>
