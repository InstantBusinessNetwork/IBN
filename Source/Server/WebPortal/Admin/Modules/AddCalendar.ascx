<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.AddCalendar" Codebehind="AddCalendar.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox" style="MARGIN-TOP: 0px" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" title="" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td class="ibn-propertysheet">
			<table cellpadding="3" class="text" cellspacing="3" border="0">
				<tr>
					<td class="text"><b><%=LocRM.GetString("Title") %>:</b></td>
					<td>
						<asp:TextBox Runat="server" ID="tbTitle" CssClass="text" Width="100%"></asp:TextBox>
					</td>
					<td style="width:20px;">
						<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" CssClass="text" ErrorMessage="*" ControlToValidate="tbTitle" Display="Dynamic"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("TimeZone") %>:</b></td>
					<td><asp:dropdownlist id="lstTimeZone" runat="server" CssClass="text"></asp:dropdownlist></td>
					<td></td>
				</tr>
				<tr>
					<td valign="bottom" align="right" height="40" colspan="3">
						<btn:imbutton class="text" id="btnSave" Runat="server" Text="" style="width:115px;" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" Text="" IsDecline="true" style="width:115px;" onserverclick="btnCancel_ServerClick"></btn:imbutton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
