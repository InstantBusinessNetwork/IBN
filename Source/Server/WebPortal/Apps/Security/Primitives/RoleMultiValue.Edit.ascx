<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RoleMultiValue.Edit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Security.Primitives.RoleMultiValue_Edit" %>
<%@ Reference Control="~/Apps/Common/SelectControl/ListSelectControl.ascx" %>
<%@ Reference Control="~/Apps/Common/SelectControl/SelectPopup.ascx" %>
<%@ Register TagPrefix="mc" TagName="ListSelectControl" Src="~/Apps/Common/SelectControl/ListSelectControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="SelectPopupControl" Src="~/Apps/Common/SelectControl/SelectPopup.ascx" %>

<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
	<tr>
		<td class="ibn-label" style="width:120px" runat="server" id="LabelCell" valign="top">
			<asp:Label runat="server" ID="MainLabel"></asp:Label>
		</td>
		<td>
			<asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
				<ContentTemplate>
					<mc:ListSelectControl runat="server" ID="MainSelectControl" SelectPopupID="MainPopupControl" />
				</ContentTemplate>
			</asp:UpdatePanel>
			<mc:SelectPopupControl ID="MainPopupControl" runat="server" ClassName="Principal"/>
		</td>
		<td style="width:20px;">
			<asp:CustomValidator runat="server" ID="MainCustomValidator" ErrorMessage="*" Display="dynamic" OnServerValidate="MainCustomValidator_ServerValidate"></asp:CustomValidator>
		</td>
	</tr>
</table>
