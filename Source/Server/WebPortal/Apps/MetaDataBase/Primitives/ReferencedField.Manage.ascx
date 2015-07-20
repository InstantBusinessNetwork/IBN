<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReferencedField.Manage.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Primitives.ReferencedField_Manage" %>
<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, ReferencedClass%>" />:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="ddlClass" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="ddlClass_SelectedIndexChanged"></asp:DropDownList>
		</td>
		<td width="20px"></td>
	</tr>
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, ReferencedField%>" />:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="ddlField" Width="100%"></asp:DropDownList>
		</td>
		<td width="20px"></td>
	</tr>
</table>