<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Reference Page="~/Incidents/IncidentBoxView.aspx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.AdditionalInfo" Codebehind="AdditionalInfo.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellpadding="0" cellspacing="0" width="100%" style="margin-top:5px">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
</table>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
	<tr>
		<td valign="top" style="width:170px" align="right"><b><%=LocRM2.GetString("tIssBox")%>:</b></td>
		<td class="ibn-value" valign="top"><asp:Label Runat="server" ID="lblFolder" /></td>
	</tr>
	<tr>
		<td valign="top" align="right"><b><%=LocRM.GetString("tTicket")%>:</b></td>
		<td class="ibn-value" valign="top"><asp:Label Runat="server" ID="lblTicket" /></td>
	</tr>
	<tr>
		<td valign="top" align="right"><b><%=LocRM.GetString("Manager")%>:</b></td>
		<td class="ibn-value" valign="top"><asp:Label Runat="server" ID="lblManager" /></td>
	</tr>
	<tr id="trClient" runat="server">
		<td valign="top" align="right"><b><%=LocRM.GetString("Client")%>:</b></td>
		<td class="ibn-value" valign="top"><asp:Label Runat="server" ID="lblClient" /></td>
	</tr>
	<tr>
		<td valign="top" align="right"><b><%=LocRM.GetString("Created")%>:</b></td>
		<td class="ibn-value" valign="top">
			<asp:label id="lblCreator" runat="server"></asp:label>
			<asp:label id="lblCreationDate" runat="server"></asp:label>
		</td>
	</tr>
	<tr id="trGenCats" runat="server">
		<td valign="top" align="right"><b><%=LocRM.GetString("Category")%>:</b></td>
		<td class="ibn-value" valign="top"><asp:Label Runat="server" ID="lblGenCats" /></td>
	</tr>
	<tr id="trIssCats" runat="server">
		<td valign="top" align="right"><b><%=LocRM.GetString("IncidentCategory")%>:</b></td>
		<td class="ibn-value" valign="top"><asp:Label Runat="server" ID="lblIssCats" /></td>
	</tr>
	<tr id="trTaskTime" runat="server">
		<td valign="top" align="right"><b><%=LocRM3.GetString("taskTime") %>:</b></td>
		<td valign="top" class="ibn-value"><asp:label id="lblTaskTime" runat="server">OverallStatus</asp:label></td>
	</tr>
	<tr>
		<td valign="top" align="right"><b><asp:Label runat="server" ID="SpentTimeLabel"></asp:Label></b></td>
		<td valign="top" class="ibn-value"><asp:Label id="lblSpentTime" runat="server"></asp:Label></td>
	</tr>
</table>

