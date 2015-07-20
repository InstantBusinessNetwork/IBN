<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropDownBoolean.Manage.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Primitives.DropDownBoolean_Manage" %>
<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, YesText%>" />:
		</td>
		<td>
			<asp:TextBox Runat="server" ID="txtYesText" Width="100%" MaxLength="100" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, Yes%>"></asp:TextBox>
		</td>
		<td width="20px">
			<asp:RequiredFieldValidator runat="server" ID="rfvYesText" ControlToValidate="txtYesText" Display="dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, NoText%>" />:
		</td>
		<td>
			<asp:TextBox Runat="server" ID="txtNoText" Width="100%" MaxLength="100" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, No%>"></asp:TextBox>
		</td>
		<td width="20px">
			<asp:RequiredFieldValidator runat="server" ID="rfvNoText" ControlToValidate="txtNoText" Display="dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal3" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, DefaultValue%>" />:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="ddlDefaultValue" Width="150px"></asp:DropDownList>
		</td>
		<td></td>
	</tr>
</table>