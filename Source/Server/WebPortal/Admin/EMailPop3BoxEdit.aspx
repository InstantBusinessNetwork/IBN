<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Admin.EMailPop3BoxEdit" CodeBehind="EMailPop3BoxEdit.aspx.cs" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
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
<body>
	<form id="frmMain" method="post" runat="server" onkeypress="disableEnterKey()" enctype="multipart/form-data">
	<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td>
				<ibn:BlockHeader ID="secHeader" runat="server" />
			</td>
		</tr>
		<tr>
			<td style="padding: 5px">
				<table border="0" cellpadding="5" cellspacing="0" width="100%" class="text">
					<tr>
						<td width="80px">
							<b>
								<%=LocRM.GetString("tName")%>:</b>
						</td>
						<td>
							<asp:TextBox runat="server" ID="txtName" Width="200px"></asp:TextBox>
							<asp:RequiredFieldValidator runat="server" ID="rfName" Display="Static" ErrorMessage="*" ControlToValidate="txtName"></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td style="font-weight: bold">
							<asp:Literal runat="server" ID="ltInternalEmail"></asp:Literal>:
						</td>
						<td>
							<asp:TextBox runat="server" ID="tbInternalEmail" Width="200px"></asp:TextBox>
							<asp:RequiredFieldValidator runat="server" ID="rfvInternalEmail" Display="Dynamic" ErrorMessage="*" ControlToValidate="tbInternalEmail"></asp:RequiredFieldValidator>
							<asp:RegularExpressionValidator runat="server" ID="revInternalEmail" Display="Dynamic" ControlToValidate="tbInternalEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage="*"></asp:RegularExpressionValidator>
						</td>
					</tr>
					<tr>
						<td style="font-weight: bold">
							<asp:Literal runat="server" ID="ltServer"></asp:Literal>:
						</td>
						<td>
							<asp:TextBox runat="server" ID="tbServer" Width="200px"></asp:TextBox>
							<asp:RequiredFieldValidator runat="server" ID="rfvServer" Display="Dynamic" ErrorMessage="*" ControlToValidate="tbServer"></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td style="font-weight: bold">
							<asp:Literal runat="server" ID="ltPort"></asp:Literal>:
						</td>
						<td>
							<asp:TextBox runat="server" ID="tbPort" Width="200px" Text="110"></asp:TextBox>
							<asp:RequiredFieldValidator runat="server" ID="rfvPort" Display="Dynamic" ErrorMessage="*" ControlToValidate="tbPort"></asp:RequiredFieldValidator>
							<asp:CompareValidator runat="server" Type="Integer" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbPort" ValueToCompare="1" Operator="GreaterThanEqual"></asp:CompareValidator>
						</td>
					</tr>
					<tr>
						<td style="font-weight: bold">
							<asp:Literal runat="server" ID="ltLogin"></asp:Literal>:
						</td>
						<td>
							<asp:TextBox runat="server" ID="tbLogin" Width="200px"></asp:TextBox>
							<asp:RequiredFieldValidator runat="server" ID="rfvLogin" Display="Dynamic" ErrorMessage="*" ControlToValidate="tbLogin"></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td style="font-weight: bold">
							<asp:Literal runat="server" ID="ltPassword"></asp:Literal>:
						</td>
						<td>
							<asp:TextBox runat="server" ID="tbPassword" TextMode="Password" Width="200px"></asp:TextBox>
							<asp:RequiredFieldValidator runat="server" ID="rfvPassword" Display="Dynamic" ErrorMessage="*" ControlToValidate="tbPassword"></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr runat="server" id="trConfirmPassword">
						<td style="font-weight: bold">
							<asp:Literal runat="server" ID="ltConfirmPassword"></asp:Literal>:
						</td>
						<td>
							<asp:TextBox runat="server" ID="tbConfirmPassword" TextMode="Password" Width="200px"></asp:TextBox>
							<asp:RequiredFieldValidator runat="server" ID="rfvConfirmPassword" Display="Dynamic" ErrorMessage="*" ControlToValidate="tbConfirmPassword"></asp:RequiredFieldValidator>
							<asp:CompareValidator runat="server" Type="String" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbConfirmPassword" Operator="Equal" ID="cvConfirmPassword" ControlToCompare="tbPassword"></asp:CompareValidator>
						</td>
					</tr>
					<tr>
						<td style="font-weight: bold">
							<%=LocRM.GetString("tUseSecureConnection") %>
						</td>
						<td>
							<asp:RadioButtonList runat="Server" ID="rblSecureConnection" RepeatDirection="Horizontal">
								<asp:ListItem Value="Never"></asp:ListItem>
								<asp:ListItem Value="SSL"></asp:ListItem>
								<asp:ListItem Value="TLS"></asp:ListItem>
							</asp:RadioButtonList>
						</td>
					</tr>
					<tr>
						<td style="font-weight: bold">
							<%=LocRM.GetString("tSmtpBox")%>:
						</td>
						<td>
							<asp:DropDownList ID="ddSmtpBoxes" runat="server" Width="200px">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td width="80px">
							<asp:Label runat="server" ID="lbSettingsValid" Visible="false" ForeColor="Red"></asp:Label>
						</td>
						<td align="right" style="padding-right: 10px">
							<asp:Button runat="server" ID="btCheckSettings" Text="Check"></asp:Button>
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
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
