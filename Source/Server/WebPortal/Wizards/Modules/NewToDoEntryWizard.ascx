<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Wizards.Modules.NewToDoEntryWizard"
	CodeBehind="NewToDoEntryWizard.ascx.cs" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ register TagPrefix="lst" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>

<script language="javascript">
<!--

	function SaveManagerId()
	{

		var cManagerSelect = document.forms[0].all.<%=ddlManager.ClientID%>;
		if(cManagerSelect != null)
		{
			document.forms[0].all.<%=txtManagerId.ClientID%>.value = cManagerSelect.value;
		}
		return true;	
	}


//-->
</script>

<asp:Panel ID="step1" runat="server">
	<div class="text">
		<%=LocRM.GetString("s1TopText" + sUnit) %></div>
	<br>
	<table cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td valign="top" width="50%">
				<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 5px;
					padding-top: 3px">
					<%=LocRM.GetString("s1ToDoEntryTitle") %>
					<%=LocRM.GetString(sUnit+"Quest") %></div>
				<asp:TextBox ID="tbTitle" runat="server" Width="260"></asp:TextBox>
				<asp:RequiredFieldValidator ID="s1rfTitle" runat="server" Display="Dynamic" ErrorMessage="*"
					ControlToValidate="tbTitle"></asp:RequiredFieldValidator>
				<div id="divPriority" runat="server">
					<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 5px;
						padding-top: 7px">
						<%=LocRM.GetString("s1Priority") %>
						<%=LocRM.GetString(sUnit) %>:</div>
					<asp:DropDownList ID="ddPriority" runat="server" Width="260">
					</asp:DropDownList>
				</div>
				<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 5px;
					padding-top: 7px">
					<%=LocRM.GetString("s1Description") %>
					<%=LocRM.GetString(sUnit+"Quest") %></div>
				<asp:TextBox ID="tbDescription" runat="server" Width="260" TextMode="MultiLine" Height="75px"></asp:TextBox>
			</td>
			<td valign="top">
				<div id="s1EntryProperties" runat="server">
					<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 5px;
						padding-top: 3px">
						<%=LocRM.GetString("s1EntryType") %>
						<%=LocRM.GetString(sUnit+"Quest") %></div>
					<asp:DropDownList ID="ddEntryType" runat="server" Width="260">
					</asp:DropDownList>
					<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 5px;
						padding-top: 7px">
						<%=LocRM.GetString("s1Location") %>
						<%=LocRM.GetString(sUnit+"Quest") %></div>
					<asp:TextBox ID="tbLocation" runat="server" Width="260"></asp:TextBox></div>
				<div id="s1ToDoProperties" runat="server">
					<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 5px;
						padding-top: 7px">
						<%=LocRM.GetString("s1ToDoCompletionType") %>
						<%=LocRM.GetString(sUnit) %>:</div>
					<asp:DropDownList ID="ddToDoCompletionType" runat="server" Width="260">
					</asp:DropDownList>
					<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 5px;
						padding-top: 7px">
						<asp:CheckBox ID="chbToDoMustBeConfirmed" runat="server" CssClass="cb"></asp:CheckBox><%=LocRM.GetString("s1ToDoMustBeConfirmed") %></div>
				</div>
				<br>
				<table cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td valign="top" align="middle" width="60">
						</td>
						<td class="text" style="padding-right: 15px" valign="top">
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step2" runat="server">
	<table cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td valign="top" width="50%">
				<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 5px;
					padding-top: 7px">
					<%=LocRM.GetString("s2Project") %>
					<%=LocRM.GetString(sUnit) %>:</div>
				<asp:DropDownList ID="ddProject" runat="server" Width="260" AutoPostBack="True" OnSelectedIndexChanged="ddProject_SelectedIndexChanged">
				</asp:DropDownList>
				<div id="s2ToDoManager" runat="server">
					<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 5px;
						padding-top: 7px">
						<%=LocRM.GetString("s2Manager") %>:</div>
					<asp:DropDownList ID="ddlManager" Width="200" runat="server" CssClass="text">
					</asp:DropDownList>
					<asp:Label ID="lblManager" runat="server" CssClass="text"></asp:Label></div>
			</td>
			<td style="padding-top: 3px" valign="top" align="middle" width="60">
				<img alt="" src="../layouts/images/quicktip.gif" border="0">
			</td>
			<td class="text" style="padding-right: 15px; padding-top: 3px" valign="top">
				<%=LocRM.GetString("s2QuickTip"+sUnit) %>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step3" runat="server">
	<table cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td valign="top" width="50%">
				<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 5px;
					padding-top: 7px">
					<%=LocRM.GetString("s3StartDate") %></div>
				<mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" TimeCssClass="text"
					DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" />
				<br />
				<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 5px;
					padding-top: 7px">
					<%=LocRM.GetString("s3EndDate") %></div>
				<mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" TimeCssClass="text"
					DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" />
				<br />
				<asp:CustomValidator ID="CustomValidator1" Display="Dynamic" ErrorMessage="Wrong date"
					runat="server"></asp:CustomValidator>
			</td>
			<td>
				<table cellpadding="2" cellspacing="2" border="0" runat="server" id="DateTimeCommentForToDo">
					<tr>
						<td valign="top" align="middle" width="60">
							<img alt="" src="../layouts/images/quicktip.gif" border="0">
						</td>
						<td class="text" style="padding-right: 15px; padding-top: 7px" valign="top">
							<%=LocRM.GetString("s3QuickTip") %>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step4" runat="server">
	<table class="text" style="padding-top: 3px" width="100%">
		<tr>
			<td width="25" align="left">
				<img height="20" src="../layouts/images/help.gif" width="20" align="left">
			</td>
			<td>
				<%=LocRM.GetString("s4TopText1"+sUnit)%>.
			</td>
		</tr>
	</table>
	<table class="text" cellspacing="0" cellpadding="2" width="100%" border="0">
		<tr>
			<td class="ibn-navframe" width="250">
				<asp:RadioButton ID="s4OnlyForMe" runat="server" CssClass="subHeader" AutoPostBack="True"
					Checked="True" GroupName="radiobuttons" OnCheckedChanged="s4AssignTeam_CheckedChanged">
				</asp:RadioButton>
			</td>
			<td width="4">
				&nbsp;
			</td>
			<td class="">
				<asp:RadioButton ID="s4AssignTeam" runat="server" CssClass="subHeader" AutoPostBack="True"
					GroupName="radiobuttons" OnCheckedChanged="s4AssignTeam_CheckedChanged"></asp:RadioButton>
			</td>
		</tr>
	</table>
	<table id="TeamTable" cellspacing="0" cellpadding="0" width="100%" runat="server">
		<tr>
			<td width="8">
				&nbsp;
			</td>
			<td>
				<fieldset>
					<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
						<tr>
							<td class="ibn-navframe boldtext" style="padding-left: 10px" width="250">
								<b>
									<%=LocRM.GetString("s4Selected") %>:</b>
							</td>
							<td width="25">
								&nbsp;
							</td>
							<td class="boldtext">
								<b>
									<%=LocRM.GetString("s4Available") %>:</b>
							</td>
						</tr>
						<tr>
							<td class="ibn-navframe" style="padding-left: 10px" valign="top" width="250">
								<!-- Data GRID -->
								<div style="overflow-y: auto; height: 180px">
									<asp:DataGrid ID="dgMembers" runat="server" Width="200" BorderWidth="0px" CellSpacing="0"
										GridLines="None" CellPadding="0" AllowSorting="False" AllowPaging="False" AutoGenerateColumns="False">
										<ItemStyle CssClass="text"></ItemStyle>
										<HeaderStyle CssClass="text"></HeaderStyle>
										<Columns>
											<asp:BoundColumn DataField="MustBeConfirmed" Visible="False"></asp:BoundColumn>
											<asp:BoundColumn DataField="UserId" Visible="False"></asp:BoundColumn>
											<asp:TemplateColumn HeaderText='Name'>
												<ItemTemplate>
													<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL((int)DataBinder.Eval(Container.DataItem, "UserId"))%>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn ItemStyle-Width="30" Visible="True">
												<ItemTemplate>
													<asp:ImageButton ID="ibDelete" runat="server" BorderWidth="0" Width="16" Height="16"
														ImageUrl="../../layouts/images/DELETE.GIF" CommandName="Delete" CausesValidation="False">
													</asp:ImageButton>
												</ItemTemplate>
											</asp:TemplateColumn>
										</Columns>
									</asp:DataGrid><!-- End Data GRID --></div>
							</td>
							<td width="25">
								&nbsp;
							</td>
							<td valign="top">
								<!-- Groups & Users -->
								<table class="text" style="margin-top: -5px" cellspacing="0" cellpadding="3" width="100%"
									border="0">
									<tr id="s4GroupRow" runat="server">
										<td style="height: 18px" width="9%">
											<%=LocRM.GetString("s4Group") %>:
										</td>
										<td style="height: 18px" width="91%">
											<lst:IndentedDropDownList ID="ddGroups" Width="190px" runat="server" CssClass="text"
												AutoPostBack="True" OnSelectedIndexChanged="ddGroups_ChangeGroup">
											</lst:IndentedDropDownList>
										</td>
									</tr>
									<tr id="s4SearchRow" runat="server">
										<td width="9%">
											<%=LocRM.GetString("s4Search") %>:
										</td>
										<td width="91%">
											<asp:TextBox ID="tbSearch" Width="125px" runat="server" CssClass="text"></asp:TextBox>
											<asp:Button ID="btnSearch" Width="60px" runat="server" CssClass="text" CausesValidation="False"
												OnClick="btnSearch_Click"></asp:Button>
										</td>
									</tr>
									<tr>
										<td valign="top">
											<%=LocRM.GetString("s4User") %>:
										</td>
										<td valign="top">
											<asp:ListBox ID="lbUsers" Width="190px" runat="server" CssClass="text" Rows="4" SelectionMode="Multiple">
											</asp:ListBox>
										</td>
									</tr>
									<tr>
										<td valign="top">
										</td>
										<td valign="top">
											<asp:CheckBox ID="chbMustBeConfirmed" runat="server"></asp:CheckBox>
										</td>
									</tr>
									<tr>
										<td valign="top">
											&nbsp;
										</td>
										<td>
											<button class="text" id="btnAdd" style="width: 160px" type="button" runat="server"
												onserverclick="btnAdd_Click">
											</button>
											<br>
											<button class="text" id="btnAddGroup" style="width: 160px" type="button" runat="server"
												onserverclick="btnAddGroup_Click">
											</button>
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
				</fieldset>
				<div>
				</div>
			</td>
			<td width="8">
				&nbsp;
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step5" runat="server" Visible="False">
	<table class="text" style="padding-top: 3px" width="100%">
		<tr>
			<td width="25px">
				<img height="20" src="../layouts/images/help.gif" width="20" align="absMiddle">
			</td>
			<td>
				<%=LocRM.GetString("s5TopText") %>
			</td>
		</tr>
	</table>
	<br>
	<table cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td valign="top" width="350%">
				<table cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td valign="top">
							<table class="subHeader" cellspacing="0" cellpadding="5" width="100%" border="0">
								<tr>
									<td width="100">
										<b>
											<%=LocRM.GetString("s5ToDoEntryTitle") %>:</b>
									</td>
									<td>
										<asp:Label ID="lblToDoEntryTitle" runat="server"></asp:Label>
									</td>
								</tr>
								<tr>
									<td>
										<b>
											<%=LocRM.GetString("s5ToDoEntryPriority") %>:</b>
									</td>
									<td>
										<asp:Label ID="lblPriority" runat="server"></asp:Label>
									</td>
								</tr>
								<tr id="trProjectTitle" runat="server">
									<td>
										<b>
											<%=LocRM.GetString("s5ProjectTitle") %>:</b>
									</td>
									<td>
										<asp:Label ID="lblProjectTitle" runat="server"></asp:Label>
									</td>
								</tr>
								<tr id="trManager" runat="server">
									<td>
										<b>
											<%=LocRM.GetString("s5Manager") %>:</b>
									</td>
									<td>
										<asp:Label ID="lblManagerName" runat="server"></asp:Label>
									</td>
								</tr>
								<tr id="s5RowForToDo1" runat="server">
									<td>
										<b>
											<%=LocRM.GetString("s5CompletionType") %>:</b>
									</td>
									<td>
										<asp:Label ID="lblCompletionType" runat="server"></asp:Label>
									</td>
								</tr>
								<tr id="s5RowForToDo2" runat="server">
									<td>
									</td>
									<td>
										<%=LocRM.GetString("s5MustBeConfirmed") %>
									</td>
								</tr>
								<tr id="s5RowForEntry1" runat="server">
									<td>
										<b>
											<%=LocRM.GetString("s5Type") %>:</b>
									</td>
									<td>
										<asp:Label ID="lblEntryType" runat="server"></asp:Label>
									</td>
								</tr>
								<tr id="s5RowForEntry2" runat="server">
									<td>
										<b>
											<%=LocRM.GetString("s5Location") %>:</b>
									</td>
									<td>
										<asp:Label ID="lblLocation" runat="server"></asp:Label>
									</td>
								</tr>
								<tr>
									<td valign="top">
										<b>
											<%=LocRM.GetString("s5Description") %>:</b>
									</td>
									<td>
										<div style="overflow-y: auto; height: 40px">
											<asp:Label ID="lblDescription" runat="server"></asp:Label></div>
									</td>
								</tr>
							</table>
						</td>
						<td valign="top" width="230">
							<table class="subHeader" cellspacing="0" cellpadding="5" width="230" border="0">
								<tr id="s5StartDateRow" runat="server">
									<td width="80">
										<b>
											<%=LocRM.GetString("s5StartDate") %>:</b>
									</td>
									<td width="150">
										<asp:Label ID="lblStartDate" runat="server"></asp:Label>
									</td>
								</tr>
								<tr id="s5EndDateRow" runat="server">
									<td>
										<b>
											<%=LocRM.GetString("s5EndDate") %>:</b>
									</td>
									<td>
										<asp:Label ID="lblEndDate" runat="server"></asp:Label>
									</td>
								</tr>
								<tr id="s5OnlyRow" runat="server">
									<td colspan="2">
										<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 10px;
											padding-top: 3px">
											<b>
												<%=LocRM.GetString("s5OnlyForMe") %></b></div>
									</td>
								</tr>
								<tr id="s5TeamRow" runat="server">
									<td colspan="2">
										<div class="subHeader" style="padding-right: 0px; padding-left: 0px; padding-bottom: 10px;
											padding-top: 3px">
											<b>
												<%=LocRM.GetString("s5Team"+sUnit) %>:</b></div>
										<div style="overflow-y: auto; height: 115px">
											<asp:DataList ID="dlTeam" runat="server" CssClass="subTitle">
												<ItemTemplate>
													<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL((int)DataBinder.Eval(Container.DataItem, "UserId"))%>
												</ItemTemplate>
											</asp:DataList></div>
										<br>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step6" runat="server" Visible="False">
	<div class="text">
		<img height="20" src="../layouts/images/help.gif" width="20" align="absMiddle">&nbsp;<%=LocRM.GetString("s6TopText") %></div>
	<br>
	<div class="SubHeader">
		<b>
			<%=LocRM.GetString("s6TextHeader") %></b><br>
		<br>
	</div>
	<div class="SubHeader" style="padding-left: 20px">
		<%=LocRM.GetString("s6Text") %></div>
</asp:Panel>
<input id="txtManagerId" type="hidden" runat="server">
