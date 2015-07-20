<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectManagers" Codebehind="ProjectManagers.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
<table class="ibn-stylebox-light ibn-propertysheet ibn-value" cellspacing="0" cellpadding="5" width="100%" border="0">
	<tr>
		<td>
			<asp:Label Runat="server" ID="lblManager" />
		</td>
		<td width="200px">
			<%= LocRM.GetString("manager")%>
		</td>
	</tr>
	<tr style="padding-bottom:10;" runat="server" id="ExecutiveManagerRow">
		<td>
			<asp:Label Runat="server" ID="lblExecManager" />
		</td>
		<td>
			<%= LocRM.GetString("exec_manager")%>
		</td>
	</tr>
</table>