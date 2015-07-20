<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DecimalPercent.Manage.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Primitives.DecimalPercent_Manage" %>
<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, DecimalScale%>" />:
		</td>
		<td>
			<asp:TextBox Runat="server" ID="txtMaxPointDigits" Width="100%" MaxLength="15"></asp:TextBox>
		</td>
		<td width="20px">
			<asp:RequiredFieldValidator id="rfvMaxPointDigits" runat="server" ErrorMessage="*" ControlToValidate="txtMaxPointDigits" Display="Dynamic"></asp:RequiredFieldValidator>
			<asp:CompareValidator ID="cvMaxPointDigits" runat="server" ErrorMessage="*" ControlToValidate="txtMaxPointDigits" ValueToCompare="0" Display="dynamic" Type="Integer" Operator="GreaterThan"></asp:CompareValidator>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal3" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, DefaultValue%>" />:
		</td>
		<td>
			<asp:TextBox Runat="server" ID="txtDefaultValue" Width="100%" MaxLength="15"></asp:TextBox>
		</td>
		<td width="20px">
			<asp:CompareValidator ID="cvDefaultValueCheck" runat="server" ErrorMessage="*" ControlToValidate="txtDefaultValue" ValueToCompare="0" Display="dynamic" Type="Double" Operator="GreaterThanEqual"></asp:CompareValidator>
			<asp:CompareValidator ID="cvDefaultValueCheckMax" runat="server" ErrorMessage="*" ControlToValidate="txtDefaultValue" ValueToCompare="100" Display="dynamic" Type="Double" Operator="LessThanEqual"></asp:CompareValidator>
		</td>
	</tr>
</table>