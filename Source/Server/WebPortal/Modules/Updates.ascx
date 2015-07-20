<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.Updates" Codebehind="Updates.ascx.cs" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<%@ Reference Control="~/Modules/Separator2.ascx" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="..\Modules\Separator2.ascx"%>
<script type="text/javascript">
function ObjectClick(Id, Uid, TypeId, obj, evt)
{
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
	else if (TypeId == 30)
		loc.href = "../Apps/BusinessProcess/Pages/AssignmentView.aspx?Id=" + Uid;
	try	{window.top.right.HideFrames2(evt);} catch(e) {}
}
</script>

<ibn:sep id="Sep1" runat="server"></ibn:sep>
<asp:Panel ID="Pan1" Runat="server">
	<asp:datagrid id="dgToday" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="False" CssClass="ibn-propertysheet">
		<columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" style="table-layout:fixed;"
					class='ibn-row'
					onmouseover="this.className='ibn-rowHover'" 
					onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick("<%# Eval("ObjectId") %>", "<%# Eval("ObjectUid") %>", <%# Eval("ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="10px"></td>
							<td width="80px">
								<%# ((DateTime)Eval("Dt")).ToShortTimeString() %>
							</td>
							<td width="35%" style="overflow:hidden;">
								<%# Eval("ObjectTitle") %>
								<%# (int)Eval("ObjectTypeId") == 7 || (int)Eval("ObjectTypeId") == 3 ? CHelper.GetProjectNumPostfix((int)Eval("ObjectId"), (string)Eval("ObjectCode")) : ""%>
							</td>
							<td width="35%">
								<%# Mediachase.IBN.Business.SystemEvents.GetSystemEventName(Eval("SystemEventTitle").ToString()) %>
							</td>
							<td>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL((int)Eval("UserId")) %>
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
	<asp:datagrid id="dgYesterday" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="False" CssClass="ibn-propertysheet">
		<columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" 
					class='ibn-row'
					onmouseover="this.className='ibn-rowHover'" 
					onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick("<%# Eval("ObjectId") %>", "<%# Eval("ObjectUid") %>", <%# Eval("ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="10px"></td>
							<td width="80px">
								<%# ((DateTime)Eval("Dt")).ToShortTimeString() %>
							</td>
							<td width="35%">
								<%# Eval("ObjectTitle") %>
								<%# (int)Eval("ObjectTypeId") == 7 || (int)Eval("ObjectTypeId") == 3 ? CHelper.GetProjectNumPostfix((int)Eval("ObjectId"), (string)Eval("ObjectCode")) : ""%>
							</td>
							<td width="35%">
								<%# Mediachase.IBN.Business.SystemEvents.GetSystemEventName(Eval("SystemEventTitle").ToString()) %>
							</td>
							<td>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL((int)Eval("UserId")) %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</columns>
	</asp:datagrid>
</asp:Panel>

<ibn:sep id="Sep3" runat="server"></ibn:sep>
<asp:Panel ID="Pan3" Runat="server">
	<asp:datagrid id="dgSomeDaysAgo" Runat="server" AutoGenerateColumns="False" AllowPaging="false" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="False" CssClass="ibn-propertysheet">
		<columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" 
					class='ibn-row'
					onmouseover="this.className='ibn-rowHover'" 
					onmouseout="this.className='ibn-row'" 
					onclick='ObjectClick("<%# Eval("ObjectId") %>", "<%# Eval("ObjectUid") %>", <%# Eval("ObjectTypeId") %>, this, event)'>
						<tr>
							<td width="10px"></td>
							<td width="130px">
								<%# ((DateTime)Eval("Dt")).ToShortDateString() %>
								<%# ((DateTime)Eval("Dt")).ToShortTimeString() %>
							</td>
							<td width="31%">
								<%# Eval("ObjectTitle") %>
								<%# (int)Eval("ObjectTypeId") == 7 || (int)Eval("ObjectTypeId") == 3 ? CHelper.GetProjectNumPostfix((int)Eval("ObjectId"), (string)Eval("ObjectCode")) : ""%>
							</td>
							<td width="31%">
								<%# Mediachase.IBN.Business.SystemEvents.GetSystemEventName(Eval("SystemEventTitle").ToString()) %>
							</td>
							<td>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL((int)Eval("UserId")) %>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</columns>
	</asp:datagrid>
</asp:Panel>