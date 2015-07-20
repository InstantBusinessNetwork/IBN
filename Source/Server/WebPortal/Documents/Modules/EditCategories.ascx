<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.EditCategories" Codebehind="EditCategories.ascx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<script language="javascript">
	function AddCategory(CategoryType,ButtonId)
	{
			var w = 640;
			var h = 350;
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
			var f = window.open('../Common/AddCategory.aspx?BtnID='+ButtonId+'&DictType=' + CategoryType, "AddCategory", winprops);
	}
</script>
<table class="text" cellspacing="6" cellpadding="0" width="100%" border="0" style="margin-top:0">
	<tr>
		<td>
			<div class="ibn-label"><%= LocRM.GetString("category")%>:</div>
			<div style="height:145px; overflow-y: auto; border: 1px solid black; margin-top:5px">
				<asp:DataGrid id="grdCategories" runat="server" allowsorting="False" allowpaging="False" width="100%" autogeneratecolumns="False" borderwidth="0" gridlines="None" cellpadding="0" CellSpacing="0" ShowFooter="False" ShowHeader="False">
					<columns>
						<asp:boundcolumn visible="false" datafield="CategoryId"></asp:boundcolumn>
						<asp:templatecolumn itemstyle-cssclass="text">
							<itemtemplate>
								<asp:checkbox runat="server" id="chkItem" Text='<%# DataBinder.Eval(Container.DataItem, "CategoryName")%>' Checked='<%# SelectedCategories.Contains((int)DataBinder.Eval(Container.DataItem, "CategoryId"))%>'></asp:checkbox>
							</itemtemplate>
						</asp:templatecolumn>
					</columns>
				</asp:DataGrid>
			</div>
			<div align="right">
				<button id="btnAddGeneralCategory" runat="server" style="border:1px;padding:0;height:20px;width:22px;background-color:transparent" type="button"><IMG height="20" src="../layouts/images/icons/dictionary_edit.gif" width="22" border="0"></button>
			</div>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="center" >
			<br>
			<btn:imbutton class="text" id="btnSave" Runat="server" style="width:120"></btn:imbutton>
			<btn:imbutton class="text" id="btnCancel" Runat="server" IsDecline="true" CausesValidation="false" style="width:120;margin-left:20"></btn:imbutton>
		</td>
	</tr>
</table>
<asp:Button id="btnRefresh" CausesValidation="False" runat="server" style="visibility:hidden;"></asp:Button>