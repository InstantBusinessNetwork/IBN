<%@ Page Language="c#" Inherits="Mediachase.UI.Web.FileStorage.DirectoryEdit" CodeBehind="DirectoryEdit.aspx.cs" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=(FolderId < 0) ? LocRM.GetString("tNewDirectory") : LocRM.GetString("EditFolder")%></title>

	<script type="text/javascript">
		//<![CDATA[
		function LoginFocusElement(elId) {
			var elem = document.getElementById(elId);
			if (!elem)
				return;
			elem.focus();
		}
		//]]>
	</script>

</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" method="post" runat="server" enctype="multipart/form-data">
	<table class="text" cellspacing="0" cellpadding="5" width="100%" border="0" style="margin-top: 20">
		<tr>
			<td valign="top" style="padding-top: 7px" width="140" align="right">
				<b>
					<%= LocRM.GetString("FolderTitle")%>:</b>
			</td>
			<td valign="top" class="ibn-value">
				<asp:TextBox ID="txtTitle" runat="server" CssClass="text" Width="340px"></asp:TextBox><br />
				<asp:RequiredFieldValidator ID="rfTitle" runat="server" Display="Dynamic" ErrorMessage="*" ControlToValidate="txtTitle"></asp:RequiredFieldValidator>
				<asp:CustomValidator ID="cvTitle" runat="server" Display="Static" CssClass="text"></asp:CustomValidator>
			</td>
		</tr>
		<tr>
			<td colspan="2" align="right" style="padding-right: 35px">
				<btn:IMButton class="text" ID="btnSave" runat="server">
				</btn:IMButton>
				<btn:IMButton class="text" ID="btnCancel" runat="server" IsDecline="true" CausesValidation="false">
				</btn:IMButton>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
