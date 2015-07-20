<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GanttChartPage.ascx.cs" Inherits="Mediachase.UI.Web.Projects.Modules.GanttChartPage" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageViewMenu" Src="~/Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="mc2" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>

<script type="text/javascript">
	//<![CDATA[
	try {
		window.moveTo(0, 0);
		window.resizeTo(screen.availWidth, screen.availHeight);
	}
	catch (e) { ; }
	
	var resizeFlag = false;
	function LayoutResizeHandler(sender, eventArgs) {
		var objGridTd = document.getElementById('_idGridTd');
		var divGridHeader = document.getElementById('divGridHeader');
		var divLinks = document.getElementById('divLinks');
		divGridHeader.style.width = objGridTd.clientWidth + "px";
		divLinks.style.left = objGridTd.clientWidth + "px";
		var mas = document.getElementsByTagName('div');
		var obj;
		for(var i=0; i<mas.length;i++)
			if(mas[i].getAttribute("name") == "divGridTitle")
			{
				obj = mas[i];
				break;
			}
		if (obj) {
			var off = GetTotalOffset(obj);
			var objHdrTitle = document.getElementById('divTitle');
			var objHdrStart = document.getElementById('divStart');
			var objHdrFinish = document.getElementById('divFinish');
			objHdrTitle.style.left = (off.Left + 3).toString() + "px";
			if (obj.parentNode && obj.parentNode.parentNode && obj.parentNode.parentNode.cells &&
				obj.parentNode.parentNode.cells.length > 3) {
				objHdrStart.style.display = "";
				objHdrStart.style.left = (off.Left + 3 + obj.offsetWidth).toString() + "px";
				objHdrFinish.style.display = "";
				objHdrFinish.style.left = (off.Left + 3 + obj.offsetWidth + obj.parentNode.parentNode.cells[2].clientWidth).toString() + "px";
			}
			else {
				objHdrStart.style.display = "none";
				objHdrFinish.style.display = "none";
			}
		}
		var objMainDiv = document.getElementById('<%=divImg.ClientID%>');
		objMainDiv.style.width = objMainDiv.clientWidth.toString() + "px";
		if(browseris.ie5up);
		{
			var divVertical = document.getElementById("divVertical");
			divVertical.style.overflowX = "hidden";
		}
	}
	
	function DeleteTask(TaskId)
	{
		document.forms[0].<%=hdnTaskId.ClientID %>.value = TaskId;
		if(confirm('<%=LocRM.GetString("Warning")%>'))
			<%=Page.ClientScript.GetPostBackEventReference(lblDeleteTaskAll,"") %>
	}

	var _linkToGantt = "";
	
	function f_onload(lx, portionWidth) {
		var _obj = document.getElementById('<%=hdnLinkToGantt.ClientID%>');
		_linkToGantt = _obj.value;
		BindDiv('<%=divImg.ClientID%>', '<%=divHeader.ClientID%>', _linkToGantt, lx, portionWidth);
	}
	//]]>
</script>

<mc2:McDock ID="DockTop" runat="server" Anchor="top" EnableSplitter="False" DefaultSize="50">
	<DockItems>
		<table width="100%" cellpadding="0" cellspacing="0" border="0">
			<tr>
				<td class="ibn-navline">
					<ibn:PageViewMenu ID="tbTasks" runat="server"></ibn:PageViewMenu>
				</td>
			</tr>
			<tr>
				<td style="padding: 3px;" class="ibn-propertysheet">
					<div class="text" id="divLinks" style="margin-top: 5px; position: relative;">
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
	</DockItems>
</mc2:McDock>
<div id="divGridHeader" style="height: 27px; background-color: #ffffff; border-top: 1px solid #808080; border-bottom: 1px solid #e4e4e4; float: left; overflow: hidden; position: relative;">
	<div style="position: absolute; top: 7px; height: 15px; float: left; color: #808080; overflow: hidden;" id="divTitle">
		<asp:Label ID="lblHeaderTitle" runat="server"></asp:Label>
	</div>
	<div style="position: absolute; top: 7px; height: 15px; float: left; color: #808080; overflow: hidden;" id="divStart">
		<asp:Label ID="lblHeaderStart" runat="server"></asp:Label>
	</div>
	<div style="position: absolute; top: 7px; height: 15px; float: left; color: #808080; overflow: hidden;" id="divFinish">
		<asp:Label ID="lblHeaderFinish" runat="server"></asp:Label>
	</div>
