<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.TrialReqEdit" Codebehind="TrialReqEdit.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\Modules\BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secH" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table style="MARGIN-TOP: 10px; MARGIN-LEFT: 20px; BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" cellspacing="3" cellpadding="3" width="100%">
				<tr>
					<td width="150"><strong><asp:label id="lblCompName" CssClass="text" Runat="server"></asp:label></strong></td>
					<td><asp:textbox id="txtCompName" Width="250px" CssClass="text" Runat="server"></asp:textbox></td>
				</tr>
				<tr>
					<td width="150"><strong><asp:label id="lblDescr" CssClass="text" Runat="server"></asp:label></strong></td>
					<td><asp:textbox Height="80" TextMode="MultiLine" Width="250px" id="txtDescr" CssClass="text" Runat="server"></asp:textbox></td>
				</tr>
				<tr>
					<td width="150"><strong><asp:label id="lblDomain" CssClass="text" Runat="server"></asp:label></strong></td>
					<td><asp:textbox id="txtDomain" Width="250px" CssClass="text" Runat="server"></asp:textbox></td>
				</tr>
				<tr>
					<td width="150"><strong><asp:label id="lblFirstName" CssClass="text" Runat="server"></asp:label></strong></td>
					<td><asp:textbox id="txtFirstName" Width="250px" CssClass="text" Runat="server"></asp:textbox></td>
				</tr>
				<tr>
					<td width="150"><strong><asp:label id="lblLastName" CssClass="text" Runat="server"></asp:label></strong></td>
					<td><asp:textbox id="txtLastName" Width="250px" CssClass="text" Runat="server"></asp:textbox></td>
				</tr>
				<tr>
					<td width="150"><strong><asp:label id="lblEMail" CssClass="text" Runat="server"></asp:label></strong></td>
					<td>
						<asp:textbox id="txtEMail" Width="250px" CssClass="text" Runat="server"></asp:textbox>
						<asp:RegularExpressionValidator id="revEMail" runat="server" ControlToValidate="txtEMail" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
					</td>
				</tr>
				<tr>
					<td width="150"><strong><asp:label id="lblPhone" CssClass="text" Runat="server"></asp:label></strong></td>
					<td><asp:textbox id="txtPhone" Width="250px" CssClass="text" Runat="server"></asp:textbox></td>
				</tr>
				<tr>
					<td width="150"><strong><asp:label id="lblLogin" CssClass="text" Runat="server"></asp:label></strong></td>
					<td><asp:textbox id="txtLogin" Width="250px" CssClass="text" Runat="server"></asp:textbox></td>
				</tr>
				<tr>
					<td width="150"><strong><asp:label id="lblPassword" CssClass="text" Runat="server"></asp:label></strong></td>
					<td>
						<asp:textbox id="txtPassword" Width="250px" CssClass="text" Runat="server"></asp:textbox>
						<asp:RequiredFieldValidator runat="server" ID="txtPasswordValidator" ControlToValidate="txtPassword" ErrorMessage="*"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td width="150"><strong><asp:label id="lblCountry" CssClass="text" Runat="server"></asp:label></strong></td>
					<td><asp:textbox id="txtCountry" Width="250px" CssClass="text" Runat="server"></asp:textbox></td>
				</tr>
				<tr>
					<td width="150"><strong><asp:label ID="lblDefLang" CssClass="text" Runat=server></asp:label></td>
					<td><asp:DropDownList id="ddLanguage" Runat="server" Width="250px" cssclass="text"></asp:DropDownList></td>
				</tr>
				<tr>
					<td></td>
					<td class="text"><%=LocRM.GetString("Comments")%></td>
				</tr>
				<tr>
					<td height="5"></td>
					<td></td>
				</tr>
				<tr>
					<td></td>
					<td>
						<asp:Button ID="btnSave" CssClass="text" Runat=server Width="80px" onclick="btnSave_Click"></asp:Button>&nbsp;&nbsp;
						<asp:button id="btnCreate" CssClass="text" Runat="server" Width="80px" onclick="btnCreate_Click"></asp:button>&nbsp;&nbsp;
						<asp:button id="btnCancel" CssClass="text" Runat="server" Width="80px" onclick="btnCancel_Click"></asp:button>&nbsp;&nbsp;
					</td>
				</tr>
				<tr>
					<td height="5"></td>
					<td></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
