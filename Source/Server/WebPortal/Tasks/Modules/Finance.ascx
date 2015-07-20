<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.Finance" Codebehind="Finance.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
		</td>
	</tr>
</table>
<table class="ibn-stylebox-light" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td id="TableCell" runat="server">
		</td>
	</tr>
</table>