</div>
<div id="divHeader" runat="server" style="height: 29px; background-color: #e5e3df; overflow: hidden; position: relative; cursor: pointer;">
</div>
<div id="divVertical" style="background-color: #ffffff; position: absolute; top: 79px; bottom: 0px; left: 0px; right: 0px; overflow-y: auto;">
	<table cellpadding="0" cellspacing="0" border="0" width="100%">
		<tr>
			<td valign="top" id="_idGridTd">
				<dg:DataGridExtended runat="server" ID="grdTasks" AllowPaging="True" AllowSorting="False" CellPadding="0" GridLines="None" CellSpacing="0" BorderWidth="0px" ShowHeader="false" AutoGenerateColumns="False" Width="100%" PageSize="10" LayoutFixed="false">
					<ItemStyle Font-Size="6pt"></ItemStyle>
					<Columns>
						<asp:BoundColumn DataField="TaskID" ItemStyle-CssClass="pp" ReadOnly="True" Visible="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="IsSummary" Visible="False" ReadOnly="True"></asp:BoundColumn>
						<asp:BoundColumn DataField="TaskNum" ItemStyle-CssClass="pp" ReadOnly="True"></asp:BoundColumn>
						<asp:BoundColumn DataField="OutlineNumber" ItemStyle-CssClass="pp" ReadOnly="True" Visible="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="OutlineLevel" ItemStyle-CssClass="pp" ReadOnly="True" Visible="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="Title" ItemStyle-CssClass="pp" ReadOnly="True" ItemStyle-Wrap="False"></asp:BoundColumn>
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
						<asp:BoundColumn DataField="StartDate" DataFormatString="{0:d} -" ItemStyle-CssClass="pp" ItemStyle-Font-Size="7pt" ReadOnly="True" ItemStyle-Wrap="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="FinishDate" DataFormatString="{0:d}" ItemStyle-CssClass="pp" ItemStyle-Font-Size="7pt" ReadOnly="True" ItemStyle-Wrap="False"></asp:BoundColumn>
						<asp:TemplateColumn ItemStyle-CssClass="pp" ItemStyle-Wrap="False">
							<ItemTemplate>
								<nobr>
							<asp:imagebutton id="ibMove" runat="server" borderwidth="0" width="14" height="14" title='<%#LocRM.GetString("tMoveTo")%>' imageurl="../../layouts/images/MoveTo.gif" commandname="Edit" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>' style="vertical-align: middle; margin-right:5px" visible='<%# !isMSProject%>'>
							</asp:imagebutton>
							<asp:imagebutton id="ibActivate" runat="server" borderwidth="0" width="16" height="16" title='<%#LocRM.GetString("Activate")%>' imageurl="../../images/IbnFramework/activate.png" commandname="Activate" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>' style="vertical-align: middle; margin-right:5px" visible='<%# !(bool)Eval("IsCompleted") && (int)Eval("StateId") == 1%>'></asp:imagebutton>
							<asp:imagebutton id="ibDeactivate" runat="server" borderwidth="0" width="16" height="16" title='<%#LocRM.GetString("Deactivate")%>' imageurl="../../images/IbnFramework/deactivate.png" commandname="Deactivate" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>' style="vertical-align: middle; margin-right:5px" visible='<%# !(bool)Eval("IsCompleted") && (int)Eval("StateId") != 1%>'></asp:imagebutton>
							<asp:Image runat="server" ID="ActivateSpacer" ImageUrl="~/Layouts/Images/spacer.gif" Width="16" Height="1" visible='<%# (bool)Eval("IsCompleted") %>' style="margin-right:5px" />
							<asp:imagebutton id="ibComplete" runat="server" borderwidth="0" width="16" height="16" title='<%#LocRM.GetString("Complete")%>' imageurl="../../images/IbnFramework/accept.gif" commandname="Complete" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>' style="vertical-align: middle; margin-right:5px" visible='<%# !(bool)Eval("IsCompleted")%>'></asp:imagebutton>
							<asp:Image runat="server" ID="CompleteSpacer" ImageUrl="~/Layouts/Images/spacer.gif" Width="16" Height="1" visible='<%# (bool)Eval("IsCompleted")%>' style="margin-right:5px" />
							<asp:HyperLink id="Hyperlink7" runat="server" imageurl="../../layouts/images/DELETE.GIF" NavigateUrl='<%# "javascript:DeleteTask(" + DataBinder.Eval(Container.DataItem, "TaskID").ToString() + ")" %>' ToolTip='<%# LocRM.GetString("tDeleteTask") %>' Width="16px"  visible='<%# !isMSProject%>'/>
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
			<td valign="top" width="80%" id="_tdGanttDivImg">
				<div id="divImg" onresize="_resizing(this.id, _linkToGantt)" runat="server" style="background-color: #e5e3df; overflow: hidden; position: relative; cursor: pointer;">
				</div>
			</td>
		</tr>
	</table>
</div>
<input id="hdnLinkToGantt" type="hidden" runat="server" />
<asp:LinkButton ID="lbChangeView" runat="server" OnClick="lbChangeView_Click" Visible="false"></asp:LinkButton>
<input id="hdnTaskId" type="hidden" runat="server" />
<asp:LinkButton ID="lblDeleteTaskAll" runat="server" Visible="False" OnClick="lblDeleteTaskAll_Click"></asp:LinkButton>