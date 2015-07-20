<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" Src="../../Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.MultipleUserEdit" CodeBehind="MultipleUserEdit.ascx.cs" %>

<script type="text/javascript">
//<![CDATA[
	function GroupExistence (sender,args)
	{
		if(((document.forms[0].<%=lbSecurityRoles.ClientID%> != null)&& (document.forms[0].<%=lbSecurityRoles.ClientID%>.selectedIndex>-1)) || ((document.forms[0].<%=lbSelectedGroups.ClientID%> != null)&& (document.forms[0].<%=lbSelectedGroups.ClientID%>.options.length>0)))
		{
			args.IsValid = true;
			return;
		}
		args.IsValid = false;
	}
	function SaveGroups()
	{
		var sControl=document.forms[0].<%=lbSelectedGroups.ClientID%>;
		
		var str="";
		if(sControl != null)
		{
			for(var i=0;i<sControl.options.length;i++)
			{
				str += sControl.options[i].value + ",";
			}
		}
		document.getElementById('<%=iGroups.ClientID%>').value = str;
	}
//]]>
</script>

<table class="ibn-stylebox" cellspacing="0" cellpadding="5px" style="width: 100%">
	<tr>
		<td colspan="2" style="padding: 0">
			<ibn:BlockHeader ID="tbSave" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td style="width: 50%; vertical-align: top">
			<asp:Label ID="lblError" runat="server" CssClass="ibn-error" Visible="False"></asp:Label>
			<ibn:BlockHeaderLight runat="server" ID="hdrBasicInfo"></ibn:BlockHeaderLight>
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="5" width="100%" border="0">
				<tr>
					<td width="175" style="padding-right: 0px" class="ibn-label">
						<asp:Label ID="lblUserType" CssClass="text" runat="server"></asp:Label>
					</td>
					<td>
						<asp:DropDownList ID="ddUserType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddUserType_PageChange" Width="140">
						</asp:DropDownList>
						&nbsp;
					</td>
					<td style="padding-left: 0px" align="left" width="150">
						<asp:CheckBox ID="chbIsActive" runat="server" CssClass="text" Text="Active" Checked="True"></asp:CheckBox>
					</td>
				</tr>
				<tr id="trLogin" runat="server">
					<td style="padding-right: 0px" align="left" class="ibn-label" width="175">
						<asp:Label ID="lblLoginTitle" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td colspan="2" align="left" class="ibn-value">
						<asp:TextBox ID="txtLogin" runat="server" CssClass="text" Width="210" MaxLength="50"></asp:TextBox><asp:RequiredFieldValidator ID="txtLoginRFValidator" runat="server" ControlToValidate="txtLogin" ErrorMessage="*"></asp:RequiredFieldValidator>
						<asp:Label ID="lblLogin" runat="server" CssClass="ibn-label"></asp:Label>
						<asp:RegularExpressionValidator ID="txtRELoginValidator" runat="server" ErrorMessage="*" ControlToValidate="txtLogin" ValidationExpression="^[\w-\.]+" Display="Dynamic"></asp:RegularExpressionValidator>
					</td>
				</tr>
				<tr id="trWinLogin" runat="server">
					<td style="padding-right: 0px" align="left" class="ibn-label">
						<asp:Label ID="lblWinLoginTitle" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td colspan="2" align="left" class="ibn-value">
						<asp:TextBox ID="txtWinLogin" runat="server" CssClass="text" Width="210" MaxLength="50"></asp:TextBox>
					</td>
				</tr>
				<tr id="trPassword1" runat="server">
					<td class="ibn-label">
						<asp:Label ID="lblPasswordTitle" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td colspan="2" class="ibn-value" style="white-space: nowrap">
						<asp:TextBox ID="txtPassword" runat="server" CssClass="text" Width="210" MaxLength="50" TextMode="Password"></asp:TextBox><asp:RequiredFieldValidator ID="PasswordValidator1" runat="server" ErrorMessage="*" ControlToValidate="txtPassword"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr id="trPassword2" runat="server">
					<td class="ibn-label" style="white-space: nowrap">
						<asp:Label ID="lblConfirmTitle" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td colspan="2" class="ibn-value" style="white-space: nowrap">
						<asp:TextBox ID="txtConfirm" runat="server" CssClass="text" Width="210" MaxLength="50" TextMode="Password"></asp:TextBox><asp:CompareValidator ID="Comparevalidator1" runat="server" ControlToValidate="txtPassword" ErrorMessage="*" ControlToCompare="txtConfirm"></asp:CompareValidator>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblFirstNameTitle" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td colspan="2" class="ibn-value" style="white-space: nowrap">
						<asp:TextBox ID="txtFirstName" runat="server" CssClass="text" Width="210" MaxLength="50"></asp:TextBox><asp:RequiredFieldValidator ID="txtFirstNameRFValidator" runat="server" ControlToValidate="txtFirstName" ErrorMessage="*"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblLastNameTitle" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td colspan="2" class="ibn-value" style="white-space: nowrap">
						<asp:TextBox ID="txtLastName" runat="server" CssClass="text" Width="210" MaxLength="50"></asp:TextBox><asp:RequiredFieldValidator ID="txtLastNameRFValidator" runat="server" ControlToValidate="txtLastName" ErrorMessage="*"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblEmailTitle" Width="110" CssClass="text" runat="server"></asp:Label><strong></strong>
					</td>
					<td colspan="2" class="ibn-value" style="white-space: nowrap">
						<asp:TextBox ID="txtEmail" runat="server" CssClass="text" Width="210" MaxLength="255"></asp:TextBox><asp:RequiredFieldValidator ID="txtEmailRFValidator" runat="server" ControlToValidate="txtEmail" ErrorMessage="*"></asp:RequiredFieldValidator><asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Wrong Email" ControlToValidate="txtEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:RegularExpressionValidator>
					</td>
				</tr>
			</table>
		</td>
		<td style="width: 50%; vertical-align: top">
			<ibn:BlockHeaderLight runat="server" ID="hdrPersonalInfo"></ibn:BlockHeaderLight>
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="5" width="100%" border="0">
				<tr>
					<td width="120" class="ibn-label">
						<asp:Label ID="lblPhotoTitle" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<cc1:McHtmlInputFile ID="fPhoto" runat="server" CssClass="text"></cc1:McHtmlInputFile>
						<asp:RegularExpressionValidator Display="Dynamic" ID="RegularExpressionValidator2" runat="server" ErrorMessage="Wrong Format" ControlToValidate="fPhoto" ValidationExpression="^[\w-\./:&amp;А-Яа-я \\]+((\.[jJ][pP][gG])|(\.[jJ][pP][eE][gG])|(\.[gG][iI][fF])|(\.[pP][nN][gG])|(\.[tT][iI][fF][fF])|(\.[bB][mM][pP]))$"></asp:RegularExpressionValidator>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblLocationTitle" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:TextBox ID="txtLocation" runat="server" CssClass="text" Width="210" MaxLength="255"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblPhoneTitle" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:TextBox ID="txtPhone" runat="server" CssClass="text" Width="210" MaxLength="255"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblFaxTitle" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:TextBox ID="txtFax" runat="server" CssClass="text" Width="210" MaxLength="255"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblMobileTitle" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:TextBox ID="txtMobile" runat="server" CssClass="text" Width="210" MaxLength="255"></asp:TextBox>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td colspan="2" style="width: 100%; vertical-align: top">
			<ibn:BlockHeaderLight runat="server" ID="hdrPreferencesInfo"></ibn:BlockHeaderLight>
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="5" width="100%" border="0">
				<tr>
					<td width="50%" valign="top">
						<table class="text" cellspacing="0" cellpadding="5" width="100%" border="0">
							<tr id="trLang" runat="server">
								<td width="120px" class="ibn-label">
									<asp:Label ID="lblLangTitle" CssClass="text" runat="server"></asp:Label>:
								</td>
								<td>
									<asp:DropDownList ID="lstLang" runat="server" CssClass="text" Width="210px">
									</asp:DropDownList>
								</td>
							</tr>
							<tr id="trTimeZone" runat="server">
								<td width="120px" class="ibn-label">
									<asp:Label ID="lblTimeZoneTitle" CssClass="text" runat="server"></asp:Label>:
								</td>
								<td>
									<asp:DropDownList ID="lstTimeZone" runat="server" CssClass="text" Width="210px">
									</asp:DropDownList>
								</td>
							</tr>
						</table>
					</td>
					<td width="50%" valign="top">
						<table class="text" cellspacing="0" cellpadding="5" width="100%" border="0">
							<tr>
								<td width="120px" class="ibn-label">
									<asp:Label ID="lblCompanyTitle" CssClass="text" runat="server"></asp:Label>:
								</td>
								<td>
									<asp:TextBox ID="txtCompany" runat="server" CssClass="text" Width="210" MaxLength="255"></asp:TextBox>
								</td>
							</tr>
							<tr>
								<td class="ibn-label">
									<asp:Label ID="lblJobTitleTitle" CssClass="text" runat="server"></asp:Label>:
								</td>
								<td>
									<asp:TextBox ID="txtJobTitle" runat="server" CssClass="text" Width="210" MaxLength="255"></asp:TextBox>
								</td>
							</tr>
							<tr>
								<td class="ibn-label">
									<asp:Label ID="lblDepartmentTitle" CssClass="text" runat="server"></asp:Label>:
								</td>
								<td>
									<asp:TextBox ID="txtDepartment" runat="server" CssClass="text" Width="210" MaxLength="255"></asp:TextBox>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td colspan="2" style="width: 100%; vertical-align: top">
			<ibn:BlockHeaderLight runat="server" ID="hdrGroupInfo"></ibn:BlockHeaderLight>
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="5" width="100%" border="0">
				<tr>
					<td width="50%" valign="top">
						<table class="text" cellspacing="0" cellpadding="5" border="0">
							<tr id="trSecurityRoles" runat="server">
								<td valign="top" width="120px" class="ibn-label">
									<asp:Label ID="lblSecurityRolesTitle" CssClass="boldtext" runat="server"></asp:Label>:
								</td>
								<td class="ibn-value">
									<asp:ListBox ID="lbSecurityRoles" SelectionMode="Multiple" runat="server" CssClass="text" Width="260px" Rows="6"></asp:ListBox>
									<br />
									<br />
									<%=LocRM.GetString("UseCtrl")%>
								</td>
							</tr>
							<tr id="trPGroups" runat="server">
								<td valign="top" style="padding-right: 0px" width="120" class="ibn-label">
									<asp:Label ID="lblPGroupsTitle" CssClass="text" runat="server"></asp:Label>:
								</td>
								<td style="padding-left: 0px; padding-bottom: 15px" align="left" width="600">
									<asp:DropDownList ID="ddPartnerGroups" CssClass="text" runat="server" Width="210px">
									</asp:DropDownList>
								</td>
							</tr>
						</table>
					</td>
					<td width="50%" valign="top">
						<table class="text" cellspacing="0" cellpadding="7" width="100%" border="0">
							<tr>
								<td class="ibn-label" style="height: 1px; width: 120px">
									<asp:Label ID="lblIMGroupTitle" CssClass="text" runat="server"></asp:Label>
								</td>
								<td>
									<asp:DropDownList ID="ddlIMGroup" runat="server" Width="210px">
									</asp:DropDownList>
								</td>
							</tr>
							<tr id="ProfileRow" runat="server">
								<td class="ibn-label">
									<%= LocRM.GetString("Profile") %>:
								</td>
								<td>
									<asp:DropDownList runat="server" ID="ProfileList" Width="210">
									</asp:DropDownList>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colspan="2" valign="top">
						<table class="text" cellspacing="0" cellpadding="5" border="0">
							<tr id="trGroups" runat="server">
								<td valign="top" width="120px" class="ibn-label">
									<asp:Label ID="lblGroupsTitle" CssClass="text" runat="server"></asp:Label>:
								</td>
								<td style="padding-bottom: 15px" align="left" width="600">
									<table class="text" id="tblGroups" style="height: 140px" cellpadding="0" border="0">
										<tr>
											<td valign="top" width="270px" style="white-space: nowrap; padding-right: 6px; padding-bottom: 0px">
												<asp:Label ID="lblAvailable" CssClass="text ibn-value" runat="server"></asp:Label><br />
												<asp:ListBox ID="lbAvailableGroups" runat="server" Width="260px" CssClass="text" Rows="7"></asp:ListBox>
											</td>
											<td style="padding-right: 6px; padding-left: 6px; padding-bottom: 0px">
												<p align="center">
													<asp:Button ID="btnAddOneGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text=">"></asp:Button><br />
													<asp:Button ID="btnAddAllGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text=">>"></asp:Button><br />
													<br />
													<asp:Button ID="btnRemoveOneGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text="<"></asp:Button><br />
													<asp:Button ID="btnRemoveAllGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text="<<"></asp:Button>
												</p>
											</td>
											<td valign="top" width="270px" style="padding-right: 20px; padding-left: 6px; padding-bottom: 0px">
												<asp:Label ID="lblSelected" CssClass="text ibn-value" runat="server"></asp:Label><br />
												<asp:ListBox ID="lbSelectedGroups" runat="server" Width="260px" CssClass="text" Rows="7"></asp:ListBox>
											</td>
										</tr>
										<tr>
											<td colspan="2">
												<asp:CustomValidator ID="GroupValidator" Style="vertical-align: top" runat="server" ErrorMessage="*" ClientValidationFunction="GroupExistence" Display="Dynamic"></asp:CustomValidator>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td colspan="2" valign="middle" align="right">
			<table>
				<tr>
					<td>
						<btn:IMButton class="text" ID="btnSave" style="width: 110px" runat="server" OnServerClick="btnSave_ServerClick">
						</btn:IMButton>
						&nbsp;&nbsp;
						<btn:IMButton class="text" ID="btnCancel" style="width: 110px" runat="server" CausesValidation="false" IsDecline="true" OnServerClick="btnCancel_ServerClick">
						</btn:IMButton>
					</td>
				</tr>
				<tr>
					<td>
						<asp:CheckBox Checked="False" CssClass="text" runat="server" ID="cbOneMore"></asp:CheckBox>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<div align="center">
	<input id="iGroups" style="visibility: hidden" runat="server" name="iGroups" />
	<asp:Button ID="bSave" Style="visibility: hidden" runat="server" Text="Button"></asp:Button></div>
