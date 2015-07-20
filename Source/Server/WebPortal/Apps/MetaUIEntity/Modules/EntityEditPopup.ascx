<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntityEditPopup.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaUIEntity.Modules.EntityEditPopup" %>
<%@ Reference Control="~/Apps/MetaUI/MetaForm/FormDocumentView.ascx" %>
<%@ Register TagPrefix="ibn" TagName="formView" Src="~/Apps/MetaUI/MetaForm/FormDocumentView.ascx" %>
<table class="ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:formView ID="EditForm" runat="server" />
		</td>
	</tr>
	<tr>
		<td style="text-align: center; padding-bottom:8px;" >
			<table align="center">
				<tr>
					<td>
						<asp:Button runat="server" ID="SaveButton" CausesValidation="true" CommandArgument="0" Text="<%$Resources : IbnFramework.Global, _mc_Save%>" OnClick="SaveButton_Click" />&nbsp;&nbsp;
						<asp:Button runat="server" ID="CancelButton" CausesValidation="false" Text="<%$Resources : IbnFramework.Global, _mc_Cancel%>" />			
					</td>
				</tr>
				<tr id="trGoToView" runat="server">
					<td align="left" style="padding-top:10px">
						<asp:CheckBox ID="cbGoToView" runat="server" Text="Go to View Page" />
					</td>
				</tr>
			</table>
		</td>
	</tr> 
</table>