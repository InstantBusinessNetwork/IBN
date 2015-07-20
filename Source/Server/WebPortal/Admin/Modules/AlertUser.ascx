<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.AlertUser" Codebehind="AlertUser.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<script type="text/javascript">
	function EnableDisable()
	{
		disable = !(document.forms[0].<%=cbUseAS.ClientID %>.checked);
		
		document.forms[0].<%=tbFirstname.ClientID %>.disabled = disable;
		document.forms[0].<%=tbLastName.ClientID %>.disabled = disable;
		document.forms[0].<%=tbEmail.ClientID %>.disabled = disable;
		document.forms[0].<%=ddSmtpBoxes.ClientID %>.disabled = disable;
		document.forms[0].<%=txtSubjectTemp.ClientID %>.disabled = disable;
		
		var obj = document.getElementById('<%=rf1.ClientID %>');
		obj.enabled = !disable;
		var obj1 = document.getElementById('<%=RequiredFieldValidator1.ClientID %>');
		obj1.enabled = !disable;
		var obj2 = document.getElementById('<%=RequiredFieldValidator2.ClientID %>');
		obj2.enabled = !disable;
		var obj3 = document.getElementById('<%=RegularExpressionValidator1.ClientID %>');
		obj3.enabled = !disable;
		var obj4 = document.getElementById('<%=cv1.ClientID %>');
		obj4.enabled = !disable;
	}
</script>
<table cellspacing="0" cellpadding="7" border="0" class="ibn-stylebox2 text" width="100%">
	<tr>
		<td style="padding:0px">
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
		<fieldset>
			<legend><asp:CheckBox ID="cbUseAS" Runat="server"></asp:CheckBox></legend>
			<table class="text" cellpadding="7" border="0">
				<tr>
					<td width="160"><b><%=LocRM.GetString("FirstName")%>:</b></td>
					<td width="400">
						<asp:TextBox id="tbFirstname" runat="server" Width="220px" CssClass="text"></asp:TextBox>
						<asp:RequiredFieldValidator id="rf1" runat="server" ControlToValidate="tbFirstname" ErrorMessage="*"></asp:RequiredFieldValidator></td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("LastName")%>:</b></td>
					<td>
						<asp:TextBox id="tbLastName" runat="server" Width="220px" CssClass="text"></asp:TextBox>
						<asp:RequiredFieldValidator CssClass="text" id="RequiredFieldValidator1" runat="server" ControlToValidate="tbLastName" ErrorMessage="*"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("Email")%>:</b></td>
					<td valign="top">
						<asp:TextBox id="tbEmail" runat="server" Width="220px" CssClass="text"></asp:TextBox>
						<asp:RequiredFieldValidator CssClass="text" id="RequiredFieldValidator2" runat="server" Display="Dynamic" ControlToValidate="tbEmail" ErrorMessage="*"></asp:RequiredFieldValidator>
						<asp:RegularExpressionValidator CssClass="text" id="RegularExpressionValidator1" runat="server" ControlToValidate="tbEmail" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
						<asp:CustomValidator id=cv1 runat="server" CssClass="Text" ErrorMessage="Cv"></asp:CustomValidator>
					</td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM2.GetString("tSmtpBox")%>:</b></td>
					<td valign="top">
						<asp:DropDownList ID="ddSmtpBoxes" runat="server" Width="220px"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("SubjectTemplate")%>:</b></td>
					<td>
						<asp:TextBox id="txtSubjectTemp" runat="server" Width="220px" CssClass="text"></asp:TextBox>
						<asp:RequiredFieldValidator CssClass="text" id="RequiredFieldValidator3" runat="server" ControlToValidate="txtSubjectTemp" ErrorMessage="*"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td></td>
					<td><asp:CheckBox ID="cbUseIMServer" runat="server" CssClass="text" /></td>
				</tr>
			</table>
		</fieldset>
		</td>
	</tr>
	<tr height="40px">
		<td vAlign="bottom" align="left" style="padding-left:200px;">
			<btn:imbutton class="text" id="btnMove" Runat="server" style="width:110px;" onserverclick="btn_Save" />&nbsp;&nbsp;
			<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" IsDecline="true" style="width:110px;" onserverclick="btn_Cancel" />
		</td>
	</tr>
</table>
