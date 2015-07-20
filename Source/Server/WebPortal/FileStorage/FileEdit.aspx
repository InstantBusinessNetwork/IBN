<%@ Page Language="c#" Inherits="Mediachase.UI.Web.FileStorage.FileEdit" CodeBehind="FileEdit.aspx.cs" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=LocRM.GetString("tEditDoc")%></title>
</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" method="post" runat="server" enctype="multipart/form-data">
	<table class="text" cellspacing="0" cellpadding="5" width="100%" border="0" style="margin-top: 20">
		<tr>
			<td style="padding-top: 8px" align="right" class="ibn-label" width="130" valign="top">
				<%= LocRM.GetString("DocTitle")%>:
			</td>
			<td valign="top">
				<asp:TextBox ID="txtTitle" runat="server" CssClass="text" Width="300px"></asp:TextBox><br />
				<asp:RequiredFieldValidator ID="rfTitle" runat="server" Display="Dynamic" ErrorMessage="*" ControlToValidate="txtTitle"></asp:RequiredFieldValidator>
				<asp:CustomValidator ID="cvTitle" runat="server" Display="Dynamic" CssClass="text"></asp:CustomValidator>
				<asp:CustomValidator ID="cvInvalidChars" runat="server" Display="Dynamic" CssClass="text"></asp:CustomValidator>
				<asp:RegularExpressionValidator ID="regValTitle" runat="server" Display="Dynamic" CssClass="text" ErrorMessage="*" ControlToValidate="txtTitle" ValidationExpression="[^\\/:*?<>|\%\&\*]*"></asp:RegularExpressionValidator>
			</td>
		</tr>
		<tr>
			<td align="right" class="ibn-label">
				<%= LocRM.GetString("Description")%>:
			</td>
			<td>
				<asp:TextBox runat="server" ID="textDescription" Width="300px" CssClass="text"></asp:TextBox>
			</td>
		</tr>
		<tr runat="server" id="trLink">
			<td align="right" valign="top" class="ibn-label" width="130">
				<%= LocRM.GetString("Link")%>:
			</td>
			<td valign="top">
				<asp:TextBox ID="txtLink" runat="server" CssClass="text" Width="300px"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td>
			</td>
			<td>
				<asp:CheckBox ID="cbKeepHistory" runat="server" CssClass="ibn-value"></asp:CheckBox>
			</td>
		</tr>
		<tr valign="bottom">
			<td colspan="2" align="right" style="padding: 0px 35px 30px 0px;">
				<btn:IMButton class="text" ID="btnSave" runat="server" style="width: 115px">
				</btn:IMButton>
				<btn:IMButton class="text" ID="btnCancel" runat="server" IsDecline="true" CausesValidation="false" style="width: 110px; margin-left: 10">
				</btn:IMButton>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
