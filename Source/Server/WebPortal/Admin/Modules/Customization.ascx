<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.Customization" Codebehind="Customization.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td vAlign="top" width="50%">
			<table class="ibn-propertysheet ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td><ibn:blockheader id="elementHeader" runat="server" /></td>
				</tr>
				<tr>
					<td>
						<table class="ibn-alternating ibn-navline text" cellspacing="0" cellpadding="3" width="100%" border="0">
							<tr>
								<td colspan="3">
									<span style="width:20px"><img width="16" height="16" border="0" src='<%= ResolveClientUrl("~/Layouts/Images/quicktip.gif") %>'></span><%=LocRM.GetString("tHelpSelectedFields")%>
								</td>
							</tr>
							<tr height="30">
								<td width="10"></td>
								<td noWrap class="ibn-label"><%=LocRM.GetString("SelectElement") %>:</td>
								<td width="100%"><asp:dropdownlist id="ddlSelectedElement" runat="server" AutoPostBack="True" Width="200" onselectedindexchanged="ddlSelectedElement_SelectedIndexChanged"></asp:dropdownlist></td>
							</tr>
							<tr height="30">
								<td></td>
								<td noWrap><asp:label id="lblSelectedType" runat="server" CssClass="ibn-label"></asp:label></td>
								<td><asp:dropdownlist id="ddlSelectedType" runat="server" AutoPostBack="True" Width="200" onselectedindexchanged="ddlSelectedType_SelectedIndexChanged"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td style="PADDING-RIGHT: 8px" vAlign="top">
						<asp:datagrid id="dgSelectedFields" Width="100%" borderwidth="0px" CellSpacing="0" gridlines="None"
							cellpadding="3" AllowSorting="False" AllowPaging="False" AutoGenerateColumns="False" Runat="server">
							<HeaderStyle Height="23"></HeaderStyle>
							<ItemStyle Height="28"></ItemStyle>
							<Columns>
								<asp:BoundColumn DataField="FieldId" Visible="False"></asp:BoundColumn>
								<asp:TemplateColumn>
									<HEADERSTYLE CssClass="ibn-vh2"></HEADERSTYLE>
									<ITEMSTYLE CssClass="ibn-vb2"></ITEMSTYLE>
									<ItemTemplate>
										<%#
											Mediachase.UI.Web.Util.CommonHelper.GetMetaField(
												(int)DataBinder.Eval(Container.DataItem, "FieldId"),
												"FieldView.aspx?ID=" + DataBinder.Eval(Container.DataItem, "FieldId").ToString() + "&ClassId=" + DataBinder.Eval(Container.DataItem, "ClassId").ToString())
										%>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HEADERSTYLE CssClass="ibn-vh2"></HEADERSTYLE>
									<ITEMSTYLE CssClass="ibn-vb2"></ITEMSTYLE>
									<ItemTemplate>
										<%# DataBinder.Eval(Container.DataItem, "DataType")%>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HEADERSTYLE CssClass="ibn-vh2" HorizontalAlign="Center" Width="50"></HEADERSTYLE>
									<ITEMSTYLE CssClass="ibn-vb2" HorizontalAlign="Center" Width="50"></ITEMSTYLE>
									<ItemTemplate>
										<%# GetBoolValue((bool)DataBinder.Eval(Container.DataItem, "IsRequired"))%>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:DropDownList Runat="server" ID="ddlIsRequired" Width="50"></asp:DropDownList>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:templatecolumn itemstyle-width="70" Visible="True">
									<HEADERSTYLE CssClass="ibn-vh2"></HEADERSTYLE>
									<ITEMSTYLE CssClass="ibn-vb2" HorizontalAlign="Right"></ITEMSTYLE>
									<ItemTemplate>
										<table cellpadding="0" cellspacing="0" width="70">
											<tr>
												<td align="right" width="35"><asp:imagebutton id="ibEdit" runat="server" borderwidth="0" width="16" height="16" title='<%# LocRM.GetString("Edit")%>' Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanChangeIsRequired")%>' imageurl="~/layouts/images/Edit.GIF" commandname="Edit" causesvalidation="False"></asp:imagebutton></td>
												<td align="right" width="35"><asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" title='<%# LocRM.GetString("Delete")%>' imageurl="~/layouts/images/DELETE.GIF" commandname="Delete" causesvalidation="False"></asp:imagebutton></td>
											</tr>
										</table>
									</ItemTemplate>
									<EditItemTemplate>
										<table cellpadding="0" cellspacing="0" width="70">
											<tr>
												<td align="right" width="35"><asp:imagebutton id="Imagebutton1" runat="server" borderwidth="0" width="16" height="16" title='<%# LocRM.GetString("Save")%>' imageurl="~/layouts/images/SaveItem.gif" commandname="Update" causesvalidation="True" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FieldId")%>'/></td>
												<td align="right" width="35"><asp:imagebutton id="Imagebutton2" runat="server" borderwidth="0" width="16" height="16" title='<%# LocRM.GetString("Cancel")%>' imageurl="~/layouts/images/cancel.gif" commandname="Cancel" causesvalidation="False" /></td>
											</tr>
										</table>
									</EditItemTemplate>
								</asp:templatecolumn>
							</Columns>
						</asp:datagrid>
					</td>
				</tr>
			</table>
		</td>
		<td vAlign="top" width="50%" style="PADDING-LEFT:7px">
			<table class="ibn-propertysheet ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td><ibn:blockheader id="fieldsHeader" runat="server" /></td>
				</tr>
				<tr>
					<td>
						<table class="ibn-alternating ibn-navline text" cellspacing="0" cellpadding="3" width="100%"
							border="0">
							<tr>
								<td colspan="3">
									<span style="width:20px"><img width="16" height="16" border="0" src='<%= ResolveClientUrl("~/Layouts/Images/quicktip.gif") %>'></span><%=LocRM.GetString("tHelpAvailableFields")%>
								</td>
							</tr>
							<tr height="30">
								<td width="10"></td>
								<td noWrap class="ibn-label"><%= LocRM.GetString("FieldType")%>:</td>
								<td width="100%"><asp:dropdownlist id="ddlType" runat="server" AutoPostBack="True" Width="200"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td style="PADDING-RIGHT: 8px" vAlign="top">
						<asp:datagrid id="dgAvailableFields" Width="100%" borderwidth="0px" CellSpacing="0" gridlines="None"
							cellpadding="3" AllowSorting=True AllowPaging="False" AutoGenerateColumns="False" Runat="server">
							<ItemStyle Height="28"></ItemStyle>
							<HeaderStyle Height="23"></HeaderStyle>
							<Columns>
								<asp:BoundColumn Visible="False" DataField="FieldId"></asp:BoundColumn>
								<asp:TemplateColumn>
									<HeaderStyle Width="25px" CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
									<HeaderTemplate>
										&nbsp;
										<asp:ImageButton id="btnCopy" Width="15" Runat="server" Height="15" ImageUrl="~/Layouts/Images/go_left.gif" CommandName="Copy" title='<%#LocRM.GetString("AddFields") %>'>
										</asp:ImageButton>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:checkbox id="chkItem" runat="server"></asp:checkbox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn SortExpression="FriendlyName">
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
									<ItemTemplate>
										<%#
											Mediachase.UI.Web.Util.CommonHelper.GetMetaField(
												(int)DataBinder.Eval(Container.DataItem, "FieldId"),
												"FieldView.aspx?ID=" + DataBinder.Eval(Container.DataItem, "FieldId").ToString())
										%>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn SortExpression="sortDataType">
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
									<ItemTemplate>
										<%# DataBinder.Eval(Container.DataItem, "DataType")%>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle Width="50px" CssClass="ibn-vb2"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton id="ibCopy" Width="15" Runat="server" Height="15" ImageUrl="~/Layouts/Images/go_left.gif" CommandName="CopyOne" title='<%#LocRM.GetString("tAddOneField") %>'>
										</asp:ImageButton>&nbsp;
										<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" title='<%# LocRM.GetString("Delete")%>' Visible='<%# (bool)(DataBinder.Eval(Container.DataItem,"CanDelete")) %>' imageurl="~/layouts/images/DELETE.GIF" commandname="Delete" causesvalidation="False">
										</asp:imagebutton>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</asp:datagrid>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
