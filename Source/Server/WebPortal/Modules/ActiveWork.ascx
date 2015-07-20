<%@ Reference Control="~/Modules/Separator2.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.ActiveWork" Codebehind="ActiveWork.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="..\Modules\Separator2.ascx"%>
<script type="text/javascript">
function ObjectClick(Id, TypeId, obj, evt)
{
	if (obj)
		obj.className = "ibn-row";
		
	var loc = window.top.right.location;
	if (TypeId == 1) 
		loc.href = "../Directory/UserView.aspx?UserID=" + Id;
	else if (TypeId == 4) 
		loc.href = "../Events/EventView.aspx?EventId=" + Id;
	else if (TypeId == 5) 
		loc.href = "../Tasks/TaskView.aspx?TaskId=" + Id;
	else if (TypeId == 6) 
		loc.href = "../ToDo/ToDoView.aspx?ToDoId=" + Id;
	else if (TypeId == 7) 
		loc.href = "../Incidents/IncidentView.aspx?IncidentId=" + Id;
	else if (TypeId == 12) 
		loc.href = "../Incidents/IncidentEdit.aspx?EMailMessageId=" + Id;
	else if (TypeId == 15) 
		loc.href = "../Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName=List_" + Id;
	else if (TypeId == 16) 
		loc.href = "../Documents/DocumentView.aspx?DocumentId=" + Id;
	
	try	{window.top.right.HideFrames2(evt);} catch(e) {}
}
</script>
<ibn:sep id="Sep1" runat="server"></ibn:sep>
<asp:Panel ID="Pan1" Runat="server">
	<asp:datagrid id="dgPendingUsers" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="False" CssClass="ibn-propertysheet">
		<columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "PrincipalId") %>, 1, this, event)'>
						<tr>
							<td width="30px"></td>
							<td width="50%">
								<%# DataBinder.Eval(Container.DataItem, "LastName") %> <%# DataBinder.Eval(Container.DataItem, "FirstName") %>
							</td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Email") %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</columns>
	</asp:datagrid>
</asp:Panel>

<ibn:sep id="Sep2" runat="server"></ibn:sep>
<asp:Panel ID="Pan2" Runat="server">
	<asp:datagrid id="dgIncidents" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "IncidentId") %>, 7, this, event)'>
						<tr>
							<td width="30" align="center">
								<%# Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)DataBinder.Eval(Container.DataItem, "PriorityId"), (string)DataBinder.Eval(Container.DataItem, "PriorityName"))%>
							</td>
							<td width="50%">
								<%# DataBinder.Eval(Container.DataItem, "Title") %> (#<%# DataBinder.Eval(Container.DataItem, "IncidentId") %>)
							</td>
							<td>
								<%# ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToShortDateString() %>
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
	<asp:datagrid id="dgMailIncidents" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "PendingMessageId") %>, 12, this, event)'>
						<tr>
							<td width="30px">&nbsp;</td>
							<td width="50%">
								<%# DataBinder.Eval(Container.DataItem, "Subject") %>
							</td>
							<td>
								<%# ((DateTime)DataBinder.Eval(Container.DataItem, "Created")).ToString("g")%>
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
	<asp:datagrid id="dgNotApprovedTaskToDo" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "ItemId") %>, <%# ((int)DataBinder.Eval(Container.DataItem, "IsToDo") == 1) ? 6 : 5 %>, this, event)'>
						<tr>
							<td width="30" align="center">
								<%# Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)DataBinder.Eval(Container.DataItem, "PriorityId"), (string)DataBinder.Eval(Container.DataItem, "PriorityName"))%>
							</td>
							<td width="50%">
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td>
								<%# (DataBinder.Eval(Container.DataItem, "StartDate") == DBNull.Value) ? "&nbsp;" :
								((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")).ToShortDateString() %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:datagrid>
</asp:Panel>

<ibn:sep id="Sep5" runat="server"></ibn:sep>
<asp:Panel ID="Pan5" Runat="server">
	<asp:datagrid id="dgAssignments" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "ItemId") %>, <%# ((int)DataBinder.Eval(Container.DataItem, "IsToDo") == 1) ? 6 : 5 %>, this, event)'>
						<tr>
							<td width="30" align="center">
								<%# Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)DataBinder.Eval(Container.DataItem, "PriorityId"), (string)DataBinder.Eval(Container.DataItem, "PriorityName"))%>
							</td>
							<td width="50%">
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td>
								<%# (DataBinder.Eval(Container.DataItem, "StartDate") == DBNull.Value) ? "&nbsp;" :
								((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")).ToShortDateString() %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:datagrid>
</asp:Panel>

<ibn:sep id="Sep6" runat="server"></ibn:sep>
<asp:Panel ID="Pan6" Runat="server">
	<asp:datagrid id="dgIncPending" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "IncidentId") %>, 7, this, event)'>
						<tr>
							<td width="30" align="center">
								<%# Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)DataBinder.Eval(Container.DataItem, "PriorityId"), (string)DataBinder.Eval(Container.DataItem, "PriorityName"))%>
							</td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Title") %> (#<%# DataBinder.Eval(Container.DataItem, "IncidentId") %>)
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:datagrid>
</asp:Panel>

<ibn:sep id="Sep7" runat="server"></ibn:sep>
<asp:Panel ID="Pan7" Runat="server">
	<asp:datagrid id="dgPendingEvents" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "EventId") %>, 4, this, event)'>
						<tr>
							<td width="30" align="center">
								<%# Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)DataBinder.Eval(Container.DataItem, "PriorityId"), (string)DataBinder.Eval(Container.DataItem, "PriorityName"))%>
							</td>
							<td width="50%">
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td>
								<%# ((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")).ToShortDateString() %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:datagrid>
