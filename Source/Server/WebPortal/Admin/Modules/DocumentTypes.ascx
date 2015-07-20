<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.DocumentTypes" Codebehind="DocumentTypes.ascx.cs" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td>
			<asp:datagrid id="dgDocType" ShowHeader="True" Width="100%" borderwidth="0px" 
			  CellSpacing="0" gridlines="None" cellpadding="3" AllowPaging="False" 
			  AutoGenerateColumns="False" Runat="server" AllowSorting="true">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="ContentTypeId" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn ItemStyle-Width="20">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<asp:Image ID="imgIcon" Width="16" Height="16" ImageUrl='<%# "~/Common/ContentIcon.aspx?IconID=" + DataBinder.Eval(Container.DataItem, "ContentTypeId") %>' Runat="server"></asp:Image>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="Extension" HeaderText="Extension" SortExpression="Extension">
						<ItemStyle CssClass="ibn-vb2" Width="100"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="100"/>
					</asp:BoundColumn>
					<asp:HyperLinkColumn DataNavigateUrlField="ContentTypeId" DataTextField="ContentTypeString" DataNavigateUrlFormatString="~/Admin/DocumentTypeEdit.aspx?ContentTypeID={0}" SortExpression="ContentTypeString">
						<ItemStyle CssClass="ibn-vb2" Width="300"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
					</asp:HyperLinkColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="100px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="100px" />
						<ItemTemplate>
							<asp:Image ID="imgWebDav" Width="16" Height="16" ImageUrl='<%# GetAllowUrl((bool)DataBinder.Eval(Container.DataItem, "AllowWebDav"))  %>' Runat="server"></asp:Image>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="100px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="100px" />
						<ItemTemplate>
							<asp:Image ID="imgNewWindow" Width="16" Height="16" ImageUrl='<%# GetAllowUrl((bool)DataBinder.Eval(Container.DataItem, "AllowNewWindow"))  %>' Runat="server"></asp:Image>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="100px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="100px" />
						<ItemTemplate>
							<asp:Image ID="imgForceDownload" Width="16" Height="16" ImageUrl='<%# GetAllowUrl((bool)DataBinder.Eval(Container.DataItem, "AllowForceDownload"))  %>' Runat="server"></asp:Image>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="FriendlyName" SortExpression="FriendlyName">
						<ItemStyle CssClass="ibn-vb2" />
						<HeaderStyle CssClass="ibn-vh2" />
					</asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Right" Width="55px" CssClass="ibn-vh-right"></HeaderStyle>
						<ItemStyle Width="55px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:HyperLink ImageUrl="~/Layouts/images/edit.gif" Runat="server" ToolTip='<%#LocRM.GetString("Edit")%>' Width=16 Height=16 NavigateUrl='<%# "~/Admin/DocumentTypeEdit.aspx?ContentTypeID=" + DataBinder.Eval(Container.DataItem, "ContentTypeId").ToString() %>' ID="Hyperlink2" NAME="Hyperlink1">
							</asp:HyperLink>&nbsp;
							<asp:ImageButton ImageUrl="~/Layouts/images/delete.gif" Runat="server" ToolTip='<%#LocRM.GetString("Delete")%>' Width=16 Height=16 CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ContentTypeId") %>' ID="ibDelete">
							</asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid>
		</td>
	</tr>
</table>