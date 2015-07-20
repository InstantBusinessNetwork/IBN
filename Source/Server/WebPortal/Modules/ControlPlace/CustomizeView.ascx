<%@ Control Language="c#" Inherits="ControlPlaceApplication.CustomizeView" Codebehind="CustomizeView.ascx.cs" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-toolbar-light">
	<tr>
		<td width="16" background="<%=Path_Img %>Layouts/Images/CustomizeView/left.gif" style="background-position: bottom; background-repeat: repeat-x;"><img src="<%=Path_Img %>Layouts/Images/spacer.gif" width="16" height="1"></td>
		<td noWrap style="padding-left:3px;padding-right:3px"><b><asp:Label ID="lbTitle" Runat="server" /></b></td>
		<td width="99%" background="<%=Path_Img %>Layouts/Images/CustomizeView/line.gif" style="background-position: bottom; background-repeat: repeat-x;"></td>
		<td noWrap id="tdDropMenu" runat="server"></td>
		<td width="16" background="<%=Path_Img %>Layouts/Images/CustomizeView/right.gif" style="background-position: bottom; background-repeat: repeat-x;"><img src="<%=Path_Img %>Layouts/Images/spacer.gif" width="16" height="1"></td>
	</tr>
</table>
<table cellspacing="0" cellpadding="0" width="100%" border=0 style="border-left:1px solid #95b7f3;border-right:1px solid #95b7f3;border-bottom:1px solid #95b7f3;">
	<tr>
		<td style="padding: 7px;" id="tdMainContent" runat="server"></td>
	</tr>
</table>
