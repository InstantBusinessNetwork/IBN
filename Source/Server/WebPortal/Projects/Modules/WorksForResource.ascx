<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.WorksForResource" CodeBehind="WorksForResource.ascx.cs" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Reference Control="~/Common/Modules/LegendControl.ascx" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" Src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="ibn" TagName="LegendControl" Src="~/Common/Modules/LegendControl.ascx" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>

<script type="text/javascript">
	//<![CDATA[
	function ChangeOT(obj)
	{
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
	function CompleteItem(ItemType, ItemId)
	{
		document.forms[0].<%=hdnItemType.ClientID %>.value = ItemType;
		document.forms[0].<%=hdnItemId.ClientID %>.value = ItemId;
		if(confirm('<%=LocRM.GetString("CompleteWarning")%>'))
			<%=Page.ClientScript.GetPostBackEventReference(CompleteButton,"") %>
	}
	//]]>
</script>

<table runat="server" id="FilterTable" class="ibn-navline ibn-alternating" cellspacing="0" cellpadding="5" width="100%" border="0" Printable="0">
	<tr>
		<td width="250px" valign="top">
			<table class="text">
				<tr id="trPerson" runat="server">
					<td class="text">
						<%=LocRM.GetString("tPerson")%>:
					</td>
					<td>
						<asp:DropDownList ID="ddPerson" runat="server" Width="190">
						</asp:DropDownList>
					</td>
				</tr>
				<tr id="trProject" runat="server">
					<td class="text">
						<%=LocRM.GetString("tProject")%>:
					</td>
					<td>
						<asp:DropDownList ID="ddPrjs" runat="server" Width="190px">
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td class="text">
						<%=LocRM.GetString("tManager")%>:
					</td>
					<td>
						<asp:DropDownList ID="ddManager" runat="server" Width="190px">
						</asp:DropDownList>
					</td>
				</tr>
				<tr runat="server" id="trClient" >
					<td class="text">
						<%=LocRM.GetString("Client")%>:
					</td>
					<td>
						<ibn:EntityDD ID="ClientControl" ObjectTypes="Contact,Organization" runat="server" Width="190px" />
					</td>
				</tr>
				<tr runat="server" id="trCategory">
					<td class="text">
						<%=LocRM.GetString("Category")%>:
					</td>
					<td>
						<asp:DropDownList ID="ddCategory" runat="server" Width="190px">
						</asp:DropDownList>
					</td>
				</tr>
			</table>
		</td>
		<td width="180px" valign="top">
			<fieldset style="height: 137px; margin-top: 4px;">
				<legend class="text">
					<%=LocRM.GetString("State")%></legend>
				<table class="text" cellspacing="0" cellpadding="0" style="padding: 4px 0px 0px 4px;">
					<tr>
						<td>
							<div style="margin-bottom: 5px;">
								<%=LocRM.GetString("tShowCompleted")%>:</div>
							<asp:DropDownList ID="ddCompleted" runat="server" Width="160px">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td>
							<div style="margin-bottom: 5px">
								<%=LocRM.GetString("tActive")%>:</div>
							<asp:DropDownList ID="ddShowActive" runat="server" Width="160px">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td>
							<div style="margin-bottom: 5px">
								<%=LocRM.GetString("tShowUpcoming")%>:</div>
							<asp:DropDownList ID="ddUpcoming" runat="server" Width="160px">
							</asp:DropDownList>
						</td>
					</tr>
				</table>
			</fieldset>
		</td>
		<td width="150px" valign="top">
			<fieldset style="height: 140px">
				<legend class="text">
					<asp:CheckBox ID="cbChkAll" onclick='javascript:ChangeOT(this)' runat="server"></asp:CheckBox></legend>
				<table class="text" cellspacing="0">
					<tr>
						<td>
							<asp:CheckBox ID="cbCalEntries" runat="server"></asp:CheckBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:CheckBox ID="cbIssues" runat="server"></asp:CheckBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:CheckBox ID="cbDocs" runat="server"></asp:CheckBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:CheckBox ID="cbTasks" runat="server"></asp:CheckBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:CheckBox ID="cbToDo" runat="server"></asp:CheckBox>
						</td>
					</tr>
				</table>
			</fieldset>
		</td>
		<td valign="top" align="right">
			<asp:LinkButton ID="lbHideFilter" runat="server" CssClass="text"></asp:LinkButton>
		</td>
	</tr>
	<tr>
		<td colspan="4" align="right">
			<asp:Button ID="btnApplyF" Runat="server" CssClass="text" Width="75px"></asp:Button>
			<asp:Button ID="btnResetF" Runat="server" CssClass="text" Width="75px"></asp:Button>
		</td>
	</tr>
</table>
<table id="tblFilterInfo" runat="server" class="ibn-navline text" cellspacing="0" cellpadding="4" width="100%" border="0" Printable="0">
	<tr>
		<td valign="top" style="padding-bottom: 5px">
			<table cellspacing="3" cellpadding="0" border="0" runat="server" id="tblFilterInfoSet" class="text">
			</table>
		</td>
		<td valign="bottom" align="right" height="100%">
			<table cellspacing="0" cellpadding="0" style="height: 100%; margin-top: -5px">
				<tr>
					<td valign="top" align="right">
						<asp:LinkButton ID="lbShowFilter" runat="server" CssClass="text"></asp:LinkButton>
					</td>
				</tr>
				<tr>
					<td valign="bottom" style="padding-top: 10px">
						<input class="text" id="btnVResetF" type="submit" runat="server" style="width: 120px" name="btnVResetF" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<dg:DataGridExtended ID="dgObjects" runat="server" Width="100%" AutoGenerateColumns="False" BorderWidth="0px" CellSpacing="0" GridLines="None" CellPadding="0" AllowSorting="True" PageSize="10" AllowPaging="True" EnableViewState="false" LayoutFixed="false">
	<Columns>
		<asp:BoundColumn Visible="False" DataField="ItemId"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="StateId"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="ItemType"></asp:BoundColumn>
		<asp:TemplateColumn SortExpression="PriorityId">
			<ItemStyle CssClass="ibn-vb4"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4" Width="25px"></HeaderStyle>
			<ItemTemplate>
				<%# GetPriorityIcon((int)Eval("ItemType"), (int)Eval("PriorityId"), Eval("PriorityName").ToString()) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Title" SortExpression="Title">
			<ItemStyle CssClass="ibn-vb4"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4"></HeaderStyle>
			<ItemTemplate>
				<%# GetTitle((int)Eval("ItemId"), Eval("Title").ToString(), (int)Eval("ItemType"), Eval("GroupName"),
										(int)Eval("StateId"), (bool)Eval("IsOverdue"), (bool)Eval("IsNewMessage"), (int)Eval("ProjectId"))%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Type">
			<ItemStyle CssClass="ibn-vb4"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4"></HeaderStyle>
			<ItemTemplate>
				<%# GetType((int)Eval("ItemType"), (int)Eval("StateId"), (bool)Eval("IsOverdue")) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Project">
			<ItemStyle CssClass="ibn-vb4"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4"></HeaderStyle>
			<ItemTemplate>
				<%# GetProject((int)Eval("ItemType"), (int)Eval("ProjectId"), Eval("ProjectTitle").ToString(), (int)Eval("StateId"), (bool)Eval("IsOverdue")) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Manager" SortExpression="ManagerId">
			<ItemStyle CssClass="ibn-vb4"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4"></HeaderStyle>
			<ItemTemplate>
				<%# GetManager((int)Eval("ItemType"), (int)Eval("ManagerId"), (int)Eval("StateId"), (bool)Eval("IsOverdue")) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Completed" SortExpression="PercentCompleted">
			<ItemStyle CssClass="ibn-vb4" Width="110px"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4" Width="110px"></HeaderStyle>
			<ItemTemplate>
				<%# GetPercentCompleted((int)Eval("ItemType"), (int)Eval("PercentCompleted"), (int)Eval("StateId"), (bool)Eval("IsOverdue")) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Start" SortExpression="StartDate">
			<ItemStyle CssClass="ibn-vb4" Width="80px"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4" Width="80px"></HeaderStyle>
			<ItemTemplate>
				<%# GetDate(Eval("StartDate"), (int)Eval("StateId"), (bool)Eval("IsOverdue")) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Finish" SortExpression="FinishDate">
			<ItemStyle CssClass="ibn-vb4" Width="80px"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4" Width="80px"></HeaderStyle>
			<ItemTemplate>
				<%# GetDate(Eval("FinishDate"), (int)Eval("StateId"), (bool)Eval("IsOverdue")) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="ActualStart" SortExpression="ActualStartDate">
			<ItemStyle CssClass="ibn-vb4" Width="110px"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4" Width="110px"></HeaderStyle>
			<ItemTemplate>
				<%# GetDate(Eval("ActualStartDate"), (int)Eval("StateId"), (bool)Eval("IsOverdue")) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="ActualFinish" SortExpression="ActualFinishDate">
			<ItemStyle CssClass="ibn-vb4" Width="110px"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4" Width="110px"></HeaderStyle>
			<ItemTemplate>
				<%# GetDate(Eval("ActualFinishDate"), (int)Eval("StateId"), (bool)Eval("IsOverdue")) %>
				<%# GetProjectDate((int)Eval("ItemId"), (int)Eval("ProjectId")) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Task Time" SortExpression="TaskTime">
			<ItemStyle CssClass="ibn-vb4" Width="120px"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4" Width="120px"></HeaderStyle>
			<ItemTemplate>
				<%# GetMinutes(Eval("TaskTime"), (int)Eval("StateId"), (bool)Eval("IsOverdue")) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Total Minutes" SortExpression="TotalMinutes">
			<ItemStyle CssClass="ibn-vb4" Width="120px"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4" Width="120px"></HeaderStyle>
			<ItemTemplate>
				<%# GetMinutes(Eval("TotalMinutes"), (int)Eval("StateId"), (bool)Eval("IsOverdue")) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Total Approved" SortExpression="TotalApproved">
			<ItemStyle CssClass="ibn-vb4" Width="120px"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4" Width="120px"></HeaderStyle>
			<ItemTemplate>
				<%# GetMinutes(Eval("TotalApproved"), (int)Eval("StateId"), (bool)Eval("IsOverdue"))%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<ItemStyle CssClass="ibn-vb4" Width="26px"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4" Width="26px"></HeaderStyle>
			<ItemTemplate>
				<span Printable="0"><%# GetEditString((int)Eval("ItemType"), (int)Eval("ItemId"))%></span>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<ItemStyle CssClass="ibn-vb4" Width="26px"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4" Width="26px"></HeaderStyle>
			<ItemTemplate>
				<a id="ibComplete" runat="server" Printable="0"
					href='<%#"javascript:CompleteItem(" + Eval("ItemType").ToString() + "," + Eval("ItemId").ToString() + ")"%>' 
					visible='<%# CanComplete((int)Eval("ItemType"), (int)Eval("PercentCompleted"), (int)Eval("StateId")) %>'><img 
					alt="" title="<%=LocRM.GetString("Complete")%>" src="<%=Page.ResolveUrl("~/Layouts/Images/accept.gif")%>" /></a>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</dg:DataGridExtended>
