<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Search.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Common.Modules.Search" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<%@ Reference Control="~/Modules/SeparatorWhite.ascx" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="~/Modules/SeparatorWhite.ascx"%>
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
		obj.className = "ibn-row-white";

	var loc = window.location;
	if (TypeId == 1) 
		loc.href = "../../../Directory/UserView.aspx?UserID=" + Id;
	else if (TypeId == 2) 
		loc.href = "../../../Directory/Directory.aspx?Tab=0&SGroupId=" + Id;
	else if (TypeId == 3) 
		loc.href = "../../../Projects/ProjectView.aspx?ProjectID=" + Id;
	else if (TypeId == 4) 
		loc.href = "../../../Events/EventView.aspx?EventId=" + Id;
	else if (TypeId == 5) 
		loc.href = "../../../Tasks/TaskView.aspx?TaskId=" + Id;
	else if (TypeId == 6) 
		loc.href = "../../../ToDo/ToDoView.aspx?ToDoId=" + Id;
	else if (TypeId == 7) 
		loc.href = "../../../Incidents/IncidentView.aspx?IncidentId=" + Id;
	else if (TypeId == 15) 
		loc.href = "../../../Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName=List_" + Id;
	else if (TypeId == 8)		// File
		loc.href = "../../../FileStorage/DownloadFile.aspx?Id=" + Id;
	else if (TypeId == 16) 
		loc.href = "../../../Documents/DocumentView.aspx?DocumentId=" + Id;
	else if (TypeId == 21) 
		loc.href = "../../../Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName=Organization&ObjectId=" + Id;
	else if (TypeId == 22) 
		loc.href = "../../../Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName=Contact&ObjectId=" + Id;
}

function ObjectClickFile(_href, _new, obj, evt)
{
	evt = (evt) ? evt : ((window.event) ? event : null);
	if (evt)
	{
		var src = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
		if (src && src.type == "image") return;
	}
	
	if (obj)
		obj.className = "ibn-row-white";

	var loc = window.location;
	if(_new==0)
		loc.href = _href;
	else
		var f = window.open(_href, "_blank");
}

function checkKey(e) 
{ 
	var _key = e.keyCode ? e.keyCode : e.which ? e.which : e.charCode;
	try {
		if (_key == 13)
			<%= Page.ClientScript.GetPostBackEventReference(btnSearch, "") %>
		else
			return true;
	}
	catch (e) {return true;}
}
</script>

<table cellpadding="5" cellspacing="0" class="text">
	<tr>
		<td><%=LocRM.GetString("SearchString") %>:</td>
		<td>
			<asp:TextBox CssClass="text" Width="350" ID="tbSearchstr" Runat="server" onkeypress="return checkKey(event);"></asp:TextBox>
			<asp:ImageButton Runat="server" id="btnSearch" Width="16" Height="16" 
				ImageUrl="~/layouts/images/search.gif" ImageAlign="AbsMiddle" 
				onclick="btnSearch_Click"></asp:ImageButton>
		</td>
	</tr>
</table>

<ibn:sep id="Sep1" runat="server"></ibn:sep>
<asp:Panel ID="Pan1" Runat="server">
	<asp:DataGrid id="dgProjects"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row-white" 
					onmouseover="this.className='ibn-row-whiteHover'" onmouseout="this.className='ibn-row-white'" 
					onclick='ObjectClick(<%# Eval("ProjectId") %>, <%= (int)Mediachase.IBN.Business.ObjectTypes.Project %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# Eval("Title") %>
								<%# CHelper.GetProjectNumPostfix((int)Eval("ProjectId"), (string)Eval("ProjectCode")) %>
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
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row-white" 
					onmouseover="this.className='ibn-row-whiteHover'" onmouseout="this.className='ibn-row-white'" 
					onclick='ObjectClick(<%# Eval("ItemId") %>, <%# ((int)Eval("IsToDo") == 1) ? (int)Mediachase.IBN.Business.ObjectTypes.ToDo : (int)Mediachase.IBN.Business.ObjectTypes.Task %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# Eval("Title") %> (#<%# Eval("ItemId") %>)
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
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row-white" 
					onmouseover="this.className='ibn-row-whiteHover'" onmouseout="this.className='ibn-row-white'" 
					onclick='ObjectClick(<%# Eval("EventId") %>, <%= (int)Mediachase.IBN.Business.ObjectTypes.CalendarEntry %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# Eval("Title") %>
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
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row-white" 
					onmouseover="this.className='ibn-row-whiteHover'" onmouseout="this.className='ibn-row-white'" 
					onclick='ObjectClick(<%# Eval("IncidentId") %>, <%= (int)Mediachase.IBN.Business.ObjectTypes.Issue %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# Eval("Title") %> (#<%# Eval("IncidentId") %>)
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>

