<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.UserFullInfo" CodeBehind="UserFullInfo.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<div style="margin-top: 5px">
	<ibn:BlockHeader ID="tbEdit" runat="server"></ibn:BlockHeader>
</div>
<table class="ibn-stylebox-light" cellspacing="0" cellpadding="0" style="width:100%">
	<tr>
		<td style="padding-left: 7px">
			<table class="ibn-propertysheet" style="padding-left: 15px" cellspacing="0" cellpadding="5" border="0">
				<tr>
					<td style="width:170px" class="ibn-label"><asp:Label ID="lblLoginTitle" runat="server"></asp:Label></td>
					<td class="ibn-value" style="width:210px"><asp:Label ID="lblLogin" runat="server"></asp:Label></td>
					<td rowspan="10"></td>
					<td style="padding-right: 17px" valign="top" align="left" rowspan="10">
						<div align="left">
							<table id="tblPhoto" cellspacing="0" cellpadding="0" style="width:200px" align="center" border="0" runat="server">
								<tr>
									<td valign="top" align="right" style="width: 10px; font-size: 1px">
										<img alt='' height="10" src="../layouts/images/photo-cornertleft.gif" width="10" />
									</td>
									<td style="font-size:1px">
										<img alt='' height="10" src="../layouts/images/photo-bgtop.gif" width="100%" />
									</td>
									<td valign="top" align="left" style="width: 17px; font-size: 1px">
										<img alt='' height="10" src="../layouts/images/photo-cornertright.gif" width="17" />
									</td>
								</tr>
								<tr>
									<td valign="top" align="left" style="width:10px; background:url(../layouts/images/photo-bgleft.gif)">
										<img alt='' height="1" src="../layouts/images/photo-bgleft.gif" width="10" />
									</td>
									<td align="center" valign="middle" bgcolor="#ffffff">
										<div align="center"><img alt=' ' id="imgPhoto" src="" border="1" name="imgPhoto" runat="server" /></div>
									</td>
									<td valign="top" align="left" style="width:17px; background:url(../layouts/images/photo-bgright.gif)">
										<img alt='' height="1" src="../layouts/images/photo-bgright.gif" width="17" />
									</td>
								</tr>
								<tr>
									<td valign="top" align="left" style="width:10px">
										<img alt='' height="33" src="../layouts/images/photo-cornerbleft.gif" width="10" />
									</td>
									<td valign="top" align="left">
										<img alt='' height="33" src="../layouts/images/photo-bgbottom.gif" width="100%" />
									</td>
									<td valign="top" align="left" style="width:17px">
										<img alt='' height="33" src="../layouts/images/photo-cornerbright.gif" width="17" />
									</td>
								</tr>
							</table>
						</div>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label runat="server" ID="lbWindowsLoginTitle"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:Label ID="lbWindowsLogin" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblPhoneTitle" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:Label ID="lblPhone" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblFaxTitle" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:Label ID="lblFax" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblMobileTitle" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:Label ID="lblMobile" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblCompanyTitle" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:Label ID="lblCompany" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblDepartmentTitle" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:Label ID="lblDepartment" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblJobTitleTitle" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:Label ID="lblJobTitle" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblLocationTitle" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:Label ID="lblLocation" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblIMGroupTitle" runat="server"></asp:Label>
					</td>
					<td class="ibn-value">
						<asp:Label ID="lblGroup" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblProfileTitle" runat="server"></asp:Label>
					</td>
					<td class="ibn-value">
						<asp:Label ID="lblProfile" runat="server"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
