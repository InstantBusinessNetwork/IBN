<%@ Reference Control="~/Directory/Modules/SecureGroups.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.ExternalEdit" CodeBehind="ExternalEdit.ascx.cs" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" Src="../../Modules/BlockHeaderLightWithMenu.ascx" %>

<script type="text/javascript">
//<![CDATA[
	function GroupExistence (sender,args)
	{
		if( (document.forms[0].<%=lbSelectedGroups.ClientID%> != null)&& (document.forms[0].<%=lbSelectedGroups.ClientID%>.options.length>0))
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
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
				<tr>
					<td width="120">
						<asp:Label ID="lblFirstNameTitle" CssClass="text" runat="server" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td style="white-space: nowrap">
						<asp:TextBox ID="txtFirstName" runat="server" Width="210" MaxLength="50" CssClass="text" TabIndex="1"></asp:TextBox><asp:RequiredFieldValidator ID="txtFirstNameRFValidator" runat="server" ControlToValidate="txtFirstName" ErrorMessage="*"></asp:RequiredFieldValidator>
					</td>
					<td>
						<asp:CheckBox ID="chbIsActive" runat="server" Font-Bold="True" CssClass="text" Text="Active" Checked="True"></asp:CheckBox>
					</td>
				</tr>
				<tr>
					<td width="120">
						<asp:Label ID="lblLastNameTitle" CssClass="text" runat="server" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td colspan="2" style="white-space: nowrap">
						<asp:TextBox ID="txtLastName" runat="server" Width="210" MaxLength="50" CssClass="text" TabIndex="2"></asp:TextBox><asp:RequiredFieldValidator ID="txtLastNameRFValidator" runat="server" ControlToValidate="txtLastName" ErrorMessage="*"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td width="120">
						<asp:Label ID="lblEmailTitle" CssClass="text" runat="server" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td colspan="2" style="white-space: nowrap">
						<asp:TextBox ID="txtEmail" runat="server" Width="210" MaxLength="255" CssClass="text" TabIndex="3"></asp:TextBox><asp:RequiredFieldValidator ID="txtEmailRFValidator" runat="server" ControlToValidate="txtEmail" ErrorMessage="*"></asp:RequiredFieldValidator>
						<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEmail" ErrorMessage="Wrong Email" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:RegularExpressionValidator>
					</td>
				</tr>
			</table>
		</td>
		<td style="width: 50%; vertical-align: top">
			<ibn:BlockHeaderLight runat="server" ID="hdrPersonalInfo"></ibn:BlockHeaderLight>
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
				<tr>
					<td width="120">
						<asp:Label ID="lblLocationTitle" CssClass="text" runat="server" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:TextBox ID="txtLocation" runat="server" Width="210" MaxLength="255" CssClass="text" TabIndex="12"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblPhoneTitle" CssClass="text" runat="server" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:TextBox ID="txtPhone" runat="server" Width="210" MaxLength="255" CssClass="text" TabIndex="5"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblFaxTitle" CssClass="text" runat="server" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:TextBox ID="txtFax" runat="server" Width="210" MaxLength="255" CssClass="text" TabIndex="6"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblMobileTitle" CssClass="text" runat="server" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td>
						<asp:TextBox ID="txtMobile" runat="server" Width="210" MaxLength="255" CssClass="text" TabIndex="7"></asp:TextBox>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td colspan="2" style="width: 100%; vertical-align: top">
			<ibn:BlockHeaderLight runat="server" ID="hdrAdditionalInfo"></ibn:BlockHeaderLight>
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="5" width="100%" border="0">
				<tr>
					<td width="50%" valign="top">
						<table class="text" cellspacing="0" cellpadding="7" width="100%" border="0">
							<tr>
								<td width="120px">
									<asp:Label ID="lblCompanyTitle" CssClass="text" runat="server" Font-Bold="True"></asp:Label><strong>:</strong>
								</td>
								<td>
									<asp:TextBox ID="txtCompany" runat="server" Width="210" MaxLength="255" CssClass="text" TabIndex="8"></asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									<asp:Label ID="lblJobTitleTitle" CssClass="text" runat="server" Font-Bold="True"></asp:Label><strong>:</strong>
								</td>
								<td>
									<asp:TextBox ID="txtJobTitle" runat="server" Width="210" MaxLength="255" CssClass="text" TabIndex="9"></asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									<asp:Label ID="lblDepartmentTitle" CssClass="text" runat="server" Font-Bold="True"></asp:Label><strong>:</strong>
								</td>
								<td>
									<asp:TextBox ID="txtDepartment" runat="server" Width="210" MaxLength="255" CssClass="text" TabIndex="10"></asp:TextBox>
								</td>
							</tr>
						</table>
					</td>
					<td width="50%" valign="top">
						<table class="text" cellspacing="0" cellpadding="7" width="100%" border="0">
							<tr>
								<td width="120">
									<asp:Label ID="lblPhotoTitle" CssClass="text" runat="server" Font-Bold="True"></asp:Label><strong>:</strong>
								</td>
								<td>
									<cc1:McHtmlInputFile ID="fPhoto" runat="server" CssClass="text" TabIndex="4"></cc1:McHtmlInputFile>
									<asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="fPhoto" ErrorMessage="Wrong Format" ValidationExpression="^[\w-\./:&amp;Р-пр-џ \\]+((\.[jJ][pP][gG])|(\.[jJ][pP][eE][gG])|(\.[gG][iI][fF])|(\.[pP][nN][gG])|(\.[tT][iI][fF][fF])|(\.[bB][mM][pP]))$"></asp:RegularExpressionValidator>
								</td>
							</tr>
							<tr>
								<td>
								</td>
								<td style="padding-right: 17px" align="left">
									<div id="Picture" align="left" runat="server" visible="false">
										<table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
											<tr>
												<td valign="top" align="right" width="10">
													<img height="10" src="../layouts/images/photo-cornertleft.gif" width="10">
												</td>
												<td>
													<img height="10" src="../layouts/images/photo-bgtop.gif" width="100%">
												</td>
												<td valign="top" align="left">
													<img height="10" src="../layouts/images/photo-cornertright.gif" width="17">
												</td>
											</tr>
											<tr>
												<td valign="top" align="left" width="10" background="../layouts/images/photo-bgleft.gif">
													<img height="1" src="../layouts/images/photo-bgleft.gif" width="10">
												</td>
												<td valign="middle" align="center">
													<div align="center">
														<img id="imgPhoto" runat="server" src="~/layouts/images/transparentpoint.gif" border="1" name="imgPhoto"></div>
												</td>
												<td valign="top" align="left">
													<img height="126" src="../layouts/images/photo-bgright.gif" width="17">
												</td>
											</tr>
											<tr>
												<td valign="top" align="left" width="10">
													<img height="33" src="../layouts/images/photo-cornerbleft.gif" width="10">
												</td>
												<td valign="top" align="left">
													<img height="33" src="../layouts/images/photo-bgbottom.gif" width="100%">
												</td>
												<td valign="top" align="left">
													<img height="33" src="../layouts/images/photo-cornerbright.gif" width="17">
												</td>
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
		<td colspan="2" style="width: 100%; vertical-align: top">
			<ibn:BlockHeaderLight runat="server" ID="hdrGroupInfo"></ibn:BlockHeaderLight>
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
				<tr id="trGroups" runat="server">
					<td valign="top" width="120px">
						<asp:Label ID="lblGroupsTitle" CssClass="text" runat="server" Font-Bold="True"></asp:Label><strong>:</strong>
					</td>
					<td style="padding-bottom: 15px" align="left" colspan="3">
						<table class="text" id="tblGroups" cellpadding="0" border="0">
							<tr>
								<td valign="top" style="width: 270px; white-space: nowrap; padding-right: 6px; padding-bottom: 6px">
									<asp:Label ID="lblAvailable" CssClass="text" runat="server"></asp:Label><br />
									<asp:ListBox ID="lbAvailableGroups" runat="server" Width="260px" CssClass="text" Rows="7"></asp:ListBox>
								</td>
								<td style="padding-right: 6px; padding-left: 6px; padding-bottom: 6px">
									<p align="center">
										<asp:Button ID="btnAddOneGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text=">"></asp:Button><br />
										<asp:Button ID="btnAddAllGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text=">>"></asp:Button><br />
										<br />
										<asp:Button ID="btnRemoveOneGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text="<"></asp:Button><br />
										<asp:Button ID="btnRemoveAllGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text="<<"></asp:Button>
									</p>
								</td>
								<td valign="top" width="270px" style="padding-right: 20px; padding-left: 6px; padding-bottom: 6px">
									<asp:Label ID="lblSelected" CssClass="text" runat="server"></asp:Label><br />
									<asp:ListBox ID="lbSelectedGroups" runat="server" Width="260px" CssClass="text" Rows="7"></asp:ListBox>
								</td>
							</tr>
							<tr>
								<td colspan="3">
									<asp:CustomValidator ID="GroupValidator" Style="vertical-align: top" runat="server" ErrorMessage="*" ClientValidationFunction="GroupExistence"></asp:CustomValidator>
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
			<btn:IMButton style="width: 110px;" class="text" ID="btnSave" runat="server" OnServerClick="btnSave_ServerClick">
			</btn:IMButton>
			&nbsp;&nbsp;
			<btn:IMButton class="text" CausesValidation="false" ID="btnCancel" style="width: 110px;" runat="server" Text="" IsDecline="true" OnServerClick="btnCancel_ServerClick">
			</btn:IMButton>
		</td>
	</tr>
</table>
<div align="center">
	<input id="iGroups" style="visibility: hidden" runat="server" name="iGroups" />
	<asp:Button ID="bSave" runat="server" Text="Button" Style="visibility: hidden"></asp:Button>
</div>
