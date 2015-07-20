<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListSecurity.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls.ListSecurity" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="ibn" TagName="blockheader" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ register TagPrefix="lst" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
<table class="text" height="100%" cellspacing="0" cellpadding="2" width="100%" border="0">
	<tr height="22">
		<td class="boldtext" width="290" height="22"><%=LocRM.GetString("tAvailable") %> :</td>
		<td class="ibn-navframe boldtext"><%=LocRM.GetString("tSelected") %> :</td>
	</tr>
	<tr style="HEIGHT: 100%">
		<td valign="top" width="290" style="padding-left:5px">
			<!-- Groups & Users -->
			<table class="text" style="MARGIN-TOP: 5px" cellspacing="0" cellpadding="3" width="100%">
				<tr>
					<td width="9%" class="boldtext"><%=LocRM.GetString("tGroup") %>:</td>
					<td width="91%">
						<lst:indenteddropdownlist id="ddGroups" runat="server" CssClass="text" Width="190px" AutoPostBack="True" onselectedindexchanged="ddGroups_ChangeGroup"></lst:indenteddropdownlist>
					</td>
				<tr>
					<td vAlign="top" class="boldtext"><%=LocRM.GetString("tUser") %>:</td>
					<td vAlign="top">
						<asp:ListBox id="lbUsers" runat="server" Width="190px" CssClass="text" Rows="5" SelectionMode="Multiple"></asp:ListBox>
					</td>
				</tr>
				<tr>
					<td vAlign="top" class="boldtext"><%=LocRM.GetString("tRights") %>:</td>
					<td vAlign="top">
						<asp:dropdownlist id="ddRights" runat="server" Width="190px" CssClass="text"></asp:dropdownlist>
					</td>
				</tr>
				<tr>
					<td vAlign="top" height="28"></td>
					<td style="padding-top:15px">
						<Button id="btnSave" runat="server" Text="Button" style="DISPLAY:none" type="button" onserverclick="btnSave_Click">
						</Button>
						<Button id="btnAddGroup" runat="server" onclick="DisableButtons(this);" Class="text" style="width:120px;" type="button" onserverclick="btnAddGroup_Click">
						</Button>
					</td>
				</tr>
				<tr>
					<td vAlign="top" height="28"></td>
					<td style="padding-top:2px">
						<Button id="btnAdd" runat="server" onclick="DisableButtons(this);" Class="text" style="width:120px;" type="button" onserverclick="btnAdd_Click">
						</Button>
					</td>
				</tr>
			</table>
			<!-- End Groups & Users -->
		</td>
		<td vAlign="top" height="100%" class="ibn-navframe">
			<div style="OVERFLOW-Y: auto; HEIGHT: 262px">
			<asp:datagrid Width="100%" id="dgMembers" Runat="server" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="3" borderwidth="0px" OnDeleteCommand="dgMembers_DeleteCommand">
				<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="PrincipalId" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn HeaderText='Name'>
						<ItemTemplate>
							<%# GetLink( (int)DataBinder.Eval(Container.DataItem, "PrincipalId"),(bool)DataBinder.Eval(Container.DataItem, "IsGroup") )%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText='Rights'>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Rights")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:templatecolumn itemstyle-width="30" Visible="True">
						<itemtemplate>
							<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" commandname="Delete" causesvalidation="False" imageurl="~/layouts/images/DELETE.GIF">
							</asp:imagebutton>
						</itemtemplate>
					</asp:templatecolumn>
				</Columns>
			</asp:datagrid>
			</div>
		</td>
	</tr>
</table>