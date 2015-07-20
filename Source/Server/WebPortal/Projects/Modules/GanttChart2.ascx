<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GanttChart2.ascx.cs" Inherits="Mediachase.UI.Web.Projects.Modules.GanttChart2" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>

<script type="text/javascript">
//<![CDATA[
var iDivTimerGanttAdd = -1;
function PrepareClosing(_divId)
{
	iDivTimerGanttAdd = window.setInterval(closeMenu, 500);
}

function CancelClosing()
{
	if(iDivTimerGanttAdd>=0)
	{
		window.clearInterval(iDivTimerGanttAdd);
		iDivTimerGanttAdd = -1;
	}
}

function DeleteTask(TaskId)
{
	document.forms[0].<%=hdnTaskId.ClientID %>.value = TaskId;
	if(confirm('<%=LocRM.GetString("Warning")%>'))
		<%=Page.ClientScript.GetPostBackEventReference(lblDeleteTaskAll,"") %>
}
function openMenu(curObjID,CurId,IsMilestone)
{
	var menuItems = document.getElementById("tblMenuItems");
	if (IsMilestone == "True")
	{
		menuItems.rows[2].style.display = "none";
		menuItems.rows[3].style.display = "none";
	}
	else
	{
		menuItems.rows[2].style.display = "block";
		menuItems.rows[3].style.display = "block";
	}
	menu = getMenu('divInsertMenu');
	document.forms[0].<%=hdnTaskId.ClientID %>.value = CurId;
	var curObj = document.getElementById(curObjID);
	off = GetTotalOffset(curObj);
	menu.style.left = (off.Left).toString() + "px";
	menu.style.top = (off.Top - 8).toString() + "px";
	menu.style.display = "";
}
function closeMenu()
{
	getMenu('divInsertMenu').style.display = "none";
	document.forms[0].<%=hdnTaskId.ClientID %>.value = "";
}
function getMenu(s)
{
	return document.getElementById(s)
}
function GetTotalOffset(eSrc)
{
	this.Top = 0;
	this.Left = 0;
	while (eSrc)
	{
		this.Top += eSrc.offsetTop;
		this.Left += eSrc.offsetLeft;
		eSrc = eSrc.offsetParent;
	}
	return this;
}
function Insert(s)
{
	var TaskId = document.forms[0].<%=hdnTaskId.ClientID %>.value;
	if(s=="before")
	{
		window.top.right.location.href = "../Tasks/TaskEdit.aspx?BeforeTaskId="+TaskId+"&Back=Gantt";
	}
	else if(s=="after")
	{
		window.top.right.location.href = "../Tasks/TaskEdit.aspx?AfterTaskId="+TaskId+"&Back=Gantt";
	}
	else if(s=="first")
	{
		window.top.right.location.href = "../Tasks/TaskEdit.aspx?FirstForTaskId="+TaskId+"&Back=Gantt";
	}
	else if(s=="last")
	{
		window.top.right.location.href = "../Tasks/TaskEdit.aspx?LastForTaskId="+TaskId+"&Back=Gantt";
	}
}
function Over(obj)
{
	obj.style.backgroundColor ="#dadada";
}
function Out(obj)
{
	obj.style.backgroundColor ="#efefef";
}
var _linkToGantt = "";
function f_onload(lx, portionWidth)
{
	var _obj = document.getElementById('<%=hdnLinkToGantt.ClientID%>');
	_linkToGantt = _obj.value;
	BindDiv('<%=divImg.ClientID%>', _linkToGantt, -1, '', '', lx, portionWidth);
}

function contentPageLoad()
{
	var obj = document.getElementById('<%=divImg.ClientID%>');
	if (obj)
		obj.style.top = "0px"
}
//]]>
</script>

