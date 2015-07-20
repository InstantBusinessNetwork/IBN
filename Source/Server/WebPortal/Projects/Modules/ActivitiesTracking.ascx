<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ActivitiesTracking" Codebehind="ActivitiesTracking.ascx.cs" %>
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
		<td class="ibn-navline ibn-alternating text" style="padding-top:8px" valign="top">
			<table border="0" cellpadding="4" cellspacing="0" width="100%">
				<tr>
					<td class="text" width="80px"><%=LocRM.GetString("tPrjGroup")%>:</td>
					<td width="200px"><asp:DropDownList ID="ddPrjGroup" Runat=server Width="190px" CssClass="text"></asp:DropDownList></td>
					<td class="text" width="100px" align=right><%=LocRM.GetString("tPrjPhase")%>:&nbsp;</td>
					<td width="200px"><asp:DropDownList ID="ddPrjPhase" Runat=server Width="170px" CssClass="text"></asp:DropDownList></td>
					<td></td>
				</tr>
				<tr height="45px">
					<td class="text" width="80px"><%=LocRM.GetString("Status")%>:&nbsp;</td>
					<td width="200px"><asp:DropDownList ID="ddStatus" Runat="server" Width="190px" CssClass="text"></asp:DropDownList></td>
					<td colspan="3" align="right" style="padding-right:20px">
						<asp:button id="btnApplyFilter" CssClass="text" Runat="server" Width="70px"></asp:button>&nbsp;
						<asp:button id="btnResetFilter" CssClass="text" Runat="server" CausesValidation="False" Width="70px"></asp:button>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<asp:PlaceHolder ID="phItems" Runat="server" />
		</td>
	</tr>
</table>