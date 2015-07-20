<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ResourcesTracking" Codebehind="ResourcesTracking.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<style type="text/css">
	.cellstyle {font-family: verdana;font-size: .68em;
				vertical-align: center;height:26px;border-bottom:1px #e4e4e4 solid}
	.cellstyle2 {font-family: verdana;font-size: .68em; background:#f2f2f2;
				vertical-align: center;height:23px;border-bottom:1px #e4e4e4 solid}
	.cellstyle3 {font-family: verdana;font-size: .68em; background:#eaeaea;
				vertical-align: center;height:23px;border-bottom:1px #e4e4e4 solid}
	.alt-tblstyle {height:100%; width:64px; 
			background:#f2f2f2; cellpadding:0; cellspacing:0;border:0px}
	.alt-tblstyle2 {height:100%; width:100%; 
			background:#f2f2f2; cellpadding:0; cellspacing:0;border:0px}
	.tbl-wstyle {height:100%; width:53px; 
			background:#ffffff; cellpadding:0; cellspacing:0;border:0px}
	.headstyle {padding-top:5px;padding-bottom:5px; border-bottom:1px #e4e4e4 solid}
	.headstyle2 {padding-top:5px;padding-bottom:5px}
</style>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="MARGIN-TOP:0px">
	<tr>
		<td class="ms-toolbar">
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:PlaceHolder ID="phItems" Runat="server" />
		</td>
	</tr>
</table>