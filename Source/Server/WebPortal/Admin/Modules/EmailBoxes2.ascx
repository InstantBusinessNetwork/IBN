<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EmailBoxes2" Codebehind="EmailBoxes2.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Import Namespace="Mediachase.IBN.Business.Pop3" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td><asp:label id="lblError" CssClass="ibn-alerttext" style="padding-right: 10px" Runat="server" /></td>
	</tr>
	<tr>
		<td>
			<asp:datagrid id=dgBoxes ShowHeader="True" Width="100%" borderwidth="0px" 
				CellSpacing="0" gridlines="None" cellpadding="3" AllowPaging="False" 
				AutoGenerateColumns="False" AllowSorting="True" Runat="server">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="Id"></asp:BoundColumn>
					<asp:TemplateColumn sortExpression="Name">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<a href='<%# ResolveClientUrl("~/Admin/MailIncidents.aspx") %>?MailboxId=<%# DataBinder.Eval(Container.DataItem, "Id") %>'>
								<%# DataBinder.Eval(Container.DataItem, "Name")%>
							</a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn sortExpression="IsActiveInt">
						<ItemStyle CssClass="ibn-vb2" Width="30px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="30px"/>
						<ItemTemplate>
								<%# GetStatus(DataBinder.Eval(Container.DataItem, "IsActive"),DataBinder.Eval(Container.DataItem, "LastRequest"),DataBinder.Eval(Container.DataItem, "LastSuccessfulRequest"),DataBinder.Eval(Container.DataItem, "LastErrorText")) %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn sortExpression="HandlerName">
						<ItemStyle CssClass="ibn-vb2" Width="250px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"  Width="250px"/>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "HandlerName")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn sortExpression="LastRequest">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<%# ((DateTime)DataBinder.Eval(Container.DataItem, "LastRequest")==DateTime.MinValue) ? "&nbsp;" :
							DataBinder.Eval(Container.DataItem, "LastRequest") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn sortExpression="LastSuccessfulRequest">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<%# ((DateTime)DataBinder.Eval(Container.DataItem, "LastSuccessfulRequest")==DateTime.MinValue) ? "&nbsp;" :
							DataBinder.Eval(Container.DataItem, "LastSuccessfulRequest") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Right" Width="75px" CssClass="ibn-vh-right"></HeaderStyle>
						<ItemStyle Width="75px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate >
							<a href='<%# ResolveClientUrl("~/Admin/MailIncidents.aspx") %>?MailboxId=<%# DataBinder.Eval(Container.DataItem, "Id") %>'><img border='0' width='16px' src='<%# ResolveClientUrl("~/layouts/images/edit.gif") %>' height='16px' title='<%# LocRM.GetString("tEdit")%>'/></a>
							&nbsp;
							<asp:imagebutton id="ibChangeStatus" runat="server" borderwidth="0" width="16" height="16" 
							title='<%# (bool)DataBinder.Eval(Container.DataItem, "IsActive")?
							LocRM.GetString("tChangeStatusStop") : LocRM.GetString("tChangeStatusRun") %>' 
							imageurl='<%# (bool)DataBinder.Eval(Container.DataItem, "IsActive")?
							ResolveUrl("~/layouts/images/icons/status_stopped.gif") : ResolveUrl("~/layouts/images/icons/status_active.gif") %>' commandname="ChangeStatus" causesvalidation="False">
							</asp:imagebutton>
							&nbsp;
							<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" imageurl="~/layouts/images/delete.gif" commandname="Delete" causesvalidation="False" title='<%# LocRM.GetString("tDelete")%>'>
							</asp:imagebutton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:LinkButton ID="lbAddNewBox" CausesValidation=False Runat=server Visible=False onclick="lbAddNewBox_Click"></asp:LinkButton>