<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Wizards.Modules.NewUserWizard" CodeBehind="NewUserWizard.ascx.cs" %>

<script type="text/javascript">
	function GroupExistence (sender,args)
	{
		if((document.forms[0].<%=lbSelectedGroups.ClientID%> != null)&& (document.forms[0].<%=lbSelectedGroups.ClientID%>.options.length>0))
		{
			args.IsValid = true;
			return;
		}
		args.IsValid = false;
	}
	function SaveGroups()
	{
		var sControl=document.forms[0].<%=lbSelectedGroups.ClientID%>;
		var str="";
		if(sControl != null)
		{
			for(var i=0;i<sControl.options.length;i++)
			{
				str += sControl.options[i].value + ",";
			}
		}
		document.getElementById('<%=iGroups.ClientID%>').value = str;
	}
</script>

<asp:Panel ID="step1" runat="server" Height="100%">
	<div class="text">
		<%=LocRM.GetString("s1TopDiv") %></div>
	<br/>
	<table width="100%">
		<tr>
			<td>
			</td>
			<td>
				<div class="text">
					<%=LocRM.GetString("s1TopText") %></div>
				<br/>
			</td>
		</tr>
		<tr>
			<td width="10%">
			</td>
			<td width="40%">
				<table>
					<tr>
						<td>
							<asp:RadioButtonList ID="rbAccount" runat="server" CssClass="text">
							</asp:RadioButtonList>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblUserType" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
				</table>
			</td>
			<td valign="top" align="middle" width="60">
				<img alt="" src="../layouts/images/quicktip.gif" border="0">
			</td>
			<td class="text" style="padding-right: 15px" valign="top">
				<%=s1Comments%>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step2" runat="server">
	<div class="text">
		<%=LocRM.GetString("s2TopDiv") %></div>
	<table width="100%">
		<tr>
			<td valign="top" width="50%">
				<fieldset style="height: 180px; margin: 0; padding: 2px">
					<legend class="text" id="lgdContactInf" runat="server"></legend>
					<table cellspacing="2" cellpadding="0" width="100%" border="0">
						<tr id="trLogin" runat="server">
							<td class="text" width="100">
								<b>
									<%=LocRM.GetString("tLogin")%>:</b>
							</td>
							<td>
								<asp:TextBox ID="txtLogin" runat="server" CssClass="text" Width="150px"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfLogin" runat="server" CssClass="text" ControlToValidate="txtLogin" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
								<asp:CustomValidator ID="cvLogin" runat="server" Display="Dynamic" ErrorMessage="Duplicate Login"></asp:CustomValidator>
								<asp:RegularExpressionValidator ID="txtRELoginValidator" runat="server" ControlToValidate="txtLogin" Display="Dynamic" ErrorMessage="*" ValidationExpression="^[\w-\.]+"></asp:RegularExpressionValidator>
							</td>
						</tr>
						<tr id="trPass" runat="server">
							<td class="text" width="100">
								<b>
									<%=LocRM.GetString("tPassword")%>:</b>
							</td>
							<td>
								<asp:TextBox ID="txtPassword" CssClass="text" runat="server" MaxLength="50" TextMode="Password" Width="150"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfPass" runat="server" ControlToValidate="txtPassword" ErrorMessage="*"></asp:RequiredFieldValidator>
							</td>
						</tr>
						<tr id="trConfirm" runat="server">
							<td class="text" width="100">
								<b>
									<%=LocRM.GetString("tConfirm")%>:</b>
							</td>
							<td>
								<asp:TextBox ID="txtConfirm" CssClass="text" runat="server" MaxLength="50" TextMode="Password" Width="150"></asp:TextBox>
								<asp:CompareValidator ID="cvConfirm" runat="server" ControlToValidate="txtPassword" ErrorMessage="*" ControlToCompare="txtConfirm"></asp:CompareValidator>
							</td>
						</tr>
						<tr>
							<td class="text" width="100">
								<b>
									<%=LocRM.GetString("tFirstName")%>:</b>
							</td>
							<td>
								<asp:TextBox ID="txtFirstName" runat="server" CssClass="text" Width="150px"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfFirstName" runat="server" CssClass="text" ControlToValidate="txtFirstName" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
							</td>
						</tr>
						<tr>
							<td class="text" width="100">
								<b>
									<%=LocRM.GetString("tLastName")%>:</b>
							</td>
							<td>
								<asp:TextBox ID="txtLastName" runat="server" CssClass="text" Width="150px"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfLastName" runat="server" CssClass="text" ControlToValidate="txtLastName" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
							</td>
						</tr>
						<tr>
							<td class="text" width="100">
								<b>
									<%=LocRM.GetString("tEMail")%>:</b>
							</td>
							<td>
								<asp:TextBox ID="txtEMail" runat="server" CssClass="text" Width="150px"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfEMail" runat="server" CssClass="text" ControlToValidate="txtEMail" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator ID="revEMail" runat="server" ControlToValidate="txtEMail" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
								<asp:CustomValidator ID="EmaiDuplication" runat="server" Display="Dynamic" ErrorMessage="Duplicate E-mail" Enabled="False"></asp:CustomValidator>
							</td>
							<tr>
								<td class="text" width="100">
									<b>
										<%=LocRM.GetString("tMobilePhone")%>:</b>
								</td>
								<td>
									<asp:TextBox ID="txtMobilePhone" runat="server" CssClass="text" Width="150px"></asp:TextBox>
								</td>
							</tr>
					</table>
				</fieldset>
			</td>
			<td valign="top" width="50%">
				<table cellspacing="0" cellpadding="0" border="0" width="100%">
					<tr>
						<td valign="top">
							<fieldset style="height: 125px; margin: 0; padding: 2px">
								<legend class="text" id="lgdCompanyinf" runat="server"></legend>
								<table cellspacing="2" cellpadding="0" width="100%">
									<tr>
										<td class="text" width="100">
											<b>
												<%=LocRM.GetString("tCompany")%>:</b>
										</td>
										<td>
											<asp:TextBox ID="txtCompany" runat="server" CssClass="text" Width="170px"></asp:TextBox>
										</td>
									</tr>
									<tr>
										<td class="text" width="100">
											<b>
												<%=LocRM.GetString("tDepartment")%>:</b>
										</td>
										<td>
											<asp:TextBox ID="txtDepartment" runat="server" CssClass="text" Width="170px"></asp:TextBox>
										</td>
									</tr>
									<tr>
										<td class="text" width="100">
											<b>
												<%=LocRM.GetString("tPosition")%>:</b>
										</td>
										<td>
											<asp:TextBox ID="txtPosition" runat="server" CssClass="text" Width="170px"></asp:TextBox>
										</td>
									</tr>
									<tr>
										<td class="text" width="100">
											<b>
												<%=LocRM.GetString("tLocation")%>:</b>
										</td>
										<td>
											<asp:TextBox ID="txtLocation" runat="server" CssClass="text" Width="170px"></asp:TextBox>
										</td>
									</tr>
									<tr>
										<td class="text" width="100">
											<b>
												<%=LocRM.GetString("tWorkPhone")%>:</b>
										</td>
										<td>
											<asp:TextBox ID="txtWorkPhone" runat="server" CssClass="text" Width="170px"></asp:TextBox>
										</td>
									</tr>
								</table>
							</fieldset>
						</td>
					</tr>
					<tr>
						<td>
							<fieldset style="height: 45px; margin: 0; padding: 2px">
								<legend class="text" id="lgdLang" runat="server"></legend>
								<table cellspacing="2" cellpadding="0" width="100%">
									<tr style="height: 10px">
										<td>
										</td>
									</tr>
									<tr>
										<td class="text" width="100">
											<b>
												<%=LocRM.GetString("tLanguage")%>:</b>
										</td>
										<td>
											<asp:DropDownList ID="ddLang" runat="server" CssClass="text" Width="170px">
											</asp:DropDownList>
										</td>
									</tr>
								</table>
							</fieldset>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td colspan="2" class="text">
				<fieldset style="height: 40px; vertical-align: top;">
					<legend class="text">
						<%=LocRM.GetString("TimeZone")%></legend>
					<table width="100%">
						<tr>
							<td>
								<asp:DropDownList ID="lstTimeZone" runat="server" CssClass="text" Width="100%">
								</asp:DropDownList>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step3" runat="server">
	<div class="text">
		<%=LocRM.GetString("s3TopDiv") %></div>
	<br/>
	<fieldset id="fsGroups" runat="server" style="margin: 0; padding: 2px">
		<legend class="text" id="lgdSelGroup" runat="server"></legend>
		<table cellspacing="0" cellpadding="0" width="100%">
			<tr>
				<td>
					<table cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td width="60">
							</td>
							<td class="text">
								<b>
									<%=LocRM.GetString("tBusinessGroup")%>:</b>
							</td>
						</tr>
						<tr>
							<td width="60">
							</td>
							<td align="left" colspan="3">
								<table class="text" id="tblGroups" style="height: 130px" cellpadding="0" border="0">
									<tr>
										<td valign="top" nowrap width="45%" style="padding-right: 6px; padding-bottom: 6px">
											<asp:Label ID="lblAvailable" runat="server" CssClass="text"></asp:Label><br/>
											<asp:ListBox ID="lbAvailableGroups" runat="server" Width="100%" CssClass="text" Height="90%"></asp:ListBox>
										</td>
										<td style="padding-right: 6px; padding-left: 6px; padding-bottom: 6px">
											<div align="center">
												<asp:Button ID="btnAddOneGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text=">"></asp:Button><br/>
												<asp:Button ID="btnAddAllGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text=">>"></asp:Button><br/>
												<br/>
												<asp:Button ID="btnRemoveOneGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text="<"></asp:Button><br/>
												<asp:Button ID="btnRemoveAllGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text="<<"></asp:Button></div>
										</td>
										<td valign="top" width="45%" style="padding-right: 20px; padding-left: 6px; padding-bottom: 6px">
											<asp:Label ID="lblSelected" runat="server" CssClass="text"></asp:Label><br/>
											<asp:ListBox ID="lbSelectedGroups" runat="server" Width="97%" CssClass="text" Height="90%" BorderWidth="1"></asp:ListBox>
											<asp:CustomValidator ID="GroupValidator" Style="vertical-align: top" runat="server" ErrorMessage="*" ClientValidationFunction="GroupExistence"></asp:CustomValidator>
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr id="trIMGroup" runat="server">
				<td>
					<table cellspacing="0" cellpadding="0" width="100%">
						<tr>
							<td width="60">
							</td>
							<td class="text" width="150">
								<b>
									<%=LocRM.GetString("tCommGroup")%>:</b>
							</td>
							<td align="left">
								<asp:DropDownList ID="ddIMGroup" runat="server" CssClass="text" Width="150px">
								</asp:DropDownList>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</fieldset>
	<fieldset id="fsRole" runat="server" style="margin: 0; padding: 2px">
		<legend class="text" id="lgdRole" runat="server"></legend>
		<table cellspacing="0" cellpadding="0" width="100%">
			<tr>
				<td align="middle">
					<asp:CheckBoxList ID="cbRole" runat="server" CssClass="text">
					</asp:CheckBoxList>
				</td>
			</tr>
		</table>
	</fieldset>
	<fieldset id="fsTextExt" runat="server">
		<legend class="text" id="lgdTextExt" runat="server"></legend>
		<table cellspacing="0" cellpadding="0" width="100%">
			<tr>
				<td valign="top" align="middle" width="60">
					<img alt="" src="../layouts/images/quicktip.gif" border="0">
				</td>
				<td class="text" style="padding-right: 15px" valign="center">
					<%=LocRM.GetString("tTextForExternal")%>
				</td>
			</tr>
		</table>
	</fieldset>
	<fieldset id="fsPartner" runat="server">
		<legend class="text" id="lgdTextPartner" runat="server"></legend>
		<table cellspacing="0" cellpadding="0" width="100%" style="margin-top: 5px">
			<tr>
				<td class="text" valign="top">
					<asp:Label ID="lblGroupsTitle" runat="server" CssClass="text" Font-Bold="True"></asp:Label><strong>:</strong>
				</td>
				<td class="text" style="padding-bottom: 5px" align="left">
					<asp:DropDownList ID="ddPartnerGroups" CssClass="text" runat="server" Width="210px">
					</asp:DropDownList>
				</td>
			</tr>
		</table>
	</fieldset>
	<input id="iGroups" type="hidden" name="iGroups" runat="server">
