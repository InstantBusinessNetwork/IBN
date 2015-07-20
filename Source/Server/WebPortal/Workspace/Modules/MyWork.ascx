<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyWork.ascx.cs" Inherits="Mediachase.UI.Web.Workspace.Modules.MyWork" %>
<%@ Import Namespace="Mediachase.UI.Web.Util" %>
<table class="ibn-propertysheet ibn-stylebox2" style="MARGIN:0;" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td class="ibn-propertysheet">
			<asp:Label ID="NoItemsLabel" Runat="server" CssClass="text" ForeColor="gray"></asp:Label>
			<asp:DataGrid id="MainGrid" Runat="server" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False"
				AllowPaging="true" AllowSorting="False" CellSpacing="0" PageSize="10" PagerStyle-Visible="true"
				PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2"
				ShowHeader="true" EnableViewState="True" OnPageIndexChanged="MainGrid_PageIndexChanged">
				<PagerStyle CssClass="text ibn-TPHeader" HorizontalAlign="Right"></PagerStyle>
				<Columns>
					<asp:TemplateColumn SortExpression="PriorityId">
						<HeaderStyle CssClass="ibn-vh2" Width="16"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="16"></ItemStyle>
						<ItemTemplate>
							<%# CommonHelper.GetPriorityIcon((int)Eval("PriorityId"), this.Page) %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2-hid"></ItemStyle>
						<ItemTemplate>
							<%#
							((bool)Eval("IsNewMessage") ? "<b>" : "") +
							CommonHelper.GetObjectLinkWithIcon
							(
								(int)Eval("ObjectTypeId"),
								(int)Eval("ObjectId"),
								Eval("ObjectName").ToString(),
								(int)Eval("StateId"),
								(bool)Eval("IsOverdue"),
								this.Page					
							)
							+ ((bool)Eval("IsNewMessage") ? "</b>" : "")
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" Width="38px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="38px"></ItemStyle>
						<ItemTemplate>
							<%# 
								(Eval("PercentCompleted") != DBNull.Value)
								? Eval("PercentCompleted").ToString() + "%"
								: "" 
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" Width="80px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="80px"></ItemStyle>
						<ItemTemplate>
							<%# ((DateTime)Eval("FinishDate") < DateTime.UtcNow.AddYears(10))
								? ((DateTime)Eval("FinishDate") < Mediachase.IBN.Business.Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow) ? "<span style='color:red'>" + ((DateTime)Eval("FinishDate")).ToShortDateString() + "</span>" : ((DateTime)Eval("FinishDate")).ToShortDateString())
								: ""
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
			<asp:Label ID="NoCalendarLabel" Runat="server" CssClass="text"></asp:Label>
		</td>
	</tr>
</table>