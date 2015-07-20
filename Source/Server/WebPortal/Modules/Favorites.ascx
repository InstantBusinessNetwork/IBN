<%@ Reference Control="~/Modules/Separator2.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.Favorites" Codebehind="Favorites.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="..\Modules\Separator2.ascx"%>
<script type="text/javascript">

function ObjectClick(Id, TypeId, obj, evt)
{
	evt = (evt) ? evt : ((window.event) ? event : null);
	if (evt)
	{
		var src = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
		if (src && src.type == "image") return;
	}
	
	if (obj)
		obj.className = "ibn-row";

	var loc = window.top.right.location;
	if (TypeId == 1) 
		loc.href = "../Directory/UserView.aspx?UserID=" + Id;
	else if (TypeId == 3) 
		loc.href = "../Projects/ProjectView.aspx?ProjectID=" + Id;
	else if (TypeId == 4) 
		loc.href = "../Events/EventView.aspx?EventId=" + Id;
	else if (TypeId == 5) 
		loc.href = "../Tasks/TaskView.aspx?TaskId=" + Id;
	else if (TypeId == 6) 
		loc.href = "../ToDo/ToDoView.aspx?ToDoId=" + Id;
	else if (TypeId == 7) 
		loc.href = "../Incidents/IncidentView.aspx?IncidentId=" + Id;
	else if (TypeId == 15) 
		loc.href = "../Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName=List_" + Id;
	else if (TypeId == 16) 
		loc.href = "../Documents/DocumentView.aspx?DocumentId=" + Id;
	else if (TypeId == 21) 
		loc.href = "../Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName=Organization&ObjectId=" + Id;
	else if (TypeId == 22) 
		loc.href = "../Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName=Contact&ObjectId=" + Id;
	else if (TypeId == 25) 
		loc.href = "../Reports/ReportHistory.aspx?TemplateId=" + Id;
	else if (TypeId == 29) 
		loc.href = "../Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName=CalendarEvent&ObjectId=" + Id;
		
	try	{window.top.right.HideFrames2(evt);} catch(e) {}
}
</script>

<ibn:sep id="Sep1" runat="server"></ibn:sep>
<asp:Panel ID="Pan1" Runat="server">
	<asp:DataGrid id="dgProjects"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>, <%# DataBinder.Eval(Container.DataItem, "ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td width="30">
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="../layouts/images/Delete.gif" CommandName="Delete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>'></asp:imagebutton>
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
	<asp:DataGrid id="dgTasks"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>, <%# DataBinder.Eval(Container.DataItem, "ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td width="30">
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="../layouts/images/Delete.gif" CommandName="Delete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>'></asp:imagebutton>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>

<ibn:sep id="Sep3" runat="server"></ibn:sep>
<asp:Panel ID="Pan3" Runat="server">
	<asp:DataGrid id="dgToDo"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>, <%# DataBinder.Eval(Container.DataItem, "ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td width="30">
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="../layouts/images/Delete.gif" CommandName="Delete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>'></asp:imagebutton>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>

<ibn:sep id="Sep4" runat="server"></ibn:sep>
<asp:Panel ID="Pan4" Runat="server">
	<asp:DataGrid id="dgEvents"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>, <%# DataBinder.Eval(Container.DataItem, "ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td width="30">
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="../layouts/images/Delete.gif" CommandName="Delete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>'></asp:imagebutton>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>

<ibn:sep id="Sep5" runat="server"></ibn:sep>
<asp:Panel ID="Pan5" Runat="server">
	<asp:DataGrid id="dgIncidents"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>, <%# DataBinder.Eval(Container.DataItem, "ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td width="30">
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="../layouts/images/Delete.gif" CommandName="Delete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>'></asp:imagebutton>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>

<ibn:sep id="Sep7" runat="server"></ibn:sep>
<asp:Panel ID="Pan7" Runat="server">
	<asp:DataGrid id="dgLists"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>, <%# DataBinder.Eval(Container.DataItem, "ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td width="30">
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="../layouts/images/Delete.gif" CommandName="Delete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>'></asp:imagebutton>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>

<ibn:sep id="Sep8" runat="server"></ibn:sep>
<asp:Panel ID="Pan8" Runat="server">
	<asp:DataGrid id="dgDocuments"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>, <%# DataBinder.Eval(Container.DataItem, "ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td width="30">
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="../layouts/images/Delete.gif" CommandName="Delete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>'></asp:imagebutton>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>

<ibn:sep id="Sep10" runat="server"></ibn:sep>
<asp:Panel ID="Pan10" Runat="server">
	<asp:DataGrid id="dgUsers" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0" ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick(<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>, <%# DataBinder.Eval(Container.DataItem, "ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td width="30">
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="../layouts/images/Delete.gif" CommandName="Delete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ObjectId") %>'></asp:imagebutton>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>		
</asp:Panel>

<ibn:sep id="Sep9" runat="server"></ibn:sep>
<asp:Panel ID="Pan9" Runat="server">
	<asp:DataGrid id="dgReports"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick(<%# Eval("ObjectId") %>, <%# Eval("ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# Eval("Title") %>
							</td>
							<td width="30">
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="../layouts/images/Delete.gif" CommandName="Delete" causesvalidation="False" CommandArgument='<%# Eval("ObjectId") %>'></asp:imagebutton>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>

<ibn:sep id="Sep21" runat="server"></ibn:sep>
<asp:Panel ID="Pan21" Runat="server">
	<asp:DataGrid id="dgOrganizations"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0" ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick("<%# DataBinder.Eval(Container.DataItem, "ObjectUid") %>", <%# DataBinder.Eval(Container.DataItem, "ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td width="30">
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="../layouts/images/Delete.gif" CommandName="Delete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ObjectUid") %>'></asp:imagebutton>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>

<ibn:sep id="Sep22" runat="server"></ibn:sep>
<asp:Panel ID="Pan22" Runat="server">
	<asp:DataGrid id="dgContacts"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0" ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick("<%# DataBinder.Eval(Container.DataItem, "ObjectUid") %>", <%# DataBinder.Eval(Container.DataItem, "ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td width="30">
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="../layouts/images/Delete.gif" CommandName="Delete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ObjectUid") %>'></asp:imagebutton>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>

<ibn:sep id="Sep29" runat="server"></ibn:sep>
<asp:Panel ID="Pan29" Runat="server">
	<asp:DataGrid id="dgCalendarEvent"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0" ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" 
					onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick("<%# DataBinder.Eval(Container.DataItem, "ObjectUid") %>", <%# DataBinder.Eval(Container.DataItem, "ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "Title") %>
							</td>
							<td width="30">
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="../layouts/images/Delete.gif" CommandName="Delete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ObjectUid") %>'></asp:imagebutton>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>