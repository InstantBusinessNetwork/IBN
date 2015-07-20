<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.CompanyInfo" CodeBehind="CompanyInfo.ascx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" Src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="FormHelper" Src="~/Modules/FormHelper.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>

<script type="text/javascript">
	//<![CDATA[
	function clogochange(strFile) {
		if (strFile != "") {
			document.forms[0].img_clogo.src = "file:///" + strFile;
		}
	}
	//]]>
</script>

<table cellspacing="0" cellpadding="0" border="0" class="ibn-stylebox2 text" width="100%">
	<tr>
		<td style="padding: 0px">
			<ibn:BlockHeader ID="secHeader" runat="server" Title="" />
		</td>
	</tr>
	<tr>
		<td style="padding:5px">
			<ibn:BlockHeaderLight runat="server" ID="bhl1" />
			<table class="ibn-stylebox-light text" border="0" cellpadding="5px" cellspacing="0" width="100%">
				<tr>
					<td>
						<table cellspacing="0" cellpadding="5px" width="100%" border="0" class="text">
							<tr>
								<td>
									<b><%=LocRM.GetString("Logo") %>:</b>
								</td>
								<td>
									<div><img alt="" border="1px" id="img_clogo" src='<%=Mediachase.UI.Web.Util.CommonHelper.GetCompanyLogoUrl(Page)%>' name="img_clogo" /></div>
									<cc1:McHtmlInputFile onkeypress="clogochange(this.value);" ID="clogo" Size="50" onpropertychange="clogochange(this.value)" onclick="clogochange(this.value);" runat="server" CssClass="text" />
									<div id="resetLogoBlock" runat="server" visible="false"><asp:CheckBox ID="checkboxResetLogo" runat="server" /></div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr valign="top">
		<td style="padding: 0 5px 5px 5px">
			<ibn:BlockHeaderLight runat="server" ID="bhl3" />
			<table class="ibn-stylebox-light text" border="0" cellpadding="5px" cellspacing="0" width="100%">
				<tr>
					<td>
						<table cellspacing="0" cellpadding="0" width="100%" border="0" class="text">
							<tr>
								<td style="padding: 7px 3px 0 7px">
									<b><%=LocRM.GetString("SupportPerson") %>:</b>
								</td>
							</tr>
							<tr>
								<td style="padding: 4px 3px 3px 18px">
									<asp:TextBox ID="txtSupportName" CssClass="ibn-input" MaxLength="100" Width="300" runat="server"></asp:TextBox>
									<ibn:FormHelper ID="FormHelper2" runat="server" ResKey="CompanyInfo_SupportInfoHint" Width="350px" Position="BR" />
								</td>
							</tr>
							<tr>
								<td style="padding: 7px 3px 0 7px">
									<b><%=LocRM.GetString("SupportEmail") %>:</b>
								</td>
							</tr>
							<tr>
								<td style="padding: 4px 3px 3px 18px">
									<asp:TextBox ID="txtSupportEmail" CssClass="ibn-input" MaxLength="100" Width="300px" runat="server"></asp:TextBox>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<div align="center" style="padding: 15px">
				<btn:IMButton ID="btnSave" runat="server" class="text" style="width: 110px;" OnServerClick="btnSave_ServerClick"></btn:IMButton>
				&nbsp;&nbsp;
				<btn:IMButton ID="btnCancel" CausesValidation="false" runat="server" class="text" Text='' IsDecline="true" style="width: 110px;" OnServerClick="btnCancel_Click"></btn:IMButton>
			</div>
		</td>
	</tr>
</table>
