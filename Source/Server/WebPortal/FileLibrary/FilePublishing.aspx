<%@ Register TagPrefix="Mc" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Page Language="c#" Inherits="Mediachase.IBN.UI.Web.FileLibrary.FilePublishing" EnableViewState="false" CodeBehind="FilePublishing.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title>FilePublishing</title>
</head>
<body>
	<form method="post" runat="server" enctype="multipart/form-data">
	<input id="FileName" style="z-index: 101; left: 488px; position: absolute; top: 40px" type="text" name="Text1" runat="server">
	<input type="text" id="Login" runat="server">
	<input type="text" id="Password" runat="server">
	<input type="text" id="ObjectId" runat="server">
	<input type="text" id="ObjectTypeId" runat="server">
	<input type="text" id="AssetId" runat="server">
	<Mc:McHtmlInputFile ID="PublishedFile" runat="server" />
	<input type="button" id="btnSubmit" runat="server" onserverclick="btnSubmit_Click">
	</form>
</body>
</html>
