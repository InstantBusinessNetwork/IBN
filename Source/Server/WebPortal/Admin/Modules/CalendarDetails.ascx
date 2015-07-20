<%@ Reference Page="~/Admin/CalendarDetails.aspx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.CalendarDetails" Codebehind="CalendarDetails.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%">
	<tr>
		<td width="50%" valign="top">
			<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td><ibn:blockheader id="secHeader" runat="server" /></td>
				</tr>
				<tr>
					<td>
						<asp:datagrid id="dgCalendar" ShowHeader="True" Width="100%" borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="3" AllowSorting="False" AllowPaging="False" AutoGenerateColumns="False" Runat="server">
							<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="DayOfWeek" Visible="False" />
								<asp:BoundColumn DataField="DayTitle">
									<ItemStyle Width="120px" CssClass="ibn-vb2 ibn-label"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Intervals">
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn>
									<ItemStyle CssClass="ibn-vb2" Width="40"></ItemStyle>
									<ItemTemplate>
										<a runat="server" Visible='<%#!isMSProject%>' href='<%#Page.ResolveUrl("~/Admin/IntervalsEditor.aspx?CalendarID=" + CalendarID + "&amp;amp;DayID=" + DataBinder.Eval(Container.DataItem, "DayOfWeek"))%>'><img alt="" title="<%=LocRM.GetString("Edit")%>" src="<%=Page.ResolveUrl("~/Layouts/Images/Edit.GIF")%>" /></a>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</asp:datagrid>
					</td>
				</tr>
			</table>
		</td>
		<td width="50%" valign="top" style="PADDING-LEFT:7px">
			<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td><ibn:blockheader id="secHeader1" runat="server" /></td>
				</tr>
				<tr>
					<td>
						<asp:datagrid id="dgCalendarEx" ShowHeader="True" Width="100%" borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="3" AllowSorting="False" AllowPaging="False" AutoGenerateColumns="False" Runat="server">
							<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="ExceptionId" Visible="False" />
								<asp:BoundColumn DataField="ExceptionInterval">
									<ItemStyle Width="240px" CssClass="ibn-vb2 ibn-label"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Intervals">
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn>
									<ItemStyle CssClass="ibn-vb2" Width="70"></ItemStyle>
									<ItemTemplate>
										<a runat="server" Visible='<%#!isMSProject%>' href='<%#Page.ResolveUrl("~/Admin/ExceptionsEditor.aspx")+"?CalendarID=" + CalendarID + "&amp;amp;ExceptionID=" + DataBinder.Eval(Container.DataItem, "ExceptionId")%>'><img alt="" title="<%=LocRM.GetString("Edit")%>" src="<%=Page.ResolveUrl("~/Layouts/Images/Edit.GIF")%>" /></a>&nbsp;&nbsp;
										<asp:imagebutton id="ibDelete" runat="server" Visible='<%# !isMSProject%>' style="vertical-align: middle" ToolTip='<%#LocRM.GetString("Delete")%>' borderwidth="0" width="16" height="16" imageurl="~/layouts/images/DELETE.GIF" causesvalidation="False" CommandName="Delete"></asp:imagebutton>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</asp:datagrid>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
