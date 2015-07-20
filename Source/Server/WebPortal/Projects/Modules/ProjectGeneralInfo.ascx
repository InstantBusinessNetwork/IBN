<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectGeneralInfo" Codebehind="ProjectGeneralInfo.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
	<tr>
		<td>
			<span class="ibn-label"><%= LocRM.GetString("description")%>:</span><br />
			<asp:Label Runat="server" ID="lblDescription" CssClass="ibn-description"></asp:Label>
		</td>
	</tr>
	<tr runat="server" id="GoalsRow">
		<td>
			<span class="ibn-label"><%= LocRM.GetString("goals")%>:</span><br />
			<asp:Label Runat="server" ID="lblGoals" CssClass="ibn-description"></asp:Label>
		</td>
	</tr>
	<tr runat="server" id="ScopeRow">
		<td>
			<span class="ibn-label"><%= LocRM.GetString("scope")%>:</span><br />
			<asp:Label Runat="server" ID="lblScope" CssClass="ibn-description"></asp:Label>
		</td>
	</tr>
	<tr runat="server" id="DeliverablesRow">
		<td style="padding-bottom:10px;">
			<span class="ibn-label"><%= LocRM.GetString("deliverables")%>:</span><br />
			<asp:Label Runat="server" ID="lblDeliverables" CssClass="ibn-description"></asp:Label>
		</td>
	</tr>
</table>

