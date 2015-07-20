<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.SettingsEdit" Codebehind="SettingsEdit.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px;">
	<tr>
		<td><ibn:blockheader id="secH" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="PADDING-RIGHT: 8px; PADDING-LEFT: 8px; PADDING-TOP: 8px" valign="top">
			<!-- body -->
			<table class="ms-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0" style="table-layout:fixed">
				<tr>
					<td width="20">&nbsp;</td>
					<td width="800">
						<fieldset><legend class="text" id=lgdSets runat="server"></legend>
							<table class="text" cellspacing="4" cellpadding="2" width="100%" border="0">
								<tr>
									<td width="180"><strong><asp:label id=lblPeriod CssClass="text" Runat="server"></asp:label></strong></td>
									<td><asp:textbox id=txtPeriod CssClass="text" Runat="server" Width="300px"></asp:textbox><asp:requiredfieldvalidator id=RequiredFieldValidator2 runat="server" ErrorMessage="*" ControlToValidate="txtPeriod"></asp:requiredfieldvalidator><asp:rangevalidator id=RangeValidator2 runat="server" ErrorMessage="minimum value - 1 Day" ControlToValidate="txtPeriod" MinimumValue="1" MaximumValue="2000000000" Type="Integer"></asp:rangevalidator></td>
								</tr>
								<tr>
									<td width="180"><strong><asp:label id=lblMaxHDD CssClass="text" Runat="server"></asp:label></strong></td>
									<td><asp:textbox id=txtMaxHDD CssClass="text" Runat="server" Width="300px"></asp:textbox><asp:requiredfieldvalidator id=RequiredFieldValidator1 runat="server" ErrorMessage="*" ControlToValidate="txtMaxHDD"></asp:requiredfieldvalidator><asp:rangevalidator id=RangeValidator1 runat="server" ErrorMessage="minimum value -30 Mb" ControlToValidate="txtMaxHDD" MinimumValue="30" MaximumValue="2000000000" Type="Integer"></asp:rangevalidator></td>
								</tr>
								<tr>
									<td valign="top" width="180"><strong><asp:label id="lblAllowedUsers" Runat="server" CssClass="text"></asp:label></strong></td>
									<td>
										<asp:textbox id=txtMaxUsers CssClass="text" Runat="server" Width="300px"></asp:textbox><asp:requiredfieldvalidator id=RequiredFieldValidator3 runat="server" ErrorMessage="*" ControlToValidate="txtMaxUsers"></asp:requiredfieldvalidator><asp:rangevalidator id=RangeValidator3 runat="server" ErrorMessage="*" ControlToValidate="txtMaxUsers" MinimumValue="-1" MaximumValue="2000000000" Type="Integer"></asp:rangevalidator><br />
										Use -1 for maximum user count allowed by license
									</td>
								</tr>
								<tr>
									<td valign="top" width="180"><strong><asp:label id="lblAllowedExternalUsers" Runat="server" CssClass="text"></asp:label></strong></td>
									<td>
										<asp:textbox id=tbExternalUsers CssClass="text" Runat="server" Width="300px"></asp:textbox><asp:requiredfieldvalidator id=RequiredFieldValidator4 runat="server" ErrorMessage="*" ControlToValidate="tbExternalUsers"></asp:requiredfieldvalidator><asp:rangevalidator id=RangeValidator4 runat="server" ErrorMessage="*" ControlToValidate="tbExternalUsers" MinimumValue="-1" MaximumValue="2000000000" Type="Integer"></asp:rangevalidator><br />
										Use -1 for maximum external user count allowed by license
									</td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>&nbsp;</td>
				</tr>
				
				<tr>
					<td>&nbsp;</td>
					<td>
						<fieldset style="margin-top:5px;"><legend class="text" id="lgdGeneral" runat="server"></legend>
							<table class="text" cellspacing="4" cellpadding="2" width="100%" border="0">
								<tr>
									<td width="180"></td>
									<td>
										<asp:CheckBox runat="server" ID="chkAutoDeactivateExpired" Font-Bold="false" />
									</td>
								</tr>
								<tr>
									<td width="180"></td>
									<td>
										<asp:CheckBox runat="server" ID="chkAutoDeleteOutdated" Font-Bold="false" />
									</td>
								</tr>
								<tr>
									<td width="180"><strong><asp:label id="lblOutdatePeriod" CssClass="text" Runat="server"></asp:label></strong></td>
									<td>
										<asp:textbox id="txtOutdatePeriod" CssClass="text" Runat="server" Width="300px"></asp:textbox>
										<asp:requiredfieldvalidator id="RequiredFieldValidator5" runat="server" ErrorMessage="*" ControlToValidate="txtOutdatePeriod"></asp:requiredfieldvalidator>
										<asp:rangevalidator id=RangeValidator5 runat="server" ErrorMessage="minimum value - 1 Day" ControlToValidate="txtOutdatePeriod" MinimumValue="1" MaximumValue="2000000000" Type="Integer" Display="Dynamic"></asp:rangevalidator>
									</td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>&nbsp;</td>
				</tr>
				
				<tr>
					<td>&nbsp;</td>
					<td>
						<fieldset style="margin-top:5px;"><legend class="text" id="lgdTariff" runat="server"></legend>
							<table class="text" cellspacing="4" cellpadding="2" width="100%" border="0">
								<tr>
									<td width="180"></td>
									<td><asp:CheckBox runat="server" ID="UseTariffsCheckBox" Font-Bold="false" /></td>
								</tr>
								<tr>
									<td width="180"></td>
									<td><asp:CheckBox runat="server" ID="AutoDeactivateUnpaidCheckBox" Font-Bold="false" /></td>
								</tr>
								<tr>
									<td><strong><%= LocRM.GetString("DefaultTariff")%>:</strong></td>
									<td>
										<asp:DropDownList runat="server" ID="DefaultTariff" Width="300"></asp:DropDownList>
									</td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>&nbsp;</td>
				</tr>
				
				<tr>
					<td>&nbsp;</td>
					<td>
						<fieldset style="margin-top:5px;"><legend class="text" id="legendDns" runat="server"></legend>
							<table class="text" cellspacing="4" cellpadding="2" width="100%" border="0">
								<tr>
									<td width="180"><strong><asp:label id=lblDnsParentDomain CssClass="text" Runat="server"></asp:label></strong></td>
									<td><asp:textbox id=txtDnsParentDomain CssClass="text" Runat="server" Width="300px"></asp:textbox></td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>&nbsp;</td>
				</tr>
				
				<tr>
					<td>&nbsp;</td>
					<td>
						<fieldset style="margin-top:5px;"><legend class="text" id="legendIis" runat="server"></legend>
							<table class="text" cellspacing="4" cellpadding="2" width="100%" border="0">
								<tr>
									<td width="180"><strong><asp:label id="lblIisIpAddress" CssClass="text" Runat="server"></asp:label></strong></td>
									<td><asp:textbox id="txtIisIpAddress" CssClass="text" Runat="server" Width="300px"></asp:textbox></td>
								</tr>
								<tr>
									<td width="180"><strong><asp:label id="lblIisPort" CssClass="text" Runat="server"></asp:label></strong></td>
									<td>
										<asp:textbox id="txtIisPort" CssClass="text" Runat="server" Width="300px"></asp:textbox>
										<asp:requiredfieldvalidator id="RequiredFieldvalidatorIisPort" runat="server" ErrorMessage="*" ControlToValidate="txtIisPort"></asp:requiredfieldvalidator>
										<asp:rangevalidator id="RangeValidatorIisPort" runat="server" ErrorMessage="1 - 32767" ControlToValidate="txtIisPort" MinimumValue="1" MaximumValue="32767" Type="Integer"></asp:rangevalidator>
									</td>
								</tr>
								<tr>
									<td width="180"><strong><%= LocRM.GetString("DefaultTrialPool")%>:</strong></td>
									<td><asp:DropDownList runat="server" ID="DefaultTrialPoolList" Width="300"></asp:DropDownList></td>
								</tr>
								<tr>
									<td width="180"><strong><%= LocRM.GetString("DefaultBillablePool")%>:</strong></td>
									<td><asp:DropDownList runat="server" ID="DefaultBillablePoolList" Width="300"></asp:DropDownList></td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>&nbsp;</td>
				</tr>
				
				<tr>
					<td>&nbsp;</td>
					<td>
						<fieldset style="margin-top:5px;"><legend class="text" id="SmtpSettingsLegend" runat="server"></legend>
							<table class="text" cellspacing="4" cellpadding="2" width="100%" border="0">
								<tr>
									<td width="180"><strong><%= LocRM.GetString("SmtpServer")%>:</strong></td>
									<td><asp:textbox id="SmtpServerValue" CssClass="text" Runat="server" Width="300px"></asp:textbox></td>
								</tr>
								<tr>
									<td width="180"><strong><%= LocRM.GetString("SmtpPort")%>:</strong></td>
									<td>
										<asp:textbox id="SmtpPortValue" CssClass="text" Runat="server" Width="300px"></asp:textbox>
										<asp:requiredfieldvalidator id="RequiredFieldvalidator10" runat="server" ErrorMessage="*" ControlToValidate="SmtpPortValue"></asp:requiredfieldvalidator>
										<asp:rangevalidator id="RangeValidator10" runat="server" ErrorMessage="1 - 32767" ControlToValidate="SmtpPortValue" MinimumValue="1" MaximumValue="32767" Type="Integer"></asp:rangevalidator>
									</td>
								</tr>
								<tr>
									<td width="180"><strong><%= LocRM.GetString("SmtpSecureConnection")%>:</strong></td>
									<td><asp:DropDownList runat="server" ID="SmtpSecureConnectionList" Width="300"></asp:DropDownList></td>
								</tr>
								<tr>
									<td width="180"></td>
									<td><asp:CheckBox runat="server" ID="SmtpAuthenticateValue" Font-Bold="false" /></td>
								</tr>
								<tr>
									<td width="180"><strong><%= LocRM.GetString("SmtpUser")%>:</strong></td>
									<td><asp:textbox id="SmtpUserValue" CssClass="text" Runat="server" Width="300px"></asp:textbox></td>
								</tr>
								<tr>
									<td width="180"><strong><%= LocRM.GetString("SmtpPassword")%>:</strong></td>
									<td><asp:textbox id="SmtpPasswordValue" CssClass="text" Runat="server" Width="300px"></asp:textbox></td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>&nbsp;</td>
				</tr>
				
				<tr>
					<td>&nbsp;</td>
					<td>
						<fieldset style="margin-top:5px;"><legend class="text" id=lgdNotif runat="server"></legend>
							<table class="text" cellspacing="4" cellpadding="2" width="100%" border="0">
								<tr>
									<td width="180"><strong><asp:label id=lblEMailFrom CssClass="text" Runat="server"></asp:label></strong></td>
									<td colspan="3"><asp:textbox id=txtEMailFrom CssClass="text" Runat="server" Width="300px"></asp:textbox><asp:regularexpressionvalidator id=revEMailFrom runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage="*" Display="Dynamic" ControlToValidate="txtEMailFrom"></asp:regularexpressionvalidator></td>
								</tr>
								<tr>
									<td width="180"><strong><asp:label id="lblOperatorEmail" CssClass="text" Runat="server"></asp:label></strong></td>
									<td colspan="3"><asp:textbox id="txtOperatorEmail" CssClass="text" Runat="server" Width="300px"></asp:textbox><asp:regularexpressionvalidator id="revOperatorEmail" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage="*" Display="Dynamic" ControlToValidate="txtOperatorEmail"></asp:regularexpressionvalidator></td>
								</tr>
								<tr>
									<td width="180"></td>
									<td colspan="3"><asp:CheckBox runat="server" ID="chkSendSpam" Font-Bold="false" /></td>
								</tr>
								<tr>
									<td width="180"></td>
									<td width="300" style="padding-left:20px;"><asp:CheckBox runat="server" ID="chkSendSpamOneDayAfter" Font-Bold="false" /></td>
									<td width="120"><asp:label id="lblOneDayAfterPeriod" CssClass="text" Runat="server" Font-Bold="true"></asp:label></td>
									<td>
										<asp:textbox id="txtOneDayAfterPeriod" CssClass="text" Runat="server" Width="100px"></asp:textbox>
										<asp:requiredfieldvalidator id="RequiredFieldValidator6" runat="server" ErrorMessage="*" ControlToValidate="txtOneDayAfterPeriod"></asp:requiredfieldvalidator>
										<asp:rangevalidator id=RangeValidator6 runat="server" ErrorMessage="minimum value - 1 Day" ControlToValidate="txtOneDayAfterPeriod" MinimumValue="1" MaximumValue="2000000000" Type="Integer" Display="Dynamic"></asp:rangevalidator>
									</td>
								</tr>
								<tr>
									<td width="180"></td>
									<td width="300" style="padding-left:20px;"><asp:CheckBox runat="server" ID="chkSendSpamOneWeekAfter" Font-Bold="false" /></td>
									<td width="120"><asp:label id="lblOneWeekAfterPeriod" CssClass="text" Runat="server" Font-Bold="true"></asp:label></td>
									<td>
										<asp:textbox id="txtOneWeekAfterPeriod" CssClass="text" Runat="server" Width="100px"></asp:textbox>
										<asp:requiredfieldvalidator id="RequiredFieldValidator7" runat="server" ErrorMessage="*" ControlToValidate="txtOneWeekAfterPeriod"></asp:requiredfieldvalidator>
										<asp:rangevalidator id=RangeValidator7 runat="server" ErrorMessage="minimum value - 1 Day" ControlToValidate="txtOneWeekAfterPeriod" MinimumValue="1" MaximumValue="2000000000" Type="Integer" Display="Dynamic"></asp:rangevalidator>
									</td>
								</tr>
								<tr>
									<td width="180"></td>
									<td width="300" style="padding-left:20px;"><asp:CheckBox runat="server" ID="chkSendSpamOneWeekBefore" Font-Bold="false" /></td>
									<td width="120"><asp:label id="lblOneWeekBeforePeriod" CssClass="text" Runat="server" Font-Bold="true"></asp:label></td>
									<td>
										<asp:textbox id="txtOneWeekBeforePeriod" CssClass="text" Runat="server" Width="100px"></asp:textbox>
										<asp:requiredfieldvalidator id="RequiredFieldValidator8" runat="server" ErrorMessage="*" ControlToValidate="txtOneWeekBeforePeriod"></asp:requiredfieldvalidator>
										<asp:rangevalidator id=RangeValidator8 runat="server" ErrorMessage="minimum value - 1 Day" ControlToValidate="txtOneWeekBeforePeriod" MinimumValue="1" MaximumValue="2000000000" Type="Integer" Display="Dynamic"></asp:rangevalidator>
									</td>
								</tr>
								<tr>
									<td width="180"></td>
									<td width="300" style="padding-left:20px;"><asp:CheckBox runat="server" ID="chkSendSpamOneDayBefore" Font-Bold="false" /></td>
									<td width="120"><asp:label id="lblOneDayBeforePeriod" CssClass="text" Runat="server" Font-Bold="true"></asp:label></td>
									<td>
										<asp:textbox id="txtOneDayBeforePeriod" CssClass="text" Runat="server" Width="100px"></asp:textbox>
										<asp:requiredfieldvalidator id="RequiredFieldValidator9" runat="server" ErrorMessage="*" ControlToValidate="txtOneDayBeforePeriod"></asp:requiredfieldvalidator>
										<asp:rangevalidator id=RangeValidator9 runat="server" ErrorMessage="minimum value - 1 Day" ControlToValidate="txtOneDayBeforePeriod" MinimumValue="1" MaximumValue="2000000000" Type="Integer" Display="Dynamic"></asp:rangevalidator>
									</td>
								</tr>
								<tr>
									<td width="180"></td>
									<td colspan="3"><asp:CheckBox runat="server" ID="SendBillableSpamCheckBox" Font-Bold="false" /></td>
								</tr>
								<tr>
									<td width="180"></td>
									<td colspan="3" style="padding-left:20px;"><asp:CheckBox runat="server" ID="SendBillableSpam7dayCheckBox" Font-Bold="false" /></td>
								</tr>
								<tr>
									<td width="180"></td>
									<td colspan="3" style="padding-left:20px;"><asp:CheckBox runat="server" ID="SendBillableSpam3dayCheckBox" Font-Bold="false" /></td>
								</tr>
								<tr>
									<td width="180"></td>
									<td colspan="3" style="padding-left:20px;"><asp:CheckBox runat="server" ID="SendBillableSpam1dayCheckBox" Font-Bold="false" /></td>
								</tr>
								<tr>
									<td width="180"></td>
									<td colspan="3" style="padding-left:20px;"><asp:CheckBox runat="server" ID="SendBillableSpamNegativeBalanceCheckBox" Font-Bold="false" /></td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>&nbsp;</td>
				</tr>
			</table>
			<table class="text" cellspacing="4" cellpadding="4" width="100%" border="0">
				<tr>
					<td width="800" align=right>
						<asp:button id="btnSave" CssClass="text" Runat="server" Width="80px" onclick="btnSave_Click"></asp:button>&nbsp;&nbsp;
						<input class="text" id=btnCancel style="WIDTH: 80px" onclick=window.parent.history.back() type=button value='<%=LocRM.GetString("Cancel")%>' name=btnCancel>
					</td>
					<td>&nbsp;</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