<asp:CustomValidator ID="cvUnableMove" Display="Dynamic" CssClass="text" runat="server"></asp:CustomValidator><br />
<table cellpadding="0" cellspacing="0" width="100%" border="0" align="left">
	<tr class="ibn-alternating ibn-navline" id="trBasePlanFilter" runat="server">
		<td colspan="2" style="padding: 10px;" valign="middle" class="text">
			<asp:Literal runat="server" ID="ltCompareWith"></asp:Literal>:&nbsp;
			<asp:DropDownList runat="server" ID="ddlBasePlans" Width="340px">
			</asp:DropDownList>
			&nbsp;&nbsp;
			<btn:IMButton class="text" runat="server" ID="ibtnApply" style="width: 110px;">
			</btn:IMButton>
		</td>
	</tr>
	<tr>
		<td valign="top">
			<dg:DataGridExtended runat="server" ID="grdTasks" AllowPaging="True" AllowSorting="False" CellPadding="0" GridLines="None" CellSpacing="0" BorderWidth="0px" AutoGenerateColumns="False" Width="100%" PageSize="10" LayoutFixed="false">
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<ItemStyle Font-Size="6pt"></ItemStyle>
				<Columns>
					<asp:BoundColumn DataField="TaskID" HeaderText="ID" ItemStyle-CssClass="pp" ReadOnly="True" Visible="False"></asp:BoundColumn>
					<asp:BoundColumn DataField="IsSummary" Visible="False" ReadOnly="True"></asp:BoundColumn>
					<asp:BoundColumn DataField="TaskNum" HeaderStyle-CssClass="pp" HeaderText="#" ItemStyle-CssClass="pp" ReadOnly="True"></asp:BoundColumn>
					<asp:BoundColumn DataField="OutlineNumber" HeaderStyle-CssClass="pp" HeaderText="Number" ItemStyle-CssClass="pp" ReadOnly="True" Visible="False"></asp:BoundColumn>
					<asp:BoundColumn DataField="OutlineLevel" HeaderStyle-CssClass="pp" HeaderText="Lvl" ItemStyle-CssClass="pp" ReadOnly="True" Visible="False"></asp:BoundColumn>
					<asp:BoundColumn DataField="Title" HeaderStyle-CssClass="pp" HeaderText="Title" ItemStyle-CssClass="pp" ReadOnly="True" ItemStyle-Wrap="False"></asp:BoundColumn>
					<asp:BoundColumn DataField="IsCollapsed" Visible="False" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn Visible="False">
						<ItemTemplate>
							<asp:Button ID="btnLeft" runat="server" CommandName="MoveLeft" CausesValidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>'></asp:Button>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn Visible="False">
						<ItemTemplate>
							<asp:Button ID="btnRight" runat="server" CommandName="MoveRight" CausesValidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>'></asp:Button>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn Visible="False">
						<ItemTemplate>
							<asp:Button ID="btnCollapse" runat="server" CommandName="Collapse" CausesValidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>'></asp:Button>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn Visible="False">
						<ItemTemplate>
							<asp:Button ID="btnExpand" runat="server" CommandName="Expand" CausesValidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>'></asp:Button>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="StartDate" DataFormatString="{0:d} -" HeaderStyle-CssClass="pp" HeaderText="Start" ItemStyle-CssClass="pp" ItemStyle-Font-Size="7pt" ReadOnly="True" ItemStyle-Wrap="False"></asp:BoundColumn>
					<asp:BoundColumn DataField="FinishDate" DataFormatString="{0:d}" HeaderStyle-CssClass="pp" HeaderText="Finish" ItemStyle-CssClass="pp" ItemStyle-Font-Size="7pt" ReadOnly="True" ItemStyle-Wrap="False"></asp:BoundColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="pp" HeaderText="" ItemStyle-CssClass="pp" HeaderStyle-Width="147px" ItemStyle-Width="147px" ItemStyle-Wrap="False">
						<ItemTemplate>
							<nobr>
							<asp:imagebutton id="ibMove" runat="server" borderwidth="0" width="14" height="14" title='<%#LocRM.GetString("tMoveTo")%>' imageurl="../../layouts/images/MoveTo.gif" commandname="Edit" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>' style="vertical-align: middle; margin-right:5px" visible='<%# !isMSProject%>'>
							</asp:imagebutton>
							<%# isMSProject?"":"<a id=\"IC"+DataBinder.Eval(Container.DataItem, "TaskID").ToString()+"\" href=\"javascript:openMenu('IC"+DataBinder.Eval(Container.DataItem, "TaskID").ToString()+"',"+DataBinder.Eval(Container.DataItem, "TaskID").ToString()+",'" + DataBinder.Eval(Container.DataItem, "IsMilestone").ToString() + "')\" title='"+LocRM.GetString("tInsert")+"'><img src='../layouts/images/Newitem.gif' width='16' height='16' border='0' style='margin-right:5px'></a>"%>
							<a href='../Tasks/TaskEdit.aspx?TaskId=<%# DataBinder.Eval(Container.DataItem, "TaskId")%>&Back=Gantt' title='<%= LocRM.GetString("tEditTask")%>'><img src="../layouts/images/Edit.gif" width="16" height="16" border="0" style='margin-right:5px'></a>
							<asp:imagebutton id="ibActivate" runat="server" borderwidth="0" width="16" height="16" title='<%#LocRM.GetString("Activate")%>' imageurl="../../images/IbnFramework/activate.png" commandname="Activate" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>' style="vertical-align: middle; margin-right:5px" visible='<%# !(bool)Eval("IsCompleted") && (int)Eval("StateId") == 1%>'></asp:imagebutton>
							<asp:imagebutton id="ibDeactivate" runat="server" borderwidth="0" width="16" height="16" title='<%#LocRM.GetString("Deactivate")%>' imageurl="../../images/IbnFramework/deactivate.png" commandname="Deactivate" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>' style="vertical-align: middle; margin-right:5px" visible='<%# !(bool)Eval("IsCompleted") && (int)Eval("StateId") != 1%>'></asp:imagebutton>
							<asp:Image runat="server" ID="ActivateSpacer" ImageUrl="~/Layouts/Images/spacer.gif" Width="16" Height="1" visible='<%# (bool)Eval("IsCompleted") %>' style="margin-right:5px" />
							<asp:imagebutton id="ibComplete" runat="server" borderwidth="0" width="16" height="16" title='<%#LocRM.GetString("Complete")%>' imageurl="../../images/IbnFramework/accept.gif" commandname="Complete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>' style="vertical-align: middle; margin-right:5px" visible='<%# !(bool)Eval("IsCompleted")%>'></asp:imagebutton>
							<asp:Image runat="server" ID="CompleteSpacer" ImageUrl="~/Layouts/Images/spacer.gif" Width="16" Height="1" visible='<%# (bool)Eval("IsCompleted")%>' style="margin-right:5px" />
							<asp:HyperLink id="Hyperlink7" runat="server" imageurl="../../layouts/images/DELETE.GIF" NavigateUrl='<%# "javascript:DeleteTask(" + DataBinder.Eval(Container.DataItem, "TaskID").ToString() + ")" %>' ToolTip='<%# LocRM.GetString("tDeleteTask") %>' Width="16px" visible='<%# !isMSProject%>'/>
							</nobr>
						</ItemTemplate>
						<EditItemTemplate>
							<table cellpadding="0" cellspacing="0" border="0">
								<tr>
									<td>
										<asp:DropDownList ID="ddl" runat="server" CssClass="text" Font-Size="10px">
										</asp:DropDownList>
									</td>
									<td style="padding-left: 5px">
										<asp:ImageButton ID="Imagebutton1" runat="server" BorderWidth="0" Width="16" Height="16" title='<%#LocRM.GetString("tSave")%>' ImageUrl="../../layouts/images/SaveItem.gif" CommandName="Update" CausesValidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>' />
									</td>
									<td style="padding-left: 5px">
										<asp:ImageButton ID="Imagebutton2" runat="server" BorderWidth="0" Width="16" Height="16" title='<%#LocRM.GetString("tCancel")%>' ImageUrl="../../layouts/images/cancel.gif" CommandName="Cancel" CausesValidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>' />
									</td>
								</tr>
							</table>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="IsCompleted" Visible="False" ReadOnly="True"></asp:BoundColumn>
					<asp:BoundColumn DataField="StateId" Visible="False" ReadOnly="True"></asp:BoundColumn>
					<asp:BoundColumn DataField="IsMilestone" Visible="False" ReadOnly="True"></asp:BoundColumn>
				</Columns>
			</dg:DataGridExtended>
		</td>
		<td valign="top" width="80%">
			<div id="divImg" onresize="_resizing(this.id, _linkToGantt)" runat="server" style="background-color: #e5e3df; overflow: hidden; position: relative; cursor: pointer;">
			</div>
			<div class="text" style="margin-top: 5px; text-align: center">
				<img alt="" src="../layouts/images/left.gif" align="textTop" style="border: 0;" />
				<asp:LinkButton ID="lbStartPrj" runat="server" CausesValidation="False">Project Start</asp:LinkButton>
				&nbsp;&nbsp;
				<asp:LinkButton ID="lbToday" runat="server" CausesValidation="False">Today</asp:LinkButton>
				&nbsp;&nbsp;
				<asp:LinkButton ID="lbEndPrj" runat="server" CausesValidation="False">Project End</asp:LinkButton>
				<img alt="" src="../layouts/images/right.gif" align="textTop" style="border: 0;" />
			</div>
		</td>
	</tr>
