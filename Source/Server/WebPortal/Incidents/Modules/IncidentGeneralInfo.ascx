<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.IncidentGeneralInfo" Codebehind="IncidentGeneralInfo.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellpadding="0" cellspacing="0" width="100%" style="margin-top:5px">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
</table>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
	<tr>
		<td valign="top" width="110px" align="right"><b><%=LocRM.GetString("Title")%>:</b></td>
		<td class="ibn-value" valign="top"><asp:label id="lblTitle" runat="server"></asp:label></td>
		<td valign="top" width="90px" align="right"><b><%=LocRM.GetString("Type")%>:</b></td>
		<td class="ibn-value" valign="top"><asp:label id="lblType" runat="server"></asp:label></td>
	</tr>
	<tr>
		<td valign="top" align="right"><b><%=LocRM.GetString("Severity")%>:</b></td>
		<td class="ibn-value" valign="top"><asp:label id="lblSeverity" runat="server"></asp:label></td>
		<td valign="top" align="right"><b><%=LocRM.GetString("Priority")%>:</b></td>
		<td class="ibn-value" valign="top"><asp:label id="lblPriority" runat="server"></asp:label></td>
	</tr>
	<tr>
		<td style="padding-bottom:10" valign="top" align="right"><b><%=LocRM.GetString("Description")%>:</b></td>
		<td colspan="3" Class="ibn-description" valign="top">
			<asp:label id="lblDescription" runat="server"></asp:label>
		</td>
	</tr>
</table>

