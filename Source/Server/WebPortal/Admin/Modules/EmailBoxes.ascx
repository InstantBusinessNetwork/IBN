<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EmailBoxes" Codebehind="EmailBoxes.ascx.cs" %>
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
				AutoGenerateColumns="False" Runat="server">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="Id"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<a href='<%# ResolveClientUrl("~/Admin/MailIncidents.aspx") %>?MailboxId=<%# DataBinder.Eval(Container.DataItem, "Id") %>&Type=<%# GetPop3BoxType((Pop3Box)Container.DataItem) %>'>
								<%# GetType(DataBinder.Eval(Container.DataItem, "Name"), GetPop3BoxType((Pop3Box)Container.DataItem))%>
							</a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="120px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="120px"/>
						<ItemTemplate>
							<a href='<%# ResolveClientUrl("~/Admin/MailIncidents.aspx") %>?MailboxId=<%# DataBinder.Eval(Container.DataItem, "Id") %>&Type=<%# GetPop3BoxType((Pop3Box)Container.DataItem) %>'>
								<%# DataBinder.Eval(Container.DataItem, "Server") %>
							</a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="50px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"  Width="50px"/>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Port") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Login") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="100px" HorizontalAlign=Center></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="100px"/>
						<ItemTemplate>
							<img width="16px" height="16px" 
							src="<%# ((int)GetPop3BoxPropertyValue((Pop3Box)Container.DataItem, "AutoApproveForKnown"))==1? 
							ResolveClientUrl("~/layouts/images/accept.gif") : ResolveClientUrl("~/layouts/images/deny.gif") %>"/>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="100px" HorizontalAlign=Center></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="100px"/>
						<ItemTemplate>
							<img width="16px" height="16px" 
							src="<%# ((int)GetPop3BoxPropertyValue((Pop3Box)Container.DataItem, "AutoKillForUnknown"))==1? 
							ResolveClientUrl("~/layouts/images/accept.gif") : ResolveClientUrl("~/layouts/images/deny.gif") %>"/>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="100px" HorizontalAlign=Center></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="100px"/>
						<ItemTemplate>
							<img width="16px" height="16px" 
							src="<%# ((int)GetPop3BoxPropertyValue((Pop3Box)Container.DataItem, "OnlyExternalSenders"))==1? 
							ResolveClientUrl("~/layouts/images/accept.gif") : ResolveClientUrl("~/layouts/images/deny.gif") %>"/>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="25px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="25px" />
						<ItemTemplate>
							<%# ((int)GetPop3BoxType((Pop3Box)Container.DataItem)!=0) ? 
							"<a title='"+LocRM.GetString("tMoveto")+"' href='" + ResolveClientUrl("~/FileStorage/default.aspx") + "?Tab=0&FolderId="+ GetPop3BoxPropertyValue((Pop3Box)Container.DataItem, "FolderId")+"'><img border='0' width='16px' height='16px' src='" + ResolveClientUrl("~/layouts/images/folder.gif") + "' /></a>" : "" %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Right" Width="25px" CssClass="ibn-vh-right"></HeaderStyle>
						<ItemStyle Width="25px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" imageurl="~/layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
							</asp:imagebutton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:LinkButton ID="lbAddNewIncBox" CausesValidation=False Runat=server Visible=False onclick="lbAddNewIncBox_Click"></asp:LinkButton>
<asp:LinkButton ID="lbAddNewFld" CausesValidation=False Runat=server Visible=False onclick="lbAddNewFld_Click"></asp:LinkButton>