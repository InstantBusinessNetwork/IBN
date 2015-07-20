<%@ Reference Control="~/Directory/Modules/SecureGroups.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="../../Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.PartnerEdit" Codebehind="PartnerEdit.ascx.cs" %>
<SCRIPT language="javascript">
	function SendNotification()
	{
		<%= Page.ClientScript.GetPostBackEventReference(lbSend, "")%>
	}
</SCRIPT>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px;margin-left:2px">
	<tr>
		<td colspan=2><ibn:blockheader id="tbSave" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="padding:5px" width="50%" valign=top>
			<asp:label id="lblError" runat="server" CssClass="ibn-error" Visible="False"></asp:label>
			<ibn:BlockHeaderLight runat="server" id="hdrBasicInfo"></ibn:BlockHeaderLight>
				<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
					<tr>
						<td width="120">
							<div align="left"><asp:label id="lblLoginTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG></div>
						</td>
						<td><asp:textbox id="txtLogin" runat="server" CssClass="text" width="210" maxlength="50" tabIndex=1></asp:textbox><asp:requiredfieldvalidator id="txtLoginRFValidator" runat="server" controltovalidate="txtLogin" errormessage="*"></asp:requiredfieldvalidator>
							<asp:label id="lblLogin" runat="server" font-bold="True"></asp:label>
							<asp:regularexpressionvalidator id="txtRELoginValidator" runat="server" ErrorMessage="*" ControlToValidate="txtLogin" ValidationExpression="^[\w-\.]+" Display="Dynamic"></asp:regularexpressionvalidator></td>
						<td><asp:checkbox id="chbIsActive" runat="server" CssClass="text" Font-Bold="True" Text="Active" Checked="True"></asp:checkbox></td>
					</tr>
					<tr>
						<td width="120"><asp:label id="lblPasswordTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG></td>
						<td noWrap colspan=2><asp:textbox id="txtPassword" runat="server" CssClass="text" width="210" maxlength="50" textmode="Password" tabIndex=2></asp:textbox><asp:requiredfieldvalidator Enabled="False" id="PasswordValidator1" runat="server" ErrorMessage="*" ControlToValidate="txtPassword"></asp:requiredfieldvalidator>
						</td>
					</tr>
					<tr>
						<td noWrap width="120"><nobr><asp:label id="lblConfirmTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG></nobr></td>
						<td noWrap colspan=2><asp:textbox id="txtConfirm" runat="server" CssClass="text" width="210" maxlength="50" textmode="Password" tabIndex=3></asp:textbox><asp:comparevalidator id="Comparevalidator1" runat="server" controltovalidate="txtPassword" errormessage="*" controltocompare="txtConfirm"></asp:comparevalidator></td>
					</tr>
					<tr>
						<td width="120"><asp:label id="lblFirstNameTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG>
						</td>
						<td noWrap colspan=2><asp:textbox id="txtFirstName" runat="server" CssClass="text" width="210" maxlength="50" tabIndex=4></asp:textbox><asp:requiredfieldvalidator id="txtFirstNameRFValidator" runat="server" controltovalidate="txtFirstName" errormessage="*"></asp:requiredfieldvalidator></td>
					</tr>
					<tr>
						<td width="120"><asp:label id="lblLastNameTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG></td>
						<td noWrap colspan=2><asp:textbox id="txtLastName" runat="server" CssClass="text" width="210" maxlength="50" tabIndex=5></asp:textbox><asp:requiredfieldvalidator id="txtLastNameRFValidator" runat="server" controltovalidate="txtLastName" errormessage="*"></asp:requiredfieldvalidator></td>
					</tr>
					<tr>
						<td width="120"><asp:label id="lblEmailTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG></td>
						<td noWrap colspan=2><asp:textbox id="txtEmail" runat="server" CssClass="text" width="210" maxlength="255" tabIndex=6></asp:textbox><asp:requiredfieldvalidator id="txtEmailRFValidator" runat="server" controltovalidate="txtEmail" errormessage="*"></asp:requiredfieldvalidator><asp:regularexpressionvalidator id="RegularExpressionValidator1" runat="server" ErrorMessage="Wrong Email" ControlToValidate="txtEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:regularexpressionvalidator></td>
					</tr>
				</table>
		</td>
		<td style="padding:5px" width="50%" valign=top>
			<asp:label id="Label2" runat="server" CssClass="ibn-error" Visible="False"></asp:label>	
			<ibn:BlockHeaderLight runat="server" id="hdrPersonalInfo"></ibn:BlockHeaderLight>
				<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
					<tr>
						<td width=120><asp:label id="lblLocationTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG></td>
						<td><asp:textbox id="txtLocation" runat="server" CssClass="text" width="210" maxlength="255" tabIndex=14></asp:textbox></td>
					</tr>
					<tr>
						<td><asp:label id="lblPhoneTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG></td>
						<td><asp:textbox id="txtPhone" runat="server" CssClass="text" width="210" maxlength="255" tabIndex=8></asp:textbox></td>
					</tr>
					<tr>
						<td><asp:label id="lblFaxTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG></td>
						<td><asp:textbox id="txtFax" runat="server" CssClass="text" width="210" maxlength="255" tabIndex=9></asp:textbox></td>
					</tr>
					<tr>
						<td><asp:label id="lblMobileTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG></td>
						<td><asp:textbox id="txtMobile" runat="server" CssClass="text" width="210" maxlength="255" tabIndex=10></asp:textbox></td>
					</tr>
				</table>
		</td>
	</tr>
	<tr>
		<td style="padding:0 5 5 5" width="100%" valign=top colspan=2>
			<ibn:BlockHeaderLight runat="server" id="hdrAdditionalInfo"></ibn:BlockHeaderLight>
				<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="5" width="100%" border="0">
					<tr>
						<td width="50%" valign=top>
							<table class="text" cellspacing="0" cellpadding="7" width="100%" border="0">
								<tr>
									<td width="120px"><asp:label id="lblCompanyTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG>
									</td>
									<td><asp:textbox id="txtCompany" runat="server" CssClass="text" width="210" maxlength="255" tabIndex=11></asp:textbox></td>
								</tr>
								<tr>
									<td><asp:label id="lblJobTitleTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG>
									</td>
									<td><asp:textbox id="txtJobTitle" runat="server" CssClass="text" width="210" maxlength="255" tabIndex=12></asp:textbox></td>
								</tr>
								<tr>
									<td><asp:label id="lblDepartmentTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG></td>
									<td><asp:textbox id="txtDepartment" runat="server" CssClass="text" width="210" maxlength="255" tabIndex=13></asp:textbox></td>
								</tr>
								<tr id="ProfileRow" runat="server">
									<td class="ibn-label"><%= LocRM.GetString("Profile") %>:</td>
									<td>
										<asp:DropDownList runat="server" ID="ProfileList" Width="210"></asp:DropDownList>
									</td>
								</tr>
							</table>
						</td>
						<td width="50%" valign=top>
							<table class="text" cellspacing="0" cellpadding="7" width="100%" border="0">
								<tr>
									<td width="120"><asp:label id="lblPhotoTitle" CssClass="text" Runat="server" Font-Bold="True"></asp:label><STRONG>:</STRONG></td>
									<td><cc1:mchtmlinputfile id="fPhoto" runat="server" CssClass="text" tabIndex=7></cc1:mchtmlinputfile><asp:regularexpressionvalidator id="RegularExpressionValidator2" runat="server" ErrorMessage="Wrong Format" ControlToValidate="fPhoto" ValidationExpression="^[\w-\./:&amp;Р-пр-џ \\]+((\.[jJ][pP][gG])|(\.[jJ][pP][eE][gG])|(\.[gG][iI][fF])|(\.[pP][nN][gG])|(\.[tT][iI][fF][fF])|(\.[bB][mM][pP]))$"></asp:regularexpressionvalidator></td>
								</tr>
								<tr>
									<td></td>
									<td style="PADDING-RIGHT: 17px" align="left">
										<div id="Picture" align="left" runat="server" visible="false">
											<table cellspacing="0" cellpadding="0" width="100%" align=left border=0>
												<tr>
													<td vAlign="top" align="right" width="10"><IMG height=10 src="../layouts/images/photo-cornertleft.gif" width=10></td>
													<td><IMG height="10" src="../layouts/images/photo-bgtop.gif" width="100%"></td>
													<td vAlign="top" align="left"><IMG height="10" src="../layouts/images/photo-cornertright.gif" width="17"></td>
												</tr>
												<tr>
													<td vAlign="top" align="left" width="10" background="../layouts/images/photo-bgleft.gif"><IMG height="1" src="../layouts/images/photo-bgleft.gif" width="10"></td>
													<td vAlign="center" align="middle">
														<div align="center"><IMG id="imgPhoto" src="~/layouts/images/transparentpoint.gif" border="1" name="imgPhoto" runat="server"></div>
													</td>
													<td vAlign="top" align="left"><IMG height="126" src="../layouts/images/photo-bgright.gif" width="17"></td>
												</tr>
												<tr>
													<td vAlign="top" align="left" width="10"><IMG height="33" src="../layouts/images/photo-cornerbleft.gif" width="10"></td>
													<td vAlign="top" align="left"><IMG height="33" src="../layouts/images/photo-bgbottom.gif" width="100%"></td>
													<td vAlign="top" align="left"><IMG height="33" src="../layouts/images/photo-cornerbright.gif" width="17"></td>
												</tr>
											</table>
										</div>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
		</td>
	</tr>
	<tr>
		<td vAlign="center" align="right" colSpan="2" height="60" style="padding-Right: 10px;"><btn:imbutton class="text" id="btnSave" style="width:110px;" Runat="server" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
			<btn:imbutton class="text" id="btnCancel" style="width:110px;" Runat="server" Text="" CausesValidation="false" IsDecline="true" onserverclick="btnCancel_ServerClick"></btn:imbutton></td>
	</tr>
</table>
<div align="center">&nbsp;
	<asp:button id="bSave" style="VISIBILITY: hidden" runat="server" Text="Button"></asp:button></div>
<asp:LinkButton ID="lbSend" Runat=server Visible="False" onclick="btnSend_Click"></asp:LinkButton>