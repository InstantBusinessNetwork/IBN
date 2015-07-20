<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.ReportConfig" Codebehind="ReportConfig.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<script language=javascript>
function FolderSecurity(ReportId)
	{
		ShowWizard('<%= ResolveUrl("~/Admin/ReportSecurity.aspx")%>' + '?ReportId=' + ReportId, 650, 310);
	}
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
<tr>
	<td>
		<ibn:blockheader id="tbLightRS" runat="server"></ibn:blockheader>
	</td>
</tr>
<tr>
	<td>
		<dg:datagridextended id="grdMain" runat="server" allowsorting="True" allowpaging="True" width="100%"
				autogeneratecolumns="False" borderwidth="0" gridlines="None" cellpadding="1"	PageSize="10" LayoutFixed="True">
			<columns>
				<asp:boundcolumn visible="false" datafield="Id"></asp:boundcolumn>
				<asp:boundcolumn visible="false" datafield="CategoryId"></asp:boundcolumn>
				<asp:templatecolumn sortexpression="sortName" headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2">
					<itemtemplate>
						<%# DataBinder.Eval(Container.DataItem, "Name")%>
					</itemtemplate>
				</asp:templatecolumn>
				<asp:templatecolumn HeaderStyle-Width="140px" headerstyle-cssclass="ibn-vh2" ItemStyle-Width="140px" itemstyle-cssclass="ibn-vb2">
					<itemtemplate>
						<%# DataBinder.Eval(Container.DataItem, "Type")%>
					</itemtemplate>
				</asp:templatecolumn>
				<asp:templatecolumn sortexpression="Category" headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2">
					<itemtemplate>
						<%# DataBinder.Eval(Container.DataItem, "Category")%>
					</itemtemplate>
					<edititemtemplate>
						<asp:DropDownList ID="ddCategory" Runat=server Width="90%"></asp:DropDownList>
					</edititemtemplate>
				</asp:templatecolumn>
				<asp:TemplateColumn>
					<HeaderStyle HorizontalAlign="Right" Width="51px" CssClass="ibn-vh-right"></HeaderStyle>
					<ItemStyle Width="51px" CssClass="ibn-vb2"></ItemStyle>
					<ItemTemplate>
						<asp:imagebutton ImageAlign="AbsMiddle" id="ibEdit" runat="server" borderwidth="0" title='<%# LocRM.GetString("Edit")%>' imageurl="~/layouts/images/edit.gif" commandname="Edit" causesvalidation="False">
						</asp:imagebutton>&nbsp;
						<%# DataBinder.Eval(Container.DataItem, "ActionVS")%>
					</ItemTemplate>
					<EditItemTemplate>
						<asp:imagebutton id="ibSave" runat="server" borderwidth="0" title='<%# LocRM.GetString("Save")%>' imageurl="~/layouts/images/Saveitem.gif" commandname="Update" causesvalidation="True">
						</asp:imagebutton>
						&nbsp;
						<asp:imagebutton id="ibCancel" runat="server" borderwidth="0" imageurl="~/layouts/images/cancel.gif" title='<%# LocRM.GetString("Cancel")%>' commandname="Cancel" causesvalidation="False">
						</asp:imagebutton>
					</EditItemTemplate>
				</asp:TemplateColumn>
			</columns>
		</dg:datagridextended>
	</td>
</tr>
</table>
