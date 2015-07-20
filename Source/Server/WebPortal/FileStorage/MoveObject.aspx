<%@ Page Language="c#" Inherits="Mediachase.UI.Web.FileStorage.MoveObject" CodeBehind="MoveObject.aspx.cs" %>
<%@ Reference Control="~/Modules/DirectoryTreeView.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="DirTree" Src="~/Modules/DirectoryTreeView.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=LocRM.GetString("tbTitle")%></title>

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
<body class="UserBackground" id="pT_body">
	<form id="frmMain" method="post" runat="server" enctype="multipart/form-data">
	<table id="mainTable" class="ibn-stylebox-light text" style="height: 100%;" height="100%" cellspacing="0" cellpadding="2" width="100%" border="0">
		<tr>
			<td valign="top" class="text" style="padding-right: 15px; padding-left: 15px; padding-bottom: 15px; padding-top: 15px">
				<table cellspacing="0" cellpadding="0" border="0" class="ibn-propertysheet">
					<tr>
						<td class="text">
							<strong>
								<asp:Label ID="lblMoveTo" runat="server" CssClass="text"></asp:Label>:&nbsp;</strong>
						</td>
					</tr>
					<tr>
						<td>
							<ibn:DirTree ID="ctrlDirTree" runat="server" Width="400px" Height="250px" />
							<asp:Label ID="lblNotValid" runat="server" CssClass="ibn-error" Visible="False">*</asp:Label>
						</td>
					</tr>
					<tr>
						<td valign="bottom" align="right" height="40" colspan="2">
							<btn:IMButton class="text" ID="btnMove" runat="server">
							</btn:IMButton>
							&nbsp;&nbsp;
							<btn:IMButton class="text" CausesValidation="false" ID="btnCancel" runat="server" IsDecline="true">
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
