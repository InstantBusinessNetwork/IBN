<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.AddRelatedProject" Codebehind="AddRelatedProject.ascx.cs" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<script language="javascript">
function AddProject(ProjectId)
{
	document.forms[0].<%=hdnProjectId.ClientID %>.value = ProjectId;
	<%=Page.ClientScript.GetPostBackEventReference(lbAddProject,"") %>
}
</script>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox" style="MARGIN-TOP:0px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" title="" />
		</td>
	</tr>
	<tr>
		<td>
			<dg:DataGridExtended id="dgProjects" runat="server" EnableViewState="False" width="100%" 
				autogeneratecolumns="False" borderwidth="0px" CellSpacing="0" gridlines="None" 
				cellpadding="0" allowsorting="True" pagesize="10" allowpaging="True" 
				bordercolor="#afc9ef" HeaderStyle-BackColor="#F0F8FF">
				<columns>
					<asp:boundcolumn visible="False" datafield="ProjectId"></asp:boundcolumn>
					<asp:TemplateColumn sortexpression="Title" headertext="Title">
						<HeaderStyle></HeaderStyle>
						<HeaderTemplate>
						<table border=0 cellpadding=2 cellspacing=0 width=100% class="ibn-propertysheet" style="BORDER-right: 1px solid #afc9ef;BORDER-bottom: 1px solid #afc9ef">
								<tr>
									<td>
										<asp:LinkButton Runat=server CommandName="Sort" CommandArgument="Title"
							Text = <%# LocRM.GetString("Title") %> ID="Linkbutton1" CausesValidation=False>
										</asp:LinkButton>
									</td>
								</tr>
								<tr>
									<td>
										<table width=100% border=0 cellspacing="0" cellpadding="0" class="ibn-propertysheet">
											<tr>
												<td><asp:LinkButton Runat=server CommandName="Sort" CommandArgument="StatusId" Text =<%# LocRM.GetString("Status")%> ID="Linkbutton6" CausesValidation=False></asp:LinkButton></td>
												<td width=120>
													<asp:LinkButton Runat=server CommandName="Sort" CommandArgument="PriorityId" Text = <%# LocRM.GetString("Priority") %> ID="Linkbutton5" CausesValidation=False></asp:LinkButton>
												</td>
												<td width=120>
													<asp:LinkButton Runat=server CommandName="Sort" CommandArgument="TargetStartDate" Text = <%# LocRM.GetString("StartDate") %> ID="Linkbutton2" CausesValidation=False></asp:LinkButton>
												</td>
												<td width=120>
													<asp:LinkButton Runat=server CommandName="Sort" CommandArgument="TargetFinishDate" Text =<%# LocRM.GetString("FinishDate") %> ID="Linkbutton3" CausesValidation=False></asp:LinkButton>
												</td>
												<td width=190>
													<asp:LinkButton Runat=server CommandName="Sort" CommandArgument="ManagerName" Text =<%# LocRM.GetString("Manager") %> ID="Linkbutton4" CausesValidation=False></asp:LinkButton>
												</td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</HeaderTemplate>
						<ItemTemplate>
							<table border=0 cellpadding=2 cellspacing=0 width=100% class="ibn-propertysheet" style="BORDER-RIGHT: #afc9ef 1px solid;">
							<tr  style="FONT-SIZE: 11px;">
								<td>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetProjectStatus
								(
									(int)DataBinder.Eval(Container.DataItem, "ProjectId"),
									DataBinder.Eval(Container.DataItem, "Title").ToString(),
									(int)DataBinder.Eval(Container.DataItem, "StatusId")
								)
								%>
								</td>
							</tr>
							<tr>
								<td>
									<table width=100% border=0 cellspacing="0" cellpadding="0" class="ibn-propertysheet ibn-styleheader">
										<tr>
											<td>
												<%# DataBinder.Eval(Container.DataItem, "StatusName")%>
											</td>
											<td width=120>
												<%# DataBinder.Eval(Container.DataItem, "PriorityName")%>
											</td>
											<td width=120>
												<%# ((DateTime)DataBinder.Eval(Container.DataItem, "TargetStartDate")).ToString("d") %>
											</td>
											<td width=120>
												<%# ((DateTime)DataBinder.Eval(Container.DataItem, "TargetFinishDate")).ToString("d") %>
											</td>
											<td width=190>
												<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "ManagerId"))%>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							</table>
					</ItemTemplate>
					</asp:TemplateColumn>
					<asp:templatecolumn>
						<headerstyle width="30px"></headerstyle>
						<itemstyle cssclass="ibn-vb2" width="30px"></itemstyle>
						<itemtemplate>
							<asp:HyperLink id="ibAdd" runat="server" imageurl="../../layouts/images/icons/relprojects.GIF"  NavigateUrl='<%# "javascript:AddProject(" + DataBinder.Eval(Container.DataItem, "ProjectId").ToString() + ")" %>' >
						</asp:HyperLink>&nbsp;
					</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</dg:DataGridExtended>
		</td>
	</tr>
</table>
<INPUT id="hdnProjectId" type="hidden" name="hdnProjectId" runat="server">
<asp:linkbutton id="lbAddProject" runat="server" Visible="False" onclick="Add_Click"></asp:linkbutton>