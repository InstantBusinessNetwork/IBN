<%@ Reference Control="~/Modules/Separator1.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.ChangeRoles" Codebehind="ChangeRoles.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="~/Modules/Separator1.ascx"%>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx"%>
<script language="javascript" type="text/javascript">
<!--
function DeleteProject(ProjectId)
{
	document.forms[0].<%=hdnProjectId.ClientID %>.value = ProjectId;
	if(confirm('<%=LocRM.GetString("tWarningProject")%>'))
		<%=Page.ClientScript.GetPostBackEventReference(lblDeleteProjectAll,"") %>
}

function DeleteToDo(ToDoId)
{
	document.forms[0].<%=hdnToDoId.ClientID %>.value = ToDoId;
	if(confirm('<%=LocRM.GetString("tWarningTodo")%>'))
		<%=Page.ClientScript.GetPostBackEventReference(lbDeleteToDoAll,"") %>
}

function DeleteTask(TaskId)
{
	document.forms[0].<%=hdnTaskId.ClientID %>.value = TaskId;
	if(confirm('<%=LocRM.GetString("tWarningTask")%>'))
		<%=Page.ClientScript.GetPostBackEventReference(lblDeleteTaskAll,"") %>
}

function DeleteEvent(EventId)
{
	document.forms[0].<%=hdnEventId.ClientID %>.value = EventId;
	if(confirm('<%=LocRM.GetString("tWarningEvent")%>'))
		<%=Page.ClientScript.GetPostBackEventReference(lbEventDeleteAll,"") %>
}

function DeleteIncident(IncidentId)
{
	document.forms[0].<%=hdnIncidentId.ClientID %>.value = IncidentId;
	if(confirm('<%=LocRM.GetString("tWarningIssue")%>'))
		<%=Page.ClientScript.GetPostBackEventReference(lbIncidentDeleteAll,"") %>
}

