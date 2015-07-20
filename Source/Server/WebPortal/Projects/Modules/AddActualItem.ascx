<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.AddActualItem" Codebehind="AddActualItem.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="ibn-stylebox text" cellspacing="0" cellpadding="0" border="0" width="100%" style="MARGIN-TOP: 0px;padding-bottom:10px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<table cellpadding="7" cellspacing="0" border="0" width="100%">
				<tr>
					<td width="80px" class="text"><b><%=LocRM.GetString("tAccount")%>:</b></td>
					<td>
						<asp:DropDownList ID="ddAccounts" Runat=server Width="170px" CssClass="text"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td class="text" valign="top"><b><%=LocRM.GetString("Description")%>:</b></td>
					<td>
						<asp:TextBox ID="txtDescription" Runat="server" CssClass=text TextMode=MultiLine Width="170px" Rows="3"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="text"><b><%=LocRM.GetString("Actual")%>:</b></td>
					<td>
						<asp:TextBox ID="txtValue" Runat=server CssClass=text Width="100px"></asp:TextBox>
						<asp:RequiredFieldValidator ID="rfVal" Runat=server CssClass="text" ErrorMessage="*" ControlToValidate="txtValue" Display=Dynamic></asp:RequiredFieldValidator>
						<asp:CompareValidator ID="cvVal" Runat=server CssClass="text" ErrorMessage="*" Display=Dynamic ControlToValidate="txtValue" ValueToCompare="0" Type=Currency Operator=GreaterThanEqual></asp:CompareValidator>
					</td>
			</table>
		</td>
	</tr>
	<tr>
		<td style="padding-left:100px">
			<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;" onserverclick="btnSave_click"></btn:imbutton>&nbsp;&nbsp;
			<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true" onserverclick="btnCancel_click"></btn:imbutton>
		</td>
	</tr>
</table>