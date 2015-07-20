<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentCreate.ascx.cs" Inherits="Mediachase.Ibn.WebAsp.Modules.PaymentCreate" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<script type="text/javascript" src="../Scripts/cal.js"></script>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin:0px;">
	<tr>
		<td><ibn:BlockHeader id="secHeader" runat="server"></ibn:BlockHeader></td>
	</tr>
	<tr>
		<td>
			<table cellpadding="7" class="text">
				<tr>
					<td><b><%=LocRM.GetString("CompanyName")%>:</b></td>
					<td><asp:DropDownList ID="ddCompanies" runat="server" Width="300px"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("Amount")%>:</b></td>
					<td><asp:TextBox ID="txtAmount" runat="server" CssClass="text" Width="300px"></asp:TextBox>
					<asp:RequiredFieldValidator ID="rf1" runat="server" ControlToValidate="txtAmount" ErrorMessage="*"
					Display="Dynamic" CssClass="text"></asp:RequiredFieldValidator>
					<asp:CompareValidator ID="cv1" runat="server" ControlToValidate="txtAmount" ErrorMessage="*"
					Display="Dynamic" CssClass="text" Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("Bonus")%>:</b></td>
					<td><asp:TextBox ID="txtBonus" runat="server" CssClass="text" Width="300px"></asp:TextBox>
					<asp:RequiredFieldValidator ID="rf2" runat="server" ControlToValidate="txtBonus" ErrorMessage="*"
					Display="Dynamic" CssClass="text"></asp:RequiredFieldValidator>
					<asp:CompareValidator ID="cv2" runat="server" ControlToValidate="txtBonus" ErrorMessage="*"
					Display="Dynamic" CssClass="text" Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("OrderNo")%>:</b></td>
					<td>
						<asp:TextBox runat="server" ID="OrderNo" Width="300px" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("PaymentDate")%>:</b></td>
					<td>
						<asp:textbox id="PaymentDate" runat="server" CssClass="text" width="97"></asp:textbox><button 
							id="PaymentDateButton" 
							style="BORDER-RIGHT: 0px; BORDER-TOP: 0px; BORDER-LEFT: 0px; WIDTH: 39px; PADDING-TOP: 0px; BORDER-BOTTOM: 0px; POSITION: relative; TOP: 0px; HEIGHT: 24px; BACKGROUND-COLOR: transparent" 
							onclick="ShowCal('<%=PaymentDate.ClientID %>','PaymentDateButton');" 
							type=button><IMG height="21" src="../layouts/images/calendar.gif" width="34" border="0"></button>
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
