<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.AdminReports" Codebehind="AdminReports.ascx.cs" %>
<%@ Reference Control="~/Modules/Separator1.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="~/Modules/Separator1.ascx"%>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx"%>

<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:BlockHeader id="Header3" title='<%#LocRM.GetString("QuickInformation")%>' runat="server" />
		</td>
	</tr>
	<tr>
		<td style="padding:0px;">
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
				<tr>
					<td valign="top">
						<table class="text" cellspacing="0" cellpadding="3" border="0" width="100%">
							<tr>
								<td colspan="2" style="PADDING:0px">
									<ibn:sep width="100%" id="Sep1" runat="server" title='<%#LocRM.GetString("UserAndGroupStats")%>' IsClickable="false" />
								</td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("TotalUsers")%>:</td>
								<td class="ibn-label"><asp:label id="lblTotalUsers" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("TotalGroups")%>:</td>
								<td class="ibn-label"><asp:label id="lblTotalGroups" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td colspan="2" class="ibn-vb2" height="1" style="padding:0px"><img src='<%= ResolveClientUrl("~/layouts/images/blank.gif") %>' /></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("SecureGroups")%>:</td>
								<td class="ibn-label"><asp:label id="lblSecureGroups" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("PartnerGroups")%>:</td>
								<td class="ibn-label"><asp:label id="lblPartnerGroups" Runat="server"></asp:label></td>
							</tr>
							<tr id="trIMBlock0" runat="server">
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("ContactGroups")%>:</td>
								<td class="ibn-label"><asp:label id="lblContactGroups" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td colspan="2" class="ibn-vb2" height="1" style="padding:0px"><img src='<%= ResolveClientUrl("~/layouts/images/blank.gif") %>' /></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("InternalActiveUsers")%>:</td>
								<td class="ibn-label"><asp:label id="InternalActiveUsersLabel" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("InternalDisabledUsers")%>:</td>
								<td class="ibn-label"><asp:label id="InternalDisabledUsersLabel" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td colspan="2" class="ibn-vb2" height="1" style="padding:0px"><img src='<%= ResolveClientUrl("~/layouts/images/blank.gif") %>' /></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("ExternalActiveUsers")%>:</td>
								<td class="ibn-label"><asp:label id="ExternalActiveUsersLabel" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("ExternalDisabledUsers")%>:</td>
								<td class="ibn-label"><asp:label id="ExternalDisabledUsersLabel" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td colspan="2" class="ibn-vb2" height="1" style="padding:0px"><img src='<%= ResolveClientUrl("~/layouts/images/blank.gif") %>' /></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("TotalActiveUsers")%>:</td>
								<td class="ibn-label"><asp:label id="TotalActiveUsersLabel" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("TotalDisabledUsers")%>:</td>
								<td class="ibn-label"><asp:label id="TotalDisabledUsersLabel" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td colspan="2" class="ibn-vb2" height="1" style="padding:0px"><img src='<%= ResolveClientUrl("~/layouts/images/blank.gif") %>' /></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("RegularUsers")%>:</td>
								<td class="ibn-label"><asp:label id="lblRegularUsers" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("PartnerUsers")%>:</td>
								<td class="ibn-label"><asp:label id="lblPartnerUsers" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("ExternalUsers")%>:</td>
								<td class="ibn-label"><asp:label id="lblExternalUsers" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td colspan="2" style="PADDING:0px">
									<ibn:sep width="100%" id="Sep2" runat="server" title='<%#LocRM.GetString("AccessUsageStats")%>' IsClickable="false" />
								</td>
							</tr>
							<tr id="trIMBlock1" runat="server">
								<td width="250px" style="padding-left:10px"><asp:Label ID="lblIMCount" runat="server"></asp:Label>:</td>
								<td class="ibn-label"><asp:label id="lblIMLoginCount" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("PortalLoginCount")%>:</td>
								<td class="ibn-label"><asp:label id="lblPortalCount" Runat="server"></asp:label></td>
							</tr>
							<tr id="trIMBlock2" runat="server">
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("MessagesSent")%>:</td>
								<td class="ibn-label"><asp:label id="lblMessagesSent" Runat="server"></asp:label></td>
							</tr>
							<tr id="trIMBlock3" runat="server">
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("MessagesReceived")%>:</td>
								<td class="ibn-label"><asp:label id="lblMessagesReceived" Runat="server"></asp:label></td>
							</tr>
							<tr>
								<td colspan="2" style="PADDING:0px">
									<ibn:sep width="100%" id="Sep4" runat="server" title='<%#LocRM.GetString("PortalRestrictions")%>' IsClickable="false" />
								</td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("DatabaseSize")%>:</td>
								<td>
								<asp:label id="DatabaseSizeInfoLabel" Runat="server"></asp:label>
								</td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("InternalActiveUsers")%>:</td>
								<td>
									<asp:label id="InternalUsersInfoLabel" Runat="server"></asp:label>
								</td>
							</tr>
							<tr>
								<td width="250px" style="padding-left:10px"><%=LocRM.GetString("ExternalActiveUsers")%>:</td>
								<td>
									<asp:label id="ExternalUsersInfoLabel" Runat="server"></asp:label>
								</td>
							</tr>
						</table>
					</td>
					<td valign="top" runat="server" id="tdStatusLog" style="border-left: solid 1px #C2C2C2; width: 400px;">
						<table class="text" cellspacing="0" cellpadding="3" border="0" width="100%">
							<tr>
								<td colspan="2" style="padding:0px;">
									<ibn:sep width="100%" id="Sep3" runat="server" title='<%#LocRM.GetString("StatusLog")%>' IsClickable="false" />
									<br />
								</td>
							</tr>
							<tr>
								<td style="padding-left:10px; width:100px"><%=LocRM.GetString("Group")%>:</td>
								<td class="ibn-label">
									<asp:DropDownList runat="server" ID="ddlGroup" Width="250" OnSelectedIndexChanged="ddlGroup_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
								</td>
							</tr>
							<tr>
								<td style="padding-left:10px; width:100px"><%=LocRM.GetString("User")%>:</td>
								<td class="ibn-label">
									<asp:DropDownList runat="server" ID="ddlUser" Width="250"></asp:DropDownList>
								</td>
							</tr>
							<tr>
								<td style="padding-left:10px; width:100px"><%=LocRM.GetString("From")%>:</td>
								<td class="ibn-label">
									<mc:Picker ID="fromDate" runat="server" DateCssClass="text" TimeCssClass="text" DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" DateIsRequired="true" />
								</td>
							</tr>
							<tr>
								<td style="padding-left:10px; width:100px"><%=LocRM.GetString("To")%>:</td>
								<td class="ibn-label">
									<mc:Picker ID="toDate" runat="server" DateCssClass="text" TimeCssClass="text" DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" DateIsRequired="true" />
								</td>
							</tr>
							<tr>
								<td></td>
								<td>
									<asp:Button runat="server" ID="btnGenerate" OnClick="btnGenerate_Click" />
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
