<%@ Reference Page="~/Admin/WebStubs.aspx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.AdmWebStubs" CodeBehind="WebStubs.ascx.cs" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:DataGrid ID="dgStubs" CellPadding="1" GridLines="Horizontal" runat="server" BorderWidth="0" AutoGenerateColumns="False" Width="100%" EnableViewState="True">
				<Columns>
					<asp:BoundColumn DataField="StubId" Visible="False" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="60" HeaderText="Icon">
						<ItemTemplate>
							<div style='background-image: url(<%=Page.ResolveUrl("~/Layouts/Images/area.gif")%>); background-repeat: no-repeat;'>
								<table border="0" cellpadding="0" cellspacing="0" style="width:40px;height:40px;background-color: transparent; table-layout: fixed;" title='<%# DataBinder.Eval(Container.DataItem, "ToolTip")%>'>
									<tr>
										<td align="center" valign="middle">
											<a href='<%# DataBinder.Eval(Container.DataItem, "Url")%>' target="_blank" style="text-decoration: none">
												<span>
													<%# GetIcon
													(
														(int)DataBinder.Eval(Container.DataItem, "HasIcon"),
														(string)DataBinder.Eval(Container.DataItem, "Abbreviation"),
														(int)DataBinder.Eval(Container.DataItem, "StubId")
													)
													%>
												</span>
											</a>
										</td>
									</tr>
								</table>
							</div>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="ToolTip" HeaderText="Title" HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2"></asp:BoundColumn>
					<asp:HyperLinkColumn DataNavigateUrlField="Url" DataTextField="Url" HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" Target="_blank"></asp:HyperLinkColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="100">
						<ItemTemplate>
							<%# GetGroups
								(
									(int)DataBinder.Eval(Container.DataItem, "StubId")
								) 
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" ItemStyle-Width="65">
						<ItemTemplate>
							<asp:HyperLink ID="ibEdit" runat="server" NavigateUrl='<%# "~/Admin/AddEditWebStub.aspx?Group=1&StubId=" + DataBinder.Eval(Container.DataItem, "StubId").ToString()%>' ToolTip='<%#LocRM.GetString("Edit")%>'><img alt='' src='<%=Page.ResolveUrl("~/Layouts/Images/Edit.GIF")%>' /></asp:HyperLink>&nbsp;&nbsp;
							<asp:ImageButton ID="ibDelete" runat="server" ImageUrl="~/Layouts/Images/DELETE.GIF" CausesValidation="False" CommandName="Delete" ToolTip='<%#LocRM.GetString("Delete") %>' AlternateText='<%#LocRM.GetString("Delete") %>'></asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
