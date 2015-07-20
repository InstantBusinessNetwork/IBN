<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivitiesByMilestones2.ascx.cs" Inherits="Mediachase.UI.Web.Projects.Modules.ActivitiesByMilestones2" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>

<script type="text/javascript">
	//<![CDATA[
	function OpenWindow(query, w, h, scroll) {
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;

		winprops = 'modal=1,resizable=0,height=' + h + ',width=' + w + ',top=' + t + ',left=' + l;
		if (scroll) winprops += ',scrollbars=1';
		var f = window.open(query, "_blank", winprops);
	}

	function CheckForCustomFilter(obj) {
		if (obj != null && obj.selectedIndex == obj.options.length - 1) {
			OpenWindow('../Projects/ProjectsByBusinessScoresPopUp.aspx', 350, 350, false);
		}

	}

	//==============GanttChart scripts======================
	var _linkToGantt = "";
	function f_onload(lx, portionWidth) {
		var obj = document.getElementById('<%=hdnLinkToGantt.ClientID%>');
		_linkToGantt = obj.value;
		BindDiv('<%=divImg.ClientID%>', _linkToGantt, -1, '', '', lx, portionWidth);
	}

	function closeLegendMenu() {
		document.getElementById('divLegendMenu').style.display = "none";
	}

	function GetTotalOffset(eSrc) {
		this.Top = 0;
		this.Left = 0;
		while (eSrc) {
			this.Top += eSrc.offsetTop;
			this.Left += eSrc.offsetLeft;
			eSrc = eSrc.offsetParent;
		}
		return this;
	}

	function ShowLegend(objId) {
		menu = document.getElementById('divLegendMenu');
		var curObj = document.getElementById(objId);
		var off = GetTotalOffset(curObj);
		menu.style.left = (off.Left - 260).toString() + "px";
		menu.style.top = off.Top.toString() + "px";
		menu.style.display = "";
	}
	//]]>
</script>

