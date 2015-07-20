<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.FieldView" Codebehind="FieldView.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" title="" runat="server" /></td>
	</tr>
	<tr>
		<td>
			<table cellpadding="6" class="text" cellspacing="0" border="0">
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("FieldName") %>:</td>
					<td><asp:Label Runat="server" ID="lblFieldName"></asp:Label></td>
				</tr>
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("FieldFriendlyName") %>:</td>
					<td><asp:Label Runat="server" ID="lblFieldFriendlyName"></asp:Label></td>
				</tr>
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("FieldDescription") %>:</td>
					<td><asp:Label Runat="server" ID="lblFieldDescription"></asp:Label></td>
				</tr>
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("FieldType") %>:</td>
					<td><asp:Label Runat="server" ID="lblFieldType"></asp:Label></td>
				</tr>
				<tr id="trAllowNulls" runat="server"> 
					<td class="ibn-label"><%=LocRM.GetString("FieldAllowNulls") %>:</td>
					<td><asp:Label Runat="server" ID="lblAllowNulls"></asp:Label></td>
				</tr>
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("FieldSaveHistory") %>:</td>
					<td><asp:Label Runat="server" ID="lblSaveHistory"></asp:Label></td>
				</tr>
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("FieldAllowSearch") %>:</td>
					<td><asp:Label Runat="server" ID="lblAllowSearch"></asp:Label></td>
				</tr>
			</table>
		</td>
	</tr>
</table>

<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" runat="server" id="tblDictItems">
	<tr>
		<td><ibn:blockheader id="secItemsHeader" runat="server" /></td>
	</tr>
	<tr>
		<td>
			<asp:DataGrid id="dgDic" runat="server" cellpadding="3" gridlines="Horizontal" borderwidth="0" autogeneratecolumns="False" width="100%" allowsorting="False">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="Id" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" Width="50"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="50"></ItemStyle>
						<ItemTemplate>
							<%# ((int)DataBinder.Eval(Container.DataItem,"Index") + 1).ToString() %>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList ID="ddIndex" CssClass="text" Width="100%" Runat="server">
							</asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem,"Value") %>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="tbName" Text='<%# DataBinder.Eval(Container.DataItem,"Value") %>' CssClass="text" Width="100%" Runat="server">
							</asp:TextBox>
							<asp:RequiredFieldValidator ID="rfName" CssClass="text" ControlToValidate="tbName" ErrorMessage='<%#LocRMDict.GetString("Required") %>' Display="Dynamic" Runat="server"/>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh2" Width="50"></headerstyle>
						<itemstyle cssclass="ibn-vb2" Width="50"></itemstyle>
						<itemtemplate>
							<asp:imagebutton id="ibMove" runat="server" borderwidth="0" title='<%# LocRMDict.GetString("Edit")%>' imageurl="~/layouts/images/Edit.gif" commandname="Edit" causesvalidation="False">
							</asp:imagebutton>
							&nbsp;&nbsp;
							<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRMDict.GetString("Delete")%>' imageurl="~/layouts/images/Delete.gif" commandname="Delete" causesvalidation="False" >
							</asp:imagebutton>
						</itemtemplate>
						<EditItemTemplate>
							<asp:imagebutton id="Imagebutton1" runat="server" borderwidth="0" title='<%# LocRMDict.GetString("Save")%>' imageurl="~/layouts/images/Saveitem.gif" commandname="Update" causesvalidation="True">
							</asp:imagebutton>
							&nbsp;&nbsp;
							<asp:imagebutton id="Imagebutton2" runat="server" borderwidth="0" imageurl="~/layouts/images/cancel.gif" title='<%# LocRMDict.GetString("Cancel")%>' commandname="Cancel" causesvalidation="False">
							</asp:imagebutton>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:Button ID="btnAddNewItem" Runat="server" CausesValidation="False" style="display:none" onclick="btnAddNewItem_Click"></asp:Button>
<asp:LinkButton ID="lbSortAsc" runat="server" Visible="false" OnClick="lbSortAsc_Click"></asp:LinkButton>
<asp:LinkButton ID="lbSortDesc" runat="server" Visible="false" OnClick="lbSortDesc_Click"></asp:LinkButton>