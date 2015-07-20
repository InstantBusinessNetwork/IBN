<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Reference.Manage.@.ListInfo.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Primitives.Reference_Manage_All_ListInfo" %>
<%@ register TagPrefix="lst" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td class="ibn-label" style="width:120px;">
			<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.ListInfo, BusinessObjectOrList%>" />:
		</td>
		<td>
			<lst:indenteddropdownlist runat="server" ID="ClassList" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="ClassList_SelectedIndexChanged"></lst:indenteddropdownlist>
		</td>
		<td style="width:20px;"></td>
	</tr>
	<tr>
		<td></td>
		<td>
			<asp:CheckBox runat="server" ID="chkUseObjectRoles" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, UseObjectRoles%>" Checked="false" />
		</td>
		<td style="width:20px;"></td>
	</tr>
</table>