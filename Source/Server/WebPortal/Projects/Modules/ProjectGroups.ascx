<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectGroups" Codebehind="ProjectGroups.ascx.cs" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<script language="javascript">
function DeleteProjectGroup(ProjectGroupId)
{
	if(confirm('<%=LocRM.GetString("PortfolioWarning")%>'))
	{
		document.forms[0].<%=hdnProjectGroupId.ClientID %>.value = ProjectGroupId;
		<%=Page.ClientScript.GetPostBackEventReference(lbDeleteProjectGroup,"") %>
	}
}
</script>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox" style="MARGIN-TOP:0px;margin-left:2px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" title="" />
		</td>
	</tr>
	<tr>
		<td>
			<dg:DataGridExtended id="dgProjectGroups" runat="server" width="100%" autogeneratecolumns="False" CellSpacing="0" gridlines="None" cellpadding="3" borderwidth="0px" allowsorting="True" pagesize="10" allowpaging="True">
				<columns>
					<asp:boundcolumn visible="False" datafield="ProjectGroupId"></asp:boundcolumn>
					<asp:TemplateColumn SortExpression="Title">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<a href='../Projects/ProjectGroupView.aspx?ProjectGroupId=<%# DataBinder.Eval(Container.DataItem,"ProjectGroupId").ToString() %>'>
								<%# DataBinder.Eval(Container.DataItem,"Title") %>
							</a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="CreationDate">
						<HeaderStyle CssClass="ibn-vh2" Width="120px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToString("d") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="CreatorId">
						<HeaderStyle CssClass="ibn-vh2" Width="170px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "CreatorId")) %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:templatecolumn itemstyle-width="60" Visible="True">
						<headerstyle CssClass="ibn-vh-right" horizontalalign="Right" width="60px"></headerstyle>
						<itemstyle CssClass="ibn-vb2" horizontalalign="Right" width="60px"></itemstyle>
						<itemtemplate>
							<asp:HyperLink ImageUrl = "../../layouts/images/Edit.GIF" NavigateUrl='<%# "~/Projects/ProjectGroupEdit.aspx?ProjectGroupID=" + DataBinder.Eval(Container.DataItem, "ProjectGroupID").ToString() %>' Runat="server" ID="Hyperlink1" NAME="Hyperlink1" ToolTip='<%#LocRM.GetString("Edit")%>'>
							</asp:HyperLink>
							&nbsp;
							<asp:HyperLink id="ibDelete" runat="server" imageurl="../../layouts/images/DELETE.GIF" NavigateUrl='<%# "javascript:DeleteProjectGroup(" + DataBinder.Eval(Container.DataItem, "ProjectGroupId").ToString() + ")" %>' ToolTip='<%#LocRM.GetString("Delete")%>' >
							</asp:HyperLink>&nbsp;
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</dg:DataGridExtended>
			<asp:linkbutton id="lbDeleteProjectGroup" runat="server" Visible="False" onclick="Delete_Click"></asp:linkbutton>
			<INPUT id="hdnProjectGroupId" type="hidden" name="hdnProjectGroupId" runat="server">
		</td>
	</tr>
</table>