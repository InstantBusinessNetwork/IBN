<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Reports.Modules.StringFilter" Codebehind="StringFilter.ascx.cs" %>
<table cellspacing="0" cellpadding="2" border="0">
	<tr>
		<td runat=server id="Migrated_tdTitle" align="left" style="PADDING-TOP:10px" class="text">
			<b><asp:Label ID="lblTitle" Runat="server" CssClass="text"></asp:Label>:</b>&nbsp;&nbsp;&nbsp;
		</td>
		<td align="left" style="PADDING-TOP:10px">
			<asp:TextBox id="txtValue" runat="server" CssClass="text" Wrap="False" Width="150" MaxLength="512"></asp:TextBox>
		</td>
	</tr>
</table>