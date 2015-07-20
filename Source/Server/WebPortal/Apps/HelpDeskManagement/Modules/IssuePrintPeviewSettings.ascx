<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IssuePrintPeviewSettings.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules.IssuePrintPeviewSettings" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<style type="text/css">
	.pad5 tr td {
		padding:5px;
	}
</style>
<div style="padding: 5px;">
<ibn:BlockHeaderLight2 HeaderCssClass="ibn-toolbar-light" ID="showHeader" runat="server" />
<table width="100%" class="ibn-stylebox-light ibn-propertysheet pad5">
	<tr>
		<td>
			<asp:CheckBox ID="showTitle" runat="server" Checked="true" />
		</td>
		<td>
			<asp:CheckBox ID="showDescription" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox ID="showCode" runat="server" />
		</td>
		<td>
			<asp:CheckBox ID="showIssBox" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox ID="showPriority" runat="server" Checked="true" />
		</td>
		<td>
			<asp:CheckBox ID="showStatus" runat="server" Checked="true" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox ID="showResponsible" runat="server" Checked="true" />
		</td>
		<td>
			<asp:CheckBox ID="showManager" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox ID="showGenCats" runat="server" />
		</td>
		<td>
			<asp:CheckBox ID="showIssCats" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox ID="showClient" runat="server" Checked="true" />
		</td>
		<td>
			<asp:CheckBox ID="showOpenDate" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox ID="showCreationInfo" runat="server" />
		</td>
		<td>
			<asp:CheckBox ID="showLastModifiedInfo" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox ID="showExpectedDate" runat="server" Checked="true" />
		</td>
		<td>
			<asp:CheckBox ID="showResolveDate" runat="server" Checked="true" />
		</td>
	</tr>
</table>
<br />
<ibn:BlockHeaderLight2 HeaderCssClass="ibn-toolbar-light" ID="showForum" runat="server" />
<table width="100%" class="ibn-stylebox-light ibn-propertysheet pad5">
	<tr>
		<td>
			<asp:RadioButtonList ID="rbList" runat="server" CellPadding="6" RepeatDirection="Vertical">
			</asp:RadioButtonList>
		</td>
	</tr>
</table>
</div>
<div style="padding: 15px;text-align:center;">
	<ibn:IMButton ID="btnSave" runat="server" style="width:110px;"></ibn:IMButton>&nbsp;
	<ibn:IMButton ID="btnCancel" runat="server" style="width:110px;"></ibn:IMButton>
</div>