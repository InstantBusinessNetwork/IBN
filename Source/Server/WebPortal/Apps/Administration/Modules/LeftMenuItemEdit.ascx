<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeftMenuItemEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Administration.Modules.LeftMenuItemEdit" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="mc" Src="~/Apps/Common/Modules/ResourceEditor.ascx" TagName="ResourceEditor" %>
<table cellpadding="0" cellspacing="7" width="100%" style="table-layout: fixed;" border="0">
	<tr>
		<td class="ibn-label" valign="top">
			<asp:Literal ID="Literal2" runat="server" Text="<%$ Resources: IbnFramework.GlobalMetaInfo, DisplayText %>"></asp:Literal>:
		</td>
		<td style="width:220px;">
			<mc:ResourceEditor runat="server" ID="ctrlTitleText" WebServiceUrl="~/Apps/Common/WebServices/WsResourceEditor.asmx" CheckValueTimeout="1000" CloseTextBoxTimeout="0" Width="200" CssClassLabel="labelClass" CssClassSuccess="successLabelBlock" CssClassFailed="failedLabelBlock"/>
		</td>
	</tr>
	<tr>
		<td class="ibn-label">
			<asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: IbnFramework.GlobalMetaInfo, MenuItemLink %>"></asp:Literal>:
		</td>
		<td>
			<asp:TextBox runat="server" ID="ItemUrl" Width="200"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td class="ibn-label">
			<asp:Literal ID="Literal3" runat="server" Text="<%$ Resources: IbnFramework.GlobalMetaInfo, DisplayOrder %>"></asp:Literal>:
		</td>
		<td>
			<asp:TextBox runat="server" ID="ItemOrder" Width="200" Text="10000"></asp:TextBox>
			<asp:RequiredFieldValidator runat="server" ID="ItemOrderRequiredValidator" ControlToValidate="ItemOrder" Display="dynamic" ErrorMessage="*" Font-Bold="true">
			</asp:RequiredFieldValidator><asp:RangeValidator runat="server" ID="ItemOrderRangeValidator" ControlToValidate="ItemOrder" CultureInvariantValues="true" Type="Integer" MinimumValue="0" MaximumValue="999999" ErrorMessage="*" Display="dynamic" Font-Bold="true"></asp:RangeValidator>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="center">
			<br />
			<ibn:IMButton ID="SaveButton" runat="server" class="text" Text="<%$ Resources: IbnFramework.Common, btnOk %>" OnServerClick="SaveButton_ServerClick" style="width:100px"/>
			<br /><br />
			<ibn:IMButton ID="CancelButton" runat="server" class="text" Text="<%$ Resources: IbnFramework.Common, btnCancel %>" IsDecline="true" style="width:100px"/>
		</td>
	</tr>
</table>