</asp:Panel>
<asp:Panel ID="step4" runat="server">
	<div>
		<asp:Label ID="lblstep4" runat="server" CssClass="text"></asp:Label></div>
	<br/>
	<fieldset>
		<legend class="text" id="lgdWelcome" runat="server"></legend>
		<table width="100%">
			<tr>
				<td valign="top" align="middle" width="60">
					<img alt="" src="../layouts/images/quicktip.gif" border="0">
				</td>
				<td class="text" style="padding-right: 15px" valign="top" width="150">
					<%=LocRM.GetString("tTextForWelcome")%>
				</td>
				<td align="right">
					<asp:TextBox ID="txtWelcome" runat="server" CssClass="text" Width="300px" TextMode="MultiLine" Height="150px"></asp:TextBox>
				</td>
				<td width="5">
				</td>
			</tr>
		</table>
	</fieldset>
</asp:Panel>
<asp:Panel ID="step5" runat="server">
	<div class="text">
		<img height="20" src="../layouts/images/help.gif" width="20" align="absMiddle">
		&nbsp;<%=LocRM.GetString("s5TopText") %>
	</div>
	<br/>
	<asp:Label ID="lblError" runat="server" CssClass="ibn-alerttext" Visible="False"></asp:Label>
</asp:Panel>
