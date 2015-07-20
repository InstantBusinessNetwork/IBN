<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="leftTemplate.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Shell.Modules.leftTemplate"%>
<asp:Repeater ID="TabItems" runat="server">
	<ItemTemplate>
		<div class="NavigationNavBarItem" onclick="leftTemplate_onNavMenuSelect(this, <%#GetIndex()%>)" onmouseover="leftTemplate_onMouseOver(this);" onmouseout="leftTemplate_onMouseOut(this);">
			<table cellpadding="0" cellspacing="1" width="100%">
				<tr>
					<td style="width: 25px; padding: 3px;"><asp:Image ID="Image2" runat="server" AlternateText=" " Width="16" Height="16" BorderWidth="0" ImageUrl='<%#Eval("ImageUrl")%>'/></td>
					<td><%#Eval("Title")%></td>
				</tr>
			</table>
		</div>
	</ItemTemplate>
</asp:Repeater>
