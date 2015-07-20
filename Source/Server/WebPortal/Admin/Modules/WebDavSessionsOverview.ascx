<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebDavSessionsOverview.ascx.cs"
	Inherits="Mediachase.UI.Web.Admin.Modules.WebDavSessionsOverview" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server" Title="<%$ Resources: IbnFramework.Admin, webDavSessionList %>" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:GridView ID="dgWebDavSessions" ShowHeader="True" Width="100%" BorderWidth="0px"
				CellSpacing="0" GridLines="None" CellPadding="3" AllowPaging="False" AutoGenerateColumns="False"
				runat="server" AllowSorting="true" OnRowCommand="dgWebDavSessions_RowCommand">
				<EmptyDataTemplate>
				<asp:Label runat="server" CssClass="ibn-vb2" Text="<%$ Resources: IbnFramework.Admin, webDavSessionNoData %>"></asp:Label>
				</EmptyDataTemplate>
				<Columns>
					<asp:BoundField Visible="False" DataField="WebDavElementPropertyId" ReadOnly="True">
					</asp:BoundField>
					<asp:TemplateField ItemStyle-Width="20">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<asp:Image ID="imgIcon" Width="16" Height="16" ImageUrl='<%# "~/Common/ContentIcon.aspx?IconID=" + DataBinder.Eval(Container.DataItem, "ContentTypeId") %>' Runat="server"></asp:Image>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="FileName" HeaderText="<%$ Resources: IbnFramework.Admin, webDavSessionFileName %>">
						<ItemStyle CssClass="ibn-vb2" />
						<HeaderStyle CssClass="ibn-vh2" />
					</asp:BoundField>
					<asp:BoundField DataField="LockedBy" HeaderText="<%$ Resources: IbnFramework.Admin, webDavSessionLockBy %>">
						<ItemStyle CssClass="ibn-vb2" />
						<HeaderStyle CssClass="ibn-vh2" />
					</asp:BoundField>
					<asp:TemplateField HeaderText="<%$ Resources: IbnFramework.Admin, webDavSessionStartTime %>">
						<ItemStyle CssClass="ibn-vb2" />
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<asp:Label runat="server" Text= '<%# GetNormalizedDateTime((DateTime)Eval("StartLocking")) %>'/>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="<%$ Resources: IbnFramework.Admin, webDavSessionDuration %>">
						<ItemStyle CssClass="ibn-vb2" />
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<asp:Label runat="server" Text= '<%# GetNormalizedDuration((TimeSpan)Eval("Duration")) %>'/>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderStyle HorizontalAlign="Right" Width="55px" CssClass="ibn-vh-right"></HeaderStyle>
						<ItemStyle Width="55px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
						
							<asp:ImageButton ImageUrl="~/Layouts/images/delete.gif" Runat="server" ToolTip='<%$ Resources: IbnFramework.Admin, webDavSessionDelete%>' 
							                  Width=16 Height=16 ID="ibUnlock"  CommandName="unlock" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "WebDavElementPropertyId") %>'>
							</asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>
</table>
