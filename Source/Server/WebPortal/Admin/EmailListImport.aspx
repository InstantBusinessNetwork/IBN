<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Admin.EmailListImport" CodeBehind="EmailListImport.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title><%=LocRM.GetString("tImportList")%></title>
</head>
<body>
	<form method="post" runat="server" id="form1">
	<table class="ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td>
				<ibn:BlockHeader ID="secHeader" runat="server" />
			</td>
		</tr>
		<tr>
			<td style="padding: 5px;" valign="top">
				<b>
					<%=LocRM.GetString("tFile")%>:</b>&nbsp;&nbsp;&nbsp;<cc1:McHtmlInputFile CssClass="text" Size="40" Width="280px" ID="mcImportFile" runat="server"></cc1:McHtmlInputFile>
			</td>
		</tr>
		<tr style="height: 50px;" valign="bottom">
			<td align="right">
				<mc:IMButton class="text" ID="btnSave" runat="server" style="width: 140px;">
				</mc:IMButton>
				&nbsp;&nbsp;
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
