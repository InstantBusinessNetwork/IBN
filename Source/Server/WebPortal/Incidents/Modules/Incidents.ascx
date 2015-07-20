<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.Incidents" Codebehind="Incidents.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageViewMenu" src="..\..\Modules\PageViewMenu.ascx" %>
<script language=javascript>
	function ImportWizard()
	{
		var w = 600;
		var h = 400;
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
		var f = window.open('../wizards/ImportDataWizard.aspx?Type=Iss', "Wizard", winprops);
	}
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td class="ms-toolbar">
			<ibn:PageViewMenu id="secHeader" runat="server" title="ToolBar" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:PlaceHolder ID="phItems" Runat="server" />
		</td>
	</tr>
</table>