function DeleteDocument(DocumentId)
{
	document.forms[0].<%=hdnDocumentId.ClientID %>.value = DocumentId;
	if(confirm('<%=LocRM.GetString("tWarningDocument")%>'))
		<%=Page.ClientScript.GetPostBackEventReference(lbDocumentDeleteAll,"") %>
}
//-->
</script>
<table class="ibn-stylebox" style="MARGIN-TOP: 0px; margin-left:2px" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="tbHeader" title="" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<!-- Projects -->
			<ibn:sep id="sep1" title="" runat="server" IsClickable="false"></ibn:sep>
			<asp:datagrid id="dgPrj" ShowHeader="true" cellpadding="2" borderwidth="0px" GridLines="None" Width="100%" PagerStyle-HorizontalAlign="Right" PagerStyle-Visible="true" CellSpacing="0" AllowSorting="False" AutoGenerateColumns="False" PagerStyle-Mode="NumericPages" EnableViewState="False" Runat="server">
				<itemstyle cssclass="ibn-vb2"></itemstyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<itemstyle cssclass="ibn-vb2 ibn-propertysheet"></itemstyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetProjectStatus (
								(int)DataBinder.Eval(Container.DataItem, "ProjectId"),
								(string)DataBinder.Eval(Container.DataItem, "Title"),
								(int)DataBinder.Eval(Container.DataItem, "StatusId")
								)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderStyle-Width="270px">
						<itemstyle cssclass="ibn-vb2" Width="270"></itemstyle>
						<ItemTemplate>
							<%# GetProjectRole
								(
									(int)DataBinder.Eval(Container.DataItem, "IsManager"),
									(int)DataBinder.Eval(Container.DataItem, "IsExecutiveManagerId"),
									(int)DataBinder.Eval(Container.DataItem, "IsTeamMember"),
									(int)DataBinder.Eval(Container.DataItem, "IsSponsor"),
									(int)DataBinder.Eval(Container.DataItem, "IsStakeholder")
								)
								%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn ItemStyle-Width="75" HeaderStyle-Width="75">
						<itemstyle cssclass="ibn-vb2"></itemstyle>
						<ItemTemplate>
							<asp:HyperLink Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanView")) %>' ImageUrl = "../../layouts/images/icon-search.GIF" NavigateUrl='<%# "~/Projects/ProjectView.aspx?ProjectID=" + DataBinder.Eval(Container.DataItem, "ProjectID").ToString() %>' Runat="server" ToolTip='<%#LocRM.GetString("View") %>' ID="Hyperlink2" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:HyperLink style='<%# (GetBool((int)DataBinder.Eval(Container.DataItem, "CanEdit")))? "":"visibility:hidden;" %>' ImageUrl = "../../layouts/images/Edit.GIF" NavigateUrl='<%# "~/Projects/ProjectEdit.aspx?ProjectID=" + DataBinder.Eval(Container.DataItem, "ProjectID").ToString() %>' Runat="server" ToolTip='<%#LocRM.GetString("Edit") %>' ID="Hyperlink1" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:HyperLink id="ibDelete" runat="server" imageurl="../../layouts/images/DELETE.GIF" Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanDelete")) %>' NavigateUrl='<%# "javascript:DeleteProject(" + DataBinder.Eval(Container.DataItem, "ProjectId").ToString() + ")" %>' ToolTip='<%#LocRM.GetString("Delete") %>'>
							</asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid>
			<!-- ToDos -->
			<ibn:sep id="sep2" title="" runat="server" IsClickable="false"></ibn:sep>
			<asp:datagrid id="dgToDo" ShowHeader="true" cellpadding="2" borderwidth="0px" GridLines="None" Width="100%" PagerStyle-HorizontalAlign="Right" PagerStyle-Visible="true" CellSpacing="0" AllowSorting="False" AutoGenerateColumns="False" PagerStyle-Mode="NumericPages" EnableViewState="False" Runat="server">
				<itemstyle cssclass="ibn-vb2"></itemstyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<itemstyle cssclass="ibn-vb2 ibn-propertysheet"></itemstyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetTaskToDoLink (
								(int)DataBinder.Eval(Container.DataItem, "ToDoId"),
								1,
								DataBinder.Eval(Container.DataItem, "Title").ToString(),
								(int)DataBinder.Eval(Container.DataItem, "StateId")
								)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderStyle-Width="270px">
						<itemstyle cssclass="ibn-vb2" Width="270"></itemstyle>
						<ItemTemplate>
							<%# GetToDoRole
								(
									(int)DataBinder.Eval(Container.DataItem, "IsManager"),
									(int)DataBinder.Eval(Container.DataItem, "IsResource")
								)
								%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn ItemStyle-Width="75" HeaderStyle-Width="75">
						<itemstyle cssclass="ibn-vb2"></itemstyle>
						<ItemTemplate>
							<asp:HyperLink Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanView")) %>' ImageUrl = "../../layouts/images/icon-search.GIF" NavigateUrl='<%# "~/ToDo/ToDoView.aspx?ToDoID=" + DataBinder.Eval(Container.DataItem, "ToDoId").ToString() %>' Runat="server" ToolTip='<%#LocRM.GetString("View") %>' ID="Hyperlink3" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:HyperLink style='<%# (GetBool((int)DataBinder.Eval(Container.DataItem, "CanEdit")))? "":"visibility:hidden;" %>' ImageUrl = "../../layouts/images/Edit.GIF" NavigateUrl='<%# "~/ToDo/ToDoEdit.aspx?ToDoID=" + DataBinder.Eval(Container.DataItem, "ToDoId").ToString() %>' Runat="server" ToolTip='<%#LocRM.GetString("Edit") %>' ID="Hyperlink4" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:HyperLink id="ibDelete" runat="server" imageurl="../../layouts/images/DELETE.GIF" Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanDelete")) %>' NavigateUrl='<%# "javascript:DeleteToDo(" + DataBinder.Eval(Container.DataItem, "ToDoId").ToString() + ")" %>' ToolTip='<%#LocRM.GetString("Delete") %>'>
							</asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid>
			<!-- Tasks -->
			<ibn:sep id="sep3" title="" runat="server" IsClickable="false"></ibn:sep>
			<asp:datagrid id="dgTasks" ShowHeader="true" cellpadding="2" borderwidth="0px" GridLines="None" Width="100%" PagerStyle-HorizontalAlign="Right" PagerStyle-Visible="true" CellSpacing="0" AllowSorting="False" AutoGenerateColumns="False" PagerStyle-Mode="NumericPages" EnableViewState="False" Runat="server">
				<itemstyle cssclass="ibn-vb2"></itemstyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<itemstyle cssclass="ibn-vb2 ibn-propertysheet"></itemstyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetTaskToDoLink (
								(int)DataBinder.Eval(Container.DataItem, "TaskId"),
								0,
								DataBinder.Eval(Container.DataItem, "Title").ToString(),
								(int)DataBinder.Eval(Container.DataItem, "StateId")
								)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderStyle-Width="270px">
						<itemstyle cssclass="ibn-vb2" Width="270"></itemstyle>
						<ItemTemplate>
							<%# GetToDoRole(0,(int)DataBinder.Eval(Container.DataItem, "IsResource"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn ItemStyle-Width="75" HeaderStyle-Width="75">
						<itemstyle cssclass="ibn-vb2"></itemstyle>
						<ItemTemplate>
							<asp:HyperLink Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanView")) %>' ImageUrl = "../../layouts/images/icon-search.GIF" NavigateUrl='<%# "~/Tasks/TaskView.aspx?TaskId=" + DataBinder.Eval(Container.DataItem, "TaskId").ToString() %>' Runat="server" ToolTip='<%#LocRM.GetString("View") %>' ID="Hyperlink5" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:HyperLink style='<%# (GetBool((int)DataBinder.Eval(Container.DataItem, "CanEdit")))? "":"visibility:hidden;" %>' ImageUrl = "../../layouts/images/Edit.GIF" NavigateUrl='<%# "~/Tasks/TaskEdit.aspx?TaskId=" + DataBinder.Eval(Container.DataItem, "TaskId").ToString() %>' Runat="server" ToolTip='<%#LocRM.GetString("Edit") %>' ID="Hyperlink6" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:HyperLink id="Hyperlink7" runat="server" imageurl="../../layouts/images/DELETE.GIF" Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanDelete")) %>' NavigateUrl='<%# "javascript:DeleteTask(" + DataBinder.Eval(Container.DataItem, "TaskId").ToString() + ")" %>' ToolTip='<%#LocRM.GetString("Delete") %>'>
							</asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid>
			<!-- Events -->
			<ibn:sep id="sep4" title="" runat="server" IsClickable="false"></ibn:sep>
			<asp:datagrid id="dgEvent" ShowHeader="true" cellpadding="2" borderwidth="0px" GridLines="None" Width="100%" PagerStyle-HorizontalAlign="Right" PagerStyle-Visible="true" CellSpacing="0" AllowSorting="False" AutoGenerateColumns="False" PagerStyle-Mode="NumericPages" EnableViewState="False" Runat="server">
				<ItemStyle CssClass="ibn-vb2"></ItemStyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2 ibn-propertysheet"></ItemStyle>
						<ItemTemplate>
							<%# GetEventLink (
								(int)DataBinder.Eval(Container.DataItem, "EventId"),
								(string)DataBinder.Eval(Container.DataItem, "Title"),
								(int)DataBinder.Eval(Container.DataItem, "CanView")
								)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderStyle-Width="270px">
						<ItemStyle CssClass="ibn-vb2" Width="270"></ItemStyle>
						<ItemTemplate>
							<%# GetToDoRole
								(
									(int)DataBinder.Eval(Container.DataItem, "IsManager"),
									(int)DataBinder.Eval(Container.DataItem, "IsResource")
								)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle Width="75px"></HeaderStyle>
						<ItemStyle Width="75px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:HyperLink Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanView")) %>' ImageUrl = "../../layouts/images/icon-search.GIF" NavigateUrl='<%# "~/Events/EventView.aspx?EventId=" + DataBinder.Eval(Container.DataItem, "EventId").ToString() %>' Runat="server" ToolTip='<%#LocRM.GetString("View") %>' ID="Hyperlink8" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:HyperLink style='<%# (GetBool((int)DataBinder.Eval(Container.DataItem, "CanEdit")))? "":"visibility:hidden;" %>' ImageUrl = "../../layouts/images/Edit.GIF" NavigateUrl='<%# "~/Events/EventEdit.aspx?EventId=" + DataBinder.Eval(Container.DataItem, "EventId").ToString() %>' Runat="server" ToolTip='<%#LocRM.GetString("Edit") %>' ID="Hyperlink9" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:HyperLink id="Hyperlink10" runat="server" imageurl="../../layouts/images/DELETE.GIF" Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanDelete")) %>' NavigateUrl='<%# "javascript:DeleteEvent(" + DataBinder.Eval(Container.DataItem, "EventId").ToString() + ")" %>' ToolTip='<%#LocRM.GetString("Delete") %>'>
							</asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Right" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
			<!-- Incidents -->
			<ibn:sep id="sep5" title="" runat="server" IsClickable="false"></ibn:sep>
			<asp:datagrid id="dgIncidents" ShowHeader="true" cellpadding="2" borderwidth="0px" GridLines="None" Width="100%" PagerStyle-HorizontalAlign="Right" PagerStyle-Visible="true" CellSpacing="0" AllowSorting="False" AutoGenerateColumns="False" PagerStyle-Mode="NumericPages" EnableViewState="False" Runat="server">
				<ItemStyle CssClass="ibn-vb2"></ItemStyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2 ibn-propertysheet"></ItemStyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetIncidentTitle(
								(int)DataBinder.Eval(Container.DataItem, "IncidentId"),
								(string)DataBinder.Eval(Container.DataItem, "Title")
							)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderStyle-Width="270px">
						<ItemStyle Width="270px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# GetToDoRole
								(
									(int)DataBinder.Eval(Container.DataItem, "IsManager"),
									(int)DataBinder.Eval(Container.DataItem, "IsResource")
								)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle Width="75px"></HeaderStyle>
						<ItemStyle Width="75px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:HyperLink Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanView")) %>' ImageUrl = "../../layouts/images/icon-search.GIF" NavigateUrl='<%# "~/Incidents/IncidentView.aspx?IncidentId=" + DataBinder.Eval(Container.DataItem, "IncidentId").ToString() %>' Runat="server" ToolTip='<%#LocRM.GetString("View") %>' ID="Hyperlink11" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:HyperLink style='<%# (GetBool((int)DataBinder.Eval(Container.DataItem, "CanEdit")))? "":"visibility:hidden;" %>' ImageUrl = "../../layouts/images/Edit.GIF" NavigateUrl='<%# "~/Incidents/IncidentEdit.aspx?IncidentId=" + DataBinder.Eval(Container.DataItem, "IncidentId").ToString() %>' Runat="server" ToolTip='<%#LocRM.GetString("Edit") %>' ID="Hyperlink12" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:HyperLink id="Hyperlink13" runat="server" imageurl="../../layouts/images/DELETE.GIF" Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanDelete")) %>' NavigateUrl='<%# "javascript:DeleteIncident(" + DataBinder.Eval(Container.DataItem, "IncidentId").ToString() + ")" %>' ToolTip='<%#LocRM.GetString("Delete") %>'>
							</asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Right" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
			<!-- Documents -->
			<ibn:sep id="sep6" title="" runat="server" IsClickable="false"></ibn:sep>
			<asp:datagrid id="dgDocuments" ShowHeader="true" cellpadding="2" borderwidth="0px" GridLines="None" Width="100%" PagerStyle-HorizontalAlign="Right" PagerStyle-Visible="true" CellSpacing="0" AllowSorting="False" AutoGenerateColumns="False" PagerStyle-Mode="NumericPages" EnableViewState="False" Runat="server">
				<ItemStyle CssClass="ibn-vb2"></ItemStyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2 ibn-propertysheet"></ItemStyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetDocumentTitle(
								(string)DataBinder.Eval(Container.DataItem, "Title"),
								(int)DataBinder.Eval(Container.DataItem, "DocumentId"),
								(bool)DataBinder.Eval(Container.DataItem, "IsCompleted"),
								(int)DataBinder.Eval(Container.DataItem, "ReasonId")
							)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderStyle-Width="270px">
						<ItemStyle Width="270px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# GetToDoRole
								(
									(int)DataBinder.Eval(Container.DataItem, "IsManager"),
									(int)DataBinder.Eval(Container.DataItem, "IsResource")
								)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle Width="75px"></HeaderStyle>
						<ItemStyle Width="75px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:HyperLink Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanView")) %>' ImageUrl = "../../layouts/images/icon-search.GIF" NavigateUrl='<%# "~/Documents/DocumentView.aspx?DocumentId=" + DataBinder.Eval(Container.DataItem, "DocumentId").ToString() %>' Runat="server" ToolTip='<%#LocRM.GetString("View") %>' ID="Hyperlink14" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:HyperLink style='<%# (GetBool((int)DataBinder.Eval(Container.DataItem, "CanEdit")))? "":"visibility:hidden;" %>' ImageUrl = "../../layouts/images/Edit.GIF" NavigateUrl='<%# "~/Documents/DocumentEdit.aspx?DocumentId=" + DataBinder.Eval(Container.DataItem, "DocumentId").ToString() %>' Runat="server" ToolTip='<%#LocRM.GetString("Edit") %>' ID="Hyperlink15" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:HyperLink id="Hyperlink16" runat="server" imageurl="../../layouts/images/DELETE.GIF" Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanDelete")) %>' NavigateUrl='<%# "javascript:DeleteDocument(" + DataBinder.Eval(Container.DataItem, "DocumentId").ToString() + ")" %>' ToolTip='<%#LocRM.GetString("Delete") %>'>
							</asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Right" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
		</td>
	</tr>
