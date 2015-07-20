<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.ProjectManagement.ColumnTemplates.CanManage_AddResources_Selected" %>
<div style="cursor:default;text-align:center;">
	<asp:CheckBox ID="cbItemCanManage" runat="server" Visible='<%# Eval("CanManage") != DBNull.Value %>' Checked='<%# Eval("CanManage") != DBNull.Value && (bool)Eval("CanManage")%>' />
</div>