</table>
<div onmouseover="CancelClosing()" onmouseout="PrepareClosing(this.id)" id="divInsertMenu" style="position: absolute; top: 30px; left: 100px; z-index: 255; padding: 0px; display: none; border: 0px;" class="ibn-rtetoolbarmenu ibn-propertysheet ibn-selectedtitle">
	<table style="border-top: 1px solid #888888; border-left: 1px solid #888888; border-right: 1px solid #888888" cellpadding="2" cellspacing="0" class="text menuClass" id="tblMenuItems">
		<tr>
			<td onmouseover="Over(this)" onmouseout="Out(this)" onclick="javascript:Insert('before')">
				<%=LocRM.GetString("tInsBefore")%>
			</td>
		</tr>
		<tr>
			<td onmouseover="Over(this)" onmouseout="Out(this)" onclick="javascript:Insert('after')">
				<%=LocRM.GetString("tInsAfter")%>
			</td>
		</tr>
		<tr>
			<td onmouseover="Over(this)" onmouseout="Out(this)" onclick="javascript:Insert('first')">
				<%=LocRM.GetString("tInsFirst")%>
			</td>
		</tr>
		<tr>
			<td onmouseover="Over(this)" onmouseout="Out(this)" onclick="javascript:Insert('last')">
				<%=LocRM.GetString("tInsLast")%>
			</td>
		</tr>
	</table>
</div>
<asp:LinkButton ID="lblDeleteTaskAll" runat="server" Visible="False" OnClick="lblDeleteTaskAll_Click"></asp:LinkButton>
<input id="hdnTaskId" type="hidden" runat="server" />
<input id="hdnLinkToGantt" type="hidden" runat="server" />
