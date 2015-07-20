<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.ResourceWorks" CodeBehind="ResourceWorks.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="../../Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ResWorks" Src="../../Projects/Modules/WorksForResource.ascx" %>
<table cellspacing="0" cellpadding="0" class="ibn-stylebox text" style="width: 100%">
	<tr Printable="0">
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<ibn:ResWorks ID="ctrlRes" runat="server"></ibn:ResWorks>
		</td>
	</tr>
</table>
