<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VersionEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.DocumentManagement.Modules.VersionEdit" %>
<%@ Reference Control="~/Apps/MetaUI/MetaForm/FormDocumentView.ascx" %>
<%@ Register TagPrefix="ibn" TagName="formDocView" Src="~/Apps/MetaUI/MetaForm/FormDocumentView.ascx "%>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td valign="top">
			<div id="mainDiv" style="overflow:auto;">
				<ibn:formDocView ID="formView" runat="server" />
			</div>
		</td>
	</tr>
	<tr style="height:40px;">
		<td align="center" style="padding:5px;">
			<asp:Button runat="server" ID="btnSave" OnClick="btnSave_Click" Width="80px" />&nbsp;&nbsp;
			<asp:Button runat="server" ID="btnCancel" Width="80px" />
		</td>
	</tr>
</table>