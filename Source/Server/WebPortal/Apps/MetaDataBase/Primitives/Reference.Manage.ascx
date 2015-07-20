<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Reference.Manage.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Primitives.Reference_Manage" %>
<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td class="ibn-label" width="120px">
			<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, ReferencedClass%>" />:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="ClassList" Width="100%"></asp:DropDownList>
		</td>
		<td width="20px"></td>
	</tr>
	<tr>
		<td></td>
		<td>
			<asp:CheckBox runat="server" ID="chkUseObjectRoles" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, UseObjectRoles%>" Checked="false" />
		</td>
		<td width="20px"></td>
	</tr>
</table>