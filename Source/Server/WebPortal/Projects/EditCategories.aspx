<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Projects.EditCategories" CodeBehind="EditCategories.aspx.cs" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=LocRM.GetString("Categorization")%></title>

	<script type="text/javascript">
		//<![CDATA[
		function AddCategory(CategoryType,ButtonId)
		{
			var w = 640;
			var h = 350;
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
			var f = window.open('../Common/AddCategory.aspx?BtnID='+ButtonId+'&DictType=' + CategoryType, "AddCategory", winprops);
		}
		//]]>
	</script>

</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" method="post" runat="server" style="margin: 0px; padding: 0px;">
	<table class="text" cellspacing="6" cellpadding="0" width="100%" border="0" style="margin-top: 0">
		<tr id="trCategories" runat="server">
			<td>
				<div class="ibn-label">
					<%= LocRM.GetString("category")%>:</div>
				<div style="height: 100px; overflow-y: auto; border: 1px solid black; margin-top: 5px">
					<asp:DataGrid ID="grdCategories" runat="server" AllowSorting="False" AllowPaging="False" Width="100%" AutoGenerateColumns="False" BorderWidth="0" GridLines="None" CellPadding="0" CellSpacing="0" ShowFooter="False" ShowHeader="False">
						<Columns>
							<asp:BoundColumn Visible="false" DataField="CategoryId"></asp:BoundColumn>
							<asp:TemplateColumn ItemStyle-CssClass="text">
								<ItemTemplate>
									<asp:CheckBox runat="server" ID="chkItem" Text='<%# DataBinder.Eval(Container.DataItem, "CategoryName")%>' Checked='<%# SelectedCategories.Contains((int)DataBinder.Eval(Container.DataItem, "CategoryId"))%>'></asp:CheckBox>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
				</div>
				<div align="right">
					<button id="btnAddGeneralCategory" runat="server" style="border: 1px; padding: 0; height: 20px; width: 22px; background-color: transparent" type="button">
						<img alt="" height="20" src="../layouts/images/icons/dictionary_edit.gif" width="22" border="0" /></button>
				</div>
			</td>
		</tr>
		<tr>
			<td>
				<div class="ibn-label">
					<%= LocRM.GetString("ProjectCategory")%>:</div>
				<div style="height: 100px; overflow-y: auto; border: 1px solid black; margin-top: 5px">
					<asp:DataGrid ID="grdProjectCategories" runat="server" AllowSorting="False" AllowPaging="False" Width="100%" AutoGenerateColumns="False" BorderWidth="0" GridLines="None" CellPadding="0" CellSpacing="0" ShowFooter="False" ShowHeader="False">
						<Columns>
							<asp:BoundColumn Visible="false" DataField="CategoryId"></asp:BoundColumn>
							<asp:TemplateColumn ItemStyle-CssClass="text">
								<ItemTemplate>
									<asp:CheckBox runat="server" ID="chkItem" Text='<%# DataBinder.Eval(Container.DataItem, "CategoryName")%>' Checked='<%# SelectedProjectCategories.Contains((int)DataBinder.Eval(Container.DataItem, "CategoryId"))%>'></asp:CheckBox>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
				</div>
				<div align="right">
					<button id="btnAddProjectCategory" runat="server" style="border: 1px; padding: 0; height: 20px; width: 22px; background-color: transparent" type="button">
						<img alt="" height="20" src="../layouts/images/icons/dictionary_edit.gif" width="22" border="0" /></button>
				</div>
			</td>
		</tr>
		<tr>
			<td>
				<div class="ibn-label">
					<%= LocRM.GetString("tPortfolio")%>:</div>
				<div style="height: 100px; overflow-y: auto; border: 1px solid black; margin-top: 5px">
					<asp:DataGrid ID="grdPortfolios" runat="server" AllowSorting="False" AllowPaging="False" Width="100%" AutoGenerateColumns="False" BorderWidth="0" GridLines="None" CellPadding="0" CellSpacing="0" ShowFooter="False" ShowHeader="False">
						<Columns>
							<asp:BoundColumn Visible="false" DataField="ProjectGroupId"></asp:BoundColumn>
							<asp:TemplateColumn ItemStyle-CssClass="text">
								<ItemTemplate>
									<asp:CheckBox runat="server" ID="chkItem" Text='<%# DataBinder.Eval(Container.DataItem, "Title")%>' Checked='<%# SelectedPortfolios.Contains((int)DataBinder.Eval(Container.DataItem, "ProjectGroupId"))%>'></asp:CheckBox>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
				</div>
			</td>
		</tr>
		<tr>
			<td colspan="2" align="center">
				<btn:IMButton class="text" ID="btnSave" runat="server" style="width: 120">
				</btn:IMButton>
				<btn:IMButton class="text" ID="btnCancel" runat="server" IsDecline="true" CausesValidation="false" style="width: 120; margin-left: 20">
				</btn:IMButton>
				<asp:Button ID="btnRefresh" CausesValidation="False" runat="server" Style="visibility: hidden;"></asp:Button>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
