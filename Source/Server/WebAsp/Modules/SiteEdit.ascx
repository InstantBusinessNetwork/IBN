<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.SiteEdit" Codebehind="SiteEdit.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="BlockHeader.ascx" %>
<script type="text/javascript" src="../Scripts/cal.js"></script>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px;">
	<tr>
		<td><ibn:blockheader id="secH" title="" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table class="text" id="BigTable" border="0" cellpadding="0" cellspacing="7">
				<tbody>
					<tr>
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("Company")%>:</strong></td>
						<td><asp:textbox id="txtCompanyName" cssclass="text" Width="300px" Runat="server"></asp:textbox><asp:requiredfieldvalidator id="Requiredfieldvalidator1" runat="server" ControlToValidate="txtCompanyName" ErrorMessage="*"></asp:requiredfieldvalidator></td>
					</tr>
					<tr>
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("Type")%>:</strong></td>
						<td>
							<asp:DropDownList runat="server" ID="TypeList" Width="300" AutoPostBack="true" onselectedindexchanged="TypeList_SelectedIndexChanged"></asp:DropDownList>
						</td>
					</tr>
					<tr style="padding-top:15px;">
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("Scheme")%>:</strong></td>
						<td>
							<asp:DropDownList runat="server" ID="SchemeList" Width="300"></asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("Domain")%>:</strong></td>
						<td>
							<asp:TextBox id="txtDomain" Width="300" Runat="server" cssClass="text"></asp:TextBox>
							<asp:RequiredFieldValidator runat="server" ID="txtDomainRfValidator" ControlToValidate="txtDomain" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("Port")%>:</strong></td>
						<td>
							<asp:TextBox id="PortValue" Width="300" Runat="server" cssClass="text"></asp:TextBox>
							<asp:RangeValidator runat="server" ID="PortValueRangeValidator" ControlToValidate="PortValue" ErrorMessage="*" Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="65535"></asp:RangeValidator>
						</td>
					</tr>
					<tr>
						<td style="WIDTH: 176px"></td>
						<td><asp:checkbox id=IsActive runat="server" CssClass="text"></asp:checkbox></td>
					</tr>

					
					<tr style="padding-top:15px;">
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("ContactName")%>:</strong></td>
						<td><asp:textbox id="txtContactName" cssclass="text" Width="300px" Runat="server"></asp:textbox></td>
					</tr>
					<tr>
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("ContactPhone")%>:</strong></td>
						<td><asp:textbox id="txtContactPhone" cssclass="text" Width="300px" Runat="server"></asp:textbox></td>
					</tr>
					<tr>
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("ContactEmail")%>:</strong></td>
						<td>
							<asp:textbox id="txtContactEmail" cssclass="text" Width="300px" Runat="server"></asp:textbox>
							<asp:RegularExpressionValidator id="RegularExpressionValidator1" runat="server" ControlToValidate="txtContactEmail" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
						</td>
					</tr>
					<tr>
						<td style="WIDTH: 176px"></td>
						<td>
							<asp:checkbox id=SendSpamCheckBox runat="server" CssClass="text"></asp:checkbox>
						</td>
					</tr>
					
					<tr runat="server" id="TariffRow" style="padding-top:15px;">
						<td class="text" style="width:176px;"><b><%=LocRM.GetString("Tariff")%>:</b></td>
						<td>
							<asp:DropDownList runat="server" ID="TariffList" Width="300" AutoPostBack="true" onselectedindexchanged="TariffList_SelectedIndexChanged"></asp:DropDownList>
						</td>
					</tr>
					<tr runat="server" id="BalanceRow">
						<td class="text" style="width:176px;"><b><%=LocRM.GetString("Balance")%>:</b></td>
						<td>
							<asp:textbox id="BalanceValue" cssclass="text" Width="300px" Runat="server"></asp:textbox>
							<asp:requiredfieldvalidator id="Requiredfieldvalidator4" runat="server" ControlToValidate="BalanceValue" ErrorMessage="*"></asp:requiredfieldvalidator>
							<asp:RangeValidator runat="server" ID="RangeValidator3" ControlToValidate="BalanceValue" ErrorMessage="*" MinimumValue="-100000" MaximumValue="10000000" Type="Currency"></asp:RangeValidator>
						</td>
					</tr>
					<tr runat="server" id="AlertThresholdRow">
						<td class="text" style="width:176px;"><b><%=LocRM.GetString("AlertThreshold")%>:</b></td>
						<td>
							<asp:textbox id="AlertThresholdValue" cssclass="text" Width="300px" Runat="server"></asp:textbox>
							<asp:requiredfieldvalidator id="Requiredfieldvalidator8" runat="server" ControlToValidate="AlertThresholdValue" ErrorMessage="*"></asp:requiredfieldvalidator>
							<asp:RangeValidator runat="server" ID="RangeValidator6" ControlToValidate="AlertThresholdValue" ErrorMessage="*" MinimumValue="-100000" MaximumValue="10000000" Type="Currency"></asp:RangeValidator>
						</td>
					</tr>
					<tr runat="server" id="CreditLimitRow">
						<td class="text" style="width:176px;"><b><%=LocRM.GetString("CreditLimit")%>:</b></td>
						<td>
							<asp:textbox id="CreditLimitValue" cssclass="text" Width="300px" Runat="server"></asp:textbox>
							<asp:requiredfieldvalidator id="Requiredfieldvalidator7" runat="server" ControlToValidate="CreditLimitValue" ErrorMessage="*"></asp:requiredfieldvalidator>
							<asp:RangeValidator runat="server" ID="RangeValidator5" ControlToValidate="CreditLimitValue" ErrorMessage="*" MinimumValue="0" MaximumValue="100000" Type="Currency"></asp:RangeValidator>
						</td>
					</tr>
					<tr runat="server" id="DiscountRow">
						<td class="text" style="width:176px;"><b><%=LocRM.GetString("Discount")%>:</b></td>
						<td>
							<asp:textbox id="DiscountValue" cssclass="text" Width="300px" Runat="server"></asp:textbox>
							<asp:requiredfieldvalidator id="Requiredfieldvalidator5" runat="server" ControlToValidate="DiscountValue" ErrorMessage="*"></asp:requiredfieldvalidator>
							<asp:RangeValidator runat="server" ID="RangeValidator4" ControlToValidate="DiscountValue" ErrorMessage="*" MinimumValue="0" MaximumValue="100" Type="Integer"></asp:RangeValidator>
						</td>
					</tr>
					
					<tr>
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("OccupiedDiskSpace")%>:</strong></td>
						<td><asp:textbox id="txtOccupiedDiskSpace" cssclass="text" Width="300px" Runat="server" Enabled="false"></asp:textbox></td>
					</tr>
					<tr>
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("MaxDiskSpace")%>:</strong></td>
						<td>
							<asp:textbox id="MaxDiskSpaceValue" cssclass="text" Width="300px" Runat="server"></asp:textbox>
							<asp:requiredfieldvalidator id="Requiredfieldvalidator6" runat="server" ControlToValidate="MaxDiskSpaceValue" ErrorMessage="*"></asp:requiredfieldvalidator>
							<asp:RangeValidator runat="server" ID="MaxDiskSpaceRV" ControlToValidate="MaxDiskSpaceValue" ErrorMessage="*" MinimumValue="-1" MaximumValue="100000" Type="Integer"></asp:RangeValidator>
						</td>
					</tr>
					<tr>
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("ActiveInternalUsers")%>:</strong></td>
						<td><asp:textbox id="txtActiveInternalUsers" cssclass="text" Width="300px" Runat="server" Enabled="false"></asp:textbox></td>
					</tr>
					<tr>
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("AllowedInternalUsers")%>:</strong></td>
						<td>
							<asp:textbox id="MaxUsersValue" cssclass="text" Width="300px" Runat="server"></asp:textbox>
							<asp:requiredfieldvalidator id="Requiredfieldvalidator2" runat="server" ControlToValidate="MaxUsersValue" ErrorMessage="*"></asp:requiredfieldvalidator>
							<asp:RangeValidator runat="server" ID="RangeValidator1" ControlToValidate="MaxUsersValue" ErrorMessage="*" MinimumValue="-1" MaximumValue="100000" Type="Integer"></asp:RangeValidator>
							<br />
							Use '-1' value for maximum users count allowed by license
						</td>
					</tr>
					<tr>
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("ActiveExternalUsers")%>:</strong></td>
						<td><asp:textbox id="txtActiveExternalUsers" cssclass="text" Width="300px" Runat="server" Enabled="false"></asp:textbox></td>
					</tr>
					<tr>
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("AllowedExternalUsers")%>:</strong></td>
						<td>
							<asp:textbox id="MaxExternalUsersValue" cssclass="text" Width="300px" Runat="server"></asp:textbox>
							<asp:requiredfieldvalidator id="Requiredfieldvalidator3" runat="server" ControlToValidate="MaxExternalUsersValue" ErrorMessage="*"></asp:requiredfieldvalidator>
							<asp:RangeValidator runat="server" ID="RangeValidator2" ControlToValidate="MaxExternalUsersValue" ErrorMessage="*" MinimumValue="-1" MaximumValue="100000" Type="Integer"></asp:RangeValidator>
							<br />
							Use '-1' value for maximum users count allowed by license
						</td>
					</tr>
					
					
					<tr runat="server" id="DateFromRow">
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("StartDate")%>:</strong></td>
						<td>
							<asp:textbox id="txtDateFrom" runat="server" CssClass="text" width="97"></asp:textbox><button 
								id=btnStartDate 
								style="BORDER-RIGHT: 0px; BORDER-TOP: 0px; BORDER-LEFT: 0px; WIDTH: 39px; PADDING-TOP: 0px; BORDER-BOTTOM: 0px; POSITION: relative; TOP: 0px; HEIGHT: 24px; BACKGROUND-COLOR: transparent" 
								onclick="ShowCal('<%=txtDateFrom.ClientID %>','btnStartDate');" 
								type=button><IMG height="21" src="../layouts/images/calendar.gif" width="34" border="0"></button>
								<asp:customvalidator id="cvCompareDate" runat="server" ErrorMessage="Date Error"></asp:customvalidator>
						</td>
					</tr>
					<tr runat="server" id="DateToRow">
						<td class="text" style="WIDTH: 176px"><strong><%=LocRM.GetString("TrialEndDate")%>:</strong></td>
						<td>
							<asp:textbox id="txtDateTo" runat="server" CssClass="text" width="97"></asp:textbox><button 
								id=btnEndDate 
								style="BORDER-RIGHT: 0px; BORDER-TOP: 0px; BORDER-LEFT: 0px; WIDTH: 39px; PADDING-TOP: 0px; BORDER-BOTTOM: 0px; POSITION: relative; TOP: 0px; HEIGHT: 24px; BACKGROUND-COLOR: transparent" 
								onclick="ShowCal('<%=txtDateTo.ClientID %>','btnEndDate');" 
								type=button><IMG height="21" src="../layouts/images/calendar.gif" width="34" border="0"></button>
						</td>
					</tr>
					<tr>
						<td style="WIDTH: 176px"></td>
						<td>
							<asp:CustomValidator id="cvError" runat="server" ErrorMessage="CustomValidator"></asp:CustomValidator></td>
					</tr>
					<tr>
						<td style="WIDTH: 176px"></td>
						<td><asp:button id="btnSave" runat="server" Text="Button" Width="80px" CssClass="text" onclick="OnSaveCommand"></asp:button>&nbsp;
							<asp:button id="btnCancel" runat="server" Text="Button" Width="80px" CssClass="text" CausesValidation=False onclick="btnCancel_Click"></asp:button></td>
					</tr>
				</tbody>
			</table>
		</td>
	</tr>
</table>

