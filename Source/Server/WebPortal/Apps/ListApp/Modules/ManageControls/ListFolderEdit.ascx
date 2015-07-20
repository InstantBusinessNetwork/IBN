<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListFolderEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls.ListFolderEdit" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Apps/Common/Design/BlockHeader.ascx" %>
<%@ register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="ibn-stylebox text" width="100%" cellspacing="0" cellpadding="0" border="0" style="MARGIN-TOP: 0px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td vAlign="center" height="60" style="PADDING-LEFT:15px" class="text"><b>
			<asp:Label ID="lblFolderTitle" Runat="server" CssClass="text"></asp:Label>:</b>&nbsp;
			<asp:TextBox id="tbFolderTitle" Runat="server" CssClass="text" Width="250"></asp:TextBox>
			<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" CssClass="Text" ErrorMessage=" * " ControlToValidate="tbFolderTitle"></asp:RequiredFieldValidator>
		</td>
	</tr>
	<tr>
		<td vAlign="center" align="left" height="60" style="PADDING-LEFT:150px">
			<btn:imbutton class="text" id="btnCreateSave1" style="width:110px;" Runat="server" Text="" onserverclick="Button1_Click"></btn:imbutton>&nbsp;&nbsp;
			<btn:imbutton class="text" id="btnCancel" style="width:110px;" Runat="server" Text="" IsDecline="true" CausesValidation="false" onserverclick="btnCancel_ServerClick"></btn:imbutton>
		</td>
	</tr>
</table>
