<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules._Default" Codebehind="Default.ascx.cs" %>
<table cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td valign="top"><img alt="" src='<%= Page.ResolveUrl("~/layouts/images/listset.gif") %>' /></td>
		<td style="PADDING-LEFT:5px">
			<table cellspacing="0" cellpadding="4" border="0" class="text">
				<tr id="trPortalSetupHeader" runat=server>
					<td valign="top">
						<%=LocRM.GetString("tPortalSetupH")%>
					</td>
				</tr>
				<tr id="trDictionariesHeader" runat=server>
					<td valign="top">
						<%=LocRM.GetString("tDictionariesH")%>
					</td>
				</tr>
				<tr id="trCustomizationHeader" runat=server>
					<td valign="top">
						<%=LocRM.GetString("tCustomizationH")%>
					</td>
				</tr>
				<tr id="trRoutingHeader" runat=server>
					<td valign="top">
						<%=LocRM.GetString("tRoutingH")%>
					</td>
				</tr>
				<tr id="trPortalSetup1" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="CompanyInfo.aspx">
							<%=LocRM.GetString("tCompanyProfile")%></A>
					</td>
				</tr>
				<tr id="trPortalSetup4" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="CustomizeHomePage.aspx">
							<%=LocRM.GetString("tCustomizeHomePage") %></A>
					</td>
				</tr>
				<tr id="trPortalSetup9" runat=server>
					<td>
					</td>
				</tr>
				<tr id="trPortalSetup3" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="Customization.aspx">
							<%=LocRM.GetString("Customization") %></A>
					</td>
				</tr>
				<tr id="trDataHeader" runat=server>
					<td valign="top">
						<%=LocRM.GetString("tDataH")%>
					</td>
				</tr>
				<tr id="trData1" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="CalendarList.aspx">
							<%=LocRM.GetString("WorkingTimeSetup") %></A>
					</td>
				</tr>
				<tr id="trData6" runat="server">
					<td>
						<img alt="" height="6" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' width="6" /> <a href="WorkTimeSetup.aspx">
							<%=LocRM.GetString("WorkTimeSetup")%></a>
					</td>
				</tr>
				<tr id="trData2" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="ManageDictionaries.aspx">
							<%=LocRM.GetString("ManageDictionaries") %></A>
					</td>
				</tr>
				<tr id="trData3" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="MasterDataManager.aspx">
							<%=LocRM.GetString("tDataDictionaryManager") %></A>
					</td>
				</tr>
				<tr id="trData4" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="Customization.aspx">
							<%=LocRM.GetString("tFieldsEditor") %></A>
					</td>
				</tr>
				<tr id="trData5" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="MdpCustomization.aspx">
							<%=LocRM.GetString("tMdpCustomization") %></A>
					</td>
				</tr>
				<tr id="trComSetH" runat=server>
					<td valign="top">
						<%=LocRM.GetString("tComSetH")%>
					</td>
				</tr>
				<tr id="trComSet1" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="ActiveDirectory.aspx">
							<%=LocRM.GetString("tSecurity") %></A>
					</td>
				</tr>
				<tr id="trComSet2" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="LDAPSettings.aspx">
							<%=LocRM.GetString("tLDAPSettings")%></A>
					</td>
				</tr>
				<tr id="trComSet3" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="SMTPSettings.aspx">
							<%=LocRM.GetString("tSMTPSettings")%></A>
					</td>
				</tr>
				<tr id="trComSet4" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="EmailBoxes.aspx">
							<%=LocRM.GetString("tEmailBoxes")%></A>
					</td>
				</tr>
				<tr id="trComSet5" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="ReportConfig.aspx">
							<%=LocRM.GetString("tReportsSecurity")%></A>
					</td>
				</tr>
				<tr id="trComSet6" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="Miscellaneous.aspx">
							<%=LocRM.GetString("Miscellaneous")%></A>
					</td>
				</tr>
				<tr id="trDictionaries1" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="ManageDictionaries.aspx">
							<%=LocRM.GetString("ManageDictionaries") %></A>
					</td>
				</tr>
				<tr id="trDictionaries2" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="Clients.aspx">
							<%=LocRM.GetString("Clients") %></A>
					</td>
				</tr>
				<tr id="trDictionaries3" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="CalendarList.aspx">
							<%=LocRM.GetString("WorkingTimeSetup") %></A>
					</td>
				</tr>
				<tr id="trDictionaries4" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="DocumentTypes.aspx">
							<%=LocRM.GetString("DocumentTypes") %></A>
					</td>
				</tr>
				<tr id="trCustomization1" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="MasterDataManager.aspx">
							<%=LocRM.GetString("tDataDictionaryManager") %></A>
					</td>
				</tr>
				<tr id="trCustomization2" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="Customization.aspx">
							<%=LocRM.GetString("tFieldsEditor") %></A>
					</td>
				</tr>
				<tr id="trCustomization4" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="MdpCustomization.aspx">
							<%=LocRM.GetString("tMdpCustomization") %></A>
					</td>
				</tr>
				<tr id="trRouting3" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="AlertUser.aspx">
							<%=LocRM.GetString("AlertUser") %></A>
					</td>
				</tr>
				<tr id="trRouting6" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="GlobalSubscription.aspx">
							<%=LocRM2.GetString("tEventNotification") %></A>
					</td>
				</tr>
				<tr id="trRouting7" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="GlobalReminders.aspx">
							<%=LocRM2.GetString("tReminderNotifications") %></A>
					</td>
				</tr>
				<tr id="trRouting1" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="MessageTemplates.aspx">
							<%=LocRM.GetString("MessageTemplates") %></A>
					</td>
				</tr>
				<tr id="trRouting4" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="ReminderTemplates.aspx">
							<%=LocRM.GetString("ReminderTemplates") %></A>
					</td>
				</tr>
				<tr id="trRouting5" runat=server>
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <A href="SpecialTemplates.aspx">
							<%=LocRM.GetString("SpecialTemplates") %></A>
					</td>
				</tr>
				<tr id="trHelpDeskH" runat="server">
					<td valign="top">
						<%=LocRM.GetString("tHelpDeskH")%>
					</td>
				</tr>
				<tr id="trHelpDesk1" runat="server">
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <a href="HDMSettings.aspx">
							<%=LocRM.GetString("tHelpDeskComSet")%></a>
					</td>
				</tr>
				<tr id="trHelpDesk2" runat="server">
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <a href="EmailRules.aspx">
							<%=LocRM.GetString("tHelpDeskIssRules")%></a>
					</td>
				</tr>
				<tr id="trHelpDesk3" runat="server">
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <a href="EmailBlackList.aspx">
							<%=LocRM.GetString("tHelpDeskBlackList")%></a>
					</td>
				</tr>
				<tr id="trHelpDesk4" runat="server">
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /><a href="EmailWhiteList.aspx">
							<%=LocRM.GetString("tHelpDeskWhiteList")%></a>
					</td>
				</tr>
				<tr id="trHelpDesk5" runat="server">
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /><a href='<%= Page.ResolveUrl("~/Incidents/EmailLog.aspx") %>'>
							<%=LocRM.GetString("tHelpDeskEmailLog")%></a>
					</td>
				</tr>
				<tr id="trFilesFormsH" runat="server">
					<td>
						<%=LocRM.GetString("tFilesFormsH")%>
					</td>
				</tr>
				<tr id="trFilesForms1" runat="server">
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <a href="SearchSettings.aspx">
							<%=LocRM.GetString("tFilesFormsComSet")%></a>
					</td>
				</tr>
				<tr id="trFilesForms2" runat="server">
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <a href="DocumentTypes.aspx">
							<%=LocRM.GetString("tFilesFormsDocTypes")%></a>
					</td>
				</tr>
				<tr id="trAddToolsH" runat="server">
					<td>
						<%=LocRM.GetString("tAddToolsH")%>
					</td>
				</tr>
				<tr id="trAddTools1" runat="server">
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <a href="CustomSQLReport.aspx">
							<%=LocRM.GetString("tAddToolsCustSQLRep")%></a>
					</td>
				</tr>
				<tr id="trAddTools2" runat="server">
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <a href='<%= Page.ResolveUrl("~/Admin/ErrorLog.aspx") %>'>
							<%=LocRM.GetString("tAddToolsErrList")%></a>
					</td>
				</tr>
				<tr id="trAddTools3" runat="server">
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <a href='<%= Page.ResolveUrl("~/Reports/default.aspx?Tab=Administrative") %>'>
							<%=LocRM.GetString("tAddToolsStat")%></a>
					</td>
				</tr>
				<tr id="trAddTools4" runat="server">
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <a href="BroadcastAlerts.aspx">
							<%=LocRM.GetString("tAddToolsImpMess")%></a>
					</td>
				</tr>
				<tr id="trAddTools5" runat="server">
					<td>
						<img alt="" src='<%= Page.ResolveUrl("~/layouts/images/rect.gif") %>' /> <a href="webstubs.aspx">
							<%=LocRM.GetString("tAddToolsWebstubs")%></a>
					</td>
				</tr>
			</table>	
		</td>
	</tr>
</table>
