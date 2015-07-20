<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectStateInfo"
	CodeBehind="ProjectStateInfo.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<ibn:BlockHeader ID="secHeader" runat="server"></ibn:BlockHeader>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5"
	width="100%" border="0">
	<tr>
		<td width="120px" align="right">
			<b><%= LocRM.GetString("status")%>:</b>
		</td>
		<td class="ibn-value">
			<asp:Label runat="server" ID="lblStatus"></asp:Label>
		</td>
	</tr>
	<tr>
		<td align="right">
			<b><%= LocRM.GetString("tPrjPhase")%>:</b>
		</td>
		<td class="ibn-value">
			<asp:Label runat="server" ID="lblPhase"></asp:Label>
		</td>
	</tr>
	<tr>
		<td align="right">
			<b><%= LocRM.GetString("risk_level")%>:</b>
		</td>
		<td class="ibn-value">
			<asp:Label runat="server" ID="lblRiskLevel"></asp:Label>
		</td>
	</tr>
	<tr>
		<td align="right">
			<b><%= LocRM.GetString("Percent")%>:</b>
		</td>
		<td class="ibn-value">
			<asp:Label runat="server" ID="lblPercent"></asp:Label>%
		</td>
	</tr>
	<tr style="padding-bottom: 12px;" id="trPriority" runat="server">
		<td align="right">
			<b><%= LocRM.GetString("Priority")%>:</b>
		</td>
		<td class="ibn-value">
			<asp:Label runat="server" ID="lblPriority"></asp:Label>
		</td>
	</tr>
</table>
