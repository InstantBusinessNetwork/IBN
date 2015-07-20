<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeparatorWhite.ascx.cs" Inherits="Mediachase.UI.Web.Modules.SeparatorWhite" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" class="ibn-separatorheaderwhite" onmouseover="this.className='ibn-separatorheaderwhiteHover'" onmouseout="this.className='ibn-separatorheaderwhite'">
	<tr runat="server" id="trSeparator">
		<td width="16" valign="bottom">
			<asp:Image ID="imgPlusMinus" Runat="server" Height="16" Width="16" ImageAlign="Top"></asp:Image>
		</td>
		<td valign="bottom" style="padding-bottom:2">
			<asp:label id="lblTitle" Runat="server"></asp:label>
		</td>
	</tr>
</table>
<asp:Button ID="btnSubmit" Runat="server" style="display:none" 
	onclick="btnSubmit_Click"></asp:Button>
