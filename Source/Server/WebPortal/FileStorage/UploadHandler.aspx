<%@ Page Language="c#" Inherits="Mediachase.UI.Web.FileStorage.UploadHandler" CodeBehind="UploadHandler.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title>UploadHandler</title>
</head>
<body>
	<form id="uploadForm" method="post" runat="server" enctype="multipart/form-data">
	<div class="text">
		<cc1:McHtmlInputFile ID="McFileUp" class="text" Width="400px" Size="40" runat="server"></cc1:McHtmlInputFile>
		<br />
		<%= LocRM.GetString("Description")%><br />
		<asp:TextBox runat="server" ID="textDescription" Width="400" CssClass="text"></asp:TextBox>
		<input type="hidden" id="hidFName" runat="server" name="hidFName" />
		<input type="hidden" id="hidCFUKey" runat="server" name="hidCFUKey" />
		<input type="hidden" id="hidFFUId" runat="server" name="hidFFUId" />
	</div>
	</form>
</body>
</html>
