<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.WorksByCategory" Codebehind="WorksByCategory.ascx.cs" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ register TagPrefix="lst" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/BlockHeader.ascx" %>
<script language='javascript'>
	function ChangeOT(obj)
	{
		var objPrj = document.forms[0].<%=cbProjects.ClientID%>;
		if(objPrj)
			objPrj.checked = obj.checked;
		document.forms[0].<%=cbCalEntries.ClientID%>.checked = obj.checked;
		var objIss = document.forms[0].<%=cbIssues.ClientID%>;
		if(objIss)
			objIss.checked = obj.checked;
		document.forms[0].<%=cbDocs.ClientID%>.checked = obj.checked;
		var objTasks = document.forms[0].<%=cbTasks.ClientID%>;
		if(objTasks)
			objTasks.checked = obj.checked;
		document.forms[0].<%=cbToDo.ClientID%>.checked = obj.checked;
	}
</script>
<table class="ibn-stylebox text" style="MARGIN-TOP: 0px" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:BlockHeader id="secHeader" runat="server"></ibn:BlockHeader></td>
	</tr>
	<tr>
		<td>
			<table runat="server" id="FilterTable" class="ibn-navline ibn-alternating" cellspacing="0" cellpadding="5" width="100%" border="0">
				<tr>
					<td width="250px" valign="top">
						<table class="text">
							<tr height="30px">
								<td class="text" width="70px"><%=LocRM.GetString("tGroup")%>:&nbsp;</td>
								<td width="200px">
									<lst:indenteddropdownlist id="ddGroups" AutoPostBack="True" runat="server" Width="190px"></lst:indenteddropdownlist>
								</td>
							</tr>
							<tr height="30px">
								<td class="text" id="tdUser" runat="server"><%=LocRM.GetString("tUser")%>:</td>
								<td>
									<asp:DropDownList id="ddUser" runat="server" Width="190px"></asp:DropDownList>
								</td>
							</tr>
							<tr height="30px" id="trProject" runat="server">
								<td class="text"><%=LocRM.GetString("tProject")%>:</td>
								<td>
									<asp:DropDownList id="ddPrjs" runat="server" Width="190px"></asp:DropDownList>
								</td>
							</tr>
							<tr height="30px" id="trManager" runat="server">
								<td class="text"><%=LocRM.GetString("tManager")%>:</td>
								<td>
									<asp:DropDownList id="ddManager" runat="server" Width="190px"></asp:DropDownList>
								</td>
							</tr>
						</table>
					</td>
					<td width="180px" valign="top">
						<fieldset style="height:140px; margin-top:4">
							<legend class="text"><%=LocRM.GetString("State")%></legend>
							<table class="text" cellspacing="0" cellpadding="0" style="padding:4,0,0,4">
								<tr>
									<td><div style="margin-bottom:5"><%=LocRM.GetString("tShowCompleted")%>:</div>
										<asp:DropDownList id="ddCompleted" runat="server"	Width="160px"></asp:DropDownList>
									</td>
								</tr>
								<tr>
									<td><div style="margin-bottom:5"><%=LocRM.GetString("tActive")%>:</div>
										<asp:DropDownList id="ddShowActive" runat="server"	Width="160px"></asp:DropDownList>
									</td>
								</tr>
								<tr>
									<td><div style="margin-bottom:5"><%=LocRM.GetString("tShowUpcoming")%>:</div>
										<asp:DropDownList id="ddUpcoming" runat="server" Width="160px"></asp:DropDownList>
									</td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td width="150px" valign="top">
						<fieldset style="height:147px">
							<legend class="text"><asp:CheckBox ID="cbChkAll" onclick='javascript:ChangeOT(this)' Runat="server"></asp:CheckBox></legend>
							<table class="text" cellspacing="0">
								<tr>
									<td><asp:CheckBox ID="cbProjects" Runat="server"></asp:CheckBox></td>
								</tr>
								<tr>
									<td><asp:CheckBox ID="cbCalEntries" Runat="server"></asp:CheckBox></td>
								</tr>
								<tr>
									<td><asp:CheckBox ID="cbIssues" Runat="server"></asp:CheckBox></td>
								</tr>
								<tr>
									<td><asp:CheckBox ID="cbDocs" Runat="server"></asp:CheckBox></td>
								</tr>
								<tr>
									<td><asp:CheckBox ID="cbTasks" Runat="server"></asp:CheckBox></td>
								</tr>
								<tr>
									<td><asp:CheckBox ID="cbToDo" Runat="server"></asp:CheckBox></td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td height="100%">
						<table width="100%" height=100% cellspacing="0" cellpadding="0" style="margin-top:-5">
							<tr>
								<td align="right" valign="top">
									<asp:LinkButton ID="lbHideFilter" Runat="server" CssClass="text"></asp:LinkButton>
								</td>
							</tr>
							<tr>
								<td valign="bottom" align="right">
									<nobr><asp:Button ID="btnApplyF" Runat="server" CssClass="text" Width="70px"></asp:Button>
									<asp:Button ID="btnResetF" Runat="server" CssClass="text" Width="70px"></asp:Button></nobr>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<table id="tblFilterInfo" runat="server" class="ibn-navline text" cellspacing="0" cellpadding="4" width="100%" border="0">
				<tr>
					<td valign="top" style="padding-bottom:5">
						<table cellspacing="3" cellpadding="0" border="0" runat="server" id="tblFilterInfoSet" class="text">
						</table>
					</td>
					<td valign="bottom" align="right" height="100%">
						<table height="100%" cellspacing="0" cellpadding="0" style="margin-top:-5">
							<tr>
								<td valign="top" align="right">
									<asp:LinkButton ID="lbShowFilter" Runat="server" CssClass="text"></asp:LinkButton>
								</td>
							</tr>
							<tr>
								<td valign="bottom" style="padding-top:10px">
									<input class="text" id="btnVResetF" type="submit" runat="server" style="width:120px" NAME="btnVResetF"/>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<dg:datagridextended id="dgObjects" runat="server" width="100%" autogeneratecolumns="False" 
				borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="0" allowsorting="True" 
				pagesize="10" allowpaging="True" EnableViewState=false LayoutFixed=false>
				<Columns>
					<asp:BoundColumn Visible="False" DataField="ItemId"></asp:BoundColumn>
					<asp:BoundColumn Visible="False" DataField="StateId"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb4"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh4" Width="20px"></HeaderStyle>
						<ItemTemplate>
							<%# GetPriorityIcon
								(
									(int)Eval("ItemType"),
									(int)Eval("PriorityId"),
									Eval("PriorityName").ToString()
								)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Title">
						<ItemStyle CssClass="ibn-vb4"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh4"></HeaderStyle>
						<ItemTemplate>
							<%# GetTitle
								(
									(int)Eval("ItemId"),
									Eval("Title").ToString(),
									(int)Eval("ItemType"),
									Eval("GroupName"),
									(int)Eval("StateId"),
									(bool)Eval("IsOverdue"),
									(string)Eval("ItemCode")
								)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Type">
						<ItemStyle CssClass="ibn-vb4"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh4"></HeaderStyle>
						<ItemTemplate>
							<%# GetType
								(
									(int)Eval("ItemType"),
									(int)Eval("StateId"),
									(bool)Eval("IsOverdue")
								)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Resources">
						<ItemStyle CssClass="ibn-vb4"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh4"></HeaderStyle>
						<ItemTemplate>
							<%# GetResources
								(
									(int)Eval("CompletionTypeId"),
									(int)Eval("ItemType"),
									(int)Eval("ItemId"),
									(int)Eval("StateId"),
									(bool)Eval("IsOverdue")
								)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Completed">
						<ItemStyle CssClass="ibn-vb4" width="110px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh4" width="110px"></HeaderStyle>
						<ItemTemplate>
							<%# GetPercentCompleted
								(
									(int)Eval("ItemType"),
									(int)Eval("PercentCompleted"),
									(int)Eval("StateId"),
									(bool)Eval("IsOverdue")
								)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Start">
						<ItemStyle CssClass="ibn-vb4" width="80px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh4" width="80px"></HeaderStyle>
						<ItemTemplate>
							<%# GetDate
								(
									Eval("StartDate"),
									(int)Eval("StateId"),
									(bool)Eval("IsOverdue")
								)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Finish">
						<ItemStyle CssClass="ibn-vb4" width="80px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh4" width="80px"></HeaderStyle>
						<ItemTemplate>
							<%# GetDate
								(
									Eval("FinishDate"),
									(int)Eval("StateId"),
									(bool)Eval("IsOverdue")
								)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</dg:datagridextended>
		</td>
	</tr>
</table>