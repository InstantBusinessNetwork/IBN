<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.Finance" Codebehind="Finance.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px">
	<tr>
		<td>
			<ibn:BlockHeader id="secHeader" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
</table>
<table class="ibn-stylebox-light" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td id="TableCell" runat="server">
		</td>
	</tr>
</table>
