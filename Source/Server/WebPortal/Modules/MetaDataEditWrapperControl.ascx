<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.MetaDataEditWrapperControl" Codebehind="MetaDataEditWrapperControl.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataEditControl" src="MetaDataEditControl.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px">
	<tr>
		<td><ibn:blockheader id="tbMetaInfo" title="Custom Fields" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="PADDING-LEFT: 7px">
			<ibn:MetaDataEditControl id="ucMetaDataEditControl" runat="server"></ibn:MetaDataEditControl>
		<br>
		</td>
	</tr>
</table>
