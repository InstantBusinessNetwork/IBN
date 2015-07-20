<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Modules.ControlPlace.propertypage" CodeBehind="propertypage.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=LocRM.GetString("tPPage")%></title>

	<script type="text/javascript">
		//<![CDATA[
		function disableEnterKey() {
			try {
				if (window.event.keyCode == 13 && window.event.srcElement.type != "textarea")
					window.event.keyCode = 0;
			}
			catch (e) { }
		}
		//]]>
	</script>

</head>
<body>
	<form id="Form1" method="post" runat="server">
	<table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 100%; background-color: #ECE9D8">
		<tr>
			<td align="center" style="padding: 5px;" id="tdMain" runat="server" valign="top">
				<table cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td colspan="2" style="padding-bottom: 10px">
							<table class="ibn-propertysheet ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td>
										<ibn:BlockHeader ID="blocknameHeader" runat="server" />
									</td>
								</tr>
								<tr>
									<td>
										<table class="ibn-alternating ibn-navline text" cellspacing="0" cellpadding="3" width="100%" border="0">
											<tr>
												<td class="text" style="padding-right: 10px; padding-left: 10px;">
													<asp:TextBox ID="txtBlockName" CssClass="text" Width="200px" runat="server" />
												</td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td valign="top" width="50%">
							<table class="ibn-propertysheet ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td>
										<ibn:BlockHeader ID="elementHeader" runat="server" />
									</td>
								</tr>
								<tr>
									<td>
										<table class="ibn-alternating ibn-navline text" cellspacing="0" cellpadding="3" width="100%" border="0">
											<tr>
												<td>
													<span class="IconAndText">
														<img alt="" src="../../Layouts/Images/quicktip.gif" width="16" height="16"/>
														<%=LocRM.GetString("tHelpSelectedFields")%>
													</span>
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td style="padding-right: 8px" valign="top">
										<asp:DataGrid ID="dgSelectedFields" Width="100%" BorderWidth="0px" CellSpacing="0" GridLines="None" CellPadding="3" AllowSorting="False" AllowPaging="False" AutoGenerateColumns="False" runat="server">
											<HeaderStyle Height="23"></HeaderStyle>
											<ItemStyle Height="28"></ItemStyle>
											<Columns>
												<asp:BoundColumn DataField="FieldId" Visible="False"></asp:BoundColumn>
												<asp:TemplateColumn>
													<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
													<ItemStyle CssClass="ibn-vb2"></ItemStyle>
													<ItemTemplate>
														<%#
														Mediachase.UI.Web.Util.CommonHelper.GetMetaFieldProperty(this,
															(int)DataBinder.Eval(Container.DataItem, "FieldId")
															)
														%>
													</ItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn>
													<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
													<ItemStyle CssClass="ibn-vb2"></ItemStyle>
													<ItemTemplate>
														<%# DataBinder.Eval(Container.DataItem, "DataType")%>
													</ItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn Visible="True">
													<HeaderStyle CssClass="ibn-vh2" HorizontalAlign="Center" Width="50"></HeaderStyle>
													<ItemStyle CssClass="ibn-vb2" HorizontalAlign="Center" Width="50"></ItemStyle>
													<ItemTemplate>
														<%# DataBinder.Eval(Container.DataItem, "Weight")%>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:DropDownList runat="server" ID="ddlPosition" Width="50">
														</asp:DropDownList>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn ItemStyle-Width="70" Visible="True">
													<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
													<ItemStyle CssClass="ibn-vb2" HorizontalAlign="Right"></ItemStyle>
													<ItemTemplate>
														<asp:ImageButton ID="ibEdit" runat="server" BorderWidth="0" Width="16" Height="16" title='<%# LocRM.GetString("Edit")%>' ImageUrl="../../layouts/images/Edit.GIF" CommandName="Edit" CausesValidation="False"></asp:ImageButton>
														&nbsp;
														<asp:ImageButton ID="ibDelete" runat="server" BorderWidth="0" Width="16" Height="16" title='<%# LocRM.GetString("Delete")%>' ImageUrl="../../layouts/images/DELETE.GIF" CommandName="Delete" CausesValidation="False"></asp:ImageButton>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:ImageButton ID="Imagebutton1" runat="server" BorderWidth="0" Width="16" Height="16" title='<%# LocRM.GetString("Save")%>' ImageUrl="../../layouts/images/SaveItem.gif" CommandName="Update" CausesValidation="True" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FieldName")%>' />
														&nbsp;
														<asp:ImageButton ID="Imagebutton2" runat="server" BorderWidth="0" Width="16" Height="16" title='<%# LocRM.GetString("Cancel")%>' ImageUrl="../../layouts/images/cancel.gif" CommandName="Cancel" CausesValidation="False" />
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:BoundColumn Visible="False" DataField="FieldName"></asp:BoundColumn>
											</Columns>
										</asp:DataGrid>
									</td>
								</tr>
							</table>
						</td>
						<td valign="top" width="50%" style="padding-left: 7px">
							<table class="ibn-propertysheet ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td>
										<ibn:BlockHeader ID="fieldsHeader" runat="server" />
									</td>
								</tr>
								<tr>
									<td>
										<table class="ibn-alternating ibn-navline text" cellspacing="0" cellpadding="3" width="100%" border="0">
											<tr>
												<td colspan="3">
													<span class="IconAndText">
														<img alt="" src="../../Layouts/Images/quicktip.gif" width="16" height="16"/>
														<%=LocRM.GetString("tHelpAvailableFields")%>
													</span>
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td style="padding-right: 8px" valign="top">
										<asp:DataGrid ID="dgAvailableFields" Width="100%" BorderWidth="0px" CellSpacing="0" GridLines="None" CellPadding="3" AllowSorting="True" AllowPaging="False" AutoGenerateColumns="False" runat="server">
											<ItemStyle Height="28"></ItemStyle>
											<HeaderStyle Height="23"></HeaderStyle>
											<Columns>
												<asp:BoundColumn Visible="False" DataField="FieldId"></asp:BoundColumn>
												<asp:TemplateColumn>
													<HeaderStyle Width="25px" CssClass="ibn-vh2"></HeaderStyle>
													<ItemStyle CssClass="ibn-vb2"></ItemStyle>
													<HeaderTemplate>
														&nbsp;
														<asp:ImageButton ID="btnCopy" Width="15" runat="server" Height="15" ImageUrl="../../Layouts/Images/go_left.gif" CommandName="Copy" title='<%#LocRM.GetString("AddFields") %>'></asp:ImageButton>
													</HeaderTemplate>
													<ItemTemplate>
														<asp:CheckBox ID="chkItem" runat="server"></asp:CheckBox>
													</ItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn SortExpression="FriendlyName">
													<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
													<ItemStyle CssClass="ibn-vb2"></ItemStyle>
													<ItemTemplate>
														<%#
														Mediachase.UI.Web.Util.CommonHelper.GetMetaFieldProperty(this,
															(int)DataBinder.Eval(Container.DataItem, "FieldId")
															)
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
														<asp:ImageButton ID="ibCopy" Width="15" runat="server" Height="15" ImageUrl="../../Layouts/Images/go_left.gif" CommandName="CopyOne" title='<%#LocRM.GetString("tAddOneField") %>'></asp:ImageButton>&nbsp;
														<asp:ImageButton ID="Imagebutton3" runat="server" BorderWidth="0" Width="16" Height="16" title='<%# LocRM.GetString("Delete")%>' Visible='<%# (bool)(DataBinder.Eval(Container.DataItem,"CanDelete")) %>' ImageUrl="../../layouts/images/DELETE.GIF" CommandName="Delete" CausesValidation="False"></asp:ImageButton>
													</ItemTemplate>
												</asp:TemplateColumn>
												<asp:BoundColumn Visible="False" DataField="FieldName"></asp:BoundColumn>
											</Columns>
										</asp:DataGrid>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td align="right" style="padding: 0 5 5 5;">
				<asp:Label ID="lblError" CssClass="text" Style="color: red; padding-right: 10px" runat="server" />
				<btn:IMButton class="text" ID="btnSave" runat="server" style="width: 110px;" OnServerClick="btnSave_Click">
				</btn:IMButton>
				&nbsp;&nbsp;
				<btn:IMButton class="text" CausesValidation="false" ID="btnCancel" runat="server" style="width: 110px;" IsDecline="true">
				</btn:IMButton>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
