<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Reference.Edit.Organization.PrimaryContactId.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Reference_Edit_Organization_PrimaryContactId" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="mc" TagName="EntityDD" Src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td>
			<mc:EntityDD ID="refObjects" ItemCount="6" runat="server" Width="100%" />
		</td>
		<td style="width:20px;">
			<asp:CustomValidator runat="server" ID="vldCustom" ErrorMessage="*" Display="dynamic" OnServerValidate="vldCustom_ServerValidate"></asp:CustomValidator>
		</td>
	</tr>
</table>