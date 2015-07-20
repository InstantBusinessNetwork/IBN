<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PM_ChartReport.ascx.cs" Inherits="Mediachase.UI.Web.Apps.ProjectManagement.Workspace.PM_ChartReport" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx"%>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>

<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>

<table class="ibn-navline ibn-alternating text" cellpadding="5" cellspacing="0" width="100%" border="0" style="padding-left: 10px;">
	<tr>
		<td style="padding-top: 5px; padding-bottom: 5px;">
			<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.Incident, _mc_GroupType %>" />:
			<%=this.groupBy %>
		</td>
	</tr>
</table>

<div style="vertical-align: middle;" align="center">
	<asp:Image id="imgGraph" BorderWidth="0" runat="server"></asp:Image>
</div>
