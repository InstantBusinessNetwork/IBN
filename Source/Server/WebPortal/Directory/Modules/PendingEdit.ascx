<%@ Reference Control="~/Directory/Modules/SecureGroups.ascx" %>
<%@ Reference Control="~/Directory/Modules/UserEdit.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.PendingEdit" Codebehind="PendingEdit.ascx.cs" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="tbSave" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table class="text" style="PADDING-LEFT: 15px" cellspacing="0" cellpadding="5" border="0" style="margin-top:0px;margin-left:2px">
				<tr>
					<td width="120" align="left" class="ibn-label"><asp:label id="lblLoginTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td colSpan="2"><asp:textbox id="txtLogin" runat="server" width="210" maxlength="50" CssClass="text" tabIndex=1></asp:textbox>
						<asp:regularexpressionvalidator id="txtRELoginValidator" runat="server" ValidationExpression="^[\w-\.]+" ErrorMessage="*" ControlToValidate="txtLogin"></asp:regularexpressionvalidator><asp:label id="lblLogin" runat="server" font-bold="True"></asp:label></td>
					<td><asp:Label id="lblError" runat="server" CssClass="ibn-error"></asp:Label></td>
				</tr>
				<tr>
					<td width="120" class="ibn-label"><asp:label id="lblPasswordTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td noWrap width="210"><asp:textbox id="txtPassword" runat="server" width="210" maxlength="50" textmode="Password" CssClass="text" tabIndex=2></asp:textbox></td>
					<td noWrap width="120" class="ibn-label"><nobr><asp:label id="lblConfirmTitle" CssClass="text" Runat="server"></asp:label>:</nobr></td>
					<td noWrap width="210"><asp:textbox id="txtConfirm" runat="server" width="210" maxlength="50" textmode="Password" CssClass="text" tabIndex=3></asp:textbox><asp:comparevalidator id="Comparevalidator1" runat="server" controltovalidate="txtPassword" errormessage="*" controltocompare="txtConfirm"></asp:comparevalidator></td>
				</tr>
				<tr>
					<td width="120" class="ibn-label"><asp:label id="lblFirstNameTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td noWrap width="210"><asp:textbox id="txtFirstName" runat="server" width="210" maxlength="50" CssClass="text" tabIndex=4></asp:textbox><asp:requiredfieldvalidator id="txtFirstNameRFValidator" runat="server" controltovalidate="txtFirstName" errormessage="*"></asp:requiredfieldvalidator></td>
					<td width="120" class="ibn-label"><asp:label id="lblLastNameTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td noWrap width="210"><asp:textbox id="txtLastName" runat="server" width="210" maxlength="50" CssClass="text" tabIndex=5></asp:textbox><asp:requiredfieldvalidator id="txtLastNameRFValidator" runat="server" controltovalidate="txtLastName" errormessage="*"></asp:requiredfieldvalidator></td>
				</tr>
				<tr>
					<td width="120" class="ibn-label"><asp:label id="lblEmailTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td noWrap width="210"><asp:textbox id="txtEmail" runat="server" width="210" maxlength="255" CssClass="text" tabIndex=6></asp:textbox><asp:requiredfieldvalidator id="txtEmailRFValidator" runat="server" controltovalidate="txtEmail" errormessage="*"></asp:requiredfieldvalidator>
						<asp:RegularExpressionValidator id="RegularExpressionValidator1" runat="server" ControlToValidate="txtEmail" ErrorMessage="Wrong Email" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:RegularExpressionValidator></td>
					<td style="HEIGHT: 1px"></td>
					<td></td>
				</tr>
				<tr>
					<td width="120" class="ibn-label"><asp:label id="lblPhotoTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td width="210">
						<cc1:McHtmlInputFile id="fPhoto" runat="server" CssClass="text" tabIndex=7></cc1:McHtmlInputFile>
						<asp:RegularExpressionValidator id="RegularExpressionValidator2" runat="server" ControlToValidate="fPhoto" ErrorMessage="Wrong Format" ValidationExpression="^[\w-\./:&amp;Р-пр-џ \\]+((\.[jJ][pP][gG])|(\.[jJ][pP][eE][gG])|(\.[gG][iI][fF])|(\.[pP][nN][gG])|(\.[tT][iI][fF][fF])|(\.[bB][mM][pP]))$"></asp:RegularExpressionValidator></td>
					<td rowSpan="6"></td>
					<td style="PADDING-RIGHT: 17px" align="left" rowSpan="6">
						<div id="Picture" align="left" runat="server" visible="false">
							<table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
								<tr>
									<td vAlign="top" align="right" width="10"><IMG height="10" src="../layouts/images/photo-cornertleft.gif" width="10"></td>
									<td><IMG height="10" src="../layouts/images/photo-bgtop.gif" width="100%"></td>
									<td vAlign="top" align="left"><IMG height="10" src="../layouts/images/photo-cornertright.gif" width="17"></td>
								</tr>
								<tr>
									<td vAlign="top" align="left" width="10" background="../layouts/images/photo-bgleft.gif"><IMG height="1" src="../layouts/images/photo-bgleft.gif" width="10"></td>
									<td vAlign="center"  align="center"><img id="imgPhoto" runat="server" src="~/layouts/images/transparentpoint.gif" border="1" name="imgPhoto"></td>
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
				<tr>
					<td width="120" class="ibn-label"><asp:label id="lblPhoneTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td width="210"><asp:textbox id="txtPhone" runat="server" width="210" maxlength="255" CssClass="text" tabIndex=8></asp:textbox></td>
				</tr>
				<tr>
					<td width="120" class="ibn-label"><asp:label id="lblFaxTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td width="210"><asp:textbox id="txtFax" runat="server" width="210" maxlength="255" CssClass="text" tabIndex=9></asp:textbox></td>
				</tr>
				<tr>
					<td class="ibn-label"><asp:label id="lblMobileTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td width="210"><asp:textbox id="txtMobile" runat="server" width="210" maxlength="255" CssClass="text" tabIndex=10></asp:textbox></td>
				</tr>
				<tr>
					<td class="ibn-label"><asp:label id="lblCompanyTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td><asp:textbox id="txtCompany" runat="server" width="210" maxlength="255" CssClass="text" tabIndex=11></asp:textbox></td>
				</tr>
				<tr>
					<td class="ibn-label"><asp:label id="lblJobTitleTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td><asp:textbox id="txtJobTitle" runat="server" width="210" maxlength="255" CssClass="text" tabIndex=12></asp:textbox></td>
				</tr>
				<tr>
					<td class="ibn-label"><asp:label id="lblDepartmentTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td><asp:textbox id="txtDepartment" runat="server" width="210" maxlength="255" CssClass="text" tabIndex=13></asp:textbox></td>
					<td width="120" class="ibn-label"><asp:label id="lblLocationTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td><asp:textbox id="txtLocation" runat="server" width="210" maxlength="255" CssClass="text" tabIndex=14></asp:textbox></td>
				</tr>
				<tr id="trTimeZone" runat="server">
					<td class="ibn-label"><asp:label id="lblTimeZoneTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td><asp:dropdownlist id="lstTimeZone" runat="server" width="210px" CssClass="text" tabIndex=15></asp:dropdownlist></td>
					<td class="ibn-label"><asp:label id="lblLangTitle" CssClass="text" Runat="server"></asp:label>:</td>
					<td><asp:dropdownlist id="lstLang" runat="server" width="210px" CssClass="text" tabIndex=16></asp:dropdownlist></td>
				</tr>
				<tr>
					<td vAlign="center" align="right" colSpan="4" height="60"><btn:imbutton style="width:110px;" class="text" id="btnSave" Runat="server" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" CausesValidation="false" id="btnCancel" style="width:110px;" Runat="server" Text="" IsDecline="true" onserverclick="btnCancel_ServerClick"></btn:imbutton></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<div align="center">
	<asp:Button id="bSave" runat="server" Text="Button" style="VISIBILITY:hidden"></asp:Button></div>
