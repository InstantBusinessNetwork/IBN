<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.WorksForManagers"
	Codebehind="WorksForManagers.ascx.cs" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Reference Control="~/Common/Modules/LegendControl.ascx" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="LegendControl" src="~/Common/Modules/LegendControl.ascx" %>
<table class="ibn-stylebox text" style="margin-top: 0px" cellspacing="0" cellpadding="0"
	width="100%" border="0" runat="server" id="tblMain">
	<tr runat="server" id="trHeader">
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<table id="tblFilterInfo" runat="server" class="ibn-navline text" cellspacing="0"
				cellpadding="4" width="100%" border="0">
				<tr>
					<td valign="top" style="padding-bottom: 5px;">
						<table cellspacing="3" cellpadding="0" border="0" runat="server" id="tblFilterInfoSet"
							class="text">
						</table>
					</td>
					<td valign="top" align="right" height="100%">
						<table height="100%" cellspacing="0" cellpadding="0" style="margin-top: 5px;">
							<tr>
								<td valign="top" align="right">
									<asp:Label ID="lblShowFilter" runat="server" CssClass="text"></asp:Label>
								</td>
							</tr>
							<tr>
								<td valign="bottom" style="padding-top: 10px">
									<input class="text" id="btnVResetF" type="submit" runat="server" visible="false" style="width: 120px" />
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<dg:DataGridExtended ID="dgObjects" runat="server" Width="100%" AutoGenerateColumns="False"
				BorderWidth="0px" CellSpacing="0" GridLines="None" CellPadding="0" AllowSorting="True"
				PageSize="10" AllowPaging="True" EnableViewState="false" LayoutFixed="false">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="ItemId"></asp:BoundColumn>
					<asp:BoundColumn Visible="False" DataField="StateId"></asp:BoundColumn>
					<asp:TemplateColumn SortExpression="PriorityId">
						<itemstyle cssclass="ibn-vb4"></itemstyle>
						<headerstyle cssclass="ibn-vh4" width="25px"></headerstyle>
						<itemtemplate>
							<%# GetPriorityIcon
								(
									(int)DataBinder.Eval(Container.DataItem, "ItemType"),
									(int)DataBinder.Eval(Container.DataItem, "PriorityId"),
									DataBinder.Eval(Container.DataItem, "PriorityName").ToString()
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Title" SortExpression="Title">
						<itemstyle cssclass="ibn-vb4"></itemstyle>
						<headerstyle cssclass="ibn-vh4"></headerstyle>
						<itemtemplate>
							<%# GetTitle
								(
									(int)DataBinder.Eval(Container.DataItem, "ItemId"),
									DataBinder.Eval(Container.DataItem, "Title").ToString(),
									(int)DataBinder.Eval(Container.DataItem, "ItemType"),
									DataBinder.Eval(Container.DataItem, "GroupName"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(bool)DataBinder.Eval(Container.DataItem, "IsChildToDo"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue"),
									(bool)DataBinder.Eval(Container.DataItem, "IsNewMessage")
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Type">
						<itemstyle cssclass="ibn-vb4"></itemstyle>
						<headerstyle cssclass="ibn-vh4"></headerstyle>
						<itemtemplate>
							<%# GetType
								(
									(int)DataBinder.Eval(Container.DataItem, "ItemType"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Resources">
						<itemstyle cssclass="ibn-vb4"></itemstyle>
						<headerstyle cssclass="ibn-vh4"></headerstyle>
						<itemtemplate>
							<%# GetResources
								(
									(int)DataBinder.Eval(Container.DataItem, "CompletionTypeId"),
									(int)DataBinder.Eval(Container.DataItem, "ItemType"),
									(int)DataBinder.Eval(Container.DataItem, "ItemId"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Completed">
						<itemstyle cssclass="ibn-vb4" width="110px"></itemstyle>
						<headerstyle cssclass="ibn-vh4" width="110px"></headerstyle>
						<itemtemplate>
							<%# GetPercentCompleted
								(
									(int)DataBinder.Eval(Container.DataItem, "ItemType"),
									(int)DataBinder.Eval(Container.DataItem, "PercentCompleted"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Start" SortExpression="StartDate">
						<itemstyle cssclass="ibn-vb4" width="80px"></itemstyle>
						<headerstyle cssclass="ibn-vh4" width="80px"></headerstyle>
						<itemtemplate>
							<%# GetDate
								(
									DataBinder.Eval(Container.DataItem, "StartDate"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Finish" SortExpression="FinishDate">
						<itemstyle cssclass="ibn-vb4" width="80px"></itemstyle>
						<headerstyle cssclass="ibn-vh4" width="80px"></headerstyle>
						<itemtemplate>
							<%# GetDate
								(
									DataBinder.Eval(Container.DataItem, "FinishDate"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="ActualStart" SortExpression="ActualStartDate">
						<itemstyle cssclass="ibn-vb4" width="110px"></itemstyle>
						<headerstyle cssclass="ibn-vh4" width="110px"></headerstyle>
						<itemtemplate>
							<%# GetDate
								(
									DataBinder.Eval(Container.DataItem, "ActualStartDate"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="ActualFinish" SortExpression="ActualFinishDate">
						<itemstyle cssclass="ibn-vb4" width="110px"></itemstyle>
						<headerstyle cssclass="ibn-vh4" width="110px"></headerstyle>
						<itemtemplate>
							<%# GetDate
								(
									DataBinder.Eval(Container.DataItem, "ActualFinishDate"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="CreationDate" SortExpression="CreationDate">
						<itemstyle cssclass="ibn-vb4" width="110px"></itemstyle>
						<headerstyle cssclass="ibn-vh4" width="110px"></headerstyle>
						<itemtemplate>
							<%# GetDate
								(
									DataBinder.Eval(Container.DataItem, "CreationDate"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Task Time" SortExpression="TaskTime">
						<itemstyle cssclass="ibn-vb4" width="120px"></itemstyle>
						<headerstyle cssclass="ibn-vh4" width="120px"></headerstyle>
						<itemtemplate>
							<%# GetMinutes
								(
									DataBinder.Eval(Container.DataItem, "TaskTime"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Total Minutes" SortExpression="TotalMinutes">
						<itemstyle cssclass="ibn-vb4" width="120px"></itemstyle>
						<headerstyle cssclass="ibn-vh4" width="120px"></headerstyle>
						<itemtemplate>
							<%# GetMinutes
								(
									DataBinder.Eval(Container.DataItem, "TotalMinutes"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Total Approved" SortExpression="TotalApproved">
						<itemstyle cssclass="ibn-vb4" width="120px"></itemstyle>
						<headerstyle cssclass="ibn-vh4" width="120px"></headerstyle>
						<itemtemplate>
							<%# GetMinutes
								(
									DataBinder.Eval(Container.DataItem, "TotalApproved"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</itemtemplate>
					</asp:TemplateColumn>
				</Columns>
			</dg:DataGridExtended>
			<div style="padding-bottom:5px;">
				<ibn:LegendControl runat="server" id="MainLegendControl"></ibn:LegendControl>
			</div>
		</td>
	</tr>
</table>
<asp:LinkButton ID="lbChangeViewDef" runat="server" Visible="false"></asp:LinkButton>
<asp:LinkButton ID="lbChangeViewDates" runat="server" Visible="false"></asp:LinkButton>
<asp:LinkButton ID="lbChangeViewTimes" runat="server" Visible="false"></asp:LinkButton>
<asp:Button ID="btnApplyG" Runat="server" Visible="false" CssClass="text"></asp:Button>