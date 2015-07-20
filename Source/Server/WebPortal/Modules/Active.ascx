<%@ Reference Control="~/Modules/Separator2.ascx" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.Active" Codebehind="Active.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="..\Modules\Separator2.ascx"%>
<script type="text/javascript">
function ProjectClick(Id, obj, evt)
{
	if (obj)
		obj.className = "ibn-row";
		
	window.top.right.location.href = "../Projects/ProjectView.aspx?ProjectId=" + Id;
	try	{window.top.right.HideFrames2(evt);} catch(e) {}
}
function TaskToDoClick(Id, IsToDo, obj, evt)
{
	if (obj)
		obj.className = "ibn-row";
		
	if (IsToDo)
		window.top.right.location.href = "../ToDo/ToDoView.aspx?ToDoId=" + Id;
	else
		window.top.right.location.href = "../Tasks/TaskView.aspx?TaskId=" + Id;
	try	{window.top.right.HideFrames2(evt);} catch(e) {}
}
function IncidentClick(Id, obj, evt)
{
	if (obj)
		obj.className = "ibn-row";
		
	window.top.right.location.href = "../Incidents/IncidentView.aspx?IncidentId=" + Id;
	try	{window.top.right.HideFrames2(evt);} catch(e) {}
}
function DocumentClick(Id, obj, evt)
{
	if (obj)
		obj.className = "ibn-row";
		
	window.top.right.location.href = "../Documents/DocumentView.aspx?DocumentId=" + Id;
	try	{window.top.right.HideFrames2(evt);} catch(e) {}
}
</script>

<ibn:sep id="Sep1" runat="server"></ibn:sep>
<asp:Panel ID="Pan1" Runat="server">
	<asp:DataGrid id="dgActiveProjects"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="False" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ProjectClick(<%# Eval("ProjectId") %>, this, event)'>
						<tr>
							<td width="30px" align="center"><%# Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)Eval("PriorityId"), (string)Eval("PriorityName"))%></td>
							<td>
								<%# Eval("Title") %>
								<%# CHelper.GetProjectNumPostfix((int)Eval("ProjectId"), (string)Eval("ProjectCode")) %>
							</td>
							<td width="120px">
								<table>
									<tr>
										<td>
											<div class="progress">
												<img alt='' src='<%#Page.ResolveUrl("~/Layouts/Images/point.gif") %>' width='<%# Eval("PercentCompleted")%>%' />
											</div>
										</td>
										<td><%# Eval("PercentCompleted")%>%</td>
									</tr>
								</table>
							</td>
							<td width="200px">
								<%= LocRM.GetString("from") %> <%# ((DateTime)Eval("TargetStartDate")).ToShortDateString() %>
								<%= LocRM.GetString("till") %> <%# ((DateTime)Eval("TargetFinishDate")).ToShortDateString() %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>

<ibn:sep id="Sep2" runat="server"></ibn:sep>
<asp:Panel ID="Pan2" Runat="server">
	<asp:datagrid id="dgActiveTaskToDo" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='TaskToDoClick(<%# Eval("ItemId") %>, <%# Eval("IsToDo") %>, this, event)'>
						<tr>
							<td width="30px" align="center">
								<%# GetPriorityIcon((int)Eval("PriorityId"), (string)Eval("PriorityName"))%>
							</td>
							<td>
								<%# Eval("Title") %>
							</td>
							<td width="120px">
								<table>
									<tr>
										<td>
											<div class="progress">
												<img alt='' src='<%#Page.ResolveUrl("~/Layouts/Images/point.gif") %>' width='<%# Eval("PercentCompleted")%>%' />
											</div>
										</td>
										<td><%# Eval("PercentCompleted")%>%</td>
									</tr>
								</table>
							</td>
							<td width="200px">
								<%# GetInterval(Eval("StartDate"), Eval("FinishDate")) %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:datagrid>
</asp:Panel>

<ibn:sep id="Sep3" runat="server"></ibn:sep>
<asp:Panel ID="Pan3" Runat="server">
	<asp:datagrid id="dgActiveIncidents" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='IncidentClick(<%# Eval("IncidentId") %>, this, event)'>
						<tr>
							<td width="30" align="center">
								<%# GetPriorityIcon((int)Eval("PriorityId"), (string)Eval("PriorityName"))%>
							</td>
							<td>
								<%# Eval("Title") %> (#<%# Eval("IncidentId") %>)
							</td>
							<td width="200px">
								<%= LocRM.GetString("from")%>
								<%# ((DateTime)Eval("CreationDate")).ToShortDateString() %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:datagrid>
</asp:Panel>

<ibn:sep id="Sep4" runat="server"></ibn:sep>
<asp:Panel ID="Pan4" Runat="server">
	<asp:datagrid id="dgActiveDocuments" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='DocumentClick(<%# Eval("DocumentId") %>, this, event)'>
						<tr>
							<td width="30" align="center">
								<%# GetPriorityIcon((int)Eval("PriorityId"), (string)Eval("PriorityName"))%>
							</td>
							<td>
								<%# Eval("Title") %>
							</td>
							<td width="200px">
								<%= LocRM.GetString("from")%>
								<%# ((DateTime)Eval("CreationDate")).ToShortDateString() %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:datagrid>
</asp:Panel>