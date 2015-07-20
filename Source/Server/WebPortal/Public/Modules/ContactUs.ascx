<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Public.Modules.ContactUs" Codebehind="ContactUs.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:BlockHeader ID="header" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td style="padding: 0 5px 0 5px">
			<p class="text">
				<%=LocRM.GetString("TopText")%>
			</p>
			<p id="message" runat="server" class="ibn-propertysheet" />
		</td>
	</tr>
</table>
<asp:Label ID="lblexp" runat="server" CssClass="ibn-alerttext" style="font-size: 30pt" Visible="false"></asp:Label>
