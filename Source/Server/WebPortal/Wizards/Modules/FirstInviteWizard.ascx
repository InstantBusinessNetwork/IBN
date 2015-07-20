<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Wizards.Modules.FirstInviteWizard" Codebehind="FirstInviteWizard.ascx.cs" %>
<asp:panel id="step1" Runat="server" Height="100%">
<div class="text"><%=LocRM.GetString("s1TopDivInvite") %></div><br>
<table height="90%" cellspacing="0" cellpadding="0" width="100%" border=0>
	<tr>
		<td vAlign=top width="100%">
<FIELDSET style="HEIGHT: 180px;margin:0;padding:2px">
	<LEGEND class="text ibn-legend-default" id="lgdContactInf" runat="server"></LEGEND>
	<table cellspacing="2" cellpadding="0" width="100%" border="0">
		<tr height="10px"><td></td></tr>
		<tr>
			<td class="text" width="80" align=right>
				<B><%=LocRM.GetString("tFirstName")%>:</B>
			</td>
			<td class="text" width="110">
				<asp:TextBox ID="txtFirstName1" Runat=server Width="110px"></asp:TextBox>
			</td>
			<td class="text" width="75" align=right>
				<B><%=LocRM.GetString("tLastName")%>:</B>
			</td>
			<td class="text" width="110">
				<asp:TextBox ID="txtLastName1" Runat=server Width="110px"></asp:TextBox>
			</td>
			<td class="text" width="45" align=right>
				<B><%=LocRM.GetString("teMail")%>:</B>
			</td>
			<td class="text" width="110">
				<asp:TextBox ID="txtEMail1" Runat=server Width="95px"></asp:TextBox>
				<asp:RegularExpressionValidator id="revEMail1" runat="server" ControlToValidate="txtEMail1" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
			</td>
		</tr>
		<tr height="10px"><td></td></tr>
		<tr>
			<td class="text" width="80" align=right>
				<B><%=LocRM.GetString("tFirstName")%>:</B>
			</td>
			<td class="text" width="110">
				<asp:TextBox ID="txtFirstName2" Runat=server Width="110px"></asp:TextBox>
			</td>
			<td class="text" width="75" align=right>
				<B><%=LocRM.GetString("tLastName")%>:</B>
			</td>
			<td class="text" width="110">
				<asp:TextBox ID="txtLastName2" Runat=server Width="110px"></asp:TextBox>
			</td>
			<td class="text" width="45" align=right>
				<B><%=LocRM.GetString("teMail")%>:</B>
			</td>
			<td class="text" width="110">
				<asp:TextBox ID="txtEMail2" Runat=server Width="95px"></asp:TextBox>
				<asp:RegularExpressionValidator id="revEMail2" runat="server" ControlToValidate="txtEMail2" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
			</td>
		</tr>
		<tr height="10px"><td></td></tr>
		<tr>
			<td class="text" width="80" align=right>
				<B><%=LocRM.GetString("tFirstName")%>:</B>
			</td>
			<td class="text" width="110">
				<asp:TextBox ID="txtFirstName3" Runat=server Width="110px"></asp:TextBox>
			</td>
			<td class="text" width="75" align=right>
				<B><%=LocRM.GetString("tLastName")%>:</B>
			</td>
			<td class="text" width="110">
				<asp:TextBox ID="txtLastName3" Runat=server Width="110px"></asp:TextBox>
			</td>
			<td class="text" width="45" align=right>
				<B><%=LocRM.GetString("teMail")%>:</B>
			</td>
			<td class="text" width="110">
				<asp:TextBox ID="txtEMail3" Runat=server Width="95px"></asp:TextBox>
				<asp:RegularExpressionValidator id="revEMail3" runat="server" ControlToValidate="txtEMail3" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
			</td>
		</tr>
		<tr height="10px"><td></td></tr>
		<tr>
			<td class="text" width="80" align=right>
				<B><%=LocRM.GetString("tFirstName")%>:</B>
			</td>
			<td class="text" width="110">
				<asp:TextBox ID="txtFirstName4" Runat=server Width="110px"></asp:TextBox>
			</td>
			<td class="text" width="75" align=right>
				<B><%=LocRM.GetString("tLastName")%>:</B>
			</td>
			<td class="text" width="110">
				<asp:TextBox ID="txtLastName4" Runat=server Width="110px"></asp:TextBox>
			</td>
			<td class="text" width="45" align=right>
				<B><%=LocRM.GetString("teMail")%>:</B>
			</td>
			<td class="text" width="110">
				<asp:TextBox ID="txtEMail4" Runat=server Width="95px"></asp:TextBox>
				<asp:RegularExpressionValidator id="revEMail4" runat="server" ControlToValidate="txtEMail4" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
			</td>
		</tr>
	</table>
</fieldset>
</td></tr>
<tr>
		<td vAlign=bottom>
			<asp:CheckBox id=cbShowNextTime Runat="server" Text='<%#LocRM.GetString("ShowOnNextLogin") %>' CssClass="subHeader" Checked="True">
			</asp:CheckBox>
		</td>
	</tr>
	</table>
</asp:panel>
<asp:panel id="step2" Runat="server" Height="100%">
<table height="100%" cellspacing="0" cellpadding="0" width="100%" border=0>
	<tr>
		<td vAlign=center width="100%">
			<table width="100%">
				<tr>
					<td width=40></td>
					<td vAlign=top width=80><IMG src="../layouts/images/check.gif" align=absMiddle></td>
					<td class=text vAlign=top><BR>
						<BR>
						<%=LocRM.GetString("s2TopText") %>
					</td>
					<td width=40></td>
				</tr>
			</table>
		</td>
	</tr>
	
</table>

<BR>
</asp:panel>