<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top: 0px">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="ibn-navline ibn-alternating text" style="padding-top: 8px" valign="top">
			<table cellpadding="5px">
				<tr>
					<td class="text" align="right">
						<b>
							<%=LocRM.GetString("tPrjGroup")%>:</b>
						<asp:DropDownList ID="ddPrjGroup" runat="server" Width="190px" CssClass="text" onchange="javascript:CheckForCustomFilter(this);">
						</asp:DropDownList>
					</td>
					<td>
					</td>
				</tr>
				<tr>
					<td class="text" align="right">
						<b>
							<%=LocRM.GetString("tCompare")%>:</b>
						<asp:DropDownList runat="server" ID="ddOrBasePlan" Width="190px">
						</asp:DropDownList>
					</td>
					<td class="text" align="right">
						/&nbsp;&nbsp;&nbsp;
						<asp:DropDownList runat="server" ID="ddBasePlan" Width="190px">
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td colspan="2" class="text" align="right">
						<btn:IMButton class="text" runat="server" ID="btnApplyFilter" style="width: 110px;">
						</btn:IMButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<table border="0" cellpadding="0" cellspacing="0" style="overflow: hidden;" width="100%">
				<tr>
					<td valign="top">
						<dg:DataGridExtended runat="server" ID="grdTasks" AllowPaging="True" AllowSorting="False" CellPadding="0" GridLines="None" CellSpacing="0" BorderWidth="0px" AutoGenerateColumns="False" Width="100%" PageSize="10" LayoutFixed="false">
							<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
							<ItemStyle Font-Size="6pt"></ItemStyle>
							<Columns>
								<asp:BoundColumn DataField="ProjectId" HeaderText="ID" ItemStyle-CssClass="pp" ReadOnly="True" Visible="False"></asp:BoundColumn>
								<asp:BoundColumn DataField="TaskId" HeaderText="ID" ItemStyle-CssClass="pp" ReadOnly="True" Visible="False"></asp:BoundColumn>
								<asp:BoundColumn DataField="IsCollapsed" Visible="False" ReadOnly="True"></asp:BoundColumn>
								<asp:TemplateColumn HeaderStyle-CssClass="pp" HeaderText="" ItemStyle-CssClass="pp" ItemStyle-Wrap="false">
									<HeaderTemplate>
										<div style="height: <%=Mediachase.IBN.Business.ResourceChart.HeaderHeight-1%>px">
											<%=LocRM.GetString("tUser")%></div>
									</HeaderTemplate>
									<ItemTemplate>
										<div style="width: 300px; overflow: hidden; height: <%=Mediachase.IBN.Business.ResourceChart.ItemHeight-1%>px">
											<%# (((int)DataBinder.Eval(Container.DataItem, "MilestonesCount")>0)? "" : "&nbsp;&nbsp;&nbsp;")%>
											<asp:ImageButton ID="btnCollapse" runat="server" CommandName="Collapse" ImageUrl='<%#ResolveUrl("~/layouts/images/minus.gif")%>' CausesValidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ProjectId")%>' Visible='<%#(int)DataBinder.Eval(Container.DataItem, "ProjectId")>0 && !(bool)DataBinder.Eval(Container.DataItem, "IsCollapsed") && (int)DataBinder.Eval(Container.DataItem, "MilestonesCount")>0%>' ImageAlign="AbsMiddle"></asp:ImageButton>
											<asp:ImageButton ID="btnExpand" runat="server" CommandName="Expand" ImageUrl='<%#ResolveUrl("~/layouts/images/plus.gif")%>' CausesValidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ProjectId")%>' Visible='<%#(int)DataBinder.Eval(Container.DataItem, "ProjectId")>0 && (bool)DataBinder.Eval(Container.DataItem, "IsCollapsed") && (int)DataBinder.Eval(Container.DataItem, "MilestonesCount")>0%>' ImageAlign="AbsMiddle"></asp:ImageButton>
											<%# (((int)DataBinder.Eval(Container.DataItem, "ProjectId")>0)? "" : "&nbsp;")%>
											<asp:HyperLink runat="server" ID="hlView" Font-Bold='<%#(int)DataBinder.Eval(Container.DataItem, "ProjectId")>0%>' CssClass="text" NavigateUrl='<%# (((int)DataBinder.Eval(Container.DataItem, "ProjectId")>0)? "../ProjectView.aspx?ProjectId="+DataBinder.Eval(Container.DataItem, "ProjectId").ToString() : "../../Tasks/TaskView.aspx?TaskId="+DataBinder.Eval(Container.DataItem, "TaskId").ToString())%>'>
											<%# DataBinder.Eval(Container.DataItem, "Title").ToString()%>
											</asp:HyperLink>
										</div>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn ItemStyle-Width="40px" HeaderStyle-CssClass="pp" HeaderText="" ItemStyle-CssClass="pp" ItemStyle-Wrap="false">
									<ItemTemplate>
										<div style="white-space: nowrap">
											<asp:ImageButton runat="server" ID="ibProjectStart" ImageUrl="../../layouts/images/left.gif" CausesValidation="False" Width="16px" Height="16px" CommandName="ProjectStart" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ProjectId")%>' Visible='<%#(int)DataBinder.Eval(Container.DataItem, "ProjectId")>0%>' ImageAlign="AbsMiddle"></asp:ImageButton>
											<asp:ImageButton runat="server" ID="ibProjectEnd" ImageUrl="../../layouts/images/right.gif" CausesValidation="False" Width="16px" Height="16px" CommandName="ProjectEnd" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ProjectId")%>' Visible='<%#(int)DataBinder.Eval(Container.DataItem, "ProjectId")>0%>' ImageAlign="AbsMiddle"></asp:ImageButton>
										</div>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</dg:DataGridExtended>
					</td>
					<td valign="top" width="750">
						<div id="divImg" onresize="_resizing(this.id, _linkToGantt)" runat="server" style="background-color: #e5e3df; overflow: hidden; position: relative; cursor: pointer;">
						</div>
						<div class="text" style="margin-top: 5; text-align: center">
							<asp:LinkButton ID="lbToday" runat="server" CausesValidation="False">Today</asp:LinkButton>
							&nbsp;&nbsp;
						</div>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<input id="hdnLinkToGantt" type="hidden" runat="server" />
<div id="divLegendMenu" style="position: absolute; width: 330px; border: solid 1px #333333; display: none;" class="ibn-rtetoolbarmenu ibn-propertysheet ibn-selectedtitle">
	<img alt="" src="../../Layouts/images/deny_black.gif" width="15" height="15" border="0" align="right" hspace="0" vspace="0" style="cursor: pointer;" class="borderWhite" onclick="closeLegendMenu()" runat="server" id="imgClose" />
	<asp:Table ID="tblLegend" runat="server" CellPadding="4" CellSpacing="0" CssClass="text">
	</asp:Table>
</div>
