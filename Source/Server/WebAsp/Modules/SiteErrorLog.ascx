<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.SiteErrorLog" Codebehind="SiteErrorLog.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\Modules\BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secH" runat="server"></ibn:blockheader></td>
	</tr>
	<tr runat="server" id="trTable">
		<td>
			<asp:datagrid runat="server" id="dgErrors" enableviewstate="False" autogeneratecolumns="False" borderstyle="None" gridlines="Horizontal" borderwidth="0px" cellpadding="3" width="100%">
				<columns>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headertext="Error ID" datafield="ErrorID"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headertext="Creation Time" datafield="CreationTime" itemstyle-width="150" headerstyle-width="150"></asp:boundcolumn>
				</columns>
			</asp:datagrid>
		</td>
	</tr>
	<tr runat="server" id="trLabel">
		<td width="100%">
			<div style="Padding: 10px;">
				<asp:Label ID="lblError" Runat="server" EnableViewState="false"></asp:Label>
			</div>
		</td>
	</tr>
</table>
