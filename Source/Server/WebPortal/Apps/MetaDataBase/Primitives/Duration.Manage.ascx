<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Duration.Manage.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Primitives.Duration_Manage" %>
<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal3" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, DefaultValue%>" />:
		</td>
		<td>
			<asp:TextBox Runat="server" ID="txtDefaultValue" Width="150px" MaxLength="11" Text="0"></asp:TextBox>
			<asp:CompareValidator ID="cvDefaultValue" runat="server" ErrorMessage="*" ControlToValidate="txtDefaultValue" ValueToCompare="0" Display="dynamic" Type="Integer" Operator="GreaterThanEqual"></asp:CompareValidator>
		</td>
	</tr>
</table>