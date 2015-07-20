<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Projects.SaveBasePlanPopUp" CodeBehind="SaveBasePlanPopUp.aspx.cs" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title>SaveBasePlanPopUp</title>
</head>
<body>
	<form id="Form1" method="post" runat="server">
	<table width="99%" cellpadding="0" cellspacing="5" align="center">
		<tr class="text">
			<td align="center">
				<asp:DropDownList runat="server" ID="ddBasePlan" Width="340px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr class="text">
			<td align="right" style="padding: 5px; margin: 5px;">
				<btn:IMButton class="text" runat="server" ID="btnOk" style="width: 110px;">
				</btn:IMButton>
				<btn:IMButton class="text" runat="server" ID="btnCancel" style="width: 110px;">
				</btn:IMButton>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
