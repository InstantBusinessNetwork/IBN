<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Public.Modules.SignUp" CodeBehind="SignUp.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<script type="text/javascript">
	//<![CDATA[
	function SetTimeZone() {
		var date = new Date();
		var bias = date.getTimezoneOffset();
		var obj = document.getElementById('<%=lstTimeZone.ClientID %>');
		if (obj && obj.options) {
			for (var i = 0; i < obj.options.length; i++) {
				var item = obj.options[i];
				var str = item.value.substr(0, item.value.indexOf("_"));
				if (str == bias) {
					obj.value = item.value;
					break;
				}
			}
		}
	}
	//]]>
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:BlockHeader ID="tbSave" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<div style="width:700px;padding:10px;" class="ibn-propertysheet">
				<asp:Label ID="lblHeader" runat="server"></asp:Label>
			</div>
			<table id="tblInputForm" runat="server" class="text" style="padding: 0px 5px;" cellspacing="0"
				cellpadding="7" border="0">
				<tr>
					<td style="width: 150px; vertical-align: top">
						<asp:Label ID="lblLoginTitle" runat="server" AssociatedControlID="txtLogin" CssClass="text" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td valign="top" colspan="2">
						<div style="float: left;">
							<asp:TextBox ID="txtLogin" runat="server" Width="210" MaxLength="50" CssClass="text"></asp:TextBox>
						</div>
						<div style="float: left; width: 8px;">
							&nbsp;</div>
						<div>
							<asp:RegularExpressionValidator ID="txtRELoginValidator" runat="server" ErrorMessage="*"
								ControlToValidate="txtLogin" ValidationExpression="[\-0-9A-Za-z_.]+" Display="Dynamic"></asp:RegularExpressionValidator>
							<asp:Label ID="lblError" runat="server" CssClass="ibn-error"></asp:Label>
						</div>
						<div style="clear: both;">
							<asp:Label ID="lblWarning" runat="server" CssClass="ibn-error"></asp:Label>
						</div>
					</td>
				</tr>
				<tr>
					<td style="width:150px">
						<asp:Label ID="lblPasswordTitle" CssClass="text" runat="server" AssociatedControlID="txtPassword" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td style="width:250px; white-space: nowrap">
						<asp:TextBox ID="txtPassword" runat="server" Width="210" MaxLength="50" TextMode="Password"
							CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td style="white-space: nowrap">
						<asp:label id="lblConfirmTitle" CssClass="text" Runat="server" AssociatedControlID="txtConfirm" Font-Bold="True"></asp:label><strong>:</strong>
					</td>
					<td style="white-space: nowrap">
						<asp:TextBox ID="txtConfirm" runat="server" Width="210" MaxLength="50" TextMode="Password"
							CssClass="text"></asp:TextBox><asp:CompareValidator ID="Comparevalidator1" runat="server"
								ControlToValidate="txtPassword" ErrorMessage="*" ControlToCompare="txtConfirm"></asp:CompareValidator>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblFirstNameTitle" CssClass="text" runat="server" AssociatedControlID="txtFirstName" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td style="white-space: nowrap">
						<asp:TextBox ID="txtFirstName" runat="server" Width="210" MaxLength="50" CssClass="text"></asp:TextBox><asp:RequiredFieldValidator
							ID="txtFirstNameRFValidator" runat="server" ControlToValidate="txtFirstName"
							ErrorMessage="*"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblLastNameTitle" CssClass="text" runat="server" AssociatedControlID="txtLastName" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td style="white-space: nowrap">
						<asp:TextBox ID="txtLastName" runat="server" Width="210" MaxLength="50" CssClass="text"></asp:TextBox><asp:RequiredFieldValidator
							ID="txtLastNameRFValidator" runat="server" ControlToValidate="txtLastName" ErrorMessage="*"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblEmailTitle" CssClass="text" runat="server" AssociatedControlID="txtEmail" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td style="white-space: nowrap">
						<asp:TextBox ID="txtEmail" runat="server" Width="210" MaxLength="255" CssClass="text"></asp:TextBox><asp:RequiredFieldValidator
							ID="txtEmailRFValidator" runat="server" ControlToValidate="txtEmail" ErrorMessage="*"></asp:RequiredFieldValidator>
						<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEmail"
							ErrorMessage="Wrong Email" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
							Display="Dynamic"></asp:RegularExpressionValidator>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<asp:Label ID="lblPhotoTitle" CssClass="text" runat="server" AssociatedControlID="fPhoto" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<cc1:McHtmlInputFile ID="fPhoto" runat="server" CssClass="text"></cc1:McHtmlInputFile>
						<asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="fPhoto"
							ErrorMessage="Wrong Format" ValidationExpression="^[\w-\./:&amp;Р-пр-џ \\]+((\.[jJ][pP][gG])|(\.[jJ][pP][eE][gG])|(\.[gG][iI][fF])|(\.[pP][nN][gG])|(\.[tT][iI][fF][fF])|(\.[bB][mM][pP]))$"
							Display="Dynamic"></asp:RegularExpressionValidator>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblMobileTitle" CssClass="text" runat="server" AssociatedControlID="txtMobile" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:TextBox ID="txtMobile" runat="server" Width="210" MaxLength="255" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblPhoneTitle" CssClass="text" runat="server" AssociatedControlID="txtPhone" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:TextBox ID="txtPhone" runat="server" Width="210" MaxLength="255" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblFaxTitle" CssClass="text" runat="server" AssociatedControlID="txtFax" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:TextBox ID="txtFax" runat="server" Width="210" MaxLength="255" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblCompanyTitle" CssClass="text" runat="server" AssociatedControlID="txtCompany" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:TextBox ID="txtCompany" runat="server" Width="210" MaxLength="255" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblDepartmentTitle" CssClass="text" runat="server" AssociatedControlID="txtDepartment" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:TextBox ID="txtDepartment" runat="server" Width="210" MaxLength="255" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblJobTitleTitle" CssClass="text" runat="server" AssociatedControlID="txtJobTitle" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:TextBox ID="txtJobTitle" runat="server" Width="210" MaxLength="255" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblLocationTitle" CssClass="text" runat="server" AssociatedControlID="txtLocation" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:TextBox ID="txtLocation" runat="server" Width="210" MaxLength="255" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr id="trTimeZone" runat="server">
					<td>
						<asp:Label ID="lblTimeZoneTitle" CssClass="text" runat="server" AssociatedControlID="lstTimeZone" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:DropDownList ID="lstTimeZone" runat="server" Width="210px" CssClass="text">
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblLangTitle" CssClass="text" runat="server" AssociatedControlID="lstLang" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:DropDownList ID="lstLang" runat="server" Width="210px" CssClass="text">
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td colspan="3" style="height: 60px; text-align: right; vertical-align: middle">
						<btn:IMButton ID="btnSave" runat="server" class="text" style="width: 110px;" OnServerClick="btnSave_ServerClick">
						</btn:IMButton>
						&nbsp;&nbsp;
						<btn:IMButton ID="btnCancel" runat="server" class="text" style="width: 110px;"  OnServerClick="btnCancel_ServerClick"
							CausesValidation="false" IsDecline="true">
						</btn:IMButton>
					</td>
				</tr>
			</table>
			<div style="padding: 10px;">
				<asp:Label ID="lblReqWasSent" runat="server" ForeColor="#ff0000" CssClass="text"></asp:Label>
			</div>
		</td>
	</tr>
</table>
<asp:Button ID="bSave" runat="server" Text="Button" Style="visibility: hidden"></asp:Button>