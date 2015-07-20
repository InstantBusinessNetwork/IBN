<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Reports.Modules.DictionaryFilter" Codebehind="DictionaryFilter.ascx.cs" %>
<table border="0" cellspacing="0" cellpadding="2">
	<tr valign="top">
		<td runat=server id="Migrated_tdTitle" align="left" class="text" valign="top" style="PADDING-TOP:0px">
			<b><asp:Label ID="lblTitle" Runat="server" CssClass="text"></asp:Label>:</b>&nbsp;
		</td>
	</tr>
	<tr>
		<td valign="top" style="PADDING-TOP:3px">
			<div style="border: inset 2px #eeeeee; height:120px; overflow-y: auto; background-color:White; width:400px;" runat="server" id="divBlock">
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
	</tr>
</table>