<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListTemplates.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.TemplateControls.ListTemplates" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Apps/Common/Design/BlockHeader.ascx" %>
<table style="margin-top:0px;" cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox">
	<tr>
		<td>
			<ibn:BlockHeader id="secHeader" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<dg:datagridextended id="grdMain" runat="server" allowsorting="True" allowpaging="True" width="100%" 
				autogeneratecolumns="False" PageSize="10" borderwidth="0" gridlines="None" cellpadding="3">
				<columns>
					<asp:boundcolumn datafield="ListId" visible="False"></asp:boundcolumn>
					<asp:TemplateColumn HeaderText="<%$Resources: IbnFramework.ListInfo, tListTemplateTitle %>">
						<ItemStyle CssClass="ibn-vb2" />
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<%#Eval("Name") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" />
						<HeaderStyle CssClass="ibn-vh-right" Width="75px" />
						<ItemTemplate>
							<a href='<%# ResolveClientUrl("~/Apps/ListApp/Pages/ListInfoEdit.aspx") + "?class=" + Eval("ClassName") %>'><img alt='' border='0' align='absmiddle' src='<%# ResolveClientUrl("~/layouts/images/edit.gif") %>' /></a>
							&nbsp;
							<a href='<%# ResolveClientUrl("~/Apps/ListApp/Pages/ListInfoView.aspx") + "?class=" + Eval("ClassName") %>'><img alt='' border='0' align='absmiddle' src='<%# ResolveClientUrl("~/layouts/images/customize.gif") %>' /></a>
							&nbsp;
							<asp:ImageButton ImageAlign="AbsMiddle" ID="btnDelete" runat="server" ImageUrl="~/images/ibnframework/delete.gif" CommandName="Delete" CommandArgument='<%#Eval("ListId") %>'></asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</columns>
			</dg:datagridextended>
		</td>
	</tr>
</table>