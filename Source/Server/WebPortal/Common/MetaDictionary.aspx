<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Common.MetaDictionary" CodeBehind="MetaDictionary.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	<title><%=LocRMDict.GetString("DictItems")%></title>
	

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
<body class="UserBackground" id="pT_body" style="margin:0">
	<form id="frmMain" runat="server">
	<ibn:BlockHeader ID="secItemsHeader" Title="" runat="server"></ibn:BlockHeader>
	<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
		<tr>
			<td>
				<div style="height: 320px; overflow: -moz-scrollbars-vertical; overflow-y: auto">
					<asp:DataGrid ID="dgDic" runat="server" CellPadding="1" GridLines="Horizontal" BorderWidth="0"
						AutoGenerateColumns="False" Width="100%" AllowSorting="False">
						<ItemStyle Height="21px"></ItemStyle>
						<Columns>
							<asp:BoundColumn Visible="False" DataField="Id" ReadOnly="True"></asp:BoundColumn>
							<asp:TemplateColumn>
								<HeaderStyle CssClass="ibn-vh2" Width="50"></HeaderStyle>
								<ItemStyle CssClass="ibn-vb2" Width="50"></ItemStyle>
								<ItemTemplate>
									<%# ((int)DataBinder.Eval(Container.DataItem,"Index") + 1).ToString() %>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:DropDownList ID="ddIndex" CssClass="text" Width="100%" runat="server">
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
									<asp:TextBox ID="tbName" Text='<%# DataBinder.Eval(Container.DataItem,"Value") %>'
										CssClass="text" Width="100%" runat="server">
									</asp:TextBox>
									<asp:RequiredFieldValidator ID="rfName" ControlToValidate="tbName" ErrorMessage='<%#LocRMDict.GetString("Required") %>'
										Display="Dynamic" runat="server" />
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle CssClass="ibn-vh2" Width="50"></HeaderStyle>
								<ItemStyle CssClass="ibn-vb2" Width="50"></ItemStyle>
								<ItemTemplate>
									<asp:ImageButton ID="ibMove" runat="server" BorderWidth="0" title='<%# LocRMDict.GetString("Edit")%>'
										ImageUrl="../layouts/images/Edit.gif" CommandName="Edit" CausesValidation="False">
									</asp:ImageButton>
									&nbsp;&nbsp;
									<asp:ImageButton ID="ibDelete" runat="server" BorderWidth="0" title='<%# LocRMDict.GetString("Delete")%>'
										ImageUrl="../layouts/images/Delete.gif" CommandName="Delete" CausesValidation="False">
									</asp:ImageButton>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:ImageButton ID="Imagebutton1" runat="server" BorderWidth="0" title='<%# LocRMDict.GetString("Save")%>'
										ImageUrl="../layouts/images/Saveitem.gif" CommandName="Update" CausesValidation="True">
									</asp:ImageButton>
									&nbsp;&nbsp;
									<asp:ImageButton ID="Imagebutton2" runat="server" BorderWidth="0" ImageUrl="../layouts/images/cancel.gif"
										title='<%# LocRMDict.GetString("Cancel")%>' CommandName="Cancel" CausesValidation="False">
									</asp:ImageButton>
								</EditItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
				</div>
				<button id="btnSave" runat="server" type="button" style="visibility: hidden" onserverclick="btnSave_ServerClick">
				</button>
			</td>
		</tr>
	</table>
	<asp:Button CssClass="text" ID="btnAddNewItem" runat="server" CausesValidation="False"
		Style="display: none" OnClick="btnAddNewItem_Click"></asp:Button>
	<asp:LinkButton ID="lbSortAsc" runat="server" Visible="false" OnClick="lbSortAsc_Click"></asp:LinkButton>
	<asp:LinkButton ID="lbSortDesc" runat="server" Visible="false" OnClick="lbSortDesc_Click"></asp:LinkButton>

	<script type="text/javascript">
				//<![CDATA[
				function FuncSave()
				{
					DisableButtons(document.forms[0].<%=btnSave.ClientID%>);
					<%=Page.ClientScript.GetPostBackEventReference(btnSave,"") %>;
				}
				//]]>
	</script>

	</form>
</body>
</html>