<ibn:sep id="Sep6" runat="server"></ibn:sep>
<asp:Panel ID="Pan6" Runat="server">
	<asp:DataGrid id="dgFiles"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row-white" 
					onmouseover="this.className='ibn-row-whiteHover'" onmouseout="this.className='ibn-row-white'" 
					onclick="<%# "ObjectClickFile(&quot;" + Eval("_Href") + "&quot;," + Eval("_New") + ", this, event)" %>">
						<tr>
							<td width="30px"></td>
							<td>
								<%# Eval("Title") %>
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
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row-white" 
					onmouseover="this.className='ibn-row-whiteHover'" onmouseout="this.className='ibn-row-white'" 
					onclick='ObjectClick(<%# Eval("PrimaryKeyId") %>, <%= (int)Mediachase.IBN.Business.ObjectTypes.List %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# Eval("Title") %>
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
	<asp:DataGrid id="dgUsers"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row-white" 
					onmouseover="this.className='ibn-row-whiteHover'" onmouseout="this.className='ibn-row-white'" 
					onclick='ObjectClick(<%# Eval("UserId") %>, <%= (int)Mediachase.IBN.Business.ObjectTypes.User %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# Eval("LastName")%> <%# Eval("FirstName")%>
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
	<asp:DataGrid id="dgGroups"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row-white" 
					onmouseover="this.className='ibn-row-whiteHover'" onmouseout="this.className='ibn-row-white'" 
					onclick='ObjectClick(<%# Eval("GroupId") %>, <%= (int)Mediachase.IBN.Business.ObjectTypes.UserGroup %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetResFileString(Eval("GroupName").ToString())%>
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
	<asp:DataGrid id="dgDocuments"  Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row-white" 
					onmouseover="this.className='ibn-row-whiteHover'" onmouseout="this.className='ibn-row-white'" 
					onclick='ObjectClick(<%# Eval("DocumentId") %>, <%= (int)Mediachase.IBN.Business.ObjectTypes.Document %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# Eval("Title") %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>
<ibn:sep id="Sep11" runat="server"></ibn:sep>
<asp:Panel ID="Pan11" Runat="server">
	<asp:DataGrid id="dgOrganizations" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row-white" 
					onmouseover="this.className='ibn-row-whiteHover'" onmouseout="this.className='ibn-row-white'" 
					onclick='ObjectClick(&quot;<%# Eval("PrimaryKeyId") %>&quot;, <%= (int)Mediachase.IBN.Business.ObjectTypes.Organization %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# Eval("Name")%>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>

<ibn:sep id="Sep12" runat="server"></ibn:sep>
<asp:Panel ID="Pan12" Runat="server">
	<asp:DataGrid id="dgContacts" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet">
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row-white" 
					onmouseover="this.className='ibn-row-whiteHover'" onmouseout="this.className='ibn-row-white'" 
					onclick='ObjectClick(&quot;<%# Eval("PrimaryKeyId") %>&quot;, <%= (int)Mediachase.IBN.Business.ObjectTypes.Contact %>, this, event)'>
						<tr>
							<td width="30px"></td>
							<td>
								<%# Eval("FullName")%>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid>	
</asp:Panel>