<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EditControls.DictionaryMultivalue" Codebehind="DictionaryMultivalue.ascx.cs" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<table cellpadding="0" cellspacing="0" border="0" width="330" class="ibn-propertysheet">
	<tr>
		<td valign="bottom" style="width:300px;">
			<div style="border: inset 2px #eeeeee; height:140px; overflow-y: auto; background-color:White; width:99%;" runat="server" id="divBlock">
				<asp:DataGrid id="grdMain" runat="server" allowsorting="False" 
					allowpaging="False" width="100%" autogeneratecolumns="False" 
					borderwidth="0" gridlines="None" cellpadding="0" 
					CellSpacing="4" ShowFooter="False" ShowHeader="False">
					<columns>
						<asp:boundcolumn visible="false" datafield="Id"></asp:boundcolumn>
						<asp:templatecolumn itemstyle-cssclass="text">
							<itemtemplate>
								<asp:checkbox runat="server" id="chkItem" Text='<%# Eval("Name")%>'></asp:checkbox>
							</itemtemplate>
						</asp:templatecolumn>
					</columns>
				</asp:DataGrid>
			</div>
		</td>
		<td width="30px" valign="top">
			&nbsp;
			<button id="btnEditItems" runat="server" style="border:0px;padding:0px;height:20px;width:22px;background-color:transparent" type="button" tabindex="-1"><IMG 
				height="20" title='<%=LocRM.GetString("EditDictionary") %>' src='<%=ResolveUrl("~/layouts/images/icons/dictionary_edit.gif")%>' width="22" border="0"></button>
			<br />
			&nbsp;
			<asp:CustomValidator runat="server" ID="vldCustom" ErrorMessage="*" Display="dynamic" OnServerValidate="vldCustom_ServerValidate"></asp:CustomValidator>
		</td>
	</tr>
</table>
