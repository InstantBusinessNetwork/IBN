<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Workspace.Modules.Assistant" Codebehind="Assistant.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<script type="text/javascript">
<!--
function ShowWizard(name,wdth,hght)
{
	var w = wdth;
	var h = hght;
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
	var f = window.open(name, "Wizard", winprops);
}
function OpenWindow(query,w,h,scroll)
{
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	
	winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
	if (scroll) winprops+=',scrollbars=1';
	var f = window.open(query, "_blank", winprops);
}
function ddMenuClick(obj)
{
	for(var i=0;i<obj.options.length;i++)
		if(obj.options[i].selected)
		{
			var str = obj.options[i].value;
			if (str=="NewProjectWizard")
				ShowWizard('<% =ResolveClientUrl("~/wizards/NewProjectWizard.aspx")%>',650,450);
			else if (str=="NewIssueWizard")
				ShowWizard('<% =ResolveClientUrl("~/wizards/NewIssueWizard.aspx")%>',650,450);
			else if (str=="NewEventWizard")
				ShowWizard('<% =ResolveClientUrl("~/wizards/NewToDoEntryWizard.aspx?Unit=Entry")%>',650,450);
			else if (str=="NewToDoWizard")
				ShowWizard('<% =ResolveClientUrl("~/wizards/NewToDoEntryWizard.aspx?Unit=ToDo")%>',650,450);
			else if (str=="NewUserWizard")
				ShowWizard('<% =ResolveClientUrl("~/wizards/NewUserWizard.aspx")%>',650,450);
			else if (str=="ADConvertWizard")
				ShowWizard('<% =ResolveClientUrl("~/wizards/ADConvertWizard.aspx")%>',700,500);
			else if (str=="IBNReport")
				OpenWindow('<% =ResolveClientUrl("~/Reports/XMLReport.aspx?Mode=Both")%>',750,466,true);
			else if (str=="FirstLoginWizard")
				OpenWindow('<% =ResolveClientUrl("~/wizards/FirstTimeLoginWizard.aspx")%>',650,550);
			else if (str=="FirstAdminLoginWizard")
				ShowWizard('<% =ResolveClientUrl("~/wizards/FirstTimeLoginAdminWizard.aspx")%>',650,450);
			else if (str=="ImportWizard")
				ShowWizard('<% =ResolveClientUrl("~/wizards/ImportDataWizard.aspx")%>',650,450);
			break;	
		}
	obj.options[0].selected=true;
}
//-->
</script>

<table class="ibn-stylebox ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%"
	border="0" style="margin: 0; border: none;">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" Title="" runat="server" Visible="false"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td style="padding-left: 10px">
			<table cellpadding="2" cellspacing="2" border="0" width="100%">
				<tr>
					<td class="text">
						<br />
						<asp:HyperLink ID="lHelpDocs" runat="server" CssClass="text"></asp:HyperLink>
						<asp:HyperLink ID="lDashboardView" runat="server" CssClass="text"></asp:HyperLink>
						<asp:Label ID="lblClient" runat="server" CssClass="text"></asp:Label>
						<asp:Label ID="lblCustReports" runat="server" CssClass="text"></asp:Label>
						<asp:HyperLink ID="lChangeProfile" runat="server" CssClass="text"></asp:HyperLink>
						<br />
						<b><%=LocRM.GetString("tWizards")%>:</b>&nbsp;&nbsp;&nbsp;
						<asp:DropDownList onchange="javascript:ddMenuClick(this);" Width="240px" ForeColor="#003399" runat="server" ID="ddMenu"></asp:DropDownList>
						<br />
						<br />
						<asp:Label ID="lblLastAlert" runat="server" CssClass="text" Visible="false"></asp:Label>
						<br />
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<asp:LinkButton ID="lbHide" runat="server" Visible="False"></asp:LinkButton>