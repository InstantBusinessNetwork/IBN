<%@ Page Language="C#" AutoEventWireup="true" Inherits="Mediachase.UI.Web.Incidents.AddEMailMessageUploadHandler" CodeBehind="AddEMailMessageUploadHandler.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title></title>
</head>
<body>
	<form id="uploadForm" method="post" runat="server" enctype="multipart/form-data">
	<p>
		<%=LocRM.GetString("tHelpUpload")%></p>
	<cc1:McHtmlInputFile ID="McFileUp" runat="server" onchange="NextFile()" Size="60" style="width: 60%;"></cc1:McHtmlInputFile>
	</form>
</body>
</html>
