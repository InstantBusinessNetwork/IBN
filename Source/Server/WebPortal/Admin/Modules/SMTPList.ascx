<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SMTPList.ascx.cs" Inherits="Mediachase.UI.Web.Admin.Modules.SMTPList" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" style="width:100%">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td style="padding: 5px">
			<asp:DataGrid runat="server" ID="dgSets" AutoGenerateColumns="False" AllowPaging="False"
				AllowSorting="False" CellPadding="5" GridLines="None" CellSpacing="0" BorderWidth="0px"
				Width="100%" ShowHeader="True">
				<Columns>
					<asp:BoundColumn DataField="SmtpBoxId" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<a href='SMTPSettings.aspx?BoxId=<%#DataBinder.Eval(Container.DataItem, "SmtpBoxId")%>'>
								<%# DataBinder.Eval(Container.DataItem, "Name")%></a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Server")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Port")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "User")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<img alt="" src='<%# ResolveUrl("~/layouts/images/accept_1.gif") %>' style='display: <%# (bool)DataBinder.Eval(Container.DataItem, "IsDefault") ? "" : "none"%>'/>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle HorizontalAlign="Right" CssClass="ibn-vb2" Width="51"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh-right" Width="51" />
						<ItemTemplate>
							<a href='SMTPSettings.aspx?BoxId=<%#DataBinder.Eval(Container.DataItem, "SmtpBoxId")%>'>
								<img alt="" src='<%#ResolveUrl("~/layouts/images/edit.gif") %>'/></a>&nbsp;
							<asp:ImageButton ImageAlign="Middle" ID="ibDelete" runat="server" BorderWidth="0"
								ImageUrl="~/layouts/images/delete.gif" CommandName="Delete" CausesValidation="False">
							</asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
