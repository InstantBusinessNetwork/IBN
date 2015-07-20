<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.MasterDataManager" Codebehind="MasterDataManager.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%"
	border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td class="ibn-alternating ibn-navline">
			<table cellpadding="7" cellspacing="0" border="0" width="100%">
				<tr height="40px">
					<td class="text" width="60px"><b><%=LocRM.GetString("tShow")%>:</b></td>
					<td width="130px"><asp:DropDownList ID="ddlShow" Runat=server Width="125px"></asp:DropDownList></td>
					<td class="text ibn-label" width="85px"><%=LocRM.GetString("FieldType")%>:</td>
					<td width="205px"><asp:dropdownlist id="ddlType" runat="server" Width="200"></asp:dropdownlist></td>
					<td align=right style="padding-right:10px">
						<div align="right">
							<INPUT class="text" id="btnApply" type="submit" runat="server" style="WIDTH: 80px" />&nbsp;
							<INPUT class="text" id="btnReset" type="submit" runat="server" style="WIDTH: 80px" />
						</div>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td vAlign="top">
			<asp:datagrid id="dgAvailableFields" Width="100%" borderwidth="0px" CellSpacing="0" gridlines="None"
				cellpadding="3" AllowSorting=True AllowPaging="False" AutoGenerateColumns="False" Runat="server">
				<HeaderStyle Height="22"></HeaderStyle>
				<Columns>
					<asp:BoundColumn Visible="False" DataField="FieldId"></asp:BoundColumn>
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
						<EditItemTemplate>
							<asp:TextBox ID="tbName" Width="95%" Runat=server CssClass="text" Text='<%# DataBinder.Eval(Container.DataItem, "FriendlyName")%>'></asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Description")%>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="tbDescription" Runat=server CssClass="text" Width="95%" Text='<%# DataBinder.Eval(Container.DataItem, "Description")%>'></asp:TextBox>
						</EditItemTemplate>
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
							<asp:imagebutton id="ibEdit" runat="server" borderwidth="0" width="16" height="16" title='<%# LocRM.GetString("Edit")%>' imageurl="~/layouts/images/edit.GIF" commandname="Edit" causesvalidation="False">
							</asp:imagebutton>&nbsp;
							<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" title='<%# LocRM.GetString("tDelete")%>' Visible='<%# (bool)(DataBinder.Eval(Container.DataItem,"CanDelete")) %>' imageurl="~/layouts/images/DELETE.GIF" commandname="Delete" causesvalidation="False">
							</asp:imagebutton>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:imagebutton id="ibUpdate" runat="server" borderwidth="0" width="16" height="16" title='<%# LocRM.GetString("Save")%>' imageurl="~/layouts/images/Saveitem.GIF" commandname="Update" causesvalidation="False">
							</asp:imagebutton>&nbsp;
							<asp:imagebutton id="ibCancel" runat="server" borderwidth="0" width="16" height="16" title='<%# LocRM.GetString("Cancel")%>' imageurl="~/layouts/images/cancel.GIF" commandname="Cancel" causesvalidation="False">
							</asp:imagebutton>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid>
		</td>
	</tr>
</table>