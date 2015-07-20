<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.ErrorLog" Codebehind="ErrorLog.ascx.cs" %>
<asp:datagrid runat="server" id="dgErrors" enableviewstate="False" autogeneratecolumns="False" borderstyle="None" gridlines="Horizontal" borderwidth="0px" cellpadding="3" width="100%">
	<columns>
		<asp:boundcolumn headerstyle-cssclass="ms-vh2" itemstyle-cssclass="ms-vb2" headertext="Error ID" datafield="ErrorID"></asp:boundcolumn>
		<asp:boundcolumn headerstyle-cssclass="ms-vh2" itemstyle-cssclass="ms-vb2" headertext="Creation Time" datafield="CreationTime" itemstyle-width="150" headerstyle-width="150"></asp:boundcolumn>
	</columns>
</asp:datagrid>
