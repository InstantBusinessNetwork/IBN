<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Admin.LdapSettingsEdit" CodeBehind="LdapSettingsEdit.aspx.cs" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=sTitle%></title>

	<script type="text/javascript">
		//<![CDATA[
		function disableEnterKey() {
			try {
				if (window.event.keyCode == 13 && window.event.srcElement.type != "textarea")
					window.event.keyCode = 0;
			}
			catch (e) { }
		}
		//]]>
	</script>

</head>
<body style="background-color: #ffffff">
	<form id="frmMain" method="post" runat="server" onkeypress="disableEnterKey()" enctype="multipart/form-data">
	<asp:ScriptManager ID="sm" runat="server" EnableScriptGlobalization="true" EnableScriptLocalization="true">
	</asp:ScriptManager>
	<table border="0" cellpadding="7" cellspacing="0" width="95%" class="text">
		<tr>
			<td width="100px">
				<b>
					<%=LocRM.GetString("tTitle")%>:</b>
			</td>
			<td>
				<asp:TextBox ID="txtTitle" runat="server" Width="200px"></asp:TextBox>
				<asp:RequiredFieldValidator ID="rfTitle" runat="server" ErrorMessage="*" Display="Dynamic" ControlToValidate="txtTitle"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td>
				<b>
					<%=LocRM.GetString("tDomain")%>:</b>
			</td>
			<td>
				<asp:TextBox ID="txtDomain" runat="server" Width="200px"></asp:TextBox>
				<asp:RequiredFieldValidator ID="rfDomain" runat="server" ErrorMessage="*" Display="Dynamic" ControlToValidate="txtDomain"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td>
				<b>
					<%=LocRM.GetString("tUserName")%>:</b>
			</td>
			<td>
				<asp:TextBox ID="txtUser" runat="server" Width="200px"></asp:TextBox>
				<asp:RequiredFieldValidator ID="rfUser" runat="server" ErrorMessage="*" Display="Dynamic" ControlToValidate="txtUser"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td>
				<b>
					<%=LocRM.GetString("tPassword")%>:</b>
			</td>
			<td>
				<asp:TextBox ID="txtPass" runat="server" TextMode="Password" Width="200px"></asp:TextBox>
				<asp:RequiredFieldValidator ID="rfPass" runat="server" ErrorMessage="*" Display="Dynamic" ControlToValidate="txtPass"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td>
				<b>
					<%=LocRM.GetString("tFilter")%>:</b>
			</td>
			<td>
				<asp:TextBox ID="txtFilter" runat="server" Width="200px"></asp:TextBox>
				<asp:RequiredFieldValidator ID="rfFilter" runat="server" ErrorMessage="*" Display="Dynamic" ControlToValidate="txtFilter"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td>
				<b><%=Mediachase.UI.Web.Util.CommonHelper.ProductFamilyShort%> Key:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddIBNKey" runat="server" Width="200px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td>
				<b>LDAP Key:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddLdap" runat="server" Width="95px">
				</asp:DropDownList>
				<asp:TextBox ID="txtLdap" runat="server" Width="100px"></asp:TextBox>
				<asp:RequiredFieldValidator ID="rfLdap" runat="server" ErrorMessage="*" Display="Dynamic" ControlToValidate="txtLdap"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td>
			</td>
			<td>
				<asp:CheckBox ID="cbActivate" runat="server"></asp:CheckBox>
			</td>
		</tr>
		<tr>
			<td>
			</td>
			<td>
				<asp:CheckBox ID="cbDeactivate" runat="server"></asp:CheckBox>
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<fieldset>
					<legend id="lgdAutosync" runat="server">
						<asp:CheckBox ID="cbAutosync" AutoPostBack="True" runat="server"></asp:CheckBox>
					</legend>
					<table cellpadding="5" cellspacing="0" width="100%" border="0">
						<tr>
							<td width="100px">
								<b>
									<%=LocRM.GetString("tStart")%>:</b>
							</td>
							<td>
								<mc:Picker ID="dtcStart" runat="server" DateCssClass="text" TimeCssClass="text" DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" IsRequired="true" />
							</td>
						</tr>
						<tr>
							<td width="100px">
								<b>
									<%=LocRM.GetString("tInterval")%>:</b>
							</td>
							<td>
								<asp:TextBox ID="txtInterval" runat="server" Width="200px"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfInterval" runat="server" ErrorMessage="*" Display="Dynamic" ControlToValidate="txtInterval"></asp:RequiredFieldValidator>
								<asp:CompareValidator ID="cvInterval" runat="server" Display="Dynamic" ErrorMessage="*" ControlToValidate="txtInterval" Operator="GreaterThanEqual" Type="Integer" ValueToCompare="0"></asp:CompareValidator>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
		</tr>
		<tr valign="bottom">
			<td align="right" style="padding-right: 10px" colspan="2">
				<btn:IMButton runat="server" class='text' ID="imbSave" style="width: 110px">
				</btn:IMButton>
				&nbsp;
				<btn:IMButton runat="server" class='text' ID="imbCancel" onclick="javascript:window.close();" style="width: 110px" CausesValidation="false">
				</btn:IMButton>
			</td>
		</tr>
	</table>

	<script type="text/javascript">
		//<![CDATA[
		function ChangeLdap(obj) {
			document.getElementById('<%=txtLdap.ClientID%>').value = obj.value;
		}
		//]]>
	</script>

	</form>
</body>
</html>
