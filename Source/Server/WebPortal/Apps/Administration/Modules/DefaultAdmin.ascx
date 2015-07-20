<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultAdmin.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Administration.Modules.DefaultAdmin" %>
<table cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td valign="top"><img alt="" src='<%=ResolveClientUrl("~/layouts/images/listset.gif") %>' /></td>
		<td style="padding-left:5px">
			<table cellspacing="0" cellpadding="4" border="0" class="text" id="mainTable" runat="server">
				<tr>
					<td valign="top">
						<asp:Label ID="lblParentName" runat="server" Font-Bold="true"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>