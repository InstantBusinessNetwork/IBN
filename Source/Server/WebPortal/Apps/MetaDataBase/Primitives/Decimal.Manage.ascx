<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Decimal.Manage.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Primitives.Decimal_Manage" %>
<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal4" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, MinimumValue%>" />:
		</td>
		<td>
			<asp:TextBox Runat="server" ID="txtMinValue" Width="100%" MaxLength="15"></asp:TextBox>
		</td>
		<td width="20px">
			<asp:RequiredFieldValidator id="rfvMinValue" runat="server" ErrorMessage="*" ControlToValidate="txtMinValue" Display="Dynamic"></asp:RequiredFieldValidator>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal5" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, MaximumValue%>" />:
		</td>
		<td>
			<asp:TextBox Runat="server" ID="txtMaxValue" Width="100%" MaxLength="15"></asp:TextBox>
		</td>
		<td width="20px">
			<asp:RequiredFieldValidator id="rfvMaxValue" runat="server" ErrorMessage="*" ControlToValidate="txtMaxValue" Display="Dynamic"></asp:RequiredFieldValidator>
			<asp:CompareValidator ID="cvMaxValue" runat="server" ErrorMessage="*" ControlToValidate="txtMaxValue" ControlToCompare="txtMinValue" Display="dynamic" Type="Double" Operator="GreaterThan"></asp:CompareValidator>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, DecimalPrecision%>" />:
		</td>
		<td>
			<asp:TextBox Runat="server" ID="txtMaxDigits" Width="100%" MaxLength="15"></asp:TextBox>
		</td>
		<td width="20px">
			<asp:RequiredFieldValidator id="rfvMaxDigits" runat="server" ErrorMessage="*" ControlToValidate="txtMaxDigits" Display="Dynamic"></asp:RequiredFieldValidator>
			<asp:CompareValidator ID="cvMaxDigits" runat="server" ErrorMessage="*" ControlToValidate="txtMaxDigits" ValueToCompare="0" Display="dynamic" Type="Integer" Operator="GreaterThan"></asp:CompareValidator>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, DecimalScale%>" />:
		</td>
		<td>
			<asp:TextBox Runat="server" ID="txtMaxPointDigits" Width="100%" MaxLength="15"></asp:TextBox>
		</td>
		<td width="20px">
			<asp:RequiredFieldValidator id="rfvMaxPointDigits" runat="server" ErrorMessage="*" ControlToValidate="txtMaxPointDigits" Display="Dynamic"></asp:RequiredFieldValidator>
			<asp:CompareValidator ID="cvMaxPointDigits" runat="server" ErrorMessage="*" ControlToValidate="txtMaxPointDigits" ValueToCompare="0" Display="dynamic" Type="Integer" Operator="GreaterThanEqual"></asp:CompareValidator>
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
			<asp:CompareValidator ID="cvDefaultValueCheck" runat="server" ErrorMessage="*" ControlToValidate="txtDefaultValue" Display="dynamic" Type="Double" Operator="DataTypeCheck"></asp:CompareValidator>
		</td>
	</tr>
</table>