</asp:Panel>

<ibn:sep id="Sep8" runat="server"></ibn:sep>
<asp:Panel ID="Pan8" Runat="server">
	<asp:datagrid id="dgNotAssigned" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "ItemId") %>, <%# ((int)DataBinder.Eval(Container.DataItem, "IsToDo") == 1) ? 6 : 5 %>, this, event)'>
						<tr>
							<td width="30" align="center">
								<%# Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)DataBinder.Eval(Container.DataItem, "PriorityId"), (string)DataBinder.Eval(Container.DataItem, "PriorityName"))%>
							</td>
							<td width="50%">
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td>
								<%# (DataBinder.Eval(Container.DataItem, "StartDate") == DBNull.Value) ? "&nbsp;" :
								((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")).ToShortDateString() %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:datagrid>
</asp:Panel>

<ibn:sep id="Sep10" runat="server"></ibn:sep>
<asp:Panel ID="Pan10" Runat="server">
	<asp:datagrid id="dgDocuments" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "DocumentId") %>, 16, this, event)'>
						<tr>
							<td width="30" align="center">
								<%# Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)DataBinder.Eval(Container.DataItem, "PriorityId"), (string)DataBinder.Eval(Container.DataItem, "PriorityName"))%>
							</td>
							<td width="50%">
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td>
								<%# ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToShortDateString() %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:datagrid>
</asp:Panel>

<ibn:sep id="Sep11" runat="server"></ibn:sep>
<asp:Panel ID="Pan11" Runat="server">
	<asp:datagrid id="dgPendingDocuments" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "DocumentId") %>, 16, this, event)'>
						<tr>
							<td width="30" align="center">
								<%# Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)DataBinder.Eval(Container.DataItem, "PriorityId"), (string)DataBinder.Eval(Container.DataItem, "PriorityName"))%>
							</td>
							<td width="50%">
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td>
								<%# ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToShortDateString() %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:datagrid>
</asp:Panel>

<ibn:sep id="Sep12" runat="server"></ibn:sep>
<asp:Panel ID="Pan12" Runat="server">
	<asp:datagrid id="dgDeclinedRequests" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false"  EnableViewState="false" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
						onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
						onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>, <%# DataBinder.Eval(Container.DataItem, "ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30" align="center">
								<%# Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)DataBinder.Eval(Container.DataItem, "PriorityId"), (string)DataBinder.Eval(Container.DataItem, "PriorityName"))%>
							</td>
							<td width="50%">
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td width="25%">
								<%# DataBinder.Eval(Container.DataItem, "LastName")%> <%# DataBinder.Eval(Container.DataItem, "FirstName")%>
							</td>
							<td>
								<%# ((DateTime)DataBinder.Eval(Container.DataItem, "LastSavedDate")).ToShortDateString() %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:datagrid>
</asp:Panel>