<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectTemplates" Codebehind="ProjectTemplates.ascx.cs" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<script language="javascript">
function DeleteProjectTemp(ProjectTempId)
{
	if(confirm('<%=LocRM.GetString("TemplateWarning")%>'))
	{
		document.forms[0].<%=hdnProjectTempId.ClientID %>.value = ProjectTempId;
		<%=Page.ClientScript.GetPostBackEventReference(lbDeleteProjectTemp,"") %>
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
			<dg:DataGridExtended id="dgProjectTemps" runat="server" width="100%" autogeneratecolumns="False" CellSpacing="0" gridlines="None" cellpadding="3" borderwidth="0px" allowsorting="True" pagesize="10" allowpaging="True">
				<columns>
					<asp:boundcolumn visible="False" datafield="TemplateId"></asp:boundcolumn>
					<asp:TemplateColumn SortExpression="TemplateName">
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem,"TemplateName") %>
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
					<asp:TemplateColumn SortExpression="LastSavedDate">
						<HeaderStyle CssClass="ibn-vh2" Width="120px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# ((DateTime)DataBinder.Eval(Container.DataItem, "LastSavedDate")).ToString("d") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="LastEditorId">
						<HeaderStyle CssClass="ibn-vh2" Width="170px"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "LastEditorId")) %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:templatecolumn itemstyle-width="60" Visible="True">
						<headerstyle horizontalalign="Right" cssclass="ibn-vh-right" width="60px"></headerstyle>
						<itemstyle horizontalalign="Right" CssClass="ibn-vb2" width="60px"></itemstyle>
						<itemtemplate>
							<asp:HyperLink id="ibNewPrj" runat="server" imageurl="../../layouts/images/icons/project_create.GIF" NavigateUrl='<%# "../../Projects/ProjectEdit.aspx?TemplateId=" + DataBinder.Eval(Container.DataItem, "TemplateId").ToString()%>' >
							</asp:HyperLink>&nbsp;
							<asp:HyperLink id="ibDelete" runat="server" imageurl="../../layouts/images/DELETE.GIF" NavigateUrl='<%# "javascript:DeleteProjectTemp(" + DataBinder.Eval(Container.DataItem, "TemplateId").ToString() + ")" %>' >
							</asp:HyperLink>&nbsp;
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</dg:DataGridExtended>
			<asp:linkbutton id="lbDeleteProjectTemp" runat="server" Visible="False"></asp:linkbutton>
			<INPUT id="hdnProjectTempId" type="hidden" name="hdnProjectTempId" runat="server">
		</td>
	</tr>
</table>