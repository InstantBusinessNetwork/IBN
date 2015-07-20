<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutgoingEmailLogSettings.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Administration.Modules.OutgoingEmailLogSettings" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="text" cellspacing="0" cellpadding="10" border="0">
	<tr>
		<td>
			<%= GetGlobalResourceObject("IbnFramework.Common", "tDeliveryTimeout").ToString()%>:
		</td>
		<td>
			<asp:TextBox Runat="server" ID="txtDeliveryTimeout" Width="30px" CssClass="text"></asp:TextBox>
			<%= GetGlobalResourceObject("IbnFramework.Common", "tDays").ToString()%>.
			<asp:RequiredFieldValidator ID="rfDT" runat="server" ControlToValidate="txtDeliveryTimeout"
			Display="Dynamic" ErrorMessage="*" CssClass="text"></asp:RequiredFieldValidator>
			<asp:CompareValidator ID="rvDT" runat="server" ControlToValidate="txtDeliveryTimeout"
			Display="Dynamic" ErrorMessage="*" CssClass="text" Operator="GreaterThanEqual" Type="Integer" ValueToCompare="-1"></asp:CompareValidator>
		</td>
	</tr>
	<tr>
		<td>
			<%= GetGlobalResourceObject("IbnFramework.Common", "tDeliveryAttempts").ToString()%>:
		</td>
		<td>
			<asp:TextBox Runat="server" ID="txtAttempts" Width="30px" CssClass="text"></asp:TextBox>
			<asp:RequiredFieldValidator ID="rfTA" runat="server" ControlToValidate="txtAttempts"
			Display="Dynamic" ErrorMessage="*" CssClass="text"></asp:RequiredFieldValidator>
			<asp:CompareValidator ID="rvTA" runat="server" ControlToValidate="txtAttempts"
			Display="Dynamic" ErrorMessage="*" CssClass="text" Operator="GreaterThanEqual" Type="Integer" ValueToCompare="-1"></asp:CompareValidator>
		</td>
	</tr>
	<tr>
		<td>
			<%= GetGlobalResourceObject("IbnFramework.Common", "tLogLifeTime").ToString()%>:
		</td>
		<td>
			<asp:TextBox Runat="server" ID="txtLogPeriod" Width="30px" CssClass="text"></asp:TextBox>
			<%= GetGlobalResourceObject("IbnFramework.Common", "tDays").ToString()%>.
			<asp:RequiredFieldValidator ID="rfLP" runat="server" ControlToValidate="txtLogPeriod"
			Display="Dynamic" ErrorMessage="*" CssClass="text"></asp:RequiredFieldValidator>
			<asp:CompareValidator ID="rvLP" runat="server" ControlToValidate="txtLogPeriod"
			Display="Dynamic" ErrorMessage="*" CssClass="text" Operator="GreaterThanEqual" Type="Integer" ValueToCompare="1"></asp:CompareValidator>
		</td>
	</tr>
	<tr>
		<td align="center" style="padding-top:10px;" colspan="2">
			<btn:IMButton ID="btnSave" Runat="server" Class="text" style="width:110px;"></btn:IMButton>&nbsp;&nbsp;
			<btn:IMButton runat="server" class="text" ID="btnCancel" style="width:110px" CausesValidation="false"></btn:IMButton>
		</td>
	</tr>
</table>