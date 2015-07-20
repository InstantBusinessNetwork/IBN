<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Admin.EmailIssueBoxEdit" CodeBehind="EmailIssueBoxEdit.aspx.cs" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=sTitle%></title>
</head>
<body>
	<form id="Form1" method="post" runat="server">
	<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td>
				<ibn:BlockHeader ID="secHeader" runat="server" />
			</td>
		</tr>
		<tr>
			<td style="padding: 5px">
				<table border="0" cellpadding="5" cellspacing="0">
					<tr>
						<td align="right" width="150px" style="font-weight: bold">
							<asp:Label runat="server" ID="lbIsDefault"></asp:Label>:
						</td>
						<td width="200px">
							<asp:CheckBox runat="server" ID="cbIsDefault" Text=""></asp:CheckBox>
						</td>
					</tr>
					<tr>
						<td align="right" style="font-weight: bold">
							<asp:Label runat="server" ID="lbName"></asp:Label>:
						</td>
						<td>
							<asp:TextBox runat="server" ID="tbName" Width="150px"></asp:TextBox>
							<asp:RequiredFieldValidator runat="server" ID="rfvName" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbName"></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td align="right" style="font-weight: bold">
							<asp:Label runat="server" ID="lbMask"></asp:Label>:
						</td>
						<td>
							<asp:TextBox runat="server" ID="tbMask" Width="150px"></asp:TextBox>
							<asp:RequiredFieldValidator runat="server" ID="rfvMask" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbMask"></asp:RequiredFieldValidator>
							<asp:RegularExpressionValidator runat="server" ID="revName" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbMask" ValidationExpression="\w+"></asp:RegularExpressionValidator>
						</td>
					</tr>
					<tr>
						<td>
							&nbsp;
						</td>
						<td>
							<asp:Label runat="server" ID="lblDuplicate" Visible="false" ForeColor="Red"></asp:Label>
						</td>
					</tr>
					<tr>
						<td colspan="2" valign="bottom" align="right" style="padding-right: 15px; font-weight: bold">
							<mc:IMButton runat="server" class='text' ID="imbSave" style="width: 120px">
							</mc:IMButton>
							&nbsp;
							<mc:IMButton runat="server" class='text' ID="imbCancel" onclick="javascript:window.close();" style="width: 120px" CausesValidation="false">
							</mc:IMButton>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
