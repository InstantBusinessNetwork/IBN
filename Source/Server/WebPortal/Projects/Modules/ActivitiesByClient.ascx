<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ActivitiesByClient" Codebehind="ActivitiesByClient.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
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
<script type="text/javascript">
function CollapseExpand(CEType, ContactUid, OrgUid, evt)
{
	evt = (evt) ? evt : ((window.event) ? event : null);
	if (evt)
	{
		var s = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
		if (s.toString().toLowerCase().indexOf("http://")==0 ||
				s.toString().toLowerCase().indexOf("javascript:")==0) 
			return;
	}
	
	var empty = '<%=PrimaryKeyId.Empty.ToString()%>';
	if(ContactUid != empty)
	{
		document.forms[0].<%=hdnId.ClientID %>.value = ContactUid;
		document.forms[0].<%=hdnColType.ClientID %>.value = "contact";
	}
	else if(OrgUid != empty)
	{
		document.forms[0].<%=hdnId.ClientID %>.value = OrgUid;
		document.forms[0].<%=hdnColType.ClientID %>.value = "org";
	}
	else
	{
		document.forms[0].<%=hdnId.ClientID %>.value = empty;
		document.forms[0].<%=hdnColType.ClientID %>.value = "noclient";
	}
	document.forms[0].<%=hdnCollapseExpand.ClientID %>.value = CEType;
	<%=Page.ClientScript.GetPostBackEventReference(lbCollapseExpand,"") %>
}
</script>
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
					<td width="200px"><asp:DropDownList ID="ddPrjGroup" Runat="server" Width="190px" CssClass="text"></asp:DropDownList></td>
					<td class="text" width="100px" align="right"><%=LocRM.GetString("tPrjPhase")%>:&nbsp;</td>
					<td width="200px"><asp:DropDownList ID="ddPrjPhase" Runat="server" Width="170px" CssClass="text"></asp:DropDownList></td>
					<td></td>
				</tr>
				<tr height="45px">
					<td class="text" width="80px"><%=LocRM.GetString("Status")%>:&nbsp;</td>
					<td width="200px"><asp:DropDownList ID="ddStatus" Runat="server" Width="190px" CssClass="text"></asp:DropDownList></td>
					<td class="text" width="100px" align="right"><%=LocRM.GetString("Client")%>:&nbsp;</td>
					<td width="200px"><ibn:EntityDD id="ClientControl" ObjectTypes="Contact,Organization" runat="server" Width="170px"/></td>
					<td align="right" style="padding-right:20px">
						<asp:button id="btnApplyFilter" CssClass="text" Runat="server" Width="70px"></asp:button>&nbsp;
						<asp:button id="btnResetFilter" CssClass="text" Runat="server" CausesValidation="False" Width="70px"></asp:button>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="PADDING-TOP: 5px; PADDING-BOTTOM:5px" class="ibn-propertysheet" valign="top">
			<asp:DataGrid EnableViewState="true" id="dgProjects" runat="server" cellpadding="0" gridlines="Horizontal" CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%">
				<columns>
					<asp:BoundColumn Visible="False" DataField="ProjectId"></asp:BoundColumn>
					<asp:BoundColumn Visible="False" DataField="ContactUid"></asp:BoundColumn>
					<asp:BoundColumn Visible="False" DataField="OrgUid"></asp:BoundColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetTitle(
								(bool)Eval("IsClient"),
								(int)Eval("ProjectId"),
								PrimaryKeyId.Parse(Eval("ContactUid").ToString()),
								PrimaryKeyId.Parse(Eval("OrgUid").ToString()),
								Eval("ClientName").ToString(),
								(bool)Eval("IsCollapsed"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetStatus(
								(bool)DataBinder.Eval(Container.DataItem,"IsClient"),
								DataBinder.Eval(Container.DataItem,"StatusName").ToString())
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetOpenTasks(
								(bool)DataBinder.Eval(Container.DataItem,"IsClient"),
								(int)DataBinder.Eval(Container.DataItem,"OpenTasks"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetCompletedTasks(
								(bool)DataBinder.Eval(Container.DataItem,"IsClient"),
								(int)DataBinder.Eval(Container.DataItem,"CompletedTasks"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<HeaderStyle cssclass="ibn-vh2"></HeaderStyle>
						<ItemTemplate>
							<%# GetIssues(
								(bool)DataBinder.Eval(Container.DataItem,"IsClient"),
								(int)DataBinder.Eval(Container.DataItem,"Issues"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
			<span id="spanLbl" runat="server" style="padding-left:20px;">
				<asp:Label CssClass="ibn-alerttext" ID="lblNoItems" Runat="server"></asp:Label>
			</span>
		</td>
	</tr>
</table>
<input id="hdnId" type="hidden" runat="server" />
<input id="hdnColType" type="hidden" runat="server" />
<input id="hdnCollapseExpand" type="hidden" runat="server" />
<asp:LinkButton id="lbCollapseExpand" runat="server" Visible="False"></asp:LinkButton>