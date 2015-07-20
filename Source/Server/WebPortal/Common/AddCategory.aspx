<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Common.AddCategory" CodeBehind="AddCategory.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\Modules\BlockHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=LocRM.GetString("tTitle")%></title>

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
	<ibn:BlockHeader ID="secHeader" Title="" runat="server"></ibn:BlockHeader>
	<table class="text" cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td>
				<asp:Button CssClass="text" ID="btnAddNewItem" runat="server" CausesValidation="False"
					Style="display: none" OnClick="btnAddNewItem_Click"></asp:Button>
				<asp:Label runat="server" ID="lblError" CssClass="ibn-error"></asp:Label>
				<div style="height: 320px; overflow: -moz-scrollbars-vertical; overflow-y: auto">
					<asp:DataGrid ID="dgDic" runat="server" CellPadding="1" GridLines="Horizontal" BorderWidth="0"
						AutoGenerateColumns="False" Width="620" AllowSorting="False">
						<ItemStyle Height="21px"></ItemStyle>
						<Columns>
							<asp:BoundColumn Visible="False" DataField="ItemId" ReadOnly="True"></asp:BoundColumn>
							<asp:TemplateColumn>
								<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
								<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								<ItemTemplate>
									<%# DataBinder.Eval(Container.DataItem,"ItemName") %>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox ID="tbName" Text='<%# DataBinder.Eval(Container.DataItem,"ItemName") %>'
										CssClass="text" Width="100%" runat="server">
									</asp:TextBox>
									<asp:RequiredFieldValidator ID="rfName" ControlToValidate="tbName" ErrorMessage='<%#LocRM.GetString("Required") %>'
										Display="Dynamic" runat="server" />
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle CssClass="ibn-vh2" Width="80"></HeaderStyle>
								<ItemStyle CssClass="ibn-vb2" Width="80"></ItemStyle>
								<ItemTemplate>
									<%# DataBinder.Eval(Container.DataItem,"Weight") %>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox ID="tbWeight" Text='<%# DataBinder.Eval(Container.DataItem,"Weight") %>'
										CssClass="text" Width="60px" runat="server"></asp:TextBox>
									<asp:RangeValidator ID="rvWeight" ErrorMessage="*" CssClass="text" runat="server"
										ControlToValidate="tbWeight" Display="Dynamic" MinimumValue="0" MaximumValue="1000"
										Type="Integer"></asp:RangeValidator>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle CssClass="ibn-vh2" Width="50"></HeaderStyle>
								<ItemStyle CssClass="ibn-vb2" Width="50"></ItemStyle>
								<ItemTemplate>
									<asp:ImageButton ID="ibMove" runat="server" BorderWidth="0" title='<%# LocRM.GetString("Edit")%>'
										ImageUrl="../layouts/images/Edit.gif" CommandName="Edit" CausesValidation="False">
									</asp:ImageButton>
									&nbsp;&nbsp;
									<asp:ImageButton ID="ibDelete" runat="server" BorderWidth="0" title='<%# LocRM.GetString("Delete")%>'
										ImageUrl="../layouts/images/Delete.gif" CommandName="Delete" CausesValidation="False"
										Visible='<%# GetVisibleStatus(DataBinder.Eval(Container.DataItem,"CanDelete")) %>'>
									</asp:ImageButton>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:ImageButton ID="Imagebutton1" runat="server" BorderWidth="0" title='<%# LocRM.GetString("Save")%>'
										ImageUrl="../layouts/images/Saveitem.gif" CommandName="Update" CausesValidation="True">
									</asp:ImageButton>
									&nbsp;&nbsp;
									<asp:ImageButton ID="Imagebutton2" runat="server" BorderWidth="0" ImageUrl="../layouts/images/cancel.gif"
										title='<%# LocRM.GetString("Cancel")%>' CommandName="Cancel" CausesValidation="False">
									</asp:ImageButton>
								</EditItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid></div>
				<button id="btnSave" runat="server" type="button" style="visibility: hidden" onserverclick="btnSave_ServerClick">
				</button>
			</td>
		</tr>
	</table>

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
