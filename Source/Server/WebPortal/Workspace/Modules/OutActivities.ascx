<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutActivities.ascx.cs" Inherits="Mediachase.UI.Web.Workspace.Modules.OutActivities" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx"%>
<table class="ibn-propertysheet ibn-stylebox2" style="MARGIN:0;" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" title="" runat="server" Visible="False"></ibn:blockheader></td>
	</tr>
	<tr>
		<td class="ibn-alternating ibn-navline" style="padding:3px;" align="right">
			<a href="javascript:ShowWizard(&quot;../Workspace/ActivitiesSettings.aspx?prefix=Out&amp;btn=<%= Page.ClientScript.GetPostBackEventReference(buttonRefresh, "")%>&quot;, 350, 260)"><asp:Image runat="server" ID="imageFilter" ImageUrl="~/layouts/images/Filter.GIF" BorderWidth="0" Height="16" Width="16" ImageAlign="AbsMiddle" /> <asp:Label runat="server" ID="labelLegend"></asp:Label></a>
		</td>
	</tr>
	<tr>
		<td class="ibn-propertysheet" valign="top">
			<asp:Label ID="labelNoItems" Runat="server" CssClass="text" ForeColor="gray"></asp:Label>
			<asp:DataGrid id="gridActivities" Runat="server" PagerStyle-Mode="NumericPages" AutoGenerateColumns="False"
				AllowPaging="true" AllowSorting="True" CellSpacing="0" PageSize="10" PagerStyle-Visible="true"
				PagerStyle-HorizontalAlign="Right" Width="100%" GridLines="None" borderwidth="0px" cellpadding="2"
				ShowHeader="true" EnableViewState="True" OnPageIndexChanged="GridActivities_PageIndexChanged" OnSortCommand="GridActivities_SortCommand">
				<PagerStyle CssClass="text ibn-TPHeader" HorizontalAlign="Right"></PagerStyle>
				<Columns>
					<asp:TemplateColumn SortExpression="Title">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2-hid"></ItemStyle>
						<ItemTemplate>
							<%#
							Mediachase.UI.Web.Util.CommonHelper.GetTaskToDoLink 
							(
								(int)DataBinder.Eval(Container.DataItem, "ItemId"),
								(int)DataBinder.Eval(Container.DataItem, "IsToDo"),
								DataBinder.Eval(Container.DataItem, "Title").ToString(),
								(int)DataBinder.Eval(Container.DataItem, "StateId")
							)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" Width="225px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="225px"></ItemStyle>
						<ItemTemplate>
							<%# GetResources
								(
									(int)DataBinder.Eval(Container.DataItem, "CompletionTypeId"),
									(int)DataBinder.Eval(Container.DataItem, "IsToDo"),
									(int)DataBinder.Eval(Container.DataItem, "ItemId"),
									(int)DataBinder.Eval(Container.DataItem, "PercentCompleted")					
								)
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="SortFinishDate">
						<HeaderStyle CssClass="ibn-vh2" Width="80px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="80px"></ItemStyle>
						<ItemTemplate>
							<%# (DataBinder.Eval(Container.DataItem,"FinishDate")!=DBNull.Value) 
									? ((DateTime)DataBinder.Eval(Container.DataItem,"FinishDate")).ToShortDateString() 
									: ""
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:LinkButton id="buttonHide" Runat="server" Visible="False" OnClick="ButtonHide_Click"></asp:LinkButton>
<asp:LinkButton id="buttonRefresh" Runat="server" Visible="False" OnClick="ButtonRefresh_Click"></asp:LinkButton>
