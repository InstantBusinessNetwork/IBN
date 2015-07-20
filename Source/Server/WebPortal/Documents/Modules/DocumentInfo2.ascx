<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.DocumentInfo2" Codebehind="DocumentInfo2.ascx.cs" %>
<table class="ibn-propertysheet tablePadding5" width="100%" border="0" cellpadding="0" cellspacing="0" style="margin-bottom:10;">
	<tr>
		<td class="ibn-label" style="padding-left:10" width="100"><%= LocRM.GetString("Title")%>:</td>
		<td class="ibn-value">
			<asp:Label Runat="server" ID="lblTitle"></asp:Label>
		</td>
		<td align="right">
			<asp:label id="lblState" runat="server" Font-Bold="True"></asp:label>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" style="padding-left:10"> 
			<nobr><%= LocRM.GetString("Manager")%>:</nobr>
		</td>
		<td class="ibn-legend-greyblack">
			<asp:label id="lblManager" runat="server"></asp:label>
		</td>
		<td align="right">
			<asp:label id="lblPriority" runat="server"></asp:label>
		</td>
	</tr>
	<tr>
		<td colspan="3" class="ibn-description" style="padding-left:10" align="left">
			<asp:label id="lblDescription" runat="server"></asp:label>
		</td>
	</tr>
</table>
