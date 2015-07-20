<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.Unsubscribe" CodeBehind="Unsubscribe.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top: 5px;">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server">
			</ibn:blockheader>
		</td>
	</tr>
	<tr>
		<td style="padding: 10px" class="ibn-alternating ibn-navline">
			<table class="ibn-propertysheet" cellpadding="5" cellspacing="0" width="100%">
				<tr>
					<td width="120px">
						<b>
							<%=LocRM.GetString("Notification")%>:</b>
					</td>
					<td class="ibn-value">
						<asp:Label runat="server" ID="lblNotification"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label runat="server" ID="lblObjectType"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:Label runat="server" ID="lblObject"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr runat="server" id="trUnsubscribe">
		<td style="padding: 20px" align="center" class="text">
			<h5>
				<%=LocRM.GetString("IDoNotWant")%></h5>
			<table>
				<tr>
					<td>
						<asp:RadioButtonList runat="server" ID="rblType" CssClass="text">
						</asp:RadioButtonList>
					</td>
				</tr>
				<tr>
					<td>
						<br>
						<btn:imbutton class="text" id="btnSave" Runat="server" style="width: 110px;">
						</btn:imbutton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr runat="server" id="trResults">
		<td style="padding: 20px" align="left" class="text">
			<%=LocRM.GetString("UnsubscribeThanks")%>
			<br />
			<br />
			<table class="text">
				<tr>
					<td style="padding-left: 10px">
						<img alt="" src="../layouts/images/rect.gif" />
						<asp:HyperLink runat="server" ID="lnkNotification"></asp:HyperLink>
						<br />
						<br />
						<img alt="" src="../layouts/images/rect.gif" />
						<asp:HyperLink runat="server" ID="lnkNotificationForObject"></asp:HyperLink>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
