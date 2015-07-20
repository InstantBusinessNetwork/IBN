<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.Separator2" Codebehind="Separator2.ascx.cs" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" class="ibn-separatorheader" onmouseover="this.className='ibn-separatorheaderHover'" onmouseout="this.className='ibn-separatorheader'">
	<tr runat="server" id="trSeparator">
		<td width="16" valign="bottom">
			<asp:Image ID="imgPlusMinus" Runat="server" Height="16" Width="16" ImageAlign="Top"></asp:Image>
		</td>
		<td valign="bottom" style="padding-bottom:2">
			<asp:label id="lblTitle" Runat="server"></asp:label>
		</td>
	</tr>
</table>
<asp:Button ID="btnSubmit" Runat="server" style="display:none"></asp:Button>
