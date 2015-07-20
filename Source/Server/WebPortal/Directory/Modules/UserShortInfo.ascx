<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.UserShortInfo" Codebehind="UserShortInfo.ascx.cs" %>
<table cellspacing="0" cellpadding="5" width="100%" border="0" class="ibn-propertysheet" style="margin-bottom:5;">
	<tr>
		<td width="150" class="ibn-label" style="padding-left:10" ><%= LocRM.GetString("UserName")%>:</td>
		<td width="40%" class="ibn-value">
			<asp:label id="lblLastName" runat="server"></asp:label> <asp:label id="lblFirstName" runat="server"></asp:label>
		</td>
		<td rowspan="3" valign="top">
			<asp:label id="lblGroupList" Runat="server"></asp:label>
		</td>
		<td rowspan="3" valign="top" align="right">
			<asp:label id="lblUserActivity" Font-Bold="True" style="VISIBILITY: hidden; COLOR: red" runat="server" Height="18"></asp:label>
		</td>
	</tr>
	<tr>
		<td style="padding-left:10" class="ibn-label"><%= LocRM.GetString("EmailTitle")%>:</td>
		<td class="ibn-value"><asp:label id="lblEmail" runat="server"></asp:label></td>
	</tr>
	<tr runat="server" id="LoginRow">
		<td style="padding-left:10" class="ibn-label"><asp:Label ID="lblClientLogin" runat="server"></asp:Label>:</td>
		<td class="ibn-value"><asp:label id="IbnClientLoginLabel" runat="server"></asp:label></td>
	</tr>
</table>
