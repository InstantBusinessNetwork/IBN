<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignmentEdit.ascx.cs" Inherits="Mediachase.UI.Web.Apps.BusinessProcess.Modules.AssignmentEdit" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellpadding="0" cellspacing="10" width="100%" style="table-layout:fixed;">
	<tr>
		<td class="ibn-label" style="width:100px;">
			<asp:Literal runat="server" ID="SubjectLabel" Text="<%$ Resources: IbnFramework.BusinessProcess, Subject %>"></asp:Literal>:
		</td>
		<td>
			<asp:TextBox runat="server" ID="SubjectText" Width="250"></asp:TextBox>
			<asp:RequiredFieldValidator runat="server" ID="SubjectTextValidator" ControlToValidate="SubjectText" ErrorMessage="*" Font-Bold="true"></asp:RequiredFieldValidator>
		</td>
	</tr>
	<tr>
		<td class="ibn-label">
			<asp:Literal runat="server" ID="DueDate" Text="<%$ Resources: IbnFramework.BusinessProcess, DueDate %>"></asp:Literal>:
		</td>
		<td>
			<mc:Picker ID="DueDatePicker" runat="server" DateCssClass="text" TimeCssClass="text" DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" DateIsRequired="false" />
		</td>
	</tr>
	<tr>
		<td style="padding-top:100px; text-align:center;" colspan="2">
			<btn:imbutton class="text" id="SaveButton" style="width:110px;" 
				Runat="server" onserverclick="SaveButton_ServerClick" />&nbsp;&nbsp;
			<btn:imbutton class="text" id="CancelButton" style="width:110px;" 
				Runat="server" IsDecline="true" CausesValidation="false" />
		</td>
	</tr>
</table>
