<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkFlowInstanceSchema.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BusinessProcess.Modules.WorkFlowInstanceSchema" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<style type="text/css">
	.block
	{
		font-weight:bold;
	}
	.inactive
	{
		color:Gray;
	}
	.active
	{
		color:Green;
	}
	.menuClass {color:#003399;}
	.menuClass TD {cursor:pointer;padding-left:5px; background-color: #efefef; border-bottom:1px solid #888888}
</style>
<script type="text/javascript">
<!--
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

function openMenu(curId, isBlock, curObj)
{
	var menuItems = document.getElementById("tblMenuItems");
	if (isBlock == "True")
	{
		menuItems.rows[2].style.display = "block";
	}
	else
	{
		menuItems.rows[2].style.display = "none";
	}
	menu = getMenu('divInsertMenu');
	menu.style.left = (getObjectLeft(curObj) - 150).toString() + "px";
	menu.style.top = (getObjectTop(curObj) - 1).toString() + "px";
	menu.style.display = "";
}
function closeMenu()
{
	getMenu('divInsertMenu').style.display = "none";
}
function getMenu(s)
{
	return document.getElementById(s)
}

function Insert(s)
{

}
function Over(obj)
{
	obj.style.backgroundColor ="#dadada";
}
function Out(obj)
{
	obj.style.backgroundColor ="#efefef";
}
//-->
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:1px">
	<tr>
		<td><ibn:blockheader id="MainHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<asp:DataGrid runat="server" ID="ActivityGrid" AllowPaging="false" AllowSorting="false" 
				CellPadding="3" CellSpacing="0" GridLines="None" BorderWidth="0" ShowHeader="true"
				AutoGenerateColumns="false" Width="100%">
				<HeaderStyle CssClass="ibn-vh" />
				<Columns>
					<asp:boundcolumn datafield="Id" ReadOnly="True" Visible="false"></asp:boundcolumn>
					<asp:boundcolumn datafield="Number" ItemStyle-CssClass="ibn-vb2" ReadOnly="True" HeaderText="#" HeaderStyle-Width="20px"></asp:boundcolumn>
					<asp:TemplateColumn ItemStyle-CssClass="ibn-vb2" HeaderText="<%$Resources: IbnFramework.BusinessProcess, Subject %>">
						<ItemTemplate>
							<%# GetSubject(Eval("Subject"), Eval("IsBlock"), Eval("State"), Eval("Level"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn ItemStyle-CssClass="ibn-vb2" HeaderText="<%$Resources: IbnFramework.Global, _mc_User %>">
						<ItemTemplate>
							<%# GetUserName(Eval("User")) %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn ItemStyle-CssClass="ibn-vb2" HeaderText="<%$Resources: IbnFramework.BusinessProcess, DueDate %>">
						<ItemTemplate>
							<%# GetDueDate(Eval("DueDate"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:templatecolumn headertext="" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="40px" ItemStyle-Width="40px" ItemStyle-Wrap="False">
						<itemtemplate>
							<table cellpadding="0" cellspacing="0" width="40px">
								<tr>
									<td style="width:20px;"><%# GetMenu(Eval("Id"), Eval("IsBlock"))%></td>
									<td style="width:20px;"><asp:imagebutton id="MoveButton" runat="server" borderwidth="0" width="14" height="14" title="<%$Resources: IbnFramework.BusinessProcess, MoveTo%>" imageurl="~/layouts/images/MoveTo.gif" commandname="Edit" causesvalidation="False" CommandArgument='<%# Eval("Id")%>'></asp:imagebutton></td>
								</tr>
							</table>
						</itemtemplate>
					</asp:templatecolumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<div onmouseover="CancelClosing()" onmouseout="PrepareClosing(this.id)" id="divInsertMenu" style="position:absolute; top:30px;left:100px; z-index:255;padding:0px;display:none; border: 0px;" class="ibn-rtetoolbarmenu ibn-propertysheet ibn-selectedtitle">
<table style="border-top: 1px solid #888888;border-left: 1px solid #888888;border-right: 1px solid #888888" cellpadding="2" cellspacing="0" class="text menuClass" id="tblMenuItems" width="180px">
	<tr>
		<td onmouseover="Over(this)" onmouseout="Out(this)" onclick="javascript:Insert('before')"><asp:Literal runat="server" ID="BeforeLiteral" Text="<%$ Resources: IbnFramework.BusinessProcess, InsertBefore %>"></asp:Literal></td>
	</tr>
	<tr>
		<td onmouseover="Over(this)" onmouseout="Out(this)" onclick="javascript:Insert('after')"><asp:Literal runat="server" ID="AfterLiteral" Text="<%$ Resources: IbnFramework.BusinessProcess, InsertAfter %>"></asp:Literal></td>
	</tr>
	<tr>
		<td onmouseover="Over(this)" onmouseout="Out(this)" onclick="javascript:Insert('child')"><asp:Literal runat="server" ID="ChildLiteral" Text="<%$ Resources: IbnFramework.BusinessProcess, InsertChild %>"></asp:Literal></td>
	</tr>
</table>
</div>