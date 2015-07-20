<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/Separator1.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.UserStubs" Codebehind="UserStubs.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx"%>
<%@ Register TagPrefix="ibn" TagName="sep" src="..\..\Modules\Separator1.ascx"%>
<ibn:blockheader id="tbHeader" runat="server"></ibn:blockheader>
<table class="ibn-stylebox-light" cellspacing="0" id="tblMain" runat="server" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:sep id=sep1 title="" runat="server" IsClickable="false"></ibn:sep>
			<asp:datagrid id="dgStubs" CellPadding="1" GridLines="Horizontal" runat="server" borderwidth="0" autogeneratecolumns="False" width="100%" enableviewstate="True">
				<Columns>
					<asp:BoundColumn DataField="StubId" Visible=False ReadOnly=True></asp:BoundColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="60" HeaderText="Icon">
						<ItemTemplate>
							<div style="background-image: url(../Layouts/images/area.gif);background-repeat: no-repeat;">
								<table border="0" cellpadding="0" cellspacing="0" width="40" height="40" style="background-color: transparent; table-layout: fixed;" title='<%# DataBinder.Eval(Container.DataItem, "ToolTip")%>'>
									<tr>
										<td align="center" valign="middle">
											<a href='<%# DataBinder.Eval(Container.DataItem, "Url")%>' target="_blank" style="text-decoration:none">
												<div id="pdiv">
													<%# GetIcon
										(
											(int)DataBinder.Eval(Container.DataItem, "HasIcon"),
											(string)DataBinder.Eval(Container.DataItem, "Abbreviation"),
											(int)DataBinder.Eval(Container.DataItem, "StubId")
										)
										%>
												</div>
											</a>
										</td>
									</tr>
								</table>
							</div>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="ToolTip" HeaderText="Title" HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2"></asp:BoundColumn>
					<asp:HyperLinkColumn DataNavigateUrlField="Url" DataTextField="Url" HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" Target="_blank"></asp:HyperLinkColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="100" HeaderText="Icon">
						<ItemTemplate>
							<%# GetGroups
								(
									(int)DataBinder.Eval(Container.DataItem, "StubId")
								) 
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="90" HeaderText="Icon">
						<ItemTemplate>
							<%# GetActive
								(
									(int)DataBinder.Eval(Container.DataItem, "IsVisible")
								) 
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" ItemStyle-Width="65">
						<itemtemplate>
							<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" imageurl="../../layouts/images/icons/webstub_change.GIF" causesvalidation="False" CommandName="Update" ToolTip='<%#LocRM.GetString("ChangeVisibible") %>' CommandArgument='<%#DataBinder.Eval(Container.DataItem, "IsVisible")%>'>
							</asp:imagebutton>
						</itemtemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid>
			<ibn:sep id=sep2 title="" runat="server" IsClickable="false"></ibn:sep>
			<asp:datagrid id="dgUserStubs" CellPadding="1" GridLines="Horizontal" runat="server" borderwidth="0" autogeneratecolumns="False" width="100%" enableviewstate="True">
				<Columns>
					<asp:BoundColumn DataField="StubId" Visible=False ReadOnly=True></asp:BoundColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="60" HeaderText="Icon">
						<ItemTemplate>
							<div style="background-image: url(../Layouts/images/area.gif);background-repeat: no-repeat;">
								<table border="0" cellpadding="0" cellspacing="0" width="40" height="40" style="background-color: transparent; table-layout: fixed;" title='<%# DataBinder.Eval(Container.DataItem, "ToolTip")%>'>
									<tr>
										<td align="center" valign="middle">
											<a href='<%# DataBinder.Eval(Container.DataItem, "Url")%>' target="_blank" style="text-decoration:none">
												<div id="pdiv">
													<%# GetIcon
										(
											(int)DataBinder.Eval(Container.DataItem, "HasIcon"),
											(string)DataBinder.Eval(Container.DataItem, "Abbreviation"),
											(int)DataBinder.Eval(Container.DataItem, "StubId")
										)
										%>
												</div>
											</a>
										</td>
									</tr>
								</table>
							</div>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="ToolTip" HeaderText="Title" HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2"></asp:BoundColumn>
					<asp:HyperLinkColumn DataNavigateUrlField="Url" DataTextField="Url" HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" Target="_blank"></asp:HyperLinkColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" ItemStyle-Width="65">
						<itemtemplate>
							<asp:HyperLink id="ibEdit" runat="server" imageurl="../../layouts/images/Edit.GIF" NavigateUrl='<%# "~/Admin/AddEditWebStub.aspx?Group=0&StubId=" + DataBinder.Eval(Container.DataItem, "StubId").ToString()%>' ToolTip='<%#LocRM.GetString("Edit") %>'>
							</asp:HyperLink>&nbsp;&nbsp;
							<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" imageurl="../../layouts/images/DELETE.GIF" causesvalidation="False" CommandName="Delete" ToolTip='<%#LocRM.GetString("Delete") %>'>
							</asp:imagebutton>
						</itemtemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid>
		</td>
	</tr>
</table>