</table>
<asp:linkbutton id="lblDeleteProjectAll" runat="server" Visible="False" onclick="lblDeleteProjectAll_Click"></asp:linkbutton><INPUT id=hdnProjectId type=hidden runat="server">
<asp:linkbutton id="lbDeleteToDoAll" runat="server" Visible="False" onclick="lbDeleteToDoAll_Click"></asp:linkbutton><INPUT id=hdnToDoId type=hidden runat="server">
<asp:linkbutton id="lblDeleteTaskAll" runat="server" Visible="False" onclick="lblDeleteTaskAll_Click"></asp:linkbutton><INPUT id=hdnTaskId type=hidden runat="server">
<asp:linkbutton id="lbEventDeleteAll" runat="server" Visible="False" onclick="lbEventDeleteAll_Click"></asp:linkbutton><INPUT id=hdnEventId type=hidden runat="server">
<asp:linkbutton id="lbIncidentDeleteAll" runat="server" Visible="False" onclick="lbIncidentDeleteAll_Click"></asp:linkbutton><INPUT id=hdnIncidentId type=hidden runat="server">
<asp:linkbutton id="lbDocumentDeleteAll" runat="server" Visible="False" onclick="lbDocumentDeleteAll_Click"></asp:linkbutton><INPUT id="hdnDocumentId" type=hidden runat="server">
