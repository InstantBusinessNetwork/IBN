<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.DocumentVersions" Codebehind="DocumentVersions.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/BlockHeaderLightWithMenu.ascx" %>
<script type="text/javascript">
function Publish(FileId)
{
	var CName = "<%= ContainerName%>";
	var CKey = "<%= ContainerKey%>";
	
	ShowWizard('PublishVersion.aspx?FileId='+ FileId +'&ContainerKey='+CKey+'&ContainerName='+CName, 400,480);
}
function Edit(FileId)
{
	var CName = "<%= ContainerName%>";
	var CKey = "<%= ContainerKey%>";

	ShowWizard('../FileStorage/FileEdit.aspx?FileId=' + FileId + '&ContainerKey=' + CKey + '&ContainerName=' + CName + '&history=0', 470, 200);
}
</script>
<table cellspacing="0" cellpadding="0" width="100%" border="0" runat="server" id="tblMain" style="margin-top:5px">
	<tr>
		<td><ibn:blockheader id="tbInfo" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table class="ibn-stylebox-light" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td style="padding:7">
						<asp:DataGrid id="grdMain" runat="server" AllowPaging="False" width="100%" autogeneratecolumns="False" borderwidth="0" GridLines="None" CellPadding="1">
							<columns>
								<asp:boundcolumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" datafield="Version" headertext="Ver.#" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="45" HeaderStyle-Width="70" HeaderStyle-HorizontalAlign="Center"></asp:boundcolumn>
								<asp:templatecolumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="16" ItemStyle-Width="16">
									<itemtemplate>
										<img alt="" src='<%# DataBinder.Eval(Container.DataItem, "Icon")%>' width="16" height="16" />
									</itemtemplate>
								</asp:templatecolumn>
								<asp:TemplateColumn>
									<HeaderStyle CssClass="ibn-vh2" />
									<ItemStyle CssClass="ibn-vb2" />
									<HeaderTemplate>
										<table border="0" cellpadding="2" cellspacing="0" width="100%" class="ibn-propertysheet" style="color:#808080;">
											<tr>
												<td><%# LocRM2.GetString("tTitle")%></td>
												<td width="70px"><%# LocRM2.GetString("tSize")%></td>
											</tr>
										</table>
									</HeaderTemplate>
									<ItemTemplate>
										<table border="0" cellpadding="2" cellspacing="0" width="100%" class="ibn-propertysheet">
											<tr>
												<td valign="top"><%# DataBinder.Eval(Container.DataItem, "Title")%></td>
												<td width="70px"><%# DataBinder.Eval(Container.DataItem, "Size")%></td>
											</tr>
											<tr>
												<td colspan="2" style="color: #555555;"><%# DataBinder.Eval(Container.DataItem, "Description")%></td>
											</tr>
										</table>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<ItemStyle CssClass="ibn-vb2" Width="50px" HorizontalAlign="Right"></ItemStyle>
									<ItemTemplate>
										<asp:HyperLink id="ibPublish" runat="server" imageurl="../../layouts/images/Publish.GIF" NavigateUrl='<%# "javascript:Publish(" + DataBinder.Eval(Container.DataItem, "Id").ToString() + ")" %>'></asp:HyperLink>&nbsp;
										<asp:HyperLink id="EditLink" runat="server" imageurl="../../images/IbnFramework/edit.GIF" NavigateUrl='<%# "javascript:Edit(" + DataBinder.Eval(Container.DataItem, "Id").ToString() + ")" %>' ToolTip='<%# LocRM.GetString("tbViewEdit") %>' Visible='<%# CanUpdateDelete() %>'></asp:HyperLink>&nbsp;
									</ItemTemplate>
								</asp:TemplateColumn>
							</columns>
						</asp:DataGrid>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>