<asp:DataGrid ID="dgExport" runat="server" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" EnableViewState="False" Visible="False" ItemStyle-HorizontalAlign="Left" HeaderStyle-Font-Bold="True">
	<Columns>
		<asp:BoundColumn DataField="Title" HeaderText="Title"></asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Type">
			<ItemStyle CssClass="ibn-vb4"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4"></HeaderStyle>
			<ItemTemplate>
				<%# GetType((int)Eval("ItemType"), 1, false) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="ProjectTitle" HeaderText="Project"></asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Manager">
			<ItemStyle CssClass="ibn-vb4"></ItemStyle>
			<HeaderStyle CssClass="ibn-vh4"></HeaderStyle>
			<ItemTemplate>
				<%# CHelper.GetUserName((int)Eval("ManagerId"))%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="PercentCompleted" HeaderText="Completed"></asp:BoundColumn>
		<asp:BoundColumn DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
		<asp:BoundColumn DataField="FinishDate" HeaderText="End Date" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
		<asp:BoundColumn DataField="ActualStartDate" HeaderText="Actual Start" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
		<asp:BoundColumn DataField="ActualFinishDate" HeaderText="Actual End" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
		<asp:BoundColumn DataField="TaskTime" HeaderText="Task Time"></asp:BoundColumn>
		<asp:BoundColumn DataField="TotalMinutes" HeaderText="Total Minutes"></asp:BoundColumn>
		<asp:BoundColumn DataField="TotalApproved" HeaderText="Total Approved"></asp:BoundColumn>
	</Columns>
</asp:DataGrid>
<asp:LinkButton ID="lbExport" runat="server" Visible="False"></asp:LinkButton>
<asp:LinkButton ID="CompleteButton" runat="server" Visible="False"></asp:LinkButton>
<input id="hdnItemId" type="hidden" runat="server" />
<input id="hdnItemType" type="hidden" runat="server" />
<asp:LinkButton ID="lbChangeViewDef" runat="server" Visible="false"></asp:LinkButton>
<asp:LinkButton ID="lbChangeViewDates" runat="server" Visible="false"></asp:LinkButton>
<asp:LinkButton ID="lbChangeViewTimes" runat="server" Visible="false"></asp:LinkButton>
<asp:Button ID="btnApplyG" runat="server" Visible="false" CssClass="text"></asp:Button>
<div style="padding-bottom: 5px;" Printable="0">
	<ibn:LegendControl runat="server" ID="MainLegendControl"></ibn:LegendControl>
</div>
