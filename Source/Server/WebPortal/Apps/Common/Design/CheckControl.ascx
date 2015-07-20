<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CheckControl.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.CheckControl" %>
<table style="cursor: default;" cellpadding="0" cellspacing="0" runat="server" id="tblMain">
	<tr>
		<td><asp:Image runat="server" ID="curImg" /></td>
		<td><asp:Label ID="text" runat="server"></asp:Label></td>
	</tr>
</table>
<input id="curValue" type="hidden" runat="server" />