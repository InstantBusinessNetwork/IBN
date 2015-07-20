<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BackReference.Manage.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Primitives.BackReference_Manage" %>
<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, ReferencedClass%>" />:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="ddlClass" Width="100%" AutoPostBack="false"></asp:DropDownList>
		</td>
		<td width="20px"></td>
	</tr>
</table>