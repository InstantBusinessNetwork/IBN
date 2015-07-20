<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.calendarList" Codebehind="CalendarList.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td>
			<asp:DataGrid Runat="server" ID="dgCalendar" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" ShowHeader="True">
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:BoundColumn Visible="False" DataField="CalendarId" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:HyperLink NavigateUrl='<%# "~/Admin/CalendarDetails.aspx?CalendarID=" + DataBinder.Eval(Container.DataItem, "CalendarId").ToString() %>' Runat="server" ID="hlToDetails">
								<%# DataBinder.Eval(Container.DataItem, "CalendarName") %>
							</asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:HyperLink NavigateUrl='<%# "~/Projects/ProjectView.aspx?ProjectId=" + DataBinder.Eval(Container.DataItem, "ProjectId").ToString() %>' Runat="server" ID="Hyperlink1">
								<%# DataBinder.Eval(Container.DataItem, "ProjectName") %>
							</asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle Width="100px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# ShowTimeZone((int)DataBinder.Eval(Container.DataItem, "TimeZoneId")) %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn Visible="False" HeaderText="ProjectId" DataField="ProjectId" ReadOnly="True"></asp:BoundColumn> 
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Right" Width="85px" CssClass="ibn-vh-right"></HeaderStyle>
						<ItemStyle Width="85px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<a href='<%#Page.ResolveUrl("~/Admin/AddCalendar.aspx?CalendarID=" + DataBinder.Eval(Container.DataItem, "CalendarId").ToString() +"&amp;Copy=1")%>'><img alt="" title="<%=LocRM.GetString("Copy")%>" src="<%=Page.ResolveUrl("~/Layouts/Images/Moveto.GIF")%>" /></a>
							<a href='<%#Page.ResolveUrl("~/Admin/AddCalendar.aspx?CalendarID=" + DataBinder.Eval(Container.DataItem, "CalendarId").ToString())%>'><img alt="" title="<%=LocRM.GetString("Edit")%>" src="<%=Page.ResolveUrl("~/Layouts/Images/Edit.GIF")%>" /></a>
							<asp:imagebutton id="ibDelete" runat="server" style="vertical-align: middle" borderwidth="0" width="16" height="16" imageurl="~/layouts/images/DELETE.GIF" causesvalidation="False" CommandName="Delete" Visible='<%# ((int)DataBinder.Eval(Container.DataItem, "CanDelete") == 1) ? true : false %>'></asp:imagebutton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:Button id="btnRefresh" runat="server" Text="Button" style="DISPLAY:none" onclick="btnRefresh_Click"></asp:Button>
