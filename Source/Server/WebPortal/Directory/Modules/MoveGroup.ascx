<%@ Reference Control="~/Directory/Modules/UserEdit.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.MoveGroup" Codebehind="MoveGroup.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellspacing="0" cellpadding="0" border="0" class="ibn-stylebox" width="100%" style="margin-top:0px;margin-left:2px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" title="Department 1" />
		</td>
	</tr>
	<tr>
		<td class="text" style="PADDING-RIGHT:15px; PADDING-LEFT:15px; PADDING-BOTTOM:15px; PADDING-TOP:15px">
			<table cellspacing="0" cellpadding="0" border="0" class="ibn-propertysheet">
				<tr>
					<td class="text">
						<asp:Label ID="lblMoveTo" Runat="server" CssClass="boldtext"></asp:Label>:&nbsp;
						<asp:DropDownList id="ddGroupsList" runat="server" CssClass="text" Width="198px"></asp:DropDownList>
						<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" CssClass="Text" ErrorMessage="* You must select group" ControlToValidate="ddGroupsList" Display="Dynamic"></asp:RequiredFieldValidator></td>
				</tr>
				<tr>
					<td vAlign="bottom" align="right" height="40">
						<btn:imbutton class="text" id="btnMove" Runat="server" style="width:115px;" onserverclick="btnMove_ServerClick"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true" onserverclick="btnCancel_ServerClick"></btn:imbutton></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
