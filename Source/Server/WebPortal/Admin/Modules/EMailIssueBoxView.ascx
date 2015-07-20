<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailIssueBoxView" Codebehind="EMailIssueBoxView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BHLWM" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="Time" src="~/Modules/TimeControl.ascx" %>
<script type="text/javascript">
	function OpenWindow(query,w,h,scroll)
		{
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			
			winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
			if (scroll) winprops+=',scrollbars=1';
			var f = window.open(query, "_blank", winprops);
		}
	function RefreshFromGroup(params)
	{
		var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);
		if(obj && obj.CommandArguments && obj.CommandArguments.Key)
			__doPostBack('<%=btnRefresh.UniqueID %>', obj.CommandArguments.Key);
	}
	
	function RefreshFromGroup2(params)
	{
		var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);
		if (obj && obj.CommandArguments && obj.CommandArguments.Key != null) 
			__doPostBack('<%=btnRefresh2.UniqueID %>', obj.CommandArguments.Key);
	}
	
	function CheckResponsible(sender,args)
	{
		var ddl = document.getElementById('<%= ddResponsible.ClientID %>');
		
		args.IsValid = true;
		if (ddl && ddl.value == "-1")
		{
			args.IsValid = false;	
		}
	}
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td class="ibn-alternating ibn-navline">
			<table cellpadding="7" cellspacing="0" border="0" class="text">
				<tr>
					<td><b><%= LocRM.GetString("tName")%>:</b></td>
					<td><asp:TextBox ID="lblIssBoxName" Runat="server" Width="150px" CssClass="text"></asp:TextBox>
						<asp:RequiredFieldValidator Runat="server" ID="rfvName" ErrorMessage="*" Display="Dynamic" ControlToValidate="lblIssBoxName"></asp:RequiredFieldValidator>
					</td>
					<td><asp:CheckBox Runat="server" ID="cbIsDefault"></asp:CheckBox></td>
				</tr>
				<tr>
					<td><b><%= LocRM.GetString("tIdentMask")%>:</b></td>
					<td><asp:TextBox ID="tbMask" Runat="server" Width="150px" CssClass="text"></asp:TextBox>
						<asp:RequiredFieldValidator Runat="server" ID="rfvMask" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbMask"></asp:RequiredFieldValidator>
						<asp:RegularExpressionValidator Runat="server" ID="revName" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbMask" ValidationExpression="\w+"></asp:RegularExpressionValidator>
					</td>
					<td><asp:Label Runat="server" ID="lblDuplicate" ForeColor="Red"></asp:Label></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="padding: 5px">
			<table cellpadding="0" cellspacing="7" width="100%" border="0">
				<tr>
					<td width="50%" valign="top">
						<ibn:BHLWM id="bh1" runat="server"></ibn:BHLWM>
						<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
							<tr>
								<td align="right" width="40%"><b><%=LocRM.GetString("tManager")%>:</b></td>
								<td><asp:DropDownList ID="ddManager" Runat="server" Width="200px"></asp:DropDownList></td>
							</tr>
							<tr>
								<td style="padding-left:30px" colspan="2"><asp:CheckBox ID="cbAllowAddRes" Runat="server" CssClass="text"></asp:CheckBox></td>
							</tr>
							<tr>
								<td style="padding-left:30px" colspan="2"><asp:CheckBox ID="cbAllowAddToDo" Runat="server" CssClass="text"></asp:CheckBox></td>
							</tr>
							<tr id="trCalendar" runat="server">
								<td align="right" width="40%"><b><%=LocRM.GetString("tPrjCalendar")%>:</b></td>
								<td><asp:DropDownList ID="ddCalendar" Runat="server" Width="200px"></asp:DropDownList></td>
							</tr>
							<tr>
								<td align="right"><b><%=LocRM.GetString("tExpAssignTime")%>:</b></td>
								<td>
									<table class="text">
										<tr>
											<td><ibn:Time id="ucAssignTime" ShowTime="HM" HourSpinMaxValue="999" ViewStartDate="True" runat="server" /></td>
											<td class="ibn-description"><%=LocRM2.GetString("WorkHours")%></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td align="right"><b><%=LocRM.GetString("tExpRespTime")%>:</b></td>
								<td>
									<table class="text">
										<tr>
											<td><ibn:Time id="ucResponseTime" ShowTime="HM" HourSpinMaxValue="999" ViewStartDate="True" runat="server" /></td>
											<td class="ibn-description"><%=LocRM2.GetString("WorkHours")%></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td align="right" width="40%"><b><%=LocRM.GetString("tExpDuration")%>:</b></td>
								<td>
									<table class="text">
										<tr>
											<td><ibn:Time id="ucDuration" ShowTime="HM" HourSpinMaxValue="999" ViewStartDate="True" runat="server" /></td>
											<td class="ibn-description"><%=LocRM2.GetString("WorkHours")%></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td align="right" width="40%"><b><%=LocRM.GetString("TaskTime")%>:</b></td>
								<td>
									<table class="text">
										<tr>
											<td><ibn:Time id="ucTaskTime" ShowTime="HM" HourSpinMaxValue="999" ViewStartDate="True" runat="server" /></td>
											<td class="ibn-description"><%=LocRM2.GetString("WorkHours")%></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<ibn:BHLWM id="bh2" runat="server"></ibn:BHLWM>
						<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
							<tr>
								<td align="right"><b><%=LocRM.GetString("tResponsibleType")%>:</b></td>
								<td><asp:DropDownList ID="ddRespType" Runat="server" Width="200px" AutoPostBack="True"></asp:DropDownList></td>
							</tr>
							<tr id="trCustomUser" runat="server">
								<td align="right"><b><%=LocRM.GetString("tResponsible")%>:</b></td>
								<td nowrap="nowrap">
									<asp:DropDownList ID="ddResponsible" Runat="server" Width="200px"></asp:DropDownList>
									<asp:CustomValidator runat="server" ID="ResponsibleValidator" ControlToValidate="ddResponsible" Display="Dynamic" ErrorMessage="*" OnServerValidate="ResponsibleValidator_ServerValidate" EnableClientScript="true" ClientValidationFunction="CheckResponsible"></asp:CustomValidator>
								</td>
							</tr>
							<tr>
								<td align="right" valign="top" style="padding-top:10px"><asp:Label ID="lblForResp" Runat="server" Font-Bold="true"></asp:Label></td>
								<td valign="top">
									<table cellpadding="3" cellspacing="0" border="0">
										<tr>
											<td width="202px" valign="top">
												<asp:Label ID="lblResponsible" Runat="server" CssClass="text"></asp:Label>
											</td>
											<td valign="top">
												<asp:Label CssClass="text" ID="lblChangeButton" Runat="server"></asp:Label>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td style="padding-left:30px" colspan="2"><asp:CheckBox ID="cbAllowToComeResp" Runat="server" CssClass="text"></asp:CheckBox></td>
							</tr>
							<tr>
								<td style="padding-left:30px" colspan="2"><asp:CheckBox ID="cbAllowToDeclineResp" Runat="server" CssClass="text"></asp:CheckBox></td>
							</tr>
							<tr>
								<td style="padding-left:30px" colspan="2"><asp:CheckBox ID="cbAllowToReassignResp" Runat="server" CssClass="text"></asp:CheckBox></td>
							</tr>
							<tr>
								<td style="padding-left:30px" colspan="2"><asp:CheckBox ID="cbReassignResp" Runat="server" CssClass="text"></asp:CheckBox></td>
							</tr>
						</table>
						<ibn:BHLWM id="bh3" runat="server"></ibn:BHLWM>
						<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
							<tr>
								<td style="padding-left:30px" colspan="2"><asp:CheckBox ID="cbAllowControl" Runat="server" CssClass="text" AutoPostBack="True"></asp:CheckBox></td>
							</tr>
							<tr id="trController1" runat="server">
								<td align="right" width="40%"><b><%=LocRM.GetString("tControllerType")%>:</b></td>
								<td><asp:DropDownList ID="ddContType" Runat="server" Width="200px" AutoPostBack="True"></asp:DropDownList></td>
							</tr>
							<tr id="trController" runat="server">
								<td align="right"><b><%=LocRM.GetString("tController")%>:</b></td>
								<td><asp:DropDownList ID="ddController" Runat="server" Width="200px"></asp:DropDownList></td>
							</tr>
						</table>
					</td>
					<td valign="top">
						<ibn:BHLWM id="bh5" runat="server"></ibn:BHLWM>
						<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
							<tr>
								<td align="right" width="40%"><b><%=LocRM.GetString("tExtType")%>:</b></td>
								<td><asp:DropDownList ID="ddExtActionType" Runat="server" Width="200px"></asp:DropDownList></td>
							</tr>
							<tr>
								<td colspan="2"><%=LocRM.GetString("tExtMessWarning")%></td>
							</tr>
							<tr id="tr2" runat="server">
								<td align="right" style="padding-top:15px;"><b><%=LocRM.GetString("tIntType")%>:</b></td>
								<td style="padding-top:15px;"><asp:DropDownList ID="ddIntActionType" Runat="server" Width="200px"></asp:DropDownList></td>
							</tr>
							<tr id="tr21" runat="server">
								<td colspan="2"><%=LocRM.GetString("tIntMessWarning")%></td>
							</tr>
						</table>
						<ibn:BHLWM id="bh4" runat="server"></ibn:BHLWM>
						<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
							<tr>
								<td style="padding-left:30px" colspan="2" class="text">
									<asp:CheckBox Runat="server" ID="cbAllowAutoReply" CssClass = "text">
									</asp:CheckBox>
									<br />
									<div style="padding-left:25px; padding-top:3px">
									<%=LocRM.GetString("tAllowAutoReplyHint1")%>
									
									<asp:HyperLink Runat="server" ID="hlAllowAutoReplyMessageLink" CssClass="text">
									</asp:HyperLink>
									
									<span class="text">
									<%=LocRM.GetString("tAllowAutoReplyHint2")%>
									</span>
									</div>
									<br />
									
									<asp:CheckBox Runat="server" ID="cbAllowAutoReplyClose" CssClass = "text">
									</asp:CheckBox>
									<br />
									<div style="padding-left:25px; padding-top:3px">
									<%=LocRM.GetString("tAllowAutoReplyCloseHint1")%>
									
									<asp:HyperLink Runat="server" ID="hlAllowAutoReplyCloseMessageLink" CssClass="text">
									</asp:HyperLink>
									
									<span class="text">
									<%=LocRM.GetString("tAllowAutoReplyCloseHint2")%>
									</span>
									</div>
									<br />
									
									<asp:CheckBox Runat="server" ID="cbAllowAutoSigning" CssClass="text"></asp:CheckBox>
									<div style="padding-left:25px; padding-top:3px">
									<%=LocRM.GetString("tAllowMessageAutoSigningHint1")%>
									<asp:HyperLink Runat="server" ID="hlAllowAutoSigningLink"></asp:HyperLink>
									<span class="text">
									<%=LocRM.GetString("tAllowMessageAutoSigningHint2")%>
									</span>
									</div>
									<br />
									<asp:CheckBox AutoPostBack="True" ID="cbAllowEMail" Runat="server" CssClass="text">
									</asp:CheckBox>
									<div style="padding-left:25px; padding-top:3px;color:Red;" runat="server" id="divNoIntBox">
									<%=LocRM.GetString("tNoIntBox")%>
									</div>
								</td>
							</tr>
							
							<tr id="tr3" runat="server">
								<td align="right" valign="top" style="padding-top:10px"><b><%=LocRM.GetString("tRecipients")%>:</b></td>
								<td valign="top">
									<table cellpadding="3" cellspacing="0" border="0">
										<tr>
											<td width="202px" valign="top"><asp:Label ID="lblRecipients" Runat="server" CssClass="text"></asp:Label></td>
											<td valign="top">
												<asp:Label CssClass="text" ID="lblChangeButton2" Runat="server"></asp:Label>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr valign="bottom">
					<td align="right" colspan="2">
						<btn:ImButton runat="server" class="text" ID="imbSave" style="width:110px"></btn:ImButton>&nbsp;
						<btn:ImButton runat="server" class="text" ID="imbCancel" style="width:110px" CausesValidation="false"></btn:ImButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<asp:Button id="btnRefresh" runat="server" CausesValidation="False" style="display:none;"></asp:Button>
<asp:Button id="btnRefresh2" runat="server" CausesValidation="False" style="display:none;"></asp:Button>