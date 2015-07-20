<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.History" Codebehind="History.ascx.cs" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<script type="text/javascript">

function ObjectClick(Id, TypeId, Uid, ClassName, obj, evt)
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
	else if (TypeId == 20) 
		loc.href = "../Incidents/ArticleView.aspx?ArticleId=" + Id;
	else if (TypeId == -1)
		loc.href = "../Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName=" + ClassName + "&ObjectId=" + Uid;
		
	try	{window.top.right.HideFrames2(evt);} catch(e) {}
}
</script>

<asp:DataGrid id="dgHistory" Runat="server" AutoGenerateColumns="False" AllowPaging="True" AllowSorting="False" CellSpacing="0" Width="100%" GridLines="Horizontal" borderwidth="0px" cellpadding="0"	ShowHeader="false" EnableViewState="True" CssClass="ibn-propertysheet" PagerStyle-Visible="False" PageSize="100">
	<Columns>
		<asp:TemplateColumn>
			<ItemTemplate>
				<table cellpadding="0" cellspacing="0" width="100%" class="ibn-row" style="table-layout:fixed;"
				onmouseover="this.className='ibn-rowHover'" onmouseout="this.className='ibn-row'" 
				onclick='ObjectClick(<%# Eval("ObjectId") %>, <%# Eval("ObjectTypeId") %>, "<%# Eval("ObjectUid") %>", "<%# Eval("ClassName") %>",  this, event)'>
					<tr>
						<td width="30px"></td>
						<td style="overflow:hidden;">
							<%# CHelper.GetResFileString((string)Eval("ObjectTitle")) %>
							<%# (int)Eval("ObjectTypeId") == 7 || (int)Eval("ObjectTypeId") == 3 ? CHelper.GetProjectNumPostfix((int)Eval("ObjectId"), (string)Eval("ObjectCode")) : ""%>
						</td>
						<td width="120px">
							<%# Mediachase.UI.Web.Util.CommonHelper.GetObjectTypeName((int)Eval("ObjectTypeId"), (string)Eval("ClassName"))%>
						</td>
						<td width="140px">
							<%# ((DateTime)Eval("Dt")).ToShortDateString() %>
							<%# ((DateTime)Eval("Dt")).ToShortTimeString() %>
						</td>
					</tr>
				</table>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</asp:DataGrid>