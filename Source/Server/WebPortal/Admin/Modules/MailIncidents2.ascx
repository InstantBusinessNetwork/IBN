<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.MailIncidents2" Codebehind="MailIncidents2.ascx.cs" %>
<script language="javascript">
function Check()
{
		<%= Page.ClientScript.GetPostBackEventReference(btnCheck, "")%>
}
var gBtnSaveId="<%= btnSave.ClientID %>";
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
					<td vAlign="top" width="62%">
						<fieldset><legend class="text" id="lgdPopSets" runat="server"></legend>
							<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
								<tr>
									<td style="PADDING-TOP:15px"><%=LocRM.GetString("tName")%>:</td>
									<td style="PADDING-TOP:15px" width="300px">
										<asp:TextBox CssClass="text" ID="txtMBTitle" Runat=server Width="280px"></asp:TextBox>
									</td>
									<td style="PADDING-TOP:15px" width="10px">
										<asp:RequiredFieldValidator ID="rfTitle" Runat="server" ControlToValidate="txtMBTitle"
											CssClass="text" ErrorMessage="*" Display="Dynamic" />
									</td>
									<td style="PADDING-TOP:15px"></td>
								</tr>
								<tr>
									<td><asp:label id="lblServer" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
									<td><input class="text" id="txtServer" type="text" name="" runat="server" style="WIDTH:140px">&nbsp;
										<asp:Label EnableViewState="false" Runat="server" ID="lblServerError" CssClass="ibn-alerttext"></asp:Label><br>
									</td>
								</tr>
								<tr>
									<td><asp:label id="lblPort" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
									<td><input class="text" id="txtPort" type="text" name="" runat="server" style="WIDTH:50px">&nbsp;
										<asp:Label EnableViewState="false" Runat="server" ID="lblPortError" CssClass="ibn-alerttext"></asp:Label><br>
									</td>
									<td><asp:RequiredFieldValidator ID="rfPort" Runat="server" ControlToValidate="txtPort" CssClass="text" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
										<asp:CompareValidator ID="cvPort" Runat="server" ControlToValidate="txtPort" Type="Integer" Operator="DataTypeCheck" Display="Dynamic" ErrorMessage="*"></asp:CompareValidator>
									</td>
								</tr>
								<tr>
									<td><asp:label id="lblUser" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
									<td><input class="text" id="txtUser" type="text" name="" runat="server" style="WIDTH:140px">&nbsp;
										<asp:Label EnableViewState="false" Runat="server" ID="lblUserError" CssClass="ibn-alerttext"></asp:Label><br>
									</td>
								</tr>
								<tr>
									<td><asp:label id="lblPass" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
									<td><input class="text" id="txtPass" type="password" name="" runat="server" style="WIDTH:140px">&nbsp;
										<asp:Label EnableViewState="false" Runat="server" ID="lblPassError" CssClass="ibn-alerttext"></asp:Label><br>
									</td>
								</tr>
								<tr>
									<td colspan=2 align=right style="PADDING-RIGHT: 20px">
										<asp:Label ID="lblCheck" Runat="server" CssClass="ibn-alerttext" Visible="False"></asp:Label>
									</td>
									<td align="right"><asp:button id="btnCheck" runat="server" visible="False" onclick="btnCheck_Click"></asp:button>
										
										<input class="text" id="visCheck" style="WIDTH: 160px" onclick="javascript:Check()" type="button" runat="server">
									</td>
								</tr>
								
							</table>
						</fieldset>
					</td>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td vAlign="top" width="62%" style="PADDING-TOP:10px">
						<fieldset><legend class="text" id="lgdActivity" runat="server"></legend>
							<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
								<tr>
									<td colspan="2" style="PADDING-TOP:15px">
										<asp:CheckBox ID="cbUseMailIncs" AutoPostBack="false" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
									</td>
								</tr>
								<tr>
									<td width="140"><asp:label id="lblPeriod" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
									<td><select id="txtPeriod" runat="server" style="WIDTH:140px"></select></td>
								</tr>
								<tr>
									<td colspan="2">
										<asp:CheckBox ID="cbDeleteFromServer" AutoPostBack="false" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
									</td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>&nbsp;</td>
				</tr>
				<tr id="trInfo" runat="server">
					<td vAlign="top" width="62%" style="PADDING-TOP:10px">
						<fieldset><legend class="text" id="lgdInfo" runat="server"></legend>
							<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
								<tr>
									<td width="200px" style="PADDING-TOP:15px"><asp:label id="lblLastRequestTitle" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
									<td style="PADDING-TOP:15px"><asp:label id="lblLastRequest" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
								</tr>
								<tr>
									<td width="200px"><asp:label id="lblLastSuccRequestTitle" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
									<td><asp:label id="lblLastSuccRequest" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
								</tr>
								<tr>
									<td width="200px"><asp:label id="lblErrorMessageTitle" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
									<td><asp:label id="lblErrorMessage" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td vAlign="top" width="62%" style="PADDING-TOP:10px">
						<fieldset><legend class="text" id="lgdHandler" runat="server"></legend>
							<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
								<tr>
									<td width="120px" style="PADDING-TOP:15px"><asp:label id="lblHandlerTitle" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
									<td style="PADDING-TOP:15px"><asp:DropDownList id="ddHandler" AutoPostBack="True" runat="server" style="WIDTH:250px" onselectedindexchanged="ddHandler_Change" /></td>
								</tr>
								<tr>
									<td width="120px" valign="top"><asp:label id="lblHandlerDescTitle" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
									<td><asp:label id="lblHandlerDesc" style="MARGIN-BOTTOM: 5px" CssClass="text" Runat="server"></asp:label></td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>&nbsp;</td>
				</tr>
				<tr id="trHandlerSets" runat="server">
					<td vAlign="top" width="62%" style="PADDING-TOP:10px">
						<fieldset><legend class="text" id="lgdHandlerSets" runat="server"></legend>
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td width="100%" id="tdHandlerSets" runat="server"></td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td colspan="2" vAlign="top" align="right" height="60px" nowrap><br><br><btn:imbutton class="text" style="WIDTH: 120px" id="btnSave" Runat="server" Text="" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;<btn:imbutton class="text" style="WIDTH: 120px" CausesValidation="false" id="btnCancel" Runat="server" Text="" IsDecline="true" onserverclick="btnCancel_ServerClick"></btn:imbutton></td>
				</tr>
			</table>
			<!-- end body --></td>
	</tr>
</table>
