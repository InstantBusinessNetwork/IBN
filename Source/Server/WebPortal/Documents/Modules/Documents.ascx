<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.Documents" Codebehind="Documents.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="DocumentList" src="DocumentList.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px">
	<tr>
		<td class="ms-toolbar">
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<ibn:DocumentList id="ucDocumentList" runat="server"/>
		</td>
	</tr>
</table>
