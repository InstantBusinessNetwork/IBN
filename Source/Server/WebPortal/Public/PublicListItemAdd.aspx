<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublicListItemAdd.aspx.cs" Inherits="Mediachase.UI.Web.Public.PublicListItemAdd" %>
<%@ Reference Control="~/Apps/MetaUI/MetaForm/FormDocumentView.ascx" %>
<%@ Register TagPrefix="ibn" TagName="FormDocumentView" Src="~/Apps/MetaUI/MetaForm/FormDocumentView.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<asp:ScriptManager ID="sm1" runat="server" EnableScriptGlobalization="true" EnableScriptLocalization="true">
	</asp:ScriptManager>
	<div id="divAdding" runat="server">
		<ibn:FormDocumentView ID="frmView" runat="server" />
		<div style="text-align: center;">
			<asp:Button runat="server" ID="btnSave" OnClick="btnSave_Click" />
		</div>
	</div>
	<div id="divResults" runat="server" style="text-align: center; padding: 20px;">
		<asp:Label ID="lblResults" runat="server"></asp:Label>
		<br />
		<br />
		<asp:Button ID="btnReturn" runat="server" OnClick="btnReturn_Click" />
	</div>
	</form>
</body>
</html>
