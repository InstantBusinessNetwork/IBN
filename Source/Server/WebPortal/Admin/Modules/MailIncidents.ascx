<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.MailIncidents" Codebehind="MailIncidents.ascx.cs" %>
<script language="javascript">
function EnableDisable()
{
	ChColl=document.getElementsByTagName("input");
	var obj, OcbAutoApprove, OcbAutoDelete, OcbUseExternal;
	for(j=0;j<ChColl.length;j++)
	{
		obj_temp = ChColl[j];
		_obj_id = obj_temp.id;
		if(_obj_id.indexOf("cbUseMailIncs")>=0)
			obj=obj_temp;
		if(_obj_id.indexOf("cbAutoApprove")>=0)
			OcbAutoApprove=obj_temp;
		if(_obj_id.indexOf("cbAutoDelete")>=0)
			OcbAutoDelete=obj_temp;
		if(_obj_id.indexOf("cbUseExternal")>=0)
			OcbUseExternal=obj_temp;
	}

	OtxtPeriod = document.forms[0].<%=txtPeriod.ClientID %>;
	if (OtxtPeriod != null)
		OtxtPeriod.disabled = !(obj.checked);
	
	OtxtServer = document.forms[0].<%=txtServer.ClientID %>;
	if (OtxtServer != null)
		OtxtServer.disabled = !(obj.checked);
		
	OtxtPort = document.forms[0].<%=txtPort.ClientID %>;
	if (OtxtPort != null)
		OtxtPort.disabled = !(obj.checked);
		
	OtxtUser = document.forms[0].<%=txtUser.ClientID %>;
	if (OtxtUser != null)
		OtxtUser.disabled = !(obj.checked);
		
	OtxtPass = document.forms[0].<%=txtPass.ClientID %>;
	if (OtxtPass != null)
		OtxtPass.disabled = !(obj.checked);
		
	OddFolders = document.forms[0].<%=ddFolder.ClientID %>;
	if (OddFolders != null)
		OddFolders.disabled = !(obj.checked);
		
	OvisCheck = document.forms[0].<%=visCheck.ClientID %>;
	if (OvisCheck != null)
		OvisCheck.disabled = !(obj.checked);
		
	if (OcbAutoApprove != null)
		OcbAutoApprove.disabled = !(obj.checked);
		
	if (OcbAutoDelete != null)
		OcbAutoDelete.disabled = !(obj.checked);
		
	if (OcbUseExternal != null)
		OcbUseExternal.disabled = !(obj.checked);
}
function Check()
{
		<%= Page.ClientScript.GetPostBackEventReference(btnCheck, "")%>
}
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="tbMailIncs" runat="server" /></td>
	</tr>
	<tr>
		<td style="PADDING: 8 8 0 8">
			<!-- body -->
			<table class="text" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td vAlign="top">
						<fieldset><legend class="text">
								<div align="center">
									<span>
										<asp:CheckBox ID="cbUseMailIncs" AutoPostBack="false" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
									</span>
								</div>
							</legend>
							<table width="100%">
								<tr>
									<td class="text" width="95px" style="padding-left:55px"><%=LocRM.GetString("tTitle")%>:</td>
									<td>
										<asp:TextBox CssClass="text" ID="txtMBTitle" Runat=server Width="250px"></asp:TextBox>
										<asp:RequiredFieldValidator ID="rfTitle" Runat="server" ControlToValidate="txtMBTitle"
											 CssClass="text" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
									</td>
								</tr>
							</table>
							<table class="text" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr style="PADDING-TOP:15px">
									<td width="50px">
										<div align="center"></div>
									</td>
									<td width="62%">
										<fieldset><legend class="text" id="lgdPopSets" runat="server"></legend>
											<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
												<tr>
													<td style="PADDING-TOP:15px"><asp:label id="lblServer" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
													<td style="PADDING-TOP:15px" width="160px"><input class="text" id="txtServer" disabled type="text" name="" runat="server" style="WIDTH:140px">&nbsp;
														<asp:Label EnableViewState="false" Runat="server" ID="lblServerError" CssClass="ibn-alerttext"></asp:Label><br>
													</td>
													<td style="PADDING-TOP:15px" width="10px"></td>
													<td style="PADDING-TOP:15px"></td>
												</tr>
												<tr>
													<td><asp:label id="lblPort" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
													<td><input class="text" id="txtPort" disabled type="text" name="" runat="server" style="WIDTH:140px">&nbsp;
														<asp:Label EnableViewState="false" Runat="server" ID="lblPortError" CssClass="ibn-alerttext"></asp:Label><br>
													</td>
													<td><asp:RequiredFieldValidator ID="rfPort" Runat="server" ControlToValidate="txtPort" CssClass="text" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
														<asp:CompareValidator ID="cvPort" Runat="server" ControlToValidate="txtPort" Type="Integer" Operator="DataTypeCheck" Display="Dynamic" ErrorMessage="*"></asp:CompareValidator>
													</td>
												</tr>
												<tr>
													<td><asp:label id="lblUser" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
													<td><input class="text" id="txtUser" disabled type="text" name="" runat="server" style="WIDTH:140px">&nbsp;
														<asp:Label EnableViewState="false" Runat="server" ID="lblUserError" CssClass="ibn-alerttext"></asp:Label><br>
													</td>
												</tr>
												<tr>
													<td><asp:label id="lblPass" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
													<td><input class="text" id="txtPass" disabled type="password" name="" runat="server" style="WIDTH:140px">&nbsp;
														<asp:Label EnableViewState="false" Runat="server" ID="lblPassError" CssClass="ibn-alerttext"></asp:Label><br>
													</td>
												</tr>
												<tr>
													<td width="100"><asp:label id="lblPeriod" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
													<td><select id="txtPeriod" disabled name="" runat="server" style="WIDTH:140px"></select>
													</td>
												</tr>
												<tr>
													<td colspan=2 align=right style="PADDING-RIGHT: 20px">
														<asp:Label ID="lblCheck" Runat="server" CssClass="ibn-alerttext" Visible="False"></asp:Label>
													</td>
													<td align="right"><asp:button id="btnCheck" runat="server" visible="False" onclick="btnCheck_Click"></asp:button>
														
														<input class="text" id="visCheck" disabled style="WIDTH: 160px" onclick="javascript:Check()" type="button" runat="server">
													</td>
												</tr>
												
											</table>
										</fieldset>
									</td>
									<td>&nbsp;</td>
								</tr>
								<tr id="trFolder" runat=server style="PADDING-TOP:20px">
									<td>
										<div align="center"></div>
									</td>
									<td>
										<fieldset>
											<legend class="text" id="lgdFolder" runat="server"></legend>
											<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
												<tr>
													<td style="PADDING-TOP:15px" width="100px" class="text">
														<%=LocRM.GetString("tFolder")%>
													</td>
													<td style="PADDING-TOP:15px">
														<select id="ddFolder" disabled name="" runat="server" style="WIDTH:250px"></select>
														<asp:CustomValidator ID="cvFolder" CssClass="text" Runat=server Display=Dynamic></asp:CustomValidator>
													</td>
												</tr>
											</table>
										</fieldset>
									</td>
									<td>&nbsp;</td>
								</tr>
								<tr>
									<td>&nbsp;
									</td>
								</tr>
								<tr>
									<td>
										<div align="center"></div>
									</td>
									<td>
										<fieldset><legend class="text" id="lgdIncSets" runat="server"></legend>
											<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
												<tr>
													<td style="PADDING-TOP:15px">
														<asp:CheckBox ID="cbAutoApprove" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
													</td>
												</tr>
												<tr>
													<td>
														<asp:CheckBox ID="cbAutoDelete" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
													</td>
												</tr>
												<tr>
													<td>
														<asp:CheckBox ID="cbUseExternal" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
													</td>
												</tr>
											</table>
										</fieldset>
									</td>
									<td>&nbsp;</td>
								</tr>
							</table>
							<br>
						</fieldset>
					</td>
				</tr>
				<tr>
					<td vAlign="center" align="right" height="60">
						<btn:imbutton class="text" style="WIDTH: 110px" id="btnSave" Runat="server" Text="" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" style="WIDTH: 110px" CausesValidation="false" id="btnCancel" Runat="server" Text="" IsDecline="true" onserverclick="btnCancel_ServerClick"></btn:imbutton></td>
				</tr>
			</table>
			<!-- end body --></td>
	</tr>
</table>
