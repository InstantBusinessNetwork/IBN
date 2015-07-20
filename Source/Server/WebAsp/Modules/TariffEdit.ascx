<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TariffEdit.ascx.cs" Inherits="Mediachase.Ibn.WebAsp.Modules.TariffEdit" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin:0px;">
	<tr>
		<td><ibn:BlockHeader id="secHeader" runat="server"></ibn:BlockHeader></td>
	</tr>
	<tr>
		<td>
			<table cellpadding="7" class="text">
				<tr>
					<td><b><%=LocRM.GetString("TariffName")%>:</b></td>
					<td><asp:TextBox ID="txtName" runat="server" CssClass="text" Width="300px"></asp:TextBox>
						<asp:RequiredFieldValidator ID="rf1" runat="server" ControlToValidate="txtName" ErrorMessage="*"
						Display="Dynamic" CssClass="text"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("Description")%>:</b></td>
					<td valign="top"><asp:TextBox ID="txtDescription" runat="server" CssClass="text" Rows="4" TextMode="MultiLine" Width="300px"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("TariffTypeName")%>:</b></td>
					<td><asp:DropDownList ID="ddType" runat="server" Width="300px"></asp:DropDownList></td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("CurrencyName")%>:</b></td>
					<td><asp:DropDownList ID="ddCurrency" runat="server" Width="300px"></asp:DropDownList></td>
				</tr>
				
				<tr>
					<td><b><%=LocRM.GetString("MaxHdd")%>:</b></td>
					<td><asp:TextBox ID="txtMaxHdd" runat="server" CssClass="text" Width="300px"></asp:TextBox>
					<asp:RequiredFieldValidator ID="rf2" runat="server" ControlToValidate="txtMaxHdd" ErrorMessage="*"
					Display="Dynamic" CssClass="text"></asp:RequiredFieldValidator>
					<asp:CompareValidator ID="cv2" runat="server" ControlToValidate="txtMaxHdd" ErrorMessage="*"
					Display="Dynamic" CssClass="text" Operator="GreaterThanEqual" ValueToCompare="-1" Type="Integer"></asp:CompareValidator>
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("MaxUsers")%>:</b></td>
					<td><asp:TextBox ID="txtMaxUsers" runat="server" CssClass="text" Width="300px"></asp:TextBox>
					<asp:RequiredFieldValidator ID="rf3" runat="server" ControlToValidate="txtMaxUsers" ErrorMessage="*"
					Display="Dynamic" CssClass="text"></asp:RequiredFieldValidator>
					<asp:CompareValidator ID="cv3" runat="server" ControlToValidate="txtMaxUsers" ErrorMessage="*"
					Display="Dynamic" CssClass="text" Operator="GreaterThanEqual" ValueToCompare="-1" Type="Integer"></asp:CompareValidator>
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("MaxExternalUsers")%>:</b></td>
					<td><asp:TextBox ID="txtMaxExtUsers" runat="server" CssClass="text" Width="300px"></asp:TextBox>
					<asp:RequiredFieldValidator ID="rf4" runat="server" ControlToValidate="txtMaxExtUsers" ErrorMessage="*"
					Display="Dynamic" CssClass="text"></asp:RequiredFieldValidator>
					<asp:CompareValidator ID="cv4" runat="server" ControlToValidate="txtMaxExtUsers" ErrorMessage="*"
					Display="Dynamic" CssClass="text" Operator="GreaterThanEqual" ValueToCompare="-1" Type="Integer"></asp:CompareValidator>
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("MonthlyCost")%>:</b></td>
					<td><asp:TextBox ID="txtMonthlyCost" runat="server" CssClass="text" Width="300px"></asp:TextBox>
					<asp:RequiredFieldValidator ID="rf5" runat="server" ControlToValidate="txtMonthlyCost" ErrorMessage="*"
					Display="Dynamic" CssClass="text"></asp:RequiredFieldValidator>
					<asp:CompareValidator ID="cv5" runat="server" ControlToValidate="txtMonthlyCost" ErrorMessage="*"
					Display="Dynamic" CssClass="text" Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
					</td>
				</tr>
				<tr>
					<td colspan="2" align="center">
						<asp:Button ID="btnSave" runat="server" CssClass="text" />&nbsp;&nbsp;
						<asp:Button ID="btnCancel" runat="server" CssClass="text" CausesValidation="false" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
