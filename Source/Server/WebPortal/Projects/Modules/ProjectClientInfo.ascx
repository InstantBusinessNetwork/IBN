<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectClientInfo" Codebehind="ProjectClientInfo.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellpadding="0" cellspacing="0" width="100%" style="margin-top:5px">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="3" width="100%" border="0">
				<tr>
					<td width="130px" align="right" style="padding-bottom:10"><b><%= LocRM.GetString("Client")%>:</b></td>
					<td class="ibn-value" style="padding-bottom:10">
						<asp:Label Runat="server" ID="lblClient"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>