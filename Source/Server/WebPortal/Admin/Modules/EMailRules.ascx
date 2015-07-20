<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailRules" Codebehind="EMailRules.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox text" style="MARGIN-TOP: 0px" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="padding: 5px">
			<asp:DataGrid Runat="server" ID="dgRules" AutoGenerateColumns="False" 
				AllowPaging="False" AllowSorting="False" cellpadding="5" 
				gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" 
				ShowHeader="True">
				<Columns>
					<asp:BoundColumn DataField="IncidentBoxId" Visible="False"></asp:BoundColumn>
					<asp:BoundColumn DataField="Index" ReadOnly="True" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2" HeaderStyle-Width="40px"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<%#DataBinder.Eval(Container.DataItem, "Rules")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<a href='EMailIssueBoxView.aspx?IssBoxId=<%#DataBinder.Eval(Container.DataItem, "IncidentBoxId")%>'>
								<%#DataBinder.Eval(Container.DataItem, "Name")%>
							</a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="67"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh-right" Width="67" />
						<ItemTemplate>
							<asp:imagebutton id="ibMove" runat="server" title='<%#LocRM.GetString("tChangeIndex")%>' borderwidth="0" width="14" height="14" imageurl="~/layouts/images/MoveTo.gif" commandname="Edit" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IncidentBoxId")%>' ImageAlign="AbsMiddle">
							</asp:imagebutton>&nbsp;&nbsp;
							<%#GetRuleButton((int)DataBinder.Eval(Container.DataItem, "IncidentBoxId"))%>&nbsp;
							<asp:ImageButton ID="ibDelete" Runat="server" title='<%#LocRM.GetString("tDelete")%>' CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IncidentBoxId")%>' borderwidth="0" width="16" height="16" imageurl="~/layouts/images/delete.gif" CommandName="Delete" ImageAlign="AbsMiddle"></asp:ImageButton>
						</ItemTemplate>
						<EditItemTemplate>
							<table cellpadding="0" cellspacing="0" border="0">
								<tr>
									<td>
										<asp:DropDownList ID="ddl" Runat="server" CssClass="text" Font-Size="10px"></asp:DropDownList></td>
									<td style="padding-left:5">
										<asp:imagebutton id="Imagebutton1" runat="server" borderwidth="0" width="16" height="16" title='<%#LocRM.GetString("tSave")%>' imageurl="~/layouts/images/SaveItem.gif" commandname="Update" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IncidentBoxId")%>'/></td>
									<td style="padding-left:5">
										<asp:imagebutton id="Imagebutton2" runat="server" borderwidth="0" width="16" height="16" title='<%#LocRM.GetString("tCancel")%>' imageurl="~/layouts/images/cancel.gif" commandname="Cancel" causesvalidation="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IncidentBoxId")%>'/></td>
								</tr>
							</table>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>