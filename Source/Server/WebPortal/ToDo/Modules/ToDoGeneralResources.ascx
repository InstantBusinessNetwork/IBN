<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.ToDoGeneralResources" Codebehind="ToDoGeneralResources.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px;">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
		</td>
	</tr>
</table>
<table cellspacing="0" cellpadding="1" border="0" width="100%" class="ibn-stylebox-light ibn-propertysheet" id="tblControl" runat=server>
	<tr>
		<td>
			<asp:DataGrid Runat="server" ID="dgMembers" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="3" borderwidth="0px" Width="100%" ShowHeader="true"
				oncancelcommand="dgMembers_CancelCommand" oneditcommand="dgMembers_EditCommand" 
				onupdatecommand="dgMembers_UpdateCommand" onitemdatabound="dgMembers_ItemDataBound">
				<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="UserId" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemTemplate>
							<%# GetLink( (int)Eval("UserId"),false )%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn ItemStyle-Width="110px" HeaderStyle-Width="110px">
						<ItemTemplate>
							<%# GetStatus
								(
									Eval("MustBeConfirmed"),
									Eval("ResponsePending"),
									Eval("IsConfirmed"),
									Eval("PercentCompleted")
								)%>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList runat="server" ID="PercentList"></asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle Width="50"></headerstyle>
						<itemstyle Width="50"></itemstyle>
						<itemtemplate>
							<asp:imagebutton id="ibEdit" runat="server" borderwidth="0" title='<%# LocRM.GetString("Edit")%>' imageurl="~/layouts/images/Edit.gif" commandname="Edit" causesvalidation="False" Visible='<%# CheckEdit(Eval("MustBeConfirmed"), Eval("ResponsePending"), Eval("IsConfirmed"))%>'></asp:imagebutton>
						</itemtemplate>
						<EditItemTemplate>
							<asp:imagebutton id="Imagebutton1" runat="server" borderwidth="0" title='<%# LocRM.GetString("tSave")%>' imageurl="~/layouts/images/Saveitem.gif" commandname="Update" causesvalidation="True" CommandArgument='<%# (int)Eval("UserId")%>'></asp:imagebutton>
							&nbsp;&nbsp;
							<asp:imagebutton id="Imagebutton2" runat="server" borderwidth="0" imageurl="~/layouts/images/cancel.gif" title='<%# LocRM.GetString("Cancel")%>' commandname="Cancel" causesvalidation="False"></asp:imagebutton>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:Button id="btnRefresh" runat="server" Text="Button" style="DISPLAY:none" onclick="btnRefresh_Click"></asp:Button>