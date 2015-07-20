<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.Settings" CodeBehind="Settings.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top: 0px;">
	<tr>
		<td>
			<ibn:BlockHeader ID="secH" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td style="padding-right: 8px; padding-left: 8px; padding-top: 8px">
			<table class="ms-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td style="width: 5%">
						<div>
						</div>
					</td>
					<td style="width: 62%; height: 15px" valign="top">
						<fieldset>
							<legend class="text" id="lgdSets" runat="server"></legend>
							<table class="text" cellspacing="7" cellpadding="0" width="100%" border="0">
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblPeriod" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td>
										<asp:Label ID="valPeriod" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblMaxHDD" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td>
										<asp:Label ID="valMaxHDD" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblAllowedUsers" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td>
										<asp:Label ID="valMaxUsers" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px" valign="top">
										<strong>
											<asp:Label ID="lblAllowedExternalUsers" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td>
										<asp:Label ID="lblExternalUsers" CssClass="text" runat="server"></asp:Label>
									</td>
								</tr>
							</table>
						</fieldset>
						<fieldset style="margin-top: 5px;">
							<legend class="text" id="lgdGeneral" runat="server"></legend>
							<table class="text" cellspacing="7" cellpadding="0" width="100%" border="0">
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblAutoDeactivateExpired" runat="server" CssClass="text"></asp:Label>:</strong>
									</td>
									<td>
										<asp:Label ID="valAutoDeactivateExpired" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblAutoDeleteOutdated" runat="server" CssClass="text"></asp:Label>:</strong>
									</td>
									<td>
										<asp:Label ID="valAutoDeleteOutdated" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblOutdatePeriod" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td>
										<asp:Label ID="valOutdatePeriod" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
							</table>
						</fieldset>
						<fieldset style="margin-top: 5px;">
							<legend class="text" id="lgdTariffSettings" runat="server"></legend>
							<table class="text" cellspacing="7" cellpadding="0" width="100%" border="0">
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblUseTariffs" runat="server" CssClass="text"></asp:Label>:</strong>
									</td>
									<td>
										<asp:Label ID="valUseTariffs" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr runat="server" id="AutoDeactivateUnpaidRow">
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblAutoDeactivateUnpaid" runat="server" CssClass="text"></asp:Label>:</strong>
									</td>
									<td>
										<asp:Label ID="valAutoDeactivateUnpaid" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr runat="server" id="DefaultTariffRow">
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("DefaultTariff")%>:</strong>
									</td>
									<td>
										<asp:Label ID="DefaultTariff" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
							</table>
						</fieldset>
						<fieldset style="margin-top: 5px;">
							<legend class="text" id="legendDns" runat="server"></legend>
							<table class="text" cellspacing="7" cellpadding="0" width="100%" border="0">
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblDnsParentDomain" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td>
										<asp:Label ID="valDnsParentDomain" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
							</table>
						</fieldset>
						<fieldset style="margin-top: 5px;">
							<legend class="text" id="legendIis" runat="server"></legend>
							<table class="text" cellspacing="7" cellpadding="0" width="100%" border="0">
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblIisIpAddress" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td>
										<asp:Label ID="valIisIpAddress" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblIisPort" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td>
										<asp:Label ID="valIisPort" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("DefaultTrialPool")%>:</strong>
									</td>
									<td>
										<asp:Label ID="DefaultTrialPoolValue" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("DefaultBillablePool")%>:</strong>
									</td>
									<td>
										<asp:Label ID="DefaultBillablePoolValue" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
							</table>
						</fieldset>
						<fieldset style="margin-top: 5px;">
							<legend class="text" id="SmtpLegend" runat="server"></legend>
							<table class="text" cellspacing="7" cellpadding="0" width="100%" border="0">
								<tr>
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("SmtpServer")%>:</strong>
									</td>
									<td>
										<asp:Label ID="SmtpServerValue" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("SmtpPort")%>:</strong>
									</td>
									<td>
										<asp:Label ID="SmtpPortValue" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("SmtpSecureConnection")%>:</strong>
									</td>
									<td>
										<asp:Label ID="SmtpSecureConnectionValue" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("SmtpAuthenticate")%>:</strong>
									</td>
									<td>
										<asp:Label ID="SmtpAuthenticateValue" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr runat="server" id="SmtpUserRow">
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("SmtpUser")%>:</strong>
									</td>
									<td>
										<asp:Label ID="SmtpUserValue" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
							</table>
						</fieldset>
						<fieldset style="margin-top: 5px;">
							<legend class="text" id="lgdNotif" runat="server"></legend>
							<table class="text" cellspacing="7" cellpadding="0" width="100%" border="0">
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblEMailFrom" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td colspan="3">
										<asp:Label ID="valEMailFrom" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblOperatorEmail" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td colspan="3">
										<asp:Label ID="valOperatorEmail" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblSendSpam" runat="server" CssClass="text"></asp:Label>:</strong>
									</td>
									<td colspan="3">
										<asp:Label ID="valSendSpam" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr runat="server" id="trSendSpamOneDayAfter">
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblSendSpamOneDayAfter" runat="server" CssClass="text"></asp:Label>:</strong>
									</td>
									<td style="width: 100px">
										<asp:Label ID="valSendSpamOneDayAfter" runat="server" CssClass="text"></asp:Label>
									</td>
									<td style="width: 150px" runat="server" id="tdlblOneDayAfterPeriod">
										<strong>
											<asp:Label ID="lblOneDayAfterPeriod" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td runat="server" id="tdvalOneDayAfterPeriod">
										<asp:Label ID="valOneDayAfterPeriod" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr runat="server" id="trSendSpamOneWeekAfter">
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblSendSpamOneWeekAfter" runat="server" CssClass="text"></asp:Label>:</strong>
									</td>
									<td style="width: 100px">
										<asp:Label ID="valSendSpamOneWeekAfter" runat="server" CssClass="text"></asp:Label>
									</td>
									<td style="width: 150px" runat="server" id="tdlblOneWeekAfterPeriod">
										<strong>
											<asp:Label ID="lblOneWeekAfterPeriod" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td runat="server" id="tdvalOneWeekAfterPeriod">
										<asp:Label ID="valOneWeekAfterPeriod" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr runat="server" id="trSendSpamOneWeekBefore">
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblSendSpamOneWeekBefore" runat="server" CssClass="text"></asp:Label>:</strong>
									</td>
									<td style="width: 100px">
										<asp:Label ID="valSendSpamOneWeekBefore" runat="server" CssClass="text"></asp:Label>
									</td>
									<td style="width: 150px" runat="server" id="tdlblOneWeekBeforePeriod">
										<strong>
											<asp:Label ID="lblOneWeekBeforePeriod" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td runat="server" id="tdvalOneWeekBeforePeriod">
										<asp:Label ID="valOneWeekBeforePeriod" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr runat="server" id="trSendSpamOneDayBefore">
									<td style="width: 260px">
										<strong>
											<asp:Label ID="lblSendSpamOneDayBefore" runat="server" CssClass="text"></asp:Label>:</strong>
									</td>
									<td style="width: 100px">
										<asp:Label ID="valSendSpamOneDayBefore" runat="server" CssClass="text"></asp:Label>
									</td>
									<td style="width: 150px" runat="server" id="tdlblOneDayBeforePeriod">
										<strong>
											<asp:Label ID="lblOneDayBeforePeriod" runat="server" CssClass="text"></asp:Label></strong>
									</td>
									<td runat="server" id="tdvalOneDayBeforePeriod">
										<asp:Label ID="valOneDayBeforePeriod" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr>
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("SendBillableSpam")%>:</strong>
									</td>
									<td colspan="3">
										<asp:Label ID="SendBillableSpamValue" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr runat="server" id="SendBillableSpam7dayRow">
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("SendBillableSpam7day")%>:</strong>
									</td>
									<td colspan="3">
										<asp:Label ID="SendBillableSpam7dayValue" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr runat="server" id="SendBillableSpam3dayRow">
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("SendBillableSpam3day")%>:</strong>
									</td>
									<td colspan="3">
										<asp:Label ID="SendBillableSpam3dayValue" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr runat="server" id="SendBillableSpam1dayRow">
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("SendBillableSpam1day")%>:</strong>
									</td>
									<td colspan="3">
										<asp:Label ID="SendBillableSpam1dayValue" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
								<tr runat="server" id="SendBillableSpamNegativeBalanceRow">
									<td style="width: 260px">
										<strong>
											<%= LocRM.GetString("SendBillableSpamNegativeBalance")%>:</strong>
									</td>
									<td colspan="3">
										<asp:Label ID="SendBillableSpamNegativeBalanceValue" runat="server" CssClass="text"></asp:Label>
									</td>
								</tr>
							</table>
						</fieldset>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
